﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Newtonsoft.Json;

namespace Vanrise.GenericData.Business
{
    public class GenericFieldsActionAuditChangeInfoDefinition : VRActionAuditChangeInfoDefinition
    {
        public Guid? BusinessEntityDefinitionId { get; set; }
        private Dictionary<string, DataRecordField> _fieldTypes { get; set; }
        
      
        public Dictionary<string, DataRecordField> FieldTypes
        {
            get
            {
                if (_fieldTypes != null)
                    return _fieldTypes;
                else if (BusinessEntityDefinitionId.HasValue)
                {
                    var businessEntityDefinition = new BusinessEntityDefinitionManager().GetBusinessEntityDefinition(BusinessEntityDefinitionId.Value);
                    if (businessEntityDefinition != null && businessEntityDefinition.Settings != null)
                    {
                        return businessEntityDefinition.Settings.TryGetRecordTypeFields(new BEDefinitionSettingsTryGetRecordTypeFieldsContext { BEDefinition = businessEntityDefinition });
                    }
                }
                return null;
            }
        }

        public override string RuntimeEditor
        {
            get { return "vr-genericdata-genericfields-actionauditchange-runtime"; }
        }

        public override VRActionAuditChangeInfo ResolveChangeInfo(IVRActionAuditChangeInfoResolveChangeInfoContext context)
        {
            IGenericObject oldObjAsGeneric = context.OldObjectValue.CastWithValidate<IGenericObject>("context.OldObjectValue");
            IGenericObject newObjAsGeneric = context.NewObjectValue.CastWithValidate<IGenericObject>("context.NewObjectValue");

            var changeInfo = new GenericFieldsActionAuditChangeInfo { FieldChanges = new List<GenericFieldChangeInfo>() };




            foreach (var field in newObjAsGeneric.FieldValues)
            {
                var oldFieldValue = oldObjAsGeneric.FieldValues.GetRecord(field.Key);
                var fieldType = FieldTypes.GetRecord(field.Key);
                if (fieldType != null)
                {
                    if (!fieldType.Type.AreEqual(field.Value, oldFieldValue))
                    {
                        changeInfo.FieldChanges.Add(new GenericFieldChangeInfo
                        {
                            FieldName = field.Key,
                            NewValue = field.Value,
                            OldValue = oldFieldValue
                        });
                    }
                }
            }
            context.ChangeSummary = ResolveChangeSummary(changeInfo);
            if (changeInfo.FieldChanges.Count == 0)
                context.NothingChanged = true;
            return changeInfo;
        }

        private string ResolveChangeSummary(GenericFieldsActionAuditChangeInfo changeInfo)
        {
            string summary = null;
            if (changeInfo.FieldChanges != null && changeInfo.FieldChanges.Count > 0)
            {
                summary = "Field changed: ";
                bool addCamma = false;
                foreach (var item in changeInfo.FieldChanges)
                {
                    var fieldType = FieldTypes.GetRecord(item.FieldName);
                    if (fieldType != null)
                    {
                        if (addCamma)
                            summary += ",";
                        summary += string.Format("{0}", fieldType.Title);
                        addCamma = true;
                    }
                }
            }
            return summary;
        }

        public override string ObjectRuntimeEditor
        {
            get { return "vr-genericdata-genericfields-actionauditchange-objectruntime"; }
        }

        public override VRActionAuditChangeInfo ResolveChangeInfoToView(IVRActionAuditChangeInfoResolveChangeInfoContext context)
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
