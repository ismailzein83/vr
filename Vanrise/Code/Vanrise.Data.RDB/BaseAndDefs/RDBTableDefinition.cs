using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBTableDefinition
    {
        public string DBSchemaName { get; set; }

        public string DBTableName { get; set; }

        public string IdColumnName { get; set; }

        public string CreatedTimeColumnName { get; set; }

        public string ModifiedTimeColumnName { get; set; }

        public string CachePartitionColumnName { get; set; }

        public Dictionary<string, RDBTableColumnDefinition> Columns { get; set; }

        public Dictionary<string, RDBTableExpressionColumn> ExpressionColumns { get; set; }

        public Dictionary<string, RDBTableJoinDefinition> Joins { get; set; }

        public RDBTableFilterDefinition FilterDefinition { get; set; }
    }
    
    public abstract class RDBTableExpressionColumn
    {
        public List<string> DependantJoinNames { get; set; }

        public abstract void SetExpression(IRDBTableExpressionColumnSetExpressionContext context);
    }

    public interface IRDBTableExpressionColumnSetExpressionContext
    {
        string TableAlias { get; }

        RDBExpressionContext ExpressionContext { get; }
    }

    public class RDBTableExpressionColumnSetExpressionContext : IRDBTableExpressionColumnSetExpressionContext
    {
        public RDBTableExpressionColumnSetExpressionContext(string tableAlias, RDBExpressionContext expressionContext)
        {
            this.TableAlias = tableAlias;
            this.ExpressionContext = expressionContext;
        }

        public string TableAlias { get; private set; }

        public RDBExpressionContext ExpressionContext { get; private set; }
    }

    public abstract class RDBTableJoinDefinition
    {
        public List<string> DependantJoinNames { get; set; }

        public abstract void SetJoinExpression(IRDBTableJoinSetJoinExpressionContext context);
    }

    public interface IRDBTableJoinSetJoinExpressionContext
    {
        string TableAlias { get; }

        RDBJoinContext JoinContext { get; }
    }

    public class RDBTableJoinSetJoinExpressionContext : IRDBTableJoinSetJoinExpressionContext
    {
        public RDBTableJoinSetJoinExpressionContext(string tableAlias, RDBJoinContext joinContext)
        {
            this.TableAlias = tableAlias;
            this.JoinContext = joinContext;
        }

        public string TableAlias { get; private set; }

        public RDBJoinContext JoinContext { get; private set; }
    }

    public abstract class RDBTableFilterDefinition
    {
        public abstract void SetFilterExpression(IRDBTableFilterDefinitionSetFilterExpressionContext context);
    }

    public interface IRDBTableFilterDefinitionSetFilterExpressionContext
    {
        string TableAlias { get; }

        RDBConditionContext QueryWhereContext { get; }
    }

    public class RDBTableFilterDefinitionSetFilterExpressionContext : IRDBTableFilterDefinitionSetFilterExpressionContext
    {
        public RDBTableFilterDefinitionSetFilterExpressionContext(string tableAlias, RDBConditionContext queryWhereContext)
        {
            this.TableAlias = tableAlias;
            this.QueryWhereContext = queryWhereContext;
        }

        public string TableAlias { get; private set; }

        public RDBConditionContext QueryWhereContext { get; private set; }
    }

    public class RDBTableDefinitionQuerySource : IRDBTableQuerySource
    {
        RDBSchemaManager _schemaManager;
        string _databaseName;

        public RDBTableDefinitionQuerySource(string tableName)
            : this(null, tableName)
        {
        }

        public RDBTableDefinitionQuerySource(string databaseName, string tableName)
            : this(databaseName, tableName, RDBSchemaManager.Current)
        {
        }

        public RDBTableDefinitionQuerySource(string tableName, RDBSchemaManager schemaManager)
            : this(null, tableName, schemaManager)
        {
        }

        public RDBTableDefinitionQuerySource(string databaseName, string tableName, RDBSchemaManager schemaManager)
        {
            this._databaseName = databaseName;
            this.TableName = tableName;
            _schemaManager = schemaManager;
        }


        public string TableName { get; private set; }

        public string GetDescription()
        {
            return this.TableName;
        }

        public string ToDBQuery(IRDBTableQuerySourceToDBQueryContext context)
        {
            return _schemaManager.GetTableDBName(context.DataProvider, this._databaseName, this.TableName);
        }


        public string GetDBColumnName(IRDBTableQuerySourceGetDBColumnNameContext context)
        {
            return _schemaManager.GetColumnDBName(context.DataProvider, this.TableName, context.ColumnName);
        }

        public string GetUniqueName()
        {
            return this.TableName;
        }


        public void GetIdColumnInfo(IRDBTableQuerySourceGetIdColumnInfoContext context)
        {
            var tableDefinition = _schemaManager.GetTableDefinitionWithValidate(context.DataProvider, this.TableName);
            if(!String.IsNullOrEmpty( tableDefinition.IdColumnName ))
            {
                context.IdColumnName = tableDefinition.IdColumnName;
                context.IdColumnDefinition = _schemaManager.GetColumnDefinitionWithValidate(tableDefinition, this.TableName, tableDefinition.IdColumnName);
            }
        }


        public List<string> GetColumnNames(IRDBTableQuerySourceGetColumnNamesContext context)
        {
            return _schemaManager.GetTableColumns(this.TableName);
        }


        public void GetCreatedAndModifiedTime(IRDBTableQuerySourceGetCreatedAndModifiedTimeContext context)
        {
            var tableDefinition = _schemaManager.GetTableDefinitionWithValidate(context.DataProvider, this.TableName);
            context.CreatedTimeColumnName = tableDefinition.CreatedTimeColumnName;
            context.ModifiedTimeColumnName = tableDefinition.ModifiedTimeColumnName;
        }


        public void GetColumnDefinition(IRDBTableQuerySourceGetColumnDefinitionContext context)
        {
            context.ColumnDefinition = _schemaManager.GetColumnDefinitionWithValidate(context.DataProvider, this.TableName, context.ColumnName);
        }
        
        public bool TryGetExpressionColumn(IRDBTableQuerySourceTryGetExpressionColumnContext context)
        {
            RDBTableExpressionColumn expressionColumn;
            if(_schemaManager.TryGetExpressionColumn(context.QueryBuilderContext.DataProvider, this.TableName, context.ColumnName, out expressionColumn))
            {
                context.ExpressionColumn = expressionColumn;
                if(expressionColumn.DependantJoinNames != null)
                {
                    foreach (var dependantJoinName in expressionColumn.DependantJoinNames)
                    {
                        AddJoinIfNotAdded(dependantJoinName, context.QueryBuilderContext, context.TableAlias);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddJoinIfNotAdded(string joinName, RDBQueryBuilderContext queryBuilderContext, string tableAlias)
        {
            if (_addedJoins.Contains(joinName))
                return;

            var tableDefinition = _schemaManager.GetTableDefinitionWithValidate(queryBuilderContext.DataProvider, this.TableName);

            RDBTableJoinDefinition joinDefinition;
            if (!tableDefinition.Joins.TryGetValue(joinName, out joinDefinition))
                throw new NullReferenceException($"joinDefinition '{joinName}");

            if(joinDefinition.DependantJoinNames != null && joinDefinition.DependantJoinNames.Count > 0)
            {
                foreach(var dependantJoinName in joinDefinition.DependantJoinNames)
                {
                    AddJoinIfNotAdded(dependantJoinName, queryBuilderContext, tableAlias);
                }
            }

            joinDefinition.SetJoinExpression(new RDBTableJoinSetJoinExpressionContext(tableAlias, queryBuilderContext.GetQueryJoinContext()));

            _addedJoins.Add(joinName);
        }

        HashSet<string> _addedJoins = new HashSet<string>();

        public void FinalizeBeforeResolveQuery(IRDBTableQuerySourceFinalizeBeforeResolveQueryContext context)
        {
            var tableDefinition = _schemaManager.GetTableDefinitionWithValidate(context.DataProvider, this.TableName);

            if (tableDefinition.FilterDefinition != null)
                tableDefinition.FilterDefinition.SetFilterExpression(new RDBTableFilterDefinitionSetFilterExpressionContext(context.TableAlias, context.QueryWhere));            
        }
    }

    //public class RDBTempPhysicalTableQuerySource : IRDBTableQuerySource
    //{
    //    string _tableName;
    //    RDBTableDefinition _tableDefinition;

    //    public RDBTempPhysicalTableQuerySource(string tableName, RDBTableDefinition tableDefinition)
    //    {
    //        tableName.ThrowIfNull("tableName");
    //        tableDefinition.ThrowIfNull("tableDefinition", tableName);
    //        tableDefinition.DBTableName.ThrowIfNull("tableDefinition.DBTableName", tableName);
    //        tableDefinition.Columns.ThrowIfNull("tableDefinition.Columns", tableName);
    //        this._tableName = tableName;
    //        this._tableDefinition = tableDefinition;
    //    }

    //    public string GetUniqueName()
    //    {
    //        return this._tableName;
    //    }

    //    public string GetDescription()
    //    {
    //        return this._tableName;
    //    }

    //    public string ToDBQuery(IRDBTableQuerySourceToDBQueryContext context)
    //    {
    //        return RDBSchemaManager.GetTableDBName(context.DataProvider, this._tableDefinition);
    //    }

    //    public string GetDBColumnName(IRDBTableQuerySourceGetDBColumnNameContext context)
    //    {
    //        string columnName = context.ColumnName;
    //        RDBTableColumnDefinition columnDefinition;
    //        if (!this._tableDefinition.Columns.TryGetValue(columnName, out columnDefinition))
    //            throw new Exception(String.Format(" Column '{0}' not found in table '{1}'", columnName, this._tableName));
    //        return RDBSchemaManager.GetColumnDBName(context.DataProvider, columnName, columnDefinition);
    //    }


    //    public void GetIdColumnInfo(IRDBTableQuerySourceGetIdColumnInfoContext context)
    //    {
    //        throw new NotImplementedException();
    //    }


    //    public List<string> GetColumnNames(IRDBTableQuerySourceGetColumnNamesContext context)
    //    {
    //        return this._tableDefinition.Columns.Keys.ToList();
    //    }


    //    public void GetCreatedAndModifiedTime(IRDBTableQuerySourceGetCreatedAndModifiedTimeContext context)
    //    {
    //        context.CreatedTimeColumnName = this._tableDefinition.CreatedTimeColumnName;
    //        context.ModifiedTimeColumnName = this._tableDefinition.ModifiedTimeColumnName;
    //    }


    //    public void GetColumnDefinition(IRDBTableQuerySourceGetColumnDefinitionContext context)
    //    {
    //        string columnName = context.ColumnName;
    //        RDBTableColumnDefinition columnDefinition;
    //        if (!this._tableDefinition.Columns.TryGetValue(columnName, out columnDefinition))
    //            throw new Exception(String.Format(" Column '{0}' not found in table '{1}'", columnName, this._tableName));
    //        context.ColumnDefinition = columnDefinition;
    //    }
    //}
}
