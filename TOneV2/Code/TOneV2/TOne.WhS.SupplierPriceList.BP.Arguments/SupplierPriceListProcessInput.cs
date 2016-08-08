using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Arguments
{
    public class SupplierPriceListProcessInput : BaseProcessInputArgument
    {
        public int SupplierAccountId { get; set; }

        public long FileId { get; set; }

        public int CurrencyId { get; set; }

        public DateTime PriceListDate { get; set; }

        public int SupplierPriceListTemplateId { get; set; }
        public override string GetTitle()
        {
            ICarrierAccountManager carrierAccountManager = BEManagerFactory.GetManager<ICarrierAccountManager>();
            string supplierName = carrierAccountManager.GetCarrierAccountName(SupplierAccountId);
            return String.Format("#BPDefinitionTitle# Process Started for Supplier: {0}", supplierName);
        }
    }
}
