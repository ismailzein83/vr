using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class RatePlanSettingsData : SettingData
    {           
        public IEnumerable<CostCalculationMethod> CostCalculationsMethods { get; set; }

        public int TQIPeriodValue { get; set; }

        public PeriodTypeEnum TQIPeriodType { get; set; }

        public bool FollowMasterRatesBED { get; set; }
    }
}
