using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Data.Postgres
{
    public class GetMeasureExpressionContext : IGetMeasureExpressionContext
    {
        Func<string, IGetMeasureExpressionContext, string> _getMeasureExpression;
        Func<string, bool> _isGroupingDimensionIncluded;

        public GetMeasureExpressionContext(Func<string, IGetMeasureExpressionContext, string> getMeasureExpression, Func<string, bool> isGroupingDimensionIncluded)
        {
            if (getMeasureExpression == null)
                throw new ArgumentNullException("getMeasureExpression");
            if (isGroupingDimensionIncluded == null)
                throw new ArgumentNullException("isGroupingDimensionIncluded");
            _getMeasureExpression = getMeasureExpression;
            _isGroupingDimensionIncluded = isGroupingDimensionIncluded;
        }

        public string GetMeasureExpression(string measureConfigName)
        {
            return _getMeasureExpression(measureConfigName, this);
        }


        public bool IsGroupingDimensionIncluded(string dimensionName)
        {
            return _isGroupingDimensionIncluded(dimensionName);
        }
    }
}
