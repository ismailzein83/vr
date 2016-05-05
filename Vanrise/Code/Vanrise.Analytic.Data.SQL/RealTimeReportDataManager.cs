using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.Analytic.Data.SQL
{
    public class RealTimeReportDataManager : BaseSQLDataManager, IRealTimeReportDataManager
    {
        public RealTimeReportDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #region Public Methods
        public bool AreRealTimeReportUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[Analytic].[RealTimeReport]", ref updateHandle);
        }
        public List<RealTimeReport> GetRealTimeReports()
        {
            return GetItemsSP("[Analytic].[sp_RealTimeReport_GetAll]", RealTimeReportReader);
        }
        public bool AddRealTimeReport(Entities.RealTimeReport realTimeReport, out int realTimeReportId)
        {
            object realTimeReportID;

            int recordesEffected = ExecuteNonQuerySP("Analytic.sp_RealTimeReport_Insert", out realTimeReportID, realTimeReport.Name,realTimeReport.UserID,realTimeReport.AccessType, Vanrise.Common.Serializer.Serialize(realTimeReport.Settings));
            realTimeReportId = (recordesEffected > 0) ? (int)realTimeReportID : -1;

            return (recordesEffected > 0);
        }
        public bool UpdateRealTimeReport(Entities.RealTimeReport realTimeReport)
        {
            int recordesEffected = ExecuteNonQuerySP("Analytic.sp_RealTimeReport_Update", realTimeReport.RealTimeReportId, realTimeReport.Name, realTimeReport.UserID, realTimeReport.AccessType, Vanrise.Common.Serializer.Serialize(realTimeReport.Settings));
            return (recordesEffected > 0);
        }
        #endregion

        #region Mappers
        private RealTimeReport RealTimeReportReader(IDataReader reader)
        {
            return new RealTimeReport
            {
                RealTimeReportId = (int)reader["ID"],
                Name = reader["Name"] as string,
                AccessType = (AccessType) reader["AccessType"],
                UserID = (int)reader["UserID"],
                Settings = Vanrise.Common.Serializer.Deserialize<RealTimeReportSettings>(reader["Settings"] as string),
            };
        }
        #endregion
    }
}
