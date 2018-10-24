using Vanrise.Tools.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Tools.Data
{
    public interface ITableDataManager : IDataManager
    {

        List<Table> GetTables();
        string Connection_String { set; }


    }
}
