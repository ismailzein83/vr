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
        public bool UpdateModuleRank(Guid moduleId, Guid? parentId, int rank)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_Module_UpdateRank", moduleId, parentId,rank);
            return (recordesEffected > 0);
        }
        public bool AreModulesUpdated(ref object _updateHandle)
        {
            return base.IsDataUpdated("sec.[Module]", ref _updateHandle);
        }
        public bool AddModule(Entities.Module moduleObject)
        {

            int recordesEffected = ExecuteNonQuerySP("sec.sp_Module_Insert", moduleObject.ModuleId, moduleObject.Name, moduleObject.ParentId, moduleObject.DefaultViewId, moduleObject.AllowDynamic, Vanrise.Common.Serializer.Serialize(moduleObject.Settings));

            return (recordesEffected > 0);
        }

        public bool UpdateModule(Entities.Module moduleObject)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_Module_Update", moduleObject.ModuleId, moduleObject.Name, moduleObject.ParentId, moduleObject.DefaultViewId, moduleObject.AllowDynamic, Vanrise.Common.Serializer.Serialize(moduleObject.Settings));
            return (recordesEffected > 0);
        }
        #endregion

        #region Mappers
        Entities.Module ModuleMapper(IDataReader reader)
        {
            Entities.Module module = new Entities.Module
            {
                ModuleId =GetReaderValue<Guid>(reader,"Id"),
                Name = reader["Name"] as string,
                Url = reader["Url"] as string,
                DefaultViewId = GetReaderValue<Guid?>(reader, "DefaultViewId"),
                ParentId = GetReaderValue<Guid?>(reader, "ParentId"),
                Icon = reader["Icon"] as string,
                AllowDynamic = true,// GetReaderValue<Boolean>(reader, "AllowDynamic"),
                Rank = GetReaderValue<int>(reader, "Rank"),
                Settings = Vanrise.Common.Serializer.Deserialize<ModuleSettings>(reader["Settings"] as string),
            };
            return module;
        }
        #endregion



       
    }
}
