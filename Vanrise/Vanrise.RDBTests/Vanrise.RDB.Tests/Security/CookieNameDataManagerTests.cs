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
    public class CookieNameDataManagerTests
    {
        const string DBTABLE_NAME_CookieName = "CookieName";

        ICookieNameDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<ICookieNameDataManager>();
        ICookieNameDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<ICookieNameDataManager>();
        [TestMethod]
        public void AddUpdateSelectCookieName()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Security, DBTABLE_NAME_CookieName);
            string cookieName = "TestRDB";
            var rdbResponse = _rdbDataManager.InsertIfNotExistsAndGetCookieName(cookieName);
            var sqlResponse = _sqlDataManager.InsertIfNotExistsAndGetCookieName(cookieName);
            UTUtilities.AssertObjectsAreSimilar(rdbResponse, sqlResponse);
            var rdbResponse1 = _rdbDataManager.InsertIfNotExistsAndGetCookieName(cookieName);
            var sqlResponse1 = _sqlDataManager.InsertIfNotExistsAndGetCookieName(cookieName);
            UTUtilities.AssertObjectsAreSimilar(rdbResponse, sqlResponse);
        }
    }
}
