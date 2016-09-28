using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticTableSettings
    {
        public string ConnectionString { get; set; }

        /// <summary>
        /// either ConnectionString or ConnectionStringName should have value. ConnectionString has more priority than ConnectionStringName
        /// </summary>
        public string ConnectionStringName { get; set; }

        public string TableName { get; set; }

        public string HourlyTableName { get; set; }

        public string DailyTableName { get; set; }

        public string WeeklyTableName { get; set; }

        public string MonthlyTableName { get; set; }

        public string TimeColumnName { get; set; }
        public List<Guid> DataRecordTypeIds { get; set; }

        public RequiredPermissionSettings RequiredPermission { get; set; }
    }
}
