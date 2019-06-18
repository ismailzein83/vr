using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public abstract class GenericInvoiceSettings : InvoiceTypeExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("83AF8B88-EC79-442B-83FE-07C6D6BEBB9C"); }
        }
        public Guid? InvoiceTransactionTypeId { get; set; }
        public List<Guid> UsageToOverrideTransactionTypeIds { get; set; }
        public GenericFinancialAccountConfiguration Configuration { get; set; }
        public override IEnumerable<string> GetPartnerIds(IExtendedSettingsPartnerIdsContext context)
        {
            //WHSFinancialAccountInfoFilter financialAccountInfoFilter = new WHSFinancialAccountInfoFilter { InvoiceTypeId = context.InvoiceTypeId };
            //switch (context.PartnerRetrievalType)
            //{
            //    case PartnerRetrievalType.GetAll:
            //        break;
            //    case PartnerRetrievalType.GetActive:
            //        financialAccountInfoFilter.Status = VRAccountStatus.Active;
            //        break;
            //    case PartnerRetrievalType.GetInactive:
            //        financialAccountInfoFilter.Status = VRAccountStatus.InActive;
            //        break;
            //    default: throw new NotSupportedException(string.Format("PartnerRetrievalType {0} not supported.", context.PartnerRetrievalType));
            //}
            //var carriers = new GenericFinancialAccountManager(Configuration).GetFinancialAccountsInfo(financialAccountInfoFilter);
            //if (carriers == null)
            //    return null;
            //return carriers.Select(x => x.FinancialAccountId.ToString());
            throw new NotImplementedException();
        }
    }
}
