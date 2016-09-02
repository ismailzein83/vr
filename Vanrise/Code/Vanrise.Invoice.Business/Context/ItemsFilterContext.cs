using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class ItemsFilterContext : IItemsFilterContext
    {
        public dynamic ParentItem { get; set; }
        public List<dynamic> Items { get; set; }
    }
}
