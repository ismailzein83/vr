using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBGroupByContext<T> : IRDBGroupByContextReady<T>, IRDBGroupByHavingDefined<T>, IRDBGroupByColumnsSelected<T>, IRDBGroupByAggregateColumnsSelected<T>
    {
        T _parent;
        RDBGroupBy _groupBy;
        IRDBTableQuerySource _table;
        public RDBGroupByContext(T parent, RDBGroupBy groupBy, IRDBTableQuerySource table)
        {
            _parent = parent;
            _groupBy = groupBy;
            _table = table;
        }

        public RDBSelectColumnsContext<IRDBGroupByColumnsSelected<T>> Select()
            {
                return new RDBSelectColumnsContext<IRDBGroupByColumnsSelected<T>>(this, _groupBy.Columns, _table);
            }
        

        RDBSelectAggregateContext<IRDBGroupByAggregateColumnsSelected<T>> IRDBGroupByCanSelectAggregateColumns<T>.SelectAggregates()
            {
                return new RDBSelectAggregateContext<IRDBGroupByAggregateColumnsSelected<T>>(this, _groupBy.AggregateColumns, _table);
            }
        

        RDBGroupByHavingContext<IRDBGroupByHavingDefined<T>> IRDBGroupByCanDefineHaving<T>.Having()
            {
                return new RDBGroupByHavingContext<IRDBGroupByHavingDefined<T>>(this, (condition) => _groupBy.HavingCondition = condition, _table);
            }
        

        T IRDBGroupByContextReady<T>.EndGroupBy()
        {
            return _parent;
        }
    }

    public interface IRDBGroupByContextReady<T>
    {
        T EndGroupBy();
    }

    public interface IRDBGroupByColumnsSelected<T> : IRDBGroupByContextReady<T>, IRDBGroupByCanSelectGroupByColumns<T>, IRDBGroupByCanSelectAggregateColumns<T>, IRDBGroupByCanDefineHaving<T>
    {
        

        
    }

    public interface IRDBGroupByAggregateColumnsSelected<T> : IRDBGroupByContextReady<T>, IRDBGroupByCanDefineHaving<T>
    {

    }

    public interface IRDBGroupByHavingDefined<T> : IRDBGroupByContextReady<T>
    {

    }

    public interface IRDBGroupByCanSelectGroupByColumns<T>
    {
        RDBSelectColumnsContext<IRDBGroupByColumnsSelected<T>> Select();
    }

    public interface IRDBGroupByCanSelectAggregateColumns<T>
    {
        RDBSelectAggregateContext<IRDBGroupByAggregateColumnsSelected<T>> SelectAggregates();
    }

    public interface IRDBGroupByCanDefineHaving<T>
    {
        RDBGroupByHavingContext<IRDBGroupByHavingDefined<T>> Having();
    }
}
