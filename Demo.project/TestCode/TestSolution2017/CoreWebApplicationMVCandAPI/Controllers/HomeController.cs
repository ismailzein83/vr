using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreWebApplicationMVCandAPI.Models;
using System.Text;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Routing;

namespace CoreWebApplicationMVCandAPI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.MyProp = "Proddd";

            ViewBag.Title = "Home Page";
            ViewBag.CookieName = "Test cookie name";
            ViewBag.version = 4;
            ViewBag.ProductVersion = "Product version";
            ViewBag.CompanyName = "comp name";
            ViewBag.isEnabledGA = false;

            ViewBag.IsLocalizationEnabled = "false";
            ViewBag.IsRTL = "false";
            ViewBag.RTLClass = "";


            ViewBag.HasRemoteApplications = "false";


            //return View("/Views/Home2/Index.cshtml");
            return View("/Client/CSViews/Home/Index.cshtml");
        }

        public IActionResult About()
        {
            
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public static class VRScript
    {
        public static Microsoft.AspNetCore.Html.HtmlString MyOwnHtmlHelper(string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"<b>{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}</b>");
            builder.Append("<br>");
            foreach (var filePath in Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.*", SearchOption.AllDirectories))
            {
                builder.AppendLine(filePath);
                builder.Append("<br>");
            }
            return new Microsoft.AspNetCore.Html.HtmlString($"<span>{builder.ToString()}<br><b>{message}<b/><span>");
        }

        public static Microsoft.AspNetCore.Html.HtmlString RenderScript(string fileName)
        {
            
            return new Microsoft.AspNetCore.Html.HtmlString(string.Format(@"<script src=""/js/{0}.js?v=1.33""></script>", fileName));
        }
    }
}
