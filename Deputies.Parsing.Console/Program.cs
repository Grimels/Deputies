using Deputies.Bills.Resetter;
using Deputies.BLL.Shared.Services;
using Deputies.Core.Entities;
using Deputies.DAL.Mongo;
using Deputies.Inquiries.Resetter;
using Deputies.Parsing.Common;
using FluentScheduler;
using ParliamentaryInquiry.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.Parsing.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.EnableInfo = true;
            
            var registry = new Registry();
            JobManager.UseUtcTime();
            registry.Schedule(async () =>
            {
                await ParseInquries();
            }).ToRunEvery(1).Days().At(0, 0);

            registry.Schedule(async () =>
            {
                await ParseBills();
            }).ToRunEvery(1).Days().At(1, 0);

            registry.Schedule(async () =>
            {
                var service = new NotificationsService(
                    new MongoRepository<Destination>(),
                    new MongoRepository<Inqury>(),
                    new MongoRepository<Problem>(),
                    new MongoRepository<SingleMemberDeputy>(),
                    new MongoRepository<MultiMemberDeputy>(),
                    new MongoRepository<Session>(),
                    new MongoRepository<DeputyAssociation>(),
                    new MongoRepository<Constituency>(),
                    new MongoRepository<PoliticalParty>(),
                    new MongoRepository<NotificationItem>());
                await service.Notify();
            }).ToRunEvery(0).Weeks().On(DayOfWeek.Sunday).At(10, 0);

            JobManager.UseUtcTime();
            JobManager.Initialize(registry);

            while (true)
            {
                System.Console.Read();
            }
        }

        private static async Task ParseBills()
        {
            Logger.Log("Bills parsing started");
            var billResetter = new BillReseter();
            await billResetter.ResetAsync();
            Logger.Log("Bills parsing completed");
        }

        private static async Task ParseInquries()
        {
            Logger.Log("Inquries parsing started");
            var reseter = new InquiryResetter();
            await reseter.Reset();
            Logger.Log("Inquiry parsing completed");
        }
    }
}
