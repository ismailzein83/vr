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
    public class EncryptionKeyDataManagerTests
    {
        const string DBTABLE_NAME_EncryptionKey = "EncryptionKey";

        IEncryptionKeyDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IEncryptionKeyDataManager>();
        IEncryptionKeyDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IEncryptionKeyDataManager>();
        [TestMethod]
        public void AddUpdateSelectEncryptionKey()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Security, DBTABLE_NAME_EncryptionKey);
            string EncryptionKey = "TestRDB";
            var rdbResponse = _rdbDataManager.InsertIfNotExistsAndGetEncryptionKey(EncryptionKey);
            var sqlResponse = _sqlDataManager.InsertIfNotExistsAndGetEncryptionKey(EncryptionKey);
            UTUtilities.AssertObjectsAreSimilar(rdbResponse, sqlResponse);
            var rdbResponse1 = _rdbDataManager.InsertIfNotExistsAndGetEncryptionKey(EncryptionKey);
            var sqlResponse1 = _sqlDataManager.InsertIfNotExistsAndGetEncryptionKey(EncryptionKey);
            UTUtilities.AssertObjectsAreSimilar(rdbResponse, sqlResponse);
        }
    }
}
