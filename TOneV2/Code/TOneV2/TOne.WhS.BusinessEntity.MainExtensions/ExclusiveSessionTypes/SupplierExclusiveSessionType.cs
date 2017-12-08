using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class SupplierExclusiveSessionType : VRExclusiveSessionTypeExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("80453eca-c1b1-431d-8c56-9bb92ee0551c"); }
        }

        public override bool DoesUserHaveAdminAccess(IVRExclusiveSessionDoesUserHaveAdminAccessContext context)
        {
            throw new NotImplementedException();
        }

        public override bool DoesUserHaveTakeAccess(IVRExclusiveSessionDoesUserHaveTakeAccessContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetTargetName(IVRExclusiveSessionGetTargetNameContext context)
        {
            throw new NotImplementedException();
        }
    }
}
