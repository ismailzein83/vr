using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRExpressionManager
    {
        public string EvaluateExpression(VRExpression expression, List<VRExpressionVariable> variables, IVRExpressionContext context)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (expression.ExpressionString == null)
                throw new ArgumentNullException("expression.ExpressionString");
            StringBuilder valueBuilder = new StringBuilder(expression.ExpressionString);
            if(variables != null)
            {
                foreach(var variable in variables)
                {
                    var variableContext = new VRExpressionVariableValueContext(context);
                    var value = variable.ValueExpr.GetValue(variableContext);
                    valueBuilder.Replace(variable.VariableName, value);
                }
            }
            return valueBuilder.ToString();
        }
    }
}
