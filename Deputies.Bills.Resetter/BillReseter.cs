using Deputies.Core.Entities;
using Deputies.DAL.Mongo;
using Deputies.Infrastructure;
using Deputies.Parsing.Common;
using ParliamentaryInquiry.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.Bills.Resetter
{
    public class BillReseter
    {
        private readonly BillParser billParser;
        private readonly MongoRepositoryWithoutCache<SingleMemberDeputy> singleDeputyRepository;
        private readonly MongoRepositoryWithoutCache<MultiMemberDeputy> multyDeputyRepository;
        private readonly MongoRepositoryWithoutCache<Session> sessionRepo;
        private readonly MongoRepositoryWithoutCache<SubjectOfRight> subjectsRepo;
        private readonly MongoRepositoryWithoutCache<Committee> committeeRepo;
        private readonly MongoRepositoryWithoutCache<Rubric> rubricRepo;
        private readonly MongoRepositoryWithoutCache<Bill> billsRepo;
        private readonly MongoRepositoryWithoutCache<BillInitiator> billInitiatorRepo;

        public BillReseter()
        {
            singleDeputyRepository = new MongoRepositoryWithoutCache<SingleMemberDeputy>();
            multyDeputyRepository = new MongoRepositoryWithoutCache<MultiMemberDeputy>();
            sessionRepo = new MongoRepositoryWithoutCache<Session>();
            committeeRepo = new MongoRepositoryWithoutCache<Committee>();
            rubricRepo = new MongoRepositoryWithoutCache<Rubric>();
            subjectsRepo = new MongoRepositoryWithoutCache<SubjectOfRight>();
            billsRepo = new MongoRepositoryWithoutCache<Bill>();
            billInitiatorRepo = new MongoRepositoryWithoutCache<BillInitiator>();
            this.billParser = new BillParser();
        }

        public async Task ResetAsync()
        {
            var allSessions = await sessionRepo.GetAll();
            var allSubject = await subjectsRepo.GetAll();
            var allRubrics = await rubricRepo.GetAll();
            var allCommittees = await committeeRepo.GetAll();
            var allMultyDeputies = await multyDeputyRepository.GetAll();
            var allSingleDeputies = await singleDeputyRepository.GetAll();

            var bills = await this.billParser.ParseAllBillsAsync();
            var billsEntities = new List<Bill>();

            var counter = 0;
            var count = bills.Count();
            foreach (var bill in bills)
            {
                try
                {
                    var billEntity = new Bill();
                    billEntity.Title = bill.Title;
                    billEntity.Number = bill.Number;
                    billEntity.DateRaw = bill.Date;
                    billEntity.Date = DateParser.ParseShortDate(bill.Date).Value;
                    billEntity.Included = bill.Included;

                    // session
                    if(bill.Session != 0)
                    {
                        billEntity.SessionId = allSessions.FirstOrDefault(x => x.Number == bill.Session)?.Id;
                        if (billEntity.SessionId == null)
                        {
                            await sessionRepo.Insert(new Session() { Number = bill.Session });
                            allSessions = await sessionRepo.GetAll();
                            billEntity.SessionId = allSessions.First(x => x.Number == bill.Session).Id;
                        }
                    }

                    // subject of right
                    if(!string.IsNullOrWhiteSpace(bill.SubjectOfRight))
                    {
                        billEntity.SubjectOfRightId = allSubject.FirstOrDefault(x => x.Name == bill.SubjectOfRight)?.Id;
                        if (billEntity.SubjectOfRightId == null)
                        {
                            await subjectsRepo.Insert(new SubjectOfRight() { Name = bill.SubjectOfRight });
                            allSubject = await subjectsRepo.GetAll();
                            billEntity.SubjectOfRightId = allSubject.FirstOrDefault(x => x.Name == bill.SubjectOfRight).Id;
                        }
                    }

                    // rubbric
                    if (!string.IsNullOrWhiteSpace(bill.Rubric))
                    {
                        billEntity.RubricId = allRubrics.FirstOrDefault(x => x.Name == bill.Rubric)?.Id;
                        if (billEntity.RubricId == null)
                        {
                            await rubricRepo.Insert(new Rubric() { Name = bill.Rubric });
                            allRubrics = await rubricRepo.GetAll();
                            billEntity.RubricId = allRubrics.FirstOrDefault(x => x.Name == bill.Rubric).Id;
                        }
                    }

                    // main committee
                    if(!string.IsNullOrWhiteSpace(bill.MainCommittee))
                    {
                        billEntity.MainCommitteeId = allCommittees.FirstOrDefault(x => x.Name == bill.MainCommittee)?.Id;
                        if (billEntity.MainCommitteeId == null)
                        {
                            await committeeRepo.Insert(new Committee() { Name = bill.MainCommittee });
                            allCommittees = await committeeRepo.GetAll();
                            billEntity.MainCommitteeId = allCommittees.FirstOrDefault(x => x.Name == bill.MainCommittee).Id;
                        }
                    }

                    // other committees
                    if (bill.OtherCommittees != null)
                    {
                        foreach (var committee in bill.OtherCommittees)
                        {
                            var committeeId = allCommittees.FirstOrDefault(x => x.Name == committee)?.Id;
                            if (committeeId == null)
                            {
                                await committeeRepo.Insert(new Committee() { Name = committee });
                                allCommittees = await committeeRepo.GetAll();
                                committeeId = allCommittees.FirstOrDefault(x => x.Name == committee).Id;
                            }
                            billEntity.OtherCommitteesIds.Add(committeeId);
                        }
                    }

                    // Initiators
                    if (bill.Initiators != null)
                    {
                        foreach(var initiator in bill.Initiators.Where(x => !string.IsNullOrWhiteSpace(x)))
                        {
                            var id = this.GetDeputyIdByName(initiator, allSingleDeputies, allMultyDeputies);
                            if(id != null)
                            {
                                billEntity.InitiatorsIds.Add(id);
                            }
                            else
                            {
                                var i = (await this.billInitiatorRepo.SearchFor(x => x.Name == initiator)).FirstOrDefault();
                                if(i != null)
                                {
                                    billEntity.InitiatorsIds.Add(i.Id);
                                }
                                else
                                {
                                    await this.billInitiatorRepo.Insert(new BillInitiator() { Name = initiator });
                                    i = (await this.billInitiatorRepo.SearchFor(x => x.Name == initiator)).FirstOrDefault();
                                    billEntity.InitiatorsIds.Add(i.Id);
                                }
                            }
                        }
                    }

                    // texts 
                    if(bill.Texts != null)
                    {
                        billEntity.Texts = bill.Texts.Select(x => new Link()
                        {
                            Title = x.Title,
                            URL = x.URL
                        }).ToList();
                    }


                    // documents 
                    if(bill.DocumentsRelatedToWork != null)
                    {
                        billEntity.DocumentsRelatedToWork = bill.DocumentsRelatedToWork.Select(x => new Link()
                        {
                            Title = x.Title,
                            URL = x.URL
                        }).ToList();
                    }

                    billsEntities.Add(billEntity);

                    if (++counter % 1000 == 0)
                    {
                        Logger.LogInfo("Reset " + counter + " bills");
                    }
                }
                catch (Exception e)
                {
                    Logger.LogException("An error has occured while parsing bill " + bill?.Number, e);
                    continue;
                }
            }

            await billsRepo.Clear();
            await billsRepo.InsertMany(billsEntities);
        }

        private string GetDeputyIdByName(string name, IList<SingleMemberDeputy> allSingleDeputies, IList<MultiMemberDeputy> allMultyDeputies)
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
