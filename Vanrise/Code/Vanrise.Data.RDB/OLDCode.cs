using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    #region To Remove

    #region RDB Values

    //public abstract class RDBTextValue
    //{
    //}

    //public class RDBFixedTextValue : RDBTextValue
    //{
    //    public string Value { get; set; }
    //}

    //public abstract class RDBNumberValue
    //{
    //}

    //public class RDBFixedDecimalValue : RDBNumberValue
    //{
    //    public Decimal Value { get; set; }
    //}

    //public class RDBFixedLongValue : RDBNumberValue
    //{
    //    public long Value { get; set; }
    //}

    //public class RDBFixedIntValue : RDBNumberValue
    //{
    //    public int Value { get; set; }
    //}

    //public class RDBFixedFloatValue : RDBNumberValue
    //{
    //    public float Value { get; set; }
    //}

    //public abstract class RDBDateTimeValue
    //{
    //}

    //public class RDBFixedDateTimeValue : RDBDateTimeValue
    //{
    //    public DateTime Value { get; set; }
    //}

    //public class RDBNowDateTimeValue : RDBDateTimeValue
    //{

    //}

    #endregion

    //public enum RDBTextConditionOperator { Eq = 0, NEq = 1, StartWith = 2, EndWith = 3, Contains = 4 }

    //public class RDBTextCondition : BaseRDBCondition
    //{
    //    public string ColumnName { get; set; }

    //    public RDBTextConditionOperator Operator { get; set; }

    //    public RDBTextValue Value { get; set; }
    //}

    //public class RDBTextListCondition : BaseRDBCondition
    //{
    //    public string ColumnName { get; set; }

    //    public RDBListConditionOperator Operator { get; set; }

    //    public List<RDBTextValue> Values { get; set; }
    //}

    //public enum RDBNumberConditionOperator { Eq = 0, NEq = 1, G = 2, GEq = 3, L = 4, LEq = 5 }

    //public class RDBNumberCondition : BaseRDBCondition
    //{
    //    public string ColumnName { get; set; }

    //    public RDBNumberConditionOperator Operator { get; set; }

    //    public RDBNumberValue Value { get; set; }
    //}

    //public class RDBNumberListCondition : BaseRDBCondition
    //{
    //    public string ColumnName { get; set; }

    //    public RDBListConditionOperator Operator { get; set; }

    //    public List<RDBNumberValue> Values { get; set; }
    //}

    //public enum RDBDateTimeConditionOperator { Eq = 0, NEq = 1, G = 2, GEq = 3, L = 4, LEq = 5 }

    //public class RDBDateTimeCondition : BaseRDBCondition
    //{
    //    public string ColumnName { get; set; }

    //    public RDBDateTimeConditionOperator Operator { get; set; }

    //    public RDBDateTimeValue Value { get; set; }
    //}

    //public class RDBDateTimeListCondition : BaseRDBCondition
    //{
    //    public string ColumnName { get; set; }

    //    public RDBListConditionOperator Operator { get; set; }

    //    public List<RDBDateTimeValue> Values { get; set; }
    //}

    #endregion

    //public class RDBQueryBuilder
    //{
    //    StringBuilder _builder = new StringBuilder();
    //    public RDBSelect Select()
    //    {
    //        return new RDBSelect(_builder);
    //    }


    //}

    //public class RDBInsert
    //{
    //    StringBuilder _builder;
    //    public RDBInsert(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }

    //    public RDBInsertTable IntoTable(string tableName)
    //    {
    //        _builder.AppendFormat("INSERT INTO {0}", tableName);
    //        return new RDBInsertTable(_builder);
    //    }
    //}

    //public class RDBInsertTable
    //{
    //    StringBuilder _builder;

    //    public RDBInsertTable(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }

    //    public void Value(string columnName, Object value)
    //    {

    //    }

    //    public void DBNow(string columnName)
    //    {

    //    }

    //    public void DBToday(string columnName)
    //    {

    //    }
    //}

    //public class RDBSelect
    //{
    //    StringBuilder _builder;
    //    public RDBSelect(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }

    //    public RDBSelectTable FromTable(string tableName, string alias)
    //    {
    //        _builder.AppendFormat("{0} {1}", tableName, alias);
    //        return new RDBSelectTable(_builder);
    //    }

    //    public RDBSelectTable FromTable(string tableName)
    //    {
    //        return FromTable(tableName, null);
    //    }
    //}

    //public class RDBSelectTable
    //{
    //    StringBuilder _builder;

    //    public RDBSelectTable(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }

    //    public RDBSelectTableWithColumns Columns(params string[] columnNames)
    //    {            
    //        for (int i = 0; i < columnNames.Length; i++)
    //        {
    //            _builder.Append(columnNames[i]);
    //            if (i < columnNames.Length - 1)
    //                _builder.Append(", ");
    //        }
    //        return new RDBSelectTableWithColumns(_builder);
    //    }

    //    public RDBSelectTableWithColumns Column(string column, string alias)
    //    {
    //        _builder.Append(String.Format("{0} {1}", column, alias));
    //        return new RDBSelectTableWithColumns(_builder);
    //    }
    //}

    //public class RDBSelectTableWithColumns
    //{
    //    StringBuilder _builder;

    //    public RDBSelectTableWithColumns(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }

    //    public RDBSelectTableWithColumns Columns(params string[] columnNames)
    //    {
    //        for (int i = 0; i < columnNames.Length; i++)
    //        {
    //            _builder.Append(columnNames[i]);
    //            if (i < columnNames.Length - 1)
    //                _builder.Append(", ");
    //        }
    //        return this;
    //    }

    //    public RDBSelectTableWithColumns Column(string column, string alias)
    //    {
    //        _builder.Append(String.Format("{0} {1}", column, alias));
    //        return this;
    //    }

    //    public RDBJoin Join(string tableName, string alias, RDBJoinType joinType)
    //    {
    //        string joinStatement = joinType == RDBJoinType.Inner ? "JOIN " : "LEFT JOIN ";
    //        _builder.AppendFormat("{0} {1} {2}", joinStatement, tableName, alias);
    //        return new RDBJoin(_builder);
    //    }

    //    public RDBSelectWhere Where()
    //    {
    //        _builder.AppendLine("WHERE ");
    //        return new RDBSelectWhere(_builder);
    //    }

    //}

    //public class RDBSelectTableWithJoin
    //{
    //    StringBuilder _builder;
    //    public RDBSelectTableWithJoin(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }
    //    public RDBSelectWhere Where()
    //    {
    //        _builder.AppendLine("WHERE ");
    //        return new RDBSelectWhere(_builder);
    //    }
    //}

    //public class RDBSelectTableWithWhere
    //{
    //    StringBuilder _builder;
    //    public RDBSelectTableWithWhere(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }
    //}

    //public enum RDBJoinType { Inner = 0, Left = 1 }

    //public enum RDBConditionOperator { Eq = 0, NEq = 1, G = 2, GEq = 3, L = 4, LEq = 5, IN = 6, NotIN = 7 }
    //public abstract class BaseRDBConditionGroup<T>
    //{
    //    #region ctor/Fields

    //    protected StringBuilder _builder;
    //    public BaseRDBConditionGroup(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }

    //    #endregion

    //    #region Public Methods

    //    protected abstract T ReturnThis();

    //    public T TextCondition(string columnName, RDBConditionOperator condOperator, params string[] values)
    //    {
    //        return Condition<string>(columnName, condOperator, (val) => string.Format("'{0}'", val), values);
    //    }

    //    public T IntCondition(string columnName, RDBConditionOperator condOperator, params int[] values)
    //    {
    //        return Condition<int>(columnName, condOperator, (val) => val.ToString(), values);
    //    }

    //    public T LongCondition(string columnName, RDBConditionOperator condOperator, params long[] values)
    //    {
    //        return Condition<long>(columnName, condOperator, (val) => val.ToString(), values);
    //    }

    //    public T DecimalCondition(string columnName, RDBConditionOperator condOperator, params decimal[] values)
    //    {
    //        return Condition<decimal>(columnName, condOperator, (val) => val.ToString(), values);
    //    }

    //    public RDBConditionGroup<T> ConditionGroup()
    //    {
    //        _builder.Append(" (");
    //        return new RDBConditionGroup<T>(_builder, ReturnThis());
    //    }

    //    #endregion

    //    #region Private Methods

    //    T Condition<Q>(string columnName, RDBConditionOperator condOperator, Func<Q, string> valueToQuery, params Q[] values)
    //    {
    //        _builder.AppendFormat("{0} {1} ", columnName, GetConditionOperatorText(condOperator));
    //        if (condOperator == RDBConditionOperator.IN || condOperator == RDBConditionOperator.NotIN)
    //        {
    //            _builder.Append("(");
    //            for (int i = 0; i < values.Length; i++)
    //            {
    //                _builder.Append(valueToQuery(values[i]));
    //                if (i < values.Length - 1)
    //                    _builder.Append(", ");
    //            }
    //            _builder.Append(")");
    //        }
    //        else
    //        {
    //            _builder.AppendFormat(valueToQuery(values[0]));
    //        }
    //        return ReturnThis();
    //    }

    //    string GetConditionOperatorText(RDBConditionOperator condOperator)
    //    {
    //        switch (condOperator)
    //        {
    //            case RDBConditionOperator.Eq: return "=";
    //            case RDBConditionOperator.NEq: return "<>";
    //            case RDBConditionOperator.G: return ">";
    //            case RDBConditionOperator.GEq: return ">=";
    //            case RDBConditionOperator.L: return "<";
    //            case RDBConditionOperator.LEq: return "<=";
    //            case RDBConditionOperator.IN: return "IN";
    //            case RDBConditionOperator.NotIN: return "NOT IN";
    //            default: throw new NotSupportedException(string.Format("condOperator '{0}'", condOperator));
    //        }
    //    }

    //    #endregion
    //}

    //public abstract class RDBCondition<T>
    //{
    //     #region ctor/Fields

    //    protected StringBuilder _builder;
    //    public RDBCondition(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }

    //    #endregion

    //    protected abstract T ReturnThis();

    //    public RDBLogicalOperator<T> And()
    //    {
    //        return new RDBLogicalOperator<T>(_builder, ReturnThis());
    //    }

    //    public RDBLogicalOperator<T> Or()
    //    {
    //        return new RDBLogicalOperator<T>(_builder, ReturnThis());
    //    }
    //}

    //public class RDBLogicalOperator<T> : BaseRDBConditionGroup<T>
    //{
    //    #region ctor/Fields

    //    protected StringBuilder _builder;
    //    T _originalObj;
    //    public RDBLogicalOperator(StringBuilder builder, T originalObj) : base(builder)
    //    {
    //        _builder = builder;
    //        _originalObj = originalObj;
    //    }

    //    #endregion

    //    protected override T ReturnThis()
    //    {
    //        return _originalObj;
    //    }
    //}

    //public class RDBConditionGroup<Q> : BaseRDBConditionGroup<RDBConditionGroupReady<Q>>
    //{
    //    #region ctor/Fields

    //    Q _originalObject;

    //    public RDBConditionGroup(StringBuilder builder, Q originalObject)
    //        : base(builder)
    //    {
    //        _originalObject = originalObject;
    //    }

    //    #endregion

    //    protected override RDBConditionGroupReady<Q> ReturnThis()
    //    {
    //        return new RDBConditionGroupReady<Q>(_builder, _originalObject);
    //    }

    //}

    //public class RDBConditionGroupReady<Q> : RDBCondition<RDBConditionGroupReady<Q>>
    //{
    //    #region ctor/Fields

    //    Q _originalObject;

    //    public RDBConditionGroupReady(StringBuilder builder, Q originalObject)
    //        : base(builder)
    //    {
    //        _originalObject = originalObject;
    //    }

    //    #endregion

    //    protected override RDBConditionGroupReady<Q> ReturnThis()
    //    {
    //        return this;
    //    }
    //    public Q EndConditionGroup()
    //    {
    //        _builder.Append(" )");
    //        return _originalObject;
    //    }
    //}

    //public class RDBSelectWhere : BaseRDBConditionGroup<RDBSelectWhereReady>
    //{
    //    #region ctor/Fields

    //    public RDBSelectWhere(StringBuilder builder)
    //        : base(builder)
    //    {
    //    }

    //    #endregion

    //    protected override RDBSelectWhereReady ReturnThis()
    //    {
    //        return new RDBSelectWhereReady(_builder);
    //    }
    //}

    //public class RDBSelectWhereReady : RDBCondition<RDBSelectWhereReady>
    //{
    //    #region ctor/Fields
    //    public RDBSelectWhereReady(StringBuilder builder) : base(builder)
    //    {
    //    }

    //    #endregion

    //    protected override RDBSelectWhereReady ReturnThis()
    //    {
    //        return this;
    //    }
    //    public RDBSelectTableWithWhere EndWhere()
    //    {
    //        return new RDBSelectTableWithWhere(_builder);
    //    }
    //}

    //public class RDBJoin : BaseRDBConditionGroup<RDBJoinReady>
    //{
    //    #region ctor/Fields

    //    public RDBJoin(StringBuilder builder)
    //        : base(builder)
    //    {
    //    }

    //    #endregion

    //    protected override RDBJoinReady ReturnThis()
    //    {
    //        return new RDBJoinReady(_builder);
    //    }
    //}

    //public class RDBJoinReady : RDBCondition<RDBJoinReady>
    //{
    //    #region ctor/Fields
    //    public RDBJoinReady(StringBuilder builder)
    //        : base(builder)
    //    {
    //    }

    //    #endregion

    //    protected override RDBJoinReady ReturnThis()
    //    {
    //        return this;
    //    }
    //    public RDBSelectTableWithJoin EndJoin()
    //    {
    //        return new RDBSelectTableWithJoin(_builder);
    //    }
    //}
}
