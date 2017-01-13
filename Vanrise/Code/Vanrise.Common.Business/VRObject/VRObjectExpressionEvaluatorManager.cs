using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRRazorEvaluator
    {
        public VRRazorCompilationOutput CompileExpression(VRExpression expression)
        {
            string templateKey = String.Format("ExpressionTemplate_{0}", Guid.NewGuid());
            string expressionString = expression.ExpressionString;
            if (expressionString == null)
                expressionString = "";
            var key = new RazorEngine.Templating.NameOnlyTemplateKey(templateKey, RazorEngine.Templating.ResolveType.Global, null);
            RazorEngine.Engine.Razor.AddTemplate(key, new RazorEngine.Templating.LoadedTemplateSource(expressionString));
            RazorEngine.Engine.Razor.Compile(key, typeof(VRMailContext));
            return new VRRazorCompilationOutput
            {
                CompiledExpressionKey = templateKey
            };
        }

        public string EvaluateExpression(Object compiledTemplateKey, VRObjectVariableCollection objectVariables, Dictionary<string, dynamic> objects)
        {
            var context = new VRObjectExpressionEvaluator(objectVariables, objects);
            var keyName = new RazorEngine.Templating.NameOnlyTemplateKey(compiledTemplateKey as string, RazorEngine.Templating.ResolveType.Global, null);
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            RazorEngine.Engine.Razor.RunCompile(keyName, sw, typeof(VRObjectExpressionEvaluator), context);
            return sb.ToString();
        }
    }
}
