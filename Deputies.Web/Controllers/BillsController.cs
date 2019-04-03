using Deputies.BLL.Features.Bills.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Deputies.Web.Controllers
{
    public class BillsController : Controller
    {
        private readonly IBillsService billsService;

        public BillsController(IBillsService billsService)
        {
            this.billsService = billsService;
        }

        // GET: Bills
        public async Task<ActionResult> Index(int? page, bool asc = false)
        {
            var model = await this.billsService.GetBillsAsync(page, asc);
            return View(model);
        }

        public async Task<ActionResult> Filter(string query, int? page, string[] s, string[] r, string[] sor, bool asc = false)
        {
            var result = await this.billsService.FilterAsync(query, page, s, r, sor, asc);
            return View("Index", result);
        }

        public async Task<ActionResult> Bill(string id)
        {
            var model = await this.billsService.GetBillInfoAsync(id);
            return View(model);
        }

        public async Task<ActionResult> BillsByDeputy(string deputyId, int? page, string destinationName = null, bool asc = false)
        {
            var model = await this.billsService.GetBillsByDeputyAsync(deputyId, page, asc);
            return View(model);
        }
    }
}