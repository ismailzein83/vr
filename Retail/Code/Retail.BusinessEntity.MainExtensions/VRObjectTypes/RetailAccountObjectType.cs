using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using Vanrise.Entities;

namespace Retail.BusinessEntity.MainExtensions.VRObjectTypes
{
    public class RetailAccountObjectType : VRObjectType
    {
        public override Guid ConfigId { get { return new Guid("1dd9cb13-ccbb-47ef-8514-6cca50aef298"); } }

        public Guid AccountBEDefinitionId { get; set; }

        public override object CreateObject(IVRObjectTypeCreateObjectContext context)
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            Account account = accountBEManager.GetAccount(AccountBEDefinitionId, (long)context.ObjectId);

            if (account == null)
                throw new DataIntegrityValidationException(string.Format("Account not found for AccountBEDefinitionId : '{0}' and Account ID: '{1}'", AccountBEDefinitionId, context.ObjectId));

            return account;
        }
    }
}
