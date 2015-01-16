using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace TABS
{
    [Serializable]
    public class RouteOption : IComparable<TABS.RouteOption>
    {
        public int RouteID { get; set; }
        public CarrierAccount Supplier { get; set; }
        public string SupplierID { get { return Supplier.CarrierAccountID; } }
        public int SupplierZoneID { get; set; }
        public Zone SupplierZone { get { return ObjectAssembler.Get<Zone>(SupplierZoneID); } }
        public double SupplierActiveRate { get; set; }
        public double SupplierNormalRate { get; set; }
        public double SupplierOffPeakRate { get; set; }
        public double SupplierWeekendRate { get; set; }
        public short SupplierServicesFlag { get; set; }
        public byte Priority { get; set; }
        public byte NumberOfTries { get; set; }
        public RouteOptionState RouteOptionState { get; set; }
        public DateTime Updated { get; set; }
        public byte? Percentage { get; set; }
        public DateTime? RateBED { get; set; }
        public DateTime? RateEED { get; set; }

        // Statistics 
        public double? ASR { get; set; }
        public double? ACD { get; set; }
        public double? DurationsInMinutes { get; set; }

        // the Route
        public Route Route { get { return Route.GetRouteByID(this.RouteID); } }

        public static Dictionary<int, List<RouteOption>> GetOptions(string sql)
        {
            Dictionary<int, List<RouteOption>> options = new Dictionary<int, List<RouteOption>>();
            List<RouteOption> routeOptions = null;
            using (IDataReader reader = DataHelper.ExecuteReader(sql.ToString()))
            {
                while (reader.Read())
                {
                    int index = -1;
                    RouteOption option = new RouteOption();
                    index++; option.RouteID = reader.GetInt32(index);
                    index++; option.Supplier = TABS.CarrierAccount.All[reader.GetString(index)];
                    index++; option.SupplierZoneID = reader.IsDBNull(index) ? 0 : reader.GetInt32(index);
                    index++; option.SupplierServicesFlag = reader.GetInt16(index);
                    index++; option.SupplierNormalRate = reader.GetFloat(index);
                    index++; option.SupplierOffPeakRate = reader.IsDBNull(index) ? 0 : reader.GetFloat(index);
                    index++; option.SupplierWeekendRate = reader.IsDBNull(index) ? 0 : reader.GetFloat(index);
                    index++; option.Priority = reader.IsDBNull(index) ? (byte)1 : reader.GetByte(index);
                    index++; option.NumberOfTries = reader.IsDBNull(index) ? (byte)1 : reader.GetByte(index);
                    index++; option.Percentage = reader.IsDBNull(index) ? (byte?)null : reader.GetByte(index);
                    index++; option.RouteOptionState = (RouteOptionState)reader.GetByte(index);
                    index++; if (!reader.IsDBNull(index)) option.RateBED = reader.GetDateTime(index);
                    index++; if (!reader.IsDBNull(index)) option.RateEED = reader.GetDateTime(index);
                    if (!options.TryGetValue(option.RouteID, out routeOptions))
                    {
                        routeOptions = new List<RouteOption>();
                        options[option.RouteID] = routeOptions;
                    }
                    routeOptions.Add(option);
                }
            }
            return options;
        }

        public static Dictionary<int, List<RouteOption>> GetTopNRouteOptions(List<int> routeIDs, int TopValues, bool showBlocks)
        {
            var ids = routeIDs.Select(id => id.ToString()).ToArray();
            var commaSeperated = string.Join(",", ids);

            string SQL = string.Format(@"
                WITH RouteOptionsCTE AS ( SELECT
                    RO.RouteID,
                    RO.SupplierID,
                    RO.SupplierZoneID,
                    RO.SupplierServicesFlag, 
                    RO.SupplierActiveRate,
                    RO.SupplierOffPeakRate,
                    RO.SupplierWeekendRate,
                    RO.Priority,
                    RO.NumberOfTries,
                    RO.Percentage,
                    RO.State,
                    R.BeginEffectiveDate,
                    R.EndEffectiveDate,
                    RN = Row_Number() OVER (PARTITION BY (RO.RouteID) ORDER BY RO.Priority DESC, RO.SupplierActiveRate ASC)
                FROM
                    RouteOption RO WITH(NOLOCK)
                    LEFT JOIN Rate R WITH(NOLOCK) ON RO.SupplierZoneID = R.ZoneID AND R.IsEffective = 'Y'
                WHERE RO.RouteID IN ({1}) {2}
                )

                SELECT RO.RouteID,
                    RO.SupplierID,
                    RO.SupplierZoneID,
                    RO.SupplierServicesFlag, 
                    RO.SupplierActiveRate,
                    RO.SupplierOffPeakRate,
                    RO.SupplierWeekendRate,
                    RO.Priority,
                    RO.NumberOfTries,
                    RO.Percentage,
                    RO.State,
                    RO.BeginEffectiveDate,
                    RO.EndEffectiveDate FROM RouteOptionsCTE RO WHERE RO.RN <= {0} ", TopValues, commaSeperated, showBlocks ? "" : " AND RO.State = 1 ");

            return GetOptions(SQL);
        }

        public static List<RouteOption> GetOptions(int routeID, int TopValues, bool showBlocks)
        {
            string SQL = string.Format(@"
                SELECT TOP {0}
                    RO.RouteID,
                    RO.SupplierID,
                    RO.SupplierZoneID,
                    RO.SupplierServicesFlag, 
                    RO.SupplierActiveRate,
                    RO.SupplierOffPeakRate,
                    RO.SupplierWeekendRate,
                    RO.Priority,
                    RO.NumberOfTries,
                    RO.Percentage,
                    RO.State,
                    R.BeginEffectiveDate,
                    R.EndEffectiveDate
                FROM
                    RouteOption RO WITH(NOLOCK)
                    LEFT JOIN Rate R WITH(NOLOCK) ON RO.SupplierZoneID = R.ZoneID AND R.IsEffective = 'Y'
                WHERE RO.RouteID = {1} {2}
                ORDER BY RO.Priority DESC, RO.SupplierActiveRate ASC", TopValues, routeID, showBlocks ? "" : " AND RO.State = 1 ");

            var options = GetOptions(SQL);
            List<RouteOption> results;
            if (!options.TryGetValue(routeID, out results))
                results = new List<RouteOption>();
            return results;
        }

        public class Stats
        {
            public string SupplierID { get; set; }
            public int? ZoneID { get; set; }
            public double? ASR { get; set; }
            public double? ACD { get; set; }
            public double? DurationInMinutes { get; set; }

            public bool match(string supplierId, int? zoneid)
            {
                if (string.IsNullOrEmpty(SupplierID)) return false;
                if (ZoneID == null) return false;

                return SupplierID.Equals(supplierId) && ZoneID.Equals(zoneid);
            }
        }

        public static List<TABS.RouteOption> GetOptionsWithStatistics(DateTime from, DateTime to, int routeID, int ourZoneID, bool showBlocks)
        {
            List<TABS.RouteOption> options = new List<TABS.RouteOption>();

            string SQL = @"
                SELECT 
                    RO.SupplierID,
                    RO.SupplierZoneID,
                    RO.SupplierServicesFlag, 
                    RO.SupplierActiveRate,
                    RO.SupplierOffPeakRate,
                    RO.SupplierWeekendRate,
                    RO.Priority,
                    RO.NumberOfTries,
                    RO.Percentage,
                    RO.State,
                    R.BeginEffectiveDate,
                    R.EndEffectiveDate,
                    0.0, 0.0, 0.0
                FROM RouteOption RO WITH(NOLOCK)
                    LEFT JOIN Rate R WITH(NOLOCK) ON RO.SupplierZoneID = R.ZoneID AND R.IsEffective = 'Y'
               WHERE RouteID = @P4 
               ORDER BY RO.Priority DESC, RO.SupplierActiveRate ASC";

            using (IDataReader reader = TABS.DataHelper.ExecuteReader(SQL, from, to, ourZoneID, routeID))
            {
                while (reader.Read())
                {
                    int index = -1;
                    TABS.RouteOption option = new TABS.RouteOption();
                    option.RouteID = routeID;
                    index++; option.Supplier = TABS.CarrierAccount.All[reader.GetString(index)];
                    index++; option.SupplierZoneID = reader.IsDBNull(index) ? 0 : reader.GetInt32(index);
                    index++; option.SupplierServicesFlag = reader.GetInt16(index);
                    index++; option.SupplierNormalRate = reader.GetFloat(index);
                    index++; option.SupplierOffPeakRate = reader.IsDBNull(index) ? 0 : reader.GetFloat(index);
                    index++; option.SupplierWeekendRate = reader.IsDBNull(index) ? 0 : reader.GetFloat(index);
                    index++; option.Priority = reader.GetByte(index);
                    index++; option.NumberOfTries = reader.GetByte(index);
                    index++; option.Percentage = reader.IsDBNull(index) ? (byte)0 : reader.GetByte(index);
                    index++; option.RouteOptionState = (TABS.RouteOptionState)reader.GetByte(index);
                    index++; if (!reader.IsDBNull(index)) option.RateBED = reader.GetDateTime(index);
                    index++; if (!reader.IsDBNull(index)) option.RateEED = reader.GetDateTime(index);
                    index++; if (!reader.IsDBNull(index)) option.ASR = (double?)reader.GetDecimal(index);
                    index++; if (!reader.IsDBNull(index)) option.ACD = (double?)reader.GetDecimal(index);
                    index++; if (!reader.IsDBNull(index)) option.DurationsInMinutes = (double?)reader.GetDecimal(index);
                    if (showBlocks)
                        options.Add(option);
                    else if (!showBlocks && option.RouteOptionState == RouteOptionState.Enabled)
                        options.Add(option);
                }
            }

            if (from == to) return options;

            List<Stats> stats = new List<Stats>();
            SQL = @"SELECT * FROM GetSupplierZoneStats(@P1,@P2,@P3)";
            using (IDataReader reader = TABS.DataHelper.ExecuteReader(SQL, from, to, ourZoneID))
            {
                while (reader.Read())
                {
                    int index = -1;
                    Stats stat = new Stats();
                    index++; if (!reader.IsDBNull(index)) stat.SupplierID = reader.GetString(index);
                    index++; if (!reader.IsDBNull(index)) stat.ZoneID = reader.GetInt32(index);
                    index++; if (!reader.IsDBNull(index)) stat.DurationInMinutes = (double?)reader.GetDecimal(index);
                    index++; if (!reader.IsDBNull(index)) stat.ASR = (double?)reader.GetDecimal(index);
                    index++; if (!reader.IsDBNull(index)) stat.ACD = (double?)reader.GetDecimal(index);
                    stats.Add(stat);
                }
            }

            foreach (var opt in options)
            {
                Stats stat = stats.FirstOrDefault(s => s.match(opt.SupplierID, opt.Route.OurZone.ZoneID));
                if (stat != null)
                {
                    opt.ASR = stat.ASR;
                    opt.ACD = stat.ACD;
                    opt.DurationsInMinutes = stat.DurationInMinutes;
                }
            }
            return options;
        }

        #region IComparable<RouteOption> Members

        public int CompareTo(RouteOption other)
        {
            if (other.Priority == this.Priority && other.Percentage == this.Percentage
                && other.NumberOfTries == this.NumberOfTries && other.RouteOptionState == this.RouteOptionState
                && other.Supplier.Equals(this.Supplier))
                return 1;
            else return -1;
        }

        #endregion

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            RouteOption other = obj as RouteOption;
            return other.Priority == this.Priority && other.Percentage == this.Percentage
                && other.NumberOfTries == this.NumberOfTries && other.RouteOptionState == this.RouteOptionState && other.Supplier.Equals(this.Supplier);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
