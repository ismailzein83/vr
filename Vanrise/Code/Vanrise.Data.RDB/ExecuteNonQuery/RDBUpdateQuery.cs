﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBUpdateQuery<T> : BaseRDBQuery, IUpdateQuery<T>, IUpdateQueryTableDefined<T>, IUpdateQueryFiltered<T>, IUpdateQueryColumnsAssigned<T>, IUpdateQueryNotExistsConditionDefined<T>, IUpdateQueryJoined<T>
    {
         T _parent;
        BaseRDBQueryContext _queryContext;

        public RDBUpdateQuery(T parent, BaseRDBQueryContext queryContext)
        {
            _parent = parent;
            _queryContext = queryContext;
        }

        public RDBUpdateQuery(RDBUpdateQuery<T> original)
        {
            this._parent = original._parent;
            this._queryContext = original._queryContext;
            this.Table = original.Table;
            this.ColumnValues = original.ColumnValues;
            this.Joins = original.Joins;
            this.Condition = original.Condition;
            this.NotExistCondition = original.NotExistCondition;
        }

        public IRDBTableQuerySource Table { get; set; }

        public List<RDBUpdateColumn> ColumnValues { get; private set; }

        public BaseRDBCondition Condition { get; private set; }

        public BaseRDBCondition NotExistCondition { get; private set; }

        public List<RDBJoin> Joins
        {
            get;
            private set;
        }

        public IUpdateQueryTableDefined<T> FromTable(string tableName)
        {
            return FromTable(new RDBTableDefinitionQuerySource(tableName));
        }

        public IUpdateQueryTableDefined<T> FromTable(IRDBTableQuerySource table)
        {
            this.Table = table;
            this.ColumnValues = new List<RDBUpdateColumn>();
            return this;
        }

        public RDBConditionContext<IUpdateQueryNotExistsConditionDefined<T>> IfNotExists()
            {
                return new RDBConditionContext<IUpdateQueryNotExistsConditionDefined<T>>(this, (condition) => this.NotExistCondition = condition, this.Table);
            }
        

        public IUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, BaseRDBExpression value)
        {
            this.ColumnValues.Add(new RDBUpdateColumn
            {
                ColumnName = columnName,
                Value = value
            });
            return this;
        }

        public IUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, string value)
        {
            return this.ColumnValue(columnName, new RDBFixedTextExpression { Value = value });
        }

        public IUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, int value)
        {
            return this.ColumnValue(columnName, new RDBFixedIntExpression { Value = value });
        }

        public IUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, long value)
        {
            return this.ColumnValue(columnName, new RDBFixedLongExpression { Value = value });
        }

        public IUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, decimal value)
        {
            return this.ColumnValue(columnName, new RDBFixedDecimalExpression { Value = value });
        }

        public IUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, float value)
        {
            return this.ColumnValue(columnName, new RDBFixedFloatExpression { Value = value });
        }

        public IUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, DateTime value)
        {
            return this.ColumnValue(columnName, new RDBFixedDateTimeExpression { Value = value });
        }

        public RDBJoinContext<IUpdateQueryJoined<T>> Join()
            {
                this.Joins = new List<RDBJoin>();
                return new RDBJoinContext<IUpdateQueryJoined<T>>(this, this.Joins);
            }
        

        public RDBConditionContext<IUpdateQueryFiltered<T>> Where()
            {
                return new RDBConditionContext<IUpdateQueryFiltered<T>>(this, (condition) => this.Condition = condition, this.Table);
            }
        

        protected override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            if (this.NotExistCondition != null)
            {
                var selectQuery = new RDBSelectQuery<RDBUpdateQuery<T>>(this, _queryContext);
                selectQuery.From(this.Table, 1).Where().Condition(this.NotExistCondition).SelectColumns().Column(new RDBNullExpression(), "nullColumn").EndColumns();
                var rdbNotExistsCondition = new RDBNotExistsCondition<RDBUpdateQuery<T>>()
                {
                    SelectQuery = selectQuery
                };
                var clonedUpdateQuery = new RDBUpdateQuery<T>(this);
                clonedUpdateQuery.NotExistCondition = null;
                IRDBQueryReady ifQuery = new RDBIfQuery<RDBUpdateQuery<T>>(this, _queryContext)
                {
                    Condition = rdbNotExistsCondition,
                    TrueQuery = clonedUpdateQuery
                };
                return ifQuery.GetResolvedQuery(context);
            }
            else
            {
                var resolveUpdateQueryContext = new RDBDataProviderResolveUpdateQueryContext(this.Table, this.ColumnValues, this.Condition, this.Joins, context, true);
                return context.DataProvider.ResolveUpdateQuery(resolveUpdateQueryContext);
            }
        }

        public T EndUpdate()
        {
            return _parent;
        }

        #region Private Classes

        #endregion
    }

    public class RDBUpdateColumn
    {
        public string ColumnName { get; set; }

        public BaseRDBExpression Value { get; set; }
    }

    public interface IUpdateQuery<T> :IUpdateQueryCanDefineTable<T>
    {
    }

    public interface IUpdateQueryTableDefined<T> : IUpdateQueryCanAssignColumns<T>, IUpdateQueryCanFilter<T>, IUpdateQueryCanJoin<T>, IUpdateQueryCanDefineNotExistsCondition<T>
    {

    }

    public interface IUpdateQueryReady<T> : IRDBQueryReady
    {
        T EndUpdate();
    }

    public interface IUpdateQueryColumnsAssigned<T> : IUpdateQueryReady<T>, IUpdateQueryCanAssignColumns<T>
    {

    }

    public interface IUpdateQueryFiltered<T> : IUpdateQueryReady<T>, IUpdateQueryCanAssignColumns<T>
    {

    }
    public interface IUpdateQueryNotExistsConditionDefined<T> : IUpdateQueryReady<T>, IUpdateQueryCanFilter<T>, IUpdateQueryCanAssignColumns<T>, IUpdateQueryCanJoin<T>
    {

    }
    public interface IUpdateQueryJoined<T> : IUpdateQueryReady<T>, IUpdateQueryCanFilter<T>, IUpdateQueryCanAssignColumns<T>
    {

    }

    public interface IUpdateQueryCanDefineTable<T>
    {
        IUpdateQueryTableDefined<T> FromTable(string tableName);

        IUpdateQueryTableDefined<T> FromTable(IRDBTableQuerySource table);
    }

    public interface IUpdateQueryCanAssignColumns<T>
    {
        IUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, BaseRDBExpression value);

        IUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, string value);

        IUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, int value);

        IUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, long value);

        IUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, decimal value);

        IUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, float value);

        IUpdateQueryColumnsAssigned<T> ColumnValue(string columnName, DateTime value);
    }

    public interface IUpdateQueryCanFilter<T>
    {
        RDBConditionContext<IUpdateQueryFiltered<T>> Where();
    }

    public interface IUpdateQueryCanJoin<T>
    {
        RDBJoinContext<IUpdateQueryJoined<T>> Join();
    }

    public interface IUpdateQueryCanDefineNotExistsCondition<T>
    {
        RDBConditionContext<IUpdateQueryNotExistsConditionDefined<T>> IfNotExists();
    }
}