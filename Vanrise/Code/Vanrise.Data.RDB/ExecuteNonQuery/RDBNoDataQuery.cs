using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public abstract class RDBNoDataQuery : BaseRDBQuery
    {
        public int ExecuteNonQuery()
        {
            return this.DataProvider.ExecuteNonQuery(new RDBDataProviderExecuteNonQueryContext { ResolvedQuery = GetResolvedQuery() });
        }

        public abstract RDBResolvedNoDataQuery GetResolvedQuery();

        #region Private Classes

        private class RDBDataProviderExecuteNonQueryContext : IRDBDataProviderExecuteNonQueryContext
        {
            public RDBResolvedNoDataQuery ResolvedQuery
            {
                get;
                set;
            }
        }


        #endregion
    }
}
