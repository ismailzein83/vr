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
namespace Vanrise.Tools.Data.SQL
{
    public class ColumnsDataManager : BaseSQLDataManager, IColumnsDataManager
    {
        public ColumnsDataManager() :
            base(GetConnectionStringName("", ""))
        {
        }

        public string Connection_String { get; set; }
        
        protected override string GetConnectionString()
        {
            return Connection_String;
        }
        public List<Columns> GetColumns(string schema,string table)
        {


           return GetItemsText("SELECT * FROM [" + schema + "].[" + table + "]", ColumnsMapper,null);
        }


        Columns ColumnsMapper(IDataReader reader)
        {
            return new Columns
            {
                Name = GetReaderValue<string>(reader, "Column Name"),

            };
        }
    }
}
