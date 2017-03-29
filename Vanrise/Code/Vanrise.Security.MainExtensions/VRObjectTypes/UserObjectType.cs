using System;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Security.MainExtensions.VRObjectTypes
{
    public class UserObjectType : VRObjectType
    {
        public override Guid ConfigId { get { return new Guid("45BB8E6B-D8A1-47E2-BB29-123B994F781A"); } }

        public override object CreateObject(IVRObjectTypeCreateObjectContext context)
        {
            UserManager userManager = new UserManager();
            User user = userManager.GetUserbyId((int)context.ObjectId);

            if (user == null)
                throw new DataIntegrityValidationException(string.Format("User not found for ID: '{0}'", context.ObjectId));

            return user;
        }
    }
}
