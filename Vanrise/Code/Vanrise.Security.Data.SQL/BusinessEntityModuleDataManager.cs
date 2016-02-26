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
                PermissionOptions = Common.Serializer.Deserialize<List<string>>(reader["PermissionOptions"] as string)
            };
            return module;
        }
        
        #endregion
    }
}
