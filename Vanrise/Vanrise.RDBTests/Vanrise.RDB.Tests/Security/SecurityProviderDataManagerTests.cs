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
    public class SecurityProviderDataManagerTests
    {
        const string DBTABLE_NAME_SecurityProvider = "SecurityProvider";

        ISecurityProviderDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<ISecurityProviderDataManager>();
        ISecurityProviderDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<ISecurityProviderDataManager>();
        [TestMethod]
        public void GetAndSetDefaultSecurityProvider()
        {
            var securityProviderId = Guid.NewGuid();
            var rdbResponse1 = _rdbDataManager.SetDefaultSecurityProvider(securityProviderId);
            var sqlResponse1 = _sqlDataManager.SetDefaultSecurityProvider(securityProviderId);
            UTUtilities.AssertValuesAreEqual(rdbResponse1, sqlResponse1);
            var rdbResponse = _rdbDataManager.GetDefaultSecurityProvider();
            var sqlResponse = _sqlDataManager.GetDefaultSecurityProvider();
            UTUtilities.AssertObjectsAreSimilar(rdbResponse, sqlResponse);
        }
    }
}
