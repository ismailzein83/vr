using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class BusinessEntitySettingsData : SettingData
    {
        public const string BusinessEntityTechnicalSettings = "WhS_BE_Settings";


    }

    public class CachingExpirationIntervals
    {
        public int? TodayEntites { get; set; }

        public int? PreviousEntites { get; set; }
    }
}
