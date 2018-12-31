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
    public class CountryDataManagerTests
    {
        const string DBTABLE_NAME_Country = "Country";

        ICountrytDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<ICountrytDataManager>();
        ICountrytDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<ICountrytDataManager>();
        static int countryId = 1;
        [TestMethod]
        public void AddUpdateSelectCountries()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_Country);
            Country country = new Country
            {
                CountryId = countryId,
                Name = "RDB TEST",
                CreatedBy = 1,
                LastModifiedBy = 1
            };
            var tester = new CountryMainOperationTester(country, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class CountryMainOperationTester : EntityMainOperationTester<Country, ICountrytDataManager>
        {
            public CountryMainOperationTester(Country country, ICountrytDataManager rdbDataManager, ICountrytDataManager sqlDataManager)
                : base(country, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<Country> context)
            {
                Country entity = context.Entity;
                entity.Name = entity.Name + " Updated ";
            }
            public override IEnumerable<Country> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<ICountrytDataManager> context)
            {
                var countries = context.DataManager.GetCountries();
                if (countries == null)
                    return null;
                foreach (var country in countries)
                {
                    if (country.CreatedTime.HasValue)
                        country.CreatedTime = country.CreatedTime.Value.Date;
                    if (country.LastModifiedTime.HasValue)
                        country.LastModifiedTime = country.LastModifiedTime.Value.Date;
                }
                return countries;
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<Country, ICountrytDataManager> context)
            {
                return context.DataManager.Insert(context.Entity);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<Country, ICountrytDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_Country;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<Country> context)
            {
                context.Entity.Name = Guid.NewGuid().ToString();
                context.Entity.CountryId = countryId++;
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<Country> context)
            {
                context.TargetEntity.Name = context.SourceEntity.Name;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<Country, ICountrytDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<Country> context)
            {
                context.Entity.CountryId = countryId++;
            }
        }

        #endregion
    }
}
