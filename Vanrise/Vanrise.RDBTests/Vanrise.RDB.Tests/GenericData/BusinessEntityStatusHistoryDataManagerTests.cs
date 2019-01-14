using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Data;
using Vanrise.RDBTests.Common;

namespace Vanrise.RDB.Tests.GenericData
{
    [TestClass]
    public class BusinessEntityStatusHistoryDataManagerTests
    {
        const string DBTABLE_NAME_BusinessEntityStatusHistory = "BusinessEntityStatusHistory";

        IBusinessEntityStatusHistoryDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IBusinessEntityStatusHistoryDataManager>();
        IBusinessEntityStatusHistoryDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IBusinessEntityStatusHistoryDataManager>();
        [TestMethod]
        public void AddSelectBusinessEntityStatusHistory()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_GenericData, DBTABLE_NAME_BusinessEntityStatusHistory);
            Guid beDefinition = Guid.NewGuid();
            string beEntityId = "GenericData";
            string fileName = "Test RDB File";
            Guid statusId = Guid.NewGuid();
            Guid prestatusId = Guid.NewGuid();

            var rdbInsertResponse = _rdbDataManager.Insert(beDefinition, beEntityId, fileName, statusId, prestatusId);
            var sqlInsertResponse = _sqlDataManager.Insert(beDefinition, beEntityId, fileName, statusId, prestatusId);
            UTUtilities.AssertValuesAreEqual(rdbInsertResponse, sqlInsertResponse);

            var rdbResponse = _rdbDataManager.GetLastBusinessEntityStatusHistory(beDefinition, beEntityId, fileName);

            rdbResponse.StatusChangedDate = rdbResponse.StatusChangedDate.Date;
            var sqlResponse = _sqlDataManager.GetLastBusinessEntityStatusHistory(beDefinition, beEntityId, fileName);
            sqlResponse.StatusChangedDate = sqlResponse.StatusChangedDate.Date;

            UTUtilities.AssertObjectsAreSimilar(rdbResponse, sqlResponse);

        }


    }
}
