using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public abstract class BaseRDBQuery
    {
        BaseRDBDataProvider _dataProvider = new Vanrise.Data.RDB.DataProvider.Providers.MSSQLRDBDataProvider();

        protected BaseRDBDataProvider DataProvider
        {
            get
            {
                return _dataProvider;
            }
        }
    }
}
