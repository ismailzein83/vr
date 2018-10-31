using Vanrise.Tools.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Data.SQL;
using Vanrise.Data;
using Vanrise.Common;
namespace Vanrise.Tools.Data.SQL
{
    public class SchemaDataManager : BaseSQLDataManager, ISchemaDataManager
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
        public List<Schema> GetSchemas()

        {
            return GetItemsText("select name from sys.schemas ", SchemaMapper, null);
        }

        Schema SchemaMapper(IDataReader reader)
        {
            return new Schema
            {
                Name = GetReaderValue<string>(reader, "name"),


            };
        }
    }
}
