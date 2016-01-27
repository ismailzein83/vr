using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CP.SupplierPricelist.Entities;

namespace CP.SupplierPricelist.Business.PriceListTasks
{
    public class UploadPriceListTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        public SupplierPriceListConnectorBase SupplierPriceListConnectorBase { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
