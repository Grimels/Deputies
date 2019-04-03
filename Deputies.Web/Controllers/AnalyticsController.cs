using Deputies.BLL.Features.Analitics.Services;
using Deputies.Core.Interfaces;
using ParliamentaryInquiry.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Deputies.Web.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly IAnaliticsService analiticsService;
        private readonly IUnitOfWork unitOfWork;

        public AnalyticsController(IAnaliticsService analiticsService, IUnitOfWork unitOfWork)
        {
            this.analiticsService = analiticsService;
            this.unitOfWork = unitOfWork;
        }

        public ActionResult InquriesBySessions()
        {
            return View();
        }

        public async Task<JsonResult> InquriesBySessionsData(string deputyId = null)
        {
            var result = await this.analiticsService.GetInquriesBySessionsData(deputyId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BillsBySessions()
        {
            return View();
        }

        public async Task<JsonResult> BillsBySessionsData(string deputyId = null)
        {
            var result = await this.analiticsService.GetBillsBySessionsData(deputyId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> DeputyAnalitics(string deputyId = null)
        {
            var result = await this.analiticsService.GetDeputyAnaliticsAsync(deputyId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Deputies(string deps = "", List<string> a = null, int page = 1)
        {
            var result = await this.analiticsService.GetDeputiesByAssociations(deps, a, page);
            return View(result);
        }

        [OutputCache(Duration = 86400, VaryByParam = "refreshCache", Location = OutputCacheLocation.Server, NoStore = true)]
        public async Task<ActionResult> Associations()
        {
            var response = await this.analiticsService.GetAssociationsRating();
            return this.View(response);
        }

        public ActionResult AssociationsBubbles()
        {
            return this.View();
        }

        [OutputCache(Duration = 86400, VaryByParam = "refreshCache", Location = OutputCacheLocation.Server, NoStore = true)]
        public async Task<JsonResult> AssociationsData()
        {
            var response = await this.analiticsService.GetAssociationsRating();
            return this.Json(response, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Organizations(int page = 1)
        {
            var result = await this.analiticsService.GetOrganizationsAnalitics(page);
            return View(result);
        }

        [OutputCache(Duration = 86400, VaryByParam = "refreshCache", Location = OutputCacheLocation.Server, NoStore = true)]
        public async Task<ActionResult> Problems()
        {
            var associations = await this.unitOfWork.GetRepository<DeputyAssociation>().GetAll();
            return this.View(associations);
        }

        public async Task<JsonResult> ProblemsData(List<string> a = null)
        {
            var result = await this.analiticsService.GetProblemsAnalitics(a);
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 86400, VaryByParam = "refreshCache", Location = OutputCacheLocation.Server, NoStore = true)]
        public async Task<ActionResult> Rubrics()
        {
            var associations = await this.unitOfWork.GetRepository<DeputyAssociation>().GetAll();
            return this.View(associations);
        }

        public async Task<JsonResult> RubricsData(List<string> a = null)
        {
            var result = await this.analiticsService.GetRubricsAnalitics(a);
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}