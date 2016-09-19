using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.VRObjectTypes
{
    public class TextObjectType : VRObjectType
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("55CC79FE-4569-4D34-A1D2-49FAA6445979"); } }
    }
}
