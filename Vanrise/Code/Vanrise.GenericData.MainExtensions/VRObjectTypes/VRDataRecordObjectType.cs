using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.MainExtensions.VRObjectTypes
{
    public class VRDataRecordObjectType : VRObjectType
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("BBC57155-0412-4371-83E5-1917A8BEA468"); } }
        public int RecordTypeId { get; set; }
    }
}
