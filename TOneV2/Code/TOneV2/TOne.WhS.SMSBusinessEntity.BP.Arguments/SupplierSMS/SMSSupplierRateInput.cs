using TOne.WhS.BusinessEntity.Business;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SMSBusinessEntity.BP.Arguments
{
    public class SMSSupplierRateInput : BaseProcessInputArgument
    {
        public int SupplierID { get; set; }

        public long ProcessDraftID { get; set; }

        public override string GetTitle()
        {
            string supplierName = new CarrierAccountManager().GetCarrierAccountName(this.SupplierID);
            return $"#BPDefinitionTitle# for Supplier '{supplierName}'";
        }

        public override string EntityId
        {
            get { return $"SupplierId_{this.SupplierID}"; }
        }
    }
}