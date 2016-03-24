using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.Business
{
    public class MeasureValueEvaluatorContext : IMeasureValueEvaluatorContext
    {
        StringBuilder _instanceExecutionBlockBuilder;
        Dictionary<string, MeasureConfiguration> IMeasureValueEvaluatorContext.Measures
        {
            get { throw new NotImplementedException(); }
        }

        private string BuildClassDefinition(out string fullTypeName)
        {

            StringBuilder classDefinitionBuilder = new StringBuilder(@" 
                using System;
                using System.Collections.Generic;
                using System.IO;
                using System.Data;

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : #EXECUTORBASE#
                    {                        
                        #GLOBALMEMBERS#

                        object #EXECUTORBASE#.Evaluate()
                        {
                            #EXECUTIONCODE#
                        }
                    }
                }
                ");

            classDefinitionBuilder.Replace("#EXECUTORBASE#", typeof(IMeasureValueExecutor).FullName);
            //classDefinitionBuilder.Replace("#GLOBALMEMBERS#", _globalMembersBuilder.ToString());
            classDefinitionBuilder.Replace("#EXECUTIONCODE#", _instanceExecutionBlockBuilder.ToString());

            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.Analytic.Business");
            string className = "MeasureValueExecutor";
            classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
            classDefinitionBuilder.Replace("#CLASSNAME#", className);
            fullTypeName = String.Format("{0}.{1}", classNamespace, className);

            return classDefinitionBuilder.ToString();
        }
    }
}
