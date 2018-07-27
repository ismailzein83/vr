using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class CDRCorrelationBatch
    {
        public List<dynamic> OutputRecordsToInsert { get; set; }

        public DeleteRecordsBatch DeleteRecordsBatch { get; set; }

        public CDRCorrelationBatch()
        {
            OutputRecordsToInsert = new List<dynamic>();
            DeleteRecordsBatch = new DeleteRecordsBatch();
        }
    }
}