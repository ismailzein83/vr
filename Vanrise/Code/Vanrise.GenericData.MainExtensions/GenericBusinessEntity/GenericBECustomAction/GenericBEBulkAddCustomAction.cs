using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBECustomAction
{
    public class GenericBEBulkAddCustomAction : GenericBECustomActionSettings
    {
        public override Guid ConfigId => new Guid("82D729D1-A0B3-4C6C-BC35-A361947E78FD");
        public override string ActionTypeName => "BulkAddCustomAction";
        public string RangeVariableName { get; set; }
        public string RangeFieldName { get; set; }
        public VRGenericEditorDefinitionSetting Settings { get; set; }
    }
}
