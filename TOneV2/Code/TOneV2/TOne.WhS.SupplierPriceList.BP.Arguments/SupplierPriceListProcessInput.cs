using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Arguments
{
    public class SupplierPriceListProcessInput : BaseProcessInputArgument
    {
        public int SupplierAccountId { get; set; }

        public long FileId { get; set; }

        public int CurrencyId { get; set; }

        public DateTime PriceListDate { get; set; }


        public override string GetTitle()
        {
            return String.Format("SupplierPriceList Process Started for Supplier: {0}", SupplierAccountId);
        }
    }
}
