using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;
using Vanrise.Invoice.MainExtensions;
using Vanrise.Invoice.Web.Controllers;

namespace Vanrise.Invoice.Web.VR_Invoice.Reports
{
    public partial class CustomerInvoiceReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                try
                {
                    OpenRDLCReportActionManager openRDLCReportActionManager = new MainExtensions.OpenRDLCReportActionManager();
                    string actionIdString = Request.QueryString["actionId"];
                    string invoiceActionContext = Request.QueryString["invoiceActionContext"];
                    openRDLCReportActionManager.BuildRdlcReport(ReportViewer1, new ReportInput
                    {
                        ActionId = actionIdString != null?new Guid(actionIdString):Guid.Empty,
                        Context = Vanrise.Common.Serializer.Deserialize<IInvoiceActionContext>(invoiceActionContext)
                    });
                }
                catch(Exception error)
                {
                    labelError.Text = error.Message;
                }
            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            GC.Collect();
        }
    }
}