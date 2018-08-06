using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TOne.WhS.Analytics.Business;
using TOne.WhS.Analytics.Entities.BillingReport;
using Microsoft.Reporting.WebForms;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;
using TOne.WhS.Analytics.Entities;
namespace TOne.WhS.Analytics.Web.Reports.Analytics
{
    public partial class BillingReports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Vanrise.Security.Entities.ContextFactory.GetContext().HasPermissionToActions("WhS_Analytics/ReportDefinition/GetAllRDLCReportDefinition"))
                {
                    if (!IsPostBack)
                    {
                        int reportId = Convert.ToInt32(Request.QueryString["reportId"]);

                        Guid tempPayloadId;
                        if (!Guid.TryParse(Request.QueryString["tempPayloadId"], out tempPayloadId))
                        {
                            throw new Exception("error while parsing tempPayloadId");
                        }

                        VRTempPayloadManager vrTempPayloadManager = new VRTempPayloadManager();
                        var payload = vrTempPayloadManager.GetVRTempPayload(tempPayloadId);
                        var parameters = payload.Settings as ReportParameters;

                        ReportDefinitionManager managerReport = new ReportDefinitionManager();
                        RDLCReportDefinition rdlc = managerReport.GetRDLCReportDefinition(reportId);

                        ReportDefinitionRDLCFile rdlcFile = rdlc.ReportDefinitionRDLCFiles.FindRecord(x => x.ReportDefinitionRDLCFileId == parameters.ReportDefinitionRDLCFileId);
                        if (rdlcFile == null)
                            rdlcFile = rdlc.ReportDefinitionRDLCFiles.First();

                        parameters.RDLCFileTitle = rdlcFile.Title;
                        ReportViewer1.ProcessingMode = ProcessingMode.Local;
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath(rdlcFile.RDLCURL);

                        IReportGenerator r = rdlc.GetReportGenerator();

                        ReportViewer1.LocalReport.DataSources.Clear();
                        foreach (var a in r.GenerateDataSources(parameters))
                        {
                            ReportDataSource ds = new ReportDataSource(a.Key, a.Value);
                            ReportViewer1.LocalReport.DataSources.Add(ds);
                        }
                        List<ReportParameter> BillingRDLCReportParameters = new List<ReportParameter>();
                        foreach (var p in r.GetRdlcReportParameters(parameters))
                        {
                            BillingRDLCReportParameters.Add(new ReportParameter(p.Key, p.Value.Value, p.Value.IsVisible));
                        }
                        ReportViewer1.LocalReport.SetParameters(BillingRDLCReportParameters.ToArray());
                        ReportViewer1.LocalReport.DisplayName = managerReport.GetRdlcDownloadedFileName(parameters.RDLCFileTitle,parameters.FromTime,parameters.ToTime);
                    }
                }
                else
                    throw new Exception("you are not authorized to perform this request");
            }
            catch (Exception ex)
            {
                Vanrise.Common.LoggerFactory.GetExceptionLogger().WriteException(ex);
                throw;
            }

            

        }
        protected override void Render(HtmlTextWriter writer)
        {
            try
            {
                base.Render(writer);
                GC.Collect();
            }
            catch (Exception ex)
            {
                Vanrise.Common.LoggerFactory.GetExceptionLogger().WriteException(ex);
                throw;
            }
        }
    }
}