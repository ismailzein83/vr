using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Data
{
    public interface IVariationReportDataManager : IDataManager
    {
        IEnumerable<VariationReportRecord> GetFilteredVariationReportRecords(DataRetrievalInput<VariationReportQuery> input, DataTable timePeriodsTable);
    }
}
