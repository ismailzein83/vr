using System;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class ProfileObjectType : VRObjectType
    {
        public override Guid ConfigId
        {
            get { return new Guid("08F128D1-67D6-45BD-B0DA-B6FA535FFD99"); }
        }

        public override object CreateObject(IVRObjectTypeCreateObjectContext context)
        {
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            CarrierProfile carrierProfile = carrierProfileManager.GetCarrierProfile((int)context.ObjectId);

            if (carrierProfile == null)
                throw new DataIntegrityValidationException(string.Format("Carrier Profile not found for ID: '{0}'", context.ObjectId));
            
            return carrierProfile;
        }
    }
}
