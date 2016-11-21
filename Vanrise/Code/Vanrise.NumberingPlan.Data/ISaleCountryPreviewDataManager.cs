using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Data;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Data
{
    public interface ISaleCountryPreviewDataManager : IDataManager
    {
        long ProcessInstanceId { set; }
        IEnumerable<CountryPreview> GetFilteredCountryPreview(SPLPreviewQuery query);
    }
}
