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
            string connectionString = (settings != null) ? settings.ConnectionString : null;

            schemaDataManager.Connection_String = connectionString;

            List<VRGeneratedScriptSchema> allSchemas = schemaDataManager.GetSchemas();

            Func<VRGeneratedScriptSchema, bool> filterFunc = (schema) =>
            {

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
