//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.Common.Data;
//using Vanrise.Entities;
//using Vanrise.RDBTests.Common;

//namespace Vanrise.RDB.Tests.Common
//{
//    [TestClass]
//    public class CurrencyDataManagerTests
//    {
//        const string DBTABLE_NAME_Currency = "Currency";
//        ICurrencyDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<ICurrencyDataManager>();
//        ICurrencyDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<ICurrencyDataManager>();
//        [TestMethod]
//        public void AddUpdateSelectCurrencies()
//        {
//           UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Common, DBTABLE_NAME_Currency);
//            Currency Currency = new Currency
//            {
//                Name = "RDB TEST",
//                CreatedBy = 1,
//                LastModifiedBy = 1,
//                Symbol = "RDB"
//            };
//            var tester = new CurrencyMainOperationTester(Currency, _rdbDataManager, _sqlDataManager);
//            tester.ExecuteTest(true, true, false);
//        }

//        #region Private Classes

//        private class CurrencyMainOperationTester : EntityMainOperationTester<Currency, ICurrencyDataManager>
//        {
//            public CurrencyMainOperationTester(Currency Currency, ICurrencyDataManager rdbDataManager, ICurrencyDataManager sqlDataManager)
//                : base(Currency, rdbDataManager, sqlDataManager)
//            {

//            }

//            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<Currency> context)
//            {
//                Currency entity = context.Entity;
//                entity.Name = entity.Name + " Updated ";
//            }
//            public override IEnumerable<Currency> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<ICurrencyDataManager> context)
//            {
//                var currencies = context.DataManager.GetCurrencies();
//                if (currencies == null)
//                    return null;
//                foreach (var currency in currencies)
//                {
//                    if (currency.CreatedTime.HasValue)
//                        currency.CreatedTime = currency.CreatedTime.Value.Date;
//                    if (currency.LastModifiedTime.HasValue)
//                        currency.LastModifiedTime = currency.LastModifiedTime.Value.Date;
//                }
//                return currencies;
//            }

//            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<Currency, ICurrencyDataManager> context)
//            {
//                return context.DataManager.Insert(context.Entity, out int insertedid);
//            }

//            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<Currency, ICurrencyDataManager> context)
//            {
//                return context.DataManager.Update(context.Entity);
//            }

//            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

//            public override string DBSchemaName => Constants.DBSCHEMA_NAME_Common;

//            public override string DBTableName => DBTABLE_NAME_Currency;

//            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<Currency> context)
//            {
//                context.Entity.Name = Guid.NewGuid().ToString();
//            }

//            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<Currency> context)
//            {
//                context.TargetEntity.Name = context.SourceEntity.Name;
//            }

//            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<Currency, ICurrencyDataManager> context)
//            {
//                throw new NotImplementedException();
//            }

//            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<Currency> context)
//            {
//            }
//        }

//        #endregion

//    }
//}
