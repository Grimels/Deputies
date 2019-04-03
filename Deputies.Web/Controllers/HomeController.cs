using Deputies.BLL.Features.Deputies.Services;
using Deputies.BLL.Features.Index.Services;
using Deputies.BLL.Features.Inquiries.Services;
using Deputies.BLL.Shared.Services;
using Deputies.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Deputies.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IInquryService inquryService;
        private readonly IDeputiesService deputiesService;
        private readonly IIndexService indexService;
        private readonly IMailSender mailSender;

        public HomeController(IInquryService inquryService, IDeputiesService deputiesService, IIndexService indexService, IMailSender mailSender)
        {
            this.inquryService = inquryService;
            this.deputiesService = deputiesService;
            this.indexService = indexService;
            this.mailSender = mailSender;
        }

        [OutputCache(Duration = 86400, VaryByParam = "refreshCache", Location = OutputCacheLocation.Server, NoStore = true)]
        public async Task<ActionResult> Index()
        {
            var result = await this.indexService.GetIndexModelAsync();
            return View(result);
        }

        public async Task<ActionResult> Inquries(int? page, bool asc = false)
        {
            var result = await this.inquryService.GetInquiriesAsync(page, asc);
            return this.View(result);
        }

        public async Task<ActionResult> Filter(string query, int? page, string[] f, string[] s, string[] p, bool done = false, bool proc = false, bool ind = false, bool col = false, bool asc = false)
        {
            var result = await this.inquryService.FilterAsync(query, page, f, s, p, done, proc, ind, col, asc);
            return View("Inquries", result);
        }

        public async Task<ActionResult> InquriesByDeputy(string deputyId, int? page, string destinationName = null, bool asc = false)
        {
            var result = await this.inquryService.GetInquriesByDeputyAsync(deputyId, page, destinationName, asc);
            return View(result);
        }

        public async Task<ActionResult> Deputy(string deputyId)
        {
            var result = await this.deputiesService.GetDeputyInfo(deputyId);
            return View(result);
        }

        public async Task<ActionResult> Inqury(string inquryId)
        {
            var result = await this.inquryService.GetInquiryInfoAsync(inquryId);
            return View(result);
        }

        [HttpPost]
        public ActionResult Feedback(FeedbackModel model)
        {
            // babych.igor.i@gmail.com
            this.mailSender.Send("mikhaill.davydenko@gmail.com", "Web-сервiс депутатських запитiв: Повідомлення від " + model.Name + " (" + model.From + ")", model.Body);
            return this.View();
        }
    }
}