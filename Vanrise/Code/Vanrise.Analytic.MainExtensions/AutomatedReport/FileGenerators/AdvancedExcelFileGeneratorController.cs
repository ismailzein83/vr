using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vanrise.Analytic.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AdvancedExcelFileGenerator")]
    public class AdvancedExcelFileGeneratorController: BaseAPIController
    {
        AdvancedExcelFileGeneratorManager _manager = new AdvancedExcelFileGeneratorManager();
        [HttpPost]
        [Route("DownloadAttachmentGenerator")]
        public object DownloadAttachmentGenerator(DownloadAttachmentGeneratorInput input)
        {
            input.ThrowIfNull("input");
            input.FileGenerator.ThrowIfNull("input.FileGenerator");
            var generatedFile = _manager.DownloadAttachmentGenerator(input);
            generatedFile.ThrowIfNull("generatedFile");
            return GetExcelResponse(generatedFile.FileContent, generatedFile.FileName);
           
        }
    }
    public class DownloadAttachmentGeneratorInput
    {
        public VRAutomatedReportFileGenerator FileGenerator { get; set; }

        public List<VRAutomatedReportQuery> Queries { get; set; }
    }
}
