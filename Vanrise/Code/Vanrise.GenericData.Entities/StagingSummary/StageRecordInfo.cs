using System;
using Vanrise.Reprocess.Entities;

namespace Vanrise.GenericData.Entities
{
   public class StageRecordInfo : BatchRecord
    {
       public DateTime BatchStart { get; set; }
        public override string GetBatchTitle()
        {
            return string.Format("Batch Start: {0}", BatchStart);
        }
    }
}
