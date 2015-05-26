using System.Collections.Generic;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Data.SqlClient;
using System.Data;
using System;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class PredefinedDataManager : BaseSQLDataManager, IPredefinedDataManager 
    {
        public PredefinedDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public List<CallClass> GetAllCallClasses()
        {
            string query_GetCallClasses = @"SELECT Id, Description, NetType FROM Set_CallClass; ";
            List<CallClass> callClasses = new List<CallClass>();


            ExecuteReaderText(query_GetCallClasses, (reader) =>
            {
                while (reader.Read())
                {
                    CallClass callClass = new CallClass();
                    callClass.Id = (int)reader["Id"];
                    callClass.Description = reader["Description"] as string;
                    callClass.NetType = (Enums.NetType)Enum.ToObject(typeof(Enums.NetType), GetReaderValue<int>(reader, "NetType"));
                    callClasses.Add(callClass);
                }
             }, null);
            return callClasses;
          }
      
    }
}
