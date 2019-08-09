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
        static Dictionary<string, TableParameters> DevProjectTables = new Dictionary<string, TableParameters>() {
            { "DataStore",new TableParameters{ DevProjectId=DevProjectId,TableName="DataStore",Schema="genericdata",IdColumnName="ID",WhereCondition=WhereCondition}},
            { "DataRecordType",new TableParameters{ DevProjectId=DevProjectId,TableName="DataRecordType",Schema="genericdata",IdColumnName="ID",WhereCondition=WhereCondition}},
            { "DataRecordStorage",new TableParameters{ DevProjectId=DevProjectId,TableName="DataRecordStorage",Schema="genericdata",IdColumnName="ID",WhereCondition=string.Format("rec.DevProjectID = '{0}'",DevProjectId),JoinCondition="JOIN genericdata.DataRecordType rec on MainTable.DataRecordTypeID = rec.ID"}},
            { "BusinessEntityDefinition",new TableParameters{ DevProjectId=DevProjectId,TableName="BusinessEntityDefinition",Schema="genericdata",IdColumnName="ID",WhereCondition=WhereCondition}},
            { "VRWorkflow",new TableParameters{ DevProjectId=DevProjectId,TableName="VRWorkflow",Schema="bp",IdColumnName="ID",WhereCondition=WhereCondition}},
            { "BPDefinition",new TableParameters{ DevProjectId=DevProjectId,TableName="BPDefinition",Schema="bp",IdColumnName="ID",WhereCondition=WhereCondition}},
            { "BPTaskType",new TableParameters{ DevProjectId=DevProjectId,TableName="BPTaskType",Schema="bp",IdColumnName="ID",WhereCondition=WhereCondition}},
            { "View",new TableParameters{ DevProjectId=DevProjectId,TableName="View",Schema="sec",IdColumnName="ID",WhereCondition=WhereCondition}},
            { "Connection",new TableParameters{ DevProjectId=DevProjectId,TableName="Connection",Schema="common",IdColumnName="ID",WhereCondition=WhereCondition}},
        };
        static Guid DevProjectId;
        static string WhereCondition= string.Format("DevProjectID = '{0}'", DevProjectId);
        public class TableParameters
        {
            public Guid DevProjectId { get; set; }
            public string TableName { get; set; }
            public string Schema { get; set; }
            public string IdColumnName { get; set; }
            public string WhereCondition { get; set; }
            public string JoinCondition { get; set; }
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
        public List<GeneratedScriptItemTable> GetDevProjectTemplates(Guid connectionId)
        {
            DevProjectId = connectionId;
            List<GeneratedScriptItemTable> items = new List<GeneratedScriptItemTable>();
            if(DevProjectTables!=null && DevProjectTables.Count > 0)
            {
                foreach(var table in DevProjectTables)
                {

                    var item = new GeneratedScriptItemTable()
                    {

                        ConnectionId = connectionId,
                        TableName = table.Value.TableName,
                        Schema = table.Value.Schema,
                    };
                }


            }
            return null;
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
