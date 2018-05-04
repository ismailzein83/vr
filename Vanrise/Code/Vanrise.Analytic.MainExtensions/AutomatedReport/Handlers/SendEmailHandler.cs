using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers
{
    public class SendEmailHandler : VRAutomatedReportHandlerSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("C05375FD-4C3A-44B2-ACEE-A0EDEE56B488"); }
        }

        public string To { get; set; }

        //public string CC { get; set; }

        //public string BCC { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public List<VRAutomatedReportFileGenerator> AttachementGenerators { get; set; }

        public override void Execute(IVRAutomatedReportHandlerExecuteContext context)
        {
            throw new NotImplementedException();
        }
    }
}
