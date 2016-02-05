using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataStore
    {
        public int DataStoreId { get; set; }

        public string Name { get; set; }

        public DataStoreSettings Settings { get; set; }
    }
}
