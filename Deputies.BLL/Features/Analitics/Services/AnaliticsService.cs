using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deputies.BLL.Features.Analitics.Models;
using Deputies.BLL.Features.Deputies.Models;
using Deputies.BLL.Shared.Models;
using Deputies.Core.Entities;
using Deputies.Core.Interfaces;
using ParliamentaryInquiry.Core.Entities;

namespace Deputies.BLL.Features.Analitics.Services
{
    public class AnaliticsService : IAnaliticsService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRepository<Destination> destinationRepository;
        private readonly IRepository<Inqury> inquryRepository;
        private readonly IRepository<Problem> problemsRepository;
        private readonly IRepository<SingleMemberDeputy> singleDeputyRepository;
        private readonly IRepository<MultiMemberDeputy> multyDeputyRepository;
        private readonly IRepository<Session> sessionRepo;
        private readonly IRepository<DeputyAssociation> associationsRepo;
        private readonly IRepository<Constituency> constituencyRepo;
        private readonly IRepository<PoliticalParty> partyRepo;
        private readonly IRepository<AdministrativeUnit> administrativeUnitRepo;

        public AnaliticsService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.destinationRepository = this.unitOfWork.GetRepository<Destination>();
            this.inquryRepository = this.unitOfWork.GetRepository<Inqury>();
            this.problemsRepository = this.unitOfWork.GetRepository<Problem>();
            this.singleDeputyRepository = this.unitOfWork.GetRepository<SingleMemberDeputy>();
            this.multyDeputyRepository = this.unitOfWork.GetRepository<MultiMemberDeputy>();
            this.sessionRepo = this.unitOfWork.GetRepository<Session>();
            this.associationsRepo = this.unitOfWork.GetRepository<DeputyAssociation>();
            this.constituencyRepo = this.unitOfWork.GetRepository<Constituency>();
            this.partyRepo = this.unitOfWork.GetRepository<PoliticalParty>();
            this.administrativeUnitRepo = this.unitOfWork.GetRepository<AdministrativeUnit>();
        }

        public async Task<DeputyRating> GetDeputiesByAssociations(string deps = "", List<string> a = null, int page = 1)
        {
            if (a == null)
            {
                a = new List<string>();
            }

            const int depsPerPage = 10;

            var associations = await this.associationsRepo.GetAll();
            var inquries = await this.inquryRepository.GetAll();
            var models = new List<DeputyModel>();
            if (!a.Any())
            {
                if (deps == "all" || deps == "")
                {
                    deps = "all";
                    var singles = await this.singleDeputyRepository.SearchFor(x => x.IsActive);
                    var multies = await this.multyDeputyRepository.SearchFor(x => x.IsActive);
                    models.AddRange(singles.Select(x => this.SingleMemberToModel(x, associations)));
                    models.AddRange(multies.Select(x => this.PartyMemberToModel(x, associations)));
                }
                else if (deps == "single")
                {
                    var singles = await this.singleDeputyRepository.SearchFor(x => x.IsActive);
                    models.AddRange(singles.Select(x => this.SingleMemberToModel(x, associations)));
                }
                else if (deps == "party")
                {
                    var multies = await this.multyDeputyRepository.SearchFor(x => x.IsActive);
                    models.AddRange(multies.Select(x => this.PartyMemberToModel(x, associations)));
                }
            }
            else
            {
                if (deps == "all" || deps == "")
                {
                    deps = "all";
                    var singles = await this.singleDeputyRepository.SearchFor(x => x.IsActive && a.Contains(x.AssociationId));
                    var multies = await this.multyDeputyRepository.SearchFor(x => x.IsActive && a.Contains(x.AssociationId));
                    models.AddRange(singles.Select(x => this.SingleMemberToModel(x, associations)));
                    models.AddRange(multies.Select(x => this.PartyMemberToModel(x, associations)));
                }
                else if (deps == "single")
                {
                    var singles = await this.singleDeputyRepository.SearchFor(x => x.IsActive && a.Contains(x.AssociationId));
                    models.AddRange(singles.Select(x => this.SingleMemberToModel(x, associations)));
                }
                else if (deps == "party")
                {
                    var multies = await this.multyDeputyRepository.SearchFor(x => x.IsActive && a.Contains(x.AssociationId));
                    models.AddRange(multies.Select(x => this.PartyMemberToModel(x, associations)));
                }
            }



            models = models
                .OrderByDescending(x => inquries.Count(y => y.AuthorId == x.Id) + inquries.Count(y => y.CoauthorIds.Contains(x.Id)))
                .ToList();

            var fullCount = models.Count;

            models = models.Skip((page - 1) * depsPerPage)
                .Take(depsPerPage).ToList();

            var model = new DeputyRating()
            {
                PagingModel = new PagingModel()
                {
                    Current = page,
                    Total = (int)Math.Ceiling((decimal)fullCount / (decimal)depsPerPage)
                },
                Items = new List<DeputyRatingItem>(),
                Deps = deps,
                Associations = associations.Select(x => new CheckBoxModel()
                {
                    Checked = a.Contains(x.Id),
                    Id = x.Id,
                    Label = x.Name
                }).ToList()
            };

            foreach (var deputy in models)
            {
                model.Items.Add(new DeputyRatingItem()
                {
                    Deputy = deputy,
                    IndividualInquries = inquries.Count(y => y.AuthorId == deputy.Id),
                    CollectiveInquries = inquries.Count(y => y.CoauthorIds.Contains(deputy.Id))
                });
            }

            return model;
        }

        public async Task<DeputyAnalytics> GetDeputyAnaliticsAsync(string deputyId = null)
        {
            var result = new DeputyAnalytics();
            var allSessions = await this.sessionRepo.GetAll();
            var inquries = await this.inquryRepository.GetAll();
            var problems = await this.problemsRepository.GetAll();
            var associations = await this.associationsRepo.GetAll();
            #region Sessions
            var sessionsResponse = new ChartResponse();
            sessionsResponse.Data.Add(new DataSet() { Title = "Iндивiдуальнi" });
            sessionsResponse.Data.Add(new DataSet() { Title = "Колективні" });


            foreach (var session in allSessions.OrderBy(x => x.Number))
            {
                sessionsResponse.Labels.Add("Сесiя " + session.Number);
                sessionsResponse.Data[0].Data.Add((inquries.Count(x => (x.AuthorId == deputyId || x.CoauthorIds.Contains(deputyId)) && x.SessionId == session.Id && !x.CoauthorIds.Any())));
                sessionsResponse.Data[1].Data.Add((inquries.Count(x => (x.AuthorId == deputyId || x.CoauthorIds.Contains(deputyId)) && x.SessionId == session.Id && x.CoauthorIds.Any())));
            }

            sessionsResponse.Labels.Add("Усього");
            sessionsResponse.Data[0].Data.Add((inquries.Count(x => (x.AuthorId == deputyId || x.CoauthorIds.Contains(deputyId)) && !x.CoauthorIds.Any())));
            sessionsResponse.Data[1].Data.Add((inquries.Count(x => (x.AuthorId == deputyId || x.CoauthorIds.Contains(deputyId)) && x.CoauthorIds.Any())));

            result.Sessions = sessionsResponse;
            #endregion


            // from all
            List<string> depsIds1 = new List<string>();
            var md1 = (await this.multyDeputyRepository.SearchFor(x => x.IsActive)).Select(x => x.Id);
            var sd1 = (await this.singleDeputyRepository.SearchFor(x => x.IsActive)).Select(x => x.Id);
            depsIds1.AddRange(sd1.Union(md1));
            result.FromAll = await this.DuputiesRatingChart(depsIds1, deputyId, inquries);

            // from association
            var singledep = await this.singleDeputyRepository.GetById(deputyId);
            var multydep = await this.multyDeputyRepository.GetById(deputyId);
            var fractionId = singledep?.AssociationId ?? multydep.AssociationId;
            List<string> depsIds2 = new List<string>();
            var md2 = (await this.multyDeputyRepository.SearchFor(x => x.AssociationId == fractionId && x.IsActive)).Select(x => x.Id);
            var sd2 = (await this.singleDeputyRepository.SearchFor(x => x.AssociationId == fractionId && x.IsActive)).Select(x => x.Id);
            depsIds2.AddRange(sd2.Union(md2));
            result.FromAssociation = await this.DuputiesRatingChart(depsIds2, deputyId, inquries);

            // from association
            var singledep1 = await this.singleDeputyRepository.GetById(deputyId);
            var multydep1 = await this.multyDeputyRepository.GetById(deputyId);
            if (singledep != null)
            {
                result.IsSingle = true;
                var deps = await this.singleDeputyRepository.SearchFor(x => x.IsActive);
                result.FromSameSelected = await this.DuputiesRatingChart(deps.Select(x => x.Id).ToList(), deputyId, inquries);
            }
            else
            {
                result.IsSingle = false;
                var deps = await this.multyDeputyRepository.SearchFor(x => x.IsActive);
                result.FromSameSelected = await this.DuputiesRatingChart(deps.Select(x => x.Id).ToList(), deputyId, inquries);
            }

            result.Problems = inquries.Where(x => x.AuthorId == deputyId || x.CoauthorIds.Contains(deputyId)).GroupBy(x => x.ProblemId).Where(x => x != null && x.Key != null).Select(x => new ProblemsResponseItem()
            {
                Name = problems.First(y => y.Id == x.Key).Name,
                Count = x.Count()
            }).ToList();

            result.BillsBySessions = await this.GetBillsBySessionsData(deputyId);

            var bills = await this.unitOfWork.GetRepository<Bill>().SearchFor(x => x.InitiatorsIds.Contains(deputyId));
            var rubrics = await this.unitOfWork.GetRepository<Rubric>().GetAll();
            result.Rubrics = bills.GroupBy(x => x.RubricId).Where(x => x != null && x.Key != null).Select(x => new ProblemsResponseItem()
            {
                Name = rubrics.First(y => y.Id == x.Key).Name,
                Count = x.Count()
            }).ToList();

            return result;
        }

        public async Task<ChartResponse> GetInquriesBySessionsData(string deputyId = null)
        {
            var chartResponse = new ChartResponse();
            chartResponse.Data.Add(new DataSet() { Title = "Iндивiдуальнi" });
            chartResponse.Data.Add(new DataSet() { Title = "Колективні" });
            var allSessions = await this.sessionRepo.GetAll();

            if (deputyId == null)
            {
                foreach (var session in allSessions.OrderBy(x => x.Number))
                {
                    chartResponse.Labels.Add("Сесiя " + session.Number);
                    chartResponse.Data[0].Data.Add((await this.inquryRepository.Count(x => x.SessionId == session.Id && !x.CoauthorIds.Any())));
                    chartResponse.Data[1].Data.Add((await this.inquryRepository.Count(x => x.SessionId == session.Id && x.CoauthorIds.Any())));
                }

                chartResponse.Labels.Add("Усього");
                chartResponse.Data[0].Data.Add((await this.inquryRepository.Count(x => !x.CoauthorIds.Any())));
                chartResponse.Data[1].Data.Add((await this.inquryRepository.Count(x => x.CoauthorIds.Any())));
            }
            else
            {
                foreach (var session in allSessions.OrderBy(x => x.Number))
                {
                    chartResponse.Labels.Add("Сесiя " + session.Number);
                    chartResponse.Data[0].Data.Add((await this.inquryRepository.Count(x => (x.AuthorId == deputyId || x.CoauthorIds.Contains(deputyId)) && x.SessionId == session.Id && !x.CoauthorIds.Any())));
                    chartResponse.Data[1].Data.Add((await this.inquryRepository.Count(x => (x.AuthorId == deputyId || x.CoauthorIds.Contains(deputyId)) && x.SessionId == session.Id && x.CoauthorIds.Any())));
                }

                chartResponse.Labels.Add("Усього");
                chartResponse.Data[0].Data.Add((await this.inquryRepository.Count(x => (x.AuthorId == deputyId || x.CoauthorIds.Contains(deputyId)) && !x.CoauthorIds.Any())));
                chartResponse.Data[1].Data.Add((await this.inquryRepository.Count(x => (x.AuthorId == deputyId || x.CoauthorIds.Contains(deputyId)) && x.CoauthorIds.Any())));
            }

            return chartResponse;
        }

        public async Task<ChartResponse> GetBillsBySessionsData(string deputyId = null)
        {
            var subjectOfRights = (await this.unitOfWork.GetRepository<SubjectOfRight>().GetAll()).ToList();
            var bills = (await this.unitOfWork.GetRepository<Bill>().GetAll()).ToList();

            var chartResponse = new ChartResponse();
            foreach (var item in subjectOfRights)
            {
                chartResponse.Data.Add(new DataSet() { Title = item.Name });
            }

            var allSessions = await this.sessionRepo.GetAll();

            if (deputyId == null)
            {
                foreach (var session in allSessions.OrderBy(x => x.Number))
                {
                    chartResponse.Labels.Add("Сесiя " + session.Number);
                    for (int i = 0; i < subjectOfRights.Count; i++)
                    {
                        chartResponse.Data[i].Data.Add(bills.Count(x => x.SessionId == session.Id && x.SubjectOfRightId == subjectOfRights[i].Id));
                    }
                }

                chartResponse.Labels.Add("Усього");
                for (int i = 0; i < subjectOfRights.Count; i++)
                {
                    chartResponse.Data[i].Data.Add(bills.Count(x => x.SubjectOfRightId == subjectOfRights[i].Id));
                }
            }
            else
            {
                foreach (var session in allSessions.OrderBy(x => x.Number))
                {
                    chartResponse.Labels.Add("Сесiя " + session.Number);
                    for (int i = 0; i < subjectOfRights.Count; i++)
                    {
                        chartResponse.Data[i].Data.Add(bills.Count(x => x.InitiatorsIds.Contains(deputyId) && x.SessionId == session.Id && x.SubjectOfRightId == subjectOfRights[i].Id));
                    }
                }

                chartResponse.Labels.Add("Усього");
                for (int i = 0; i < subjectOfRights.Count; i++)
                {
                    chartResponse.Data[i].Data.Add(bills.Count(x => x.InitiatorsIds.Contains(deputyId) && x.SubjectOfRightId == subjectOfRights[i].Id));
                }
            }

            return chartResponse;
        }

        public async Task<OrganizationsResponse> GetOrganizationsAnalitics(int page = 1)
        {
            var inquries = await this.inquryRepository.GetAll();
            var organizations = await this.destinationRepository.GetAll();

            var group = inquries.GroupBy(x => organizations.FirstOrDefault(y => y.Id == x.DestinationId)?.Name).Select(x => new OrganizationsResponseItem()
            {
                Count = x.Count(),
                Name = x.Key
            }).ToList();

            var total = group.Count;

            return new OrganizationsResponse()
            {
                Items = group.OrderByDescending(x => x.Count).Skip((page - 1) * 20)
                .Take(20).ToList(),
                PagingModel = new PagingModel()
                {
                    Total = (int)Math.Ceiling((decimal)total / (decimal)20),
                    Current = page
                }
            };
        }

        public async Task<IEnumerable<ProblemsResponseItem>> GetProblemsAnalitics(List<string> a = null)
        {
            IEnumerable<Inqury> inquries;

            if (a == null || !a.Any())
            {
                inquries = await this.inquryRepository.GetAll();
            }
            else
            {
                var singles = (await this.singleDeputyRepository.SearchFor(x => a.Contains(x.AssociationId))).Select(x => x.Id);
                var multies = (await this.multyDeputyRepository.SearchFor(x => a.Contains(x.AssociationId))).Select(x => x.Id);

                var allDeputiesIds = singles.Union(multies);
                inquries = await this.inquryRepository.SearchFor(x => allDeputiesIds.Contains(x.AuthorId));
            }


            var problems = await this.problemsRepository.GetAll();

            var inquriesCount = inquries.Count();

            return inquries.GroupBy(x => x.ProblemId).Where(x => x != null && x.Key != null).Select(x => new ProblemsResponseItem()
            {
                Name = problems.First(y => y.Id == x.Key).Name,
                Count = x.Count(),
                Percent = (x.Count() / inquriesCount) * 100
            }).ToList();
        }

        private async Task<ChartResponse> DuputiesRatingChart(List<string> ids, string deputyId, IEnumerable<Inqury> inquries)
        {
            var maxInquriesAll = inquries.Where(x => ids.Contains(x.AuthorId) || ids.Any(y => x.CoauthorIds.Contains(y))).GroupBy(x => x.AuthorId).Max(x => x.Count() + inquries.Count(y => y.CoauthorIds.Contains(x.Key)));
            var avgInquriesAll = Math.Round(inquries.Where(x => ids.Contains(x.AuthorId) || ids.Any(y => x.CoauthorIds.Contains(y))).GroupBy(x => x.AuthorId).Average(x => x.Count() + inquries.Count(y => y.CoauthorIds.Contains(x.Key))), 2);
            var singles = await this.singleDeputyRepository.SearchFor(x => x.IsActive);
            var multies = await this.multyDeputyRepository.SearchFor(x => x.IsActive);
            var rating = ids
                .OrderByDescending(x => inquries.Count(y => y.AuthorId == x) + inquries.Count(y => y.CoauthorIds.Contains(x)))
                .ToList();

            var placeAll = rating.FindIndex(x => x == deputyId) + 1;
            var countAll = inquries.Count(y => y.AuthorId == deputyId) + inquries.Count(y => y.CoauthorIds.Contains(deputyId));
            var response = new ChartResponse();
            response.Labels.Add("Кількість запитів лідера рейтингу");
            response.Labels.Add("Запитiв депутата");
            response.Labels.Add("Середня кiлькicть запитiв");
            response.Data.Add(new DataSet());
            response.Data.Add(new DataSet());
            response.Data.Add(new DataSet());
            response.Data[0].Data.Add(maxInquriesAll);
            response.Data[1].Data.Add(countAll);
            response.Data[2].Data.Add(avgInquriesAll);
            response.AditionalData = placeAll;
            response.AditionalData1 = ids.Count; ;

            return response;
        }

        private DeputyModel SingleMemberToModel(SingleMemberDeputy singleDeputy, IEnumerable<DeputyAssociation> associations)
        {
            var deputyModel = new DeputyModel();

            deputyModel.Id = singleDeputy.Id;
            deputyModel.Name = singleDeputy.Name;
            deputyModel.Email = singleDeputy.Email;
            deputyModel.IsActive = singleDeputy.IsActive;
            deputyModel.Link = singleDeputy.Link;
            deputyModel.PhotoLink = singleDeputy.PhotoLink;
            deputyModel.Position = singleDeputy.Position;
            deputyModel.AssociationId = singleDeputy.AssociationId;
            var association = associations.First(x => x.Id == singleDeputy.AssociationId);
            deputyModel.Association = association.Name;

            return deputyModel;
        }

        private DeputyModel PartyMemberToModel(MultiMemberDeputy multyDeputy, IEnumerable<DeputyAssociation> associations)
        {
            var deputyModel = new DeputyModel();

            deputyModel.Id = multyDeputy.Id;
            deputyModel.Name = multyDeputy.Name;
            deputyModel.Email = multyDeputy.Email;
            deputyModel.IsActive = multyDeputy.IsActive;
            deputyModel.Link = multyDeputy.Link;
            deputyModel.PhotoLink = multyDeputy.PhotoLink;
            deputyModel.Position = multyDeputy.Position;
            deputyModel.AssociationId = multyDeputy.AssociationId;
            var association = associations.First(x => x.Id == multyDeputy.AssociationId);
            deputyModel.Association = association.Name;
            return deputyModel;
        }

        public async Task<AssociationsResponse> GetAssociationsRating()
        {
            var associations = await this.associationsRepo.GetAll();
            var singles = await this.singleDeputyRepository.GetAll();
            var multies = await this.multyDeputyRepository.GetAll();
            var inquries = await this.inquryRepository.GetAll();

            var multiesModel = multies.Select(x => this.PartyMemberToModel(x, associations));
            var singleModel = singles.Select(x => this.SingleMemberToModel(x, associations));

            var deputyModels = multiesModel.Union(singleModel).ToList();

            for (var i = 0; i < deputyModels.Count(); i++)
            {
                deputyModels[i].InquriesCount = inquries.Count(x => x.AuthorId == deputyModels[i].Id || x.CoauthorIds.Contains(deputyModels[i].Id));
            }

            deputyModels = deputyModels.OrderByDescending(x => x.InquriesCount).ToList();


            var response = new AssociationsResponse();
            foreach (var association in associations)
            {
                var item = new AssociationItem();
                item.Name = association.Name;
                item.Id = association.Id;
                var deputiesIds = singles.Where(x => x.AssociationId == association.Id).Select(x => x.Id).Union(multies.Where(x => x.AssociationId == association.Id).Select(x => x.Id));
                item.Deputies = deputiesIds.Count();
                item.Inquries = inquries.Count(x => deputiesIds.Contains(x.AuthorId));
                item.InquriesPerDeputy = Math.Round(((double)item.Inquries / (double)item.Deputies), 2);
                item.TopDeputies = deputyModels.Where(x => x.AssociationId == association.Id).Take(3).ToList();
                response.Associations.Add(item);
            }
            response.MaxDeputies = response.Associations.Max(x => x.Deputies);
            response.MaxInquries = response.Associations.Max(x => x.Inquries);
            response.MaxInquriesPerDeputy = response.Associations.Max(x => x.InquriesPerDeputy);
            response.Associations = response.Associations.OrderByDescending(x => x.InquriesPerDeputy).ToList();
            for (int i = 0; i < response.Associations.Count; i++)
            {
                response.Associations[i].Place = i + 1;
            }

            return response;
        }

        public async Task<IEnumerable<ProblemsResponseItem>> GetRubricsAnalitics(List<string> associations = null)
        {
            IEnumerable<Bill> bills;

            if (associations == null || !associations.Any())
            {
                bills = await this.unitOfWork.GetRepository<Bill>().GetAll();
            }
            else
            {
                var singles = (await this.singleDeputyRepository.SearchFor(x => associations.Contains(x.AssociationId))).Select(x => x.Id);
                var multies = (await this.multyDeputyRepository.SearchFor(x => associations.Contains(x.AssociationId))).Select(x => x.Id);

                var allDeputiesIds = singles.Union(multies);
                bills = await this.unitOfWork.GetRepository<Bill>().SearchFor(x => allDeputiesIds.Any(y => x.InitiatorsIds.Contains(y)));
            }

            var rubrics = await this.unitOfWork.GetRepository<Rubric>().GetAll();

            var rubricsCount = rubrics.Count();

            return bills.GroupBy(x => x.RubricId).Where(x => x != null && x.Key != null).Select(x => new ProblemsResponseItem()
            {
                Name = rubrics.First(y => y.Id == x.Key).Name,
                Count = x.Count(),
                Percent = (x.Count() / rubricsCount) * 100
            }).ToList();
        }
    }
}
