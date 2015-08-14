﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Data
{
    public interface IHourlyReportDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<string> GetHourlyReportData(Vanrise.Entities.DataRetrievalInput<string> input);
    }
}
