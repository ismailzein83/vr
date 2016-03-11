using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataStoreConfig
    {
        public int DataStoreConfigId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Editor { get; set; }

        public string DataRecordSettingsEditor { get; set; }

        public string SummaryTransformationSettingsEditor { get; set; }
    }
}
