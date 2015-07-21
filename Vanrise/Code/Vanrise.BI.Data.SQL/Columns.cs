using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Data.SQL
{

    internal class DateTimeColumns
    {
        public const string DATE = "[Date].[Date]";

        public const string YEAR = "[Date].[Year]";

        public const string MONTHOFYEAR = "[Date].[Month Of Year]";

        public const string WEEKOFMONTH = "[Date].[Week Of Month]";

        public const string DAYOFMONTH = "[Date].[Day Of Month]";

        public const string HOUR = "[Time].[Hour]";
    }

    internal class MeasureColumns
    {
        public const string COST = "[Measures].[Cost Net]";
        public const string SALE = "[Measures].[Sale Net]";
        public const string DURATION_IN_MINUTES = "[Measures].[Duration In Minutes]";

        public const string PROFIT = "[Measures].[Profit_CALC]";

        public const string SUCCESSFUL_ATTEMPTS = "[Measures].[SuccessfulAttempts]";

        public const string ACD = "[Measures].[ACD]";

        public const string PDD = "[Measures].[PDD]";
    }

    internal class SaleZoneColumns
    {
        public const string ZONE_ID = "[SaleZones].[Zone ID]";
        public const string ZONE_NAME = "[SaleZones].[Z One Name]";
    }

    internal class CustomerAccountColumns
    {
        public const string CARRIER_ACCOUNT_ID = "[CustomerAccounts].[Carrier Account ID]";
        public const string PROFILE_NAME = "[CustomerAccounts].[Profile Name]";
    }

    internal class SupplierAccountColumns
    {
        public const string CARRIER_ACCOUNT_ID = "[SupplierAccounts].[Carrier Account ID]";
        public const string PROFILE_NAME = "[SupplierAccounts].[Profile Name]";
    }
}
