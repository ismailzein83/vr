﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Data.RDB
{
    public abstract class BaseRDBDataProvider
    {
        public abstract string UniqueName { get; }

        public abstract string NowDateTimeFunction { get; }

        public virtual string EmptyStringValue
        {
            get
            {
                return string.Empty;
            }
        }

        public abstract string ConvertToDBParameterName(string parameterName, RDBParameterDirection parameterDirection);

        public abstract string GetTableDBName(string schemaName, string tableName);

        public virtual string GetColumnDBName(string columnDBName)
        {
            return columnDBName;
        }

        public virtual string GetDBAlias(string alias)
        {
            return alias;
        }

        public abstract string GetQueryConcatenatedStrings(params string[] strings);

        //public abstract RDBResolvedQueryStatement GetStatementSetParameterValue(string parameterDBName, string parameterValue);

        //public abstract RDBResolvedQuery ResolveParameterDeclarations(IRDBDataProviderResolveParameterDeclarationsContext context);

        public abstract RDBResolvedQuery ResolveSelectQuery(IRDBDataProviderResolveSelectQueryContext context);

        public abstract RDBResolvedQuery ResolveSelectTableRowsCountQuery(IRDBDataProviderResolveSelectTableRowsCountQueryContext context);

        public abstract RDBResolvedQuery ResolveInsertQuery(IRDBDataProviderResolveInsertQueryContext context);

        public abstract RDBResolvedQuery ResolveUpdateQuery(IRDBDataProviderResolveUpdateQueryContext context);

        public abstract RDBResolvedQuery ResolveDeleteQuery(IRDBDataProviderResolveDeleteQueryContext context);

        public abstract RDBResolvedQuery ResolveTableCreationQuery(IRDBDataProviderResolveTableCreationQueryContext context);

        public abstract RDBResolvedQuery ResolveTableDropQuery(IRDBDataProviderResolveTableDropQueryContext context);

        public virtual RDBResolvedQuery ResolveIndexCreationQuery(IRDBDataProviderResolveIndexCreationQueryContext context)
        {
            throw new NotImplementedException();
        }

        public abstract RDBResolvedQuery ResolveTempTableCreationQuery(IRDBDataProviderResolveTempTableCreationQueryContext context);

        public abstract RDBResolvedQuery ResolveTempTableDropQuery(IRDBDataProviderResolveTempTableDropQueryContext context);

        public abstract void ExecuteReader(IRDBDataProviderExecuteReaderContext context);

        public abstract RDBFieldValue ExecuteScalar(IRDBDataProviderExecuteScalarContext context);

        public abstract int ExecuteNonQuery(IRDBDataProviderExecuteNonQueryContext context);

        public abstract BaseRDBStreamForBulkInsert InitializeStreamForBulkInsert(IRDBDataProviderInitializeStreamForBulkInsertContext context);

        public virtual DateTimeRange GetRDBProviderDateTimeRange()
        {
            return new DateTimeRange()
            {
                From = new DateTime(1753, 1, 1, 0, 0, 0),
                To = new DateTime(9999, 12, 31, 23, 59, 59)
            };
        }


        public virtual bool IsDataUpdated(string tableName, RDBTableDefinition tableDefinition, ref object lastReceivedDataInfo)
        {
            var queryContext = new RDBQueryContext(this);
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(tableName, "tab", null, true);
            selectQuery.SelectAggregates().Aggregate(RDBNonCountAggregateType.MAX, tableDefinition.ModifiedTimeColumnName);
            var newReceivedDataInfo = queryContext.ExecuteScalar().NullableDateTimeValue;
            return IsDataUpdated(ref lastReceivedDataInfo, newReceivedDataInfo);
        }

        public virtual bool IsDataUpdated<T>(string tableName, RDBTableDefinition tableDefinition, string columnName, T columnValue, ref object lastReceivedDataInfo)
        {
            var queryContext = new RDBQueryContext(this);
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(tableName, "tab", null, true);
            selectQuery.SelectAggregates().Aggregate(RDBNonCountAggregateType.MAX, tableDefinition.ModifiedTimeColumnName);
            selectQuery.Where().EqualsCondition(columnName).ObjectValue(columnValue);
            var newReceivedDataInfo = queryContext.ExecuteScalar().NullableDateTimeValue;
            return IsDataUpdated(ref lastReceivedDataInfo, newReceivedDataInfo);
        }

        protected bool IsDataUpdated(ref object lastReceivedDataInfo, DateTime? newReceivedDataInfo)
        {
            if (!newReceivedDataInfo.HasValue)
            {
                return false;
            }
            else
            {
                if (lastReceivedDataInfo == null || newReceivedDataInfo != (DateTime?)lastReceivedDataInfo)
                {
                    lastReceivedDataInfo = newReceivedDataInfo;
                    return true;
                }
                else
                    return false;
            }
        }
    }

    //public interface IRDBDataProviderResolveParameterDeclarationsContext : IBaseRDBResolveQueryContext
    //{

    //}

    //public class RDBDataProviderResolveParameterDeclarationsContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveParameterDeclarationsContext
    //{
    //    public RDBDataProviderResolveParameterDeclarationsContext(IBaseRDBResolveQueryContext parentContext)
    //        : base(parentContext)
    //    {            
    //    }
    //}

    public interface IRDBDataProviderResolveSelectQueryContext : IBaseRDBResolveQueryContext
    {
        RDBQueryBuilderContext QueryBuilderContext { get; }

        bool IsMainStatement { get; }

        IRDBTableQuerySource Table
        {
            get;
        }

        string TableAlias
        {
            get;
        }

        long? NbOfRecords
        {
            get;
        }

        List<RDBSelectColumn> Columns
        {
            get;
        }

        List<RDBJoin> Joins
        {
            get;
        }

        BaseRDBCondition Condition
        {
            get;
        }

        RDBGroupBy GroupBy { get; }

        List<RDBSelectSortColumn> SortColumns { get; }

        bool WithNoLock { get; }
    }

    public class RDBDataProviderResolveSelectQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveSelectQueryContext
    {
        //public RDBDataProviderResolveSelectQueryContext(IRDBTableQuerySource table, string tableAlias, long? nbOfRecords, List<RDBSelectColumn> columns, List<RDBJoin> joins,
        //    BaseRDBCondition condition, RDBGroupBy groupBy, List<RDBSelectSortColumn> sortColumns, BaseRDBQueryContext queryContext, BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
        //    : base(queryContext, dataProvider, parameterValues)
        //{
        //    this.Table = table;
        //    this.TableAlias = tableAlias;
        //    this.NbOfRecords = nbOfRecords;
        //    this.Columns = columns;
        //    this.Joins = joins;
        //    this.Condition = condition;
        //    this.GroupBy = groupBy;
        //    this.SortColumns = sortColumns;
        //}

        public RDBDataProviderResolveSelectQueryContext(bool isMainStatement, IRDBTableQuerySource table, string tableAlias, long? nbOfRecords, List<RDBSelectColumn> columns, List<RDBJoin> joins,
            BaseRDBCondition condition, RDBGroupBy groupBy, List<RDBSelectSortColumn> sortColumns, bool withNoLock, IBaseRDBResolveQueryContext parentContext, RDBQueryBuilderContext queryBuilderContext)
            : base(parentContext)
        {
            this.QueryBuilderContext = queryBuilderContext;
            this.IsMainStatement = isMainStatement;
            this.Table = table;
            this.TableAlias = tableAlias;
            this.NbOfRecords = nbOfRecords;
            this.Columns = columns;
            this.Joins = joins;
            this.Condition = condition;
            this.GroupBy = groupBy;
            this.SortColumns = sortColumns;
            this.WithNoLock = withNoLock;
        }

        public RDBQueryBuilderContext QueryBuilderContext
        {
            get;
            private set;
        }

        public bool IsMainStatement
        {
            get;
            private set;
        }

        public IRDBTableQuerySource Table
        {
            get;
            private set;
        }

        public string TableAlias
        {
            get;
            private set;
        }

        public long? NbOfRecords
        {
            get;
            private set;
        }

        public List<RDBSelectColumn> Columns
        {
            get;
            private set;
        }

        public List<RDBJoin> Joins
        {
            get;
            private set;
        }

        public BaseRDBCondition Condition
        {
            get;
            private set;
        }

        public RDBGroupBy GroupBy { get; private set; }

        public List<RDBSelectSortColumn> SortColumns { get; private set; }

        public bool WithNoLock { get; private set; }

    }

    public interface IRDBDataProviderResolveSelectTableRowsCountQueryContext : IBaseRDBResolveQueryContext
    {
        string SchemaName { get; }

        string TableName { get; }
    }

    public class RDBDataProviderResolveSelectTableRowsCountQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveSelectTableRowsCountQueryContext
    {
        public RDBDataProviderResolveSelectTableRowsCountQueryContext(string schemaName, string tableName,
            IBaseRDBResolveQueryContext parentContext)
            : base(parentContext)
        {
            this.SchemaName = schemaName;
            this.TableName = tableName;
        }

        public string SchemaName
        {
            get;
            private set;
        }

        public string TableName
        {
            get;
            private set;
        }
    }

    public class RDBResolvedQuery
    {
        public RDBResolvedQuery()
        {
            this.Statements = new List<RDBResolvedQueryStatement>();
        }

        public List<RDBResolvedQueryStatement> Statements { get; private set; }
    }

    public class RDBResolvedQueryStatement
    {
        public string TextStatement { get; set; }
    }

    public interface IRDBDataProviderResolveInsertQueryContext : IBaseRDBResolveQueryContext
    {
        RDBQueryBuilderContext QueryBuilderContext { get; }

        IRDBTableQuerySource Table { get; }

        List<RDBInsertColumn> ColumnValues { get; }

        RDBSelectQuery SelectQuery { get; }

        bool AddSelectGeneratedId { get; }
    }
    public class RDBDataProviderResolveInsertQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveInsertQueryContext
    {
        public RDBDataProviderResolveInsertQueryContext(IRDBTableQuerySource table, List<RDBInsertColumn> columnValues, RDBSelectQuery selectQuery, bool addSelectGeneratedId,
            IBaseRDBResolveQueryContext parentContext, RDBQueryBuilderContext queryBuilderContext)
            : base(parentContext)
        {
            this.QueryBuilderContext = queryBuilderContext;
            this.Table = table;
            this.ColumnValues = columnValues;
            this.SelectQuery = selectQuery;
            this.AddSelectGeneratedId = addSelectGeneratedId;
        }

        public RDBQueryBuilderContext QueryBuilderContext
        {
            get;
            private set;
        }

        public IRDBTableQuerySource Table { get; set; }

        public List<RDBInsertColumn> ColumnValues { get; private set; }

        public RDBSelectQuery SelectQuery { get; private set; }

        public bool AddSelectGeneratedId { get; private set; }
    }
    public interface IRDBDataProviderResolveUpdateQueryContext : IBaseRDBResolveQueryContext
    {
        RDBQueryBuilderContext QueryBuilderContext { get; }

        IRDBTableQuerySource Table { get; }

        string TableAlias { get; }

        List<RDBUpdateColumn> ColumnValues { get; }

        BaseRDBCondition Condition { get; }

        List<RDBJoin> Joins { get; }

        List<RDBUpdateSelectColumn> SelectColumns { get; }
    }

    public class RDBDataProviderResolveUpdateQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveUpdateQueryContext
    {
        //public RDBDataProviderResolveUpdateQueryContext(IRDBTableQuerySource table, string tableAlias, List<RDBUpdateColumn> columnValues, BaseRDBCondition condition, List<RDBJoin> joins, BaseRDBQueryContext queryContext, BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
        //    : base(queryContext, dataProvider, parameterValues)
        //{
        //    this.Table = table;
        //    this.TableAlias = tableAlias;
        //    this.ColumnValues = columnValues;
        //    this.Condition = condition;
        //    this.Joins = joins;
        //}

        public RDBDataProviderResolveUpdateQueryContext(IRDBTableQuerySource table, string tableAlias, List<RDBUpdateColumn> columnValues, BaseRDBCondition condition,
            List<RDBJoin> joins, List<RDBUpdateSelectColumn> selectColumns, IBaseRDBResolveQueryContext parentContext, RDBQueryBuilderContext queryBuilderContext)
            : base(parentContext)
        {
            this.QueryBuilderContext = queryBuilderContext;
            this.Table = table;
            this.TableAlias = tableAlias;
            this.ColumnValues = columnValues;
            this.Condition = condition;
            this.Joins = joins;
            this.SelectColumns = selectColumns;
        }

        public RDBQueryBuilderContext QueryBuilderContext
        {
            get;
            private set;
        }

        public IRDBTableQuerySource Table { get; private set; }

        public string TableAlias
        {
            get;
            private set;
        }

        public List<RDBUpdateColumn> ColumnValues { get; private set; }

        public BaseRDBCondition Condition { get; private set; }

        public List<RDBJoin> Joins { get; private set; }


        public List<RDBUpdateSelectColumn> SelectColumns { get; private set; }
    }

    public interface IRDBDataProviderResolveDeleteQueryContext : IBaseRDBResolveQueryContext
    {
        RDBQueryBuilderContext QueryBuilderContext { get; }

        IRDBTableQuerySource Table { get; }

        string TableAlias { get; }

        BaseRDBCondition Condition { get; }

        List<RDBJoin> Joins { get; }
    }

    public class RDBDataProviderResolveDeleteQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveDeleteQueryContext
    {
        public RDBDataProviderResolveDeleteQueryContext(IRDBTableQuerySource table, string tableAlias, BaseRDBCondition condition,
            List<RDBJoin> joins, IBaseRDBResolveQueryContext parentContext, RDBQueryBuilderContext queryBuilderContext)
            : base(parentContext)
        {
            this.QueryBuilderContext = queryBuilderContext;
            this.Table = table;
            this.TableAlias = tableAlias;
            this.Condition = condition;
            this.Joins = joins;
        }

        public RDBQueryBuilderContext QueryBuilderContext { get; private set; }

        public IRDBTableQuerySource Table { get; private set; }

        public string TableAlias { get; private set; }

        public BaseRDBCondition Condition { get; private set; }

        public List<RDBJoin> Joins { get; private set; }
    }

    public interface IRDBDataProviderResolveTableCreationQueryContext : IBaseRDBResolveQueryContext
    {
        string SchemaName { get; }

        string TableName { get; }

        Dictionary<string, RDBCreateTableColumnDefinition> Columns { get; }

        bool PrimaryKeyIndexNonClustered { get; }
    }

    public class RDBDataProviderResolveTableCreationQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveTableCreationQueryContext
    {
        public RDBDataProviderResolveTableCreationQueryContext(string schemaName, string tableName, Dictionary<string, RDBCreateTableColumnDefinition> columns, bool primaryKeyIndexNonClustered,
            IBaseRDBResolveQueryContext parentContext)
            : base(parentContext)
        {
            this.SchemaName = schemaName;
            this.TableName = tableName;
            this.Columns = columns;
            this.PrimaryKeyIndexNonClustered = primaryKeyIndexNonClustered;
        }

        public string SchemaName
        {
            get;
            private set;
        }

        public string TableName
        {
            get;
            private set;
        }

        public Dictionary<string, RDBCreateTableColumnDefinition> Columns
        {
            get;
            private set;
        }

        public bool PrimaryKeyIndexNonClustered
        {
            get;
            private set;
        }
    }

    public interface IRDBDataProviderResolveTableDropQueryContext : IBaseRDBResolveQueryContext
    {
        string SchemaName { get; }

        string TableName { get; }
    }

    public class RDBDataProviderResolveTableDropQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveTableDropQueryContext
    {
        public RDBDataProviderResolveTableDropQueryContext(string schemaName, string tableName,
            IBaseRDBResolveQueryContext parentContext)
            : base(parentContext)
        {
            this.SchemaName = schemaName;
            this.TableName = tableName;
        }

        public string SchemaName
        {
            get;
            private set;
        }

        public string TableName
        {
            get;
            private set;
        }
    }

    public interface IRDBDataProviderResolveIndexCreationQueryContext : IBaseRDBResolveQueryContext
    {
        string SchemaName { get; }

        string TableName { get; }

        RDBCreateIndexType IndexType { get; }

        List<string> ColumnNames { get; }
    }

    public class RDBDataProviderResolveIndexCreationQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveIndexCreationQueryContext
    {
        public RDBDataProviderResolveIndexCreationQueryContext(string schemaName, string tableName, RDBCreateIndexType indexType, List<string> columnNames, 
            IBaseRDBResolveQueryContext parentContext)
            : base(parentContext)
        {
            this.SchemaName = schemaName;
            this.TableName = tableName;
            this.ColumnNames = columnNames;
            this.IndexType = indexType;
        }

        public string SchemaName
        {
            get;
            private set;
        }

        public string TableName
        {
            get;
            private set;
        }

        public RDBCreateIndexType IndexType
        {
            get;
            private set;
        }

        public List<string> ColumnNames
        {
            get;
            private set;
        }
    }

    public interface IRDBDataProviderResolveTempTableCreationQueryContext : IBaseRDBResolveQueryContext
    {
        Dictionary<string, RDBTempTableColumnDefinition> Columns { get; }

        string TempTableName { set; }
    }

    public class RDBDataProviderResolveTempTableCreationQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveTempTableCreationQueryContext
    {
        //public RDBDataProviderResolveTempTableCreationQueryContext(Dictionary<string, RDBTableColumnDefinition> columns, BaseRDBQueryContext queryContext, BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
        //    : base(queryContext, dataProvider, parameterValues)
        //{
        //    this.Columns = columns;

        //}

        public RDBDataProviderResolveTempTableCreationQueryContext(Dictionary<string, RDBTempTableColumnDefinition> columns, IBaseRDBResolveQueryContext parentContext)
            : base(parentContext)
        {
            this.Columns = columns;
        }


        public Dictionary<string, RDBTempTableColumnDefinition> Columns
        {
            get;
            private set;
        }


        public string TempTableName
        {
            set;
            get;
        }
    }

    public interface IRDBDataProviderResolveTempTableDropQueryContext : IBaseRDBResolveQueryContext
    {
        string TempTableName { get; }
    }

    public class RDBDataProviderResolveTempTableDropQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveTempTableDropQueryContext
    {
        public RDBDataProviderResolveTempTableDropQueryContext(string tempTableName, IBaseRDBResolveQueryContext parentContext)
            : base(parentContext)
        {
            this.TempTableName = tempTableName;
        }


        public string TempTableName
        {
            get;
            private set;
        }
    }

    public interface IBaseRDBDataProviderExecuteQueryContext
    {
        IBaseRDBResolveQueryContext ResolveQueryContext { get; }

        RDBResolvedQuery Query { get; }

        bool ExecuteTransactional { get; }

        Dictionary<string, RDBParameter> Parameters { get; }
    }

    public abstract class BaseRDBDataProviderExecuteQueryContext : IBaseRDBDataProviderExecuteQueryContext
    {
        public BaseRDBDataProviderExecuteQueryContext(IBaseRDBResolveQueryContext resolveQueryContext, RDBResolvedQuery query, bool executeTransactional, Dictionary<string, RDBParameter> parameters)
        {
            this.ResolveQueryContext = resolveQueryContext;
            this.Query = query;
            this.ExecuteTransactional = executeTransactional;
            this.Parameters = parameters;
        }

        public IBaseRDBResolveQueryContext ResolveQueryContext
        {
            get;
            private set;
        }

        public RDBResolvedQuery Query
        {
            get;
            private set;
        }


        public bool ExecuteTransactional
        {
            get;
            private set;
        }

        public Dictionary<string, RDBParameter> Parameters
        {

            get;
            private set;
        }
    }


    public interface IRDBDataProviderExecuteReaderContext : IBaseRDBDataProviderExecuteQueryContext
    {
        void OnReaderReady(IRDBDataReader reader);
    }

    public interface IRDBDataProviderExecuteScalarContext : IBaseRDBDataProviderExecuteQueryContext
    {
    }

    public interface IRDBDataProviderExecuteNonQueryContext : IBaseRDBDataProviderExecuteQueryContext
    {

    }

    public interface IRDBDataProviderInitializeStreamForBulkInsertContext
    {
        string DBTableName { get; }

        char FieldSeparator { get; }

        List<RDBTableColumnDefinition> Columns { get; }
    }

    public class RDBDataProviderInitializeStreamForBulkInsertContext : IRDBDataProviderInitializeStreamForBulkInsertContext
    {
        BaseRDBDataProvider _dataProvider;
        RDBSchemaManager _schemaManager;

        public RDBDataProviderInitializeStreamForBulkInsertContext(BaseRDBDataProvider dataProvider, RDBSchemaManager schemaManager, string tableName, char fieldSeparator, string[] columnNames)
        {
            _dataProvider = dataProvider;
            this._schemaManager = schemaManager;
            this.FieldSeparator = fieldSeparator;
            this.DBTableName = _schemaManager.GetTableDBName(_dataProvider, tableName);
            columnNames.ThrowIfNull("columnNames");
            this.Columns = new List<RDBTableColumnDefinition>();
            foreach (var columnName in columnNames)
            {
                var columnDefinition = _schemaManager.GetColumnDefinitionWithValidate(_dataProvider, tableName, columnName);
                this.Columns.Add(columnDefinition);
            }
        }

        public string DBTableName
        {
            get;
            private set;
        }

        public List<RDBTableColumnDefinition> Columns
        {
            get;
            private set;
        }

        public char FieldSeparator { get; private set; }

        public string[] ColumnNames { get; private set; }
    }
}