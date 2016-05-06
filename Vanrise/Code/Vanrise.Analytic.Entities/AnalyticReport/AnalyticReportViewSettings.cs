﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticReportViewSettings : Vanrise.Security.Entities.ViewSettings
    {
        public int AnalyticReportId { get; set; }
        public int TypeId { get; set; }
        public override string GetURL(Security.Entities.View view)
        {
            return String.Format("#/viewwithparams/Analytic/Views/GenericAnalytic/Runtime/GenericAnalyticReport/{{\"analyticReportId\":\"{0}\"}}", this.AnalyticReportId);
        }
    }
}
