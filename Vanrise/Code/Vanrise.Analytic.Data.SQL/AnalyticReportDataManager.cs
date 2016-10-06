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
    public class AnalyticReportDataManagerL:BaseSQLDataManager,IAnalyticReportDataManager
    {
        public AnalyticReportDataManagerL()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #region Public Methods
        public bool AreAnalyticReportUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[Analytic].[AnalyticReport]", ref updateHandle);
        }
        public List<AnalyticReport> GetAnalyticReports()
        {
            return GetItemsSP("[Analytic].[sp_AnalyticReport_GetAll]", AnalyticReportReader);
        }
        public bool AddAnalyticReport(Entities.AnalyticReport analyticReport)
        {
            int recordesEffected = ExecuteNonQuerySP("Analytic.sp_AnalyticReport_Insert", analyticReport.AnalyticReportId, analyticReport.Name, analyticReport.UserID, analyticReport.AccessType, Vanrise.Common.Serializer.Serialize(analyticReport.Settings));

            return (recordesEffected > 0);
        }
        public bool UpdateAnalyticReport(Entities.AnalyticReport analyticReport)
        {
            int recordesEffected = ExecuteNonQuerySP("Analytic.sp_AnalyticReport_Update", analyticReport.AnalyticReportId, analyticReport.Name, analyticReport.UserID, analyticReport.AccessType, Vanrise.Common.Serializer.Serialize(analyticReport.Settings));
            return (recordesEffected > 0);
        }
        #endregion

        #region Mappers
        private AnalyticReport AnalyticReportReader(IDataReader reader)
        {
            return new AnalyticReport
            {
                AnalyticReportId =  GetReaderValue<Guid>(reader,"ID"),
                Name = reader["Name"] as string,
                AccessType = (AccessType)reader["AccessType"],
                UserID = (int)reader["UserID"],
                Settings = Vanrise.Common.Serializer.Deserialize<AnalyticReportSettings>(reader["Settings"] as string),
            };
        }
        #endregion
    }
}
