using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRCompilationController")]
    [JSONWithTypeAttribute]
    public class VRCommon_CompilationController : BaseAPIController
    {
        [HttpPost]
        [Route("ExportCompilationResult")]
        public object ExportCompilationResult(List<string> errors)
        {
            var errorsByte = new List<byte>();
            if (errors != null && errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    byte[] encodedError = System.Text.Encoding.ASCII.GetBytes(error);
                    for (var i = 0; i < encodedError.Length; i++)
                    {
                        errorsByte.Add(encodedError[i]);
                    }
                }
            }

            return base.GetExcelResponse(errorsByte.ToArray(), "CompilationResult.xls");
        }
    }
}