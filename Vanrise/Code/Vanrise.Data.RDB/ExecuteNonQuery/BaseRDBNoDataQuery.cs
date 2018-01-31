using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public abstract class BaseRDBNoDataQuery : BaseRDBQuery
    {
        public int ExecuteNonQuery()
        {
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            var resolveQueryContext = new RDBNoDataQueryGetResolvedQueryContext(this.DataProvider, parameterValues);
            return this.DataProvider.ExecuteNonQuery(new RDBDataProviderExecuteNonQueryContext(GetResolvedQuery(resolveQueryContext), parameterValues));
        }

        public abstract RDBResolvedNoDataQuery GetResolvedQuery(IRDBNoDataQueryGetResolvedQueryContext context);

        #region Private Classes

        private class RDBDataProviderExecuteNonQueryContext : IRDBDataProviderExecuteNonQueryContext
        {
            public RDBDataProviderExecuteNonQueryContext(RDBResolvedNoDataQuery resolvedQuery, Dictionary<string, object> parameterValues)
            {
                this.ResolvedQuery = resolvedQuery;
                this.ParameterValues = parameterValues;
            }

            public RDBResolvedNoDataQuery ResolvedQuery
            {
                get;
                private set;
            }

            public Dictionary<string, object> ParameterValues
            {
                get;
                private set;
            }
        }

        #endregion
    }

    public interface IRDBNoDataQueryGetResolvedQueryContext : IBaseRDBResolveQueryContext
    {
    }

    public class RDBNoDataQueryGetResolvedQueryContext : BaseRDBResolveQueryContext, IRDBNoDataQueryGetResolvedQueryContext
    {
        public RDBNoDataQueryGetResolvedQueryContext(BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
            : base(dataProvider, parameterValues)
        {
        }

        public RDBNoDataQueryGetResolvedQueryContext(IBaseRDBResolveQueryContext parentContext, bool newQueryScope)
            : base(parentContext, newQueryScope)  
        {

        }
    }
}
