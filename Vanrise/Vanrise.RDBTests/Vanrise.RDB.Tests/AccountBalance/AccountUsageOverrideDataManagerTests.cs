using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vanrise.AccountBalance.Data;

namespace Vanrise.RDB.Tests.AccountBalance
{
    [TestClass]
    public class AccountUsageOverrideDataManagerTests
    {
        internal const string DBTABLE_NAME_ACCOUNTUSAGEOVERRIDE = "AccountUsageOverride";

        IAccountUsageOverrideDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IAccountUsageOverrideDataManager>();
        IAccountUsageOverrideDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IAccountUsageOverrideDataManager>();
        
    }
}
