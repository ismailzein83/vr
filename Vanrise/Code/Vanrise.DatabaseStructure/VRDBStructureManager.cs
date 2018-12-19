using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.DatabaseStructure
{
    public class VRDBStructureManager
    {
        #region ctor/Fields

        static Dictionary<VRDBType, VRDBStructureConvertor> s_dbStructureConvertors = new Dictionary<VRDBType, VRDBStructureConvertor>();

        static VRDBStructureManager()
        {
            foreach (var convertorType in Utilities.GetAllImplementations<VRDBStructureConvertor>())
            {
                VRDBStructureConvertor convertor = Activator.CreateInstance(convertorType).CastWithValidate<VRDBStructureConvertor>("convertor");
                if (s_dbStructureConvertors.ContainsKey(convertor.DBType))
                    throw new Exception($"Duplicate VRDBStructureConvertor found of DBType '{convertor.DBType.ToString()}'");
                s_dbStructureConvertors.Add(convertor.DBType, convertor);
            }
        }

        #endregion

        #region Public Methods

        public string GetDBScriptFromMSSQLDB(string dbHandlerName, VRDBType targetDBType, string sqlServerName, string sqlDatabaseName, string sqlUsername, string sqlPassword)
        {
            var handlerType = Type.GetType($"Vanrise.DatabaseStructure.DBHandlers.{dbHandlerName}DBHandler, Vanrise.DatabaseStructure");
            handlerType.ThrowIfNull("handlerType", dbHandlerName);
            var handler = Activator.CreateInstance(handlerType).CastWithValidate<VRDBHandler>("handler", dbHandlerName);
            return GetDBScriptFromMSSQLDB(handler, targetDBType, sqlServerName, sqlDatabaseName, sqlUsername, sqlPassword);
        }

        public string GetDBScriptFromMSSQLDB(VRDBHandler dbHandler, VRDBType targetDBType, string sqlServerName, string sqlDatabaseName, string sqlUsername, string sqlPassword)
        {
            VRDBStructure dbStructure = GetMSSQLDBStructure(sqlServerName, sqlDatabaseName, sqlUsername, sqlPassword);
            dbStructure.ThrowIfNull("dbStructure", sqlDatabaseName);
            dbStructure = dbStructure.VRDeepCopy();
            dbHandler.ApplyChangesToDBStructure(new VRDBHandlerApplyChangesContext(targetDBType, dbStructure));
            return GetDBStructureConvertor(targetDBType).GenerateConvertedScript(new VRDBStructureConvertorGenerateConvertedScriptContext(dbStructure));
        }

        public VRDBStructure GetMSSQLDBStructure(string sqlServerName, string sqlDatabaseName, string sqlUsername, string sqlPassword)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        VRDBStructureConvertor GetDBStructureConvertor(VRDBType dbType)
        {
            VRDBStructureConvertor convertor;
            if (!s_dbStructureConvertors.TryGetValue(dbType, out convertor))
                throw new Exception($"Cannot find VRDBStructureConvertor of DBType '{dbType}'");
            return convertor;
        }

        #endregion

        #region Private Classes

        private class VRDBStructureConvertorGenerateConvertedScriptContext : IVRDBStructureConvertorGenerateConvertedScriptContext
        {
            public VRDBStructureConvertorGenerateConvertedScriptContext(VRDBStructure dbStructure)
            {
                this.DBStructure = dbStructure;
            }

            public VRDBStructure DBStructure { get; private set; }
        }

        private class VRDBHandlerApplyChangesContext : IVRDBHandlerApplyChangesContext
        {
            public VRDBHandlerApplyChangesContext(VRDBType dbType, VRDBStructure dbStructure)
            {
                this.DBType = DBType;
                this.DBStructure = dbStructure;
            }

            public VRDBType DBType { get; private set; }

            public VRDBStructure DBStructure { get; private set; }
        }

        #endregion
    }
}