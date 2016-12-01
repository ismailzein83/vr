using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class ProfileObjectType : VRObjectType
    {
        public override Guid ConfigId
        {
            get { return new Guid("08F128D1-67D6-45BD-B0DA-B6FA535FFD99"); }
        }
    }
}
