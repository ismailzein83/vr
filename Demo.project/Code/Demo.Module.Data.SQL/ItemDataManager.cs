using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Newtonsoft.Json;
using Demo.Module.Entities.Item;

namespace Demo.Module.Data.SQL
{
    public class ItemDataManager : BaseSQLDataManager, IItemDataManager
    {
          public ItemDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public bool AreItemsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Items]", ref updateHandle);
        }

        public List<Item> GetItems()
        {
            return GetItemsSP("[dbo].[sp_Item2_GetAll]", ItemMapper);
        }

        public bool Insert(Item item, out int insertedId)
        {
            //string infoSerializedString = null;
            //if (item.ItemInfo != null)
            //    infoSerializedString = Vanrise.Common.Serializer.Serialize(item.ItemInfo);

            //string descriptionSerializedString = null;
            //if (item.DescriptionString != null)
            //{
            //    descriptionSerializedString = Vanrise.Common.Serializer.Serialize(college.DescriptionString);
            //}

            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Item2_Insert]", out id, item.Name, item.ProductId);
            insertedId = Convert.ToInt32(id);
            return (nbOfRecordsAffected > 0);
        }

        public bool Update(Item item)
        {
            //string infoSerializedString = null;
            //if (college.CollegeInfo != null)
            //    infoSerializedString = Vanrise.Common.Serializer.Serialize(college.CollegeInfo);

            //string descriptionSerializedString = null;
            //if (college.DescriptionString != null)
            //    descriptionSerializedString = Vanrise.Common.Serializer.Serialize(college.DescriptionString);

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Item2_Update]", item.ItemId, item.Name, item.ProductId);
            return (nbOfRecordsAffected > 0);
        }

        public bool Delete(int collegeId)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Item_Delete]", collegeId);
            return (nbOfRecordsAffected > 0);
        }

        public bool AreCollegesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Items]", ref updateHandle);
        }

        Item ItemMapper(IDataReader reader)
        {
            return new Item
            {
                ItemId = GetReaderValue<int>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name"),
                ProductId = GetReaderValue<int>(reader, "ProductId"),
            };
        }
    }
}
