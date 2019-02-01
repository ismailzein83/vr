using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBCreateSchemaIfNotExistsQuery : BaseRDBQuery
    {
        string _dbSchemaName;

        RDBQueryBuilderContext _queryBuilderContext;

        internal RDBCreateSchemaIfNotExistsQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        public void DBSchemaName(string dbSchemaName)
        {
            _dbSchemaName = dbSchemaName;
        }

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            var resolveQueryContext = new RDBDataProviderResolveSchemaCreationIfNotExistsQueryContext(_dbSchemaName, context);
            return context.DataProvider.ResolveSchemaCreationIfNotExistsQuery(resolveQueryContext);
        }

    }


}

