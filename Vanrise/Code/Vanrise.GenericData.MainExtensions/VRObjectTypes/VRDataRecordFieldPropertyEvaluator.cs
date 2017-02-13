using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.MainExtensions.VRObjectTypes
{
    public class VRDataRecordFieldPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId { get { return new Guid("F663BF74-99DB-4746-8CBC-E74198E1786C"); } }
        public string FieldName { get; set; }
        public bool UseDescription { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            VRDataRecordObjectType dataRecordObjectType = context.ObjectType as VRDataRecordObjectType;
            if (dataRecordObjectType == null)
                throw new NullReferenceException("dataRecordObjectType");
             Guid dataRecordTypeId = dataRecordObjectType.RecordTypeId;
             
            var fieldValue = Common.Utilities.GetPropValueReader(this.FieldName).GetPropertyValue(context.Object);       
            if(this.UseDescription)
            {
                DataRecordTypeManager manager = new DataRecordTypeManager();
                var dataRecordField = manager.GetDataRecordField(dataRecordTypeId, this.FieldName);
                if(dataRecordField != null)
                {
                   return dataRecordField.Type.GetDescription(fieldValue);
                }
            }
            return fieldValue;
            
        }
    }
}
