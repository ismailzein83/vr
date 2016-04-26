using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Data.SQL
{
    public class GetMeasureExpressionContext : IGetMeasureExpressionContext
    {
        Func<string, IGetMeasureExpressionContext, string> _getMeasureExpression;

        public GetMeasureExpressionContext(Func<string, IGetMeasureExpressionContext, string> getMeasureExpression)
        {
            if (_getMeasureExpression == null)
                throw new ArgumentNullException("getMeasureExpression");
            _getMeasureExpression = getMeasureExpression;
        }

        public string GetMeasureExpression(string measureConfigName)
        {
            return _getMeasureExpression(measureConfigName, this);
        }
    }
}
