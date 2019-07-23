﻿using System;
using System.Collections.Generic;

namespace Vanrise.Data.RDB
{
    public class RDBJoinContext
    {
        RDBQueryBuilderContext _queryBuilderContext;
        List<RDBJoin> _joins;
        string _tableAlias;

        public RDBQueryBuilderContext QueryBuilderContext { get { return _queryBuilderContext; } }

        internal RDBJoinContext(RDBQueryBuilderContext queryBuilderContext, List<RDBJoin> joins, string tableAlias)
        {
            _queryBuilderContext = queryBuilderContext;
            _joins = joins;
            _tableAlias = tableAlias;
        }

        public void JoinOnEqualOtherTableColumn(string tableName, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName)
        {
            JoinOnEqualOtherTableColumn(RDBJoinType.Inner, tableName, tableAlias, columnName, otherTableAlias, otherTableColumnName);
        }

        public void JoinOnEqualOtherTableColumn(RDBJoinType joinType, string tableName, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName)
        {
            JoinOnEqualOtherTableColumn(joinType, tableName, tableAlias, columnName, otherTableAlias, otherTableColumnName, false);
        }

        public void JoinOnEqualOtherTableColumn(RDBJoinType joinType, string tableName, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName, bool withNoLock)
        {
            JoinOnEqualOtherTableColumn(joinType, new RDBTableDefinitionQuerySource(tableName), tableAlias, columnName, otherTableAlias, otherTableColumnName, withNoLock);
        }

        public void JoinOnEqualOtherTableColumn(IRDBTableQuerySource table, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName)
        {
            JoinOnEqualOtherTableColumn(RDBJoinType.Inner, table, tableAlias, columnName, otherTableAlias, otherTableColumnName);
        }

        public void JoinOnEqualOtherTableColumn(RDBJoinType joinType, IRDBTableQuerySource table, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName)
        {
            JoinOnEqualOtherTableColumn(joinType, table, tableAlias, columnName, otherTableAlias, otherTableColumnName, false);
        }

        public void JoinOnEqualOtherTableColumn(RDBJoinType joinType, IRDBTableQuerySource table, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName, bool withNoLock)
        {
            var join = AddJoin(table, tableAlias);
            join.JoinType = joinType;
            join.WithNoLock = withNoLock;

            join.Condition = new RDBCompareCondition
            {
                Expression1 = new RDBColumnExpression(_queryBuilderContext, tableAlias, columnName),
                Operator = RDBCompareConditionOperator.Eq,
                Expression2 = new RDBColumnExpression(_queryBuilderContext, otherTableAlias, otherTableColumnName)
            };
        }

        public RDBJoinStatementContext Join(string tableName, string tableAlias)
        {
            return Join(new RDBTableDefinitionQuerySource(tableName), tableAlias);
        }

        public RDBJoinStatementContext Join(string databaseName, string tableName, string tableAlias)
        {
            return Join(new RDBTableDefinitionQuerySource(databaseName, tableName), tableAlias);
        }

        public RDBJoinStatementContext Join(IRDBTableQuerySource table, string tableAlias)
        {
            var join = AddJoin(table, tableAlias);
            return new RDBJoinStatementContext(_queryBuilderContext, join, _tableAlias);
        }

        public RDBJoinSelectContext JoinSelect(string inlineQueryAlias)
        {
            var selectQuery = new RDBSelectQuery(_queryBuilderContext.CreateChildContext(), false);
            var join = AddJoin(selectQuery, inlineQueryAlias);
            return new RDBJoinSelectContext(_queryBuilderContext, join, _tableAlias, selectQuery);
        }

        private RDBJoin AddJoin(IRDBTableQuerySource table, string tableAlias)
        {
            _queryBuilderContext.AddTableAlias(table, tableAlias);

            RDBJoin join = new RDBJoin { Table = table, TableAlias = tableAlias };
            _joins.Add(join);
            return join;
        }

        internal void FinalizeBeforeResolveQuery(BaseRDBDataProvider dataProvider, Func<RDBConditionContext> getWhere)
        {
            if (this._joins != null && this._joins.Count > 0)
            {
                foreach (var join in new List<RDBJoin>(_joins))
                {
                    join.Table.FinalizeBeforeResolveQuery(new RDBTableQuerySourceFinalizeBeforeResolveQueryContext(dataProvider, join.TableAlias, getWhere));
                }
            }
        }
    }
}