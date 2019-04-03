using Deputies.Core.Entities;
using Deputies.Core.Interfaces;
using ParliamentaryInquiry.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Shared.Services
{
    public class NotificationsService : INotificationsService
    {
        private const string Host = "http://deputies.org.ua/";

        public NotificationsService(IRepository<Destination> destinationRepository,
            IRepository<Inqury> inquryRepository,
            IRepository<Problem> problemsRepository,
            IRepository<SingleMemberDeputy> singleDeputyRepository,
            IRepository<MultiMemberDeputy> multyDeputyRepository,
            IRepository<Session> sessionRepo,
            IRepository<DeputyAssociation> associationsRepo,
            IRepository<Constituency> constituencyRepo,
            IRepository<PoliticalParty> partyRepo,
            IRepository<NotificationItem> notificationsRepo)
        {
            this.inquryRepository = inquryRepository;
            this.problemsRepository = problemsRepository;
            this.singleDeputyRepository = singleDeputyRepository;
            this.multyDeputyRepository = multyDeputyRepository;
            this.sessionRepo = sessionRepo;
            this.associationsRepo = associationsRepo;
            this.constituencyRepo = constituencyRepo;
            this.partyRepo = partyRepo;
            this.notificationsRepo = notificationsRepo;
        }

        private readonly IRepository<Destination> destinationRepository;
        private readonly IRepository<Inqury> inquryRepository;
        private readonly IRepository<Problem> problemsRepository;
        private readonly IRepository<SingleMemberDeputy> singleDeputyRepository;
        private readonly IRepository<MultiMemberDeputy> multyDeputyRepository;
        private readonly IRepository<Session> sessionRepo;
        private readonly IRepository<DeputyAssociation> associationsRepo;
        private readonly IRepository<Constituency> constituencyRepo;
        private readonly IRepository<PoliticalParty> partyRepo;
        private readonly IRepository<NotificationItem> notificationsRepo;

        public async Task Subscribe(string email, string deputyId)
        {
            var subscriptions = await notificationsRepo.SearchFor(x => x.Email.ToLower() == email.ToLower() && x.DeputyId == deputyId);
            var subscription = subscriptions.FirstOrDefault();
            if (subscription == null)
            {
                await notificationsRepo.Insert(new NotificationItem()
                {
                    Email = email,
                    DeputyId = deputyId
                });
            }
        }

        public async Task Unsubscribe(string email)
        {
            var subscriptions = await notificationsRepo.SearchFor(x => x.Email.ToLower() == email.ToLower());
            foreach (var s in subscriptions)
            {
                await notificationsRepo.Delete(s.Id);
            }
        }

        public async Task Notify()
        {
            var mailSender = new MailSender();
            var inquries = await GetDepityInquries();
            var notifications = await notificationsRepo.GetAll();
            var groups = notifications.GroupBy(x => x.Email);
            foreach (var group in groups)
            {
                var email = group.Key;
                var deputyIds = group.Select(x => x.DeputyId).ToList();
                var groupInquries = inquries.Where(x => deputyIds.Contains(x.DeputyId)).ToList();
                var inquriesToNotify = groupInquries.Where(x => x.Count != 0).ToList();

                if (!inquriesToNotify.Any())
                {
                    continue;
                }

                var mailBody = "<p><b>Активність депутатів за останню неділю</b></p>";
                foreach (var inq in inquriesToNotify)
                {
                    var link = Host + "Home/InquriesByDeputy?deputyId=" + inq.DeputyId;
                    var anchor = "<a href=\"" + link + "\">" + inq.Count + "</a>";
                    mailBody += string.Format("<p>{0} - {1} запитів.</p>", inq.DeputyName, anchor);
                }

                mailBody += "<hr \\>";
                mailBody += "<a href=\"" + Host + "notifications/unsubscribe?email=" + email + "\">Вiдписатися</a>";

                mailSender.Send(email, "Активність депутатів за останню неділю", mailBody);
            }
        }

        private async Task<List<DepityInquries>> GetDepityInquries()
        {
            var sevenDaysBefore = DateTime.Now.AddDays(-7);
            var inquries = await this.inquryRepository.SearchFor(x => x.Date >= sevenDaysBefore);
            var singles = await this.singleDeputyRepository.GetAll();
            var multies = await this.multyDeputyRepository.GetAll();

            var deputyInquries = new List<DepityInquries>();

            foreach (var d in singles)
            {
                var count = inquries.Count(x => x.AuthorId == d.Id || x.CoauthorIds.Contains(d.Id));
                deputyInquries.Add(new DepityInquries()
                {
                    Count = count,
                    DeputyId = d.Id,
                    DeputyName = d.Name
                });
            }

            foreach (var d in multies)
            {
                var count = inquries.Count(x => x.AuthorId == d.Id || x.CoauthorIds.Contains(d.Id));
                deputyInquries.Add(new DepityInquries()
                {
                    Count = count,
                    DeputyId = d.Id,
                    DeputyName = d.Name
                });
            }

            return deputyInquries;
        }
    }

    class DepityInquries
    {
        public string DeputyId { get; set; }

        public string DeputyName { get; set; }

        public int Count { get; set; }
    }
}
