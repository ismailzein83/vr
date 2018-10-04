using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vanrise.AccountBalance.Data;
using Vanrise.Common;
using Vanrise.RDBTests.Common;

namespace Vanrise.RDB.Tests.AccountBalance
{
    [TestClass]
    public class BillingTransactionTypeDataManagerTests
    {
        internal const string DBTABLE_NAME_BILLINGTRANSACTION = "BillingTransaction";

        IBillingTransactionTypeDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IBillingTransactionTypeDataManager>();
        IBillingTransactionTypeDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IBillingTransactionTypeDataManager>();

        [TestMethod]
        public void GetBillingTransactionTypes()
        {
            var rdbResponse = _rdbDataManager.GetBillingTransactionTypes();
            var sqlResponse = _sqlDataManager.GetBillingTransactionTypes();

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);
        }
    }
}
