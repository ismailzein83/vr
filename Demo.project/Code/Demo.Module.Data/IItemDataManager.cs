using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface IItemDataManager : IDataManager
    {
        bool AreItemsUpdated(ref object updateHandle);
        List<Item> GetItems();
        bool Insert(Item item, out long insertedId);
        bool Update(Item item);
    }
}
