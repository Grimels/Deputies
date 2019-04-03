using AutoMapper;
using Deputies.BLL.Features.Analitics.Services;
using Deputies.BLL.Features.Bills.Services;
using Deputies.BLL.Features.Deputies.Services;
using Deputies.BLL.Features.Index.Services;
using Deputies.BLL.Features.Inquiries.Services;
using Deputies.BLL.Shared.Services;
using Deputies.Core.Interfaces;
using Deputies.DAL.Mongo;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Deputies.Infrastructure
{
    public static class Bootstraper
    {
        public static void Bootstrap()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            container.Register(typeof(IRepository<>), typeof(MongoRepository<>), Lifestyle.Scoped);
            container.Register<IUnitOfWork, UnitOfWork>(Lifestyle.Scoped);
            container.Register<IDeputiesService, DeputiesService>(Lifestyle.Scoped);
            container.Register<IInquryService, InquiriesService>(Lifestyle.Scoped);
            container.Register<IAnaliticsService, AnaliticsService>(Lifestyle.Scoped);
            container.Register<IIndexService, IndexService>(Lifestyle.Scoped);
            container.Register<IBillsService, BillsService>(Lifestyle.Scoped);
            container.Register<IMailSender, MailSender>(Lifestyle.Scoped);
            container.Register<INotificationsService, NotificationsService>(Lifestyle.Scoped);
            
            container.RegisterSingleton(() => GetMapper(container));

            container.RegisterMvcControllers(Assembly.Load("Deputies.Web"));

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }

        private static IMapper GetMapper(Container container)
        {
            var mp = container.GetInstance<MapperProvider>();
            return mp.GetMapper();
        }
    }
}
