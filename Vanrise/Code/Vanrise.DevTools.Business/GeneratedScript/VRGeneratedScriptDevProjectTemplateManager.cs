using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.DevTools.Data;
using Vanrise.DevTools.Entities;
namespace Vanrise.DevTools.Business
{
    public class VRGeneratedScriptDevProjectTemplateManager
    {
        static List<VRGeneratedScriptTable> TableNames = new List<VRGeneratedScriptTable>{
            new VRGeneratedScriptTable() { Name = "AnalyticTable" } ,
            new VRGeneratedScriptTable() { Name = "AnalyticItemConfig" } ,
            new VRGeneratedScriptTable() { Name = "AnalyticReport" } ,
            new VRGeneratedScriptTable() { Name ="DataAnalysisDefinition" },
            new VRGeneratedScriptTable() { Name ="DataAnalysisItemDefinition" },
            new VRGeneratedScriptTable() { Name = "VRWorkflow" } ,
            new VRGeneratedScriptTable() { Name ="BPDefinition" },
            new VRGeneratedScriptTable() { Name ="BPTaskType" } ,
            new VRGeneratedScriptTable() { Name ="ExtensionConfiguration" },
            new VRGeneratedScriptTable() { Name ="VRObjectTypeDefinition" },
            new VRGeneratedScriptTable() { Name ="MailMessageType" },
            new VRGeneratedScriptTable() { Name="StatusDefinition"},
            new VRGeneratedScriptTable() { Name ="VRComponentType" },
            new VRGeneratedScriptTable() { Name ="VRNamespace" },
            new VRGeneratedScriptTable() { Name ="VRNamespaceItem" },
            new VRGeneratedScriptTable() { Name ="VRDynamicAPIModule" },
            new VRGeneratedScriptTable() { Name ="VRDynamicAPIModule" },
            new VRGeneratedScriptTable() { Name ="VRDynamicAPI" },
            new VRGeneratedScriptTable() { Name ="ParserType" },
            new VRGeneratedScriptTable() { Name = "DataStore" } ,
            new VRGeneratedScriptTable() { Name = "DataRecordType" },
            new VRGeneratedScriptTable() { Name ="DataRecordStorage" } ,
            new VRGeneratedScriptTable() { Name ="BusinessEntityDefinition" } ,
            new VRGeneratedScriptTable() { Name ="DataRecordFieldChoice" } ,
            new VRGeneratedScriptTable() { Name ="DataTransformationDefinition" } ,
            new VRGeneratedScriptTable() { Name ="SummaryTransformationDefinition" } ,
            new VRGeneratedScriptTable() { Name ="GenericRuleDefinition" } ,
            new VRGeneratedScriptTable() { Name ="VRNumberPrefixType" } ,
            new VRGeneratedScriptTable() { Name ="VRNumberPrefix" } ,
            new VRGeneratedScriptTable() { Name ="ExecutionFlowDefinition" },
            new VRGeneratedScriptTable() { Name ="ExecutionFlow" },
            new VRGeneratedScriptTable() { Name ="QueueActivatorConfig" },
            new VRGeneratedScriptTable() { Name ="ReprocessDefinition" },
            new VRGeneratedScriptTable() { Name ="BusinessEntity" },
            new VRGeneratedScriptTable() { Name ="BusinessEntityModule" },
            new VRGeneratedScriptTable() { Name ="Module" },
            new VRGeneratedScriptTable() { Name ="View" }
        }; 
      public class VRGeneratedScriptDevProjectTableParameters
        {
            public Guid DevProjectId { get; set; }
            public string TableName { get; set; }
            public string Schema { get; set; }
            public string IdColumnName { get; set; }
            public string WhereCondition { get; set; }
            public string JoinCondition { get; set; }
        }
        public class VRGeneratedScriptDevProjectTables
        {
            public VRGeneratedScriptDevProjectTables(Guid devProjectId)
            {
                DevProjectId = devProjectId;
                WhereCondition = string.Format("DevProjectID = '{0}'", devProjectId);
                JoinedWhereCondition = string.Format("rec.DevProjectID = '{0}'", devProjectId);
                Tables = new Dictionary<string, VRGeneratedScriptDevProjectTableParameters>()
                {
                    { "VRDevProject",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="VRDevProject",Schema="common",IdColumnName="ID",WhereCondition=string.Format("ID = '{0}'", devProjectId)}},
                    { "AnalyticTable",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="AnalyticTable",Schema="Analytic",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "AnalyticItemConfig",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="AnalyticItemConfig",Schema="Analytic",IdColumnName="ID",WhereCondition=JoinedWhereCondition,JoinCondition=GetJoinCondition("Analytic","AnalyticTable","TableId")}},
                    { "AnalyticReport",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="AnalyticReport",Schema="Analytic",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "DataAnalysisDefinition",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="DataAnalysisDefinition",Schema="Analytic",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "DataAnalysisItemDefinition",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="DataAnalysisItemDefinition",Schema="Analytic",IdColumnName="ID",WhereCondition=JoinedWhereCondition,JoinCondition=GetJoinCondition("Analytic","DataAnalysisDefinition","DataAnalysisDefinitionID")}},
                    { "VRWorkflow",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="VRWorkflow",Schema="bp",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "BPDefinition",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="BPDefinition",Schema="bp",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "BPTaskType",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="BPTaskType",Schema="bp",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "ExtensionConfiguration",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="ExtensionConfiguration",Schema="common",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "VRObjectTypeDefinition",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="VRObjectTypeDefinition",Schema="common",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "MailMessageType",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="MailMessageType",Schema="common",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "StatusDefinition",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="StatusDefinition",Schema="common",IdColumnName="ID",WhereCondition=JoinedWhereCondition,JoinCondition=GetJoinCondition("genericdata","BusinessEntityDefinition","BusinessEntityDefinitionID")}},
                    { "VRComponentType",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="VRComponentType",Schema="common",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "VRNamespace",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="VRNamespace",Schema="common",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "VRNamespaceItem",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="VRNamespaceItem",Schema="common",IdColumnName="ID",WhereCondition=JoinedWhereCondition,JoinCondition=GetJoinCondition("common","VRNamespace","VRNamespaceId")}},
                    { "VRDynamicAPIModule",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="VRDynamicAPIModule",Schema="common",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "VRDynamicAPI",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="VRDynamicAPI",Schema="common",IdColumnName="ID",WhereCondition=JoinedWhereCondition,JoinCondition=GetJoinCondition("common","VRDynamicAPIModule","ModuleId")}},
                    { "ParserType",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="ParserType",Schema="dataparser",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "DataStore",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="DataStore",Schema="genericdata",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "DataRecordType",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="DataRecordType",Schema="genericdata",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "DataRecordStorage",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="DataRecordStorage",Schema="genericdata",IdColumnName="ID",WhereCondition=string.Format("rec.DevProjectID = '{0}'",DevProjectId),JoinCondition=GetJoinCondition("genericdata","DataRecordType","DataRecordTypeID")}},
                    { "DataTransformationDefinition",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="DataTransformationDefinition",Schema="genericdata",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "SummaryTransformationDefinition",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="SummaryTransformationDefinition",Schema="genericdata",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "GenericRuleDefinition",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="GenericRuleDefinition",Schema="genericdata",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "BusinessEntityDefinition",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="BusinessEntityDefinition",Schema="genericdata",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "DataRecordFieldChoice",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="DataRecordFieldChoice",Schema="genericdata",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "VRNumberPrefixType",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="VRNumberPrefixType",Schema="genericdata",IdColumnName="Id",WhereCondition=WhereCondition}},
                    { "VRNumberPrefix",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="VRNumberPrefix",Schema="genericdata",IdColumnName="Id",WhereCondition=JoinedWhereCondition,JoinCondition=GetJoinCondition("genericdata","VRNumberPrefixType","Type")}},
                    { "ExecutionFlowDefinition",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="ExecutionFlowDefinition",Schema="queue",IdColumnName="Id",WhereCondition=WhereCondition}},
                    { "ExecutionFlow",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="ExecutionFlow",Schema="queue",IdColumnName="Id",WhereCondition=JoinedWhereCondition,JoinCondition=GetJoinCondition("queue","ExecutionFlowDefinition","ExecutionFlowDefinitionID")}},
                    { "QueueActivatorConfig",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="QueueActivatorConfig",Schema="queue",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "ReprocessDefinition",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="ReprocessDefinition",Schema="reprocess",IdColumnName="Id",WhereCondition=WhereCondition}},
                    { "BusinessEntity",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="BusinessEntity",Schema="sec",IdColumnName="Id",WhereCondition=WhereCondition}},
                    { "BusinessEntityModule",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="BusinessEntityModule",Schema="sec",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "Module",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="Module",Schema="sec",IdColumnName="ID",WhereCondition=WhereCondition}},
                    { "View",new VRGeneratedScriptDevProjectTableParameters{ DevProjectId=devProjectId,TableName="View",Schema="sec",IdColumnName="ID",WhereCondition=WhereCondition}},
                };
            }
            
            public Guid DevProjectId;

            public string WhereCondition;

            public string JoinedWhereCondition;

            public string JoinCondition;
            public string GetJoinCondition(string schema,string tableName,string joinColumnName)
            {
                return string.Format("JOIN {0}.{1} rec on MainTable.{2} = rec.ID", schema,tableName,joinColumnName);
            }
            public Dictionary<string, VRGeneratedScriptDevProjectTableParameters> Tables { get; set; }
        }
        #region Public Methods

        public IEnumerable<VRDevProjectInfo> GetVRDevProjectsInfo(Guid connectionId)
        {
            IVRGeneratedScriptDevProjectTemplateDataManager templateDataManager = VRDevToolsFactory.GetDataManager<IVRGeneratedScriptDevProjectTemplateDataManager>();
            SQLConnection settings = new VRConnectionManager().GetVRConnection(connectionId).Settings as SQLConnection;
            if (settings != null)
            {
                if (settings.ConnectionString != null)
                    templateDataManager.Connection_String = settings.ConnectionString;
                else if (settings.ConnectionStringAppSettingName != null)
                    templateDataManager.Connection_String = settings.ConnectionStringAppSettingName;
                else
                    templateDataManager.Connection_String = settings.ConnectionStringName;
            }
            List<VRDevProject> allProjects = templateDataManager.GetDevProjects();

            Func<VRDevProject, bool> filterFunc = (project) =>
            {
                return true;
            };
            return allProjects.MapRecords(VRDevProjectInfoMapper, filterFunc);
        }
        public List<VRGeneratedScriptTable> GetDevProjectTableNames()
        {
            return TableNames;
        }
        public List<GeneratedScriptItemTable> GetDevProjectTemplates(VRGeneratedScriptDevProjectTemplatesInput input)
        {
            var tables = new VRGeneratedScriptDevProjectTables(input.DevProjectId);
            List<GeneratedScriptItemTable> items = null;
            if (tables != null && tables.Tables != null && tables.Tables.Count > 0)
            {
                items = new List<GeneratedScriptItemTable>();
                foreach (var table in tables.Tables)
                {
                    if (input.TableNames.Contains(table.Key) || table.Key== "VRDevProject")
                    {
                        VRGeneratedScriptTableDataManager generatedScriptTableDataManager = new VRGeneratedScriptTableDataManager();
                        var dataRows = generatedScriptTableDataManager.GetFilteredTableData(new VRGeneratedScriptTableDataQuery()
                        {
                            ConnectionId = input.ConnectionId,
                            SchemaName = table.Value.Schema,
                            TableName = table.Value.TableName,
                            WhereCondition = table.Value.WhereCondition,
                            JoinStatement = table.Value.JoinCondition
                        });

                        if (dataRows != null && dataRows.Count() > 0)
                        {
                            var item = new GeneratedScriptItemTable()
                            {
                                ConnectionId = input.ConnectionId,
                                TableName = table.Value.TableName,
                                Schema = table.Value.Schema,

                            };
                            var mergeGeneratedScriptItem = new MergeGeneratedScriptItem()
                            {
                                LastWhereCondition = table.Value.WhereCondition,
                                LastJoinStatement = table.Value.JoinCondition,
                            };
                            mergeGeneratedScriptItem.DataRows = dataRows.MapRecords(x => new GeneratedScriptItemTableRow() { FieldValues = x.FieldValues }).ToList();

                            mergeGeneratedScriptItem.Columns = new List<MergeGeneratedScriptItemColumn>();
                            VRGeneratedScriptColumnsManager generatedScriptColumnsManager = new VRGeneratedScriptColumnsManager();
                            var columnsInfo = generatedScriptColumnsManager.GetColumnsInfo(new VRGeneratedScriptColumnsInfoFilter() { ConnectionId = input.ConnectionId, SchemaName = table.Value.Schema, TableName = table.Value.TableName });
                            if (columnsInfo != null && columnsInfo.Count() > 0)
                                mergeGeneratedScriptItem.Columns.AddRange(columnsInfo.MapRecords(x => new MergeGeneratedScriptItemColumn() { ColumnName = x.Name, IncludeInInsert = true, IncludeInUpdate = true, IsIdentifier = x.Name == table.Value.IdColumnName ? true : false }));

                            item.Settings = mergeGeneratedScriptItem;
                            items.Add(item);
                        }
                    }
                }
            }
            return items;
        }
        #endregion

        #region Mappers
        public VRDevProjectInfo VRDevProjectInfoMapper(VRDevProject vrDevProject)
        {
            return new VRDevProjectInfo()
            {
                VRDevProjectID = vrDevProject.VRDevProjectID,
                Name = vrDevProject.Name
            };
        }
        #endregion

    }
}
