using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using System.Web;
using System.IO;

namespace Vanrise.Invoice.MainExtensions
{
    public class ReportFileExistCondition : InvoiceGridActionFilterCondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("77D7261A-E2C6-4A49-B6FA-4010A07F2C2B"); }
        }
        private bool? _ReportFileExists { get; set; }
        public override bool IsFilterMatch(IInvoiceGridActionFilterConditionContext context)
        {
            if (!this._ReportFileExists.HasValue)
            {
                OpenRDLCReportAction openRDLCReportAction = context.InvoiceAction.Settings.CastWithValidate<OpenRDLCReportAction>("Empty Action");
                string reportURL = openRDLCReportAction.ReportURL;
                string reportRuntimeURL = openRDLCReportAction.ReportRuntimeURL;
                string ReportUrl = null;
                if (HttpContext.Current != null)
                {
                    ReportUrl = HttpContext.Current.Server.MapPath(reportURL);
                }
                else
                {
                    string currentDir = Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(OpenRDLCReportActionManager)).Location);
                    ReportUrl = Path.Combine(currentDir, reportRuntimeURL);
                }
                if (!File.Exists(ReportUrl))
                    this._ReportFileExists = false;
                this._ReportFileExists = true;
            }
            return this._ReportFileExists.Value;
        }
    }
}
