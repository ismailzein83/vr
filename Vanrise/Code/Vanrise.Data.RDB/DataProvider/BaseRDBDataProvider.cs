﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public abstract class BaseRDBDataProvider
    {
        public abstract string UniqueName { get; }

        public abstract string NowDateTimeFunction { get; }

        public abstract string ConvertToDBParameterName(string parameterName);

        public abstract string GetTableDBName(string schemaName, string tableName);

        public abstract string GetQueryAsText(RDBResolvedQuery resolvedQuery);

        public abstract string GetQueryConcatenatedStrings(params string[] strings);

        public abstract string GetStatementSetParameterValue(string parameterDBName, string parameterValue);

        public abstract RDBResolvedQuery ResolveParameterDeclarations(IRDBDataProviderResolveParameterDeclarationsContext context);

        public abstract RDBResolvedQuery ResolveSelectQuery(IRDBDataProviderResolveSelectQueryContext context);

        public abstract RDBResolvedQuery ResolveInsertQuery(IRDBDataProviderResolveInsertQueryContext context);

        public abstract RDBResolvedQuery ResolveUpdateQuery(IRDBDataProviderResolveUpdateQueryContext context);

        public abstract RDBResolvedQuery ResolveDeleteQuery(IRDBDataProviderResolveDeleteQueryContext context);

        public abstract RDBResolvedQuery ResolveTempTableCreationQuery(IRDBDataProviderResolveTempTableCreationQueryContext context);

        public abstract void ExecuteReader(IRDBDataProviderExecuteReaderContext context);

        public abstract RDBFieldValue ExecuteScalar(IRDBDataProviderExecuteScalarContext context);

        public abstract int ExecuteNonQuery(IRDBDataProviderExecuteNonQueryContext context);

        public abstract BaseRDBStreamForBulkInsert InitializeStreamForBulkInsert(IRDBDataProviderInitializeStreamForBulkInsertContext context);
    }

    public interface IRDBDataProviderResolveParameterDeclarationsContext : IBaseRDBResolveQueryContext
    {

    }

    public class RDBDataProviderResolveParameterDeclarationsContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveParameterDeclarationsContext
    {
        public RDBDataProviderResolveParameterDeclarationsContext(IBaseRDBResolveQueryContext parentContext)
            : base(parentContext)
        {            
        }
    }

    public interface IRDBDataProviderResolveSelectQueryContext : IBaseRDBResolveQueryContext
    {
        RDBQueryBuilderContext QueryBuilderContext { get; }

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

        public RDBDataProviderResolveSelectQueryContext(IRDBTableQuerySource table, string tableAlias, long? nbOfRecords, List<RDBSelectColumn> columns, List<RDBJoin> joins,
            BaseRDBCondition condition, RDBGroupBy groupBy, List<RDBSelectSortColumn> sortColumns, IBaseRDBResolveQueryContext parentContext, RDBQueryBuilderContext queryBuilderContext)
            : base(parentContext)
        {
            this.QueryBuilderContext = queryBuilderContext;
            this.Table = table;
            this.TableAlias = tableAlias;
            this.NbOfRecords = nbOfRecords;
            this.Columns = columns;
            this.Joins = joins;
            this.Condition = condition;
            this.GroupBy = groupBy;
            this.SortColumns = sortColumns;
        }

        public RDBQueryBuilderContext QueryBuilderContext
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

    }
    
    public class RDBResolvedQuery
    {
        public RDBResolvedQuery()
        {
            this.Statements = new List<string>();
        }
        public string QueryText { get; set; }

        public List<string> Statements { get; private set; }
    }

    public interface IRDBDataProviderResolveInsertQueryContext : IBaseRDBResolveQueryContext
    {
        RDBQueryBuilderContext QueryBuilderContext { get; }

        IRDBTableQuerySource Table { get; }

        List<RDBInsertColumn> ColumnValues { get; }

        RDBSelectQuery SelectQuery { get; }

        string GeneratedIdDBParameterName { get; }
    }
    public class RDBDataProviderResolveInsertQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveInsertQueryContext
    {
        public RDBDataProviderResolveInsertQueryContext(IRDBTableQuerySource table, List<RDBInsertColumn> columnValues, RDBSelectQuery selectQuery, string idParameterName,
            IBaseRDBResolveQueryContext parentContext, RDBQueryBuilderContext queryBuilderContext)
            : base(parentContext)
        {
            this.QueryBuilderContext = queryBuilderContext;
            this.Table = table;
            this.ColumnValues = columnValues;
            this.SelectQuery = selectQuery;
            this.GeneratedIdDBParameterName = idParameterName;
        }

        public RDBQueryBuilderContext QueryBuilderContext
        {
            get;
            private set;
        }

        public IRDBTableQuerySource Table { get; set; }

        public List<RDBInsertColumn> ColumnValues { get; private set; }

        public RDBSelectQuery SelectQuery { get; private set; }
        
        public string GeneratedIdDBParameterName { get; private set; }
    }
    public interface IRDBDataProviderResolveUpdateQueryContext : IBaseRDBResolveQueryContext
    {
        RDBQueryBuilderContext QueryBuilderContext { get; }
        
        IRDBTableQuerySource Table { get; }

        string TableAlias { get; }

        List<RDBUpdateColumn> ColumnValues { get; }

        BaseRDBCondition Condition { get; }

        List<RDBJoin> Joins { get; }
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
            List<RDBJoin> joins, IBaseRDBResolveQueryContext parentContext, RDBQueryBuilderContext queryBuilderContext)
            : base(parentContext)
        {
            this.QueryBuilderContext = queryBuilderContext;
            this.Table = table;
            this.TableAlias = tableAlias;
            this.ColumnValues = columnValues;
            this.Condition = condition;
            this.Joins = joins;
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

    public interface IRDBDataProviderResolveTempTableCreationQueryContext : IBaseRDBResolveQueryContext
    {
        Dictionary<string, RDBTableColumnDefinition> Columns { get; }

        string TempTableName { set; }
    }

    public class RDBDataProviderResolveTempTableCreationQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveTempTableCreationQueryContext
    {
        //public RDBDataProviderResolveTempTableCreationQueryContext(Dictionary<string, RDBTableColumnDefinition> columns, BaseRDBQueryContext queryContext, BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
        //    : base(queryContext, dataProvider, parameterValues)
        //{
        //    this.Columns = columns;

        //}

        public RDBDataProviderResolveTempTableCreationQueryContext(Dictionary<string, RDBTableColumnDefinition> columns, IBaseRDBResolveQueryContext parentContext)
            : base(parentContext)
        {
            this.Columns = columns;
        }


        public Dictionary<string, RDBTableColumnDefinition> Columns
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

    public interface IBaseRDBDataProviderExecuteQueryContext
    {      
        string Query { get; }

        bool ExecuteTransactional { get; }

        Dictionary<string, RDBParameter> Parameters { get; }
    }

    public abstract class BaseRDBDataProviderExecuteQueryContext : IBaseRDBDataProviderExecuteQueryContext
    {
        public BaseRDBDataProviderExecuteQueryContext(string query, bool executeTransactional, Dictionary<string, RDBParameter> parameters)
        {
            this.Query = query;
            this.ExecuteTransactional = executeTransactional;
            this.Parameters = parameters;
        }

        public string Query
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
        string TableName { get; }
     
        char FieldSeparator { get; }

        string[] ColumnNames { get; }
    }

    public class RDBDataProviderInitializeStreamForBulkInsertContext : IRDBDataProviderInitializeStreamForBulkInsertContext
    {
        public RDBDataProviderInitializeStreamForBulkInsertContext(string tableName, char fieldSeparator, string[] columnNames)
        {
            this.TableName = tableName;
            this.FieldSeparator = fieldSeparator;
            this.ColumnNames = columnNames;
        }

        public string TableName { get; private set; }

        public char FieldSeparator { get; private set; }

        public string[] ColumnNames { get; private set; }
    }

}
