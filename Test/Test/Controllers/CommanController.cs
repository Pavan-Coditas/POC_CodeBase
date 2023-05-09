using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmployeeApiConsumer.Controllers
{
    public class CommanController : Controller
    {
        public IActionResult Navigation()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        public IActionResult Selectors()
        {
            return View();
        }

        public IActionResult Getpartial(string selectView)
        {
            var res = "_Selector" + selectView;
            return PartialView(res);
        }

        public IActionResult Example()
        {
            return View();
        }

        public IActionResult Error(int code)
        {           
            ViewBag.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            ViewBag.ShowRequestId = !string.IsNullOrEmpty(ViewBag.RequestId);
            ViewBag.ErrorStatusCode = code;

            return View();
        }
    }
}
