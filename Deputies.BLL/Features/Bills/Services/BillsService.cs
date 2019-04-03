using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deputies.BLL.Features.Bills.Models;
using Deputies.BLL.Features.Deputies.Services;
using Deputies.BLL.Shared.Models;
using Deputies.Core.Entities;
using Deputies.Core.Interfaces;
using ParliamentaryInquiry.Core.Entities;

namespace Deputies.BLL.Features.Bills.Services
{
    public class BillsService : IBillsService
    {
        private const int RecordsPerPage = 10;

        private readonly IUnitOfWork unitOfWork;
        private readonly IDeputiesService deputyService;

        public BillsService(IUnitOfWork unitOfWork, IDeputiesService deputyService)
        {
            this.unitOfWork = unitOfWork;
            this.deputyService = deputyService;
        }

        public async Task<BillsListModel> FilterAsync(string query, int? page, string[] s, string[] r, string[] sor, bool asc = false)
        {
            var bills = await this.unitOfWork.GetRepository<Bill>().GetAll();
            bills = bills
                .Where(x => s == null || s.Any(y => y == x.SessionId))
                .Where(x => r == null || r.Any(y => y == x.RubricId))
                .Where(x => sor == null || sor.Any(y => y == x.SubjectOfRightId))
                .ToList();

            if (!string.IsNullOrEmpty(query))
            {
                bills = bills.Where(x => x.Title.ToLower().Contains(query.ToLower())).ToList();
            }

            bills = asc ? bills.OrderBy(x => x.Date).ToList() : bills.OrderByDescending(x => x.Date).ToList();
            var billsCount = bills.Count;
            var total = (int)Math.Ceiling((decimal)billsCount / (decimal)RecordsPerPage);
            bills = bills.Skip(RecordsPerPage * ((page ?? 1) - 1)).Take(RecordsPerPage).ToList();
            var filterModel = new BillFilterModel();
            filterModel.Rubrics = (await this.unitOfWork.GetRepository<Rubric>().GetAll()).Select(x => new CheckBoxModel()
            {
                Id = x.Id,
                Label = x.Name,
                Checked = r != null && r.Contains(x.Id)
            }).ToList();

            filterModel.Sessions = (await this.unitOfWork.GetRepository<Session>().GetAll()).Select(x => new CheckBoxModel()
            {
                Id = x.Id,
                Label = x.Number.ToString(),
                Checked = s != null && s.Contains(x.Id)
            }).ToList();

            filterModel.SubjectsOfRight = (await this.unitOfWork.GetRepository<SubjectOfRight>().GetAll()).Select(x => new CheckBoxModel()
            {
                Id = x.Id,
                Label = x.Name,
                Checked = sor != null && sor.Contains(x.Id)
            }).ToList();
            filterModel.Query = query;

            return new BillsListModel()
            {
                Bills = await this.BillsToModelsAsync(bills),
                Count = (int)billsCount,
                Asc = asc,
                PagingModel = new PagingModel()
                {
                    Current = page ?? 1,
                    Total = (int)Math.Ceiling((decimal)billsCount / (decimal)RecordsPerPage)
                },
                FilterModel = filterModel
            };
        }

        public async Task<BillModel> GetBillInfoAsync(string billId)
        {
            var item = await this.unitOfWork.GetRepository<Bill>().GetById(billId);
            var subjectOfRights = await this.unitOfWork.GetRepository<SubjectOfRight>().GetAll();
            var sessions = await this.unitOfWork.GetRepository<Session>().GetAll();
            var committees = await this.unitOfWork.GetRepository<Committee>().GetAll();
            var rubrics = await this.unitOfWork.GetRepository<Rubric>().GetAll();

            var bill = new BillModel();
            bill.Id = item.Id;
            bill.DateRaw = item.DateRaw;
            bill.Title = item.Title;
            bill.Number = item.Number;
            bill.Included = item.Included;
            bill.DocumentsRelatedToWork = item.DocumentsRelatedToWork;
            bill.Texts = item.Texts;
            bill.SubjectOfRight = subjectOfRights.FirstOrDefault(x => x.Id == item.SubjectOfRightId)?.Name;
            bill.Session = sessions.FirstOrDefault(x => x.Id == item.SessionId).Number;
            bill.MainCommittee = committees.FirstOrDefault(x => x.Id == item.MainCommitteeId)?.Name;
            bill.OtherCommittees = item.OtherCommitteesIds
                ?.Select(x => committees.FirstOrDefault(y => y.Id == x)?.Name)
                .ToList();
            bill.Rubric = rubrics.FirstOrDefault(x => x.Id == item.RubricId)?.Name;
            if (item.InitiatorsIds != null)
            {
                bill.Initiators = (await this.GetInitiators(item.InitiatorsIds)).ToList();
            }

            return bill;
        }

        public async Task<BillsListModel> GetBillsAsync(int? page, bool asc = false)
        {
            var billsCount = await this.unitOfWork.GetRepository<Bill>().Count();

            var bills = await this.unitOfWork
                .GetRepository<Bill>()
                .SearchFor(x => true, RecordsPerPage, RecordsPerPage * ((page ?? 1) - 1), inqury => inqury.Date, asc);

            var total = (int)Math.Ceiling((decimal)billsCount / (decimal)RecordsPerPage);

            var filterModel = new BillFilterModel();
            filterModel.Rubrics = (await this.unitOfWork.GetRepository<Rubric>().GetAll()).Select(x => new CheckBoxModel()
            {
                Id = x.Id,
                Label = x.Name
            }).ToList();

            filterModel.Sessions = (await this.unitOfWork.GetRepository<Session>().GetAll()).Select(x => new CheckBoxModel()
            {
                Id = x.Id,
                Label = x.Number.ToString()
            }).ToList();

            filterModel.SubjectsOfRight = (await this.unitOfWork.GetRepository<SubjectOfRight>().GetAll()).Select(x => new CheckBoxModel()
            {
                Id = x.Id,
                Label = x.Name
            }).ToList();
            filterModel.Query = "";

            return new BillsListModel()
            {
                Bills = await this.BillsToModelsAsync(bills),
                Count = (int)billsCount,
                Asc = asc,
                PagingModel = new PagingModel()
                {
                    Current = page ?? 1,
                    Total = (int)Math.Ceiling((decimal)billsCount / (decimal)RecordsPerPage)
                },
                FilterModel = filterModel
            };
        }

        public async Task<BillsListModel> GetBillsByDeputyAsync(string deputyId, int? page, bool asc = false)
        {
            var deputy = (await this.deputyService.GetAllDeputiesAsync()).FirstOrDefault(x => x.Id == deputyId);

            var bills = await this.unitOfWork
                .GetRepository<Bill>()
                .SearchFor(x => x.InitiatorsIds.Contains(deputyId), RecordsPerPage, RecordsPerPage * ((page ?? 1) - 1), inqury => inqury.Date, asc);

            var billsCount = await this.unitOfWork
                .GetRepository<Bill>().Count(x => x.InitiatorsIds.Contains(deputyId));

            var total = (int)Math.Ceiling((decimal)billsCount / (decimal)RecordsPerPage);

            var filterModel = new BillFilterModel();

            return new BillsListModel()
            {
                Bills = await this.BillsToModelsAsync(bills),
                Count = (int)billsCount,
                Asc = asc,
                PagingModel = new PagingModel()
                {
                    Current = page ?? 1,
                    Total = (int)Math.Ceiling((decimal)billsCount / (decimal)RecordsPerPage)
                },
                FilterModel = filterModel,
                DeputyName = deputy.Name,
                DeputyId = deputyId
            };
        }

        private async Task<IList<BillModel>> BillsToModelsAsync(IList<Bill> bills)
        {
            var subjectOfRights = await this.unitOfWork.GetRepository<SubjectOfRight>().GetAll();
            var sessions = await this.unitOfWork.GetRepository<Session>().GetAll();
            var committees = await this.unitOfWork.GetRepository<Committee>().GetAll();
            var rubrics = await this.unitOfWork.GetRepository<Rubric>().GetAll();

            var result = new List<BillModel>();
            foreach (var item in bills)
            {
                var bill = new BillModel();
                bill.Id = item.Id;
                bill.DateRaw = item.DateRaw;
                bill.Title = item.Title;
                bill.Number = item.Number;
                bill.Included = item.Included;
                bill.DocumentsRelatedToWork = item.DocumentsRelatedToWork;
                bill.Texts = item.Texts;
                bill.SubjectOfRight = subjectOfRights.FirstOrDefault(x => x.Id == item.SubjectOfRightId)?.Name;
                bill.Session = sessions.FirstOrDefault(x => x.Id == item.SessionId).Number;
                bill.MainCommittee = committees.FirstOrDefault(x => x.Id == item.MainCommitteeId)?.Name;
                bill.OtherCommittees = item.OtherCommitteesIds
                    ?.Select(x => committees.FirstOrDefault(y => y.Id == x)?.Name)
                    .ToList();
                bill.Rubric = rubrics.FirstOrDefault(x => x.Id == item.RubricId)?.Name;
                if (item.InitiatorsIds != null)
                {
                    bill.Initiators = (await this.GetInitiators(item.InitiatorsIds)).ToList();
                }

                result.Add(bill);
            }

            return result;
        }

        private async Task<IEnumerable<Initiator>> GetInitiators(IEnumerable<string> ids)
        {
            var allBillInitiators = await this.unitOfWork.GetRepository<BillInitiator>().GetAll();
            var allDeputies = await this.deputyService.GetAllDeputiesAsync();
            var result = new List<Initiator>();

            foreach (var id in ids)
            {
                Initiator initiator;
                var deputy = allDeputies.FirstOrDefault(x => x.Id == id);
                if (deputy != null)
                {
                    initiator = new Initiator
                    {
                        Name = deputy.Name,
                        Id = deputy.Id
                    };
                }
                else
                {
                    var initiatorName = allBillInitiators.FirstOrDefault(x => x.Id == id)?.Name;
                    if (initiatorName != null)
                    {
                        initiator = new Initiator()
                        {
                            Name = initiatorName
                        };
                    }
                    else
                    {
                        continue;
                    }

                }

                result.Add(initiator);
            }

            return result;
        }
    }
}
