using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.Cost.Entities
{
    public class CDRCostTechnicalSettingData : Vanrise.Entities.SettingData 
    {
        public Guid CostCDRReprocessDefinitionId { get; set; }

        public Guid DataRecordTypeId { get; set; }

        public RecordFilterGroup FilterGroup { get; set; }
    }  
}
