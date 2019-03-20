using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PointOfInterconnectRecordFilter : StringRecordFilter
    {
        public override string GetDescription(IRecordFilterGetDescriptionContext context)
        {
            return string.Format("{0} {1} {2}", context.GetFieldTitle(FieldName), Utilities.GetEnumDescription(CompareOperator),Value);
        }

    }

}
