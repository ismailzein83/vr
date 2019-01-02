using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.RDBTests.Common;
using Vanrise.Entities;
namespace Vanrise.RDB.Tests.Common
{
    [TestClass]
    public class CurrencyExchangeRateExchangeRateDataManagerTests
    {
        const string DBTABLE_NAME_CurrencyExchangeRate = "CurrencyExchangeRate";
        ICurrencyExchangeRateDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<ICurrencyExchangeRateDataManager>();
        ICurrencyExchangeRateDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<ICurrencyExchangeRateDataManager>();
        [TestMethod]
        public void AddUpdateSelectCurrencyExchangeRates()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_CurrencyExchangeRate);
            CurrencyExchangeRate CurrencyExchangeRate = new CurrencyExchangeRate
            {
                CurrencyId = 1,
                ExchangeDate = DateTime.Now,
                Rate = 1,
            };
            var tester = new CurrencyExchangeRateMainOperationTester(CurrencyExchangeRate, _rdbDataManager, _sqlDataManager);
            tester.ExecuteTest(true, true, false);
        }

        #region Private Classes

        private class CurrencyExchangeRateMainOperationTester : EntityMainOperationTester<CurrencyExchangeRate, ICurrencyExchangeRateDataManager>
        {
            public CurrencyExchangeRateMainOperationTester(CurrencyExchangeRate CurrencyExchangeRate, ICurrencyExchangeRateDataManager rdbDataManager, ICurrencyExchangeRateDataManager sqlDataManager)
                : base(CurrencyExchangeRate, rdbDataManager, sqlDataManager)
            {

            }

            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<CurrencyExchangeRate> context)
            {
                CurrencyExchangeRate entity = context.Entity;
                entity.ExchangeDate = DateTime.Now;
            }
            public override IEnumerable<CurrencyExchangeRate> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<ICurrencyExchangeRateDataManager> context)
            {
                return context.DataManager.GetCurrenciesExchangeRate();
            }

            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<CurrencyExchangeRate, ICurrencyExchangeRateDataManager> context)
            {
                return context.DataManager.Insert(context.Entity, out int insertedid);
            }

            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<CurrencyExchangeRate, ICurrencyExchangeRateDataManager> context)
            {
                return context.DataManager.Update(context.Entity);
            }

            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

            public override string DBTableName => DBTABLE_NAME_CurrencyExchangeRate;

            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<CurrencyExchangeRate> context)
            {
                context.Entity.ExchangeDate = DateTime.Now;
            }

            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<CurrencyExchangeRate> context)
            {
                context.TargetEntity.ExchangeDate = context.SourceEntity.ExchangeDate;
            }

            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<CurrencyExchangeRate, ICurrencyExchangeRateDataManager> context)
            {
                throw new NotImplementedException();
            }

            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<CurrencyExchangeRate> context)
            {
            }
        }

        #endregion
    }
}
