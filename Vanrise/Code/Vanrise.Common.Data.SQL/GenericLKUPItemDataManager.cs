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
   public class GenericLKUPItemDataManager :BaseSQLDataManager, IGenericLKUPItemDataManager
    {

        #region ctor/Local Variables
        public GenericLKUPItemDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnString", "ConfigurationDBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public List<GenericLKUPItem> GetGenericLKUPItem()
        {
            return GetItemsSP("Common.sp_GenericLKUP_GetAll", GenericLKUPItemMapper);
        }
        public bool AreGenericLKUPItemUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Common.GenericLKUP", ref updateHandle);
        }
        public bool Insert(GenericLKUPItem genericLKUPItem)
        {
             string serializedSettings = genericLKUPItem.Settings != null ? Vanrise.Common.Serializer.Serialize(genericLKUPItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Common.sp_GenericLKUP_Insert", genericLKUPItem.GenericLKUPItemId, genericLKUPItem.Name, genericLKUPItem.BusinessEntityDefinitionId, serializedSettings);
            return (affectedRecords > 0);

        }
        public bool Update(GenericLKUPItem genericLKUPItem)
        {
            string serializedSettings = genericLKUPItem.Settings != null ? Vanrise.Common.Serializer.Serialize(genericLKUPItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Common.sp_GenericLKUP_Update", genericLKUPItem.GenericLKUPItemId, genericLKUPItem.Name, genericLKUPItem.BusinessEntityDefinitionId, serializedSettings);
            return (affectedRecords > 0);
        }
        #endregion

        #region Mappers
        GenericLKUPItem GenericLKUPItemMapper(IDataReader reader)
        {
            GenericLKUPItem genericLKUPItem = new GenericLKUPItem
            {
                GenericLKUPItemId = (Guid)reader["ID"],
                Name = reader["Name"] as string,
                BusinessEntityDefinitionId = GetReaderValue<Guid>(reader, "BusinessEntityDefinitionID"),
                Settings = reader["Settings"] as string != null ? Vanrise.Common.Serializer.Deserialize<GenericLKUPItemSettings>(reader["Settings"] as string) : null,
            };
            return genericLKUPItem;
        }

        #endregion
    }
    
}
