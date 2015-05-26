using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class GroupCountAggregate : IAggregate
    {

        Func<CDR, bool> _condition;
        Dictionary<int, int> _HoursvsCalls = new Dictionary<int, int>();
        int _MinCallsPerHour=5;


        public GroupCountAggregate(Func<CDR, bool> condition)
        {
            this._condition = condition;
        }

        public void Reset()
        {
            _HoursvsCalls.Clear();
        }

        public void EvaluateCDR(CDR cdr)
        {
            if (this._condition == null || this._condition(cdr))
            {
                    int value;
                    _HoursvsCalls.TryGetValue(cdr.ConnectDateTime.Value.Hour, out value);

                    if ( value==0)
                    {
                        _HoursvsCalls.Add(cdr.ConnectDateTime.Value.Hour, 1);
                    }
                    else
                    {
                        _HoursvsCalls[cdr.ConnectDateTime.Value.Hour] = ++value;
                    }
            }
        }

        public decimal GetResult()
        {
            return _HoursvsCalls.Where(x => x.Value >= _MinCallsPerHour).Count();
        }


    }
    
}