using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class BusinessEntityTechnicalSettingsData : SettingData
    {
        public const string BusinessEntityTechnicalSettings = "WhS_BE_TechnicalSettings";

        public RateTypeConfiguration RateTypeConfiguration { get; set; }

        public IEnumerable<StateBackupCleanupTask> Tasks { get; set; }
    }
}
