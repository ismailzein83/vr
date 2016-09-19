using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Security.MainExtensions.VRObjectTypes
{
    public class UserObjectType : VRObjectType
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("45BB8E6B-D8A1-47E2-BB29-123B994F781A"); } }
    }
}
