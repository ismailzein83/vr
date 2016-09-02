using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class GenericItemsFilter : ItemsFilter
    {
        public string FieldName { get; set; }
        public string ComparedFieldName { get; set; }
        public override IEnumerable<dynamic> GetFilteredItems(IItemsFilterContext context)
        {
            return context.Items.Where(x=> Utilities.GetPropValueReader(ComparedFieldName).GetPropertyValue(x) ==  Utilities.GetPropValueReader(FieldName).GetPropertyValue(context.ParentItem));
        }
    }
}
