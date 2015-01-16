using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TABS
{
    public abstract class ITableRow
    {
        public string Day { get; set; }
        public string Customer { get; set; }
        public string Supplier { get; set; }
        public string SaleZone { get; set; }
        public string CostZone { get; set; }
        public string DurationInSeconds { get; set; }
        public string CDPN { get; set; }
        public string CDPNOut { get; set; }
        public string Carrier { get; set; }
        public string Type { get; set; }

        public abstract string GetTableRowString();
    }
    public class MissMappedView1 : ITableRow
    {
        #region ITableRow Members

        public override string GetTableRowString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append(Day);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append(Customer);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append(Supplier);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append(CDPN);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append(CDPNOut);
            sb.Append("</td>");
            sb.Append("</tr>");
            return sb.ToString();
        }

        #endregion
    }
    public class MissMappedView2 : ITableRow
    {
        #region ITableRow Members

        public override string GetTableRowString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append(Day);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append(Customer);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append(Supplier);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append(SaleZone);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append(CostZone);
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append(DurationInSeconds);
            sb.Append("</td>");
            sb.Append("</tr>");
            return sb.ToString();
        }

        #endregion
    }

    public class MissMappedView3 : ITableRow
    {
        #region ITableRow Members

        public override string GetTableRowString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append(Carrier);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append(Type);
            sb.Append("</td>");
            sb.Append("</tr>");
            return sb.ToString();
        }

        #endregion
    }
    public class MissMappedHelper
    {
        public static DataTable GetMismappedCdrDataSource(DateTime? fromDate, DateTime? tillDate, double? top, double? minDuration)
        {
            string sql =
                @"EXEC bp_GetMissMappedCdrs 
                 @From = @P1,
                 @Till = @P2,
                 @Top = @P3,
                 @MinDuration = @p4";
            return TABS.DataHelper.GetDataTable
                    (sql,
                    fromDate,
                    tillDate.Value.AddDays(1),
                    top,
                    minDuration);
        }


        public static DataTable GetMismappedDataSource(DateTime? fromDate, DateTime? tillDate, double? top, double? minDuration)
        {
            string sql =
                @"EXEC bp_MissMapped
                 @From = @P1,
                 @Till = @P2,
                 @Top = @P3,
                 @MinDuration = @P4";
            return TABS.DataHelper.GetDataTable
                (sql,
                 fromDate,
                 tillDate.Value.AddDays(1),
                 top,
                 minDuration);

        }

        public static DataTable GetMismappedOurZoneDataSource(DateTime? fromDate, DateTime? tillDate, double? top, double? minDuration, string selectedCustomerID, string selectedSupplierID)
        {
            string sql =
                @"EXEC bp_MissOurZone
                 @Top = @P1,
                 @From = @P2,
                 @Till = @P3,
                 @MinDuration = @P4,
                 @CustomerID = @P5,
                 @SupplierID= @P6";

            return TABS.DataHelper.GetDataTable
                (sql,
                 top,
                 fromDate,
                 tillDate.Value.AddDays(1),
                 minDuration,
                 selectedCustomerID,
                 selectedSupplierID);
        }

        public static DataTable GetMismappedSupplierZoneDataSource(DateTime? fromDate, DateTime? tillDate, double? top, double? minDuration, string selectedCustomerID, string selectedSupplierID)
        {
            string sql =
               @"EXEC bp_MissSupplierZone
                @Top = @P1,
                @From = @P2,
                @Till = @P3,
                @MinDuration = @P4,
                @CustomerID = @P5,
                @SupplierID= @P6";

            return TABS.DataHelper.GetDataTable
                (sql,
                 top,
                 fromDate,
                 tillDate.Value.AddDays(1),
                 minDuration,
                 selectedCustomerID,
                 selectedSupplierID);
        }

        public static DataTable GetMismappedCDPNDataSource(DateTime? fromDate, DateTime? tillDate, double? top, double? minDuration)
        {
            string sql =
                @"EXEC bp_MissingCDPN
                 @Top = @P1,
                 @From = @P2,
                 @Till = @P3,
                 @MinDuration = @P4";

            return TABS.DataHelper.GetDataTable
                (sql,
                top,
                fromDate,
                tillDate.Value.AddDays(1),
                minDuration);
        }

        public static DataTable GetMismappedSaleDataSource(DateTime? fromDate, DateTime? tillDate, double? top, object sys_CDR_Pricing_CDRID, string selectedCustomerID, string selectedSupplierID)
        {
            string sql =
                @"EXEC bp_MissingSale 
                 @Top = @P1, 
                 @sys_CDR_Pricing_CDRID = @P2, 
                 @FromDate = @P3, 
                 @TillDate = @P4,
                 @CustomerID = @P5, 
                 @SupplierID = @P6";
            return TABS.DataHelper.GetDataTable
                (sql,
                top,
                sys_CDR_Pricing_CDRID,
                fromDate,
                tillDate.Value.AddDays(1),
                selectedCustomerID,
                selectedSupplierID);
        }

        public static DataTable GetMismappedCostDataSource(DateTime? fromDate, DateTime? tillDate, double? top, object sys_CDR_Pricing_CDRID, string selectedCustomerID, string selectedSupplierID)
        {
            string sql = @"EXEC bp_MissingCost 
                @Top = @P1, 
                @sys_CDR_Pricing_CDRID = @P2, 
                @FromDate = @P3, 
                @TillDate = @P4,
                @CustomerID = @P5, 
                @SupplierID = @P6";
            return TABS.DataHelper.GetDataTable
                (sql,
                top,
                sys_CDR_Pricing_CDRID,
                fromDate,
                tillDate.Value.AddDays(1),
                selectedCustomerID,
                selectedSupplierID);
        }

        public static List<ITableRow> Format_MismappedData(DataTable dt)
        {
            WebHelperLibrary.DatatableAsIEnumerable result = new WebHelperLibrary.DatatableAsIEnumerable(dt.Rows);
            return
                result.Select(r =>
                new MissMappedView3
                {
                    Carrier = r["Carrier"].ToString(),
                    Type = r["Type"].ToString(),
                } as ITableRow).ToList();
        }
        public static List<ITableRow> Format_MismappedCdrData(DataTable dt, bool grouped)
        {
            WebHelperLibrary.DatatableAsIEnumerable result = new WebHelperLibrary.DatatableAsIEnumerable(dt.Rows);
            if (grouped)
                return result.GroupBy(r =>
                   new
                   {
                       Day = DateTime.Parse(r["AttemptDateTime"].ToString()).ToString("yyyy-MM-dd"),
                       Customer = r["IN_CARRIER"].ToString(),
                       Supplier = r["OUT_CARRIER"].ToString(),
                       CDPN = r["CDPN"].ToString(),
                       CDPNOut = r["CDPNOut"].ToString()
                   }).Select(g => new MissMappedView1
                   {
                       Day = g.Key.Day,
                       Customer = string.IsNullOrEmpty(g.Key.Customer) ? "Undefined Carrier" : g.Key.Customer,
                       Supplier = string.IsNullOrEmpty(g.Key.Supplier) ? "Undefined Carrier" : g.Key.Supplier,
                       CDPN = g.Key.CDPN,
                       CDPNOut = g.Key.CDPNOut
                   } as ITableRow).ToList();
            return
                result.Select(r =>
                new MissMappedView1
                {
                    Day = DateTime.Parse(r["AttemptDateTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                    Customer = string.IsNullOrEmpty(r["IN_CARRIER"].ToString()) ? "Undefined Carrier" : r["IN_CARRIER"].ToString(),
                    Supplier = string.IsNullOrEmpty(r["OUT_CARRIER"].ToString()) ? "Undefined Carrier" : r["OUT_CARRIER"].ToString(),
                    CDPN = r["CDPN"].ToString(),
                    CDPNOut = r["CDPNOut"].ToString()
                } as ITableRow).ToList();
        }
        public static List<ITableRow> Format_MismappedOurZoneData(DataTable dt, bool grouped)
        {
            WebHelperLibrary.DatatableAsIEnumerable result = new WebHelperLibrary.DatatableAsIEnumerable(dt.Rows);
            if (grouped)
                return result.GroupBy(r =>
                   new
                   {
                       Day = DateTime.Parse(r["Attempt"].ToString()).ToString("yyyy-MM-dd"),
                       Supplier = r["SupplierID"].ToString(),
                       Customer = r["CustomerID"].ToString(),
                       SaleZone = r["OurZoneID"].ToString(),
                       CostZone = r["SupplierZoneID"].ToString(),
                       DurationInSeconds = r["DurationInSeconds"].ToString()
                   }).Select(g => new MissMappedView2
                   {
                       Day = g.Key.Day,
                       Customer = TABS.CarrierAccount.All.ContainsKey(g.Key.Customer) ? TABS.CarrierAccount.All[g.Key.Customer].Name : "Undefined Customer",
                       Supplier = TABS.CarrierAccount.All.ContainsKey(g.Key.Supplier) ? TABS.CarrierAccount.All[g.Key.Supplier].Name : "Undefined Supplier",
                       SaleZone = string.IsNullOrEmpty(g.Key.SaleZone) ? "Undefined Zone" : (TABS.Zone.OwnZones.ContainsKey(int.Parse(g.Key.SaleZone)) ? TABS.Zone.OwnZones[int.Parse(g.Key.SaleZone)].Name : "Undefined Zone"),
                       CostZone = string.IsNullOrEmpty(g.Key.CostZone) ? "Undefined Zone" : (TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(g.Key.CostZone)) != null ? TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(g.Key.CostZone)).Name : "Undefined Zone"),
                       DurationInSeconds = string.IsNullOrEmpty(g.Key.DurationInSeconds) ? "Undefined Duration" : g.Key.DurationInSeconds
                   } as ITableRow).ToList();
            return
                result.Select(r =>
                new MissMappedView2
                {
                    Day = DateTime.Parse(r["Attempt"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                    Customer = TABS.CarrierAccount.All.ContainsKey(r["CustomerID"].ToString()) ? TABS.CarrierAccount.All[r["CustomerID"].ToString()].Name : "Undefined Customer",
                    Supplier = TABS.CarrierAccount.All.ContainsKey(r["SupplierID"].ToString()) ? TABS.CarrierAccount.All[r["SupplierID"].ToString()].Name : "Undefined Supplier",
                    SaleZone = string.IsNullOrEmpty(r["OurZoneID"] == null ? null : r["OurZoneID"].ToString()) ? "Undefined Zone" : (TABS.Zone.OwnZones.ContainsKey(int.Parse(r["OurZoneID"].ToString())) ? TABS.Zone.OwnZones[int.Parse(r["OurZoneID"].ToString())].Name : "Undefined Zone"),
                    CostZone = string.IsNullOrEmpty(r["SupplierZoneID"] == null ? null : r["SupplierZoneID"].ToString()) ? "Undefined Zone" : (TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(r["SupplierZoneID"].ToString())) != null ? TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(r["SupplierZoneID"].ToString())).Name : "Undefined Zone"),
                    DurationInSeconds = string.IsNullOrEmpty(r["DurationInSeconds"].ToString()) ? "Undefined Duration" : r["DurationInSeconds"].ToString()
                } as ITableRow).ToList();
        }
        public static List<ITableRow> Format_MismappedSupplierZoneData(DataTable dt, bool grouped)
        {
            WebHelperLibrary.DatatableAsIEnumerable result = new WebHelperLibrary.DatatableAsIEnumerable(dt.Rows);
            if (grouped)
                return result.GroupBy(r =>
               new
               {
                   Day = DateTime.Parse(r["Attempt"].ToString()).ToString("yyyy-MM-dd"),
                   Supplier = r["SupplierID"].ToString(),
                   Customer = r["CustomerID"].ToString(),
                   SaleZone = r["OurZoneID"].ToString(),
                   CostZone = r["SupplierZoneID"].ToString(),
                   DurationInSeconds = r["DurationInSeconds"].ToString()
               }).Select(g => new MissMappedView2
               {
                   Day = g.Key.Day,
                   Customer = TABS.CarrierAccount.All.ContainsKey(g.Key.Customer) ? TABS.CarrierAccount.All[g.Key.Customer].Name : "Undefined Customer",
                   Supplier = TABS.CarrierAccount.All.ContainsKey(g.Key.Supplier) ? TABS.CarrierAccount.All[g.Key.Supplier].Name : "Undefined Supplier",
                   SaleZone = string.IsNullOrEmpty(g.Key.SaleZone) ? "Undefined Zone" : (TABS.Zone.OwnZones.ContainsKey(int.Parse(g.Key.SaleZone)) ? TABS.Zone.OwnZones[int.Parse(g.Key.SaleZone)].Name : "Undefined Zone"),
                   CostZone = string.IsNullOrEmpty(g.Key.CostZone) ? "Undefined Zone" : (TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(g.Key.CostZone)) != null ? TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(g.Key.CostZone)).Name : "Undefined Zone"),
                   DurationInSeconds = string.IsNullOrEmpty(g.Key.DurationInSeconds) ? "Undefined Duration" : g.Key.DurationInSeconds
               } as ITableRow).ToList();
            return result.Select(r =>
                new MissMappedView2
                {
                    Day = DateTime.Parse(r["Attempt"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                    Customer = TABS.CarrierAccount.All.ContainsKey(r["CustomerID"].ToString()) ? TABS.CarrierAccount.All[r["CustomerID"].ToString()].Name : "Undefined Customer",
                    Supplier = TABS.CarrierAccount.All.ContainsKey(r["SupplierID"].ToString()) ? TABS.CarrierAccount.All[r["SupplierID"].ToString()].Name : "Undefined Supplier",
                    //SaleZone = string.IsNullOrEmpty(r["OurZoneID"] == null ? null : r["OurZoneID"].ToString()) ? r["CDPN"].ToString() : (TABS.Zone.OwnZones.ContainsKey(int.Parse(r["OurZoneID"].ToString())) ? TABS.Zone.OwnZones[int.Parse(r["OurZoneID"].ToString())].Name : r["CDPN"].ToString()),
                    SaleZone = string.IsNullOrEmpty(r["OurZoneID"] == null ? null : r["OurZoneID"].ToString()) ? "Undefined Zone" : (TABS.Zone.OwnZones.ContainsKey(int.Parse(r["OurZoneID"].ToString())) ? TABS.Zone.OwnZones[int.Parse(r["OurZoneID"].ToString())].Name : "Undefined Zone"),
                    CostZone = string.IsNullOrEmpty(r["SupplierZoneID"] == null ? null : r["SupplierZoneID"].ToString()) ? "Undefined Zone" : (TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(r["SupplierZoneID"].ToString())) != null ? TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(r["SupplierZoneID"].ToString())).Name : "Undefined Zone"),
                    //CostZone = string.IsNullOrEmpty(r["SupplierZoneID"] == null ? null : r["SupplierZoneID"].ToString()) ? r["CDPN"].ToString() : (TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(r["SupplierZoneID"].ToString())) != null ? TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(r["SupplierZoneID"].ToString())).Name : r["CDPN"].ToString()),
                    DurationInSeconds = string.IsNullOrEmpty(r["DurationInSeconds"].ToString()) ? "Undefined Duration" : r["DurationInSeconds"].ToString()
                } as ITableRow).ToList();
        }
        public static List<ITableRow> Format_MismappedCDPNData(DataTable dt, bool grouped)
        {
            WebHelperLibrary.DatatableAsIEnumerable result = new WebHelperLibrary.DatatableAsIEnumerable(dt.Rows);
            if (grouped)
                return result.GroupBy(r =>
               new
               {
                   Day = DateTime.Parse(r["Attempt"].ToString()).ToString("yyyy-MM-dd"),
                   Supplier = r["SupplierID"].ToString(),
                   Customer = r["CustomerID"].ToString(),
                   CDPN = r["CDPN"].ToString(),
                   CDPNOut = r["CDPNOut"].ToString()
               }).Select(g => new MissMappedView1
               {
                   Day = g.Key.Day,
                   Customer = TABS.CarrierAccount.All.ContainsKey(g.Key.Customer) ? TABS.CarrierAccount.All[g.Key.Customer].Name : "Undefined Customer",
                   Supplier = TABS.CarrierAccount.All.ContainsKey(g.Key.Supplier) ? TABS.CarrierAccount.All[g.Key.Supplier].Name : "Undefined Supplier",
                   CDPN = g.Key.CDPN,
                   CDPNOut = g.Key.CDPNOut
               } as ITableRow).ToList();
            return result.Select(r =>
                new MissMappedView1
                {
                    Day = DateTime.Parse(r["Attempt"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                    Customer = TABS.CarrierAccount.All.ContainsKey(r["CustomerID"].ToString()) ? TABS.CarrierAccount.All[r["CustomerID"].ToString()].Name : "Undefined Customer",
                    Supplier = TABS.CarrierAccount.All.ContainsKey(r["SupplierID"].ToString()) ? TABS.CarrierAccount.All[r["SupplierID"].ToString()].Name : "Undefined Supplier",
                    CDPN = r["CDPN"].ToString(),
                    CDPNOut = r["CDPNOut"].ToString()
                } as ITableRow).ToList();
        }
        public static List<ITableRow> Format_MismappedSaleData(DataTable dt, bool grouped)
        {
            WebHelperLibrary.DatatableAsIEnumerable result = new WebHelperLibrary.DatatableAsIEnumerable(dt.Rows);
            if (grouped)
                return result.GroupBy(r =>
               new
               {
                   Day = DateTime.Parse(r["Attempt"].ToString()).ToString("yyyy-MM-dd"),
                   Supplier = r["SupplierID"].ToString(),
                   Customer = r["CustomerID"].ToString(),
                   SaleZone = r["OurZoneID"].ToString(),
                   CostZone = r["SupplierZoneID"].ToString(),
                   DurationInSeconds = r["DurationInSeconds"].ToString()
               }).Select(g => new MissMappedView2
               {
                   Day = g.Key.Day,
                   Customer = TABS.CarrierAccount.All.ContainsKey(g.Key.Customer) ? TABS.CarrierAccount.All[g.Key.Customer].Name : "Undefined Customer",
                   Supplier = TABS.CarrierAccount.All.ContainsKey(g.Key.Supplier) ? TABS.CarrierAccount.All[g.Key.Supplier].Name : "Undefined Supplier",
                   SaleZone = string.IsNullOrEmpty(g.Key.SaleZone) ? "Undefined Zone" : (TABS.Zone.OwnZones.ContainsKey(int.Parse(g.Key.SaleZone)) ? TABS.Zone.OwnZones[int.Parse(g.Key.SaleZone)].Name : "Undefined Zone"),
                   CostZone = string.IsNullOrEmpty(g.Key.CostZone) ? "Undefined Zone" : (TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(g.Key.CostZone)) != null ? TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(g.Key.CostZone)).Name : "Undefined Zone"),
                   DurationInSeconds = string.IsNullOrEmpty(g.Key.DurationInSeconds) ? "Undefined Duration" : g.Key.DurationInSeconds
               } as ITableRow).ToList();
            return result.Select(r =>
                new MissMappedView2
                {
                    Day = DateTime.Parse(r["Attempt"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                    Customer = TABS.CarrierAccount.All.ContainsKey(r["CustomerID"].ToString()) ? TABS.CarrierAccount.All[r["CustomerID"].ToString()].Name : "Undefined Customer",
                    Supplier = TABS.CarrierAccount.All.ContainsKey(r["SupplierID"].ToString()) ? TABS.CarrierAccount.All[r["SupplierID"].ToString()].Name : "Undefined Supplier",
                    SaleZone = string.IsNullOrEmpty(r["OurZoneID"] == null ? null : r["OurZoneID"].ToString()) ? r["CDPN"].ToString() : (TABS.Zone.OwnZones.ContainsKey(int.Parse(r["OurZoneID"].ToString())) ? TABS.Zone.OwnZones[int.Parse(r["OurZoneID"].ToString())].Name : "Undefined Zone"),
                    CostZone = string.IsNullOrEmpty(r["SupplierZoneID"] == null ? null : r["SupplierZoneID"].ToString()) ? "Undefined Zone" : (TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(r["SupplierZoneID"].ToString())) != null ? TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(r["SupplierZoneID"].ToString())).Name : "Undefined Zone"),

                    DurationInSeconds = string.IsNullOrEmpty(r["DurationInSeconds"].ToString()) ? "Undefined Duration" : r["DurationInSeconds"].ToString()
                } as ITableRow).ToList();
        }
        public static List<ITableRow> Format_MismappedCostData(DataTable dt, bool grouped)
        {
            WebHelperLibrary.DatatableAsIEnumerable result = new WebHelperLibrary.DatatableAsIEnumerable(dt.Rows);
            if (grouped)
                return result.GroupBy(r =>
               new
               {
                   Day = DateTime.Parse(r["Attempt"].ToString()).ToString("yyyy-MM-dd"),
                   Supplier = r["SupplierID"].ToString(),
                   Customer = r["CustomerID"].ToString(),
                   SaleZone = r["OurZoneID"].ToString(),
                   CostZone = r["SupplierZoneID"].ToString(),
                   DurationInSeconds = r["DurationInSeconds"].ToString()

               }).Select(g => new MissMappedView2
               {
                   Day = g.Key.Day,
                   Customer = TABS.CarrierAccount.All.ContainsKey(g.Key.Customer) ? TABS.CarrierAccount.All[g.Key.Customer].Name : "Undefined Customer",
                   Supplier = TABS.CarrierAccount.All.ContainsKey(g.Key.Supplier) ? TABS.CarrierAccount.All[g.Key.Supplier].Name : "Undefined Supplier",
                   SaleZone = string.IsNullOrEmpty(g.Key.SaleZone) ? "Undefined Zone" : (TABS.Zone.OwnZones.ContainsKey(int.Parse(g.Key.SaleZone)) ? TABS.Zone.OwnZones[int.Parse(g.Key.SaleZone)].Name : "Undefined Zone"),
                   CostZone = string.IsNullOrEmpty(g.Key.CostZone) ? "Undefined Zone" : (TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(g.Key.CostZone)) != null ? TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(g.Key.CostZone)).Name : "Undefined Zone"),
                   DurationInSeconds = string.IsNullOrEmpty(g.Key.DurationInSeconds) ? "Undefined Duration" : g.Key.DurationInSeconds
               } as ITableRow).ToList();
            return result.Select(r =>
                new MissMappedView2
                {
                    Day = DateTime.Parse(r["Attempt"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                    Customer = TABS.CarrierAccount.All.ContainsKey(r["CustomerID"].ToString()) ? TABS.CarrierAccount.All[r["CustomerID"].ToString()].Name : "Undefined Customer",
                    Supplier = TABS.CarrierAccount.All.ContainsKey(r["SupplierID"].ToString()) ? TABS.CarrierAccount.All[r["SupplierID"].ToString()].Name : "Undefined Supplier",
                    SaleZone = string.IsNullOrEmpty(r["OurZoneID"] == null ? null : r["OurZoneID"].ToString()) ? "Undefined Zone" : (TABS.Zone.OwnZones.ContainsKey(int.Parse(r["OurZoneID"].ToString())) ? TABS.Zone.OwnZones[int.Parse(r["OurZoneID"].ToString())].Name : "Undefined Zone"),
                    CostZone = string.IsNullOrEmpty(r["SupplierZoneID"] == null ? null : r["SupplierZoneID"].ToString()) ? "Undefined Zone" : (TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(r["SupplierZoneID"].ToString())) != null ? TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(r["SupplierZoneID"].ToString())).Name : "Undefined Zone"),
                    DurationInSeconds = string.IsNullOrEmpty(r["DurationInSeconds"].ToString()) ? "Undefined Duration" : r["DurationInSeconds"].ToString()
                } as ITableRow).ToList();
        }
        public static string DisplayAsTable(List<ITableRow> rows, string headerRow, string tableTitle)
        {
            if (rows.Count == 0)
                return string.Format("<h2>{0}</h2> <span style='color: green'><b>No errors found.</b></span>", tableTitle);
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("<h2>{0}</h2>", tableTitle));
            sb.Append("<table style='border: solid 1px black' border='1' bordercolor='#E0E0E0' cellspacing='1'>");
            sb.Append(headerRow);
            foreach (ITableRow row in rows)
            {
                sb.Append(row.GetTableRowString());
            }
            sb.Append("</table>");
            return sb.ToString();
        }

    }
}
