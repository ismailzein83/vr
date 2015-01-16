using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace TABS
{
    /// <summary>
    /// Route object mapped table Route.
    /// </summary>
    [Serializable]
    public class Route
    {
        public int RouteID { get; protected set; }
        public CarrierAccount Customer { get; set; }
        public String Code { get; set; }
        public Zone OurZone { get; set; }
        public double OurActiveRate { get; set; }
        public double OurNormalRate { get; set; }
        public double OurOffPeakRate { get; set; }
        public double OurWeekendRate { get; set; }
        public short OurServicesFlag { get; set; }
        public RouteState RouteState { get; set; }
        public DateTime Updated { get; set; }
        public bool IsToDAffected { get; set; }
        public bool IsSpecialRequestAffected { get; set; }
        public bool IsBlockAffected { get; set; }


        public static Dictionary<int, Route> GetRoutes(string query, params object[] parameters)
        {
            Dictionary<int, Route> routes = new Dictionary<int, Route>();

            using (IDataReader reader = DataHelper.ExecuteReader(query, parameters))
            {
                while (reader.Read())
                {
                    int index = 0;
                    Route route = new Route();
                    route.RouteID = reader.GetInt32(index);
                    index++; if (CarrierAccount.All.ContainsKey(reader.GetString(index))) route.Customer = CarrierAccount.All[reader.GetString(index)]; else { continue; }
                    index++; route.Code = reader.GetString(index);
                    index++; route.OurZone = Zone.OwnZones.ContainsKey(reader.GetInt32(index)) ? Zone.OwnZones[reader.GetInt32(index)] : null;
                    index++; route.OurServicesFlag = reader.GetInt16(index);
                    index++; route.OurActiveRate = reader.GetFloat(index);
                    index++; route.OurNormalRate = reader.GetFloat(index);
                    index++; route.OurOffPeakRate = reader.IsDBNull(index) ? 0 : reader.GetFloat(index);
                    index++; route.OurWeekendRate = reader.IsDBNull(index) ? 0 : reader.GetFloat(index);
                    index++; route.RouteState = (RouteState)(reader.GetByte(index));
                    index++; route.Updated = reader.GetDateTime(index);
                    index++; route.IsBlockAffected = reader.GetString(index).Equals("Y");
                    index++; route.IsSpecialRequestAffected = reader.GetString(index).Equals("Y");
                    index++; route.IsToDAffected = reader.GetString(index).Equals("Y");
                    routes[route.RouteID] = route;
                }
            }

            return routes;
        }

        public static Dictionary<int, Route> GetRoutes(string query, out int RecordCount, params object[] parameters)
        {
            Dictionary<int, Route> routes = new Dictionary<int, Route>();
            RecordCount = 0;
            using (IDataReader reader = DataHelper.ExecuteReader(query, parameters))
            {
                

                

                while (reader.Read())
                {
                    int index = 0;
                    Route route = new Route();
                    route.RouteID = reader.GetInt32(index);
                    index++; if (CarrierAccount.All.ContainsKey(reader.GetString(index))) route.Customer = CarrierAccount.All[reader.GetString(index)]; else { continue; }
                    index++; route.Code = reader.GetString(index);
                    index++; route.OurZone = Zone.OwnZones.ContainsKey(reader.GetInt32(index)) ? Zone.OwnZones[reader.GetInt32(index)] : null;
                    index++; route.OurServicesFlag = reader.GetInt16(index);
                    index++; route.OurActiveRate = reader.GetFloat(index);
                    index++; route.OurNormalRate = reader.GetFloat(index);
                    index++; route.OurOffPeakRate = reader.IsDBNull(index) ? 0 : reader.GetFloat(index);
                    index++; route.OurWeekendRate = reader.IsDBNull(index) ? 0 : reader.GetFloat(index);
                    index++; route.RouteState = (RouteState)(reader.GetByte(index));
                    index++; route.Updated = reader.GetDateTime(index);
                    index++; route.IsBlockAffected = reader.GetString(index).Equals("Y");
                    index++; route.IsSpecialRequestAffected = reader.GetString(index).Equals("Y");
                    index++; route.IsToDAffected = reader.GetString(index).Equals("Y");
                    routes[route.RouteID] = route;
                }

                reader.NextResult();

                if (reader.Read())
                    RecordCount = int.Parse(reader[0].ToString());
            }

            return routes;
        }

        public static Dictionary<int, Route> GetRoutes(CarrierAccount customer, CarrierAccount supplier, string code, string zone, int topValues, bool IsRouteOverrideAffected)
        {
            return GetRoutes(customer, supplier, code, zone, topValues, IsRouteOverrideAffected, null, null);
        }

        public static Dictionary<int, Route> GetRoutes(CarrierAccount customer, CarrierAccount supplier, string code, string zone, int topValues, bool IsRouteOverrideAffected, bool? IsBlocked)
        {
            return GetRoutes(customer, supplier, code, zone, topValues, IsRouteOverrideAffected, IsBlocked, null);
        }

        public static Dictionary<int, Route> GetRoutes(CarrierAccount customer, CarrierAccount supplier, string code, string zone, int topValues, bool IsRouteOverrideAffected, bool? IsBlocked, bool? ShowRoutesWithNoOptions)
        {
            StringBuilder SQL = new StringBuilder();

            SQL.AppendFormat(@"SELECT TOP {0}
                                    RT.RouteID,
                                    RT.CustomerID,
                                    RT.Code,	
                                    RT.OurZoneID ,
                                    RT.OurServicesFlag,
                                    RT.OurActiveRate,
                                    RT.OurNormalRate,
                                    RT.OurOffPeakRate,
                                    RT.OurWeekendRate,
                                    RT.State,                                    
                                    RT.Updated,
                                    RT.IsBlockAffected,
                                    RT.IsSpecialRequestAffected,
                                    RT.IsToDAffected
                                FROM   [Route] RT WITH(NOLOCK", topValues);

            if (!string.IsNullOrEmpty(zone))
                SQL.AppendFormat(",INDEX(IX_Route_Zone)) ");
            else
                SQL.AppendFormat(") ");

            if (IsRouteOverrideAffected)
                SQL.Append(" ,RouteOverride rov with(nolock) ");


            if (supplier != null && (ShowRoutesWithNoOptions == false || ShowRoutesWithNoOptions == null))
                SQL.AppendFormat(@" ,RouteOption ro WITH(NOLOCK)
                                    ,Zone sz with(nolock) ");

            if (!string.IsNullOrEmpty(zone))
                SQL.Append(@" ,Zone OZ with(nolock) ");

            SQL.Append(" WHERE 1=1 ");

            if (supplier != null && (ShowRoutesWithNoOptions == false || ShowRoutesWithNoOptions == null))
                SQL.AppendFormat(@" AND RT.RouteID = ro.RouteID
	                                AND ro.SupplierZoneID = sz.ZoneID
	                                AND sz.SupplierID = '{0}' ", supplier.CarrierAccountID);

            if (IsBlocked != null && IsBlocked == (bool?)false && (ShowRoutesWithNoOptions == false || ShowRoutesWithNoOptions == null))
                SQL.AppendFormat(@" AND ro.[State] = 1 ");

            if (IsRouteOverrideAffected)
                SQL.AppendFormat(@" AND rov.CustomerID = RT.CustomerID
                                    AND (RT.OurZoneID = rov.OurZoneID OR rov.Code = RT.Code)");

            if (!string.IsNullOrEmpty(zone))
                SQL.AppendFormat(@" AND OZ.ZoneID = RT.OurZoneID ");

            // Set parameters
            if (customer != null) SQL.AppendFormat(" AND RT.CustomerID = '{0}' ", customer.CarrierAccountID);
            if (!string.IsNullOrEmpty(zone)) SQL.AppendFormat(" AND OZ.Name LIKE '{0}'", zone.Replace("'", "''"));
            if (!string.IsNullOrEmpty(code)) SQL.AppendFormat(" AND RT.Code LIKE '{0}'", code);

            if (ShowRoutesWithNoOptions == true)
            {
                SQL.Append(" AND NOT EXISTS ( SELECT * FROM RouteOption rot WHERE rot.RouteID = RT.RouteID) ");
            }
            // Order by ?
            if (customer != null)
                SQL.Append(" ORDER BY RT.Code ");
            else
                SQL.Append(" ORDER BY RT.CustomerID, RT.Code ");

            return GetRoutes(SQL.ToString());
        }

        public static Dictionary<int, Route> GetRoutes(CarrierAccount customer, CarrierAccount supplier, string code, string zone, int topValues, bool IsRouteOverrideAffected, bool? IsBlocked, bool? ShowRoutesWithNoOptions,string TableName,int PageSize,out int RecordCount)
        {
            RecordCount = 0;
            StringBuilder SQL = new StringBuilder();

            SQL.AppendFormat(@"
                            Declare @exists bit                   
                              SET @exists=dbo.CheckGlobalTableExists('{1}')
                              
                              IF(@Exists = 1)
	                          BEGIN
		                           DROP TABLE TempDB.dbo.[{1}] 
	                          END  

                            SELECT TOP {0}
                                    RT.RouteID,
                                    RT.CustomerID,
                                    RT.Code,	
                                    RT.OurZoneID ,
                                    RT.OurServicesFlag,
                                    RT.OurActiveRate,
                                    RT.OurNormalRate,
                                    RT.OurOffPeakRate,
                                    RT.OurWeekendRate,
                                    RT.State,                                    
                                    RT.Updated,
                                    RT.IsBlockAffected,
                                    RT.IsSpecialRequestAffected,
                                    RT.IsToDAffected
                                INTO TempDB.dbo.[{1}]
                                FROM   [Route] RT WITH(NOLOCK", topValues,TableName);

            if (!string.IsNullOrEmpty(zone))
                SQL.AppendFormat(",INDEX(IX_Route_Zone)) ");
            else
                SQL.AppendFormat(") ");

            if (IsRouteOverrideAffected)
                SQL.Append(" ,RouteOverride rov with(nolock) ");


            if (supplier != null && (ShowRoutesWithNoOptions == false || ShowRoutesWithNoOptions == null))
                SQL.AppendFormat(@" ,RouteOption ro WITH(NOLOCK)
                                    ,Zone sz with(nolock) ");

            if (!string.IsNullOrEmpty(zone))
                SQL.Append(@" ,Zone OZ with(nolock) ");

            SQL.Append(" WHERE 1=1 ");

            if (supplier != null && (ShowRoutesWithNoOptions == false || ShowRoutesWithNoOptions == null))
                SQL.AppendFormat(@" AND RT.RouteID = ro.RouteID
	                                AND ro.SupplierZoneID = sz.ZoneID
	                                AND sz.SupplierID = '{0}' ", supplier.CarrierAccountID);

            if (IsBlocked != null && IsBlocked == (bool?)false && (ShowRoutesWithNoOptions == false || ShowRoutesWithNoOptions == null))
                SQL.AppendFormat(@" AND ro.[State] = 1 ");

            if (IsRouteOverrideAffected)
                SQL.AppendFormat(@" AND rov.CustomerID = RT.CustomerID
                                    AND (RT.OurZoneID = rov.OurZoneID OR rov.Code = RT.Code)");

            if (!string.IsNullOrEmpty(zone))
                SQL.AppendFormat(@" AND OZ.ZoneID = RT.OurZoneID ");

            // Set parameters
            if (customer != null) SQL.AppendFormat(" AND RT.CustomerID = '{0}' ", customer.CarrierAccountID);
            if (!string.IsNullOrEmpty(zone)) SQL.AppendFormat(" AND OZ.Name LIKE '{0}'", zone.Replace("'", "''"));
            if (!string.IsNullOrEmpty(code)) SQL.AppendFormat(" AND RT.Code LIKE '{0}'", code);

            if (ShowRoutesWithNoOptions == true)
            {
                SQL.Append(" AND NOT EXISTS ( SELECT * FROM RouteOption rot WHERE rot.RouteID = RT.RouteID) ");
            }
            // Order by ?
            if (customer != null)
                SQL.Append(" ORDER BY RT.Code ");
            else
                SQL.Append(" ORDER BY RT.CustomerID, RT.Code ");

            SQL.AppendFormat(@"SELECT COUNT(1) FROM TempDB.dbo.[{0}];
                          WITH FINAL AS 
                            (
                               select *,ROW_NUMBER()  OVER ( ORDER BY {2} ) AS rowNumber
                               from TempDB.dbo.[{0}]
                             )
                          SELECT * FROM FINAL WHERE RowNumber BETWEEN 1 AND {1}", TableName, PageSize, customer != null ? "Code" : "CustomerID, Code ");


            Dictionary<int, Route> routes = new Dictionary<int, Route>();

            using (IDataReader reader = DataHelper.ExecuteReader(SQL.ToString()))
            {
                if (reader.Read())
                    RecordCount = int.Parse(reader[0].ToString());

                reader.NextResult();


                while (reader.Read())
                {
                    int index = 0;
                    Route route = new Route();
                    route.RouteID = reader.GetInt32(index);
                    index++; if (CarrierAccount.All.ContainsKey(reader.GetString(index))) route.Customer = CarrierAccount.All[reader.GetString(index)]; else { continue; }
                    index++; route.Code = reader.GetString(index);
                    index++; route.OurZone = Zone.OwnZones.ContainsKey(reader.GetInt32(index)) ? Zone.OwnZones[reader.GetInt32(index)] : null;
                    index++; route.OurServicesFlag = reader.GetInt16(index);
                    index++; route.OurActiveRate = reader.GetFloat(index);
                    index++; route.OurNormalRate = reader.GetFloat(index);
                    index++; route.OurOffPeakRate = reader.IsDBNull(index) ? 0 : reader.GetFloat(index);
                    index++; route.OurWeekendRate = reader.IsDBNull(index) ? 0 : reader.GetFloat(index);
                    index++; route.RouteState = (RouteState)(reader.GetByte(index));
                    index++; route.Updated = reader.GetDateTime(index);
                    index++; route.IsBlockAffected = reader.GetString(index).Equals("Y");
                    index++; route.IsSpecialRequestAffected = reader.GetString(index).Equals("Y");
                    index++; route.IsToDAffected = reader.GetString(index).Equals("Y");
                    routes[route.RouteID] = route;
                }
            }

            return routes;
        }


        //        public static Dictionary<int, Route> GetRoutesWithNoOptions(CarrierAccount customer, string code, string zone, int topValues, bool IsRouteOverrideAffected)
        //        {
        //            StringBuilder SQL = new StringBuilder();

        //            SQL.AppendFormat(@"SELECT TOP {0}
        //                                    RT.RouteID,
        //                                    RT.CustomerID,
        //                                    RT.Code,	
        //                                    RT.OurZoneID ,
        //                                    RT.OurServicesFlag,
        //                                    RT.OurActiveRate,
        //                                    RT.OurNormalRate,
        //                                    RT.OurOffPeakRate,
        //                                    RT.OurWeekendRate,
        //                                    RT.State,                                    
        //                                    RT.Updated,
        //                                    RT.IsBlockAffected,
        //                                    RT.IsSpecialRequestAffected,
        //                                    RT.IsToDAffected
        //                                FROM   [Route] RT WITH(NOLOCK", topValues);

        //            if (!string.IsNullOrEmpty(zone))
        //                SQL.AppendFormat(",INDEX(IX_Route_Zone)) ");
        //            else
        //                SQL.AppendFormat(") ");

        //            if (IsRouteOverrideAffected)
        //                SQL.Append(" ,RouteOverride rov with(nolock) ");
        //        }



        public static Route GetRouteByID(int RouteID)
        {
            Route route = new Route();
            string SQL = string.Format(@"SELECT 
                                    RT.RouteID,
                                    RT.CustomerID,
                                    RT.Code,	
                                    RT.OurZoneID ,
                                    RT.OurServicesFlag,
                                    RT.OurActiveRate,
                                    RT.OurNormalRate,
                                    RT.OurOffPeakRate,
                                    RT.OurWeekendRate,
                                    RT.State,                                    
                                    RT.Updated,
                                    RT.IsBlockAffected,
                                    RT.IsSpecialRequestAffected,
                                    RT.IsToDAffected
                                FROM [Route] RT WITH (NOLOCK) WHERE RT.RouteID ={0}", RouteID);

            using (IDataReader reader = DataHelper.ExecuteReader(SQL))
            {
                while (reader.Read())
                {
                    int index = 0;
                    route.RouteID = reader.GetInt32(index);
                    index++; route.Customer = CarrierAccount.All[reader.GetString(index)];
                    index++; route.Code = reader.GetString(index);
                    index++; route.OurZone = Zone.OwnZones[reader.GetInt32(index)];
                    index++; route.OurServicesFlag = reader.GetInt16(index);
                    index++; route.OurActiveRate = reader.GetFloat(index);
                    index++; route.OurNormalRate = reader.GetFloat(index);
                    index++; route.OurOffPeakRate = reader.IsDBNull(index) ? 0 : reader.GetFloat(index);
                    index++; route.OurWeekendRate = reader.IsDBNull(index) ? 0 : reader.GetFloat(index);
                    index++; route.RouteState = (RouteState)(reader.GetByte(index));
                    index++; route.Updated = reader.GetDateTime(index);
                    index++; route.IsBlockAffected = reader.GetString(index).Equals("Y");
                    index++; route.IsSpecialRequestAffected = reader.GetString(index).Equals("Y");
                    index++; route.IsToDAffected = reader.GetString(index).Equals("Y");
                }
            }
            return route;
        }
    }
}