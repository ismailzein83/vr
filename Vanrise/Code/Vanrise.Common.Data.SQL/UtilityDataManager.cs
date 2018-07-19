using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class UtilityDataManager : BaseSQLDataManager, IUtilityDataManager
    {
        public DateTimeRange GetDateTimeRange()
        {
            return base.GetSQLDateTimeRange();
        }

        public bool CheckIfDefaultOrInvalid(DateTime? dateTime)
        {
            return base.CheckIfDefaultOrInvalid(dateTime);
        }
    }
}