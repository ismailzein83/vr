using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Data.RDB
{
    internal class DynamicTypeGenerator
    {
        const string RDBExpressionSetter_CLASS_CODETEMPLATE = @"

public class #CLASSNAME# : Vanrise.Analytic.Data.RDB.IAnalyticItemRDBExpressionSetter
{
    public void SetExpression(Vanrise.Analytic.Data.RDB.IAnalyticItemRDBExpressionSetterContext context)
    {
        #CODE#
    }
}

";

        const string RDBReaderValueGetter_CLASS_CODETEMPLATE = @"

public class #CLASSNAME# : Vanrise.Analytic.Data.RDB.IAnalyticItemRDBReaderValueGetter
{
    public Object GetReaderValue(Vanrise.Data.RDB.IRDBDataReader reader, string fieldName)
    {
        return reader.#METHODNAME#(fieldName);
    }
}

";
        public static ResolvedConfigs GetResolvedConfigs(IAnalyticTableQueryContext queryContext)
        {
            Guid tableId = queryContext.GetTable().AnalyticTableId;
            String cacheName = String.Concat("Vanrise.Analytic.Data.RDB_GetDynamicManager_", tableId);
            return queryContext.GetOrCreateCachedObjectBasedOnItemConfig(cacheName,
                () =>
                {                    
                    List<ResolvedAnalyticDimensionConfig> resolvedDimensionConfigs = new List<ResolvedAnalyticDimensionConfig>();
                    List<ResolvedAnalyticAggregateConfig> resolvedAggregateConfigs = new List<ResolvedAnalyticAggregateConfig>();
                    StringBuilder codeBuilder = new StringBuilder(@"
                using System;                
                using System.Data;
                using System.Linq;
                using System.Collections.Generic;

                namespace #NAMESPACE#
                {
                    #CLASSES#
                }");

                    List<string> classes = new List<string>();
                    List<Action<Assembly, string>> actionsAfterCompilation = new List<Action<Assembly, string>>();
                    if (queryContext.Dimensions != null)
                    {
                        foreach (var dimConfig in queryContext.Dimensions.Values)
                        {
                            var resolvedDimensionConfig = new ResolvedAnalyticDimensionConfig { DimensionConfig = dimConfig };
                            resolvedDimensionConfigs.Add(resolvedDimensionConfig);
                            Guid itemId = dimConfig.AnalyticDimensionConfigId;
                            string itemName = dimConfig.Name;                           
                            GenerateAndAddRDBExpressionSetterClass(dimConfig.Config.SQLExpression, itemId, itemName, resolvedDimensionConfig, classes, actionsAfterCompilation);
                            GenerateAndAddRDBReaderValueGetterClass(dimConfig.Config.FieldType.GetRuntimeType(), itemId, itemName, resolvedDimensionConfig, classes, actionsAfterCompilation);
                        }
                    }

                    if(queryContext.Aggregates != null)
                    {
                        foreach(var aggConfig in queryContext.Aggregates.Values)
                        {
                            var resolvedAggregateConfig = new ResolvedAnalyticAggregateConfig { AggregateConfig = aggConfig };
                            resolvedAggregateConfigs.Add(resolvedAggregateConfig);
                            Guid itemId = aggConfig.AnalyticAggregateConfigId;
                            string itemName = aggConfig.Name;
                            GenerateAndAddRDBExpressionSetterClass(aggConfig.Config.SQLColumn, itemId, itemName, resolvedAggregateConfig, classes, actionsAfterCompilation);
                            GenerateAndAddRDBReaderValueGetterClass(typeof(Decimal), itemId, itemName, resolvedAggregateConfig, classes, actionsAfterCompilation);
                        }
                    }
                    
                    string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.Analytic.Data.RDB");

                    codeBuilder.Replace("#NAMESPACE#", classNamespace);
                    codeBuilder.Replace("#CLASSES#", string.Join("", classes));

                    CSharpCompilationOutput compilationOutput;
                    if (!CSharpCompiler.TryCompileClass(String.Format("RDBDataProvider_{0}", tableId), codeBuilder.ToString(), out compilationOutput))
                    {
                        StringBuilder errorsBuilder = new StringBuilder();
                        if (compilationOutput.ErrorMessages != null)
                        {
                            foreach (var errorMessage in compilationOutput.ErrorMessages)
                            {
                                errorsBuilder.AppendLine(errorMessage);
                            }
                        }
                        throw new Exception(String.Format("Compile Error when building RDBDataProvider for Analytic Table Id'{0}'. Errors: {1}",
                            tableId, errorsBuilder));
                    }
                    else
                    {
                        foreach(var afterCompilationAct in actionsAfterCompilation)
                        {
                            afterCompilationAct(compilationOutput.OutputAssembly, classNamespace);
                        }
                    }
                    ResolvedConfigs resolvedConfigs = new ResolvedConfigs
                    {
                        DimensionConfigs = resolvedDimensionConfigs.ToDictionary(itm => itm.DimensionConfig.Name, itm => itm),
                        AggregateConfigs = resolvedAggregateConfigs.ToDictionary(itm => itm.AggregateConfig.Name, itm => itm)
                    };
                    return resolvedConfigs;
                });
        }

        private static void GenerateAndAddRDBExpressionSetterClass(string sqlExpression, Guid itemId, string itemName, IResolvedAnalyticItemConfig resolvedDimensionConfig, List<string> classes, List<Action<Assembly, string>> actionsAfterCompilation)
        {
            if(String.IsNullOrWhiteSpace(sqlExpression) || !sqlExpression.Contains("RDBExpressionContext."))
                return;
            StringBuilder rdbExpressionSetterClassBuilder = new StringBuilder(RDBExpressionSetter_CLASS_CODETEMPLATE);
            string className = string.Concat("RDBExpressionSetter", itemName, "_", itemId.ToString().Replace("-", ""));
            rdbExpressionSetterClassBuilder.Replace("#CLASSNAME#", className);
            rdbExpressionSetterClassBuilder.Replace("#CODE#", sqlExpression);
            classes.Add(rdbExpressionSetterClassBuilder.ToString());
            actionsAfterCompilation.Add((assbly, nmspace) =>
            {
                Type type = assbly.GetType($"{nmspace}.{className}");
                type.ThrowIfNull("type", className);
                resolvedDimensionConfig.RDBExpressionSetter = Activator.CreateInstance(type).CastWithValidate<IAnalyticItemRDBExpressionSetter>("RDBExpressionSetter", className);
            });
        }

        private static void GenerateAndAddRDBReaderValueGetterClass(Type fieldType, Guid itemId, string itemName, IResolvedAnalyticItemConfig resolvedDimensionConfig, List<string> classes, List<Action<Assembly, string>> actionsAfterCompilation)
        {
            StringBuilder rdbReaderValueGetterClassBuilder = new StringBuilder(RDBReaderValueGetter_CLASS_CODETEMPLATE);
            string className = string.Concat("RDBReaderValueGetter", itemName, "_", itemId.ToString().Replace("-", ""));
            rdbReaderValueGetterClassBuilder.Replace("#CLASSNAME#", className);
            rdbReaderValueGetterClassBuilder.Replace("#METHODNAME#", RDBUtilities.GetGetReaderValueMethodNameWithValidate(fieldType));
            classes.Add(rdbReaderValueGetterClassBuilder.ToString());
            actionsAfterCompilation.Add((assbly, nmspace) =>
            {
                Type type = assbly.GetType($"{nmspace}.{className}");
                type.ThrowIfNull("type", className);
                resolvedDimensionConfig.ReaderValueGetter = Activator.CreateInstance(type).CastWithValidate<IAnalyticItemRDBReaderValueGetter>("RDBReaderValueGetter", className);
            });
        }
    }
}
