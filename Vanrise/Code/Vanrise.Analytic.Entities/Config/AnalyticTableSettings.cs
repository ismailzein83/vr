using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticTableSettings
    {
        public string ConnectionString { get; set; }

        public string TableName { get; set; }

        public string HourlyTableName { get; set; }

        public string DailyTableName { get; set; }

        public string WeeklyTableName { get; set; }

        public string MonthlyTableName { get; set; }

        public string TimeColumnName { get; set; }
    }
}
