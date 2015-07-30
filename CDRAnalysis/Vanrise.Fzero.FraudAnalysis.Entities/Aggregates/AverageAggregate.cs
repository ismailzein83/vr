//using System;
//using System.Collections.Generic;
//using System.Reflection;

//namespace Vanrise.Fzero.FraudAnalysis.Entities
//{
//    public class AverageAggregate : IAggregate
//    {

//        Func<CDR, bool> _condition;
//        Func<CDR, Decimal> _cdrExpressionToSum;
//        decimal _sum;
//        int _count;

//        Func<CDR, Strategy, Decimal> cdrExpressionToSumWithStrategy;
//        Dictionary<Strategy, AverageAggregateStrategyInfo> _strategiesInfo;
//        List<Strategy> _strategies;
     


//        public void Reset()
//        {
//            this._sum = 0;
//            this._count = 0;
//        }

//        public void EvaluateCDR(CDR cdr)
//        {
//            if (this._condition == null || this._condition(cdr))
//            {
//                if (this._cdrExpressionToSum != null)
//                {
//                    this._sum += this._cdrExpressionToSum(cdr);
//                    this._count++;
//                }
                    
//            }
//        }

//        public decimal GetResult()
//        {
//            if (this._sum == 0 || this._count == 0)
//                return 0;
//            else
//                return _sum / _count;
//        }



//        private class AverageAggregateStrategyInfo
//        {
//            public decimal Sum { get; set; }
//            public decimal Count { get; set; }
//        }



















//    }

    
//}