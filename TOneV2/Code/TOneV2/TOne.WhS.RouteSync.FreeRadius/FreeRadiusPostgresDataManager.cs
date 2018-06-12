using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Data.Postgres;
using Vanrise.Common;
using Vanrise.Entities;
using System.Data;
using System.Transactions;

namespace TOne.WhS.RouteSync.FreeRadius
{
    public class FreeRadiusPostgresDataManager : BasePostgresDataManager, IFreeRadiusDataManager
    {
        public Guid ConfigId { get { return new Guid("EBAAB50D-CEF3-4C4B-AAC8-FC677DCEA5E7"); } }

        FreeRadiusPostgresConnectionString _connectionString;
        List<FreeRadiusPostgresConnectionString> _redundantConnectionStrings;
        Dictionary<int, SingleNodeDataManager> _freeRadiusPostgresDataManagers;

        public FreeRadiusPostgresConnectionString ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public List<FreeRadiusPostgresConnectionString> RedundantConnectionStrings
        {
            get { return _redundantConnectionStrings; }
            set { _redundantConnectionStrings = value; }
        }

        #region Public Methods

        public void PrepareTables(IFreeRadiusPrepareTablesContext context)
        {
            SwitchSyncOutput switchSyncOutput;
            ExecFreeRadiusPostgresDataManagerAction((singleNodeDataManager, dataManagerIndex) =>
            {
                singleNodeDataManager.PrepareTables();
            }, context.SwitchName, context.SwitchId, null, context.WriteBusinessHandledException, true, "initializing", out switchSyncOutput);
            context.SwitchSyncOutput = switchSyncOutput;
        }

        public void BuildSaleCodeZones(IFreeRadiusBuildSaleCodeZonesContext context)
        {
            SwitchSyncOutput switchSyncOutput;
            ExecFreeRadiusPostgresDataManagerAction((singleNodeDataManager, dataManagerIndex) =>
            {
                singleNodeDataManager.BuildSaleCodeZones(context.FreeRadiusSaleZones, context.FreeRadiusSaleCodes);
            }, context.SwitchName, context.SwitchId, context.PreviousSwitchSyncOutput, context.WriteBusinessHandledException, false, null, out switchSyncOutput);

            context.SwitchSyncOutput = switchSyncOutput;
        }

        public object PrepareDataForApply(List<Entities.ConvertedRoute> freeRadiusRoutes)
        {
            return freeRadiusRoutes;
        }

        public void ApplySwitchRouteSyncRoutes(Entities.ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            List<FreeRadiusConvertedRoute> convertedRoute = (context.PreparedItemsForApply as List<ConvertedRoute>).Select(itm => itm as FreeRadiusConvertedRoute).ToList();

            SwitchSyncOutput switchSyncOutput;
            ExecFreeRadiusPostgresDataManagerAction((singleNodeDataManager, dataManagerIndex) =>
            {
                singleNodeDataManager.ApplyFreeRadiusRoutesForDB(convertedRoute);
            }, context.SwitchName, context.SwitchId, context.PreviousSwitchSyncOutput, context.WriteBusinessHandledException, false, null, out switchSyncOutput);

            context.SwitchSyncOutput = switchSyncOutput;
        }

        public void SwapTables(Entities.ISwapTableContext context)
        {
            FreeRadiusSwapTablePayload payload = context.Payload != null ? context.Payload.CastWithValidate<FreeRadiusSwapTablePayload>("freeRadiusSwapTablePayload") : null;
            bool syncSaleCodeZones = payload != null ? payload.SyncSaleCodeZones : false;

            SwitchSyncOutput switchSyncOutput;
            ExecFreeRadiusPostgresDataManagerAction((singleNodeDataManager, dataManagerIndex) =>
            {
                string[] args = new string[] { (dataManagerIndex + 1).ToString(), context.SwitchName };

                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finalizing Database {0} for Switch '{1}'...", args);

                singleNodeDataManager.SwapTables(context.IndexesCommandTimeoutInSeconds, syncSaleCodeZones);

                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Database {0} for Switch '{1}' is finalized", args);

            }, context.SwitchName, context.SwitchId, context.PreviousSwitchSyncOutput, context.WriteBusinessHandledException, true, "finalizing", out switchSyncOutput);

            context.SwitchSyncOutput = switchSyncOutput;
        }

        public void ApplyDifferentialRoutes(IFreeRadiusApplyDifferentialRoutesContext context)
        {
            List<FreeRadiusConvertedRoute> convertedUpdatedRoutes = context.ConvertedUpdatedRoutes;

            SwitchSyncOutput switchSyncOutput;
            ExecFreeRadiusPostgresDataManagerAction((freeRadiusPostgresDataManager, dataManagerIndex) =>
            {
                freeRadiusPostgresDataManager.ApplyDifferentialRoutes(convertedUpdatedRoutes);
            }, context.SwitchName, context.SwitchId, null, context.WriteBusinessHandledException, true, "applying differential routes to", out switchSyncOutput);

            context.SwitchSyncOutput = switchSyncOutput;
        }

        #endregion

        #region Private Methods

        private void ExecFreeRadiusPostgresDataManagerAction(Action<SingleNodeDataManager, int> action, string switchName, string switchId, SwitchSyncOutput previousSwitchSyncOutput,
            Action<Exception, bool> writeBusinessHandledException, bool isBusinessException, string businessExceptionMessage, out SwitchSyncOutput switchSyncOutput)
        {
            PrepareDataManagers();

            HashSet<int> failedNodeIndexes = null;
            if (previousSwitchSyncOutput != null && previousSwitchSyncOutput.SwitchRouteSynchroniserOutputList != null)
            {
                failedNodeIndexes = previousSwitchSyncOutput.SwitchRouteSynchroniserOutputList.Select(itm => (itm as FreeRadiusSWSyncOutput).ItemIndex).ToHashSet();
                if (failedNodeIndexes != null && failedNodeIndexes.Count == _freeRadiusPostgresDataManagers.Count)
                {
                    switchSyncOutput = new SwitchSyncOutput()
                    {
                        SwitchId = switchId,
                        SwitchSyncResult = SwitchSyncResult.Failed
                    };
                    return;
                }
            }

            ConcurrentDictionary<int, SwitchRouteSynchroniserOutput> exceptions = new ConcurrentDictionary<int, SwitchRouteSynchroniserOutput>();

            Parallel.For(0, _freeRadiusPostgresDataManagers.Count, (i) =>
            {
                if (failedNodeIndexes == null || !failedNodeIndexes.Contains(i))
                {
                    try
                    {
                        action(_freeRadiusPostgresDataManagers[i], i);
                    }
                    catch (Exception ex)
                    {
                        string errorBusinessMessage = Utilities.GetExceptionBusinessMessage(ex);
                        string exceptionDetail = ex.ToString();
                        exceptions.TryAdd(i, new FreeRadiusSWSyncOutput() { ItemIndex = i, ErrorBusinessMessage = errorBusinessMessage, ExceptionDetail = exceptionDetail });
                        Exception exception = isBusinessException ? new VRBusinessException(string.Format("Error occured while {0} Database {1} for Switch '{2}'", businessExceptionMessage, i + 1, switchName), ex) : ex;
                        writeBusinessHandledException(exception, false);
                    }
                }
            });

            switchSyncOutput = exceptions.Count > 0 ? new SwitchSyncOutput()
            {
                SwitchId = switchId,
                SwitchRouteSynchroniserOutputList = exceptions.Values.ToList(),
                SwitchSyncResult = exceptions.Count == _freeRadiusPostgresDataManagers.Count ? SwitchSyncResult.Failed : SwitchSyncResult.PartialFailed
            } : null;
        }

        private void PrepareDataManagers()
        {
            if (_freeRadiusPostgresDataManagers == null)
            {
                int counter = 0;
                _freeRadiusPostgresDataManagers = new Dictionary<int, SingleNodeDataManager>();
                _freeRadiusPostgresDataManagers.Add(counter, new SingleNodeDataManager(_connectionString));
                counter++;

                if (_redundantConnectionStrings != null)
                {
                    foreach (FreeRadiusPostgresConnectionString freeRadiusPostgresConnectionString in _redundantConnectionStrings)
                    {
                        _freeRadiusPostgresDataManagers.Add(counter, new SingleNodeDataManager(freeRadiusPostgresConnectionString));
                        counter++;
                    }
                }
            }
        }

        #endregion
    }

    internal class SingleNodeDataManager : BasePostgresDataManager
    {
        const string TableName = "route";
        const string TempTableName = "route_temp";
        const string OldTableName = "route_old";

        const string SaleZoneTableName = "salezone";
        const string TempSaleZoneTableName = "salezone_temp";
        const string SaleCodeTableName = "salecode";
        const string TempSaleCodeTableName = "salecode_temp";

        string TableNameWithSchema { get { return !string.IsNullOrEmpty(_freeRadiusPostgresConnectionString.SchemaName) ? string.Format(@"""{0}"".{1}", _freeRadiusPostgresConnectionString.SchemaName, TableName) : TableName; } }
        string TempTableNameWithSchema { get { return !string.IsNullOrEmpty(_freeRadiusPostgresConnectionString.SchemaName) ? string.Format(@"""{0}"".{1}", _freeRadiusPostgresConnectionString.SchemaName, TempTableName) : TempTableName; } }
        string OldTableNameWithSchema { get { return !string.IsNullOrEmpty(_freeRadiusPostgresConnectionString.SchemaName) ? string.Format(@"""{0}"".{1}", _freeRadiusPostgresConnectionString.SchemaName, OldTableName) : OldTableName; } }

        string SaleZoneTableNameWithSchema { get { return !string.IsNullOrEmpty(_freeRadiusPostgresConnectionString.SchemaName) ? string.Format(@"""{0}"".{1}", _freeRadiusPostgresConnectionString.SchemaName, SaleZoneTableName) : SaleZoneTableName; } }
        string TempSaleZoneTableNameWithSchema { get { return !string.IsNullOrEmpty(_freeRadiusPostgresConnectionString.SchemaName) ? string.Format(@"""{0}"".{1}", _freeRadiusPostgresConnectionString.SchemaName, TempSaleZoneTableName) : TempSaleZoneTableName; } }
        string SaleCodeTableNameWithSchema { get { return !string.IsNullOrEmpty(_freeRadiusPostgresConnectionString.SchemaName) ? string.Format(@"""{0}"".{1}", _freeRadiusPostgresConnectionString.SchemaName, SaleCodeTableName) : SaleCodeTableName; } }
        string TempSaleCodeTableNameWithSchema { get { return !string.IsNullOrEmpty(_freeRadiusPostgresConnectionString.SchemaName) ? string.Format(@"""{0}"".{1}", _freeRadiusPostgresConnectionString.SchemaName, TempSaleCodeTableName) : TempSaleCodeTableName; } }


        FreeRadiusPostgresConnectionString _freeRadiusPostgresConnectionString;

        public string ConnectionString { get { return GetConnectionString(); } }

        public SingleNodeDataManager(FreeRadiusPostgresConnectionString freeRadiusPostgresConnectionString)
        {
            _freeRadiusPostgresConnectionString = freeRadiusPostgresConnectionString;
        }

        protected override string GetConnectionString()
        {
            return _freeRadiusPostgresConnectionString.ConnectionString;
        }

        public void PrepareTables()
        {
            BuildRouteTempTable();
        }

        public void BuildSaleCodeZones(List<FreeRadiusSaleZone> freeRadiusSaleZones, List<FreeRadiusSaleCode> freeRadiusSaleCodes)
        {
            BuildSaleCodeZoneTempTables();

            if (freeRadiusSaleZones != null && freeRadiusSaleZones.Count > 0)
                base.Bulk(freeRadiusSaleZones, TempSaleZoneTableNameWithSchema);

            if (freeRadiusSaleCodes != null && freeRadiusSaleCodes.Count > 0)
                base.Bulk(freeRadiusSaleCodes, TempSaleCodeTableNameWithSchema);
        }

        public void ApplyFreeRadiusRoutesForDB(List<FreeRadiusConvertedRoute> convertedRoute)
        {
            base.Bulk(convertedRoute, TempTableNameWithSchema);
        }

        public void SwapTables(int indexesCommandTimeoutInSeconds, bool syncSaleCodeZones)
        {
            string createIndexScript = string.Format(@"CREATE INDEX IX_route_CustomerId_cldsid_{0} ON {1} USING btree
                                                      (customer_id COLLATE pg_catalog.default, cldsid) TABLESPACE pg_default;
                                                      ALTER TABLE {1} CLUSTER ON IX_route_CustomerId_cldsid_{0};", Guid.NewGuid().ToString("N"), TempTableNameWithSchema);
            string swapTableScript = string.Format("ALTER TABLE IF EXISTS {0} RENAME TO {1}; ALTER TABLE {2} RENAME TO {3}; ", TableNameWithSchema, OldTableName, TempTableNameWithSchema, TableName);

            List<string> pgStrings = new List<string> { createIndexScript, swapTableScript };
            if (syncSaleCodeZones)
            {
                pgStrings.Add(string.Format("ALTER TABLE {0} RENAME TO {1};", TempSaleZoneTableNameWithSchema, SaleZoneTableName));
                pgStrings.Add(string.Format("ALTER TABLE {0} RENAME TO {1};", TempSaleCodeTableNameWithSchema, SaleCodeTableName));
            }

            ExecuteNonQuery(pgStrings.ToArray(), indexesCommandTimeoutInSeconds);
        }

        public void ApplyDifferentialRoutes(List<FreeRadiusConvertedRoute> updatedConvertedRoute)
        {
            if (updatedConvertedRoute == null || updatedConvertedRoute.Count == 0)
                return;

            List<FreeRadiusConvertedRoute> updatedRoutes = Vanrise.Common.Utilities.CloneObject(updatedConvertedRoute);

            HashSet<CustomerCode> customerCodes = updatedRoutes.Select(itm => new CustomerCode() { CustomerId = itm.Customer_id, Code = itm.Cldsid }).ToHashSet();

            List<FreeRadiusConvertedRoute> compressedAffectedRoutes = GetAffectedRoutes(customerCodes);
            if (compressedAffectedRoutes != null && compressedAffectedRoutes.Count > 0)
            {
                List<FreeRadiusConvertedRoute> decompressedAffectedRoutes = DecompressConvertedRoutes(compressedAffectedRoutes);

                foreach (var affectedRoute in decompressedAffectedRoutes)
                {
                    if (customerCodes.Contains(new CustomerCode { CustomerId = affectedRoute.Customer_id, Code = affectedRoute.Cldsid }))
                        continue;

                    updatedRoutes.Add(affectedRoute);
                }
            }

            List<FreeRadiusConvertedRoute> compressedUpdatedRoutes = Helper.CompressConvertedRoutes(updatedRoutes);
            UpdateDifferentialRoutes(compressedUpdatedRoutes, customerCodes);
        }

        private List<FreeRadiusConvertedRoute> GetAffectedRoutes(HashSet<CustomerCode> customerCodes)
        {
            if (customerCodes == null || customerCodes.Count == 0)
                return null;

            StringBuilder sb = new StringBuilder();

            foreach (var customerCode in customerCodes)
            {
                if (sb.Length == 0)
                {
                    sb.Append(string.Format(@"select customer_id, cast(clisid as text), cast(cldsid as text), code, min_perc, max_perc from {0} 
                        Where (customer_id = '{1}' and (cldsid = '{2}' or (cldsid @> '{2}' and (length(cldsid)-1) = Length('{2}')))) ", TableNameWithSchema, customerCode.CustomerId, customerCode.Code));
                }
                else
                {
                    sb.Append(string.Format(@"or (customer_id = '{0}' and (cldsid = '{1}' or (cldsid @> '{1}' and (length(cldsid)-1) = Length('{1}')))) ", customerCode.CustomerId, customerCode.Code));
                }
            }

            return GetItemsText<FreeRadiusConvertedRoute>(sb.ToString(), FreeRadiusConvertedRouteMapper, null);
        }

        private List<FreeRadiusConvertedRoute> DecompressConvertedRoutes(List<FreeRadiusConvertedRoute> compressedConvertedRoutes)
        {
            if (compressedConvertedRoutes == null || compressedConvertedRoutes.Count == 0)
                return null;

            List<FreeRadiusConvertedRoute> decompressedConvertedRoutes = new List<FreeRadiusConvertedRoute>();

            foreach (var compressedConvertedRoute in compressedConvertedRoutes)
            {
                string cldsid = compressedConvertedRoute.Cldsid;
                int leftBracketIndex = cldsid.IndexOf("[");
                if (leftBracketIndex >= 0)
                {
                    string codeWithoutLastDigit = cldsid.Substring(0, leftBracketIndex);
                    int minLastCodeDigit = Convert.ToInt32(cldsid.Substring(leftBracketIndex + 1, 1));
                    int maxLastCodeDigit = Convert.ToInt32(cldsid.Substring(leftBracketIndex + 3, 1));

                    for (int currentCodeLastDigit = minLastCodeDigit; currentCodeLastDigit <= maxLastCodeDigit; currentCodeLastDigit++)
                    {
                        decompressedConvertedRoutes.Add(new FreeRadiusConvertedRoute()
                        {
                            Customer_id = compressedConvertedRoute.Customer_id,
                            Clisis = compressedConvertedRoute.Clisis,
                            Cldsid = string.Concat(codeWithoutLastDigit, currentCodeLastDigit),
                            Option = compressedConvertedRoute.Option,
                            Min_perc = compressedConvertedRoute.Min_perc,
                            Max_perc = compressedConvertedRoute.Max_perc
                        });
                    }
                }
                else
                {
                    decompressedConvertedRoutes.Add(compressedConvertedRoute);
                }
            }

            return decompressedConvertedRoutes.Count > 0 ? decompressedConvertedRoutes : null;
        }

        private void UpdateDifferentialRoutes(List<FreeRadiusConvertedRoute> updatedRoutes, HashSet<CustomerCode> customerCodes)
        {
            StringBuilder pgQuery = new StringBuilder();

            foreach (var customerCode in customerCodes)
            {
                if (pgQuery.Length == 0)
                {
                    pgQuery.Append(string.Format(@"Delete from {0} 
                        Where (customer_id = '{1}' and (cldsid = '{2}' or (cldsid @> '{2}' and (length(cldsid)-1) = Length('{2}'))))", TableNameWithSchema, customerCode.CustomerId, customerCode.Code));
                }
                else
                {
                    pgQuery.Append(string.Format(" or (customer_id = '{0}' and (cldsid = '{1}' or (cldsid @> '{1}' and (length(cldsid)-1) = Length('{1}'))))", customerCode.CustomerId, customerCode.Code));
                }
            }

            pgQuery.Append(";");

            foreach (var updatedRoute in updatedRoutes)
            {
                pgQuery.Append(string.Format("Insert Into {0} (customer_id, clisid, cldsid, code, min_perc, max_perc) VALUES ('{1}', '{2}', '{3}', '{4}', {5}, {6});", TableNameWithSchema,
                    updatedRoute.Customer_id, updatedRoute.Clisis, updatedRoute.Cldsid, updatedRoute.Option, updatedRoute.Min_perc, updatedRoute.Max_perc));
            }

            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                Timeout = TransactionManager.DefaultTimeout
            };

            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                ExecuteNonQuery(new string[] { pgQuery.ToString() });
                transactionScope.Complete();
            }
        }

        private FreeRadiusConvertedRoute FreeRadiusConvertedRouteMapper(IDataReader reader)
        {
            return new FreeRadiusConvertedRoute()
            {
                Customer_id = reader["customer_id"] as string,
                Clisis = reader["clisid"] as string,
                Cldsid = reader["cldsid"] as string,
                Option = reader["code"] as string,
                Min_perc = Convert.ToDecimal(reader["min_perc"]), //(decimal)GetReaderValue<double>(reader, "min_perc"),
                Max_perc = Convert.ToDecimal(reader["max_perc"]) //(decimal)GetReaderValue<double>(reader, "max_perc")
            };
        }

        private void BuildRouteTempTable()
        {
            string dropTempTableScript = string.Format("DROP TABLE IF EXISTS {0};", TempTableNameWithSchema);
            string dropOldTableScript = string.Format("DROP TABLE IF EXISTS {0}; ", OldTableNameWithSchema);
            string createTempTableScript = string.Format(@"CREATE extension If not exists prefix;
                                                           CREATE TABLE {0} (       
                                                           customer_id character varying(3) COLLATE pg_catalog.""default"" NOT NULL,
                                                           clisid prefix_range NOT NULL,
                                                           cldsid prefix_range NOT NULL,
                                                           code text COLLATE pg_catalog.""default"" NOT NULL,
                                                           min_perc double precision NOT NULL,
                                                           max_perc double precision NOT NULL);", TempTableNameWithSchema);

            ExecuteNonQuery(new string[] { dropTempTableScript, dropOldTableScript, createTempTableScript });
        }

        private void BuildSaleCodeZoneTempTables()
        {
            string dropSaleZoneTableScript = string.Format("DROP TABLE IF EXISTS {0}; ", SaleZoneTableNameWithSchema);
            string dropSaleCodeTableScript = string.Format("DROP TABLE IF EXISTS {0}; ", SaleCodeTableNameWithSchema);
            string dropTempSaleZoneTableScript = string.Format("DROP TABLE IF EXISTS {0}; ", TempSaleZoneTableNameWithSchema);
            string dropTempSaleCodeTableScript = string.Format("DROP TABLE IF EXISTS {0};", TempSaleCodeTableNameWithSchema);

            string createTempSaleZoneTableScript = string.Format(@"CREATE TABLE {0} (                                                         
                                                                   ID bigint,
                                                                   Name character varying(255) COLLATE pg_catalog.""default"",
                                                                   SellingNumberPlanID integer);", TempSaleZoneTableNameWithSchema);

            string createTempSaleCodeTableScript = string.Format(@"CREATE TABLE {0} (
                                                                   ID bigint,
                                                                   Code character varying(20) COLLATE pg_catalog.""default"",
                                                                   ZoneId bigint);", TempSaleCodeTableNameWithSchema);

            ExecuteNonQuery(new string[] { dropSaleZoneTableScript, dropSaleCodeTableScript, dropTempSaleCodeTableScript, dropTempSaleZoneTableScript, 
                createTempSaleZoneTableScript, createTempSaleCodeTableScript });
        }
    }
}