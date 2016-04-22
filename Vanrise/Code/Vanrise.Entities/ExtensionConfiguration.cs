using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class ExtensionConfiguration
    {
        public int ExtensionConfigurationId { get; set; }

        public string Title { get; set; }        
    }

    public class ExtensionConfigurationEntity
    {
        public int ExtensionConfigurationId { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public Object Settings { get; set; }
    }
}
