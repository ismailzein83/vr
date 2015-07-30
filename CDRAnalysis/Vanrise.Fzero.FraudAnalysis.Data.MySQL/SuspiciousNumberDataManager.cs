//using MySql.Data.MySqlClient;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Vanrise.Data.MySQL;
//using Vanrise.Fzero.FraudAnalysis.Entities;

//namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
//{
//    public class SuspiciousNumberDataManager : BaseMySQLDataManager, ISuspiciousNumberDataManager
//    {
//        public SuspiciousNumberDataManager()
//            : base("CDRDBConnectionStringMySQL")
//        {

//        }



//        public void SaveSuspiciousNumbers(List<SuspiciousNumber> suspiciousNumbers)
//        {
//            throw new NotImplementedException();
//        }

//        public void SaveNumberProfiles(List<NumberProfile> numberProfiles)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerable<FraudResult> GetFilteredSuspiciousNumbers(string tempTableKey, int fromRow, int toRow, DateTime fromDate, DateTime toDate, List<int> strategiesList, List<int> suspicionLevelsList, List<int> caseStatusesList)
//        {
//            throw new NotImplementedException();
//        }

//        public FraudResult GetFraudResult(DateTime fromDate, DateTime toDate, List<int> strategiesList, List<int> suspicionLevelsList, string subscriberNumber)
//        {
//            throw new NotImplementedException();
//        }

//        public void UpdateSusbcriberCases(List<string> suspiciousNumbers)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
