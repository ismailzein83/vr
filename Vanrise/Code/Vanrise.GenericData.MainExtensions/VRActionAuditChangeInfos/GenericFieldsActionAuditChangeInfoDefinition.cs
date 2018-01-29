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
    //public class GenericFieldsActionAuditChangeInfoDefinition : VRActionAuditChangeInfoDefinition
    //{
    //    public Dictionary<string, DataRecordFieldType> FieldTypes { get; set; }

    //    public override string RuntimeEditor
    //    {
    //        get;
    //        set;
    //    }

    //    public override VRActionAuditChangeInfo ResolveChangeInfo(IVRActionAuditChangeInfoResolveChangeInfoContext context)
    //    {
    //        IGenericObject oldObjAsGeneric = context.OldObjectValue.CastWithValidate<IGenericObject>("context.OldObjectValue");
    //        IGenericObject newObjAsGeneric = context.NewObjectValue.CastWithValidate<IGenericObject>("context.NewObjectValue");

    //        var changeInfo = new GenericFieldsActionAuditChangeInfo { FieldChanges = new List<GenericFieldChangeInfo>() };
    //        foreach (var field in oldObjAsGeneric.FieldValues)
    //        {
    //            var newFieldValue = newObjAsGeneric.FieldValues.GetRecord(field.Key);
    //            var fieldType = FieldTypes.GetRecord(field.Key);
    //            if (fieldType != null)
    //            {
    //                if(!fieldType.AreEqual(newFieldValue, field.Value))
    //                {
    //                    changeInfo.FieldChanges.Add(new GenericFieldChangeInfo
    //                    {
    //                        FieldName = field.Key,
    //                        NewValue =newFieldValue ,
    //                        OldValue = field.Value
    //                    });
    //                }
    //            }
    //        }
    //        context.ChangeSummary = ResolveChangeSummary(changeInfo);
    //        return changeInfo;
    //    }

    //    private string ResolveChangeSummary(GenericFieldsActionAuditChangeInfo changeInfo)
    //    {
    //        string summary = null;
    //        if(changeInfo.FieldChanges != null && changeInfo.FieldChanges.Count > 0)
    //        {
    //            var numberOfChanges = 0;
    //            foreach(var item in changeInfo.FieldChanges)
    //            {
    //                if (summary == null)
    //                    summary =  string.Format("{0} changed from '{1}' to '{2}'", item.FieldName, item.OldValue, item.NewValue);
    //                else
    //                {
    //                    numberOfChanges++;
    //                }

    //            }
    //            if (numberOfChanges > 0)
    //            {
    //                summary += string.Format("and made  {0} other changes.", numberOfChanges);
    //            }
    //        }
    //        return summary;
    //    }
    //}

    //public class GenericFieldsActionAuditChangeInfo : VRActionAuditChangeInfo
    //{
    //    public List<GenericFieldChangeInfo> FieldChanges { get; set; }
    //}

    //public class GenericFieldChangeInfo
    //{
    //    public string FieldName { get; set; }

    //    public object OldValue { get; set; }

    //    public object NewValue { get; set; }
    //}
}
