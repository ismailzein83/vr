using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticReportSettings:Vanrise.Security.Entities.ViewSettings
    {

        public override string GetURL(Security.Entities.View view)
        {
            return String.Format("#/viewwithparams/Analytic/Views/GenericAnalytic/Runtime/GenericAnalyticReport/{{\"viewId\":\"{0}\"}}", view.ViewId);
        }
        public List<int> AnalyticTableIds { get; set; }

        public AnalyticReportSearchSettings  SearchSettings { get; set; }

        public List<AnalyticReportWidget> Widgets { get; set; }
    }
}
