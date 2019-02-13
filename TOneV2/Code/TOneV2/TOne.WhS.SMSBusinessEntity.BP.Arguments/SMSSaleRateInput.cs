using TOne.WhS.BusinessEntity.Business;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SMSBusinessEntity.BP.Arguments
{
    public class SMSSaleRateInput : BaseProcessInputArgument
    {
        public int CustomerID { get; set; }

        public long ProcessDraftID { get; set; }

        public override string GetTitle()
        {
            string customerName = new CarrierAccountManager().GetCarrierAccountName(this.CustomerID);
            return $"#BPDefinitionTitle# for Customer '{customerName}'";
        }

        public override string EntityId
        {
            get { return $"CustomerId_{this.CustomerID}"; }
        }
    }
}