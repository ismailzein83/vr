﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;

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
            }";

        const string RDBReaderValueGetter_CLASS_CODETEMPLATE = @"
            public class #CLASSNAME# : Vanrise.Analytic.Data.RDB.IAnalyticItemRDBReaderValueGetter
            {
                public Object GetReaderValue(Vanrise.Data.RDB.IRDBDataReader reader, string fieldName)
                {
                    return reader.#METHODNAME#(fieldName);
                }
            }";

        const string JoinRDBExpressionSetter_CLASS_CODETEMPLATE = @"
            public class #CLASSNAME# : Vanrise.Analytic.Data.RDB.IAnalyticJoinRDBExpressionSetter
            {
                public void SetExpression(Vanrise.Analytic.Data.RDB.IAnalyticJoinRDBExpressionSetterContext context)
                {
                    #CODE#
                }
            }";

        public static ResolvedConfigs GetResolvedConfigs(IAnalyticTableQueryContext queryContext)
        {
            Guid tableId = queryContext.GetTable().AnalyticTableId;
            String cacheName = String.Concat("Vanrise.Analytic.Data.RDB_GetDynamicManager_", tableId);

            return queryContext.GetOrCreateCachedObjectBasedOnItemConfig(cacheName,
                () =>
                {                    
                    List<ResolvedAnalyticDimensionConfig> resolvedDimensionConfigs = new List<ResolvedAnalyticDimensionConfig>();
                    List<ResolvedAnalyticAggregateConfig> resolvedAggregateConfigs = new List<ResolvedAnalyticAggregateConfig>();
                    List<ResolvedAnalyticJoinConfig> resolvedJoinConfigs = new List<ResolvedAnalyticJoinConfig>();
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
                            var runtimeType = aggConfig.Config.FieldType != null ? aggConfig.Config.FieldType.GetRuntimeType() : typeof(Decimal?);
                            GenerateAndAddRDBReaderValueGetterClass(runtimeType, itemId, itemName, resolvedAggregateConfig, classes, actionsAfterCompilation);
                        }
                    }

                    if(queryContext.Joins != null)
                    {
                        foreach(var joinConfigEntry in queryContext.Joins)
                        {
                            var joinConfig = joinConfigEntry.Value;
                            var resolvedJoinConfig = new ResolvedAnalyticJoinConfig
                            {
                                JoinName = joinConfigEntry.Key,
                                JoinConfig = joinConfig
                            };
                            resolvedJoinConfigs.Add(resolvedJoinConfig);
                            GenerateAndAddJoinRDBExpressionSetterClass(resolvedJoinConfig, classes, actionsAfterCompilation);
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
                        AggregateConfigs = resolvedAggregateConfigs.ToDictionary(itm => itm.AggregateConfig.Name, itm => itm),
                        JoinConfigs = resolvedJoinConfigs.ToDictionary(itm => itm.JoinName, itm => itm)
                    };
                    return resolvedConfigs;
                });
        }

        private static void GenerateAndAddRDBExpressionSetterClass(string sqlExpression, Guid itemId, string itemName, IResolvedAnalyticItemConfig resolvedDimensionConfig, List<string> classes, List<Action<Assembly, string>> actionsAfterCompilation)
        {
            if(String.IsNullOrWhiteSpace(sqlExpression))
                return;

            string className = string.Concat("RDBExpressionSetter", itemName, "_", itemId.ToString().Replace("-", ""));

            StringBuilder rdbExpressionSetterClassBuilder = new StringBuilder(RDBExpressionSetter_CLASS_CODETEMPLATE);
            rdbExpressionSetterClassBuilder.Replace("#CLASSNAME#", className);

            if (sqlExpression.Contains("RDBExpressionContext."))
                rdbExpressionSetterClassBuilder.Replace("#CODE#", sqlExpression);
            else
                rdbExpressionSetterClassBuilder.Replace("#CODE#", $@"context.RDBExpressionContext.Column(""{sqlExpression}"");");

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
            string className = string.Concat("RDBReaderValueGetter", itemName, "_", itemId.ToString().Replace("-", ""));

            StringBuilder rdbReaderValueGetterClassBuilder = new StringBuilder(RDBReaderValueGetter_CLASS_CODETEMPLATE);
            rdbReaderValueGetterClassBuilder.Replace("#CLASSNAME#", className);
            rdbReaderValueGetterClassBuilder.Replace("#METHODNAME#", RDBUtilities.GetGetReaderValueMethodNameWithValidate(fieldType, true));

            classes.Add(rdbReaderValueGetterClassBuilder.ToString());

            actionsAfterCompilation.Add((assbly, nmspace) =>
            {
                Type type = assbly.GetType($"{nmspace}.{className}");
                type.ThrowIfNull("type", className);
                resolvedDimensionConfig.ReaderValueGetter = Activator.CreateInstance(type).CastWithValidate<IAnalyticItemRDBReaderValueGetter>("RDBReaderValueGetter", className);
            });
        }

        private static void GenerateAndAddJoinRDBExpressionSetterClass(ResolvedAnalyticJoinConfig resolvedJoinConfig, List<string> classes, List<Action<Assembly, string>> actionsAfterCompilation)
        {
            string className = string.Concat("JoinRDBExpressionSetter", "_", resolvedJoinConfig.JoinName);

            StringBuilder joinRDBExpressionSetterClassBuilder = new StringBuilder(JoinRDBExpressionSetter_CLASS_CODETEMPLATE);
            joinRDBExpressionSetterClassBuilder.Replace("#CLASSNAME#", className);
            joinRDBExpressionSetterClassBuilder.Replace("#CODE#", resolvedJoinConfig.JoinConfig.Config.JoinStatement);
            classes.Add(joinRDBExpressionSetterClassBuilder.ToString());

            actionsAfterCompilation.Add((assbly, nmspace) =>
            {
                Type type = assbly.GetType($"{nmspace}.{className}");
                type.ThrowIfNull("type", className);
                resolvedJoinConfig.JoinRDBExpressionSetter = Activator.CreateInstance(type).CastWithValidate<IAnalyticJoinRDBExpressionSetter>("JoinRDBExpressionSetter", className);
            });
        }
    }
}
