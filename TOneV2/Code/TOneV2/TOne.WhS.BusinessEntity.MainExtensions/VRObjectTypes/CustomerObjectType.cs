using System;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class CustomerObjectType : VRObjectType
    {
        public override Guid ConfigId
        {
            get { return new Guid("48E39E5B-58A2-4799-89B3-F54ED3C48807"); }
        }

        public override object CreateObject(IVRObjectTypeCreateObjectContext context)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierAccount account = carrierAccountManager.GetCarrierAccount((int)context.ObjectId);

            if (account == null)
                throw new DataIntegrityValidationException(string.Format("Customer not found for ID: '{0}'", context.ObjectId));

            if(account.AccountType==CarrierAccountType.Supplier)
                throw new DataIntegrityValidationException(string.Format("Carrier Account of ID '{0}' is not a Customer", context.ObjectId));

            return account;
        }
    }
}
