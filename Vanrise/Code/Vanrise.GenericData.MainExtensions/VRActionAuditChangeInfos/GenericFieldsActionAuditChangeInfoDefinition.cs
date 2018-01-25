using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.MainExtensions.VRActionAuditChangeInfos
{
    public class GenericFieldsActionAuditChangeInfoDefinition : VRActionAuditChangeInfoDefinition
    {
        public Dictionary<string, DataRecordFieldType> FieldTypes { get; set; }

        public override string RuntimeEditor
        {
            get;
            set;
        }

        public override VRActionAuditChangeInfo ResolveChangeInfo(IVRActionAuditChangeInfoResolveChangeInfoContext context)
        {
            IGenericObject oldObjAsGeneric = context.OldObjectValue.CastWithValidate<IGenericObject>("context.OldObjectValue");
            IGenericObject newObjAsGeneric = context.NewObjectValue.CastWithValidate<IGenericObject>("context.NewObjectValue");

            var changeInfo = new GenericFieldsActionAuditChangeInfo { FieldChanges = new List<GenericFieldChangeInfo>() };
            throw new NotImplementedException();
            context.ChangeSummary = ResolveChangeSummary(changeInfo);
            return changeInfo;
        }

        private string ResolveChangeSummary(GenericFieldsActionAuditChangeInfo changeInfo)
        {
            throw new NotImplementedException();
        }
    }

    public class GenericFieldsActionAuditChangeInfo : VRActionAuditChangeInfo
    {
        public List<GenericFieldChangeInfo> FieldChanges { get; set; }
    }

    public class GenericFieldChangeInfo
    {
        public string FieldName { get; set; }

        public object OldValue { get; set; }

        public object NewValue { get; set; }
    }
}
