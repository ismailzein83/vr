using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Vanrise.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }

//    public class VRLocalizationHttpModules : IHttpModule
//    {
//        public void Dispose()
//        {
           
//        }

//        public void Init(HttpApplication application)
//        {
//            application.BeginRequest += application_BeginRequest;
//        }

//        void application_BeginRequest(object sender, System.EventArgs e)
//        {
//            HttpApplication application = (HttpApplication)sender;            
//            HttpContext context = application.Context;
//            string regss = "VRRes\\..*?\\.VREnd";
//            string text = @"fdsgfdsg dsg dskgfldsjf dsfjds f 345 [ \ dd .323 dsf\n kdsfj dskfjds f
//dsfdsgg VRRes.dkkkk.VREnd kfjds kfpg jfdsg fdspogfd 89743543 skdf 90433 @4e3 kjdr
//gfdhg fdh VRRes.fdlk sjkd sfj.VREnd fjsdofk dsigh gfdsg  VRRes.sijerwt.dsfdsfd435 #@ ()sf.VREnd
//";
//            var matches = System.Text.RegularExpressions.Regex.Matches(text, regss);

//            foreach (var match in matches)
//            {

//            }
//            //if (context.Request.Url.AbsolutePath.EndsWith(".html"))
//            //{
//            //    string physicalPath = context.Request.PhysicalPath;
//            //    string fileContent = File.ReadAllText(physicalPath);
//            //    string rootPath = context.Server.MapPath("/");
//            //    string generatedFilespath = Path.Combine(rootPath, "vr-generated");
//            //    if (!Directory.Exists(generatedFilespath))
//            //        Directory.CreateDirectory(generatedFilespath);
//            //    string fileName = Guid.NewGuid().ToString() + ".html";
//            //    File.WriteAllText(Path.Combine(generatedFilespath, fileName), String.Format("<h1>Modified Programmatically<h1><br>{0}", fileContent));
//            //    context.RewritePath(String.Format("/vr-generated/{0}", fileName));
//            //}
//            //string filePath = context.Request.FilePath;
//            //string fileExtension =
//            //    VirtualPathUtility.GetExtension(filePath);
//            //if (fileExtension.Equals(".aspx"))
//            //{
//            //    context.Response.Write("<h1><font color=red>" +
//            //        "HelloWorldModule: Beginning of Request" +
//            //        "</font></h1><hr>");
//            //}
//        }
//    }

}
