using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public enum OrderType { ByAllFields = 1, AdvancedFieldOrder = 2, ByAllFieldsDescending = 3 }

    public abstract class AdvancedOrderOptionsBase
    {
    }

    public class AdvancedFieldOrderOptions : AdvancedOrderOptionsBase
    {
        public List<AdvancedFieldOrderItem> Fields { get; set; }
  
    }
    public class AdvancedFieldOrderItem
    {
        public string FieldName { get; set; }

        public OrderDirection OrderDirection { get; set; }
    }

}
