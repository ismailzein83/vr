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
            return GetItemsSP("sec.sp_Module_GetModules", ModuleMapper);
        }

        Entities.Module ModuleMapper(IDataReader reader)
        {
            Entities.Module module = new Entities.Module
            {
                ModuleId = (int)reader["Id"],
                Name = reader["Name"] as string,
                Url = reader["Url"] as string,
                Parent = GetReaderValue<int>(reader, "Parent"),
                Icon = reader["Icon"] as string
            };
            return module;
        }
    }
}
