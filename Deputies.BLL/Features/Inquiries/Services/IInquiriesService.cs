using Deputies.BLL.Features.Inquiries.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Inquiries.Services
{
    public interface IInquryService
    {
        Task<InquriesListModel> FilterAsync(string query, int? page, string[] fractionsIds, string[] sessionsIds, string[] problemsIds, bool includeDone = false, bool includeInprocess = false, bool includeIndividual = false, bool includeCollective = false, bool asc = false);

        Task<InquriesListModel> GetInquiriesAsync(int? page, bool asc = false);

        Task<InquriesListModel> GetInquriesByDeputyAsync(string deputyId, int? page, string destinationName = null, bool asc = false);

        Task<InquiryModel> GetInquiryInfoAsync(string inquryId);
    }
}
