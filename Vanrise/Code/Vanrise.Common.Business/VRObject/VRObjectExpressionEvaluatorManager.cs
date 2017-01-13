using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRObjectExpressionEvaluatorManager
    {
        public string EvaluateExpression(VRExpression expression, VRObjectExpressionEvaluator context)
        {
            if (string.IsNullOrEmpty(expression.ExpressionString))
                return null;

            string templateKeyLocal = string.Format("TemplateKey_{0}", Guid.NewGuid());
            var key = new RazorEngine.Templating.NameOnlyTemplateKey(templateKeyLocal, RazorEngine.Templating.ResolveType.Global, null);
            RazorEngine.Engine.Razor.AddTemplate(key, new RazorEngine.Templating.LoadedTemplateSource(expression.ExpressionString));
            RazorEngine.Engine.Razor.Compile(key, typeof(VRObjectExpressionEvaluator));

            var keyName = new RazorEngine.Templating.NameOnlyTemplateKey(templateKeyLocal, RazorEngine.Templating.ResolveType.Global, null);
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            RazorEngine.Engine.Razor.RunCompile(keyName, sw, typeof(VRObjectExpressionEvaluator), context);
            return sb.ToString();
        }
    }
}
