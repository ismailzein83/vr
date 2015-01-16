using System;
using System.Data;

namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Route Update Check", "Checks the system if a route update/rebuild is necessary and takes appropriate action. This should be run on hourly basis.")]
    public class RouteUpdateChecker : RouteAffectorBase
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("TABS.RouteUpdateChecker");

        public override void Run()
        {
            log.Info("Route Update Check requested to Run");

            if (IsRunning)
            {
                log.Info("Route Operation Already Running. Aborting Request.");
                return;
            }

            log.Info("Route Update Check Started");

            _LastRun = DateTime.Now;

            bool success = true;

            try
            {
                // A Route Rebuild is required, no need to check for other stuff
                if (SystemParameter.IsRouteBuildRequired.BooleanValue.Value == true)
                {
                    Components.TaskManager.LocateAndRun(typeof(RouteBuilder));
                }
                else
                {
                    using (IDbConnection connection = DataHelper.GetOpenConnection())
                    {
                        // flag actions to take
                        bool rebuildRequired = SystemParameter.IsRouteBuildRequired.BooleanValue.Value;
                        bool hasSpecialRequestChanges = false;
                        bool hasRouteBlockChanges = false;

                        // If no rebuild is required
                        if ( ! rebuildRequired)
                        {
                            // Create a command to query changes from DB
                            IDbCommand command = connection.CreateCommand();
                            IDbDataParameter pWhen = DataHelper.AddParameter(command, "@When", DateTime.Now);
                            IDbDataParameter pRateChanges = DataHelper.AddParameter(command, "@RateChanges", 'N');
                            IDbDataParameter pSpecialRequestChanges = DataHelper.AddParameter(command, "@SpecialRequestChanges", 'N');
                            IDbDataParameter pRouteBlockChanges = DataHelper.AddParameter(command, "@RouteBlockChanges", 'N');
                            pRateChanges.Direction = ParameterDirection.Output;
                            pSpecialRequestChanges.Direction = ParameterDirection.Output;
                            pRouteBlockChanges.Direction = ParameterDirection.Output;
                            command.CommandText = "bp_GetHourlyChangeFlags";
                            command.CommandType = CommandType.StoredProcedure;
                            // Execute
                            command.ExecuteNonQuery();
                            
                            // update actions to take
                            rebuildRequired = "Y".Equals(pRateChanges.Value.ToString());
                            hasSpecialRequestChanges = "Y".Equals(pSpecialRequestChanges.Value.ToString());
                            hasRouteBlockChanges = "Y".Equals(pRouteBlockChanges.Value.ToString());
                        }

                        // Rate Changes means all others discarded
                        if (rebuildRequired)
                        {
                            Components.TaskManager.LocateAndRun(typeof(RouteBuilder));
                        }
                        // 
                        else
                        {
                            if (hasSpecialRequestChanges)
                                Components.TaskManager.LocateAndRun(typeof(RouteAffector_SepcialRequests));
                            if (hasRouteBlockChanges)
                                Components.TaskManager.LocateAndRun(typeof(RouteAffector_RouteBlocks));
                        }

                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _Exception = ex;
                success = false;
            }

            // Stopped Running
            _LastRunDuration = DateTime.Now.Subtract(_LastRun.Value);

            log.Info("Route Update Checker Finished");
            IsLastRunSuccessful = success;
        }

        public override string Status { get { return string.Empty; } }
    }
}
