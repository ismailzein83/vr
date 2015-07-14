using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class DataSourceAdapterType
    {
        public int AdapterTypeId { get; set; }

        public string Name { get; set; }

        public AdapterTypeInfo Info { get; set; }
    }

    public class AdapterTypeInfo
    {
        public string AdapterTemplateURL { get; set; }

        public string FQTN { get; set; }
    }
}
