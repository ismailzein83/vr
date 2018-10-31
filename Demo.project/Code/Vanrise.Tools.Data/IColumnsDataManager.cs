using Vanrise.Tools.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Tools.Data
{
    public interface IColumnsDataManager : IDataManager
    {

        List<Columns> GetColumns(string tableName);
        string Connection_String { set; }


    }
}
