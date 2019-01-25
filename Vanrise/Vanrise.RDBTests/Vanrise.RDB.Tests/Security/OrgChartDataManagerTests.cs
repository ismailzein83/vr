using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.RDBTests.Common;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;
namespace Vanrise.RDB.Tests.Security
{
    [TestClass]
    public class OrgChartDataManagerTests
    {
        const string DBTABLE_NAME_OrgChart = "OrgChart";

        IOrgChartDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IOrgChartDataManager>();
        IOrgChartDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IOrgChartDataManager>();
        [TestMethod]
        public void AddUpdateSelectOrgCharts()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Security, DBTABLE_NAME_OrgChart);
            OrgChart OrgChart = new OrgChart
            {
                Hierarchy = new List<Member>(),
                Name = "RDB TEST",

            };
            var tester = new OrgChartMainOperationTester(OrgChart, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, true);

        }

        #region Private Classes

        private class OrgChartMainOperationTester : EntityMainOperationTester<OrgChart, IOrgChartDataManager>
        {
            public OrgChartMainOperationTester(OrgChart OrgChart, IOrgChartDataManager rdbDataManager, IOrgChartDataManager sqlDataManager)
                : base(OrgChart, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<OrgChart> context)
            {
                OrgChart entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<OrgChart> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IOrgChartDataManager> context)
            {
                return context.DataManager.GetOrgCharts();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<OrgChart, IOrgChartDataManager> context)
            {
                return context.DataManager.AddOrgChart(context.Entity, out int insertedid);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<OrgChart, IOrgChartDataManager> context)
            {
                return context.DataManager.UpdateOrgChart(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Security;

            public override string DBTableName => DBTABLE_NAME_OrgChart;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<OrgChart> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<OrgChart> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<OrgChart, IOrgChartDataManager> context)
            {
                context.DataManager.DeleteOrgChart(context.Entity.OrgChartId);
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<OrgChart> context)
            {
            }
        }

        #endregion
    }
}
