using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.SupplierPricelist.Entities
{
    public class CustomerTypeConfig
    {
        public int CustomerTypeConfigId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string SettingsEditor { get; set; }
    }
}
