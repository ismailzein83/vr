using Vanrise.Tools.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Tools.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common.Business;
namespace Vanrise.Tools.Business
{
    public class SchemaManager
    {

        #region Public Methods
        public IEnumerable<SchemaInfo> GetSchemasInfo(SchemaInfoFilter schemaInfoFilter)
        {
            ISchemaDataManager schemaDataManager = VRToolsFactory.GetDataManager<ISchemaDataManager>();


            Guid connectionId = schemaInfoFilter.ConnectionId;
            SQLConnection settings = new VRConnectionManager().GetVRConnection(connectionId).Settings as SQLConnection;
            string connectionString = (settings != null) ? settings.ConnectionString : null;

            schemaDataManager.Connection_String = connectionString;

            List<Schema> allSchemas = schemaDataManager.GetSchemas();

            Func<Schema, bool> filterFunc = (schema) =>
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
        public SchemaInfo SchemaInfoMapper(Schema schema)
        {
            return new SchemaInfo
            {
                Name = schema.Name,
            };
        }
        #endregion

    }

}
