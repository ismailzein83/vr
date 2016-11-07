using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.SQL
{
    public class BusinessEntityDataManager : BaseSQLDataManager, IBusinessEntityDataManager
    {
        public BusinessEntityDataManager() : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }
        
        public IEnumerable<BusinessEntity> GetEntities()
        {
            return GetItemsSP("sec.sp_BusinessEntity_GetAll", EntityMapper);
        }

        public bool ToggleBreakInheritance(Guid entityId)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_BusinessEntity_ToggleBreakInheritance", entityId);
            return (recordesEffected > 0);
        }

        public bool AreBusinessEntitiesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("sec.BusinessEntity", ref updateHandle);
        }
        public bool AddBusinessEntity(BusinessEntity businessEntity)
        {
            string sertializedObject = null;
            if(businessEntity.PermissionOptions !=null)
            {
                sertializedObject = Vanrise.Common.Serializer.Serialize(businessEntity.PermissionOptions, true);
            }
            object entityID;

            int recordesEffected = ExecuteNonQuerySP("sec.sp_BusinessEntity_Insert", businessEntity.EntityId, businessEntity.Name, businessEntity.Title, businessEntity.ModuleId, businessEntity.BreakInheritance, sertializedObject);

            return (recordesEffected > 0);
        }

        public bool UpdateBusinessEntity(BusinessEntity businessEntity)
        {
            string sertializedObject = null;
            if (businessEntity.PermissionOptions != null)
            {
                sertializedObject = Vanrise.Common.Serializer.Serialize(businessEntity.PermissionOptions, true);
            }
            int recordesEffected = ExecuteNonQuerySP("sec.sp_BusinessEntity_Update", businessEntity.EntityId, businessEntity.Name,businessEntity.Title, businessEntity.ModuleId, businessEntity.BreakInheritance, sertializedObject);
            return (recordesEffected > 0);
        }



        public bool UpdateBusinessEntityRank(Guid entityId, Guid moduleId)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_BusinessEntity_UpdateModule", entityId, moduleId);
            return (recordesEffected > 0);
        }

        #region Mappers

        Entities.BusinessEntity EntityMapper(IDataReader reader)
        {
            Entities.BusinessEntity module = new Entities.BusinessEntity
            {
                EntityId = GetReaderValue<Guid>(reader,"Id"),
                Name = reader["Name"] as string,
                Title = reader["Title"] as string,
                ModuleId =  GetReaderValue<Guid>(reader,"ModuleId"),
                BreakInheritance = (bool)reader["BreakInheritance"],
                PermissionOptions = Common.Serializer.Deserialize<List<string>>(reader["PermissionOptions"] as string)
            };
            return module;
        }
        
        #endregion



    }
}
