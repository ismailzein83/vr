using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Common;
using Vanrise.Security.Data.SQL;
using Vanrise.Data.RDB;
using Vanrise.Runtime.Entities;
using Vanrise.AccountBalance.Data.RDB;
using Vanrise.AccountBalance.Entities;
using Vanrise.Entities;
using System.Activities.Statements;
using System.Activities;
using Microsoft.CSharp.Activities;
using System.Activities.Expressions;
using System.Activities.XamlIntegration;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.MainExtensions.VRWorkflowVariableTypes;
using Vanrise.GenericData.MainExtensions.DataRecordFields;

namespace Retail.Runtime.Tasks
{
    public class IsmailTask : ITask
    {
      
        public void Execute()
        {           
            
            CallGetAnalyticRecords();
            //GenerateVRWorkflow();
            //CreateWFProgrammatically();
            //Parallel.For(0, 1, (i) =>
            //    {
            //        TestStringConcatenation();
            //    });
            //Console.ReadKey();
            //GenerateRuntimeNodeConfiguration();
            //TestAppDomain();
            //Console.ReadKey();
            //TestRDBSelectQuery();
            //var analyticTableMeasureExternalSource =
            //    new Vanrise.Analytic.Entities.AnalyticMeasureExternalSourceConfig
            //    {
            //        ExtendedSettings =
            //            new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.AnalyticTableMeasureExternalSource
            //            {
            //                AnalyticTableId = new Guid("4F4C1DC0-6024-4AB9-933D-20F456360112"),
            //                DimensionMappingRules = new List<Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule>
            //    {
            //        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule { Settings= new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.ExcludeDimensions { ExcludedDimensions = new List<string> { "TrafficDirection" }}},
            //        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule { Settings= new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SameDimensionName()}
            //    },
            //                MeasureMappingRules = new List<Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule>
            //                {
            //                     new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
            //                     {
            //                          Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
            //                          {
            //                               MeasureName = "CountCDRs",
            //                                MappedMeasures = new List<string> {"CountDistinctOnNetLocalCDRs"}
            //                          }
            //                     },
            //                     new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
            //                     {
            //                          Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
            //                          {
            //                               MeasureName = "TotalDurationInSeconds",
            //                                MappedMeasures = new List<string> {"TotalDurationInSecDistinctOnNetLocalCDRs"}
            //                          }
            //                     },
            //                     new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
            //                     {
            //                          Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
            //                          {
            //                               MeasureName = "TotalDuration",
            //                                MappedMeasures = new List<string> {"TotalDurationInSecDistinctOnNetLocalCDRs"}
            //                          }
            //                     },
            //                     new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
            //                     {
            //                          Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
            //                          {
            //                               MeasureName = "TotalSaleDuration",
            //                                MappedMeasures = new List<string> {"TotalSaleDurationInSecDistinctOnNetLocalCDRs"}
            //                          }
            //                     }
            //                }
            //            }
            //    };
            //var serializedAnalyticTableMeasureExternalSource = Vanrise.Common.Serializer.Serialize(analyticTableMeasureExternalSource);
            //TestData testData = new TestData();
            //string originatorNumber = Console.ReadLine();
            //int batchSize = int.Parse(Console.ReadLine());
            //while (true)
            //{
            //    DateTime start = DateTime.Now;
            //    var cdrs = testData.ReadCDRs(true, batchSize, originatorNumber);
            //    Console.WriteLine("{0} CDRs read in {1} with top", cdrs.Count, DateTime.Now - start);
            //    cdrs = null;
            //    start = DateTime.Now;
            //    cdrs = testData.ReadCDRs(false, batchSize, originatorNumber);
            //    Console.WriteLine("{0} CDRs read in {1} without top", cdrs.Count, DateTime.Now - start);
            //    cdrs = null;
            //    originatorNumber = Console.ReadLine();
            //    batchSize = int.Parse(Console.ReadLine());
            //}
            DateTime date = DateTime.Now;
            new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
            Vanrise.Common.Business.OverriddenConfigurationManager overriddenConfigurationManager = new Vanrise.Common.Business.OverriddenConfigurationManager();
            var zajilScript = overriddenConfigurationManager.GenerateOverriddenConfigurationGroupScript(new Guid("CF1EAF73-93DB-416F-92FC-F8C0E7EE6EA7"));
            var multinetScript = overriddenConfigurationManager.GenerateOverriddenConfigurationGroupScript(new Guid("D79E9957-3EA5-49C1-AEDB-15F251BEDCDC"));
            var raScript = overriddenConfigurationManager.GenerateOverriddenConfigurationGroupScript(new Guid("0ef03fe5-6a6b-4271-a97b-d9a14298cfea"));
            var devScript = overriddenConfigurationManager.GenerateOverriddenConfigurationDevScript();
            var runtimeServices = new List<RuntimeService>();

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueRegulatorService);

            QueueActivationRuntimeService queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService); queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService); queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService); queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService); queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService); queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService);

            SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(summaryQueueActivationService); summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(summaryQueueActivationService); summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(summaryQueueActivationService); summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(summaryQueueActivationService); summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(summaryQueueActivationService);

            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(schedulerService);

            //Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(bigDataService);

            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dsRuntimeService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            Console.ReadKey();

            //Vanrise.Security.Entities.ContextFactory.GetContext().SetContextUserId(1);
            //var accountManager = new Retail.BusinessEntity.Business.AccountManager();
            //IAccountPayment dummy;
            //var financialAccounts = accountManager.GetCachedAccounts().Values.Where(a => accountManager.HasAccountPayment(a.AccountId, false, out dummy));
            //foreach(var a in financialAccounts)
            //{
            //    Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            //    try
            //    {
            //        invoiceManager.GenerateInvoice(new Vanrise.Invoice.Entities.GenerateInvoiceInput
            //                {
            //                    InvoiceTypeId = new Guid("384c819d-6e21-4e9a-9f08-11c7b81ad329"),
            //                    PartnerId =  a.AccountId.ToString(),
            //                    IssueDate = DateTime.Today,
            //                    FromDate = DateTime.Parse("2016-11-01"),
            //                    ToDate = DateTime.Parse("2016-12-01")
            //                }
            //                );
            //    }
            //catch(Vanrise.Invoice.Entities.InvoiceGeneratorException ex)
            //    {
            //        Console.WriteLine("Invoice has no data");
            //    }
            //}

            Console.WriteLine("DONE");
            Console.ReadKey();
        }

        private void CallGetAnalyticRecords()
        {
            var analyticManager = new Vanrise.Analytic.Business.AnalyticManager();
            var analyticQuery = new Vanrise.Analytic.Entities.AnalyticQuery
            {
                TableId = new Guid("d722f557-9cdc-4634-a86e-a941bf51c035"),
                FromTime = DateTime.Parse("2010-01-01"),
                DimensionFields = new List<string> { "OriginationZone", "ServiceType" },
                MeasureFields = new List<string> { "CountCDRs", "TotalDuration" },
                SubTables = new List<Vanrise.Analytic.Entities.AnalyticQuerySubTable>
                {
                    new  Vanrise.Analytic.Entities.AnalyticQuerySubTable
                    {
                         Dimensions = new List<string> { "DestinationZone", "Branch" },
                         Measures = new List<string> { "CountCDRs", "TotalDuration"},
                         OrderType = Vanrise.Analytic.Entities.AnalyticQueryOrderType.ByAllMeasures
                    }
                },
                WithSummary = true
            };
            Vanrise.Analytic.Entities.AnalyticRecord summaryRecord;
            List<Vanrise.Analytic.Entities.AnalyticResultSubTable> resultSubTables;
            var rslt = analyticManager.GetAllFilteredRecords(analyticQuery, out summaryRecord, out resultSubTables);
            string serializedResultSubTables = Serializer.Serialize(resultSubTables);
            string serializedRslt = Serializer.Serialize(rslt);

            List<AnalyticCustomRecord> customRecords = new List<AnalyticCustomRecord>();
            foreach (var record in rslt)
            {
                var customRecord = new AnalyticCustomRecord
                {
                    CountCDRs = (int)record.MeasureValues["CountCDRs"].Value,
                    TotalDuration = (decimal)record.MeasureValues["TotalDuration"].Value,
                    CalculatedCountCDRs = record.SubTables[0].MeasureValues.Sum(itm => (int)itm["CountCDRs"].Value),
                    CalculatedTotalDuration = record.SubTables[0].MeasureValues.Sum(itm => (decimal)itm["TotalDuration"].Value)
                };
                if (customRecord.CountCDRs != customRecord.CalculatedCountCDRs || customRecord.TotalDuration - customRecord.CalculatedTotalDuration > 0.000000000001M)
                    throw new Exception("Invalid SubTables Measures");
                customRecords.Add(customRecord);
            }
            string serializedCustomRecords = Serializer.Serialize(customRecords);

            if(summaryRecord != null)
            {
                string serializedSummary = Serializer.Serialize(summaryRecord);

                List<AnalyticCustomRecord> customVerticalRecords = new List<AnalyticCustomRecord>();
                int colIndex = 0;
                foreach (var subTableSummaryMeasures in summaryRecord.SubTables[0].MeasureValues)
                {
                    var customRecord = new AnalyticCustomRecord
                    {
                        CountCDRs = (int)subTableSummaryMeasures["CountCDRs"].Value,
                        TotalDuration = (decimal)subTableSummaryMeasures["TotalDuration"].Value,
                        CalculatedCountCDRs = rslt.Sum(record => (int)record.SubTables[0].MeasureValues[colIndex]["CountCDRs"].Value),
                        CalculatedTotalDuration = rslt.Sum(record => (decimal)record.SubTables[0].MeasureValues[colIndex]["TotalDuration"].Value),
                    };
                    if (customRecord.CountCDRs != customRecord.CalculatedCountCDRs || customRecord.TotalDuration - customRecord.CalculatedTotalDuration > 0.000000000001M)
                        throw new Exception("Invalid SubTables Measures");
                    customVerticalRecords.Add(customRecord);
                    colIndex++;
                }
                string serializedCustomVerticalRecords = Serializer.Serialize(customVerticalRecords);
            }
            
            
        }

        private class AnalyticCustomRecord
        {
            public int CountCDRs { get; set; }

            public int CalculatedCountCDRs { get; set; }

            public decimal TotalDuration { get; set; }

            public decimal CalculatedTotalDuration { get; set; }
        }

        

        private void GenerateVRWorkflow()
        {
            var workflow = new VRWorkflow
            {
                VRWorkflowId = Guid.NewGuid(),
                Settings = new VRWorkflowSettings
                {
                    Arguments = new VRWorkflowArgumentCollection
                    {
                        new VRWorkflowArgument { Name= "InputArg1", Direction = VRWorkflowArgumentDirection.In, Type = new VRWorkflowGenericVariableType { FieldType = new FieldTextType()}},
                        new VRWorkflowArgument { Name= "InputArg2", Direction = VRWorkflowArgumentDirection.In, Type = new VRWorkflowGenericVariableType { FieldType = new FieldNumberType{ DataType = FieldNumberDataType.Int}}},
                        new VRWorkflowArgument { Name= "OutputArg1", Direction = VRWorkflowArgumentDirection.Out, Type = new VRWorkflowGenericVariableType { FieldType = new FieldTextType()}},
                        new VRWorkflowArgument { Name= "InOutArg1", Direction = VRWorkflowArgumentDirection.InOut, Type = new VRWorkflowGenericVariableType { FieldType = new FieldTextType()}}
                    },
                    RootActivity = new VRWorkflowActivity
                    {
                        Settings = new VRWorkflowSequenceActivity
                        {
                            Variables = new VRWorkflowVariableCollection
                            {
                                new VRWorkflowVariable
                                {
                                     Name = "Variable1",
                                     Type = new VRWorkflowGenericVariableType { FieldType = new FieldTextType()}
                                },
                                new VRWorkflowVariable
                                {
                                     Name = "Variable2",
                                     Type = new VRWorkflowGenericVariableType { FieldType = new FieldTextType()}
                                },
                                new VRWorkflowVariable
                                {
                                     Name = "IntVariable1",
                                     Type = new VRWorkflowGenericVariableType { FieldType = new FieldNumberType{ DataType = FieldNumberDataType.Int }}
                                },
                                new VRWorkflowVariable
                                {
                                     Name = "ListVariable1",
                                     Type = new VRWorkflowGenericVariableType { FieldType = new FieldArrayType { FieldType = new FieldTextType() }}
                                }
                            },
                            Activities = new VRWorkflowActivityCollection
                            {
                                new VRWorkflowActivity
                                {
                                    Settings = new VRWorkflowAssignActivity 
                                    {
                                        Items = new List<VRWorkflowAssignActivityItem>
                                        {
                                            new VRWorkflowAssignActivityItem { To = "Variable1", Value = "\"New Activity: Variable 1 Value \" + DateTime.Now.ToString()"},
                                            new VRWorkflowAssignActivityItem { To = "ListVariable1", Value = "new List<string> { \"Item 1\", \"Item 2\", \"Item 3\" }"},
                                            new VRWorkflowAssignActivityItem { To = "IntVariable1", Value = "5"}
                                        }
                                    }
                                },
                                new VRWorkflowActivity
                                {
                                    Settings = new VRWorkflowCustomLogicActivity { Code = "Console.WriteLine(\"From Custom Activity\");Console.WriteLine(\"Value of Variable1: {0}\", Variable1);Variable2 = \"I changed the Value of the Variable2\";"}
                                },
                                new VRWorkflowActivity
                                {
                                    Settings = new VRWorkflowCustomLogicActivity { Code = "Console.WriteLine(\"Value of Variable2: {0}\", this.Variable2);"}
                                },
                                new VRWorkflowActivity
                                {
                                    Settings = new VRWorkflowCustomLogicActivity { Code = "Console.WriteLine(\"Value of IntVariable1: {0}\", IntVariable1);"}
                                },
                                new VRWorkflowActivity
                                {
                                    Settings = new VRWorkflowCustomLogicActivity { Code = @"foreach(var item in ListVariable1)
{
    Console.WriteLine(""List Item: {0}"", item);
}"}
                                },
                                new VRWorkflowActivity
                                {
                                    Settings = new VRWorkflowAssignActivity 
                                    {
                                        Items = new List<VRWorkflowAssignActivityItem>
                                        {
                                            new VRWorkflowAssignActivityItem { To = "ListVariable1[1]", Value = "\"new Value for second Item \" + DateTime.Now.ToString() "}
                                        }
                                    }
                                },
                                new VRWorkflowActivity
                                {
                                    Settings = new VRWorkflowCustomLogicActivity { Code = @"foreach(var item in ListVariable1)
{
    Console.WriteLine(""List Item: {0}"", item);
}"}
                                },
                                new VRWorkflowActivity
                                {
                                    Settings = new VRWorkflowCustomLogicActivity { Code = "Console.WriteLine(\"Value of InputArg1: {0}\", InputArg1);"}
                                },
                                new VRWorkflowActivity
                                {
                                    Settings = new VRWorkflowCustomLogicActivity { Code = "OutputArg1 = \"VAlue returned in Output Arguments. using Arguments\";"}
                                },
                                new VRWorkflowActivity
                                {
                                    Settings = new VRWorkflowIfElseActivity{
                                         Condition = "1 == 1",
                                         TrueActivity = new VRWorkflowActivity
                                                        {
                                                            Settings = new VRWorkflowCustomLogicActivity { Code = "Console.WriteLine(\"Condition is true\");"}
                                                        }
                                    }
                                },
                                new VRWorkflowActivity
                                {
                                    Settings = new VRWorkflowIfElseActivity{
                                         Condition = "InputArg2 >= 10",
                                         TrueActivity = new VRWorkflowActivity
                                                        {
                                                            Settings = new VRWorkflowCustomLogicActivity { Code = "Console.WriteLine(\"Input Argument '{0}' is greater than 10\", InputArg2);"}
                                                        },
                                         FalseActivity = new VRWorkflowActivity
                                                        {
                                                            Settings = new VRWorkflowCustomLogicActivity { Code = "Console.WriteLine(\"Input Argument '{0}' is less than 10\", InputArg2);"}
                                                        }
                                    }
                                },
                                new VRWorkflowActivity
                                {
                                    Settings = new VRWorkflowForEachActivity
                                    {
                                         List = "ListVariable1",
                                          IterationVariableName = "Item",
                                           IterationVariableType = new VRWorkflowGenericVariableType { FieldType = new FieldTextType()},
                                            Activity = new VRWorkflowActivity
                                            {
                                                Settings = new VRWorkflowCustomLogicActivity { Code = "Console.WriteLine(\"Foreach Iteration: '{0}' \", Item);"}
                                            }
                                    }
                                },
                                new VRWorkflowActivity
                                {
                                    Settings = new VRWorkflowSequenceActivity
                                    {
                                         Variables = new VRWorkflowVariableCollection
                                         {
                                             new VRWorkflowVariable
                                             {
                                                 Name = "SubVariable1",
                                                 Type = new VRWorkflowGenericVariableType { FieldType = new FieldTextType()}
                                             }
                                         },
                                         Activities = new VRWorkflowActivityCollection
                                         {
                                             new VRWorkflowActivity
                                            {
                                                Settings = new VRWorkflowAssignActivity 
                                                {
                                                    Items = new List<VRWorkflowAssignActivityItem>
                                                    {
                                                        new VRWorkflowAssignActivityItem { To = "SubVariable1", Value = "\"Sub Variable 1 Value \" + DateTime.Now.ToString()"},
                                                        new VRWorkflowAssignActivityItem { To = "IntVariable1", Value = "55"}
                                                    }
                                                }
                                            },
                                            new VRWorkflowActivity
                                            {
                                                Settings = new VRWorkflowCustomLogicActivity { Code = "Console.WriteLine(\"SubVariable1 in SUB: '{0}' \", SubVariable1);"}
                                            },
                                            new VRWorkflowActivity
                                            {
                                                Settings = new VRWorkflowCustomLogicActivity { Code = "Console.WriteLine(\"IntVariable1 in SUB: '{0}' \", IntVariable1);"}
                                            },
                                            new VRWorkflowActivity
                                            {
                                                 Settings = new VRWorkflowSequenceActivity
                                                {
                                                     Activities = new VRWorkflowActivityCollection
                                                     {
                                                        new VRWorkflowActivity
                                                        {
                                                            Settings = new VRWorkflowCustomLogicActivity { Code = "Console.WriteLine(\"IntVariable1 in SUB SUB: '{0}' \", IntVariable1);"}
                                                        }
                                                     }
                                                }
                                            }
                                         }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            Activity wfWorkflow;
            List<string> errorMessage;
            if (new VRWorkflowManager().TryCompileWorkflow(workflow, out wfWorkflow, out errorMessage))
            {
                var output = WorkflowInvoker.Invoke(wfWorkflow, new Dictionary<string, object> { {"InputArg1", "this is teh value send in INputArg1"}, {"InputArg2", 43} });
            }
        }

        private void CreateWFProgrammatically()
        {
            var wf = new WF();
            Compile(wf);
            WorkflowInvoker.Invoke(wf);
            WorkflowInvoker.Invoke(wf);
            WorkflowInvoker.Invoke(wf);
            WorkflowInvoker.Invoke(wf);
            Console.ReadKey();
        }

        

        static void Compile(Activity dynamicActivity)
        {

            TextExpressionCompilerSettings settings = new TextExpressionCompilerSettings

            {

                Activity = dynamicActivity,

                Language = "C#",

                ActivityName = dynamicActivity.GetType().FullName.Split('.').Last() + "_CompiledExpressionRoot",

                ActivityNamespace = string.Join(".", dynamicActivity.GetType().FullName.Split('.').Reverse().Skip(1).Reverse()),

                RootNamespace = null,

                GenerateAsPartialClass = false,

                AlwaysGenerateSource = true,

            };



            TextExpressionCompilerResults results =

                new TextExpressionCompiler(settings).Compile();

            if (results.HasErrors)
            {

                throw new Exception("Compilation failed.");

            }



            ICompiledExpressionRoot compiledExpressionRoot =

                Activator.CreateInstance(results.ResultType,

                    new object[] { dynamicActivity }) as ICompiledExpressionRoot;

            CompiledExpressionInvoker.SetCompiledExpressionRootForImplementation(

                dynamicActivity, compiledExpressionRoot);

        }

        private void TestStringConcatenation()
        {
            List<string> lst1 = new List<string>();

            for (int i = 0; i < 23;i++)
            {
                lst1.Add("item " + i.ToString());
            }

            int nbOfRows = 2000000;

            DateTime start = DateTime.Now;
            StringBuilder str = new StringBuilder();
            //for (int i = 0; i < nbOfRows; i++)
            //{
            //    str.AppendFormat(@"{0}{23}{1}{23}{2}{23}{3}{23}{4}{23}{5}{23}{6}{23}{7}{23}{8}{23}{9}{23}{10}{23}{11}{23}{12}{23}{13}{23}{14}{23}{15}{23}{16}{23}{17}{23}{18}{23}{19}{23}{20}{23}{21}{23}{22}",
            //                            lst1[0], lst1[1], lst1[2],
            //                            lst1[3], lst1[4], lst1[5],
            //                            lst1[6], lst1[7], lst1[8],
            //                            lst1[9],  lst1[10], lst1[11], lst1[12],
            //                            lst1[13], lst1[14], lst1[15],
            //                            lst1[16], lst1[17], lst1[18],
            //                            lst1[19], lst1[20], lst1[21],lst1[22],"\t");
                
            //}
            //Console.WriteLine("AppendFormat took '{0}'", DateTime.Now - start);

            //str = new StringBuilder();
            //GC.Collect();
            start = DateTime.Now;
            for (int i = 0; i < nbOfRows; i++)
            {
                foreach(var fld in lst1)
                {
                    str.Append(fld);
                    str.Append("\t");
                }
                str.AppendLine();
            }
            Console.WriteLine("String Builder took '{0}'", DateTime.Now - start);

            //str = new StringBuilder();
            //GC.Collect();

            //start = DateTime.Now;
            //for (int i = 0; i < nbOfRows; i++)
            //{
            //    List<string> lst2 = new List<string>(lst1);
            //    str.AppendLine(String.Join(",", lst2));
            //}
            //Console.WriteLine("String Builder Append with Join took '{0}'", DateTime.Now - start);
            //str = new StringBuilder();
            //GC.Collect();

            //start = DateTime.Now;
            //List<string> rows = new List<string>();
            //for (int i = 0; i < nbOfRows; i++)
            //{
            //    List<string> lst2 = new List<string>(lst1);
            //    rows.Add(String.Join(",", lst2));
            //}
            //string final = String.Join("\n", rows);
            //Console.WriteLine("Join Only took '{0}'", DateTime.Now - start);

           // Console.ReadKey();
        }

        private void GenerateRuntimeNodeConfiguration()
        {
            var runtimeNodeConfigSettings = new Vanrise.Runtime.Entities.RuntimeNodeConfigurationSettings
            {
                Processes = new Dictionary<Guid, RuntimeProcessConfiguration>()
            };
            var processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 1,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("8382D9CA-05FC-485C-807C-3F6F5F617FD5"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Scheduler Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.Runtime.SchedulerService{ Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("2B7DFB5B-529F-4298-A3B1-0AD5F7F7122B"), new RuntimeProcessConfiguration
            {
                Name = "Scheduler Service Process",
                Settings = processSettings
            });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 1,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("8554CD70-01C8-4B7D-BBB8-37C976B8DA25"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Business Process Regulator Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.BusinessProcess.BPRegulatorRuntimeService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("A40DB74D-F99C-4A2D-A571-CB626506A3D8"), new RuntimeProcessConfiguration
            {
                Name = "Business Process Regulator Service Process",
                Settings = processSettings
            });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 3,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("BFC6D07D-1355-4C34-AC8E-D86BAD74B413"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Business Process Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.BusinessProcess.BusinessProcessService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("93546ACE-D83A-4B17-966F-281A0B5EF1A3"), new RuntimeProcessConfiguration
            {
                Name = "Business Process Services Process",
                Settings = processSettings
            });

            var serializedNodeConfigSettings1 = Vanrise.Common.Serializer.Serialize(runtimeNodeConfigSettings);

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 1,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("F4DDAB13-00C9-4A3C-962D-E7B298F94471"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Queue Regulator Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.Queueing.QueueRegulatorRuntimeService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("4FE96F64-1C25-4C65-9A18-2F9E0384547D"), new RuntimeProcessConfiguration
            {
                Name = "Queue Regulator Service Process",
                Settings = processSettings
            });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 3,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("EBF8DD5D-5082-46B9-8CA7-77F1AF6D8268"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Queue Activation Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.Queueing.QueueActivationRuntimeService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("1C395B1D-9EC6-4B87-B8E7-35370C09BC26"), new RuntimeProcessConfiguration
            {
                Name = "Queue Activation Services Process",
                Settings = processSettings
            });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 3,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("555CE82F-F932-4494-89A3-1838E5CBE8A0"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Summary Queue Activation Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.Queueing.SummaryQueueActivationRuntimeService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("45EFD1BA-D606-41A7-A43D-2E9B1F8DE040"), new RuntimeProcessConfiguration
            {
                Name = "Summary Queue Activation Services Process",
                Settings = processSettings
            });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 3,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("650F9943-4FE6-4087-BB67-EAFB0D19AC02"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Data Source Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("49FA4851-39C4-45E9-8F01-12B66A15D96A"), new RuntimeProcessConfiguration
            {
                Name = "Data Source Services Process",
                Settings = processSettings
            });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 2,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("4F90E325-5DDD-4544-98F9-FA5D55A0C594"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Big Data Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("0057B8AD-9A79-41BA-826A-4A27AA76E6C0"), new RuntimeProcessConfiguration
            {
                Name = "Big Data Services Process",
                Settings = processSettings
            });

            var serializedNodeConfigSettings = Vanrise.Common.Serializer.Serialize(runtimeNodeConfigSettings);
        }

        RDBTableDefinition _userTable = new RDBTableDefinition
        {
            DBSchemaName = "sec",
            DBTableName = "User",
            IdColumnName = "ID",
            Columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                  {"ID", new RDBTableColumnDefinition { DBColumnName = "DBID", DataType = RDBDataType.Int}},
                  {"Name", new RDBTableColumnDefinition { DBColumnName = "DBName", DataType = RDBDataType.NVarchar, Size = 255}},
                   {"GroupID", new RDBTableColumnDefinition { DBColumnName = "DBGroupID", DataType = RDBDataType.Int}},
                   {"Age", new RDBTableColumnDefinition { DBColumnName = "DBAge", DataType = RDBDataType.Decimal, Size=20, Precision = 8}},
                   {"CityID", new RDBTableColumnDefinition { DBColumnName = "DBCityID", DataType = RDBDataType.Int}},
                   {"Email", new RDBTableColumnDefinition { DBColumnName = "DBEmail", DataType = RDBDataType.NVarchar, Size = 255}}
            }
        };

        RDBTableDefinition _groupTable = new RDBTableDefinition
        {
            DBSchemaName = "sec",
            DBTableName = "Group",
            IdColumnName = "ID",
            Columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                  {"ID", new RDBTableColumnDefinition { DBColumnName = "DBID", DataType = RDBDataType.Int}},
                  {"Name", new RDBTableColumnDefinition { DBColumnName = "DBName", DataType = RDBDataType.NVarchar, Size = 255}}
            }
        };

        void TestRDBSelectQuery()
        {
            RDBSchemaManager.Current.RegisterDefaultTableDefinition("User", _userTable);
            var dataProvider = new Vanrise.Data.RDB.DataProvider.Providers.MSSQLRDBDataProvider("Data Source=.;Initial Catalog=test;User ID=sa; Password=p@ssw0rd");

            var selectFromSelect = new RDBQueryContext(dataProvider).Select()
                .FromSelect("mainTable").From("User", "u").SelectColumns().AllTableColumns("u").EndColumns().EndSelect()
                .Join().JoinSelect(RDBJoinType.Inner, "t2").From("User", "u").SelectColumns().AllTableColumns("u").EndColumns().EndSelect().EqualsCondition("mainTable", "ID", "t2", "ID").EndJoin()
                .SelectColumns().Column("Email").EndColumns().EndSelect().GetResolvedQuery().QueryText;

            var insertQuery1 = new RDBQueryContext(dataProvider).Insert().IntoTable("User").IfNotExists("u").EqualsCondition("Email", "user@nodomain.com")
                .GenerateIdAndAssignToParameter("UserId").ColumnValue("Name", "user 1").ColumnValue("Email", "user@nodomain.com").EndInsert().GetResolvedQuery().QueryText;

            Vanrise.AccountBalance.Entities.BillingTransactionQuery query = new Vanrise.AccountBalance.Entities.BillingTransactionQuery
            {
                AccountTypeId = new Guid("BB50E990-AFDD-4561-B0A0-973FA79D58B4"),
                AccountsIds = new List<string> { "Ar1", "acc3", "acc555" },
                TransactionTypeIds = new List<Guid> { new Guid("69D9C78A-BE92-4E14-AF9D-0D12228AC9FD"), new Guid("E26B10F9-EB37-4448-A900-13A2BA454C0F"), new Guid("AF4E26F2-FBEA-42F9-BB2D-2EF14B97BD83") },
                EffectiveDate = DateTime.Now,
                FromTime = DateTime.Parse("2017-04-03"),
                IsEffectiveInFuture = false,
                Status = Vanrise.Entities.VRAccountStatus.Active,
                ToTime = DateTime.Now
            };

            Guid accountId = new Guid("E02A4F97-7E0E-46FB-995D-29C126D0B039");
            string TABLE_NAME = "VR_AccountBalance_BillingTransaction";
            new BillingTransactionDataManager();
            var getFilteredQuery = new RDBQueryContext(dataProvider)
                   .Select()
                   .From(TABLE_NAME, "bt")
                   .Join()
                   .JoinLiveBalance("liveBalance", "bt")
                   .EndJoin()
                   .Where().And()
                                .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                                .ConditionIf(() => query.AccountsIds != null && query.AccountsIds.Count() > 0, ctx => ctx.ListCondition("AccountID", RDBListConditionOperator.IN, query.AccountsIds))
                                .ConditionIf(() => query.TransactionTypeIds != null && query.TransactionTypeIds.Count() > 0, ctx => ctx.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, query.TransactionTypeIds))
                                .CompareCondition("TransactionTime", RDBCompareConditionOperator.GEq, query.FromTime)
                                .ConditionIf(() => query.ToTime.HasValue, ctx => ctx.CompareCondition("TransactionTime", RDBCompareConditionOperator.LEq, query.ToTime.Value))
                                .EqualsCondition("AccountTypeID", query.AccountTypeId)
                                .LiveBalanceActiveAndEffectiveCondition("liveBalance", query.Status, query.EffectiveDate, query.IsEffectiveInFuture)
                           .EndAnd()
                   .SelectColumns().AllTableColumns("bt").EndColumns()
                   .EndSelect()
                   .GetResolvedQuery().QueryText;


            BillingTransaction billingTransaction = new BillingTransaction
                {
                     AccountId = "acc2",
                      TransactionTime = DateTime.Now,
                       Notes  ="this is the notes"
                };
                long? invoiceId = null;
            var insertQuery = new RDBQueryContext(dataProvider)
                .Insert()
                .IntoTable(TABLE_NAME)
                .GenerateIdAndAssignToParameter("BillingTransactionId")
                .ColumnValue("AccountID", billingTransaction.AccountId)
                .ColumnValue("AccountTypeID", billingTransaction.AccountTypeId)
                .ColumnValue("Amount", billingTransaction.Amount)
                .ColumnValue("CurrencyId", billingTransaction.CurrencyId)
                .ColumnValue("TransactionTypeID", billingTransaction.TransactionTypeId)
                .ColumnValue("TransactionTime", billingTransaction.TransactionTime)
                .ColumnValue("Notes", billingTransaction.Notes)
                .ColumnValue("Reference", billingTransaction.Reference)
                .ColumnValue("SourceID", billingTransaction.SourceId)
                .ColumnValue("Settings", (billingTransaction.Settings != null) ? Vanrise.Common.Serializer.Serialize(billingTransaction.Settings) : null)
                .ColumnValueIf(() => invoiceId.HasValue, ctx => ctx.ColumnValue("CreatedByInvoiceID", invoiceId.Value))
                .EndInsert().GetResolvedQuery().QueryText;

            Guid accountTypeId = new Guid("BB50E990-AFDD-4561-B0A0-973FA79D58B4");
            var GetBillingTransactionsByBalanceUpdated = new RDBQueryContext(dataProvider)
                   .Select()
                   .From(TABLE_NAME, "bt")
                   .Where().And()
                                .EqualsCondition("AccountTypeID", accountTypeId)
                                .Or()
                                    .And()
                                        .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                                        .ConditionIfColumnNotNull("IsBalanceUpdated").EqualsCondition("IsBalanceUpdated", false)
                                    .EndAnd()
                                    .And()
                                        .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", true)
                                        .ConditionIfColumnNotNull("IsBalanceUpdated").EqualsCondition("IsBalanceUpdated", true)
                                        .ConditionIfColumnNotNull("IsSubtractedFromBalance").EqualsCondition("IsSubtractedFromBalance", false)
                                    .EndAnd()
                                .EndOr()
                           .EndAnd()
                    .SelectColumns().AllTableColumns("bt").EndColumns()
                    .EndSelect().GetResolvedQuery().QueryText;

            List<Guid> billingTransactionTypeIds = new List<Guid> { new Guid("69D9C78A-BE92-4E14-AF9D-0D12228AC9FD"), new Guid("E26B10F9-EB37-4448-A900-13A2BA454C0F"), new Guid("AF4E26F2-FBEA-42F9-BB2D-2EF14B97BD83") };

           var GetBillingTransactionsForSynchronizerProcess = new RDBQueryContext(dataProvider)
                   .Select()
                   .From(TABLE_NAME, "bt")
                   .Where().And()
                                .EqualsCondition("AccountTypeID", accountTypeId)
                                .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                                .NotNullCondition("SourceID")
                                .ConditionIf(() => billingTransactionTypeIds != null && billingTransactionTypeIds.Count() > 0, ctx => ctx.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, billingTransactionTypeIds))
                           .EndAnd()
                    .SelectColumns().AllTableColumns("bt").EndColumns()
                    .EndSelect()
                    .GetResolvedQuery().QueryText;


            var btAccountTimesTempTableColumns = new Dictionary<string, RDBTableColumnDefinition>();
            btAccountTimesTempTableColumns.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            btAccountTimesTempTableColumns.Add("TransactionTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            var btAccountTimesTempTableQuery = new RDBTempTableQuery(btAccountTimesTempTableColumns);

            List<BillingTransactionByTime> billingTransactionsByTime = new List<BillingTransactionByTime>();
            billingTransactionsByTime.Add(new BillingTransactionByTime { AccountId = "acc1", TransactionTime = DateTime.Parse("2015-03-02") });
            billingTransactionsByTime.Add(new BillingTransactionByTime { AccountId = "acc2", TransactionTime = DateTime.Parse("2015-03-04") });
            billingTransactionsByTime.Add(new BillingTransactionByTime { AccountId = "acc3", TransactionTime = DateTime.Parse("2015-03-06") });
            billingTransactionsByTime.Add(new BillingTransactionByTime { AccountId = "acc4", TransactionTime = DateTime.Parse("2015-03-07") });

            var GetBillingTransactionsByTransactionTypes = new RDBQueryContext(dataProvider)
                .StartBatchQuery()
                .AddQuery().CreateTempTable(btAccountTimesTempTableQuery)
                .Foreach(billingTransactionsByTime, (item, batchQuery) =>
                    {
                        batchQuery.AddQuery().Insert().IntoTable(btAccountTimesTempTableQuery).ColumnValue("AccountID", item.AccountId).ColumnValue("TransactionTime", item.TransactionTime);
                    })
                .AddQuery()
                    .Select()
                    .From(TABLE_NAME, "bt")
                    .Join()
                    .Join(RDBJoinType.Inner, btAccountTimesTempTableQuery, "btAccountTimes")
                        .And()
                            .EqualsCondition("bt", "AccountID", new RDBColumnExpression { TableAlias = "btAccountTimes", ColumnName = "AccountID" })
                            .CompareCondition("bt", "TransactionTime", RDBCompareConditionOperator.G, new RDBColumnExpression { TableAlias = "btAccountTimes", ColumnName = "TransactionTime" })
                        .EndAnd()
                    .EndJoin()
                    .Where().And()
                                .EqualsCondition("AccountTypeID", accountTypeId)
                                .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                                .ConditionIf(() => billingTransactionTypeIds != null && billingTransactionTypeIds.Count() > 0, ctx => ctx.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, billingTransactionTypeIds))
                            .EndAnd()
                    .SelectColumns().Columns("AccountID", "Amount", "CurrencyId", "TransactionTime", "TransactionTypeID").EndColumns()
                    .EndSelect()
                .EndBatchQuery()
                .GetResolvedQuery().QueryText;

            TABLE_NAME = "VR_AccountBalance_LiveBalance";

            

            decimal initialBalance = 33;
                 int currencyId = 3;
            decimal currentBalance = 54;
            DateTime? bed = null; DateTime? eed = null;
            VRAccountStatus status =  VRAccountStatus.Active;
            bool isDeleted = false;

            var TryAddLiveBalanceAndGet = new RDBQueryContext(dataProvider)
                    .StartBatchQuery()
                    .AddQuery().DeclareParameters().AddParameter("ID", RDBDataType.BigInt).AddParameter("CurrencyIdToReturn", RDBDataType.Int).EndParameterDeclaration()
                    .AddQuery().Select()
                                .From(TABLE_NAME, "lv")
                                .Where().And()
                                            .EqualsCondition("AccountTypeID", accountTypeId)
                                            .EqualsCondition("AccountID", accountId)
                                            .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                                        .EndAnd()
                                .SelectColumns().ColumnToParameter("ID", "ID").ColumnToParameter("CurrencyID", "CurrencyIdToReturn").EndColumns()
                                .EndSelect()
                    .AddQuery().If().IfCondition().NullCondition(new RDBParameterExpression { ParameterName = "ID" })
                                .ThenQuery().StartBatchQuery()
                                            .AddQuery().Insert()
                                                        .IntoTable(TABLE_NAME)
                                                        .GenerateIdAndAssignToParameter("ID", true, false)
                                                        .ColumnValue("AccountTypeID", accountTypeId)
                                                        .ColumnValue("AccountID", accountId)
                                                        .ColumnValue("InitialBalance", initialBalance)
                                                        .ColumnValue("CurrentBalance", currentBalance)
                                                        .ColumnValue("CurrencyID", currencyId)
                                                        .ColumnValueDBNullIfDefaultValue("BED", bed)
                                                        .ColumnValueDBNullIfDefaultValue("EED", eed)
                                                        .ColumnValue("Status", (int)status)
                                                        .ColumnValue("IsDeleted", isDeleted)
                                                       .EndInsert()
                                            .AddQuery().SetParameterValues().ParamValue("CurrencyIdToReturn", currencyId).EndParameterValues()
                                            .EndBatchQuery()
                                .EndIf()
                     .AddQuery().Select().FromNoTable().SelectColumns().Parameter("ID", "ID").FixedValue(accountId, "AccountID").EndColumns().EndSelect()
                    .EndBatchQuery().GetResolvedQuery().QueryText;

            var tempTableColumns = new Dictionary<string, RDBTableColumnDefinition> 
            {
                {"ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt}},
                {"UpdateValue", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6}}
            };

            IEnumerable<LiveBalanceToUpdate> liveBalancesToUpdate = new List<LiveBalanceToUpdate>
            {
                new LiveBalanceToUpdate { LiveBalanceId = 2, Value = 43.4M},
                new LiveBalanceToUpdate { LiveBalanceId = 4, Value = 34.2M}
            };
            var tempTableQuery = new RDBTempTableQuery(tempTableColumns);
           var updateLiveBalances = new RDBQueryContext(dataProvider)
                    .StartBatchQuery()
                .AddQuery().CreateTempTable(tempTableQuery)
                .Foreach(liveBalancesToUpdate, (lvToUpdate, context) => context.AddQuery().Insert().IntoTable(tempTableQuery).ColumnValue("ID", lvToUpdate.LiveBalanceId).ColumnValue("UpdateValue", lvToUpdate.Value).EndInsert())
                .AddQuery().Update().FromTable(LiveBalanceDataManager.TABLE_NAME)
                                    .Join("lv").JoinOnEqualOtherTableColumn(tempTableQuery, "lvToUpdate", "ID", "lv", "ID").EndJoin()
                                    .ColumnValue("CurrentBalance", new RDBArithmeticExpression
                                    {
                                        Operator = RDBArithmeticExpressionOperator.Add,
                                        Expression1 = new RDBColumnExpression { TableAlias = "lv", ColumnName = "CurrentBalance", DontAppendTableAlias = true },
                                        Expression2 = new RDBColumnExpression { TableAlias = "lvToUpdate", ColumnName = "UpdateValue" }
                                    })
                                    .EndUpdate().EndBatchQuery().GetResolvedQuery().QueryText;

            long balanceUsageQueueId = 5;
            IEnumerable<AccountUsageToUpdate> accountsUsageToUpdate = new List<AccountUsageToUpdate>
            {
                 new AccountUsageToUpdate { AccountUsageId = 3, Value = 45.777M},
                 new AccountUsageToUpdate { AccountUsageId = 4, Value = 32.734M},
            };

            Guid? correctionProcessId = Guid.NewGuid();

            var updateTest = new RDBQueryContext(dataProvider).Update().FromTable("User").Where().EqualsCondition("ID", 3).ColumnValue("Name", "User 1").EndUpdate()
                .GetResolvedQuery().QueryText;

          var  UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue = new RDBQueryContext(dataProvider)
                .StartBatchQuery()
                    .UpdateLiveBalances(liveBalancesToUpdate)
                    .UpdateAccountUsages(accountsUsageToUpdate, correctionProcessId)
                    .AddQuery().Delete().FromTable(BalanceUsageQueueDataManager.TABLE_NAME).Where().EqualsCondition("ID", balanceUsageQueueId).EndDelete()
                .EndBatchQuery().GetResolvedQuery().QueryText;

            IEnumerable<long> billingTransactionIds = new List<long> { 3,5};
                IEnumerable<long> accountUsageIdsToOverride = new List<long> { 35, 66};
                IEnumerable<AccountUsageOverride> accountUsageOverrides = new List<AccountUsageOverride> {
                new AccountUsageOverride { AccountTypeId = Guid.NewGuid(), AccountId = "44", AccountUsageOverrideId = 2},
                new AccountUsageOverride { AccountTypeId = Guid.NewGuid(), AccountId = "66", AccountUsageOverrideId = 2},
                new AccountUsageOverride { AccountTypeId = Guid.NewGuid(), AccountId = "22", AccountUsageOverrideId = 2}
            };
            IEnumerable<long> overridenUsageIdsToRollback = new List<long> { 2,57};
            IEnumerable<long> deletedTransactionIds = new List<long> { 77, 3 };
            var UpdateLiveBalancesFromBillingTransactions =
                   
            new RDBQueryContext(dataProvider)
                .StartBatchQuery()
                .AddQueryIf(() => overridenUsageIdsToRollback != null && overridenUsageIdsToRollback.Count() > 0, 
                    ctx => ctx.Update()
                                .FromTable(AccountUsageDataManager.TABLE_NAME)
                                .Where().ListCondition("ID", RDBListConditionOperator.IN, overridenUsageIdsToRollback)
                                .ColumnValue("IsOverridden", new RDBNullExpression()).ColumnValue("OverriddenAmount", new RDBNullExpression())
                              .EndUpdate())
                .AddQueryIf(() => accountUsageIdsToOverride != null && accountUsageIdsToOverride.Count() > 0,
                    ctx => ctx.Update()
                                .FromTable(AccountUsageDataManager.TABLE_NAME)
                                .Where().ListCondition("ID", RDBListConditionOperator.IN, accountUsageIdsToOverride)
                                .ColumnValue("IsOverridden", true).ColumnValue("OverriddenAmount", new RDBColumnExpression { ColumnName = "UsageBalance"})
                              .EndUpdate())
                .AddQueryIf(()=> deletedTransactionIds != null && deletedTransactionIds.Count() > 0,
                    ctx => ctx.StartBatchQuery()
                                .AddQuery().Delete().FromTable(AccountUsageOverrideDataManager.TABLE_NAME).Where().ListCondition("OverriddenByTransactionID", RDBListConditionOperator.IN, deletedTransactionIds).EndDelete()
                                .AddQuery().Update().FromTable(BillingTransactionDataManager.TABLE_NAME).Where().ListCondition("ID", RDBListConditionOperator.IN, deletedTransactionIds).ColumnValue("IsSubtractedFromBalance", true).EndUpdate()
                              .EndBatchQuery())
                .AddQueryIf(() => accountUsageOverrides != null && accountUsageOverrides.Count() > 0,
                    ctx => ctx.StartBatchQuery()
                                .Foreach(accountUsageOverrides, 
                                    (accountUsageOverride, foreachCtx) => foreachCtx.AddQuery().Insert()
                                                                                                .IntoTable(AccountUsageOverrideDataManager.TABLE_NAME)
                                                                                                .ColumnValue("AccountTypeID", accountUsageOverride.AccountTypeId)
                                                                                                .ColumnValue("AccountID", accountUsageOverride.AccountId)
                                                                                                .ColumnValue("TransactionTypeID", accountUsageOverride.TransactionTypeId)
                                                                                                .ColumnValue("PeriodStart", accountUsageOverride.PeriodStart)
                                                                                                .ColumnValue("PeriodEnd", accountUsageOverride.PeriodEnd)
                                                                                                .ColumnValue("OverriddenByTransactionID", accountUsageOverride.OverriddenByTransactionId)
                                                                                               .EndInsert())
                              .EndBatchQuery())
                .AddQueryIf(() => billingTransactionIds != null && billingTransactionIds.Count() > 0,
                    ctx => ctx.Update().FromTable(BillingTransactionDataManager.TABLE_NAME).Where().ListCondition("ID", RDBListConditionOperator.IN, billingTransactionIds).ColumnValue("IsBalanceUpdated", true).EndUpdate())
                .UpdateLiveBalances(liveBalancesToUpdate)
                .EndBatchQuery().GetResolvedQuery().QueryText;

            List<LiveBalanceNextThresholdUpdateEntity> updateEntities = new List<LiveBalanceNextThresholdUpdateEntity>{
                new LiveBalanceNextThresholdUpdateEntity { AccountTypeId = Guid.NewGuid(), AccountId = "43534", AlertRuleId = 5, NextAlertThreshold = 64},
                new LiveBalanceNextThresholdUpdateEntity { AccountTypeId = Guid.NewGuid(), AccountId = "74"},
                new LiveBalanceNextThresholdUpdateEntity { AccountTypeId = Guid.NewGuid(), AccountId = "685", AlertRuleId =6, NextAlertThreshold = 435}
            };
           
            var tempTableColumns1 = new Dictionary<string, RDBTableColumnDefinition>();
            tempTableColumns1.Add("AccountTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            tempTableColumns1.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            tempTableColumns1.Add("NextAlertThreshold", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            tempTableColumns1.Add("AlertRuleID", new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            var tempTableQuery1 = new RDBTempTableQuery(tempTableColumns1);
            var UpdateBalanceRuleInfos = new RDBQueryContext(dataProvider)
                .StartBatchQuery()
                    .AddQuery().CreateTempTable(tempTableQuery1)
                    .Foreach(updateEntities,
                        (updateEntity, ctx) => ctx.AddQuery().Insert().IntoTable(tempTableQuery1)
                                                                .ColumnValue("AccountTypeID", updateEntity.AccountTypeId)
                                                                .ColumnValue("AccountID", updateEntity.AccountId)
                                                                .ColumnValueDBNullIfDefaultValue("NextAlertThreshold", updateEntity.NextAlertThreshold)
                                                                .ColumnValueDBNullIfDefaultValue("AlertRuleID", updateEntity.AlertRuleId)
                                                             .EndInsert())
                    .AddQuery()
                        .Update().FromTable(TABLE_NAME)
                            .Join("lb")
                            .Join(RDBJoinType.Inner, tempTableQuery1, "updateEntities").And().EqualsCondition("lb", "AccountTypeID", "updateEntities", "AccountTypeID").EqualsCondition("lb", "AccountID", "updateEntities", "AccountID").EndAnd()
                            .EndJoin()
                            .ColumnValue("NextAlertThreshold", new RDBColumnExpression { TableAlias = "updateEntities", ColumnName = "NextAlertThreshold" })
                            .ColumnValue("AlertRuleID", new RDBColumnExpression { TableAlias = "updateEntities", ColumnName = "AlertRuleID" })
                        .EndUpdate()
                .EndBatchQuery().GetResolvedQuery().QueryText;

          var  GetLiveBalancesToAlert =  new RDBQueryContext(dataProvider)
                    .Select()
                        .From(TABLE_NAME, "lb")
                        .Where().And()
                                    .EqualsCondition("AccountTypeID", accountTypeId)
                                    .LiveBalanceToAlertCondition("lb")
                                    .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                                .EndAnd()
                        .SelectColumns().AllTableColumns("lb").EndColumns()
                    .EndSelect().GetResolvedQuery().QueryText;
        

        var GetLiveBalancesToClearAlert = new RDBQueryContext(dataProvider)
                   .Select()
                       .From(TABLE_NAME, "lb")
                       .Where().And()
                                   .EqualsCondition("AccountTypeID", accountTypeId)
                                   .LiveBalanceToClearAlertCondition("lb")
                                   .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                               .EndAnd()
                       .SelectColumns().AllTableColumns("lb").EndColumns()
                   .EndSelect().GetResolvedQuery().QueryText;

        var HasLiveBalancesUpdateData = new RDBQueryContext(dataProvider)
                    .If().IfCondition()
                            .ExistsCondition()
                            .From(TABLE_NAME, "lb", 1)
                            .Where()
                                .And()
                                    .EqualsCondition("AccountTypeID", accountTypeId)
                                    .Or()
                                        .And().LiveBalanceToAlertCondition("lb").EndAnd()
                                        .And().LiveBalanceToClearAlertCondition("lb").EndAnd()
                                    .EndOr()
                                .EndAnd()
                            .SelectColumns().Column("ID").EndColumns()
                            .EndSelect()
                    .ThenQuery().Select().FromNoTable().SelectColumns().FixedValue(true, "rslt").EndColumns().EndSelect()
                    .ElseQuery().Select().FromNoTable().SelectColumns().FixedValue(false, "rslt").EndColumns().EndSelect()
                    .EndIf().GetResolvedQuery().QueryText;

        AccountBalanceQuery query2 = new AccountBalanceQuery
        {
             AccountTypeId = Guid.NewGuid(),
             AccountsIds = new List<string> {"343", "6334", "6454" },
              Balance = 43,
               Sign = ">", Status = VRAccountStatus.Active, EffectiveDate = DateTime.Now, Top = 34, OrderBy = "ASC"
        };
            var GetFilteredAccountBalances = new RDBQueryContext(dataProvider)
                        .Select()
                        .From(TABLE_NAME, "lb", query2.Top)
                        .Where().And()
                                    .EqualsCondition("AccountTypeID", query2.AccountTypeId)
                                    .ConditionIf(() => query2.AccountsIds != null && query2.AccountsIds.Count() > 0, ctx => ctx.ListCondition("AccountID", RDBListConditionOperator.IN, query2.AccountsIds))
                                    .ConditionIf(() => query2.Sign != null, ctx => ctx.CompareCondition("CurrentBalance", ConvertBalanceCompareSign(query2.Sign), query2.Balance))
                                    .ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted", false)
                                    .LiveBalanceActiveAndEffectiveCondition("lb", query2.Status, query2.EffectiveDate, query2.IsEffectiveInFuture)
                                .EndAnd()
                        .SelectColumns().AllTableColumns("lb").EndColumns()
                        .Sort().ByColumn("CurrentBalance", query2.OrderBy == "ASC" ? RDBSortDirection.ASC : RDBSortDirection.DESC).EndSort()
                        .EndSelect().GetResolvedQuery().QueryText;

            TABLE_NAME = "VR_AccountBalance_AccountUsage";

            var GetOverridenAccountUsagesByDeletedTransactionIds = 
             new RDBQueryContext(dataProvider)
                        .Select()
                        .From(TABLE_NAME, "au")
                        .Join()
                            .JoinSelect(RDBJoinType.Inner, "usageOverride")
                                .From(AccountUsageOverrideDataManager.TABLE_NAME, "usageOverride")
                                .Where().ListCondition("OverriddenByTransactionID", RDBListConditionOperator.IN, deletedTransactionIds)
                                .SelectColumns().Columns("AccountTypeID", "AccountID", "TransactionTypeID", "PeriodStart", "PeriodEnd").EndColumns()
                                .EndSelect()
                            .And()
                                .EqualsCondition("au", "AccountTypeID", "usageOverride", "AccountTypeID")
                                .EqualsCondition("au", "AccountID", "usageOverride", "AccountID")
                                .EqualsCondition("au", "TransactionTypeID", "usageOverride", "TransactionTypeID")
                                .CompareCondition("PeriodStart", RDBCompareConditionOperator.GEq, new RDBColumnExpression { TableAlias = "usageOverride", ColumnName = "PeriodStart" })
                                .CompareCondition("PeriodEnd", RDBCompareConditionOperator.LEq, new RDBColumnExpression { TableAlias = "usageOverride", ColumnName = "PeriodEnd" })
                            .EndAnd()
                        .EndJoin()
                        .SelectColumns().AllTableColumns("au").EndColumns()
                        .EndSelect().GetResolvedQuery().QueryText;

            List<AccountUsageByTime> accountUsagesByTime = new List<AccountUsageByTime> {
                new AccountUsageByTime { AccountId = "353", EndPeriod = DateTime.Now},
                new AccountUsageByTime { AccountId = "6543", EndPeriod = DateTime.Today}
            };

            
        
            var tempTableColumns3 = new Dictionary<string, RDBTableColumnDefinition>();
            tempTableColumns3.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            tempTableColumns3.Add("PeriodEnd", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            var tempTableQuery3 = new RDBTempTableQuery(tempTableColumns3);
            var GetAccountUsagesByTransactionTypes = new RDBQueryContext(dataProvider)
                        .StartBatchQuery()
                            .AddQuery().CreateTempTable(tempTableQuery3)
                            .Foreach(accountUsagesByTime, (usageByTime, ctx) =>
                                        ctx.AddQuery()
                                            .Insert()
                                            .IntoTable(tempTableQuery)
                                            .ColumnValue("AccountID", usageByTime.AccountId)
                                            .ColumnValue("PeriodEnd", usageByTime.EndPeriod)
                                            .EndInsert())
                            .AddQuery().Select()
                                        .From(TABLE_NAME, "au")
                                        .Join().Join(RDBJoinType.Inner, tempTableQuery3, "queryTable")
                                                .And()
                                                    .EqualsCondition("au", "AccountID", "queryTable", "AccountID")
                                                    .CompareCondition("PeriodEnd", RDBCompareConditionOperator.G, new RDBColumnExpression { TableAlias = "queryTable", ColumnName = "PeriodEnd" })
                                                .EndAnd()
                                        .EndJoin()
                                        .Where().And()
                                                    .EqualsCondition("AccountTypeID", accountTypeId)
                                                    .ConditionIf(() => billingTransactionTypeIds != null && billingTransactionTypeIds.Count() > 0, ctx => ctx.ListCondition("TransactionTypeID", RDBListConditionOperator.IN, billingTransactionTypeIds))
                                                    .ConditionIfColumnNotNull("IsOverridden").EqualsCondition("IsOverridden", false)
                                                .EndAnd()
                                        .SelectColumns().AllTableColumns("au").EndColumns()
                                       .EndSelect()
                            .EndBatchQuery().GetResolvedQuery().QueryText;

            Console.ReadKey();
            //RDBSchemaManager.Current.RegisterDefaultTableDefinition("SEC_User", _userTable);
            //RDBSchemaManager.Current.RegisterDefaultTableDefinition("SEC_Group", _groupTable);
            //RDBSchemaManager.Current.OverrideTableInfo(new Vanrise.Data.RDB.DataProvider.Providers.MSSQLRDBDataProvider().UniqueName, "SEC_User", null, "[User]");
            //int? id = 6;


            
           // Dictionary<string, Object> parameterValues;

           // Dictionary<string, Object> outputParameters;

           // var nbOfRows = new RDBQueryContext(dataProvider).Insert().IntoTable("SEC_User").GenerateIdAndAssignToParameter("UserId").ColumnValue("Name", "Ismail").EndInsert().ExecuteNonQuery(out outputParameters);

           // var selectQuery = new RDBQueryContext(dataProvider).Select().FromTable("SEC_User")
           //     .Join().JoinOnEqualOtherTableColumn("SEC_Group", "ID", "SEC_User", "GroupID").EndJoin()
           //     .Where().And()
           //                 .ConditionIf(() => id.HasValue).EqualsCondition("ID", 5)
           //                 .EqualsCondition("Name", "5dddd")
           //                 .Or()
           //                     .ConditionIf(() => id.HasValue).EqualsCondition("SEC_Group", "ID", 3)
           //                     .EqualsCondition("SEC_Group", "ID", 4)
           //                     .ConditionIf(() => id.HasValue).EqualsCondition("SEC_Group", "ID", id.Value)
           //                     .ConditionIfNotDefaultValue(DateTime.Now).EqualsCondition("Name", "fdsgf")
           //                     .ContainsCondition("Name", "sa")
           //                 .EndOr()
           //                 .CompareCondition("Age", RDBCompareConditionOperator.GEq, 45)
           //                 .ListCondition("CityID", RDBListConditionOperator.IN, new List<int> { 3, 5, 7, 85, 6, 5 })
           //                 .ListCondition("Name", RDBListConditionOperator.NotIN, new List<string> { "Admin", "Sales", "Billing", "Technical" })
           //             .EndAnd()
           //     .SelectColumns().Columns("ID", "Name").Column("SEC_Group", "Name", "GroupName").EndColumns().EndSelect();


           // var resolvedSelectquery = selectQuery.GetResolvedQuery();

           // var insertQuery = new RDBQueryContext(dataProvider).Insert().IntoTable("SEC_User")
           //     .IfNotExists().EqualsCondition("Email", "test@nodomain.com")
           //     .GenerateIdAndAssignToParameter("UserId").ColumnValue("Name", "test").ColumnValue("Email", "test@nodomain.com").ColumnValue("GroupID", 5).EndInsert();
           // var resolvedInsertQuery = insertQuery.GetResolvedQuery();


           // var updateQuery = new RDBQueryContext(dataProvider).Update().FromTable("SEC_User")
           // .IfNotExists().And().CompareCondition("ID", RDBCompareConditionOperator.NEq, 5).EqualsCondition("Email", "test@nodomain.com").EndAnd()
           // .Where().EqualsCondition("ID", 5)
           // .ColumnValue("Name", "test").ColumnValue("Email", "test@nodomain.com").ColumnValue("GroupID", 5).EndUpdate()
           // ;
           // var resolvedUpdateQuery = updateQuery.GetResolvedQuery();

           // Dictionary<string, RDBTableColumnDefinition> tempUserColumns = new Dictionary<string, RDBTableColumnDefinition>
           // {
           //     {"ID", new RDBTableColumnDefinition { DBColumnName = "DBID", DataType = RDBDataType.Int}},
           //       {"Name", new RDBTableColumnDefinition { DBColumnName = "DBName", DataType = RDBDataType.NVarchar, Size = 255}},
           //        {"GroupID", new RDBTableColumnDefinition { DBColumnName = "DBGroupID", DataType = RDBDataType.Int}},
           //        {"Age", new RDBTableColumnDefinition { DBColumnName = "DBAge", DataType = RDBDataType.Decimal, Size=20, Precision = 8}},
           //        {"CityID", new RDBTableColumnDefinition { DBColumnName = "DBCityID", DataType = RDBDataType.Int}},
           //        {"Email", new RDBTableColumnDefinition { DBColumnName = "DBEmail", DataType = RDBDataType.NVarchar, Size = 255}}
           // };

           // var tempUserTable = new RDBTempTableQuery(tempUserColumns);

           //var batchQuery = new RDBQueryContext(dataProvider)
           //    .StartBatchQuery()
           //         .AddQuery().CreateTempTable(tempUserTable)
           //         .AddQuery().Insert().IntoTable(tempUserTable).ColumnValue("ID", 2).ColumnValue("GroupID", 4).ColumnValue("Age", 34).EndInsert()
           //         .AddQuery().Insert().IntoTable(tempUserTable).ColumnValue("ID", 4).ColumnValue("GroupID", 5).ColumnValue("Age", 32).EndInsert()
           //         .AddQuery().Insert().IntoTable(tempUserTable).ColumnValue("ID", 3).ColumnValue("GroupID", 4).ColumnValue("Age", 22).EndInsert()
           //         .AddQuery().Insert().IntoTable(tempUserTable).ColumnValue("ID", 5).ColumnValue("GroupID", 4).ColumnValue("Age", 57).EndInsert()
           //         .AddQuery().Update().FromTable("SEC_User")
           //                     .Join().JoinOnEqualOtherTableColumn(tempUserTable, "ID", new RDBTableDefinitionQuerySource("SEC_User"), "ID").EndJoin()
           //                     //.ColumnValue("Name", new RDBColumnExpression { Table = tempUserTable, ColumnName = "Name" })
           //                     .ColumnValue("GroupID", new RDBColumnExpression { Table = tempUserTable, ColumnName = "GroupID" })
           //                     .ColumnValue("Age", new RDBColumnExpression { Table = tempUserTable, ColumnName = "Age" })
           //                     .ColumnValue("CityID", new RDBColumnExpression { Table = tempUserTable, ColumnName = "CityID" })
           //                     .EndUpdate()
           //    .EndBatchQuery()
           //                     //.ColumnValue("Email", new RDBColumnExpression { Table = tempUserTable, ColumnName = "Email" })
           //                     ;

           //  var resolvedBatchQuery = batchQuery.GetResolvedQuery();

           //  var selectQueryWithGrouping = new RDBQueryContext(dataProvider).Select().FromTable("SEC_User")//.StartSelectAggregates().Count("count").Avg("SEC_User", "Age", "AvgAge").EndSelectAggregates()
           //      .GroupBy()
           //          .Select().Column("CityID").Column("GroupID").EndColumns()
           //          .SelectAggregates().Count("Nb").Aggregate(RDBNonCountAggregateType.AVG, "Age", "AvgAge").EndSelectAggregates()
           //          .Having().And().CompareCount(RDBCompareConditionOperator.G, 1).CompareAggregate(RDBNonCountAggregateType.SUM, "Age", RDBCompareConditionOperator.G, 5).EndAnd()
           //      .EndGroupBy()
           //      .Sort().ByColumn("CityID", RDBSortDirection.ASC).ByAlias("AvgAge", RDBSortDirection.DESC).EndSort().EndSelect();
           //  var resolvedSelectWithGroupingquery = selectQueryWithGrouping.GetResolvedQuery();
           // Console.ReadKey();
           // //var query = new RDBQueryBuilder().Select().FromTable("User", "u").Columns("ID", "u.Name", "Settings", "g.Name")
           // //    .Join("Group", "g", RDBJoinType.Inner)
           // //    .IntCondition("", RDBConditionOperator.G, 0).And().DecimalCondition("", RDBConditionOperator.G, 3).EndJoin()
           // //    .Where()
           // //    .TextCondition("Name", RDBConditionOperator.Eq, "Sami")
           // //    .And().ConditionGroup().IntCondition("ID", RDBConditionOperator.L, 5).Or().IntCondition("ID", RDBConditionOperator.G, 10).EndConditionGroup()
           // //    .And().DecimalCondition("Age", RDBConditionOperator.G, 24).EndWhere();
        }

        private static RDBCompareConditionOperator ConvertBalanceCompareSign(string sign)
        {
            switch (sign)
            {
                case ">": return RDBCompareConditionOperator.G;
                case ">=": return RDBCompareConditionOperator.GEq;
                case "<": return RDBCompareConditionOperator.L;
                case "<=": return RDBCompareConditionOperator.LEq;
                default: throw new NotSupportedException(String.Format("Sign '{0},", sign));
            }
        }
        void TestAppDomain()
        {
            var domain = AppDomain.CreateDomain("ChildDomain");
            domain.DomainUnload += domain_DomainUnload;
            domain.ProcessExit += domain_ProcessExit;
            domain.UnhandledException += domain_UnhandledException;
            // use the proper assembly and type name.
            // child is a remote object here, ChildExample.dll is not loaded into the main domain
            ChildDomainClass child = domain.CreateInstanceAndUnwrap("Retail.Runtime", "Retail.Runtime.Tasks.ChildDomainClass") as ChildDomainClass;
            var parentDomainObj = new ParentDomainClass();
            // pass the host to the child
            child.Initialize(parentDomainObj);
            System.Threading.Thread.Sleep(2100);
            AppDomain.Unload(domain);
            //// now child can send feedbacks
            //child.DoSomeChildishJob();
        }

        static void domain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("domain_UnhandledException. {0}", e.ExceptionObject);
        }

        static void domain_ProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("Process Exit");
        }

        static void domain_DomainUnload(object sender, EventArgs e)
        {
            Console.WriteLine("Domain Unload");
        }

        private class TestData : Vanrise.Data.SQL.BaseSQLDataManager
        {
            protected override string GetConnectionString()
            {
                return @"Server=192.168.110.185\MSSQL2014;Database=ZajilBI_Online_CDR;Trusted_Connection=True;";
            }

            public List<CDR> ReadCDRs(bool withTop, int nbOfCDRs, string originatorNumber)
            {
                List<CDR> cdrs = new List<CDR>();
                StringBuilder query = new StringBuilder(sql);
                if (withTop)
                {
                    query.Replace("#TOP#", "Top " + nbOfCDRs.ToString());
                    query.Replace("#FILTER#", " AND OriginatorNumber = '" + originatorNumber + "'");
                }
                else
                {
                    query.Replace("#TOP#", "");
                    query.Replace("#FILTER#", " ");
                }
                ExecuteReaderText(query.ToString(), (reader) =>
                {
                    int index = 0;
                    while (reader.Read())
                    {
                        if (!withTop)
                        {
                            if (reader["OriginatorNumber"] as string != originatorNumber)
                                continue;
                        }
                        cdrs.Add(new CDR
                        {
                            ID = reader["ID"],
                            Call_Id = reader["Call_Id"],
                            ConnectDateTime = reader["ConnectDateTime"],
                            DisconnectDateTime = reader["DisconnectDateTime"],
                            DurationInSeconds = reader["DurationInSeconds"],
                            DisconnectReason = reader["DisconnectReason"],
                            CallProgressState = reader["CallProgressState"],
                            Account = reader["Account"],
                            OriginatorId = reader["OriginatorId"],
                            OriginatorNumber = reader["OriginatorNumber"],
                            OriginatorFromNumber = reader["OriginatorFromNumber"],
                            OriginalDialedNumber = reader["OriginalDialedNumber"],
                            TerminatorId = reader["TerminatorId"],
                            TerminatorNumber = reader["TerminatorNumber"],
                            IncomingGwId = reader["IncomingGwId"],
                            OutgoingGwId = reader["OutgoingGwId"],
                            TransferredCall_Id = reader["TransferredCall_Id"],
                            OriginatorIP = reader["OriginatorIP"],
                            TerminatorIP = reader["TerminatorIP"],
                            AttemptDateTime = reader["AttemptDateTime"],
                            FileName = reader["FileName"],
                            QueueItemId = reader["QueueItemId"],
                            InitiationCallType = reader["InitiationCallType"],
                            TerminationCallType = reader["TerminationCallType"]
                        });
                        index++;
                        if (index == nbOfCDRs)
                            break;
                    }
                }, null);
                return cdrs;
            }

            const string sql = @"SELECT #TOP# [ID]
      ,[Call_Id]
      ,[ConnectDateTime]
      ,[DisconnectDateTime]
      ,[DurationInSeconds]
      ,[DisconnectReason]
      ,[CallProgressState]
      ,[Account]
      ,[OriginatorId]
      ,[OriginatorNumber]
      ,[OriginatorFromNumber]
      ,[OriginalDialedNumber]
      ,[TerminatorId]
      ,[TerminatorNumber]
      ,[IncomingGwId]
      ,[OutgoingGwId]
      ,[TransferredCall_Id]
      ,[OriginatorIP]
      ,[TerminatorIP]
      ,[AttemptDateTime]
      ,[FileName]
      ,[QueueItemId]
      ,[InitiationCallType]
      ,[TerminationCallType]
  FROM[Retail_CDR].[CDR]  
where [AttemptDateTime] > '2016-11-15' and [AttemptDateTime] < '2016-11-20'
#FILTER#
order by [AttemptDateTime] desc";
        }

        public class CDR
        {
            public Object ID { get; set; }
            public Object Call_Id { get; set; }
            public Object ConnectDateTime { get; set; }
            public Object DisconnectDateTime { get; set; }
            public Object DurationInSeconds { get; set; }
            public Object DisconnectReason { get; set; }
            public Object CallProgressState { get; set; }
            public Object Account { get; set; }
            public Object OriginatorId { get; set; }
            public Object OriginatorNumber { get; set; }
            public Object OriginatorFromNumber { get; set; }
            public Object OriginalDialedNumber { get; set; }
            public Object TerminatorId { get; set; }
            public Object TerminatorNumber { get; set; }
            public Object IncomingGwId { get; set; }
            public Object OutgoingGwId { get; set; }
            public Object TransferredCall_Id { get; set; }
            public Object OriginatorIP { get; set; }
            public Object TerminatorIP { get; set; }
            public Object AttemptDateTime { get; set; }
            public Object FileName { get; set; }
            public Object QueueItemId { get; set; }
            public Object InitiationCallType { get; set; }
            public Object TerminationCallType { get; set; }
        }
    }

    public class ParentDomainClass : MarshalByRefObject
    {
        Guid _id;
        public ParentDomainClass()
        {
            _id = Guid.NewGuid();
            Console.WriteLine("Parent Domain Object created: {0}", _id);
        }

        public void CallFromChild()
        {
            Console.WriteLine("Call from client. Parent Id '{0}'", _id);
        }
    }

    public class ChildDomainClass : MarshalByRefObject
    {
        ParentDomainClass _parent;
        public void Initialize(ParentDomainClass parent)
        {
            _parent = parent;
            Task t = new Task(() =>
            {
                System.Threading.Thread.Sleep(2000);
                _parent.CallFromChild();
            });
            t.Start();
        }
    }
    public class WF : Activity
    {

        public InArgument<string> InputText { get; set; }

        public WF()
        {
            var sequence = new Sequence();
            var variable1 = new Variable<string>("Variable1");
            sequence.Variables.Add(variable1);
            this.Implementation = () => new Sequence
            {

                Variables = 
                 {
                      new Variable<string>{  Name = "Variable1", Default = "Var 1"}, 
                      new Variable<string>{  Name = "Variable2"}, 
                      new Variable<List<string>>{  Name = "List1"}
                 },
                Activities = 
                 {
                     //new WriteLine { Text = "From WriteLine"},
                     new WriteLine { Text = new InArgument<string>(new CSharpValue<string>("Variable1"))},
                     new WriteLine { Text = new CSharpValue<string>("DateTime.Now.ToString()")},
                     new ReadFromConsole { Output = new CSharpReference<string>("Variable2")},
                     new WriteLine { Text = new CSharpValue<string>("String.Format(\" this text {0} is written at {1}\", Variable2, DateTime.Now)")},
                     new Assign { To = new OutArgument<string>(new CSharpReference<string>("Variable1")), Value = new InArgument<string>(new CSharpValue<string>("\"new value for variable 1 \" + DateTime.Now.ToString()"))  },
                     new WriteLine { Text = new CSharpValue<string>("Variable1")},
                     new Assign { To = new OutArgument<List<string>>(new CSharpReference<List<string>>("List1")), Value = new InArgument<List<string>>(new CSharpValue<List<string>>("new List<string> { \"item1\", \"item2\", \"item3\" }"))  },
                     new Assign { To = new OutArgument<string>(new CSharpReference<string>("List1[1]")), Value = new InArgument<string>(new CSharpValue<string>("\" List Item2 value changed \" + DateTime.Now.ToString()"))  },
                     new WriteLine { Text = new CSharpValue<string>("List1[1]")},
                     new Assign { To = new OutArgument<string>(new CSharpReference<string>("List1[1]")), Value = new InArgument<string>(new CSharpValue<string>("List1[1] + \" value appended \" + DateTime.Now.ToString()"))  },
                     new WriteLine { Text = new CSharpValue<string>("List1[1]")}
                 }
            };
        }
    }

    public class ReadFromConsole : CodeActivity
    {
        public OutArgument<string> Output { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            string line = Console.ReadLine();
            this.Output.Set(context, line);
        }
    }

}