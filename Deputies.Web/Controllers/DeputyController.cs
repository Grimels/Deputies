using Deputies.BLL.Features.Deputies.Services;
using Deputies.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Deputies.Web.Controllers
{
    public class DeputyController : Controller
    {
        private readonly IDeputiesService deputiesService;

        public DeputyController(IDeputiesService deputiesService)
        {
            this.deputiesService = deputiesService;
        }

        public ActionResult SingleMember()
        {
            return View();
        }

        public ActionResult PartyMember()
        {
            return View();
        }

        public ActionResult ByAssociation()
        {
            return View();
        }

        public async Task<JsonCamelCaseResult> SingleDeputiesCountByRegions()
        {
            var result = await this.deputiesService.GetDeputiesCountByRegionAsync();
            return new JsonCamelCaseResult(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<PartialViewResult> SingleMemberByRegion(string region)
        {
            var result = await this.deputiesService.GetDeputiesByRegionAsync(region);
            return PartialView(result);
        }

        public async Task<PartialViewResult> DeputiesByParty(string partyId)
        {
            var result = await this.deputiesService.GetDeputiesByPartyAsync(partyId);
            return PartialView(result);
        }

        public async Task<PartialViewResult> DeputiesByAssociation(string id)
        {
            var result = await this.deputiesService.GetDeputiesByAssociationAsync(id);
            return PartialView(result);
        }
    }
}