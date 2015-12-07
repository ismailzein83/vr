using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.Sales.Entities;

namespace TOne.Sales.Data.SQL
{
    public class StateBackupDataManager : BaseTOneDataManager, IStateBackupDataManager
    {
        public StateBackup Create(StateBackupType backupType, string carrierAccountId)
        {
            StateBackup stateBackup = new StateBackup(backupType, carrierAccountId);

            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream gz = new GZipStream(stream, CompressionMode.Compress, true))
                {
                    using (StreamWriter writer = new StreamWriter(gz))
                    {
                        string pricelistCondition, zoneCondition, rateCondition, codeCondition, commissionCondition;

                        GetSqlConditions(stateBackup, true, out pricelistCondition, out zoneCondition, out rateCondition, out codeCondition, out commissionCondition);

                        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MainDBConnString"].ConnectionString))
                        {
                            SqlCommand command = connection.CreateCommand();
                            SqlDataAdapter adp = new SqlDataAdapter();
                            DataTable Data = new DataTable();
                            writer.WriteLine(string.Format("[BackupType={0}{1}]", backupType, carrierAccountId == null ? "" : ", ID:" + carrierAccountId));

                            command.CommandText = "SELECT E.* FROM PriceList as E WITH (NOLOCK) " + pricelistCondition;
                            command.CommandTimeout = 0;
                            adp.SelectCommand = command;
                            adp.Fill(Data);
                            ReadAndCompress(Data, "PriceList", writer);
                            //using (SqlDataReader dr = command.ExecuteReader())
                            //    ReadAndCompress(dr, "PriceList", writer);

                            if (backupType == StateBackupType.Full || (backupType == StateBackupType.Supplier))
                            //if (backupType == StateBackupType.Full || (backupType == StateBackupType.Customer && StateBackupType.Customer.ToString().Contains("SYS")) == true)
                            {
                                command.CommandText = "SELECT E.* FROM Zone as E WITH (NOLOCK) " + zoneCondition;
                                command.CommandTimeout = 0;
                                adp.SelectCommand = command;
                                Data.Dispose();
                                GC.Collect();
                                Data = new DataTable();
                                adp.Fill(Data);
                                ReadAndCompress(Data, "Zone", writer);
                                // using (SqlDataReader dr = command.ExecuteReader())
                                //  ReadAndCompress(dr, "Zone", writer);

                                command.CommandText = "SELECT C.* FROM Code as C WITH (NOLOCK)" + codeCondition;
                                command.CommandTimeout = 0;
                                adp.SelectCommand = command;
                                Data.Dispose();
                                GC.Collect();
                                Data = new DataTable();
                                adp.Fill(Data);
                                ReadAndCompress(Data, "Code", writer);

                                if (backupType == StateBackupType.Full)
                                {
                                    command.CommandText = "SELECT * from CodeGroup WITH (NOLOCK)";
                                    command.CommandTimeout = 0;
                                    adp.SelectCommand = command;
                                    Data.Dispose();
                                    GC.Collect();
                                    Data = new DataTable();
                                    adp.Fill(Data);
                                    ReadAndCompress(Data, "CodeGroup", writer);
                                    // using (SqlDataReader dr = command.ExecuteReader())
                                    //   ReadAndCompress(dr, "CodeGroup", writer);

                                    command.CommandText = "SELECT * FROM Currency sp WITH (NOLOCK)";
                                    command.CommandTimeout = 0;
                                    adp.SelectCommand = command;
                                    Data.Dispose();
                                    GC.Collect();
                                    Data = new DataTable();
                                    adp.Fill(Data);
                                    ReadAndCompress(Data, "Currency", writer);

                                    command.CommandText = "SELECT * FROM CarrierAccount WITH (NOLOCK)";
                                    command.CommandTimeout = 0;
                                    adp.SelectCommand = command;
                                    Data.Dispose();
                                    GC.Collect();
                                    Data = new DataTable();
                                    adp.Fill(Data);
                                    ReadAndCompress(Data, "CarrierAccount", writer);
                                }

                            }
                            command.CommandText = "SELECT R.* FROM Rate as R WITH (NOLOCK)" + rateCondition;
                            command.CommandTimeout = 0;
                            adp.SelectCommand = command;
                            Data.Dispose();
                            GC.Collect();
                            Data = new DataTable();
                            adp.Fill(Data);
                            ReadAndCompress(Data, "Rate", writer);

                            gz.Flush();
                            writer.Close();
                        }
                        gz.Close();
                        stateBackup.StateData = stream.ToArray();
                    }
                }
            }
            return stateBackup;
        }

        private static void ReadAndCompress(DataTable data, string tableName, StreamWriter writer)
        {
            string[] removableColumns = new string[] { "iseffective", "timestamp" };
            string[] encodedTextColumns = new string[] { "description", "notes" };
            WriteData(data, writer, tableName, removableColumns, encodedTextColumns);
        }

        public static void WriteData(DataTable data, StreamWriter writer, string tableName, string[] removableColumns, string[] encodedTextColumns)
        {
            // Write table rows
            DataReaderTextReader reader = new DataReaderTextReader(data, TextDataFormat.TabSeparatedValues, removableColumns, encodedTextColumns);
            DataTable dtMeta = null;
            using (DataTableReader r = new DataTableReader(data))
            {
                dtMeta = r.GetSchemaTable();
            }

            //get the initial position
            MemoryStream ms = ((GZipStream)writer.BaseStream).BaseStream as MemoryStream;
            //long pos = ms.Position;
            // Write table name and header
            writer.WriteLine(string.Format("[{0}]: {1} Fields, {2} Rows", tableName, dtMeta.Rows.Count, 0));
            foreach (DataRow row in dtMeta.Rows)
                if (!removableColumns.Select(c => c.ToLower()).Contains(row["ColumnName"].ToString().ToLower()))
                {
                    Type colType = (Type)row["DataType"];
                    string colName = (string)row["ColumnName"];
                    string base64Suffix = (encodedTextColumns.Select(c => c.ToLower()).Contains(colName.ToLower()) || colType == typeof(byte[]) ? "_Base64" : string.Empty);
                    string colTypeConvertedName = (colType == typeof(byte[]) ? typeof(string).FullName : colType.FullName);
                    writer.Write(string.Format("{0}:{1}\t", colName + base64Suffix, colTypeConvertedName));
                }
            writer.WriteLine();

            string rowLine = null;
            int rowCount = 0;
            do
            {
                rowLine = null;
                if (rowCount < data.Rows.Count)
                {
                    rowLine = reader.ReadLine(data.Rows[rowCount]);
                    if (rowLine != null)
                    {
                        rowLine = rowLine.Replace('\n', ' ').Replace('\r', ' ');
                        writer.WriteLine(rowLine);
                    }
                    if (rowLine != null) rowCount++;
                }
            } while (rowLine != null);

            // Separate tables by new line
            writer.WriteLine();
            writer.Flush();
        }

        protected static void GetSqlConditions(StateBackup stateBackup, bool restoreZonesAndCodes
    , out string pricelistCondition
    , out string zoneCondition
    , out string rateCondition
    , out string codeCondition
    , out string commissionCondition)
        {
            StateBackupType backupType = stateBackup.StateBackupType;
            string carrierAccountId = (backupType == StateBackupType.Full)
                    ? null
                    : ((backupType == StateBackupType.Customer) ? stateBackup.CustomerId : stateBackup.SupplierId);

            // Not a Customer Backup (full and Supplier backup types) we MUST restore all codes and zones as well
            if (stateBackup.StateBackupType != StateBackupType.Customer) restoreZonesAndCodes = true;
            else
            {
                if (carrierAccountId == "SYS") restoreZonesAndCodes = true;
                else restoreZonesAndCodes = false;
            }
            if (restoreZonesAndCodes)
            {
                pricelistCondition = (backupType != StateBackupType.Full) ? string.Format(" WHERE {0}ID = '{1}' ", backupType, carrierAccountId) : "";
                zoneCondition = (backupType != StateBackupType.Full) ? string.Format(" WHERE SupplierID = '{0}' ", backupType == StateBackupType.Customer ? "SYS" : carrierAccountId) : "";
                rateCondition = (backupType != StateBackupType.Full) ? string.Format(" WHERE PriceListID IN (SELECT E.PriceListID FROM PriceList E WITH (NOLOCK) {0})", pricelistCondition) : "";
                codeCondition = (backupType != StateBackupType.Full) ? string.Format(" WHERE ZoneID IN (SELECT E.ZoneID FROM Zone E WITH (NOLOCK) {0})", zoneCondition) : "";
                commissionCondition = (backupType != StateBackupType.Full) ? string.Format(" WHERE ZoneID IN (SELECT E.ZoneID FROM Zone E WITH (NOLOCK) {0})", zoneCondition) : "";
            }
            else
            {
                zoneCondition = " WHERE 1=0 ";
                codeCondition = " WHERE 1=0 ";
                commissionCondition = " WHERE 1=0";
                pricelistCondition = string.Format(" WHERE CustomerID = '{0}' AND SupplierID = 'SYS' ", carrierAccountId);
                rateCondition = string.Format(" WHERE PriceListID IN (SELECT E.PriceListID FROM PriceList E WITH (NOLOCK) {0})", pricelistCondition);
            }
        }




        public bool Save(StateBackup stateBackup, out int stateBackupId)
        {
            object id;


            int recordesEffected = ExecuteNonQuerySP("Sales.[sp_SateBackup_Insert]", out id, stateBackup.StateBackupType, stateBackup.CustomerId, null, DateTime.Now, stateBackup.Notes, stateBackup.StateData, 0);

            stateBackupId = (recordesEffected > 0) ? (Int32)id : -1;
            return (recordesEffected > 0);
        }
    }
}
