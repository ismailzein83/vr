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
    public class EntityPersonalizationDataManager : BaseSQLDataManager, IEntityPersonalizationDataManager
    {

        #region ctor/Local Variables
        public EntityPersonalizationDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        #endregion

        #region Public Methods
        public List<EntityPersonalization> GetEntityPersonalizations()
        {
            return GetItemsSP("Common.sp_EntityPersonalization_GetAll", EntityPersonalizationMapper);
        }
        public bool AreEntityPersonalizationUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Common.EntityPersonalization", ref updateHandle);
        }
        public bool Save(EntityPersonalization entityPersonalization)
        {
            string serializedDetails = entityPersonalization.Setting != null ? Vanrise.Common.Serializer.Serialize(entityPersonalization.Setting) : null;
            int affectedRecords = ExecuteNonQuerySP("Common.[sp_EntityPersonalization_InsertIfNotExistsOrUpdate]", entityPersonalization.EntityUniqueName, entityPersonalization.UserId, serializedDetails, entityPersonalization.CreatedBy);

            if (affectedRecords > 0)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region Mappers
        EntityPersonalization EntityPersonalizationMapper(IDataReader reader)
        {
            EntityPersonalization EntityPersonalization = new EntityPersonalization
            {
                EntityPersonalizationId = (long)reader["ID"],
                UserId = GetReaderValue<int?>(reader, "UserId"),
                EntityUniqueName = reader["EntityUniqueName"] as string,
                Setting = reader["Details"] as string != null ? Vanrise.Common.Serializer.Deserialize<EntityPersonalizationExtendedSetting>(reader["Details"] as string) : null,
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                CreatedBy = GetReaderValue<int?>(reader, "CreatedBy"),
                LastModifiedBy = GetReaderValue<int?>(reader, "LastModifiedBy"),
                LastModifiedTime = GetReaderValue<DateTime?>(reader, "LastModifiedTime"),
            };
            return EntityPersonalization;
        }

        #endregion
    }
}
