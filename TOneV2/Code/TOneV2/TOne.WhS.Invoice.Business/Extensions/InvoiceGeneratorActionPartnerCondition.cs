using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            string partner = context.generateInvoiceInput.PartnerId;
            string[] partnerArr = partner.Split('_');
            switch (partnerArr[0])
            {
                case "Profile": return this.CarrierType == CarrierType.Profile;
                case "Account": return this.CarrierType == CarrierType.Account;
            }
            return false;
        }
    }
}
