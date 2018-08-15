using System;
using System.Web.Http;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Web.Base;
using System.IO;

namespace TOne.WhS.Sales.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "CodeCompare")]
    public class CodeCompareController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredCodeCompare")]
        public object GetFilteredCodeCompare(Vanrise.Entities.DataRetrievalInput<CodeCompareQuery> input)
        {
            CodeCompareManager manager = new CodeCompareManager();
            return GetWebResponse(input, manager.GetFilteredCodeCompare(input), "Code Comparison.xlsx");
        }
        [HttpPost]
        [Route("ExportCodeCompareTemplate")]
        public object ExportCodeCompareTemplate(CodeCompareQuery query)
        {
            string templateRelativePath = "~/Client/Modules/WhS_CodePreparation/Template/Code preparation sample.xls";
            string templateAbsolutePath = HttpContext.Current.Server.MapPath(templateRelativePath);
            byte[] templateBytes = File.ReadAllBytes(templateAbsolutePath);
            byte[] templateWithDataBytes = new CodeCompareManager().ExportCodeCompareTemplate(templateBytes,query);
            if (templateWithDataBytes == null)
                return null;
            MemoryStream memoryStream = new System.IO.MemoryStream();
            memoryStream.Write(templateWithDataBytes, 0, templateWithDataBytes.Length);
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            return GetExcelResponse(memoryStream, "Code Comparison Result.xlsx");
        }
    }
}