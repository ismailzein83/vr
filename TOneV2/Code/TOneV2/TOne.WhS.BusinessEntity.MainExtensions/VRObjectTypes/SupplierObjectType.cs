using System;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class SupplierObjectType : VRObjectType
    {
        public override Guid ConfigId
        {
            get { return new Guid("374D9D90-0663-47BD-9E7D-E9693330F6B9"); }
        }

        public override object CreateObject(IVRObjectTypeCreateObjectContext context)
        {
            dynamic objectId = context.ObjectId;
            if (objectId == null)
                return null;

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierAccount account = carrierAccountManager.GetCarrierAccount((int)objectId);

            if (account == null)
                throw new DataIntegrityValidationException(string.Format("Customer not found for ID: '{0}'", context.ObjectId));

            if (account.AccountType == CarrierAccountType.Customer)
                throw new DataIntegrityValidationException(string.Format("Carrier Account of ID '{0}' is not a Supplier", context.ObjectId));

            return account;
        }
    }
}