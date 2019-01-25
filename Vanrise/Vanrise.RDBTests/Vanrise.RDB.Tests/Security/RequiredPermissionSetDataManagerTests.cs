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
    public class RequiredPermissionSetDataManagerTests
    {
        const string DBTABLE_NAME_RequiredPermission = "RequiredPermissionSet";

        IRequiredPermissionSetDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IRequiredPermissionSetDataManager>();
        IRequiredPermissionSetDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IRequiredPermissionSetDataManager>();
        [TestMethod]
        public void GetAndAddRequiredPermissionSet()
        {
            var RequiredPermissionId = Guid.NewGuid();
            var rdbResponse1 = _rdbDataManager.AddIfNotExists("BusinessEntity", "VR_Sec/User/GetUser");
            var sqlResponse1 = _sqlDataManager.AddIfNotExists("BusinessEntity", "VR_Sec/User/GetUser");
            UTUtilities.AssertValuesAreEqual(rdbResponse1, sqlResponse1);
            var rdbResponse = _rdbDataManager.GetAll();
            var sqlResponse = _sqlDataManager.GetAll();
            UTUtilities.AssertObjectsAreSimilar(rdbResponse, sqlResponse);
        }
    }
}
