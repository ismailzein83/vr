using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;
using Vanrise.Invoice.MainExtensions;
using Vanrise.Invoice.Web.Controllers;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

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

                    Guid actionId = actionIdString != null ? new Guid(actionIdString) : Guid.Empty;
                    var context = Vanrise.Common.Serializer.Deserialize<IInvoiceActionContext>(invoiceActionContext);

                    if (!context.DoesUserHaveAccess(actionId))
                        throw new UnauthorizedAccessException("you are not authorized to perform this request");


                    openRDLCReportActionManager.BuildRdlcReport(ReportViewer1, new ReportInput
                    {
                        ActionId = actionId,
                        Context = context
                    });
                }
                catch(Exception error)
                {
                    LoggerFactory.GetExceptionLogger().WriteException(error);
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