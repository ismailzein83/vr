using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBSwapTablesQuery : BaseRDBQuery
    {
        RDBQueryBuilderContext _queryBuilderContext;
        RDBSchemaManager _schemaManager;
        string _existingTable;
        string _newTable;
        bool _keepExistingTable;

        internal RDBSwapTablesQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        public void TableNames(string existingTable, string newTable, bool keepExistingTable)
        {
            this.TableNames(existingTable, newTable, keepExistingTable, RDBSchemaManager.Current);

        }

        public void TableNames(string existingTable, string newTable, bool keepExistingTable, RDBSchemaManager schemaManager)
        {
            _schemaManager = schemaManager;
            _existingTable = existingTable;
            _newTable = newTable;
            _keepExistingTable = keepExistingTable;
        }

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            var tableDefinition1 = _schemaManager.GetTableDefinitionWithValidate(_queryBuilderContext.DataProvider, _existingTable);
            var tableDefinition2 = _schemaManager.GetTableDefinitionWithValidate(_queryBuilderContext.DataProvider, _newTable);

            var resolveQueryContext = new RDBDataProviderResolveSwapTablesQueryContext(tableDefinition1.DBSchemaName, tableDefinition1.DBTableName, tableDefinition2.DBTableName, _keepExistingTable, context);
            return context.DataProvider.ResolveSwapTablesQuery(resolveQueryContext);
        }
    }
}
