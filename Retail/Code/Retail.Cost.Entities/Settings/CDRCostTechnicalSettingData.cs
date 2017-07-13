using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Reprocess.Entities;

namespace Retail.Cost.Entities
{
    public class CDRCostTechnicalSettingData : Vanrise.Entities.SettingData 
    {
        public Guid CostCDRReprocessDefinitionId { get; set; }

        public ReprocessChunkTimeEnum ChunkTime { get; set; }

        public Guid DataRecordTypeId { get; set; }

        public RecordFilterGroup FilterGroup { get; set; }
    }  
}
