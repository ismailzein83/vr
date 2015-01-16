using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;

namespace TABS
{
    public class StateBackup : StateBackupInfo, IComparer<DataTable>
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(typeof(StateBackup));

        /// <summary>
        /// Create a State Backup from a previously saved file
        /// </summary>
        /// <param name="filename">The file name</param>
        /// <param name="fileContents">the contents of the file</param>
        /// <returns></returns>
        public static StateBackup Create(string filename, byte[] fileContents)
        {
            StateBackup stateBackup = new StateBackup();
            string[] parts = filename.Split('.');
            StateBackupType type = (StateBackupType)Enum.Parse(typeof(StateBackupType), parts[0]);
            CarrierAccount carrier = parts[1] == "ALL" ? null : CarrierAccount.All[parts[1]];
            WebHelperLibrary.DateTimeParser parser = new WebHelperLibrary.DateTimeParser("yyyyMMdd_HHmmss");
            DateTime? created = parser.Parse(parts[2]);
            if (type == StateBackupType.Customer) stateBackup.Customer = carrier;
            else if (type == StateBackupType.Supplier) stateBackup.Supplier = carrier;
            stateBackup.StateData = fileContents;
            if (created.HasValue) stateBackup.Created = created.Value;
            else stateBackup.Created = DateTime.Now;
            stateBackup.User = SecurityEssentials.Web.Helper.CurrentWebUser;
            return stateBackup;
        }

        public virtual byte[] StateData { get; set; }

        public StateBackup() {
            //string parsed = string.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second).Trim();
            //DateTime result = DateTime.ParseExact(parsed, "yyyy-MM-dd HH:mm:ss", null);
            this.Created = DateTime.Now;
            StateBackupType = StateBackupType.Full; 
        }

        public StateBackup(StateBackupType backupType, CarrierAccount carrierAccount)
            : this()
        {
            this.StateBackupType = backupType;
            switch (backupType)
            {
                case StateBackupType.Customer:
                    this.Customer = carrierAccount;
                    this.Supplier = null;
                    break;
                case StateBackupType.Supplier:
                    this.Customer = null;
                    this.Supplier = carrierAccount;
                    break;
            }
        }

        /// <summary>
        /// Get a suitable file name for download
        /// </summary>
        /// <returns></returns>
        public string GetDownloadFileName()
        {
            string carrier = this.StateBackupType == StateBackupType.Full ? "ALL" : (this.StateBackupType == StateBackupType.Customer ? this.Customer.CarrierAccountID : this.Supplier.CarrierAccountID);
            return string.Format(
                "{0}.{1}.{2:yyyyMMdd_HHmmss}.StateBackup.gz",
                this.StateBackupType.ToString(),
                carrier,
                this.Created
            );
        }

        /// <summary>
        /// Use the current HTTP Context to download this State Backup
        /// </summary>
        public void Download()
        {
            var context = System.Web.HttpContext.Current;
            var response = context.Response;
            response.ClearHeaders();
            response.ClearContent();
            response.Buffer = false;
            response.BufferOutput = false;
            response.AppendHeader("Content-Type", "application/binary");
            response.AppendHeader("Content-Disposition", "attachment; filename=\"" + GetDownloadFileName() + "\"");
            response.AppendHeader("Content-Length", this.StateData.Length.ToString());
            response.BinaryWrite(this.StateData);
            response.End();
        }


        protected static void GetSqlConditions(StateBackup stateBackup, bool restoreZonesAndCodes
            , out string pricelistCondition
            , out string zoneCondition
            , out string rateCondition
            , out string codeCondition
            , out string commissionCondition)
        {
            StateBackupType backupType = stateBackup.StateBackupType;
            CarrierAccount carrierAccount = (backupType == StateBackupType.Full)
                    ? null
                    : ((backupType == StateBackupType.Customer) ? stateBackup.Customer : stateBackup.Supplier);

            // Not a Customer Backup (full and Supplier backup types) we MUST restore all codes and zones as well
            if (stateBackup.StateBackupType != StateBackupType.Customer) restoreZonesAndCodes = true;

            if (restoreZonesAndCodes)
            {
                pricelistCondition = (backupType != StateBackupType.Full) ? string.Format(" WHERE {0}ID = '{1}' ", backupType, carrierAccount.CarrierAccountID) : "";
                zoneCondition = (backupType != StateBackupType.Full) ? string.Format(" WHERE SupplierID = '{0}' ", backupType == StateBackupType.Customer ? "SYS" : carrierAccount.CarrierAccountID) : "";
                rateCondition = (backupType != StateBackupType.Full) ? string.Format(" WHERE PriceListID IN (SELECT E.PriceListID FROM PriceList E WITH (NOLOCK) {0})", pricelistCondition) : "";
                codeCondition = (backupType != StateBackupType.Full) ? string.Format(" WHERE ZoneID IN (SELECT E.ZoneID FROM Zone E WITH (NOLOCK) {0})", zoneCondition) : "";
                commissionCondition = (backupType != StateBackupType.Full) ? string.Format(" WHERE ZoneID IN (SELECT E.ZoneID FROM Zone E WITH (NOLOCK) {0})", zoneCondition) : "";
            }
            else
            {
                zoneCondition = " WHERE 1=0 ";
                codeCondition = " WHERE 1=0 ";
                commissionCondition = " WHERE 1=0";
                pricelistCondition = string.Format(" WHERE CustomerID = '{0}' AND SupplierID = 'SYS' ", carrierAccount.CarrierAccountID);
                rateCondition = string.Format(" WHERE PriceListID IN (SELECT E.PriceListID FROM PriceList E WITH (NOLOCK) {0})", pricelistCondition);
            }
        }


        protected static void GetSqlDeletes(StateBackup stateBackup, bool restoreZonesAndCodes
            , out string pricelistDelete
            , out string zoneDelete
            , out string rateDelete
            , out string codeDelete
            , out string pricelistDataDelete)
        {
            bool typeFull = stateBackup.StateBackupType == StateBackupType.Full, typeCust = stateBackup.StateBackupType == StateBackupType.Customer;
            CarrierAccount carrierAccount = typeFull ? null : (typeCust ? stateBackup.Customer : stateBackup.Supplier);

            // Not a Customer Backup (full and Supplier backup types) we MUST restore all codes and zones as well
            if (!typeCust) restoreZonesAndCodes = true;

            if (restoreZonesAndCodes)
            {
                if (!typeFull)
                {
                    string pricelistCondition = string.Format("WHERE {0}ID = '{1}' ", stateBackup.StateBackupType, carrierAccount.CarrierAccountID);
                    string rateCondition = string.Format("WHERE PriceListID IN (SELECT E.PriceListID FROM PriceList E WITH (NOLOCK) {0})", pricelistCondition);
                    string zoneCondition = string.Format("WHERE SupplierID = '{0}' ", typeCust ? "SYS" : carrierAccount.CarrierAccountID);

                    pricelistDelete = string.Format("delete from Pricelist WITH (TABLOCKX) {0}", pricelistCondition);
                    zoneDelete = string.Format("delete from Zone WITH (TABLOCKX) {0}", zoneCondition);
                    rateDelete = string.Format("delete from Rate WITH (TABLOCKX) {0}", rateCondition);
                    codeDelete = string.Format("delete from Code WITH (TABLOCKX) WHERE ZoneID IN (SELECT E.ZoneID FROM Zone E WITH (NOLOCK) {0})", zoneCondition);
                    //commissionDelete = string.Format("delete from Commission WITH (TABLOCKX) WHERE ZoneID IN (SELECT E.ZoneID FROM Zone E {0})", zoneCondition);
                    pricelistDataDelete = string.Format("delete from PricelistData WITH (TABLOCKX) {0}", rateCondition);
                }
                else
                {
                    pricelistDelete = "truncate table Pricelist";
                    zoneDelete = "truncate table Zone";
                    rateDelete = "truncate table Rate";
                    codeDelete = "truncate table Code";
                    pricelistDataDelete = "truncate table PricelistData";
                }
            }
            else
            {
                string pricelistCondition = string.Format("WHERE CustomerID = '{0}' AND SupplierID = 'SYS' ", carrierAccount.CarrierAccountID);
                string rateCondition = string.Format("WHERE PriceListID IN (SELECT E.PriceListID FROM PriceList E WITH (NOLOCK) {0})", pricelistCondition);
                zoneDelete = string.Empty;
                codeDelete = string.Empty;
                pricelistDelete = string.Format("delete from PriceList WITH (TABLOCKX) {0}", pricelistCondition);
                rateDelete = string.Format("delete from rate WITH (TABLOCKX) {0}", rateCondition);
                pricelistDataDelete = string.Format("delete from PricelistData WITH (TABLOCKX) {0}", rateCondition);
            }
        }

        public static StateBackup Create(StateBackupType backupType, CarrierAccount carrierAccount)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream gz = new GZipStream(stream, CompressionMode.Compress, true))
                {
                    using (StreamWriter writer = new StreamWriter(gz))
                    {
                        StateBackup stateBackup = new StateBackup(backupType, carrierAccount);

                        log.Info(stateBackup.ToString());

                        string pricelistCondition, zoneCondition, rateCondition, codeCondition, commissionCondition;

                        GetSqlConditions(stateBackup, true, out pricelistCondition, out zoneCondition, out rateCondition, out codeCondition, out commissionCondition);

                        using (SqlConnection connection = (SqlConnection)TABS.DataHelper.GetOpenConnection())
                        {
                            SqlCommand command = connection.CreateCommand();
                            SqlDataAdapter adp = new SqlDataAdapter();
                            DataTable Data = new DataTable();
                            writer.WriteLine(string.Format("[BackupType={0}{1}]", backupType, carrierAccount == null ? "" : ", ID:" + carrierAccount.CarrierAccountID));

                            command.CommandText = "SELECT E.* FROM PriceList as E WITH (NOLOCK) " + pricelistCondition;
                            command.CommandTimeout = 0;
                            adp.SelectCommand = command;
                            adp.Fill(Data);
                            ReadAndCompress(Data, "PriceList", writer);
                            //using (SqlDataReader dr = command.ExecuteReader())
                            //    ReadAndCompress(dr, "PriceList", writer);

                            if (backupType == StateBackupType.Full || (backupType == StateBackupType.Supplier && StateBackupType.Supplier.ToString().Contains("SYS"))==false)
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

                            //using (SqlDataReader dr = command.ExecuteReader())
                              //  ReadAndCompress(dr, "Code", writer);

                            //command.CommandText = "SELECT C.* FROM Commission as C WITH (NOLOCK)" + commissionCondition;
                            //command.CommandTimeout = 0;
                            //adp.SelectCommand = command;
                            //Data.Dispose();
                            //GC.Collect();
                            //Data = new DataTable();
                            //adp.Fill(Data);
                            //ReadAndCompress(Data, "Commission", writer);

                            command.CommandText = "SELECT R.* FROM Rate as R WITH (NOLOCK)" + rateCondition;
                            command.CommandTimeout = 0;
                            adp.SelectCommand = command;
                            Data.Dispose();
                            GC.Collect();
                            Data = new DataTable();
                            adp.Fill(Data);
                            ReadAndCompress(Data, "Rate", writer);


                            gz.Flush();

                            stateBackup.StateData = stream.ToArray();
                        }

                        return stateBackup;
                    }
                }
            }
        }
        public static StateBackup Create(StateBackupType backupType, CarrierAccount carrierAccount, SqlConnection connection)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream gz = new GZipStream(stream, CompressionMode.Compress, true))
                {
                    using (StreamWriter writer = new StreamWriter(gz))
                    {
                        StateBackup stateBackup = new StateBackup(backupType, carrierAccount);

                        log.Info(stateBackup.ToString());

                        string pricelistCondition, zoneCondition, rateCondition, codeCondition, commissionCondition;

                        GetSqlConditions(stateBackup, true, out pricelistCondition, out zoneCondition, out rateCondition, out codeCondition, out commissionCondition);

                        SqlCommand command = connection.CreateCommand();
                        SqlDataAdapter adp = new SqlDataAdapter();
                        DataTable Data = new DataTable();
                        writer.WriteLine(string.Format("[BackupType={0}{1}]", backupType, carrierAccount == null ? "" : ", ID:" + carrierAccount.CarrierAccountID));

                        command.CommandText = "SELECT E.* FROM PriceList as E WITH (NOLOCK) " + pricelistCondition;
                        command.CommandTimeout = 0;
                        adp.SelectCommand = command;
                        adp.Fill(Data);
                        ReadAndCompress(Data, "PriceList", writer);
                        

                        if (backupType != StateBackupType.Full && (backupType == StateBackupType.Supplier && StateBackupType.Supplier.ToString().Contains("SYS")) == false)
                        {
                         
                        }

                        command.CommandText = "SELECT E.* FROM Zone as E WITH (NOLOCK) " + zoneCondition;
                        command.CommandTimeout = 0;
                        adp.SelectCommand = command;
                        Data.Dispose();
                        GC.Collect();
                        Data = new DataTable();
                        adp.Fill(Data);
                        ReadAndCompress(Data, "Zone", writer);
                     
                        command.CommandText = "SELECT C.* FROM Code as C WITH (NOLOCK)" + codeCondition;
                        command.CommandTimeout = 0;
                        adp.SelectCommand = command;
                        Data.Dispose();
                        GC.Collect();
                        Data = new DataTable();
                        adp.Fill(Data);
                        ReadAndCompress(Data, "Code", writer);
                     

                        command.CommandText = "SELECT R.* FROM Rate as R WITH (NOLOCK)" + rateCondition;
                        command.CommandTimeout = 0;
                        adp.SelectCommand = command;
                        Data.Dispose();
                        GC.Collect();
                        Data = new DataTable();
                        adp.Fill(Data);
                        ReadAndCompress(Data, "Rate", writer);
                   

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
                        // using (SqlDataReader dr = command.ExecuteReader())
                        //   ReadAndCompress(dr, "Currency", writer);

                        command.CommandText = "SELECT * FROM CarrierAccount WITH (NOLOCK)";
                        command.CommandTimeout = 0;
                        adp.SelectCommand = command;
                        Data.Dispose();
                        GC.Collect();
                        Data = new DataTable();
                        adp.Fill(Data);
                        ReadAndCompress(Data, "CarrierAccount", writer);
                        // using (SqlDataReader dr = command.ExecuteReader())
                        //  ReadAndCompress(dr, "CarrierAccount", writer);


                        gz.Flush();

                        stateBackup.StateData = stream.ToArray();


                        return stateBackup;
                    }
                    
                }
            }
        }




        /// <summary>
        /// Read from the SqlDataReader and Compress to the compression stream.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="tableName"></param>
        private static void ReadAndCompress(SqlDataReader reader, string tableName, StreamWriter writer)
        {
            Components.TableBackupHelper.WriteData(reader, writer, tableName);
        }
        private static void ReadAndCompress(DataTable Data, string tableName, StreamWriter writer)
        {
            Components.TableBackupHelper.WriteData(Data, writer, tableName);
        }
        /// <summary>
        /// This is a fix for legacy data where the SourceFileBytes field was in the PriceList table
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static DataSet GetFixedData(DataSet data)
        {
            if (data.Tables.Contains("PriceList") && data.Tables["PriceList"].Columns.Contains("SourceFileBytes"))
            {
                DataTable priceLists = data.Tables["PriceList"];
                DataTable priceListData = priceLists.Clone();
                var idColumn = priceLists.Columns["PriceListID"];
                var bytesColumn = priceLists.Columns["SourceFileBytes"];

                if (!data.Tables.Contains("PriceListData"))
                {
                    int i = 0;
                    while (i < priceListData.Columns.Count)
                    {
                        var column = priceListData.Columns[i];
                        if (column.ColumnName != idColumn.ColumnName && priceListData.Columns[i].ColumnName != bytesColumn.ColumnName)
                            priceListData.Columns.RemoveAt(i);
                        else
                            i++;
                    }
                }
                else
                {
                    priceListData = data.Tables["PriceListData"];
                }

                // Copy Data
                foreach (DataRow row in priceLists.Rows)
                {
                    if (row[bytesColumn] != null && row[bytesColumn] != DBNull.Value)
                    {
                        var rowCopy = priceListData.NewRow();
                        rowCopy[idColumn.ColumnName] = row[idColumn];
                        rowCopy[bytesColumn.ColumnName] = row[bytesColumn];
                        priceListData.Rows.Add(rowCopy);
                    }
                }

                // Remove bytes column from PriceList
                priceLists.Columns.Remove(bytesColumn);
            }
            return data;
        }

        public static DataSet GetData(StateBackup stateBackup)
        {
            DataSet data = Components.TableBackupHelper.GetData(new string[] { "description", "notes" }, stateBackup.StateData);
            return GetFixedData(data);
        }

        protected static void RestoreFirstTime(StateBackup stateBackup, SecurityEssentials.User currentUser, bool restoreZonesAndCodes, out Exception exp)
        {
            exp = null;
            // in case of a first time restore we have to delete it from database



        }
        /// <summary>
        /// Restore the given State Backup and flag it restored by given user.
        /// If this is a customer backup and the restoreZonesAndCodes flag is set 
        /// this will affect Sale Zones and Codes and thus all pricelists and rates for all customers.
        /// </summary>
        /// <param name="stateBackup"></param>
        /// <param name="currentUser"></param>
        /// <param name="restoreZonesAndCodes"></param>
        /// <param name="exp"></param>
        public static void Restore(StateBackup stateBackup, SecurityEssentials.User currentUser, bool restoreZonesAndCodes, out Exception exp)
        {
            bool Fail = false;
            NHibernate.ITransaction transaction = null;
            try
            {
               
                //Restore(stateBackup, restoreZonesAndCodes);
                Restore(stateBackup, restoreZonesAndCodes, out transaction,Fail);
                StateBackupInfo stateBackupInfo = ObjectAssembler.Get<StateBackupInfo>(stateBackup.ID);
                stateBackupInfo.ResponsibleForRestoring = currentUser.Name;
                stateBackupInfo.RestoreDate = DateTime.Now;
                string withoutZonesAndCodes = " (* Restored without Zones and Codes *)";
                if (!restoreZonesAndCodes)
                {
                    if (!stateBackupInfo.Notes.EndsWith(withoutZonesAndCodes)) stateBackupInfo.Notes += withoutZonesAndCodes;
                }
                else
                {
                    if (stateBackupInfo.Notes.EndsWith(withoutZonesAndCodes)) stateBackupInfo.Notes = stateBackupInfo.Notes.Replace(withoutZonesAndCodes, "");
                }
                Exception ex;
                bool sucess=ObjectAssembler.SaveOrUpdate(stateBackupInfo,transaction,false, out ex);
                exp = ex;
                if (ex == null && sucess == true && Fail == false)
                { transaction.Commit(); ObjectAssembler.ClearAllCollections(); }
                else
                    transaction.Rollback();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                exp = e;
            }

        }

        protected static void Delete(IDbCommand command, string sql, string condition)
        {

        }

        /// <summary>
        /// Restore the state of the main entities in the database from a previous state backup 
        /// This may cause inconsistencies in related entities created after the state creation.
        /// </summary>
        /// <param name="stateBackup"></param>
        public static void Restore(StateBackup stateBackup, bool restoreZonesAndCodes)
        {
            //var count = TABS.StateBackup.List().Count(s => s.StateBackupCarriers.Equals(stateBackup.StateBackupCarriers));
            
            var count = TABS.StateBackup.List().Count(s => s.Supplier != null && s.Supplier.Equals(stateBackup.Supplier));
            var isCodePreparationRestore = (stateBackup.Supplier != null && stateBackup.Supplier.Equals(TABS.CarrierAccount.SYSTEM));
            //case restor for specific supplier
            if (count == 1 && !isCodePreparationRestore)
            {
                string customerID = stateBackup.Customer != null ? stateBackup.Customer.CarrierAccountID : null;
                string supplierID = stateBackup.Supplier != null ? stateBackup.Supplier.CarrierAccountID : null;

                string carrier = supplierID == null ? customerID : supplierID;

                string deleteRateQuery = string.Format("Delete From Rate WITH (TABLOCKX) where zoneID in (select z.zoneid from zone z with(nolock) where z.supplierid='{0}' )", carrier);
                string deletePricelistQuery = string.Format("Delete From Pricelist WITH (TABLOCKX) where {0} = '{1}' ", customerID == null ? "supplierID" : "customerID", carrier);
                string deleteCodeQuery = string.Format("Delete From Code WITH (TABLOCKX) where zoneID in (select z.zoneid from zone z with(nolock) where z.supplierid='{0}' )", carrier);
                string deleteZoneQuery = string.Format("Delete From Zone WITH (TABLOCKX) where supplierid='{0}' ", carrier);


                lock (typeof(StateBackup))
                {
                    // Clean Up
                    using (IDbConnection connection = DataHelper.GetOpenConnection())
                    {
                        using (IDbTransaction transaction = connection.BeginTransaction())
                        {

                            try
                            {
                                IDbCommand command = connection.CreateCommand();
                                command.CommandTimeout = 0;
                                command.Transaction = transaction;

                                command.CommandText = deleteRateQuery;
                                command.ExecuteNonQuery();
                                command.CommandText = deletePricelistQuery;
                                command.ExecuteNonQuery();
                                command.CommandText = deleteCodeQuery;
                                command.ExecuteNonQuery();
                                command.CommandText = deleteZoneQuery;
                                command.ExecuteNonQuery();

                                var cleanUpcommand = connection.CreateCommand();
                                cleanUpcommand.Transaction = transaction;
                                cleanUpcommand.CommandText = "EXEC bp_CleanUpAfterStateBackupRestore";
                                cleanUpcommand.ExecuteNonQuery();

                                transaction.Commit();

                                log.InfoFormat("Restored From {0}", stateBackup);


                                // After a restore we need a refresh
                                ObjectAssembler.ClearAllCollections();
                            }
                            catch (Exception ex1)
                            {
                                log.Error("Error Restoring State Backup", ex1);
                                throw ex1;
                            }
                        }
                    }
                }


                return;
            }
            //Restore for Sys Supplier/CodePreperation
            DataSet data = GetData(stateBackup);

            // Only Customer Backup type has option to restore zones and codes or not
            restoreZonesAndCodes = stateBackup.StateBackupType != StateBackupType.Customer ? true : restoreZonesAndCodes;

            lock (typeof(StateBackup))
            {
                // Clean Up
                using (IDbConnection connection = DataHelper.GetOpenConnection())
                {
                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {

                        try
                        {
                            IDbCommand command = connection.CreateCommand();
                            command.CommandTimeout = 0;
                            command.Transaction = transaction;

                            string pricelistCondition, zoneCondition, rateCondition, codeCondition, commissionCondition;
                            GetSqlConditions(stateBackup, restoreZonesAndCodes, out pricelistCondition, out zoneCondition, out rateCondition, out codeCondition, out commissionCondition);

                            List<DataTable> tablesToRestore = new List<DataTable>();
                            foreach (DataTable table in data.Tables)
                            {
                                if (table.TableName.StartsWith("CodeGroup") || table.TableName.StartsWith("CarrierAccount") || table.TableName.StartsWith("Currency"))
                                { }
                                else tablesToRestore.Add(table);

                            }
                            // Sort tables most dependent first
                            tablesToRestore.Sort(stateBackup);

                            foreach (var mode in new string[] { "delete", "write" })
                            {
                                // If writing, begin by least depedent
                                if (mode == "write") tablesToRestore.Reverse();

                                foreach (DataTable table in tablesToRestore)
                                {
                                    // If not restoring zones and codes
                                    if (!restoreZonesAndCodes)
                                    {
                                        switch (table.TableName.ToLower())
                                        {
                                            case "zone":
                                            case "code":
                                                continue;
                                        }
                                    }

                                    if (mode == "delete")
                                    {
                                        // DELETE FROM Table before bulk operation
                                        switch (table.TableName.ToLower())
                                        {
                                            case "code":
                                                if (restoreZonesAndCodes && stateBackup.StateBackupType != StateBackupType.Customer)
                                                {
                                                    // Delete Codes
                                                    command.CommandText = "DELETE FROM Code WITH (TABLOCKX) " + codeCondition;
                                                    command.ExecuteNonQuery();
                                                }
                                                break;

                                            case "commission":
                                                //if (restoreZonesAndCodes && stateBackup.StateBackupType != StateBackupType.Customer)
                                                //{
                                                    command.CommandText = "DELETE FROM Commission WITH (TABLOCKX) " + commissionCondition;
                                                    command.ExecuteNonQuery();
                                                //}
                                                break;

                                            //case "pricelistdata":
                                            //    // Delete Pricelist Data
                                            //    command.CommandText = "DELETE FROM PriceListData WITH (TABLOCKX) " + rateCondition;
                                            //    command.ExecuteNonQuery();
                                            //    break;

                                            case "rate":
                                                // Delete Rates
                                                command.CommandText = "DELETE FROM Rate WITH (TABLOCKX) " + rateCondition;
                                                command.ExecuteNonQuery();
                                                break;

                                            case "zone":
                                                if (restoreZonesAndCodes && stateBackup.StateBackupType != StateBackupType.Customer)
                                                {
                                                    // Delete Zones
                                                    command.CommandText = "DELETE FROM Zone WITH (TABLOCKX) " + zoneCondition;
                                                    command.ExecuteNonQuery();
                                                }
                                                break;

                                            case "pricelist":
                                                // Delete Pricelists
                                                command.CommandText = "DELETE FROM PriceList WITH (TABLOCKX) " + pricelistCondition;
                                                command.ExecuteNonQuery();
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        // Insert into Table as a Bulk
                                        if (table.TableName != "PriceListData")
                                        {
                                            SqlBulkCopy bulkCopy = new SqlBulkCopy((SqlConnection)connection, SqlBulkCopyOptions.KeepIdentity, (SqlTransaction)transaction);
                                            foreach (DataColumn column in table.Columns)
                                                bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(column.ColumnName, column.ColumnName));
                                            bulkCopy.DestinationTableName = table.TableName;
                                            bulkCopy.BulkCopyTimeout = 15 * 60; // 15 minute timeout
                                            bulkCopy.WriteToServer(table);
                                            bulkCopy.Close();
                                        }
                                    }
                                }
                            }

                            var cleanUpcommand = connection.CreateCommand();
                            cleanUpcommand.Transaction = transaction;
                            cleanUpcommand.CommandText = "EXEC bp_CleanUpAfterStateBackupRestore";
                            cleanUpcommand.ExecuteNonQuery();

                            transaction.Commit();

                            log.InfoFormat("Restored From {0}", stateBackup);

                            // After a restore we need a refresh
                            ObjectAssembler.ClearAllCollections();
                        }

                        catch (Exception ex)
                        {
                            log.Error("Error Restoring State Backup", ex);
                            throw ex;
                        }
                    }
                }
            }
        }

        public static void Restore(StateBackup stateBackup, bool restoreZonesAndCodes,out NHibernate.ITransaction transactiontocommit,bool Fail)
        {
            //var count = TABS.StateBackup.List().Count(s => s.StateBackupCarriers.Equals(stateBackup.StateBackupCarriers));
            transactiontocommit = null;
            Fail = true;
            var count = TABS.StateBackup.List().Count(s => s.Supplier != null && s.Supplier.Equals(stateBackup.Supplier));

            var isCodePreparationRestore = (stateBackup.Supplier != null && stateBackup.Supplier.Equals(TABS.CarrierAccount.SYSTEM));
            //case restor for specific supplier
            if (count == 1 && !isCodePreparationRestore)
            {
                string customerID = stateBackup.Customer != null ? stateBackup.Customer.CarrierAccountID : null;
                string supplierID = stateBackup.Supplier != null ? stateBackup.Supplier.CarrierAccountID : null;

                string carrier = supplierID == null ? customerID : supplierID;

                string deleteRateQuery = string.Format("Delete From Rate WITH (TABLOCKX) where zoneID in (select z.zoneid from zone z with(nolock) where z.supplierid='{0}' )", carrier);
                string deletePricelistQuery = string.Format("Delete From Pricelist WITH (TABLOCKX) where {0} = '{1}' ", customerID == null ? "supplierID" : "customerID", carrier);
                string deleteCodeQuery = string.Format("Delete From Code WITH (TABLOCKX) where zoneID in (select z.zoneid from zone z with(nolock) where z.supplierid='{0}' )", carrier);
                string deleteCommissionQuery = string.Format("Delete From Commission WITH (TABLOCKX) where zoneID in (select z.zoneid from zone z with(nolock) where z.supplierid='{0}' )", carrier);
                string deleteTodConsiderationQuery = string.Format("Delete From TodConsideration WITH (TABLOCKX) where zoneID in (select z.zoneid from zone z with(nolock) where z.supplierid='{0}' )", carrier);
                string deleteRouteBlockQuery = string.Format("Delete From RouteBlock WITH (TABLOCKX) where zoneID in (select z.zoneid from zone z with(nolock) where z.supplierid='{0}' )", carrier);
                string deleteZoneQuery = string.Format("Delete From Zone WITH (TABLOCKX) where supplierid='{0}' ", carrier);


                lock (typeof(StateBackup))
                {
                    // Clean Up
                    IDbConnection connection = TABS.ObjectAssembler.CurrentSession.Connection;
                    
                        NHibernate.ITransaction transaction = TABS.ObjectAssembler.CurrentSession.BeginTransaction();
                        
                            transactiontocommit = transaction;
                            try
                            {
                                IDbCommand command = connection.CreateCommand();
                                command.CommandTimeout = 0;
                                transaction.Enlist(command);

                                command.CommandText = deleteRateQuery;
                                command.ExecuteNonQuery();
                                command.CommandText = deletePricelistQuery;
                                command.ExecuteNonQuery();
                                command.CommandText = deleteCodeQuery;
                                command.ExecuteNonQuery();
                                command.CommandText = deleteCommissionQuery;
                                command.ExecuteNonQuery();
                                command.CommandText = deleteTodConsiderationQuery;
                                command.ExecuteNonQuery();
                                command.CommandText = deleteRouteBlockQuery;
                                command.ExecuteNonQuery();
                                command.CommandText = deleteZoneQuery;
                                command.ExecuteNonQuery();

                                var cleanUpcommand = connection.CreateCommand();
                                transaction.Enlist(cleanUpcommand);
                                cleanUpcommand.CommandText = "EXEC bp_CleanUpAfterStateBackupRestore";
                                cleanUpcommand.ExecuteNonQuery();

                               // transaction.Commit();
                               
                                log.InfoFormat("Restored From {0}", stateBackup);


                                // After a restore we need a refresh
                                //ObjectAssembler.ClearAllCollections(); moved after complete all operations
                                Fail = false;
                            }
                            catch (Exception ex1)
                            {
                                
                                log.Error("Error Restoring State Backup", ex1);
                                Fail = true;
                                throw ex1;
                                
                            }
                        
                    
                }


                return;
            }
            //Restore for Sys Supplier/CodePreperation
            DataSet data = GetData(stateBackup);

            // Only Customer Backup type has option to restore zones and codes or not
            restoreZonesAndCodes = stateBackup.StateBackupType != StateBackupType.Customer ? true : restoreZonesAndCodes;

            lock (typeof(StateBackup))
            {
                // Clean Up
               IDbConnection connection = TABS.ObjectAssembler.CurrentSession.Connection;
                
                    NHibernate.ITransaction transaction = TABS.ObjectAssembler.CurrentSession.BeginTransaction();
                    
                        transactiontocommit = transaction;
                        try
                        {
                            IDbCommand command = connection.CreateCommand();
                            command.CommandTimeout = 0;
                            transaction.Enlist(command);

                            string pricelistCondition, zoneCondition, rateCondition, codeCondition, commissionCondition;
                            GetSqlConditions(stateBackup, restoreZonesAndCodes, out pricelistCondition, out zoneCondition, out rateCondition, out codeCondition, out commissionCondition);

                            List<DataTable> tablesToRestore = new List<DataTable>();
                            foreach (DataTable table in data.Tables)
                            {
                                if (table.TableName.StartsWith("CodeGroup") || table.TableName.StartsWith("CarrierAccount") || table.TableName.StartsWith("Currency"))
                                { }
                                else tablesToRestore.Add(table);

                            }
                            // Sort tables most dependent first
                            tablesToRestore.Sort(stateBackup);

                            foreach (var mode in new string[] { "delete", "write" })
                            {
                                // If writing, begin by least depedent
                                if (mode == "write") tablesToRestore.Reverse();

                                foreach (DataTable table in tablesToRestore)
                                {
                                    // If not restoring zones and codes
                                    if (!restoreZonesAndCodes)
                                    {
                                        switch (table.TableName.ToLower())
                                        {
                                            case "zone":
                                            case "code":
                                                continue;
                                        }
                                    }

                                    if (mode == "delete")
                                    {
                                        // DELETE FROM Table before bulk operation
                                        switch (table.TableName.ToLower())
                                        {
                                            case "code":
                                                if (restoreZonesAndCodes && stateBackup.StateBackupType != StateBackupType.Customer)
                                                {
                                                    // Delete Codes
                                                    command.CommandText = "DELETE FROM Code WITH (TABLOCKX) " + codeCondition;
                                                    command.ExecuteNonQuery();
                                                }
                                                break;

                                            //case "pricelistdata":
                                            //    // Delete Pricelist Data
                                            //    command.CommandText = "DELETE FROM PriceListData WITH (TABLOCKX) " + rateCondition;
                                            //    command.ExecuteNonQuery();
                                            //    break;
                                            //case "commission":
                                            //    if (restoreZonesAndCodes && stateBackup.StateBackupType != StateBackupType.Customer)
                                            //    {
                                            //        // Delete Zones
                                            //        command.CommandText = "DELETE FROM Commission WITH (TABLOCKX) " + commissionCondition;
                                            //        command.ExecuteNonQuery();
                                            //    }
                                            //    break;

                                            case "rate":
                                                // Delete Rates
                                                command.CommandText = "DELETE FROM Rate WITH (TABLOCKX) " + rateCondition;
                                                command.ExecuteNonQuery();
                                                break;

                                            case "zone":
                                                if (restoreZonesAndCodes && stateBackup.StateBackupType != StateBackupType.Customer)
                                                {
                                                   // bug 2887
                                                    //Delete Commissions related to zones
                                                   // command.CommandText = "DELETE FROM Commission WITH (TABLOCKX) " + commissionCondition;
                                                   // command.ExecuteNonQuery();
                                                    //Delete ToDs related to zones
                                                   // command.CommandText = "DELETE FROM ToDConsideration WITH (TABLOCKX) " + commissionCondition;
                                                   // command.ExecuteNonQuery();
                                                    //selete Tariffs related to zones
                                                   // command.CommandText = "DELETE FROM Tariff WITH (TABLOCKX) " + commissionCondition;
                                                   // command.ExecuteNonQuery();
                                                    // Delete Zones
                                                    command.CommandText = "DELETE FROM Zone WITH (TABLOCKX) " + zoneCondition;
                                                    command.ExecuteNonQuery();
                                                    
                                                }
                                                break;

                                            case "pricelist":
                                                // Delete Pricelists
                                                if (stateBackup.StateBackupType == StateBackupType.Supplier && stateBackup.SupplierName.ToString() != "SYS")
                                                {
                                                    string[] PricelistID = stateBackup.Notes.Split(new char[] { '#' });
                                                    if (PricelistID.Count() > 1)
                                                    {
                                                        command.CommandText = "DELETE FROM PriceListData WITH (TABLOCKX) Where PriceListID =" + PricelistID[1].ToString();
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                                command.CommandText = "DELETE FROM PriceList WITH (TABLOCKX) " + pricelistCondition;
                                                command.ExecuteNonQuery();
                                               
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        // Insert into Table as a Bulk
                                        if (table.TableName != "PriceListData" && table.TableName != "Commission")
                                        {
                                            SqlBulkCopy bulkCopy = new SqlBulkCopy((SqlConnection)connection, SqlBulkCopyOptions.KeepIdentity, (SqlTransaction)command.Transaction);
                                            foreach (DataColumn column in table.Columns)
                                                bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(column.ColumnName, column.ColumnName));
                                            bulkCopy.DestinationTableName = table.TableName;
                                            bulkCopy.BulkCopyTimeout = 15 * 60; // 15 minute timeout
                                            bulkCopy.WriteToServer(table);
                                            bulkCopy.Close();
                                        }
                                    }
                                }
                            }

                            var cleanUpcommand = connection.CreateCommand();
                            transaction.Enlist(cleanUpcommand);
                            cleanUpcommand.CommandText = "EXEC bp_CleanUpAfterStateBackupRestore";
                            cleanUpcommand.ExecuteNonQuery();

                            //transaction.Commit();

                            log.InfoFormat("Restored From {0}", stateBackup);

                            // After a restore we need a refresh
                            //ObjectAssembler.ClearAllCollections(); moved after complete operation
                            Fail = false;
                        }

                        catch (Exception ex)
                        {
                            log.Error("Error Restoring State Backup", ex);
                            Fail = true;
                            throw ex;
                            
                        }
                    
                
            }
        }

        public override string Identifier
        {
            get { return "StateBackup:" + ID; }
        }

        #region IComparer<DataTable> Members

        public int Compare(DataTable x, DataTable y)
        {
            string[] names = { "code", "rate", "zone", "pricelistdata", "pricelist" };
            int xIndex = Array.IndexOf(names, x.TableName.Trim().ToLower());
            int yIndex = Array.IndexOf(names, y.TableName.Trim().ToLower());
            return xIndex.CompareTo(yIndex);
        }

        #endregion
    }
}
