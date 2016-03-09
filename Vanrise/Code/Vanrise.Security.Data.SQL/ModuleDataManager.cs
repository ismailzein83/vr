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
        public bool AddModule(Entities.Module moduleObject, out int moduleId)
        {
            object moduleID;

            int recordesEffected = ExecuteNonQuerySP("sec.sp_Module_Insert", out moduleID, moduleObject.Name, moduleObject.Title,moduleObject.ParentId, moduleObject.AllowDynamic);
            moduleId = (recordesEffected > 0) ? (int)moduleID : -1;

            return (recordesEffected > 0);
        }

        public bool UpdateModule(Entities.Module moduleObject)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_Module_Update", moduleObject.ModuleId, moduleObject.Name, moduleObject.Title,moduleObject.ParentId, moduleObject.AllowDynamic);
            return (recordesEffected > 0);
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
