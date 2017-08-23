using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public enum CarrierType { Profile = 0, Account = 1 }
    public class InvoiceGeneratorActionPartnerCondition : PartnerInvoiceFilterCondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("6F31ACAD-1FD8-4350-AC3B-1EEAE60A1D5D"); }
        }
        public CarrierType CarrierType { get; set; }
        public override bool IsFilterMatch(IPartnerInvoiceFilterConditionContext context)
        {
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.generateInvoiceInput.PartnerId));


            if (financialAccount.CarrierProfileId.HasValue)
            {
                return this.CarrierType == CarrierType.Profile;
            }else
            {
                return this.CarrierType == CarrierType.Account;
            }
        }
    }
}
