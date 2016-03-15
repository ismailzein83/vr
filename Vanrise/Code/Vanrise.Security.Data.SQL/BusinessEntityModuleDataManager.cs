﻿using System;
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

        public bool ToggleBreakInheritance(string entityId)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_BusinessEntityModule_ToggleBreakInheritance", entityId);
            return (recordesEffected > 0);
        }

        public bool AreBusinessEntityModulesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("sec.BusinessEntityModule", ref updateHandle);
        }
        public bool AddBusinessEntityModule(Entities.BusinessEntityModule moduleObject, out int moduleId)
        {
            object moduleID;

            int recordesEffected = ExecuteNonQuerySP("sec.sp_BusinessEntityModule_Insert", out moduleID, moduleObject.Name, moduleObject.ParentId, moduleObject.BreakInheritance);
            moduleId = (recordesEffected > 0) ? (int)moduleID : -1;

            return (recordesEffected > 0);
        }

        public bool UpdateBusinessEntityModule(Entities.BusinessEntityModule moduleObject)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_BusinessEntityModule_Update", moduleObject.ModuleId, moduleObject.Name, moduleObject.ParentId, moduleObject.BreakInheritance);
            return (recordesEffected > 0);
        }
        #region Mappers

        Entities.BusinessEntityModule ModuleMapper(IDataReader reader)
        {
            Entities.BusinessEntityModule module = new Entities.BusinessEntityModule
            {
                ModuleId = (int)reader["Id"],
                Name = reader["Name"] as string,
                Title = reader["Title"] as string,
                ParentId = GetReaderValue<int>(reader, "ParentId"),
                BreakInheritance = (bool)reader["BreakInheritance"],
                PermissionOptions = new List<string>() { "View", "Add", "Edit", "Delete", "Full Control" }
            };
            return module;
        }
        
        #endregion


     
    }
}
