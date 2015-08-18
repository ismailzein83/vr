using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.BusinessEntity.Business;

namespace TOne.Analytics.Data.SQL
{
   public class TrafficStatisticCommon
    {
      public string GetTableName(IEnumerable<TrafficStatisticGroupKeys> groupKeys, GenericFilter filter)
       {
           foreach (var groupKey in groupKeys)
           {
               if (groupKey == TrafficStatisticGroupKeys.CodeBuy || groupKey == TrafficStatisticGroupKeys.CodeSales)
                   return "TrafficStatsByCode ts WITH(NOLOCK ,INDEX(IX_TrafficStatsByCode_DateTimeFirst))";
           }
           if (filter.CodeSales != null && filter.CodeSales.Count > 0 || filter.CodeSales != null && filter.CodeSales.Count > 0)
               return "TrafficStatsByCode ts WITH(NOLOCK ,INDEX(IX_TrafficStatsByCode_DateTimeFirst))";
           return "TrafficStats ts WITH(NOLOCK ,INDEX(IX_TrafficStats_DateTimeFirst)) ";
       }
      public void AddFilterToQuery(GenericFilter filter, StringBuilder whereBuilder, HashSet<string> joinStatement)
       {
           AddFilter(whereBuilder, filter.SwitchIds, "ts.SwitchID");
           AddFilter(whereBuilder, filter.CustomerIds, "ts.CustomerID");
           AddFilter(whereBuilder, filter.SupplierIds, "ts.SupplierID");
           if (filter.CodeGroups != null && filter.CodeGroups.Count > 0)
           {
               joinStatement.Add(GetJoinQuery(TrafficStatisticGroupKeys.OurZone));
               AddFilter(whereBuilder, filter.CodeGroups, "z.CodeGroup");

           }

           AddFilter(whereBuilder, filter.PortIn, "ts.Port_IN");
           AddFilter(whereBuilder, filter.PortOut, "ts.Port_OUT");
           AddFilter(whereBuilder, filter.ZoneIds, "ts.OurZoneID");
           AddFilter(whereBuilder, filter.SupplierZoneId, "ts.SupplierZoneID");
           if (filter.GateWayIn != null && filter.GateWayIn.Count > 0)
           {
               joinStatement.Add(GetJoinQuery(TrafficStatisticGroupKeys.GateWayIn));
               AddFilter(whereBuilder, filter.GateWayIn, "cscIn.GateWayName");

           }
           if (filter.GateWayOut != null && filter.GateWayOut.Count > 0)
           {
               joinStatement.Add(GetJoinQuery(TrafficStatisticGroupKeys.GateWayOut));
               AddFilter(whereBuilder, filter.GateWayOut, "cscOut.GateWayName");

           }
           AddFilter(whereBuilder, filter.CodeBuy, "ts.SupplierCode");
           AddFilter(whereBuilder, filter.CodeSales, "ts.OurCode");

       }
      public void AddFilter<T>(StringBuilder whereBuilder, IEnumerable<T> values, string column)
       {
           if (values != null && values.Count() > 0)
           {
               if (typeof(T) == typeof(string))
                   whereBuilder.AppendFormat("AND {0} IN ('{1}')", column, String.Join("', '", values));
               else
                   whereBuilder.AppendFormat("AND {0} IN ({1})", column, String.Join(", ", values));
           }
       }
      public string GetColumnFilter(TrafficStatisticGroupKeys column, string columnFilterValue)
      {
          switch (column)
          {
              case TrafficStatisticGroupKeys.OurZone:
                  return String.Format("{0} = '{1}'", OurZoneIDColumnName, columnFilterValue);
              case TrafficStatisticGroupKeys.CustomerId:
                  return String.Format("{0} = '{1}'", CustomerIDColumnName, columnFilterValue);
              case TrafficStatisticGroupKeys.SupplierId:
                  return String.Format("{0} = '{1}'", SupplierIDColumnName, columnFilterValue);
              case TrafficStatisticGroupKeys.Switch:
                  return String.Format("{0} = '{1}'", SwitchIdColumnName, columnFilterValue);
              case TrafficStatisticGroupKeys.PortIn:
                  return String.Format("{0} = '{1}'", Port_INColumnName, columnFilterValue);
              case TrafficStatisticGroupKeys.PortOut:
                  return String.Format("{0} = '{1}'", Port_OutColumnName, columnFilterValue);
              case TrafficStatisticGroupKeys.CodeGroup:
                  return String.Format("{0} = '{1}'", CodeGroupIDColumnName, columnFilterValue);
              case TrafficStatisticGroupKeys.SupplierZoneId:
                  return String.Format("{0} = '{1}'", SupplierZoneIDColumnName, columnFilterValue);
              case TrafficStatisticGroupKeys.GateWayIn:
                  return String.Format("{0} = '{1}'", GateWayInIDColumnName, columnFilterValue);
              case TrafficStatisticGroupKeys.GateWayOut:
                  return String.Format("{0} = '{1}'", GateWayOutIDColumnName, columnFilterValue);
              case TrafficStatisticGroupKeys.CodeSales:
                  return String.Format("{0} = '{1}'", CodeSalesIDColumnName, columnFilterValue);
              case TrafficStatisticGroupKeys.CodeBuy:
                  return String.Format("{0} = '{1}'", CodeBuyIDColumnName, columnFilterValue);
              default: return null;
          }
      }
      public void FillBEProperties<T>(GenericSummaryBigResult<T> Data, TrafficStatisticGroupKeys[] groupKeys)
      {
          BusinessEntityInfoManager manager = new BusinessEntityInfoManager();

          foreach (GroupSummary<T> data in Data.Data)
          {

              for (int i = 0; i < groupKeys.Length; i++)
              {
                  TrafficStatisticGroupKeys groupKey = groupKeys[i];
                  string Id = data.GroupKeyValues[i].Id;
                  switch (groupKey)
                  {
                      case TrafficStatisticGroupKeys.OurZone:
                          if (Id != "N/A")
                              data.GroupKeyValues[i].Name = manager.GetZoneName(Convert.ToInt32(Id));
                          break;

                      case TrafficStatisticGroupKeys.CustomerId:
                          if (Id != "N/A")
                              data.GroupKeyValues[i].Name = manager.GetCarrirAccountName(Id);
                          break;

                      case TrafficStatisticGroupKeys.SupplierId:
                          if (Id != "N/A")
                              data.GroupKeyValues[i].Name = manager.GetCarrirAccountName(Id);
                          break;
                      case TrafficStatisticGroupKeys.Switch:
                          if (Id != "N/A")
                              data.GroupKeyValues[i].Name = manager.GetSwitchName(Convert.ToInt32(Id));
                          break;
                      case TrafficStatisticGroupKeys.SupplierZoneId:
                          if (Id != "N/A")
                              data.GroupKeyValues[i].Name = manager.GetZoneName(Convert.ToInt32(Id)); break;
                      default: break;
                  }

              }

          }
      }
      public string GetConstName(TrafficStatisticGroupKeys column)
      {
          switch (column)
          {
              case TrafficStatisticGroupKeys.OurZone:
                  return OurZoneIDColumnName;
              case TrafficStatisticGroupKeys.CustomerId:
                  return CustomerIDColumnName;
              case TrafficStatisticGroupKeys.SupplierId:
                  return SupplierIDColumnName;
              case TrafficStatisticGroupKeys.Switch:
                  return SwitchIdColumnName;
              case TrafficStatisticGroupKeys.PortIn:
                  return Port_INColumnName;
              case TrafficStatisticGroupKeys.PortOut:
                  return Port_OutColumnName;
              case TrafficStatisticGroupKeys.CodeGroup:
                  return CodeGroupNameColumnName;
              case TrafficStatisticGroupKeys.SupplierZoneId:
                  return SupplierZoneIDColumnName;
              case TrafficStatisticGroupKeys.GateWayIn:
                  return GateWayInIDColumnName;
              case TrafficStatisticGroupKeys.GateWayOut:
                  return GateWayOutIDColumnName;
              case TrafficStatisticGroupKeys.CodeSales:
                  return CodeSalesIDColumnName;
              case TrafficStatisticGroupKeys.CodeBuy:
                  return CodeBuyIDColumnName;
              default: return null;
          }
      }
      public void GetColumnStatements(TrafficStatisticGroupKeys groupKey, out string columnName, HashSet<string> joinStatement, out string groupByStatement)
      {
          GetColumnNames(groupKey, out columnName);
          switch (groupKey)
          {
              case TrafficStatisticGroupKeys.OurZone:
                  joinStatement = null;
                  groupByStatement = null;
                  break;
              case TrafficStatisticGroupKeys.CustomerId:
                  joinStatement = null;
                  groupByStatement = null;
                  break;

              case TrafficStatisticGroupKeys.SupplierId:
                  joinStatement = null;
                  groupByStatement = null;
                  break;
              case TrafficStatisticGroupKeys.Switch:
                  columnName = String.Format("ts.SwitchID as {0}", GetConstName(TrafficStatisticGroupKeys.Switch));
                  groupByStatement = "ts.SwitchID";
                  joinStatement = null;
                  break;
              case TrafficStatisticGroupKeys.CodeGroup:
                  columnName = String.Format("z.CodeGroup as {0}, c.Name {1}", CodeGroupIDColumnName, GetConstName(TrafficStatisticGroupKeys.CodeGroup));
                  joinStatement.Add(String.Format("{0} LEFT JOIN CodeGroup c ON z.CodeGroup=c.Code", OurZonesJoinQuery));
                  groupByStatement = "z.CodeGroup, c.Name";
                  break;
              case TrafficStatisticGroupKeys.PortIn:
                  joinStatement = null;
                  groupByStatement = null;
                  break;
              case TrafficStatisticGroupKeys.PortOut:
                  joinStatement = null;
                  groupByStatement = null;
                  break;
              case TrafficStatisticGroupKeys.SupplierZoneId:
                  joinStatement = null;
                  groupByStatement = null;
                  break;
              case TrafficStatisticGroupKeys.GateWayIn:
                  joinStatement.Add(GateWayInJoinQuery);
                  columnName = String.Format("cscIn.GateWayName as {0}", GetConstName(TrafficStatisticGroupKeys.GateWayIn));
                  groupByStatement = "cscIn.GateWayName";
                  break;
              case TrafficStatisticGroupKeys.GateWayOut:
                  joinStatement.Add(GateWayOutJoinQuery);
                  columnName = String.Format("cscOut.GateWayName as {0}", GetConstName(TrafficStatisticGroupKeys.GateWayOut));
                  groupByStatement = "cscOut.GateWayName";
                  break;
              case TrafficStatisticGroupKeys.CodeBuy:
                  joinStatement = null;
                  groupByStatement = null;
                  break;
              case TrafficStatisticGroupKeys.CodeSales:
                  joinStatement = null;
                  groupByStatement = null;
                  break;
              default:
                  columnName = null;
                  groupByStatement = null;
                  break;
          }
      }
      public string GetJoinQuery(TrafficStatisticGroupKeys column)
      {
          switch (column)
          {
              case TrafficStatisticGroupKeys.OurZone:
                  return OurZonesJoinQuery;
              case TrafficStatisticGroupKeys.GateWayIn:
                  return GateWayInJoinQuery;
              case TrafficStatisticGroupKeys.GateWayOut:
                  return GateWayOutJoinQuery;
              
              default: return null;
          }
      }
      public string GetNeededStatment(GenericFilter filter, IEnumerable<TrafficStatisticGroupKeys> groupKeys)
      {
          StringBuilder neededSelectStatement = new StringBuilder();
          foreach (TrafficStatisticGroupKeys groupKey in groupKeys)
          {
              if (groupKey == TrafficStatisticGroupKeys.SupplierId || groupKey == TrafficStatisticGroupKeys.PortOut || groupKey == TrafficStatisticGroupKeys.GateWayOut || filter.SupplierIds.Count != 0)
              {
                  neededSelectStatement.Append(" Case WHEN SUM(ts.Attempts)>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.SuccessfulAttempts)*100.0/Sum(ts.Attempts)) ELSE 0 END AS ABR ");
                  neededSelectStatement.Append(", Case WHEN (Sum(ts.Attempts)-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.SuccessfulAttempts)*100.0/(Sum(ts.Attempts)-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))) ELSE 0 END AS ASR ");
                  neededSelectStatement.Append(" ,Case WHEN (Sum(ts.Attempts)-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.DeliveredNumberOfCalls)*100.0/(Sum(ts.Attempts)-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))) ELSE 0 END AS NER ");
                  return neededSelectStatement.ToString();
              }

          }

          neededSelectStatement.Append(" Case WHEN SUM(ts.NumberOfCalls)>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.SuccessfulAttempts)*100.0/Sum(ts.NumberOfCalls)) ELSE 0 END AS ABR ");
          neededSelectStatement.Append(" ,Case WHEN (Sum(ts.NumberOfCalls)-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.SuccessfulAttempts)*100.0/(Sum(ts.NumberOfCalls)-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))) ELSE 0 END AS ASR ");
          neededSelectStatement.Append(" ,Case WHEN (Sum(ts.NumberOfCalls)-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.DeliveredNumberOfCalls)*100.0/(Sum(ts.NumberOfCalls)-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))) ELSE 0 END AS NER ");
          return neededSelectStatement.ToString();
      }
      public void GetColumnNames(TrafficStatisticGroupKeys column, out string idColumn)
      {
          switch (column)
          {
              case TrafficStatisticGroupKeys.OurZone:
                  idColumn = OurZoneIDColumnName;
                  break;
              case TrafficStatisticGroupKeys.CustomerId:
                  idColumn = CustomerIDColumnName;
                  break;

              case TrafficStatisticGroupKeys.SupplierId:
                  idColumn = SupplierIDColumnName;
                  break;
              case TrafficStatisticGroupKeys.Switch:
                  idColumn = SwitchIdColumnName;
                  break;
              case TrafficStatisticGroupKeys.PortIn:
                  idColumn = Port_INColumnName;
                  break;
              case TrafficStatisticGroupKeys.PortOut:
                  idColumn = Port_OutColumnName;
                  break;
              case TrafficStatisticGroupKeys.CodeGroup:
                  idColumn = CodeGroupIDColumnName;
                  break;
              case TrafficStatisticGroupKeys.SupplierZoneId:
                  idColumn = SupplierZoneIDColumnName;
                  break;
              case TrafficStatisticGroupKeys.GateWayIn:
                  idColumn = GateWayInIDColumnName;
                  break;
              case TrafficStatisticGroupKeys.GateWayOut:
                  idColumn = GateWayOutIDColumnName;
                  break;
              case TrafficStatisticGroupKeys.CodeSales:
                  idColumn = CodeSalesIDColumnName;
                  break;
              case TrafficStatisticGroupKeys.CodeBuy:
                  idColumn = CodeBuyIDColumnName;
                  break;
              default:
                  idColumn = null;
                  break;
          }
      }
      #region Constant
        public const string SwitchIdColumnName = "SwitchID";
        public const string OurZoneIDColumnName = "OurZoneID";
        public const string CustomerIDColumnName = "CustomerID";
        public const string SupplierIDColumnName = "SupplierID";
        public const string Port_INColumnName = "Port_IN";
        public const string Port_OutColumnName = "Port_OUT";
        public const string CodeGroupNameColumnName = "CodeGroupName";
        public const string SupplierZoneIDColumnName = "SupplierZoneID";
        public const string GateWayInIDColumnName = "GateWayInName";
        public const string GateWayOutIDColumnName = "GateWayOutName";
        public const string CodeGroupIDColumnName = "CodeGroup";
        public const string CodeBuyIDColumnName = "SupplierCode";
        public const string CodeSalesIDColumnName = "OurCode";
        public const string OurZonesJoinQuery = " LEFT JOIN  OurZones z ON ts.OurZoneID = z.ZoneID";
        public const string GateWayInJoinQuery = "Left JOIN SwitchConnectivity cscIn  ON (','+cscIn.Details+',' LIKE '%,'+ts.Port_IN +',%' ) AND(ts.SwitchID = cscIn.SwitchID) AND ts.CustomerID =cscIn.CarrierAccount ";
        public const string GateWayOutJoinQuery = "Left JOIN SwitchConnectivity cscOut ON  (','+cscOut.Details+',' LIKE '%,'+ts.Port_OUT +',%') AND (ts.SwitchID = cscOut.SwitchID)  AND ts.SupplierID  =cscOut.CarrierAccount  ";
      #endregion
    }
}
