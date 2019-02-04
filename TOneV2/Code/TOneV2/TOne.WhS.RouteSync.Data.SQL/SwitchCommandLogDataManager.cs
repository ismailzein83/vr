using TOne.WhS.RouteSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.RouteSync.Data.SQL
{
    public class SwitchCommandLogDataManager : BaseSQLDataManager, ISwitchCommandLogDataManager
    {
        #region Fields/Ctor

        public SwitchCommandLogDataManager()
            : base(GetConnectionStringName("LoggingDBConnStringKey", "LogDBConnString"))
        {
        }

        #endregion

        #region Public Methods

        public bool Insert(SwitchCommandLog switchCommandLog, out long insertedId)
        {
            object switchCommandLogId;
            int affectedRecords = ExecuteNonQuerySP("[RouteSync].[sp_SwitchCommandLog_Insert]", out switchCommandLogId, switchCommandLog.ProcessInstanceId, switchCommandLog.SwitchId,
                switchCommandLog.Command, switchCommandLog.Response);

            insertedId = (affectedRecords > 0) ? (long)switchCommandLogId : -1;
            return (affectedRecords > 0);
        }

        #endregion
    }
}