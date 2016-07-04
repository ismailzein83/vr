using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRExpressionVariableValueContext : IVRExpressionVariableValueContext
    {
        IVRExpressionContext _expressionContext;
        public VRExpressionVariableValueContext(IVRExpressionContext expressionContext)
        {
            _expressionContext = expressionContext;
        }
        public IVRExpressionContext ExpressionContext
        {
            get { return _expressionContext; }
        }
    }
}
