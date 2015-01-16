using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TABS.Components
{
    public partial class Engine
    {
        protected static object RouteChekDummyObject = new object();
        private static int RouteBuildOnErrorCount = 0;
        /// <summary>
        /// Enumerations of Possible Routing Operations performed by the engine
        /// </summary>

        public enum RoutingOperation
        {
            /// <summary>
            /// Will cause a rebuild for all routes and route options 
            /// </summary>
            Rebuild_Routes,
            /// <summary>
            /// Update Routes from ToD Considerations
            /// </summary>
            Update_From_ToD,
            /// <summary>
            /// Update Routes from special requests
            /// </summary>
            Update_From_Special_Requests,
            /// <summary>
            /// Update Routes from route blocks
            /// </summary>
            Update_From_Route_Blocks
        }

        /// <summary>
        /// Execute a routing operation
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static bool ExecuteRoutingOperation(RoutingOperation operation, log4net.ILog log)
        {
            switch (operation)
            {
                case RoutingOperation.Rebuild_Routes:
                    return BuildRoutes(log);
                default:
                    return ExecuteRoutingUpdate(operation, log);
            }
        }

        /// <summary>
        /// Update the routing tables from ToD Considerations, Special Requests or Route Blocks.
        /// </summary>
        /// <param name="operation">The operation to execute</param>
        /// <param name="log">The logger</param>
        /// <returns>True on Success</returns>
        public static bool ExecuteRoutingUpdate(RoutingOperation operation, log4net.ILog log)
        {
            bool success = false;
            string operationName = operation.ToString().Replace('_', ' ');
            try
            {
                log.InfoFormat("Updating Routes ({0}) Started", operationName);
                DateTime start = DateTime.Now;
                using (IDbConnection connection = DataHelper.GetOpenConnection())
                {
                    IDbCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    switch (operation)
                    {
                        case RoutingOperation.Update_From_ToD:
                            command.CommandText = "bp_UpdateRoutesFromToD";
                            break;
                        case RoutingOperation.Update_From_Special_Requests:
                            command.CommandText = "bp_UpdateRoutesFromSpecialRequests_Effectors";
                            break;
                        case RoutingOperation.Update_From_Route_Blocks:
                            command.CommandText = "bp_UpdateRoutesFromRouteBlocks_Effectors";
                            break;
                        default:
                            command.CommandText = "SELECT 0";
                            break;
                    }
                    command.CommandTimeout = 0; // Wait until finish
                    command.ExecuteNonQuery();
                }
                TimeSpan spent = DateTime.Now.Subtract(start);
                log.InfoFormat("Updating Routes ({0}) Finsihed, time: {1}", operationName, spent);
                success = true;
                SystemParameter.LastRouteUpdate.Value = DateTime.Now;
                using (NHibernate.ISession session = DataConfiguration.OpenSession())
                {
                    session.Update(SystemParameter.LastRouteUpdate);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while Updating Routes ({0})", ex);
                logMail.Error("Error while Updating Routes ({0})", ex);
                success = false;
            }

            if (success)
            {
                SystemParameter.LastRouteUpdate.Value = DateTime.Now;
                using (NHibernate.ISession session = DataConfiguration.OpenSession())
                {
                    session.Update(SystemParameter.LastRouteUpdate);
                }
            }

            return success;
        }

        public static bool SynchRoutes(log4net.ILog log, RouteSynchType updateType)
        {
            return SynchRoutes(log, updateType, TABS.Switch.All.Values.ToList());
        }
        protected static void CleanSystemMessages(log4net.ILog log)
        {
            try
            {
                TABS.DataHelper.ExecuteNonQuery("TRUNCATE TABLE SystemMessage");
                log.InfoFormat("Table SystemMessage truncated successfuly");
            }
            catch (Exception ex)
            {
                log.Error("Error while truncating SystemMessage, please truncate the table form controller page", ex);
            }

        }
        /// <summary>
        /// Update the Routes on all switches
        /// </summary>
        /// <param name="log">The logger for the operation</param>
        /// <param name="updateType">The type of update to perform</param>
        /// <returns>True if all updates were successful. The LastRouteUpdate value will be the time at which the operation concluded.</returns>
        public static bool SynchRoutes(log4net.ILog log, RouteSynchType updateType, List<TABS.Switch> CustomSwitchs)
        {
            // Check for running condition
            //TABS.Addons.Utilities.MemoryConsumption.TotalMemoryStart("Start Syn Memory With");
            lock (RouteChekDummyObject)
            {
                if (IsRouteOperationRunning) return false;

                // Mark as started running
                _IsRouteOperationRunning = true;
            }

            // Check if route build process ended successfully
            object routeBuildEnd = DataHelper.ExecuteScalar("SELECT [Message] FROM SystemMessage WHERE MessageID = 'BuildRoutes: End'");
            if (routeBuildEnd == null || routeBuildEnd == DBNull.Value)
            {
                log.ErrorFormat("Trying to Synch Routes but Route Build was not completed");
                CleanSystemMessages(log);
                logMail.ErrorFormat("Trying to Synch Routes but Route Build was not completed");
                _IsRouteOperationRunning = false;
                return false;
            }

            bool isSuccess = true;

            string updateTypeString = updateType.ToString().Replace('_', ' ');

            log.InfoFormat("Starting Route Synchronization, Suggested mode: {0}", updateTypeString);

            // Generate Routing Info
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            DateTime? lastSync = (updateType == RouteSynchType.Differential) ? Engine.LastRouteSynch : null;

            foreach (Switch attachedSwitch in CustomSwitchs)
            {
                // If Switch is not included in Routing Updates, skip it
                if (!attachedSwitch.Enable_Routing) continue;

                log.InfoFormat("Synchronizing Routes on Switch `{0}` ({1})", attachedSwitch, updateTypeString);
                try
                {
                    Extensibility.ISwitchManager manager = attachedSwitch.SwitchManager;
                    if (manager != null)
                    {
                        log.Info("Running Garbage Collection");
                        GC.Collect();
                        log.Info("Finished Garbage Collection");

                        DateTime switchUpdateStart = DateTime.Now;
                        _RouteOpertationStatus = string.Format("Synchronizing Routes to {0}", attachedSwitch);
                        if (manager.SynchRouting(attachedSwitch, updateType))
                        {
                            end = DateTime.Now;
                            log.InfoFormat("{0} Switch Manager completed Route Synchronization ({1}) in {2} Successfully", attachedSwitch, updateTypeString, end.Subtract(switchUpdateStart));
                            attachedSwitch.LastRouteUpdate = start;
                            DataHelper.ExecuteNonQuery("UPDATE [Switch] Set LastRouteUpdate = @P1 WHERE SwitchID = @P2", attachedSwitch.LastRouteUpdate, attachedSwitch.SwitchID);
                        }
                        else
                        {
                            log.ErrorFormat("{0} Switch Manager failed to complete Route Synchronization ({1})", attachedSwitch, updateTypeString);
                            logMail.ErrorFormat("{0} Switch Manager failed to complete Route Synchronization ({1})", attachedSwitch, updateTypeString);
                        }
                    }
                    else
                    {
                        log.InfoFormat("Switch `{0}` has no compatible manager defined", attachedSwitch);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Error synchronizing Routes on Switch `{0}` ({1})", attachedSwitch, updateTypeString), ex);
                    logMail.Error(string.Format("Error synchronizing Routes on Switch `{0}` ({1})", attachedSwitch, updateTypeString), ex);
                    isSuccess = false;
                }
            }

            _RouteOpertationStatus = null;

            // Signal last route synch
            SystemParameter.LastRouteSynch.Value = start;
            try
            {
                using (NHibernate.ISession session = DataConfiguration.OpenSession())
                {
                    session.Update(SystemParameter.LastRouteSynch);
                    session.Flush();
                }
            }
            catch (Exception ex)
            {
                log.Error("Error updating last synch time", ex);
                logMail.Error("Error updating last synch time", ex);
            }
            end = DateTime.Now;
            log.InfoFormat("Route Synchronization Finished (Total time: {0}, {1})", end.Subtract(start), updateTypeString);

            log.Info("Running Final Garbage Collection");
            GC.Collect();
            GC.Collect();
            log.Info("Finished Final Garbage Collection");

            _IsRouteOperationRunning = false;
            //TABS.Addons.Utilities.MemoryConsumption.TotalMemoryEnd("End Syn Memory With");
            return isSuccess;
        }

        protected static bool IsToBeDeleted(CarrierAccount carrieraccount, bool IsCustomer)
        {
            if (carrieraccount.IsDeleted
                 || carrieraccount.ActivationStatus == ActivationStatus.Inactive
                || carrieraccount.RoutingStatus == RoutingStatus.Blocked) return true;

            if (IsCustomer && carrieraccount.RoutingStatus == RoutingStatus.BlockedInbound) return true;

            if (!IsCustomer && carrieraccount.RoutingStatus == RoutingStatus.BlockedOutbound) return true;

            return false;
        }

        protected static void RemoveDeletedAccountsFromOverrides(log4net.ILog log)
        {
            var toUpdate = new List<TABS.RouteOverride>();
            var toDelete = new List<TABS.RouteOverride>();

            var overrides = TABS.RouteOverride.All;

            foreach (var carrierRouteOverride in overrides)
            {

                if (IsToBeDeleted(carrierRouteOverride.Key, true))
                {
                    toDelete.AddRange(carrierRouteOverride.Value);
                    continue;
                }

                foreach (var routeOverride in carrierRouteOverride.Value)
                {
                    // remove from options
                    var options = TABS.RouteOverride.GetRouteOptions(routeOverride._RouteOptions);
                    var optionsToDelete = new List<CarrierAccount>();

                    if (options != null)
                    {
                        foreach (var option in options)
                        {
                            if (IsToBeDeleted(option, false))
                                optionsToDelete.Add(option);
                        }
                    }

                    if (optionsToDelete.Count > 0 && options.Except(optionsToDelete).Count() > 0)
                    {
                        routeOverride._RouteOptions = options.Except(optionsToDelete).Select(c => c.CarrierAccountID).Aggregate((c1, c2) => c1 + "|" + c2);
                        toUpdate.Add(routeOverride);
                    }

                    if (optionsToDelete.Count > 0 && options.Except(optionsToDelete).Count() == 0)
                    {
                        routeOverride._RouteOptions = TABS.CarrierAccount.BLOCKED.CarrierAccountID;
                        toUpdate.Add(routeOverride);
                    }

                    // remove from blocks
                    var blocks = TABS.RouteOverride.GetBlockedSuppliers(routeOverride._BlockedSuppliers);
                    var blocksToDelete = new List<CarrierAccount>();

                    if (blocks != null)
                    {
                        foreach (var block in blocks)
                        {
                            if (IsToBeDeleted(block, false))
                                blocksToDelete.Add(block);
                        }
                    }

                    if (blocksToDelete.Count > 0 && blocks.Except(blocksToDelete).Count() > 0)
                    {
                        routeOverride._BlockedSuppliers = blocks.Except(blocksToDelete).Select(c => c.CarrierAccountID).Aggregate((c1, c2) => c1 + "|" + c2);
                        toUpdate.Add(routeOverride);
                    }

                    if (blocksToDelete.Count > 0 && blocks.Except(blocksToDelete).Count() == 0)
                        toDelete.Add(routeOverride);

                }

            }

            using (var session = TABS.DataConfiguration.CurrentSession)
            {

                var transaction = session.BeginTransaction();
                try
                {
                    lock (typeof(RouteOverride))
                    {
                        foreach (var routeOverride in toUpdate)
                            session.SaveOrUpdate(routeOverride);
                        foreach (var routeOverride in toDelete)
                            session.Delete(routeOverride);
                        transaction.Commit();
                        session.Flush();
                        session.Close();
                    }

                    if (toDelete.Count > 0)
                        TABS.RouteOverride.ClearCachedCollections();
                }
                catch (Exception tsEx)
                {
                    transaction.Rollback();
                    log.Error("Error Saving Routing Overrides (When deleting accounts)", tsEx);
                    logMail.Error("Error Saving Routing Overrides (When deleting accounts)", tsEx);
                }

            }
        }

        /// <summary>
        /// Build (rebuild) completely the routes and routing options for the system.
        /// </summary>
        /// <param name="log">The logger for information about execution</param>
        /// <returns></returns>
        public static bool BuildRoutes(log4net.ILog log)
        {
            // Check for running condition
            lock (RouteChekDummyObject)
            {
                if (IsRouteOperationRunning) return false;

                // Mark as started running
                _IsRouteOperationRunning = true;
            }

            bool routeBuildWasFlagged = SystemParameter.IsRouteBuildRequired.BooleanValue.Value;
            SystemParameter.IsRouteBuildRequired.BooleanValue = false;

            log.Info("Build Routes Started");

            bool success = false;

            // Build the code map
            try
            {
                log.Info("Building Code Map started");
                TimeSpan codeMapBuildTime;
                _RouteOpertationStatus = "Building Code Map";
                int count = TABS.CodeMap.Build(out codeMapBuildTime);
                log.InfoFormat("Building Code Map: Success - count: {0}, time: {1}", count, codeMapBuildTime);
                success = true;
                //TABS.CodeMap.ClearCachedCollections();
                TABS.CodeMap.MappedCode.All = null;
                TABS.CodeMap.CodeMapSupplier.All = null;
                GC.Collect();
                TABS.CodeMap.MappedCode.All = new Dictionary<string, List<string>>();
                TABS.CodeMap.CodeMapSupplier.All = new Dictionary<string, CodeMap.CodeMapSupplier>();
            }
            catch (Exception ex)
            {
                log.Error("Error while building Code Map", ex);
                logMail.Error("Error while building Code Map", ex);
                success = false;
            }

            // Build the Routes if previous operation is successful.
            if (success && !IsRouteOperationStopRequested)
            {
                log.Info("Build Routes started");
                using (IDbConnection connection = DataHelper.GetOpenConnection())
                {
                    try
                    {
                        _RouteOpertationStatus = "Executing Build Routes";
                        DateTime start = DateTime.Now;
                        IDbCommand command = connection.CreateCommand();
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "bp_BuildRoutes_New";
                        DataHelper.AddParameter(command, "@MaxSuppliersPerRoute", SystemParameter.MaxSuppliersPerRoute.Value);
                        DataHelper.AddParameter(command, "@IncludeBlockedZones", SystemParameter.Include_Blocked_Zones_In_ZoneRates.BooleanValue.Value ? "Y" : "N");

                        // Routing Tables Options
                        DataHelper.AddParameter(command, "@RoutingTableFileGroup", SystemParameter.Routing_Table_FileGroup.Value);
                        DataHelper.AddParameter(command, "@RoutingIndexesFileGroup", SystemParameter.Routing_Indexes_FileGroup.Value);
                        DataHelper.AddParameter(command, "@SORT_IN_TEMPDB", SystemParameter.Routing_Sort_Indexes_In_Temp.BooleanValue.Value ? "ON" : "OFF");

                        IDbDataParameter parameter = DataHelper.AddParameter(command, "@UpdateStamp", DateTime.Now);
                        parameter.Direction = ParameterDirection.Output;
                        command.CommandTimeout = 0;
                        command.ExecuteNonQuery();
                        success = true;
                        SystemParameter.LastRouteBuild.Value = (parameter.Value == null || parameter.Value == DBNull.Value) ? null : parameter.Value;
                        log.InfoFormat("Build Routes ended. Time: {0}", DateTime.Now.Subtract(start));
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error while executing bp_BuildRoutes", ex);
                        logMail.Error("Error while executing bp_BuildRoutes", ex);
                        success = false;
                    }
                }
            }

            if (success)
            {
                try
                {
                    SystemParameter.LastRouteBuild.Value = DateTime.Now;
                    using (NHibernate.ISession session = DataConfiguration.OpenSession())
                    {
                        var transaction = session.BeginTransaction();
                        session.Update(SystemParameter.LastRouteBuild);
                        transaction.Commit();
                        session.Flush();
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error Updating Last Route Build Time", ex);
                    logMail.Error("Error Updating Last Route Build Time", ex);
                }
            }
            else
            {
                if (routeBuildWasFlagged)
                    SystemParameter.IsRouteBuildRequired.BooleanValue = routeBuildWasFlagged;
            }



            log.InfoFormat("Build Routes Ended, Success: {0}", success);

            _RouteOpertationStatus = null;
            _IsRouteOperationRunning = false;
            _IsRouteOperationStopRequested = false;
            //3= System parameter 
            if (!success && RouteBuildOnErrorCount <3)
            {
                CleanSystemMessages(log);
                log.InfoFormat("Error in route build, trying to rerun route build automaticly at {0}", DateTime.Now);
                RouteBuildOnErrorCount++;
                BuildRoutes(log);
            }
            if (success == true)
                RouteBuildOnErrorCount = 0;
            GC.Collect();
            GC.Collect();
            return success;
        }
        public static bool BuildRoutesForCodeComparison(log4net.ILog log)
        {
            // Check for running condition
            lock (RouteChekDummyObject)
            {
                if (IsRouteOperationRunning) { 
                    log.Info("Build Routes for codeComparison not started /route Operation Running"); return false;
                }

                // Mark as started running
                _IsRouteOperationRunning = true;
            }

            bool routeBuildWasFlagged = SystemParameter.IsRouteBuildRequired.BooleanValue.Value;
            SystemParameter.IsRouteBuildRequired.BooleanValue = false;

            log.Info("Build Routes for CodeComparison Started");

            bool success = false;

            // Build the code map
            try
            {
                log.Info("Building Code Map started for code comparison");
                TimeSpan codeMapBuildTime;
                _RouteOpertationStatus = "Building Code Map";
                int count = TABS.CodeMap.BuildForCodeComparison(out codeMapBuildTime);
                log.InfoFormat("Building Code Map: Success - count: {0}, time: {1}", count, codeMapBuildTime);
                success = true;
                TABS.CodeMap.MappedCode.All = null;
                TABS.CodeMap.CodeMapSupplier.All = null;
                GC.Collect();
                TABS.CodeMap.MappedCode.All = new Dictionary<string, List<string>>();
                TABS.CodeMap.CodeMapSupplier.All = new Dictionary<string, CodeMap.CodeMapSupplier>();
            }
            catch (Exception ex)
            {
                log.Error("Error while building Code Map for Code Comarison", ex);
                logMail.Error("Error while building Code Map for Code Comarison", ex);
                success = false;
            }
            log.InfoFormat("Build Routes Ended, Success: {0}", success);
            GC.Collect();
            GC.Collect();
            _IsRouteOperationRunning = false;
            return success;
        }
    }
}