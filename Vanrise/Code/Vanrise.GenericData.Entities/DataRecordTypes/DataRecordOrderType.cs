using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
namespace Vanrise.GenericData.Entities
{
    public enum OrderType { ByAllFields = 1, AdvancedFieldOrder = 2, ByAllFieldsDescending = 3 }

    public abstract class AdvancedOrderOptionsBase
    {
        public virtual List<string> GetAdditionalFieldNames()
        {
            return null;
        }
    }

    public class AdvancedFieldOrderOptions : AdvancedOrderOptionsBase
    {
        public List<AdvancedFieldOrderItem> Fields { get; set; }
        public override List<string> GetAdditionalFieldNames()
        {
            if(Fields != null && Fields.Count > 0)
            {
                return Fields.MapRecords(x => x.FieldName).ToList();
            }
            return null;
        }

    }
    public class AdvancedFieldOrderItem
    {
        public string FieldName { get; set; }

        public OrderDirection OrderDirection { get; set; }
    }

}
