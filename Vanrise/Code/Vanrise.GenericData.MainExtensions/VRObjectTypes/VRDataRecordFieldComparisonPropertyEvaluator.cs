using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.MainExtensions.VRObjectTypes
{
    public enum DataRecordFieldComparisonOperator
    {
        [Description(" = ")]
        Equals = 0
    };

    public class VRDataRecordFieldComparisonPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId { get { return new Guid("FE0EE225-6893-410F-8095-1834DB99D7B7"); } }

        public string SourceFieldName { get; set; }
        public string TargetFieldName { get; set; }
        public DataRecordFieldComparisonOperator Operator { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            var sourceFieldValue = Common.Utilities.GetPropValueReader(this.SourceFieldName).GetPropertyValue(context.Object);
            var targetFieldValue = Common.Utilities.GetPropValueReader(this.TargetFieldName).GetPropertyValue(context.Object);

            if (sourceFieldValue == null || targetFieldValue == null)
                return null;

            if (this.Operator == 0)
            {
                if (sourceFieldValue == targetFieldValue)
                    return 1;
                return 2;
            }

            return null;
        }
    }
}
