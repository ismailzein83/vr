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
    public class CityDataManagerTests
    {
        const string DBTABLE_NAME_City = "City";

        ICityDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<ICityDataManager>();
        ICityDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<ICityDataManager>();

        [TestMethod]
        public void AddUpdateSelectCities()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_City);
            City city = new City
            {
                Name = "RDB TEST",
                CountryId = 1,
                Settings = new CitySettings
                {
                    Abbreviation ="RDB",
                },
                CreatedBy = 1,
                LastModifiedBy = 1
            };
            var tester = new CityMainOperationTester(city, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class CityMainOperationTester : EntityMainOperationTester<City, ICityDataManager>
        {
            public CityMainOperationTester(City city, ICityDataManager rdbDataManager, ICityDataManager sqlDataManager)
                : base(city, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<City> context)
            {
                City entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
                entity.CountryId = 1;
                entity.Settings.Abbreviation = "test";
            }
            public override IEnumerable<City> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<ICityDataManager> context)
            {
                var cities = context.DataManager.GetCities();
                if (cities == null)
                    return null;
                foreach(var city in cities)
                {
                    city.CreatedTime = city.CreatedTime.Date;
                    if (city.LastModifiedTime.HasValue)
                      city.LastModifiedTime = city.LastModifiedTime.Value.Date;
                }
                return cities;
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<City, ICityDataManager> context)
            {
                return context.DataManager.Insert(context.Entity, out int cityId);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<City, ICityDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_City;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<City> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<City> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<City, ICityDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<City> context)
            {
            }
        }

        #endregion
    }
}
