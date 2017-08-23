using System;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class WHSFinancialAccountObjectType : VRObjectType
    {
        public override Guid ConfigId
        {
            get { return new Guid("8837F293-1465-46E0-9792-950856AB2445"); }
        }

        public override object CreateObject(IVRObjectTypeCreateObjectContext context)
        {
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            WHSFinancialAccount financialAccount = financialAccountManager.GetFinancialAccount((int)context.ObjectId);

            if (financialAccount == null)
                throw new DataIntegrityValidationException(string.Format("FinancialAccount not found for ID: '{0}'", context.ObjectId));
            return financialAccount;
        }
    }
}
