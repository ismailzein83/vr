using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.InvToAccBalanceRelation.Entities
{
    public abstract class InvToAccBalanceRelationDefinitionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("53E07D3A-3D37-4F81-BAD0-225713E41B9A"); }
        }

        public InvToAccBalanceRelationDefinitionExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class InvToAccBalanceRelationDefinitionExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract List<InvoiceAccountInfo> GetBalanceInvoiceAccounts(IInvToAccBalanceRelGetBalanceInvoiceAccountsContext context);

        public abstract List<BalanceAccountInfo> GetInvoiceBalanceAccounts(IInvToAccBalanceRelGetInvoiceBalanceAccountsContext context);
    }

    public interface IInvToAccBalanceRelGetBalanceInvoiceAccountsContext
    {
        Guid AccountTypeId { get; }

        string AccountId { get; }

        DateTime EffectiveOn { get; }
    }

    public interface IInvToAccBalanceRelGetInvoiceBalanceAccountsContext
    {
        Guid InvoiceTypeId { get; }

        string PartnerId { get; }

        DateTime EffectiveOn { get; }
    }
}
