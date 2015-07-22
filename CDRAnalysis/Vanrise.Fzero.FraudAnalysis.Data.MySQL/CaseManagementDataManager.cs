using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class CaseManagementDataManager : BaseMySQLDataManager, ICaseManagementDataManager
    {
        public CaseManagementDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }

        public bool SaveSubscriberCase(SubscriberCase subscriberCaseObject)
        {
            throw new NotImplementedException();
        }
    }
}
