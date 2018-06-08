using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Business
{
    public class ItemShapeDescriptionContext : IItemShapeDescriptionContext
    {
        public Item Item { get; set; }
    }
}
