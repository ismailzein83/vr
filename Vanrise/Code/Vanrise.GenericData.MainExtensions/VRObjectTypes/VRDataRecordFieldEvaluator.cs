using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.MainExtensions.VRObjectTypes
{
    public class VRDataRecordFieldEvaluator : VRObjectPropertyEvaluator
    {
        public string FieldName { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            //VRDataRecordObjectType dataRecordObjectType = context.ObjectType as VRDataRecordObjectType;
            //if (dataRecordObjectType == null)
            //    throw new NullReferenceException("dataRecordObjectType");
            //int dataRecordTypeId = dataRecordObjectType.RecordTypeId;

            return Common.Utilities.GetPropValueReader(this.FieldName).GetPropertyValue(context.Object);           
        }
    }
}
