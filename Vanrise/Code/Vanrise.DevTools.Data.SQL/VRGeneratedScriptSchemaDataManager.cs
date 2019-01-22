using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.DevTools.Data;
using Vanrise.DevTools.Entities;

namespace Vanrise.DevTools.Data.SQL
{
    public class SchemaDataManager : BaseSQLDataManager, IVRGeneratedScriptSchemaDataManager
    {
        public SchemaDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }
        public string Connection_String { get; set; }

        protected override string GetConnectionString()
        {
            return Connection_String;
        }
        public List<VRGeneratedScriptSchema> GetSchemas()

        {
            return GetItemsText("select name from sys.schemas ", SchemaMapper, null);
        }

        VRGeneratedScriptSchema SchemaMapper(IDataReader reader)
        {
            return new VRGeneratedScriptSchema
            {
                Name = GetReaderValue<string>(reader, "name"),


            };
        }
    }
}
