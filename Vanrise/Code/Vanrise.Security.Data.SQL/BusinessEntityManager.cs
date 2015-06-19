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
    public class BusinessEntityManager : BaseSQLDataManager, IBusinessEntityDataManager
    {
        public BusinessEntityManager() : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }
        public List<Entities.BusinessEntity> GetEntities()
        {
            return GetItemsSP("sec.sp_BusinessEntity_GetAll", EntityMapper);
        }

        public bool ToggleBreakInheritance(string entityId)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_BusinessEntity_ToggleBreakInheritance", entityId);
            return (recordesEffected > 0);
        }

        Entities.BusinessEntity EntityMapper(IDataReader reader)
        {
            Entities.BusinessEntity module = new Entities.BusinessEntity
            {
                EntityId = (int)reader["Id"],
                Name = reader["Name"] as string,
                ModuleId = (int) reader["ModuleId"],
                BreakInheritance = (bool) reader["BreakInheritance"],
                PermissionOptions = Common.Serializer.Deserialize<List<string>>(reader["PermissionOptions"] as string)
            };
            return module;
        }
    }
}
