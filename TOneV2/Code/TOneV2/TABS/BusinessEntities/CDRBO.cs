using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace TABS.BusinessEntities
{
    public class CDRBO
    {
        public static DataTable GetCDRFields()
        {
            return DataHelper.GetDataTable("SELECT TOP 1 * FROM CDR");  
        }

        //public static void GetImportedCDRFields(string tableName, IDbConnection connection, List<string> Names)
        //{
        //    using (connection)
        //    {
        //        connection.Open();
        //        IDbCommand command = connection.CreateCommand();
        //        command.CommandText = "SELECT TOP 1 * FROM [" + tableName + "]";
        //        IDataReader reader = command.ExecuteReader();
        //        int i = -1;
        //        while (++i < reader.FieldCount)
        //            Names.Add(reader.GetName(i));
        //        reader.Close();
        //    }
        //}

        public static IDbCommand GetInsertCommand(StringBuilder fields, StringBuilder parameters, List<string> paramList)
        {
            string insertCommand = string.Format("INSERT INTO [CDR] ({0}) VALUES ({1})", fields, parameters);
            IDbCommand command = new System.Data.SqlClient.SqlCommand();
            command.CommandText = insertCommand;
            foreach (string parameter in paramList)
                TABS.DataHelper.AddParameter(command, parameter, DBNull.Value);
            return command;
        }
    }
}
