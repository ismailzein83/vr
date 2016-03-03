using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.SQL
{
    public class ExtensibleBEItemDataManager : BaseSQLDataManager, IExtensibleBEItemDataManager
    {
        public ExtensibleBEItemDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }
   
        #region Public Methods
        public List<ExtensibleBEItem> GetExtensibleBEItems()
        {
            return GetItemsSP("genericdata.sp_ExtensibleBEItem_GetAll", ExtensibleBEItemMapper);
        }
        public bool AreExtensibleBEItemUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.ExtensibleBEItem", ref updateHandle);
        }
        public bool UpdateExtensibleBEItem(ExtensibleBEItem extensibleBEItem)
        {
            string serializedObj = null;
            if (extensibleBEItem != null)
            {
                serializedObj = Vanrise.Common.Serializer.Serialize(extensibleBEItem);
            }
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_ExtensibleBEItem_Update", extensibleBEItem.ExtensibleBEItemId, serializedObj);
            return (recordesEffected > 0);
        }
        public bool AddExtensibleBEItem(ExtensibleBEItem extensibleBEItem, out int extensibleBEItemId)
        {
            object extensibleBEItemID;
            string serializedObj = null;
            if (extensibleBEItem != null)
            {
                serializedObj = Vanrise.Common.Serializer.Serialize(extensibleBEItem);
            }
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_ExtensibleBEItem_Insert", out extensibleBEItemID, serializedObj);
            extensibleBEItemId = (recordesEffected > 0) ? (int)extensibleBEItemID : -1;

            return (recordesEffected > 0);
        }
        #endregion

        #region Mappers
        ExtensibleBEItem ExtensibleBEItemMapper(IDataReader reader)
        {
            ExtensibleBEItem extensibleBEItem = Vanrise.Common.Serializer.Deserialize<ExtensibleBEItem>(reader["Details"] as string);
            if (extensibleBEItem != null)
            {
                extensibleBEItem.ExtensibleBEItemId = Convert.ToInt32(reader["ID"]);
            }
            return extensibleBEItem;
        }

        #endregion
    }
}
