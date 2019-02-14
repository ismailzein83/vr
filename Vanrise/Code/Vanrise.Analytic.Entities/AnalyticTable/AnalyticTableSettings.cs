using System;
using System.Collections.Generic;
using Vanrise.Security.Entities;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticTableSettings
    {
        /// <summary>
        /// either ConnectionString or ConnectionStringName should have value. ConnectionString has more priority than ConnectionStringName
        /// </summary>
        public string ConnectionString { get; set; }

        public string ConnectionStringName { get; set; }

        public string ConnectionStringAppSettingName { get; set; }

        public string TableName { get; set; }

        public string HourlyTableName { get; set; }

        public string DailyTableName { get; set; }

        public string WeeklyTableName { get; set; }

        public string MonthlyTableName { get; set; }

        public string TimeColumnName { get; set; }

        public List<Guid> DataRecordTypeIds { get; set; }

        public RequiredPermissionSettings RequiredPermission { get; set; }

        public AnalyticDataProvider DataProvider { get; set; }


        public Guid? StatusDefinitionBEId { get; set; }

        public Guid? RecommendedStatusDefinitionId { get; set; }

    }
}