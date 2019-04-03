using Deputies.BLL.Features.Deputies.Models;
using ParliamentaryInquiry.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Deputies.Services
{
    public interface IDeputiesService
    {
        Task<DeputiesByRegionModel> GetDeputiesByRegionAsync(string regionName);

        Task<IEnumerable<DeputiesCountByRegionModel>> GetDeputiesCountByRegionAsync();

        Task<DeputiesByAssociationModel> GetDeputiesByAssociationAsync(string associationId);

        Task<DeputiesByPartyModel> GetDeputiesByPartyAsync(string partyId);

        Task<IEnumerable<Deputy>> GetAllDeputiesAsync();

        Task<DeputyInfoModel> GetDeputyInfo(string deputyId);
    }
}
