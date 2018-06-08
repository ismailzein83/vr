using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Newtonsoft.Json;

namespace Demo.Module.Data.SQL
{
    public class ItemDataManager : BaseSQLDataManager, IItemDataManager
    {
        #region Constructors
        public ItemDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }
        #endregion

        #region Public Methods
        public bool AreItemsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Items]", ref updateHandle);
        }

        public List<Item> GetItems()
        {
            return GetItemsSP("[dbo].[sp_Item2_GetAll]", ItemMapper);
        }

        public bool Insert(Item item, out long insertedId)
        {
            object id;
            
            string serializedItemSettings = null;
            if (item.Settings != null)
                serializedItemSettings = Vanrise.Common.Serializer.Serialize(item.Settings);

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Item2_Insert]", out id, item.Name, item.ProductId, serializedItemSettings);
            bool result = (nbOfRecordsAffected > 0);
            if (result)
                insertedId = (long)id;
            else
                insertedId = 0;
            return result;
        }

        public bool Update(Item item)
        {
            string serializedItemSettings = null;
            if (item.Settings != null)
                serializedItemSettings = Vanrise.Common.Serializer.Serialize(item.Settings);

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Item2_Update]", item.ItemId, item.Name, item.ProductId, serializedItemSettings);
            return (nbOfRecordsAffected > 0);
        }
       
        #endregion


        #region Mappers
        Item ItemMapper(IDataReader reader)
        {
            return new Item
            {
                ItemId = GetReaderValue<long>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name"),
                ProductId = GetReaderValue<long>(reader, "ProductId"),
                Settings = Vanrise.Common.Serializer.Deserialize<ItemSettings>(reader["Settings"] as string)

            };
        }
        #endregion
    }
}
