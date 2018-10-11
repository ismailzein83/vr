using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vanrise.AccountBalance.Data;
using Vanrise.Common;
using Vanrise.RDBTests.Common;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.RDB.Tests.AccountBalance
{
    [TestClass]
    public class BillingTransactionTypeDataManagerTests
    {
        internal const string DBTABLE_NAME_BILLINGTRANSACTION = "BillingTransactionType";

        IBillingTransactionTypeDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IBillingTransactionTypeDataManager>();
        IBillingTransactionTypeDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IBillingTransactionTypeDataManager>();

        [TestMethod]
        public void GetBillingTransactionTypes()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, DBTABLE_NAME_BILLINGTRANSACTION);

            bool isCredit = true;
            for(int i=0;i<10;i++)
            {
                string query = string.Format("INSERT INTO [VR_AccountBalance].[BillingTransactionType] (ID, [Name], [IsCredit], [Settings]) Values ('{0}', '{1}', {2}, '{3}')", Guid.NewGuid(), Guid.NewGuid(), isCredit ? "1" : "0", Serializer.Serialize(new BillingTransactionTypeSettings { ManualAdditionDisabled = !isCredit }));
                isCredit = !isCredit;
                UTUtilities.ExecuteDBNonQuery(Constants.CONNSTRING_NAME_CONFIG, query);
            }

            var rdbResponse = _rdbDataManager.GetBillingTransactionTypes();
            var sqlResponse = _sqlDataManager.GetBillingTransactionTypes();

            UTUtilities.AssertObjectsAreSimilar(sqlResponse, rdbResponse);
        }
    }
}
