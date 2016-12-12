﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public abstract class ItemsFilter
    {
        public abstract Guid ConfigId { get; }
        public abstract IEnumerable<dynamic> GetFilteredItems(IItemsFilterContext context);
    }
    public interface IItemsFilterContext
    {
        dynamic ParentItem { get; }
        IEnumerable<dynamic> Items { get; }
    }
}
