using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRExpression
    {
        public string ExpressionString { get; set; }
    }

    public class VRExpressionVariable
    {
        public string VariableName { get; set; }

        public VRExpressionVariableValue ValueExpr { get; set; }
    }

    public abstract class VRExpressionVariableValue
    {
        public int ConfigId { get; set; }

        public abstract dynamic GetValue(IVRExpressionVariableValueContext context);
    }

    public interface IVRExpressionContext
    {

    }

    public interface IVRExpressionVariableValueContext
    {
        IVRExpressionContext ExpressionContext { get; }
    }
}
