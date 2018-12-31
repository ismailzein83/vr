using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.RDBTests.Common;

namespace Vanrise.RDB.Tests.Common
{
    [TestClass]
    public class RegionDataManagerTests
    {
        const string DBTABLE_NAME_Region = "Region";

        IRegionDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IRegionDataManager>();
        IRegionDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IRegionDataManager>();

        [TestMethod]
        public void AddUpdateSelectRegions()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_Region);
            Region Region = new Region
            {
                Name = "RDB TEST",
                CountryId = 1,
                Settings = new RegionSettings
                {
                }
            };
            var tester = new RegionMainOperationTester(Region, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class RegionMainOperationTester : EntityMainOperationTester<Region, IRegionDataManager>
        {
            public RegionMainOperationTester(Region Region, IRegionDataManager rdbDataManager, IRegionDataManager sqlDataManager)
                : base(Region, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<Region> context)
            {
                Region entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
                entity.Settings = new RegionSettings();
            }
            public override IEnumerable<Region> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IRegionDataManager> context)
            {
                var regions =  context.DataManager.GetRegions();
                if (regions == null)
                    return null;
                foreach(var region in regions)
                {
                    if (region.LastModifiedTime.HasValue)
                        region.LastModifiedTime = region.LastModifiedTime.Value.Date;
                    region.CreatedTime = region.CreatedTime.Date;
                }
                return regions;
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<Region, IRegionDataManager> context)
            {
                return context.DataManager.Insert(context.Entity, out int id);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<Region, IRegionDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_Region;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<Region> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<Region> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<Region, IRegionDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<Region> context)
            {
            }
        }
        #endregion 
    }
}
