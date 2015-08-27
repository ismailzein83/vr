using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.BusinessEntity.Business;

namespace TOne.Analytics.Data.SQL
{
    public class CarrierZoneSummaryStatsCommon
    {
        public void GetColumnNames(CarrierZoneSummaryStatsGroupKeys column, out string idColumn)
        {
            switch (column)
            {
                case CarrierZoneSummaryStatsGroupKeys.CustomerId:
                    idColumn = CustomerIDColumnName;
                    break;
                case CarrierZoneSummaryStatsGroupKeys.SupplierId:
                    idColumn = SupplierIDColumnName;
                    break;
                case CarrierZoneSummaryStatsGroupKeys.ZoneId:
                    idColumn = ZoneIDColumnName;
                    break;
                default:
                    idColumn = null;
                    break;
            }
        }

        #region Constant
        public const string CustomerIDColumnName = "CustomerID";
        public const string SupplierIDColumnName = "SupplierID";
        public const string ZoneIDColumnName = "ZoneId";
        
        //public const string OurZonesJoinQuery = " LEFT JOIN  OurZones z ON ts.OurZoneID = z.ZoneID";
        //public const string GateWayInJoinQuery = "Left JOIN SwitchConnectivity cscIn  ON (','+cscIn.Details+',' LIKE '%,'+ts.Port_IN +',%' ) AND(ts.SwitchID = cscIn.SwitchID) AND ts.CustomerID =cscIn.CarrierAccount ";
        //public const string GateWayOutJoinQuery = "Left JOIN SwitchConnectivity cscOut ON  (','+cscOut.Details+',' LIKE '%,'+ts.Port_OUT +',%') AND (ts.SwitchID = cscOut.SwitchID)  AND ts.SupplierID  =cscOut.CarrierAccount  ";
        #endregion
    }
}
