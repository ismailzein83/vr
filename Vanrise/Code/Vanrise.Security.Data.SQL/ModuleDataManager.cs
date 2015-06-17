using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Security.Data.SQL
{
    class ModuleDataManager : BaseSQLDataManager, IModuleDataManager
    {
        public List<Entities.Module> GetModules()
        {
            return GetItemsSP("sec.sp_Module_GetAll", ModuleMapper);
        }

        Entities.Module ModuleMapper(IDataReader reader)
        {
            Entities.Module module = new Entities.Module
            {
                ModuleId = (int)reader["Id"],
                Name = reader["Name"] as string,
                Url = reader["Url"] as string,
                ParentId = GetReaderValue<int>(reader, "ParentId"),
                Icon = reader["Icon"] as string
            };
            return module;
        }
    }
}
