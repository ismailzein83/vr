using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Security.Data.SQL
{
    public class BusinessEntityModuleDataManager : BaseSQLDataManager, IBusinessEntityModuleDataManager
    {
        public BusinessEntityModuleDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

        public IEnumerable<Entities.BusinessEntityModule> GetModules()
        {
            return GetItemsSP("sec.sp_BusinessEntityModule_GetAll", ModuleMapper);
        }

        public bool ToggleBreakInheritance(Guid entityId)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_BusinessEntityModule_ToggleBreakInheritance", entityId);
            return (recordesEffected > 0);
        }

        public bool AreBusinessEntityModulesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("sec.BusinessEntityModule", ref updateHandle);
        }
        public bool AddBusinessEntityModule(Entities.BusinessEntityModule moduleObject)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_BusinessEntityModule_Insert", moduleObject.ModuleId, moduleObject.Name, moduleObject.ParentId, moduleObject.BreakInheritance);
            return (recordesEffected > 0);
        }

        public bool UpdateBusinessEntityModule(Entities.BusinessEntityModule moduleObject)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_BusinessEntityModule_Update", moduleObject.ModuleId, moduleObject.Name, moduleObject.ParentId, moduleObject.BreakInheritance);
            return (recordesEffected > 0);
        }




        public bool UpdateBusinessEntityModuleRank(Guid moduleId, Guid? parentId)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_BusinessEntityModule_UpdateParent", moduleId, parentId);
            return (recordesEffected > 0);
        }
        #region Mappers

        Entities.BusinessEntityModule ModuleMapper(IDataReader reader)
        {
            Entities.BusinessEntityModule module = new Entities.BusinessEntityModule
            {
                ModuleId = GetReaderValue<Guid>(reader,"Id"),
                Name = reader["Name"] as string,
               // Title = reader["Title"] as string,
                ParentId = GetReaderValue<Guid?>(reader, "ParentId"),
                BreakInheritance = (bool)reader["BreakInheritance"],
                PermissionOptions = new List<string>() { "View", "Add", "Edit", "Delete", "Full Control" }
            };
            return module;
        }
        
        #endregion



    }
}
