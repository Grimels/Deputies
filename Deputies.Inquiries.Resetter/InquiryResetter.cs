using Deputies.Core.Interfaces;
using Deputies.DAL.Mongo;
using Deputies.Infrastructure;
using Deputies.Parsing.Common;
using Newtonsoft.Json;
using ParliamentaryInquiry.Core.Entities;
using RadaGovUa.Parser.Helpers;
using RadaGovUa.Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.Inquiries.Resetter
{
    public class InquiryResetter
    {
        private readonly MongoRepositoryWithoutCache<Destination> destinationRepository;
        private readonly MongoRepositoryWithoutCache<Inqury> inquryRepository;
        private readonly MongoRepositoryWithoutCache<Problem> problemsRepository;
        private readonly MongoRepositoryWithoutCache<SingleMemberDeputy> singleDeputyRepository;
        private readonly MongoRepositoryWithoutCache<MultiMemberDeputy> multyDeputyRepository;
        private readonly MongoRepositoryWithoutCache<Session> sessionRepo;
        private readonly DeputyRequestsParserHelper deputyRequestsParserHelper;

        public InquiryResetter()
        {
            destinationRepository = new MongoRepositoryWithoutCache<Destination>();
            inquryRepository = new MongoRepositoryWithoutCache<Inqury>();
            problemsRepository = new MongoRepositoryWithoutCache<Problem>();
            singleDeputyRepository = new MongoRepositoryWithoutCache<SingleMemberDeputy>();
            multyDeputyRepository = new MongoRepositoryWithoutCache<MultiMemberDeputy>();
            sessionRepo = new MongoRepositoryWithoutCache<Session>();
            deputyRequestsParserHelper = new DeputyRequestsParserHelper();
        }

        public async Task Reset()
        {
            var allInquriesEntity = new List<Inqury>();

            var allInquries = await this.deputyRequestsParserHelper.Parse();

            var allMultyDeputies = await multyDeputyRepository.GetAll();
            var allSingleDeputies = await singleDeputyRepository.GetAll();
            var allSessions = await sessionRepo.GetAll();

            var allProblemsList = await problemsRepository.GetAll();
            var allDestinationList = await destinationRepository.GetAll();

            var counter = 1;
            foreach (var inquryDto in allInquries)
            {
                try
                {
                    if (string.IsNullOrEmpty(inquryDto.RequestId))
                    {
                        continue;
                    }

                    var inqury = new Inqury();

                    if (inquryDto.RequestCoAuthors != null)
                    {
                        var allCoautorsNames =
                            inquryDto.RequestCoAuthors.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();
                        allCoautorsNames.Remove(inquryDto.RequestAuthor);

                        foreach (var coauthor in allCoautorsNames)
                        {
                            var coaauthorId = this.GetDeputyIsByName(coauthor, allSingleDeputies, allMultyDeputies);
                            if (coaauthorId != null)
                            {
                                inqury.CoauthorIds.Add(coaauthorId);
                            }
                        }
                    }


                    inqury.Body = inquryDto.RequestBody;
                    inqury.BodyUrl = inquryDto.RequestBodyUrl;
                    inqury.DateRaw = inquryDto.RequestDate;
                    inqury.Date = DateParser.ParseLongDate(inqury.DateRaw);
                    inqury.DeadlineRaw = inquryDto.DeadlineDate;
                    inqury.RequestNumber = inquryDto.RequestId;

                    if (inquryDto.Whom != null)
                    {
                        var destination = allDestinationList.FirstOrDefault(x => x.NameRaw == inquryDto.Whom);
                        if (destination == null)
                        {
                            var organization = OrganizationParser.ParseOrganization(inquryDto.Whom);
                            await destinationRepository.Insert(new Destination() { NameRaw = inquryDto.Whom, Full = organization.Full, Name = organization.Name });
                            allDestinationList = await destinationRepository.GetAll();
                            destination = allDestinationList.First(x => x.NameRaw == inquryDto.Whom);
                        }
                        inqury.DestinationId = destination.Id;
                    }


                    if (inquryDto.Problem != null)
                    {
                        var problem = allProblemsList.FirstOrDefault(x => x.Name == inquryDto.Problem);
                        if (problem == null)
                        {
                            await problemsRepository.Insert(new Problem() { Name = inquryDto.Problem });
                            allProblemsList = await problemsRepository.GetAll();
                            problem = allProblemsList.First(x => x.Name == inquryDto.Problem);
                        }
                        inqury.ProblemId = problem.Id;
                    }


                    var authorId = this.GetDeputyIsByName(inquryDto.RequestAuthor, allSingleDeputies, allMultyDeputies);
                    if (authorId == null)
                    {
                        if (!inqury.CoauthorIds.Any())
                        {
                            continue;
                        }
                        else
                        {
                            authorId = inqury.CoauthorIds.First();
                        }
                    }

                    var sessionNum = int.Parse(inquryDto.SessionId.Split(new string[] { " " }, StringSplitOptions.None)[0]);
                    inqury.SessionId = allSessions.FirstOrDefault(x => x.Number == sessionNum)?.Id;
                    if (inqury.SessionId == null)
                    {
                        await sessionRepo.Insert(new Session() { Number = sessionNum });
                        allSessions = await sessionRepo.GetAll();
                        inqury.SessionId = allSessions.First(x => x.Number == sessionNum).Id;
                    }

                    inqury.AuthorId = authorId;

                    foreach (var answerDto in inquryDto.Answers)
                    {
                        var answer = new InquryAnswer();
                        answer.FamiliarizationDateRaw = answerDto.FamiliarizationDate;
                        answer.FamiliarizationDate = DateParser.ParseShortDate(answerDto.FamiliarizationDate);
                        answer.AnswerBody = answerDto.AnswerBody;
                        answer.AnswerBodyUrl = answerDto.AnswerBodyUrl;
                        answer.AnswerDateRaw = answerDto.AnswerDate;
                        answer.AnswerDate = DateParser.ParseShortDate(answerDto.AnswerDate);
                        inqury.InquryAnswers.Add(answer);
                    }

                    allInquriesEntity.Add(inqury);


                    if (++counter % 1000 == 0)
                    {
                        Logger.LogInfo("Reset " + counter + " inquries");
                    }
                }
                catch (Exception e)
                {
                    Logger.LogException("An error has occured while parsing inquiry " + inquryDto.RequestBody, e);
                    continue;
                }
            }

            await inquryRepository.Clear();
            await inquryRepository.InsertMany(allInquriesEntity);

        }

        private string GetDeputyIsByName(string name, IList<SingleMemberDeputy> allSingleDeputies, IList<MultiMemberDeputy> allMultyDeputies)
        {
            var deputyName = name.TrimEndString(" VIII скл.", StringComparison.CurrentCulture);
            deputyName = deputyName.TrimEnd(' ');
            var deputy = allSingleDeputies.FirstOrDefault(x => x.Name == deputyName);
            if (deputy == null)
            {
                var multyDeputy = allMultyDeputies.FirstOrDefault(x => x.Name == deputyName);
                return multyDeputy?.Id;
            }
            else
            {
                return deputy.Id;
            }
        }
    }
}
