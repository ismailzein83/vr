using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Data;
using Vanrise.RDBTests.Common;
using Vanrise.GenericData.Entities;
namespace Vanrise.RDB.Tests.GenericData
{
    [TestClass]
    public class StagingSummaryRecordDataManagerTests
    {
        const string DBTABLE_NAME_StagingSummaryRecord = "StagingSummaryRecord";

        IStagingSummaryRecordDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
        IStagingSummaryRecordDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
        [TestMethod]
        public void AddSelectStagingSummaryRecords()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_TRANSACTION, Constants.DBSCHEMA_NAME_Reporocess, DBTABLE_NAME_StagingSummaryRecord);
            var stagingSummaryRecord = new Vanrise.GenericData.Entities.StagingSummaryRecord
            {
                StageName = "test",
                BatchStart = DateTime.Now,
                AlreadyFinalised = false,
                BatchEnd = DateTime.Now,
                Data = new byte[100],
                Payload = "test",
                ProcessInstanceId = 10
            };

            var rdbStreamObject = _rdbDataManager.InitialiazeStreamForDBApply();
            _rdbDataManager.WriteRecordToStream(stagingSummaryRecord, rdbStreamObject);
            var rdbStream =_rdbDataManager.FinishDBApplyStream(rdbStreamObject);
            _rdbDataManager.ApplyStreamToDB(rdbStream);

            var sqlStreamObject = _sqlDataManager.InitialiazeStreamForDBApply();
            _sqlDataManager.WriteRecordToStream(stagingSummaryRecord, sqlStreamObject);
            var sqlStream = _sqlDataManager.FinishDBApplyStream(sqlStreamObject);
            _sqlDataManager.ApplyStreamToDB(sqlStream);


            Vanrise.GenericData.Entities.StagingSummaryRecord rdbResponse = null;
            _rdbDataManager.GetStagingSummaryRecords(stagingSummaryRecord.ProcessInstanceId, stagingSummaryRecord.StageName, stagingSummaryRecord.BatchStart, stagingSummaryRecord.BatchEnd, (summaryRecord) =>
            {
                rdbResponse = summaryRecord;
            });
            Vanrise.GenericData.Entities.StagingSummaryRecord sqlResponse = null;
            _sqlDataManager.GetStagingSummaryRecords(stagingSummaryRecord.ProcessInstanceId, stagingSummaryRecord.StageName, stagingSummaryRecord.BatchStart, stagingSummaryRecord.BatchEnd, (summaryRecord) =>
            {
                sqlResponse = summaryRecord;
            });
            UTUtilities.AssertObjectsAreSimilar(rdbResponse, sqlResponse);


            List<StagingSummaryInfo> rdbInfoResponse =  _rdbDataManager.GetStagingSummaryInfo(stagingSummaryRecord.ProcessInstanceId, stagingSummaryRecord.StageName);
            List<StagingSummaryInfo> sqlInfoResponse = _sqlDataManager.GetStagingSummaryInfo(stagingSummaryRecord.ProcessInstanceId, stagingSummaryRecord.StageName);
            UTUtilities.AssertObjectsAreSimilar(rdbInfoResponse, sqlInfoResponse);
            _rdbDataManager.DeleteStagingSummaryRecords(stagingSummaryRecord.ProcessInstanceId, stagingSummaryRecord.StageName, stagingSummaryRecord.BatchStart, stagingSummaryRecord.BatchEnd);
            _sqlDataManager.DeleteStagingSummaryRecords(stagingSummaryRecord.ProcessInstanceId, stagingSummaryRecord.StageName, stagingSummaryRecord.BatchStart, stagingSummaryRecord.BatchEnd);
        }


    }
}
