using AutoMapper;
using Deputies.BLL.Features.Deputies.Models;
using ParliamentaryInquiry.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Deputy, DeputyModel>(MemberList.Source);
            CreateMap<SingleMemberDeputy, SingleMemberDeputyModel>(MemberList.Source);
        }
    }
}
