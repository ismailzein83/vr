using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public abstract class ItemsFilter
    {
        public Guid ConfigId { get; set; }
        public abstract IEnumerable<dynamic> GetFilteredItems(IItemsFilterContext context);
    }
    public interface IItemsFilterContext
    {
        dynamic ParentItem { get; }
        List<dynamic> Items { get; }
    }
}
