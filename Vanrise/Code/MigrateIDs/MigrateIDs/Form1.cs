using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vanrise.Common;

namespace MigrateIDs
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

           var values = Enum.GetValues(typeof(MigratorOptions)).Cast<MigratorOptions>();
           Dictionary<MigratorOptions,string> enums = new Dictionary<MigratorOptions,string>();
            foreach(var v in values)
            {
               enums.Add(v,v.ToString());
            }
            comboBox1.DataSource = new BindingSource(enums, null);
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
        }
        private bool MigrateBusinessEntity()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                MainTableName = "BusinessEntity",
                SchemaName = "sec",
                OldColumnName = "OldID",
                IDColumnName = "ID",
                UpdatedTables = new List<MiratorTableInfo>{
                    new MiratorTableInfo {
                        TableName = "[Analytic].[AnalyticTable]",
                        ColumnName = "[Settings]",
                        Identifier="EntityId\""
                    },
                     new MiratorTableInfo {
                        TableName = "[genericdata].[DataRecordStorage]",
                        ColumnName = "[Settings]",
                        Identifier="EntityId\""
                    },
                    new MiratorTableInfo{
                        TableName ="[genericdata].[GenericRuleDefinition]",
                        ColumnName = "Details",
                        Identifier="EntityId\""
                    },
                    new MiratorTableInfo{
                        TableName ="[Analytic].[AnalyticItemConfig]",
                        ColumnName = "Config",
                        Identifier="EntityId\""
                    },
                    new MiratorTableInfo{
                        TableName ="[bp].[BPDefinition]",
                        ColumnName = "Config",
                        Identifier="EntityId\""
                    },
                    new MiratorTableInfo{
                        TableName ="[genericdata].[BusinessEntityDefinition]",
                        ColumnName = "Settings",
                        Identifier="EntityId\""
                    }
                }
            };
            MigratorManager manager = new MigratorManager();
            string query = manager.BuildQuery(parameter);
            return manager.Migrate(query);
        }
        private bool MigrateDataRecordType()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                MainTableName = "DataRecordType",
                SchemaName = "genericdata",
                OldColumnName = "OldID",
                IDColumnName = "ID",
                UpdatedTables = new List<MiratorTableInfo>{
                    new MiratorTableInfo {
                        TableName = "[Analytic].[AnalyticItemConfig]",
                        ColumnName = "[Config]",
                        Identifier="DataRecordTypeId\""
                    },
                     new MiratorTableInfo {
                        TableName = "[Analytic].[DataAnalysisDefinition]",
                        ColumnName = "[Settings]",
                        Identifier="DataRecordTypeId\""
                    },
                    new MiratorTableInfo{
                        TableName ="[Analytic].[DataAnalysisItemDefinition]",
                        ColumnName = "Settings",
                        Identifier="RecordTypeId\""
                    },
                    new MiratorTableInfo{
                        TableName ="[genericdata].[BusinessEntityDefinition]",
                        ColumnName = "Settings",
                        Identifier="DataRecordTypeId\""
                    },
                    new MiratorTableInfo{
                        TableName ="[genericdata].[SummaryTransformationDefinition]",
                        ColumnName = "Details",
                        Identifier="RawItemRecordTypeId\""
                    },
                    new MiratorTableInfo{
                        TableName ="[genericdata].[SummaryTransformationDefinition]",
                        ColumnName = "Details",
                        Identifier="SummaryItemRecordTypeId\""
                    },
                    new MiratorTableInfo{
                        TableName ="[genericdata].[DataTransformationDefinition]",
                        ColumnName = "Details",
                        Identifier="DataRecordTypeId\""
                    },
                    new MiratorTableInfo{
                        TableName ="[Analytic].[AnalyticReport]",
                        ColumnName = "Settings",
                        Identifier="DataRecordTypeId\""
                    },
                    new MiratorTableInfo{
                        TableName ="[sec].[View]",
                        ColumnName = "Settings",
                        Identifier="DataRecordTypeId\""
                    },
                    new MiratorTableInfo{
                        TableName ="[VR_Invoice].[InvoiceType]",
                        ColumnName = "Settings",
                        Identifier="InvoiceDetailsRecordTypeId\"",
                        ConnectionStringKey = "SecondDBConnStringKey"
                    }
                    ,
                    new MiratorTableInfo{
                        TableName ="[Mediation_Generic].[MediationDefinition]",
                        ColumnName = "Details",
                        Identifier="ParsedRecordTypeId\"",
                        ConnectionStringKey = "MediationDBConnStringKey"
                    }
                    ,
                    new MiratorTableInfo{
                        TableName ="[Mediation_Generic].[MediationDefinition]",
                        ColumnName = "Details",
                        Identifier="CookedRecordTypeId\"",
                        ConnectionStringKey = "MediationDBConnStringKey"
                    }
                    ,
                    new MiratorTableInfo{
                        TableName ="[queue].[ExecutionFlowDefinition]",
                        ColumnName = "Stages",
                        Identifier="DataRecordTypeId\"",
                       // ConnectionStringKey = "TransactionDBConnStringKey"
                    } ,
                    new MiratorTableInfo{
                        TableName ="[Retail_BE].[AccountPartDefinition]",
                        ColumnName = "Details",
                        Identifier="RecordTypeId\"",
                       ConnectionStringKey = "RetailDBConnStringKey"
                    }
                }
            };
            MigratorManager manager = new MigratorManager();
            string query = manager.BuildQuery(parameter);
            return manager.Migrate(query);
        }
        private bool MigrateBusinessEntityDefinition()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                MainTableName = "BusinessEntityDefinition",
                SchemaName = "genericdata",
                OldColumnName = "OldID",
                IDColumnName = "ID",
                UpdatedTables = new List<MiratorTableInfo>{
                    new MiratorTableInfo {
                        TableName = "[genericdata].[BELookupRuleDefinition]",
                        ColumnName = "[Details]",
                        Identifier="BusinessEntityDefinitionId\""
                    },
                     new MiratorTableInfo {
                        TableName = "[genericdata].[ExtensibleBEItem]",
                        ColumnName = "[Details]",
                        Identifier="BusinessEntityDefinitionId\""
                    },
                    new MiratorTableInfo{
                        TableName ="[genericdata].[DataRecordType]",
                        ColumnName = "Fields",
                        Identifier="BusinessEntityDefinitionId\""
                    },
                    new MiratorTableInfo{
                        TableName ="[common].[Setting]",
                        ColumnName = "Data",
                        Identifier="AccountBusinessEntityDefinitionId\""
                    },
                    new MiratorTableInfo{
                        TableName ="[sec].[View]",
                        ColumnName = "Settings",
                        Identifier="BusinessEntityDefinitionId\"",
                    },
                    new MiratorTableInfo{
                        TableName ="[genericdata].[GenericRuleDefinition]",
                        ColumnName = "Details",
                        Identifier="BusinessEntityDefinitionId\"",
                    },
                    new MiratorTableInfo{
                        TableName ="[Analytic].[AnalyticItemConfig]",
                        ColumnName = "Config",
                        Identifier="BusinessEntityDefinitionId\"",
                    },
                    new MiratorTableInfo{
                        TableName ="[genericdata].[DataTransformationDefinition]",
                        ColumnName = "Details",
                        Identifier="BusinessEntityDefinitionId\"",
                    },
                    new MiratorTableInfo{
                        TableName ="[Analytic].[DataAnalysisItemDefinition]",
                        ColumnName = "Settings",
                        Identifier="BusinessEntityDefinitionId\"",
                    }
                    
                }
            };
            MigratorManager manager = new MigratorManager();
            string query = manager.BuildQuery(parameter);
            return manager.Migrate(query);
        }
        private bool MigrateBELookupRuleDefinition()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                MainTableName = "BELookupRuleDefinition",
                SchemaName = "genericdata",
                OldColumnName = "OldID",
                IDColumnName = "ID",
                UpdatedTables = new List<MiratorTableInfo>{
                    new MiratorTableInfo {
                        TableName = "[genericdata].[DataTransformationDefinition]",
                        ColumnName = "[Details]",
                        Identifier="BELookupRuleDefinitionId\""
                    },
                }
            };
            MigratorManager manager = new MigratorManager();
            string query = manager.BuildQuery(parameter);
            return manager.Migrate(query);
        }
        private bool MigrateGenericRuleDefinition()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                MainTableName = "GenericRuleDefinition",
                SchemaName = "genericdata",
                OldColumnName = "OldID",
                IDColumnName = "ID",
                UpdatedTables = new List<MiratorTableInfo>{
                    new MiratorTableInfo {
                        TableName = "[common].[Setting]",
                        ColumnName = "[Data]",
                        Identifier="BalanceAlertRuleDefinitionId\""
                    },new MiratorTableInfo {
                        TableName = "[sec].[View]",
                        ColumnName = "[Settings]",
                        Identifier="RuleDefinitionId\""
                    },new MiratorTableInfo {
                        TableName = "[genericdata].[DataTransformationDefinition]",
                        ColumnName = "[Details]",
                        Identifier="RuleDefinitionId\""
                    },new MiratorTableInfo {
                        TableName = "[rules].[Rule]",
                        ColumnName = "[RuleDetails]",
                        Identifier="DefinitionId\""
                    },new MiratorTableInfo {
                        TableName = "[genericdata].[GenericRuleDefinition]",
                        ColumnName = "[Details]",
                        Identifier="GenericRuleDefinitionId\""
                    },new MiratorTableInfo {
                        TableName = "[Retail].[ServiceType]",
                        ColumnName = "[Settings]",
                        Identifier="RuleDefinitionId\"",
                         ConnectionStringKey = "RetailDBConnStringKey"
                    },new MiratorTableInfo {
                        TableName = "[Retail].[ServiceType]",
                        ColumnName = "[Settings]",
                        Identifier="IdentificationRuleDefinitionId\"",
                        ConnectionStringKey = "RetailDBConnStringKey"
                    },
                }
            };
            MigratorManager manager = new MigratorManager();
            string query = manager.BuildQuery(parameter);
            return manager.Migrate(query);
        }
        private bool MigrateDataTransfomationDefinition()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                MainTableName = "DataTransformationDefinition",
                SchemaName = "genericdata",
                OldColumnName = "OldID",
                IDColumnType = "int",
                IsPrimaryColumn = true,
                IDColumnName = "ID",
                UpdatedTables = new List<MiratorTableInfo>{
                     new MiratorTableInfo {
                        TableName = "[genericdata].DataTransformationDefinition",
                        ColumnName = "[Details]",
                        Identifier="DataTransformationDefinitionId\""
                    },
                    new MiratorTableInfo {
                        TableName = "[queue].[ExecutionFlowDefinition]",
                        ColumnName = "[Stages]",
                        Identifier="DataTransformationDefinitionId\""
                    },new MiratorTableInfo {
                        TableName = "[genericdata].[SummaryTransformationDefinition]",
                        ColumnName = "[Details]",
                        Identifier="TransformationDefinitionId\""
                    },new MiratorTableInfo {
                        TableName = "[common].[Setting]",
                        ColumnName = "[Data]",
                        Identifier="CustomerTransformationId\""
                    },new MiratorTableInfo {
                        TableName = "[common].[Setting]",
                        ColumnName = "[Data]",
                        Identifier="SupplierTransformationId\""
                    },new MiratorTableInfo {
                        TableName = "[queue].[QueueInstance]",
                        ColumnName = "[Settings]",
                        Identifier="DataTransformationDefinitionId\""
                    },new MiratorTableInfo {
                        TableName = "[genericdata].DataTransformationDefinition",
                        ColumnName = "[Details]",
                        Identifier="DataTransformationId\""
                    }
                }
            };
            MigratorManager manager = new MigratorManager();
            //manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            //manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            //manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            string query = manager.BuildQuery(parameter);
            return manager.Migrate(query);
        }
        private bool MigrateServiceType()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                MainTableName = "ServiceType",
                SchemaName = "Retail",
                OldColumnName = "OldID",
                IDColumnName = "ID",
                UpdatedTables = new List<MiratorTableInfo>{
                    new MiratorTableInfo {
                        TableName = "[Retail_BE].[ActionDefinition]",
                        ColumnName = "[Settings]",
                        Identifier="EntityTypeId\""
                    }, new MiratorTableInfo {
                        TableName = "[dbo].[OperatorDeclaredInfo]",
                        ColumnName = "[Settings]",
                        Identifier="ServiceTypeId\""
                    }, new MiratorTableInfo {
                        TableName = "[Retail].[Package]",
                        ColumnName = "[Settings]",
                        Identifier="ServiceTypeId\""
                    },
                }
            };
            MigratorManager manager = new MigratorManager();
            string query = manager.BuildQuery(parameter);
            return manager.Migrate(query);
        }
        private bool MigratePartDefinition()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                MainTableName = "AccountPartDefinition",
                SchemaName = "Retail_BE",
                OldColumnName = "OldID",
                IDColumnName = "ID",
                UpdatedTables = new List<MiratorTableInfo>{
                    new MiratorTableInfo {
                        TableName = "[Retail_BE].[AccountType]",
                        ColumnName = "[Settings]",
                        Identifier="PartDefinitionId\""
                    },
                     new MiratorTableInfo {
                        TableName = "[Retail_BE].[AccountType]",
                        ColumnName = "[Settings]",
                        Identifier="PartDefinitionId\""
                    }
                }
            };
            MigratorManager manager = new MigratorManager();
            string query = manager.BuildQuery(parameter);
            return manager.Migrate(query);
        }
        private bool MigrateSummaryDataTransfomationDefinition()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                MainTableName = "SummaryTransformationDefinition",
                SchemaName = "genericdata",
                OldColumnName = "OldID",
                IDColumnType="int",
                IDColumnName = "ID",
                IsPrimaryColumn = true,
                UpdatedTables = new List<MiratorTableInfo>{
                    new MiratorTableInfo {
                        TableName = "[queue].[ExecutionFlowDefinition]",
                        ColumnName = "[Stages]",
                        Identifier="SummaryTransformationDefinitionId\""
                    },
                    new MiratorTableInfo {
                        TableName = "[genericdata].SummaryTransformationDefinition",
                        ColumnName = "[Details]",
                        Identifier="SummaryTransformationDefinitionId\""
                    },new MiratorTableInfo {
                        TableName = "[queue].[QueueInstance]",
                        ColumnName = "[Settings]",
                        Identifier="SummaryTransformationDefinitionId\""
                    }
                }
            };
            MigratorManager manager = new MigratorManager();
            //manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            //manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            //manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            string query = manager.BuildQuery(parameter);
            return manager.Migrate(query);
        }
        private bool MigrateDataRecordStorage()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                MainTableName = "DataRecordStorage",
                SchemaName = "genericdata",
                OldColumnName = "OldID",
                IDColumnName = "ID",
                IDColumnType = "int",
                IsPrimaryColumn = true,
                UpdatedTables = new List<MiratorTableInfo>{
                    new MiratorTableInfo {
                        TableName = "[genericdata].[SummaryTransformationDefinition]",
                        ColumnName = "[Details]",
                        Identifier="DataRecordStorageId\""
                    },new MiratorTableInfo {
                        TableName = "[queue].[ExecutionFlowDefinition]",
                        ColumnName = "[Stages]",
                        Identifier="DataRecordStorageId\""
                    },new MiratorTableInfo {
                        TableName = "[queue].[ExecutionFlowDefinition]",
                        ColumnName = "[Stages]",
                        Identifier="DataRecordStorageId\""
                    },new MiratorTableInfo {
                        TableName = "[queue].[QueueInstance]",
                        ColumnName = "[Settings]",
                        Identifier="DataRecordStorageId\""
                    }
                }
            };
            MigratorManager manager = new MigratorManager();
            //manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            //manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            //manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));


            //MiratorParameter parameter1 = new MiratorParameter
            //{
            //    MainTableName = "DataRecordStorage",
            //    SchemaName = "genericdata",
            //    OldColumnName = "OldDataStoreID",
            //    IDColumnName = "DataStoreID",
            //    UpdatedTables = new List<MiratorTableInfo>()
            //};
            //manager.Migrate(manager.BuildRenameQuery(parameter1));
            string query = manager.BuildQuery(parameter);
            return manager.Migrate(query);
        }
        private bool MigrateAnalyticReport()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                MainTableName = "AnalyticReport",
                SchemaName = "Analytic",
                OldColumnName = "OldID",
                IDColumnName = "ID",
                IDColumnType = "int",
                IsPrimaryColumn = true,
                UpdatedTables = new List<MiratorTableInfo>{
                    new MiratorTableInfo {
                        TableName = "[sec].[View]",
                        ColumnName = "[Settings]",
                        Identifier="AnalyticReportId\""
                    }
                }
            };
            MigratorManager manager = new MigratorManager();
            string query = manager.BuildQuery(parameter);
            //manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            //manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            //manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            return manager.Migrate(query);
        }
        private bool MigrateSchedulerTaskTriggerType()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "runtime",
                MainTableName = "SchedulerTaskTriggerType",
                OldColumnName = "OldID",
                IDColumnName = "ID",
                IDColumnType = "int",
                IsPrimaryColumn = true,
                UpdatedTables = new List<MiratorTableInfo>()
            };
            MigratorManager manager = new MigratorManager();
            //manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            //manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            //manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            return true;
        }
        private bool MigrateSchedulerTaskActionType()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "runtime",
                MainTableName = "SchedulerTaskActionType",
                OldColumnName = "OldID",
                IDColumnName = "ID",
                IDColumnType = "int",
                IsPrimaryColumn = true,
                UpdatedTables = new List<MiratorTableInfo>()
            };
            MigratorManager manager = new MigratorManager();
            //manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            //manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            //manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            return true;
        }
        private bool MigrateDataStore()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "genericdata",
                MainTableName = "DataStore",
                OldColumnName = "OldID",
                IDColumnName = "ID",
                IDColumnType = "int",
                IsPrimaryColumn = true,
                UpdatedTables = new List<MiratorTableInfo>()
            };
            MigratorManager manager = new MigratorManager();
            //manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            //manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            //manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            return true;
        }
        private bool MigrateSetting()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "common",
                MainTableName = "Setting",
                OldColumnName = "OldID",
                IDColumnName = "ID",
                IDColumnType = "int",
                IsPrimaryColumn = true,
                UpdatedTables = new List<MiratorTableInfo>()
            };
            MigratorManager manager = new MigratorManager();
            //manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            //manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            //manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            return true;
        }
        private bool MigrateView()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "sec",
                MainTableName = "View",
                OldColumnName = "OldType",
                IDColumnName = "Type",
                IDColumnType = "int",
                UpdatedTables = new List<MiratorTableInfo>()
            };
            MigratorManager manager = new MigratorManager();
            //manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            //manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            //manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));

            return true;
        }
        private bool MigrateViewType()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "sec",
                MainTableName = "ViewType",
                OldColumnName = "OldID",
                IDColumnName = "ID",
                IDColumnType = "int",
                IsPrimaryColumn = true,
                UpdatedTables = new List<MiratorTableInfo>()
            };
            MigratorManager manager = new MigratorManager();
            //manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            //manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            //manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            return true;
        }
        private bool MigrateScheduleTask()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "runtime",
                MainTableName = "ScheduleTask",
                OldColumnName = "OldActionTypeId",
                IDColumnName = "ActionTypeId",
                IDColumnType="int",
                UpdatedTables = new List<MiratorTableInfo>()
            };
            MigratorManager manager = new MigratorManager();
            //manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            //manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            //manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));

            MiratorParameter parameter1 = new MiratorParameter
            {
                SchemaName = "runtime",
                MainTableName = "ScheduleTask",
                OldColumnName = "OldTriggerTypeId",
                IDColumnName = "TriggerTypeId",
                IDColumnType = "int",
                UpdatedTables = new List<MiratorTableInfo>()
            };
            //manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            //manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            //manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            return true;
        }
        private bool MigrateExtensibleBEItem()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "genericdata",
                MainTableName = "ExtensibleBEItem",
                OldColumnName = "OldID",
                IDColumnName = "ID",
                IDColumnType = "int",
                IsPrimaryColumn = true,
                UpdatedTables = new List<MiratorTableInfo>()
            };
            MigratorManager manager = new MigratorManager();
            //manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            //manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            //manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            return true;
        }
        private bool MigrateAnalyticItemConfig()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "Analytic",
                MainTableName = "AnalyticItemConfig",
                OldColumnName = "OldID",
                IDColumnName = "ID",
                IDColumnType = "int",
                IsPrimaryColumn = true,
                UpdatedTables = new List<MiratorTableInfo>()
            };
            MigratorManager manager = new MigratorManager();
            //manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            //manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            //manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            return true;
        }

        private bool MigrateBPBusinessRuleDefinition()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "bp",
                MainTableName = "BPBusinessRuleDefinition",
                OldColumnName = "OldID",
                IDColumnName = "ID",
                IDColumnType = "int",
                IsPrimaryColumn = true,
                UpdatedTables = new List<MiratorTableInfo>()
            };
            MigratorManager manager = new MigratorManager();
            manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));



            MiratorParameter parameter1 = new MiratorParameter
            {
                SchemaName = "bp",
                MainTableName = "BPBusinessRuleDefinition",
                OldColumnName = "OldBPDefintionId",
                IDColumnName = "BPDefintionId",
                IDColumnType = "int",
                IsPrimaryColumn = false,
                UpdatedTables = new List<MiratorTableInfo>()
            };
            manager.Migrate(manager.BuildAddOldColumnQuery(parameter1));
            manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter1));
            manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter1));


            return true;
        }
        private bool MigrateBPBusinessRuleAction()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "bp",
                MainTableName = "BPBusinessRuleAction",
                OldColumnName = "OldBusinessRuleDefinitionId",
                IDColumnName = "BusinessRuleDefinitionId",
                IDColumnType = "int",
                UpdatedTables = new List<MiratorTableInfo>()
            };
            MigratorManager manager = new MigratorManager();
            manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            return true;
        }
        private bool MigrateBPDefinition()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "bp",
                MainTableName = "BPDefinition",
                OldColumnName = "OldId",
                IDColumnName = "ID",
                IDColumnType = "int",
                IsPrimaryColumn = true,
                UpdatedTables = new List<MiratorTableInfo>()
            };
            MigratorManager manager = new MigratorManager();
            manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            return true;
        }
        private bool MigrateBPTaskType()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "bp",
                MainTableName = "BPTaskType",
                OldColumnName = "OldId",
                IDColumnName = "ID",
                IDColumnType = "int",
                IsPrimaryColumn = true,
                UpdatedTables = new List<MiratorTableInfo>()
            };
            MigratorManager manager = new MigratorManager();
            manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            return true;
        }
        private bool MigrateBPBusinessRuleSet()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "bp",
                MainTableName = "BPBusinessRuleSet",
                OldColumnName = "OldBPDefinitionId",
                IDColumnName = "BPDefinitionId",
                IDColumnType = "int",
                IsPrimaryColumn = false,
                UpdatedTables = new List<MiratorTableInfo>()
            };
            MigratorManager manager = new MigratorManager();
            manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            return true;
        }
        private bool MigrateReprocessDefinition()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "reprocess",
                MainTableName = "ReprocessDefinition",
                OldColumnName = "OldId",
                IDColumnName = "Id",
                IDColumnType = "int",
                IsPrimaryColumn = true,
                UpdatedTables = new List<MiratorTableInfo>()
            };
            MigratorManager manager = new MigratorManager();
            manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            return true;
        }
        private bool MigrateQueueExecutionFlow()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "queue",
                MainTableName = "ExecutionFlow",
                OldColumnName = "OldId",
                IDColumnName = "Id",
                IDColumnType = "int",
                IsPrimaryColumn = true,
                UpdatedTables = new List<MiratorTableInfo>
                {
                    new MiratorTableInfo {
                        TableName = "[integration].[DataSource]",
                        ColumnName = "[Settings]",
                        Identifier="ExecutionFlowId\""
}
                }
            };
            MigratorManager manager = new MigratorManager();
            manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            manager.Migrate(manager.BuildQuery(parameter));



            MiratorParameter parameter1 = new MiratorParameter
            {
                SchemaName = "queue",
                MainTableName = "ExecutionFlow",
                OldColumnName = "OldExecutionFlowDefinitionID",
                IDColumnName = "ExecutionFlowDefinitionID",
                IDColumnType = "int",
                IsPrimaryColumn = false,
                UpdatedTables = new List<MiratorTableInfo>()
            };
            manager.Migrate(manager.BuildAddOldColumnQuery(parameter1));
            manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter1));
            manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter1));
            return true;
        }

        private bool MigrateQueueExecutionFlowDefinition()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "queue",
                MainTableName = "ExecutionFlowDefinition",
                OldColumnName = "OldId",
                IDColumnName = "Id",
                IDColumnType = "int",
                IsPrimaryColumn = true,
                UpdatedTables = new List<MiratorTableInfo>
                {
                    new MiratorTableInfo {
                        TableName = "[reprocess].[ReprocessDefinition]",
                        ColumnName = "[Settings]",
                        Identifier="ExecutionFlowDefinitionId\""
}
                }
            };
            MigratorManager manager = new MigratorManager();
            manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            manager.Migrate(manager.BuildQuery(parameter));
            return true;
        }
    
        private bool MigrateQueueInstance()
        {
            MiratorParameter parameter = new MiratorParameter
            {
                SchemaName = "queue",
                MainTableName = "QueueInstance",
                OldColumnName = "OldExecutionFlowID",
                IDColumnName = "ExecutionFlowID",
                IDColumnType = "int",
                IsPrimaryColumn = false,
                UpdatedTables = new List<MiratorTableInfo>()
            };
            MigratorManager manager = new MigratorManager();
            manager.Migrate(manager.BuildAddOldColumnQuery(parameter));
            manager.Migrate(manager.BuildUpdateOldIdsQuery(parameter));
            manager.Migrate(manager.BuildDropOldIdAndAddNewOneColumnQuery(parameter));
            return true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MigratorOptions option = (MigratorOptions)comboBox1.SelectedValue;
                bool result = false;
                switch (option)
                {
                    case MigratorOptions.BusinessEntity: result = MigrateBusinessEntity(); break;
                    case MigratorOptions.RecordType: result = MigrateDataRecordType(); break;
                    case MigratorOptions.BusinessEntityDefinition: result = MigrateBusinessEntityDefinition(); break;
                    case MigratorOptions.BELookupRuleDefinition: result = MigrateBELookupRuleDefinition(); break;
                    case MigratorOptions.GenericRuleDefinition: result = MigrateGenericRuleDefinition(); break;
                    case MigratorOptions.DataTransfomationDefinition: result = MigrateDataTransfomationDefinition(); break;
                    case MigratorOptions.ServiceType: result = MigrateServiceType(); break;
                    case MigratorOptions.SummaryDataTransfomationDefinition: result = MigrateSummaryDataTransfomationDefinition(); break;
                    case MigratorOptions.DataRecordStorage: result = MigrateDataRecordStorage(); break;
                    case MigratorOptions.AnalyticReport: result = MigrateAnalyticReport(); break;
                    case MigratorOptions.SchedulerTaskTriggerType: result = MigrateSchedulerTaskTriggerType(); break;
                    case MigratorOptions.PartDefinition: result = MigratePartDefinition(); break;
                    case MigratorOptions.SchedulerTaskActionType: result = MigrateSchedulerTaskActionType(); break;
                    case MigratorOptions.AnalyticItemConfig: result = MigrateAnalyticItemConfig(); break;
                    case MigratorOptions.ExtensibleBEItem: result = MigrateExtensibleBEItem(); break;
                    case MigratorOptions.ScheduleTask: result = MigrateScheduleTask(); break;
                    case MigratorOptions.ViewType: result = MigrateViewType(); break;
                    case MigratorOptions.Setting: result = MigrateSetting(); break;
                    case MigratorOptions.View: result = MigrateView(); break;
                    case MigratorOptions.DataStore: result = MigrateDataStore(); break;
                    case MigratorOptions.BPBusinessRuleDefinition: result = MigrateBPBusinessRuleDefinition(); break;

                    case MigratorOptions.BPBusinessRuleAction: result = MigrateBPBusinessRuleAction(); break;
                    case MigratorOptions.BPDefinition: result = MigrateBPDefinition(); break;
                    case MigratorOptions.BPTaskType: result = MigrateBPTaskType(); break;
                    case MigratorOptions.BPBusinessRuleSet: result = MigrateBPBusinessRuleSet(); break;
                    case MigratorOptions.ReprocessDefinition: result = MigrateReprocessDefinition(); break;
                    case MigratorOptions.QueueExecutionFlow: result = MigrateQueueExecutionFlow(); break;
                    case MigratorOptions.QueueInstance: result = MigrateQueueInstance(); break;
                    case MigratorOptions.ExecutionFlowDefinition: result = MigrateQueueExecutionFlowDefinition(); break;

                    default: return;
                }
                if (result)
                    MessageBox.Show("Migration done successfully.");
                else
                    MessageBox.Show("Migration failed.");
            }catch(Exception ex)
            {
                MessageBox.Show(string.Format("Error Migration: {0}", ex));
            }
         
        }

    }
}
