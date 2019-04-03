using Deputies.BLL.Features.Bills.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Bills.Services
{
    public interface IBillsService
    {
        Task<BillsListModel> GetBillsAsync(int? page, bool asc = false);

        Task<BillsListModel> GetBillsByDeputyAsync(string deputyId, int? page, bool asc = false);

        Task<BillsListModel> FilterAsync(string query, int? page, string[] s, string[] r, string[] sor, bool asc = false);

        Task<BillModel> GetBillInfoAsync(string billId);
    }
}
