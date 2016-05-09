﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public interface IReportGenerator
    {
        Dictionary<string, IEnumerable> GenerateDataSources(ReportParameters parameters);

        Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters);
    }
}
