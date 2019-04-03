using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Deputies.BLL.Features.Deputies.Models;
using Deputies.BLL.Features.Deputies.Services;
using Deputies.BLL.Features.Inquiries.Models;
using Deputies.BLL.Shared.Models;
using Deputies.Core.Interfaces;
using ParliamentaryInquiry.Core.Entities;

namespace Deputies.BLL.Features.Inquiries.Services
{
    public class InquiriesService : IInquryService
    {
        private const int RecordsPerPage = 10;

        private readonly IUnitOfWork unitOfWork;
        private readonly IDeputiesService deputiesService;
        private readonly IMapper mapper;

        public InquiriesService(IUnitOfWork unitOfWork, IDeputiesService deputiesService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.deputiesService = deputiesService;
            this.mapper = mapper;
        }

        public async Task<InquriesListModel> FilterAsync(string query, int? page, string[] fractionsIds, string[] sessionsIds, string[] problemsIds, bool includeDone = false, bool includeInprocess = false, bool includeIndividual = false, bool includeCollective = false, bool asc = false)
        {
            var allDeputies = await this.deputiesService.GetAllDeputiesAsync();
            var allProblems = await this.unitOfWork.GetRepository<Problem>().GetAll();
            IEnumerable<string> deputiesIds;
            if (fractionsIds != null)
            {
                deputiesIds = allDeputies.Where(x => fractionsIds.Contains(x.AssociationId)).Select(x => x.Id);
            }
            else
            {
                deputiesIds = allDeputies.Select(x => x.Id);
            }

            var inquiries = await this.unitOfWork.GetRepository<Inqury>().GetAll();

            inquiries = asc ? inquiries.OrderBy(x => x.Date).ToList() : inquiries.OrderByDescending(x => x.Date).ToList();

            var inquriesfiltered = inquiries.Where(x => deputiesIds.Contains(x.AuthorId)).ToList();
            inquriesfiltered = inquriesfiltered.Where(x => sessionsIds == null || sessionsIds.Contains(x.SessionId)).ToList();
            inquriesfiltered = inquriesfiltered.Where(x => problemsIds == null || problemsIds.Contains(x.ProblemId)).ToList();
            if (!includeDone || !includeInprocess)
            {
                if (includeDone)
                {
                    inquriesfiltered = inquriesfiltered.Where(x => x.InquryAnswers.Any()).ToList();
                }
                else if (includeInprocess)
                {
                    inquriesfiltered = inquriesfiltered.Where(x => !x.InquryAnswers.Any()).ToList();
                }
            }

            if (!includeCollective || !includeIndividual)
            {
                if (includeCollective)
                {
                    inquriesfiltered = inquriesfiltered.Where(x => x.CoauthorIds.Any()).ToList();
                }
                else if (includeIndividual)
                {
                    inquriesfiltered = inquriesfiltered.Where(x => !x.CoauthorIds.Any()).ToList();
                }
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                var allDestinations = await this.unitOfWork.GetRepository<Destination>().GetAll();
                inquriesfiltered = inquriesfiltered.Where(x => !string.IsNullOrWhiteSpace(x.Body) && (x.Body.ToLower().Contains(query.ToLower())) || (allProblems.FirstOrDefault(pr => pr.Id == x.ProblemId)?.Name ?? "").ToLower().Contains(query.ToLower()) || (allDestinations.FirstOrDefault(pr => pr.Id == x.DestinationId)?.NameRaw ?? "").ToLower().Contains(query.ToLower()) || this.QueryOnAuthor(query, x, allDeputies)).ToList();
            }

            var inquriesCount = inquriesfiltered.Count();
            inquriesfiltered = inquriesfiltered.Skip(RecordsPerPage * ((page ?? 1) - 1)).Take(RecordsPerPage).ToList();

            var filterModel = new InquiryFilterModel();
            filterModel.Fractions = (await this.unitOfWork.GetRepository<DeputyAssociation>().GetAll()).Select(x => new CheckBoxModel()
            {
                Id = x.Id,
                Label = x.Name,
                Checked = fractionsIds != null && fractionsIds.Contains(x.Id)
            }).ToList();

            filterModel.Sessions = (await this.unitOfWork.GetRepository<Session>().GetAll()).Select(x => new CheckBoxModel()
            {
                Id = x.Id,
                Label = x.Number.ToString(),
                Checked = sessionsIds != null && sessionsIds.Contains(x.Id)
            }).ToList();

            filterModel.Problems = (allProblems).Select(x => new CheckBoxModel()
            {
                Id = x.Id,
                Label = x.Name,
                Checked = problemsIds != null && problemsIds.Contains(x.Id)
            }).ToList();
            filterModel.Col = includeCollective;
            filterModel.Done = includeDone;
            filterModel.Ind = includeIndividual;
            filterModel.Proc = includeInprocess;
            filterModel.Query = query;

            return new InquriesListModel()
            {
                Inquries = await this.InquriesToModelsAsync(inquriesfiltered.ToList()),
                Count = inquriesCount,
                Asc = asc,
                PagingModel = new PagingModel()
                {
                    Current = page ?? 1,
                    Total = (int)Math.Ceiling((decimal)inquriesCount / (decimal)RecordsPerPage)
                },
                FilterModel = filterModel
            };
        }

        private bool QueryOnAuthor(string query, Inqury inqury, IEnumerable<Deputy> deputies)
        {
            return deputies.Any(x => x.Id == inqury.AuthorId && x.Name.ToLower().Contains(query.ToLower()));
        }

        private async Task<IList<InquiryModel>> InquriesToModelsAsync(IList<Inqury> inquries)
        {
            var authorsIds = inquries.Select(x => x.AuthorId).ToList();
            authorsIds.AddRange(inquries.SelectMany(x => x.CoauthorIds));

            var problemsIds = inquries.Select(x => x.ProblemId);
            var destinationIds = inquries.Select(x => x.DestinationId);

            var problems = await this.unitOfWork.GetRepository<Problem>().SearchFor(x => problemsIds.Contains(x.Id));
            var destinations = await this.unitOfWork.GetRepository<Destination>().SearchFor(x => destinationIds.Contains(x.Id));
            var sessions = await this.unitOfWork.GetRepository<Session>().GetAll();
            var deputies = await this.deputiesService.GetAllDeputiesAsync();

            var inquriesModels = new List<InquiryModel>();

            foreach (var inqury in inquries)
            {
                var inquryModel = new InquiryModel();
                inquryModel.Problem = problems.FirstOrDefault(x => x.Id == inqury.ProblemId)?.Name;

                var deputyName = deputies.FirstOrDefault(x => x.Id == inqury.AuthorId)?.Name;

                inquryModel.Id = inqury.Id;
                inquryModel.Author = deputyName;
                inquryModel.AuthorId = inqury.AuthorId;
                inquryModel.Destination = destinations.FirstOrDefault(x => x.Id == inqury.DestinationId)?.Full;
                inquryModel.Body = inqury.Body;
                inquryModel.BodyUrl = inqury.BodyUrl;
                inquryModel.DateRaw = inqury.DateRaw;
                inquryModel.DeadlineRaw = inqury.DeadlineRaw;
                inquryModel.RequestNumber = inqury.RequestNumber;
                inquryModel.CoauthorIds = inqury.CoauthorIds;

                foreach (var coauthorId in inquryModel.CoauthorIds)
                {
                    inquryModel.Coauthors.Add(new DeputyModel()
                    {
                        Id = coauthorId,
                        Name = deputies.FirstOrDefault(x => x.Id == coauthorId)?.Name
                    });
                }

                inquryModel.InquryAnswers = this.mapper.Map<List<InquiryAnswerModel>>(inqury.InquryAnswers);
                inquryModel.Session = sessions.First(x => x.Id == inqury.SessionId).Number.ToString();

                inquriesModels.Add(inquryModel);
            }

            return inquriesModels;
        }

        public async Task<InquriesListModel> GetInquiriesAsync(int? page, bool asc = false)
        {
            var inquriesCount = await this.unitOfWork.GetRepository<Inqury>().Count();

            var inquries = await this.unitOfWork.GetRepository<Inqury>().SearchFor(x => true, RecordsPerPage, RecordsPerPage * ((page ?? 1) - 1),
inqury => inqury.Date, asc);

            var filterModel = new InquiryFilterModel();
            filterModel.Fractions = (await this.unitOfWork.GetRepository<DeputyAssociation>().GetAll()).Select(x => new CheckBoxModel()
            {
                Id = x.Id,
                Label = x.Name
            }).ToList();

            filterModel.Sessions = (await this.unitOfWork.GetRepository<Session>().GetAll()).Select(x => new CheckBoxModel()
            {
                Id = x.Id,
                Label = x.Number.ToString()
            }).ToList();

            filterModel.Problems = (await this.unitOfWork.GetRepository<Problem>().GetAll()).Select(x => new CheckBoxModel()
            {
                Id = x.Id,
                Label = x.Name
            }).ToList();
            filterModel.Query = "";

            return new InquriesListModel()
            {
                Inquries = await this.InquriesToModelsAsync(inquries),
                Count = (int)inquriesCount,
                Asc = asc,
                PagingModel = new PagingModel()
                {
                    Current = page ?? 1,
                    Total = (int)Math.Ceiling((decimal)inquriesCount / (decimal)RecordsPerPage)
                },
                FilterModel = filterModel
            };
        }

        public async Task<InquriesListModel> GetInquriesByDeputyAsync(string deputyId, int? page, string destinationName = null, bool asc = false)
        {
            var inquryRepository = this.unitOfWork.GetRepository<Inqury>();

            string deputyName = (await this.deputiesService.GetAllDeputiesAsync()).FirstOrDefault(x => x.Id == deputyId).Name;

            List<string> destinationIds = new List<string>();

            if (destinationName != null)
            {
                destinationIds = (await this.unitOfWork.GetRepository<Destination>().SearchFor(x => x.NameRaw.Contains(destinationName))).Select(x => x.Id).ToList();
            }

            IList<Inqury> inquries;
            long inquriesCount;

            if (destinationName == null)
            {
                inquriesCount = await inquryRepository.Count(x => x.AuthorId == deputyId || x.CoauthorIds.Contains(deputyId));

                inquries = await inquryRepository.SearchFor(x => x.AuthorId == deputyId || x.CoauthorIds.Contains(deputyId), RecordsPerPage, RecordsPerPage * ((page ?? 1) - 1),
    inqury => inqury.Date, asc);
            }
            else
            {
                inquriesCount = await inquryRepository.Count(x => (x.AuthorId == deputyId || x.CoauthorIds.Contains(deputyId)) && destinationIds.Contains(x.DestinationId));

                inquries = await inquryRepository.SearchFor(x => (x.AuthorId == deputyId || x.CoauthorIds.Contains(deputyId)) && destinationIds.Contains(x.DestinationId), RecordsPerPage, RecordsPerPage * ((page ?? 1) - 1),
    inqury => inqury.Date, asc);
            }



            var total = (int)Math.Ceiling((decimal)inquriesCount / (decimal)RecordsPerPage);

            return new InquriesListModel()
            {
                Inquries = await this.InquriesToModelsAsync(inquries),
                Count = (int)inquriesCount,
                Asc = asc,
                PagingModel = new PagingModel()
                {
                    Current = page ?? 1,
                    Total = (int)Math.Ceiling((decimal)inquriesCount / (decimal)RecordsPerPage)
                },
                DeputyName = deputyName,
                Destination = destinationName,
                DeputyId = deputyId
            };
        }

        public async Task<InquiryModel> GetInquiryInfoAsync(string inquryId)
        {
            var sessions = await this.unitOfWork.GetRepository<Session>().GetAll();
            var inqury = await this.unitOfWork.GetRepository<Inqury>().GetById(inquryId);
            var problem = await this.unitOfWork.GetRepository<Problem>().GetById(inqury.ProblemId);
            var destination = await this.unitOfWork.GetRepository<Destination>().GetById(inqury.DestinationId);
            var allDeputies =  await this.deputiesService.GetAllDeputiesAsync();


            var inquryModel = new InquiryModel();
            inquryModel.Problem = problem?.Name;

            var deputyName = allDeputies.FirstOrDefault(x => x.Id == inqury.AuthorId)?.Name;

            inquryModel.Id = inqury.Id;
            inquryModel.Author = deputyName;
            inquryModel.AuthorId = inqury.AuthorId;
            inquryModel.Destination = destination?.Full;
            inquryModel.Body = inqury.Body;
            inquryModel.BodyUrl = inqury.BodyUrl;
            inquryModel.DateRaw = inqury.DateRaw;
            inquryModel.DeadlineRaw = inqury.DeadlineRaw;
            inquryModel.RequestNumber = inqury.RequestNumber;
            inquryModel.CoauthorIds = inqury.CoauthorIds;
            inquryModel.Session = sessions.First(x => x.Id == inqury.SessionId).Number.ToString();


            foreach (var coauthorId in inquryModel.CoauthorIds)
            {
                inquryModel.Coauthors.Add(new DeputyModel()
                {
                    Id = coauthorId,
                    Name = allDeputies.FirstOrDefault(x => x.Id == coauthorId)?.Name
                });
            }

            inquryModel.InquryAnswers = this.mapper.Map<List<InquiryAnswerModel>>(inqury.InquryAnswers);

            return inquryModel;
        }
    }
}
