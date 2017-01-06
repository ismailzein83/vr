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
        public override Guid ConfigId { get { return  new Guid("C61DE7AE-34D3-457B-9C00-78F002AF508B"); } }
        public string FieldName { get; set; }
        public string ComparedFieldName { get; set; }
        public override IEnumerable<dynamic> GetFilteredItems(IItemsFilterContext context)
        {
            var fieldValue = context.ParentItem.Details.GetType().GetProperty(FieldName).GetValue(context.ParentItem, null);// Utilities.GetPropValueReader(FieldName).GetPropertyValue(context.ParentItem);
            return context.Items.Where(x => x.GetType().GetProperty(ComparedFieldName).GetValue(x, null) == fieldValue);
        }
    }
}
