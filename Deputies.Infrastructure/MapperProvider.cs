using AutoMapper;
using AutoMapper.Configuration;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.Infrastructure
{
    class MapperProvider
    {
        private readonly Container _container;

        public MapperProvider(Container container)
        {
            _container = container;
        }

        public IMapper GetMapper()
        {
            var mce = new MapperConfigurationExpression();
            mce.ConstructServicesUsing(_container.GetInstance);

            mce.AddProfiles(typeof(AutoMapperProfile).Assembly);

            var mc = new MapperConfiguration(mce);

            mc.AssertConfigurationIsValid();

            IMapper m = new Mapper(mc, t => _container.GetInstance(t));


            return m;
        }
    }
}
