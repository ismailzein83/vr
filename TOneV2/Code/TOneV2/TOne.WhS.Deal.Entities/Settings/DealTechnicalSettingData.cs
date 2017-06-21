using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Reprocess.Entities;

namespace TOne.WhS.Deal.Entities
{
    public class DealTechnicalSettingData
    {
        public Guid ReprocessDefinitionId { get; set; }

        public ReprocessChunkTimeEnum ChunkTime { get; set; }
    }
}
