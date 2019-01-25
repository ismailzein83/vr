using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.RDBTests.Common;
using Vanrise.Security.Data;

namespace Vanrise.RDB.Tests.Security
{
    [TestClass]
    public class SystemActionDataManagerTests
    {
        const string DBTABLE_NAME_SystemAction = "SystemAction";

        ISystemActionDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<ISystemActionDataManager>();
        ISystemActionDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<ISystemActionDataManager>();
        [TestMethod]
        public void GetSystemActions()
        {
            var rdbResponse = _rdbDataManager.GetSystemActions();
            var sqlResponse = _sqlDataManager.GetSystemActions();
            UTUtilities.AssertObjectsAreSimilar(rdbResponse, sqlResponse);
        }
    }
}
