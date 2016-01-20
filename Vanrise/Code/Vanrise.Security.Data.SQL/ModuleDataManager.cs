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
     
        #region ctor
        public ModuleDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public List<Entities.Module> GetModules()
        {
            return GetItemsSP("sec.sp_Module_GetAll", ModuleMapper);
        }
        public bool UpdateModuleRank(int moduleId, int rank)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_Module_UpdateRank", moduleId, rank);
            return (recordesEffected > 0);
        }
        public bool AreModulesUpdated(ref object _updateHandle)
        {
            return base.IsDataUpdated("sec.[Module]", ref _updateHandle);
        }

        #endregion

        #region Mappers
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
                AllowDynamic = GetReaderValue<Boolean>(reader, "AllowDynamic"),
                Rank = GetReaderValue<int>(reader, "Rank"),
            };
            return module;
        }
        #endregion

    }
}
