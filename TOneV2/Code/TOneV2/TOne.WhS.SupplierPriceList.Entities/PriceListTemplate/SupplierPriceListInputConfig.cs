using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public class SupplierPriceListInputConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_SupPL_SupplierPriceListConfiguration";
        public string Editor { get; set; }

    }
}
