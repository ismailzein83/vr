using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Item
    {
        public long ItemId { get; set; }
        public string Name { get; set; }
        public long ProductId { get; set; }
        public ItemSettings Settings { get; set; }
    }


    public class ItemSettings
    {
        public ItemShape ItemShape { get; set; }
    }
    public abstract class ItemShape
    {
        public abstract Guid ConfigId { get; }
        public abstract string GetItemAreaDescription(IItemShapeDescriptionContext context);
    }
    public interface IItemShapeDescriptionContext
    {
        Item Item { get; }
    }
}
