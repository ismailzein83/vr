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
        public ModuleDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

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
                Title = reader["Title"] as string,
                Url = reader["Url"] as string,
                ParentId = GetReaderValue<int>(reader, "ParentId"),
                Icon = reader["Icon"] as string,
                AllowDynamic = GetReaderValue<int>(reader, "AllowDynamic"),
            };
            return module;
        }
    }
}
