using Vanrise.Tools.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Tools.Data
{
    public interface ITableDataDataManager : IDataManager
    {

        List<TableData> GetTableData(string schemaName,string tableName,string whereCondition);
        string Connection_String { set; }


    }
}
