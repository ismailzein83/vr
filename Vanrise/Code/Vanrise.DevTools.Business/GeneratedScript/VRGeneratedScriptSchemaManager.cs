using Vanrise.DevTools.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Vanrise.Common;
using Vanrise.DevTools.Entities;
using Vanrise.Common.Business;
namespace Vanrise.DevTools.Business
{
    public class VRGeneratedScriptSchemaManager
    {

        #region Public Methods
        public IEnumerable<VRGeneratedScriptSchemaInfo> GetSchemasInfo(VRGeneratedScriptSchemaInfoFilter schemaInfoFilter)
        {
            IVRGeneratedScriptSchemaDataManager schemaDataManager = VRDevToolsFactory.GetDataManager<IVRGeneratedScriptSchemaDataManager>();


            Guid connectionId = schemaInfoFilter.ConnectionId;
            SQLConnection settings = new VRConnectionManager().GetVRConnection(connectionId).Settings as SQLConnection;
            if (settings != null)
            {
                if (settings.ConnectionString != null)
                    schemaDataManager.Connection_String = settings.ConnectionString;
                else if (settings.ConnectionStringAppSettingName != null)
                    schemaDataManager.Connection_String = settings.ConnectionStringAppSettingName;
                else
                    schemaDataManager.Connection_String = settings.ConnectionStringName;
            }

            List<VRGeneratedScriptSchema> allSchemas = schemaDataManager.GetSchemas();

            Func<VRGeneratedScriptSchema, bool> filterFunc = (schema) =>
            {
                if (schema.Name.StartsWith("db_") || schema.Name == "sys")
                    return false;
                return true;
            };
            return allSchemas.MapRecords(SchemaInfoMapper, filterFunc).OrderBy(schema => schema.Name);
        }


        #endregion

        #region Private Classes

        #endregion

        #region Private Methods

        #endregion

        #region Mappers
        public VRGeneratedScriptSchemaInfo SchemaInfoMapper(VRGeneratedScriptSchema schema)
        {
            return new VRGeneratedScriptSchemaInfo
            {
                Name = schema.Name,
            };
        }
        #endregion

    }

}
