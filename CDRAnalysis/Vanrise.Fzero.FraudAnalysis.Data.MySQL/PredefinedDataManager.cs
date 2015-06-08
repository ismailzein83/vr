using System;
using System.Collections.Generic;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class PredefinedDataManager : BaseMySQLDataManager, IPredefinedDataManager 
    {
        public PredefinedDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }


        public List<CallClass> GetAllCallClasses()
        {
            MySQLManager manager = new MySQLManager();
            string query_GetCallClasses = @"SELECT Id, Description, NetType FROM Set_CallClass; ";
            List<CallClass> callClasses = new List<CallClass>();


            manager.ExecuteReader(query_GetCallClasses,
                null, (reader) =>
                {

                    while (reader.Read())
                    {
                        CallClass callClass = new CallClass();
                        callClass.Id = (int)reader["Id"];
                        callClass.Description = reader["Description"] as string;
                        callClass.NetType = (Enums.NetType)Enum.ToObject(typeof(Enums.NetType), GetReaderValue<int>(reader, "NetType"));
                        callClasses.Add(callClass);
                    }



                });


            return callClasses;
        }






        public List<Period> GetPeriods()
        {
            throw new NotImplementedException();
        }
    }
}
