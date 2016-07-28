using CBLSummerPortfolio07132016.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CBLSummerPortfolio07132016.Controllers
{
    public class HomeController : Controller
    {
        [RequireHttps]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Portfolio()
        {
            ViewBag.Message = "Portfolio.";

            return View();
        }
        public ActionResult Resume()
        {
            ViewBag.Message = "Resume.";

            return View();
        }
        public ActionResult JS2()
        {
            ViewBag.Message = "Javascript Exercise 2.";

            return View();
        }
        public ActionResult JS4()
        {
            ViewBag.Message = "Javascript Exercise 2.";

            return View();
        }
        public ActionResult PortfolioTest()
        {
            ViewBag.Message = "Portfolio.";

            return View();
        }
        public ActionResult ComingSoon()
        {
            ViewBag.Message = "ComingSoon";

            return View();
        }

        public ActionResult Blog()
        {
            return View();
        }

        //Post Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact([Bind(Include = "Id, Name, Email, Subject, Message, Phone, MessageSent")] Contact contact)
        {
            contact.MessageSent = DateTime.Now;

            var svc = new EmailService();
            var msg = new IdentityMessage();
            msg.Subject = "Contact From Azure.Net Web Site " + contact.Name + " re: " + contact.Subject;
            msg.Body = contact.Message;
            await svc.SendAsync(msg);

            return View("Thanks");


        }
    }
}
