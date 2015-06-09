using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Security.Data.SQL
{
    public class BusinessEntityManager : BaseSQLDataManager, IBusinessEntityDataManager
    {
        public List<Entities.BusinessEntity> GetEntities()
        {
            return GetItemsSP("sec.sp_BusinessEntity_GetEntities", EntityMapper);
        }

        Entities.BusinessEntity EntityMapper(IDataReader reader)
        {
            Entities.BusinessEntity module = new Entities.BusinessEntity
            {
                EntityId = (int)reader["Id"],
                Name = reader["Name"] as string,
                ModuleId = (int) reader["ModuleId"],
                PermissionOptions = Common.Serializer.Deserialize<List<string>>(reader["PermissionOptions"] as string)
            };
            return module;
        }
    }
}
