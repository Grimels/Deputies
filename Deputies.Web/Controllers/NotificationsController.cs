using Deputies.BLL.Shared.Services;
using Deputies.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Deputies.Web.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly INotificationsService notificationsService;

        public NotificationsController(INotificationsService notificationsService)
        {
            this.notificationsService = notificationsService;
        }

        [HttpPost]
        public async Task<ActionResult> Subscribe(NotificationItem item)
        {
            await notificationsService.Subscribe(item.Email, item.DeputyId);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Unsubscribe(string email)
        {
            await notificationsService.Unsubscribe(email);
            return View();
        }
    }
}