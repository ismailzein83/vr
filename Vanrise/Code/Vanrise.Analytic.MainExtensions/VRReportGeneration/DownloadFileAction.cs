using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.VRReportGeneration
{
    public class DownloadFileAction : VRReportGenerationAction
    {
        public override string ActionTypeName
        {
            get
            {
                return "DownloadFile";
            }
        }

        public override Guid ConfigId
        {
            get { return new Guid("1654683D-6168-47F4-B157-661A1FE88A95"); }
        }
        public VRAutomatedReportFileGenerator FileGenerator { get; set; }
    }
}
