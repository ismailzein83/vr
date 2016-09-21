using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace CP.SupplierPricelist.Entities
{
    public class CustomerConnectorConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "CP_SupplierPriceList_CustomerConnector";
        public string Editor { get; set; }
    }
}
