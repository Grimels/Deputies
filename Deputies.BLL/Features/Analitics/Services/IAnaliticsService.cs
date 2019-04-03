using Deputies.BLL.Features.Analitics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Analitics.Services
{
    public interface IAnaliticsService
    {
        Task<DeputyAnalytics> GetDeputyAnaliticsAsync(string deputyId = null);

        Task<ChartResponse> GetInquriesBySessionsData(string deputyId = null);

        Task<DeputyRating> GetDeputiesByAssociations(string deputies = "", List<string> associations = null, int page = 1);

        Task<OrganizationsResponse> GetOrganizationsAnalitics(int page = 1);

        Task<IEnumerable<ProblemsResponseItem>> GetProblemsAnalitics(List<string> associations = null);

        Task<IEnumerable<ProblemsResponseItem>> GetRubricsAnalitics(List<string> associations = null);

        Task<AssociationsResponse> GetAssociationsRating();

        Task<ChartResponse> GetBillsBySessionsData(string deputyId = null);
    }
}
