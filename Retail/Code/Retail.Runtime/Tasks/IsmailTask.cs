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

namespace Retail.Runtime.Tasks
{
    public class IsmailTask : ITask
    {
      
        public void Execute()
        {
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
            RDBSchemaManager.Current.RegisterDefaultTableDefinition("SEC_User", _userTable);
            RDBSchemaManager.Current.RegisterDefaultTableDefinition("SEC_Group", _groupTable);
            RDBSchemaManager.Current.OverrideTableInfo(new Vanrise.Data.RDB.DataProvider.Providers.MSSQLRDBDataProvider().UniqueName, "SEC_User", null, "[User]");
            int? id = 6;

            var dataProvider = new Vanrise.Data.RDB.DataProvider.Providers.MSSQLRDBDataProvider("Data Source=.;Initial Catalog=test;User ID=sa; Password=p@ssw0rd");
            Dictionary<string, Object> parameterValues;

            Dictionary<string, Object> outputParameters;

            var nbOfRows = new RDBQueryContext(dataProvider).Insert().IntoTable("SEC_User").GenerateIdAndAssignToParameter("UserId").ColumnValue("Name", "Ismail").EndInsert().ExecuteNonQuery(out outputParameters);

            var selectQuery = new RDBQueryContext(dataProvider).Select().FromTable("SEC_User")
                .Join().JoinOnEqualOtherTableColumn("SEC_Group", "ID", "SEC_User", "GroupID").EndJoin()
                .Where().And()
                            .ConditionIf(() => id.HasValue, (conditionContext) => conditionContext.EqualsCondition("ID", 5))
                            .EqualsCondition("Name", "5dddd")
                            .Or()
                                .ConditionIf(() => id.HasValue, (conditionContext) => conditionContext.EqualsCondition("SEC_Group", "ID", 3))
                                .EqualsCondition("SEC_Group", "ID", 4)
                                .ConditionIf(() => id.HasValue, (conditionContext) => conditionContext.EqualsCondition("SEC_Group", "ID", id.Value))
                                .ConditionIfNotDefaultValue(DateTime.Now, (conditionContext) => conditionContext.EqualsCondition("Name", "fdsgf"))
                                .ContainsCondition("Name", "sa")
                            .EndOr()
                            .CompareCondition("Age", RDBCompareConditionOperator.GEq, 45)
                            .ListCondition("CityID", RDBListConditionOperator.IN, new List<int> { 3, 5, 7, 85, 6, 5 })
                            .ListCondition("Name", RDBListConditionOperator.NotIN, new List<string> { "Admin", "Sales", "Billing", "Technical" })
                        .EndAnd()
                .SelectColumns().Columns("ID", "Name").Column("SEC_Group", "Name", "GroupName").EndColumns().EndSelect();


            var resolvedSelectquery = selectQuery.GetResolvedQuery();

            var insertQuery = new RDBQueryContext(dataProvider).Insert().IntoTable("SEC_User")
                .IfNotExists().EqualsCondition("Email", "test@nodomain.com")
                .GenerateIdAndAssignToParameter("UserId").ColumnValue("Name", "test").ColumnValue("Email", "test@nodomain.com").ColumnValue("GroupID", 5).EndInsert();
            var resolvedInsertQuery = insertQuery.GetResolvedQuery();


            var updateQuery = new RDBQueryContext(dataProvider).Update().FromTable("SEC_User")
            .IfNotExists().And().CompareCondition("ID", RDBCompareConditionOperator.NEq, 5).EqualsCondition("Email", "test@nodomain.com").EndAnd()
            .Where().EqualsCondition("ID", 5)
            .ColumnValue("Name", "test").ColumnValue("Email", "test@nodomain.com").ColumnValue("GroupID", 5).EndUpdate()
            ;
            var resolvedUpdateQuery = updateQuery.GetResolvedQuery();

            Dictionary<string, RDBTableColumnDefinition> tempUserColumns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {"ID", new RDBTableColumnDefinition { DBColumnName = "DBID", DataType = RDBDataType.Int}},
                  {"Name", new RDBTableColumnDefinition { DBColumnName = "DBName", DataType = RDBDataType.NVarchar, Size = 255}},
                   {"GroupID", new RDBTableColumnDefinition { DBColumnName = "DBGroupID", DataType = RDBDataType.Int}},
                   {"Age", new RDBTableColumnDefinition { DBColumnName = "DBAge", DataType = RDBDataType.Decimal, Size=20, Precision = 8}},
                   {"CityID", new RDBTableColumnDefinition { DBColumnName = "DBCityID", DataType = RDBDataType.Int}},
                   {"Email", new RDBTableColumnDefinition { DBColumnName = "DBEmail", DataType = RDBDataType.NVarchar, Size = 255}}
            };

            var tempUserTable = new RDBTempTableQuery(tempUserColumns);

           var batchQuery = new RDBQueryContext(dataProvider)
               .StartBatchQuery()
                    .AddQuery().CreateTempTable(tempUserTable)
                    .AddQuery().Insert().IntoTable(tempUserTable).ColumnValue("ID", 2).ColumnValue("GroupID", 4).ColumnValue("Age", 34).EndInsert()
                    .AddQuery().Insert().IntoTable(tempUserTable).ColumnValue("ID", 4).ColumnValue("GroupID", 5).ColumnValue("Age", 32).EndInsert()
                    .AddQuery().Insert().IntoTable(tempUserTable).ColumnValue("ID", 3).ColumnValue("GroupID", 4).ColumnValue("Age", 22).EndInsert()
                    .AddQuery().Insert().IntoTable(tempUserTable).ColumnValue("ID", 5).ColumnValue("GroupID", 4).ColumnValue("Age", 57).EndInsert()
                    .AddQuery().Update().FromTable("SEC_User")
                                .Join().JoinOnEqualOtherTableColumn(tempUserTable, "ID", new RDBTableDefinitionQuerySource("SEC_User"), "ID").EndJoin()
                                //.ColumnValue("Name", new RDBColumnExpression { Table = tempUserTable, ColumnName = "Name" })
                                .ColumnValue("GroupID", new RDBColumnExpression { Table = tempUserTable, ColumnName = "GroupID" })
                                .ColumnValue("Age", new RDBColumnExpression { Table = tempUserTable, ColumnName = "Age" })
                                .ColumnValue("CityID", new RDBColumnExpression { Table = tempUserTable, ColumnName = "CityID" })
                                .EndUpdate()
               .EndBatchQuery()
                                //.ColumnValue("Email", new RDBColumnExpression { Table = tempUserTable, ColumnName = "Email" })
                                ;

             var resolvedBatchQuery = batchQuery.GetResolvedQuery();

             var selectQueryWithGrouping = new RDBQueryContext(dataProvider).Select().FromTable("SEC_User")//.StartSelectAggregates().Count("count").Avg("SEC_User", "Age", "AvgAge").EndSelectAggregates()
                 .GroupBy()
                     .Select().Column("CityID").Column("GroupID").EndColumns()
                     .SelectAggregates().Count("Nb").Aggregate(RDBNonCountAggregateType.AVG, "Age", "AvgAge").EndSelectAggregates()
                     .Having().And().CompareCount(RDBCompareConditionOperator.G, 1).CompareAggregate(RDBNonCountAggregateType.SUM, "Age", RDBCompareConditionOperator.G, 5).EndAnd()
                 .EndGroupBy()
                 .Sort().ByColumn("CityID", RDBSortDirection.ASC).ByAlias("AvgAge", RDBSortDirection.DESC).EndSort().EndSelect();
             var resolvedSelectWithGroupingquery = selectQueryWithGrouping.GetResolvedQuery();
            Console.ReadKey();
            //var query = new RDBQueryBuilder().Select().FromTable("User", "u").Columns("ID", "u.Name", "Settings", "g.Name")
            //    .Join("Group", "g", RDBJoinType.Inner)
            //    .IntCondition("", RDBConditionOperator.G, 0).And().DecimalCondition("", RDBConditionOperator.G, 3).EndJoin()
            //    .Where()
            //    .TextCondition("Name", RDBConditionOperator.Eq, "Sami")
            //    .And().ConditionGroup().IntCondition("ID", RDBConditionOperator.L, 5).Or().IntCondition("ID", RDBConditionOperator.G, 10).EndConditionGroup()
            //    .And().DecimalCondition("Age", RDBConditionOperator.G, 24).EndWhere();
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

}
