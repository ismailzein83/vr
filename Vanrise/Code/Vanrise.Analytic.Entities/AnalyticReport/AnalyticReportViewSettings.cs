using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticReportViewSettings : Vanrise.Security.Entities.ViewSettings
    {
        public Guid TypeId { get; set; }
        public Guid AnalyticReportId { get; set; }


        public override string GetURL(Security.Entities.View view)
        {
            return String.Format("#/viewwithparams/Analytic/Views/GenericAnalytic/Runtime/GenericAnalyticReport/{{\"analyticReportId\":\"{0}\"}}", this.AnalyticReportId);
        }

        public override bool DoesUserHaveAccess(Security.Entities.IViewUserAccessContext context)
        {
            var analyticReport = BEManagerFactory.GetManager<IAnalyticReportManager>().GetAnalyticReportById(this.AnalyticReportId);
            return analyticReport != null && analyticReport.Settings.DoesUserHaveAccess(context);
        }
    }
}
