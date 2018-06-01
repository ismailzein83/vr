using Demo.Module.Entities;
using Demo.Module.Entities.Item;
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

        bool Insert(Item item, out int insertedId);

        bool Update(Item item);

        bool Delete(int Id);
    }
}
