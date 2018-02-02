using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBBulkInsertQueryContext : IRDBBulkInsertStreamForBulkInsertInitialized
    {
        BaseRDBDataProvider _dataProvider;

        public BaseRDBDataProvider DataProvider
        {
            get
            {
                return _dataProvider;
            }
        }

        public RDBBulkInsertQueryContext(BaseRDBDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public IRDBBulkInsertStreamForBulkInsertInitialized InitializeStreamForBulkInsert(RDBTableDefinition table, string[] columnNames)
        {
            return this;
        }
    }

    public interface IRDBBulkInsertStreamForBulkInsertInitialized
    {

    }
}
