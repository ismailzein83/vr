using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class VRNamespaceItemDataManager : BaseSQLDataManager, IVRNamespaceItemDataManager
    {
        #region ctor/Local Variables
        public VRNamespaceItemDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public List<VRNamespaceItem> GetVRNamespaceItems()
        {
            return GetItemsSP("common.sp_VRNamespaceItem_GetAll", VRNamespaceItemMapper);
        }

        public bool AreVRNamespaceItemUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.VRNamespaceItem", ref updateHandle);
        }

        public bool Insert(VRNamespaceItem vrNamespaceItem)
        {
            string serializedSettings = vrNamespaceItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vrNamespaceItem.Settings) : null;
            return ExecuteNonQuerySP("common.sp_VRNamespaceItem_Insert", vrNamespaceItem.VRNamespaceItemId, vrNamespaceItem.VRNamespaceId, vrNamespaceItem.Name, serializedSettings,
                vrNamespaceItem.CreatedBy, vrNamespaceItem.LastModifiedBy) > 0;
        }

        public bool Update(VRNamespaceItem vrNamespaceItem)
        {
            string serializedSettings = vrNamespaceItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vrNamespaceItem.Settings) : null;
            return ExecuteNonQuerySP("common.sp_VRNamespaceItem_Update", vrNamespaceItem.VRNamespaceItemId, vrNamespaceItem.VRNamespaceId, vrNamespaceItem.Name, serializedSettings,
                  vrNamespaceItem.LastModifiedBy) > 0;
        }

        #endregion

        #region Mappers

        VRNamespaceItem VRNamespaceItemMapper(IDataReader reader)
        {
            VRNamespaceItem vrNamespaceItem = new VRNamespaceItem
            {
                VRNamespaceItemId = (Guid)reader["ID"],
                VRNamespaceId = (Guid)reader["vrNamespaceId"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<VRNamespaceItemSettings>(reader["Settings"] as string),
                CreatedBy = (int)reader["CreatedBy"],
                LastModifiedBy = (int)reader["LastModifiedBy"]
            };
            return vrNamespaceItem;
        }

        #endregion
    }
}
