using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class SaleAreaExclusiveSessionType : VRExclusiveSessionTypeExtendedSettings
    {

        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
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
