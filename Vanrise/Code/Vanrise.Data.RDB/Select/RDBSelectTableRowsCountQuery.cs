﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBSelectTableRowsCountQuery : BaseRDBQuery
    {
        RDBQueryBuilderContext _queryBuilderContext;
        RDBSchemaManager _schemaManager;
        string _tableName;

        internal RDBSelectTableRowsCountQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        public void TableName(string tableName)
        {
            this.TableName(tableName, RDBSchemaManager.Current);
        }

        public void TableName(string tableName, RDBSchemaManager schemaManager)
        {
            _schemaManager = schemaManager;
            _tableName = tableName;
        }

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            var tableDefinition = _schemaManager.GetTableDefinitionWithValidate(_queryBuilderContext.DataProvider, _tableName);
            var resolveQueryContext = new RDBDataProviderResolveSelectTableRowsCountQueryContext(tableDefinition.DBSchemaName, tableDefinition.DBTableName, context);
            return context.DataProvider.ResolveSelectTableRowsCountQuery(resolveQueryContext);
        }
    }
}
