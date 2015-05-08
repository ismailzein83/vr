using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace TABS
{
    public class ADOObjectAssembler
    {


        private Double ConvertDateTimeToDouble(string ConvertDt)
        {
            // convert DateTime to Double
            string st = String.Format("{0:yyyy-MM-dd}", ConvertDt);
            // dt is now a .NET date time
            DateTime dt = DateTime.Parse(st.ToString());
            // convert double
            double d = Convert.ToDouble(dt.ToOADate());
            return d;
        }

        public Dictionary<int, Zone> GetOwnZones(DateTime when)
        {
            Dictionary<int, Zone> zones = new Dictionary<int, Zone>(2000);
            try
            {

                List<Zone> zoneList = Get_OwnZones(when);
                foreach (Zone zone in zoneList)
                {
                    zones[zone.ZoneID] = zone;
                    zone.EffectiveCodes = new List<Code>();
                }

                List<Code> codes = Get_OwnCodes(when);
                foreach (Code code in codes)
                    if (code.Zone != null)
                    {
                        Zone zone = null;
                        if (zones.TryGetValue(code.Zone.ZoneID, out zone))
                            zone.EffectiveCodes.Add(code);
                    }
                zoneList = null;
                codes = null;
                return zones;
            }
            catch (Exception EX)
            {
                throw new Exception("---: " + EX.Message);
            }
        }


        public static Dictionary<int, TABS.Switch> GetSwitchs()
        {
            //Dictionary<CarrierAccount, List<Zone>> _DICAllEffectedZones = AllEffectiveSupplierZonesDictionary(whenEffective, _CachedData);
            Dictionary<int, TABS.Switch> results = new Dictionary<int, TABS.Switch>();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    SqlCommand comm = new SqlCommand(@"select SwitchID,Symbol,Name,Description,Configuration,LastCDRImportTag,LastImport,LastRouteUpdate ,[UserID],Enable_CDR_Import,Enable_Routing,[timestamp],LastAttempt,NominalTrunkCapacityInE1s,NominalVoipCapacityInE1s from Switch", conn);
                    comm.Connection = conn;
                    conn.Open();
                    SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    int index = -1;
                    while (reader.Read())
                    {
                        TABS.Switch S = new Switch();
                        index = -1;
                        index++; S.SwitchID = int.Parse(reader[index].ToString());
                        index++; S.Symbol = reader[index].ToString();
                        index++; S.Name = reader[index].ToString();
                        index++; S.Description = reader[index].ToString();
                        index++; S.Configuration.InnerXml = reader[index].ToString();
                        index++; S.LastCDRImportTag = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; S.LastImport = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; S.LastRouteUpdate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; index++; S.Enable_CDR_Import = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; S.Enable_Routing = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; index++; S.LastAttempt = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; S.NominalTrunkCapacityInE1s = int.Parse(reader[index].ToString());
                        index++; S.NominalVoipCapacityInE1s = int.Parse(reader[index].ToString());
                        results[S.SwitchID] = S;

                    }
                    conn.Close();
                }
                return results;
            }
            catch (Exception EX)
            {
                throw new Exception("MyObjectAssembler_New->GetSwitchs():- " + EX.Message);
            }

        }

        //        public Dictionary<CarrierAccount, List<Tariff>> GetAllSupplierTarrif(DateTime whenEffective, TABS.EffectiveCachedData _CachedData, Dictionary<CarrierAccount, List<Zone>> _DICAllEffectedZones)
        //        {
        //            //Dictionary<CarrierAccount, List<Zone>> _DICAllEffectedZones = AllEffectiveSupplierZonesDictionary(whenEffective, _CachedData);
        //            Dictionary<CarrierAccount, List<Tariff>> results = new Dictionary<CarrierAccount, List<Tariff>>();
        //            try
        //            {
        //                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
        //                conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
        //                SqlCommand comm = new SqlCommand(@"select TariffID ,ZoneID ,CustomerID ,SupplierID ,CallFee ,FirstPeriodRate ,FirstPeriod ,RepeatFirstPeriod ,FractionUnit 
        //                                            ,BeginEffectiveDate ,EndEffectiveDate from Tariff T WITH (NOLOCK) where T.BeginEffectiveDate <= '" + whenEffective.ToString("yyyy-MM-dd") + "' AND (T.EndEffectiveDate IS NULL OR T.EndEffectiveDate >= '" + whenEffective.ToString("yyyy-MM-dd") + "') AND T.CustomerID='SYS' ", conn);
        //                comm.Connection = conn;
        //                conn.Open();
        //                SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        //                int index = -1;
        //                int zoneID;
        //                CarrierAccount SYSTEM = GetSYSTEMCarrierAccounts(_CachedData.CarrierProfiles, _CachedData.CarrierGroups);
        //                while (reader.Read())
        //                {
        //                    Tariff T = new Tariff();
        //                    index = -1;
        //                    index++; T.TariffID = int.Parse(reader[index].ToString());
        //                    index++; zoneID = int.Parse(reader[index].ToString());

        //                    //index++; T.Customer = TABS.CarrierAccount.SYSTEM;
        //                    index++; T.Customer = SYSTEM;
        //                    index++; T.Supplier = _CachedData.Supplires.ContainsKey(reader[index].ToString()) ? _CachedData.Supplires[reader[index].ToString()] : null;
        //                    if (T.Supplier == null) continue;
        //                    T.Zone = _DICAllEffectedZones.ContainsKey(T.Supplier) ? _DICAllEffectedZones[T.Supplier].Where(z => z.Equals(zoneID)).FirstOrDefault() : null;
        //                    index++; T.CallFee = reader.IsDBNull(index) ? 0 : decimal.Parse(reader[index].ToString());
        //                    index++; T.FirstPeriodRate = reader.IsDBNull(index) ? 0 : decimal.Parse(reader[index].ToString());
        //                    index++; T.FirstPeriod = reader.IsDBNull(index) ? 0 : int.Parse(reader[index].ToString());
        //                    index++; T.RepeatFirstPeriod = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
        //                    index++; T.FractionUnit = reader.IsDBNull(index) ? 0 : int.Parse(reader[index].ToString());
        //                    index++; T.BeginEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
        //                    index++; T.EndEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
        //                    if (T.Supplier != null)
        //                        if (results.ContainsKey(T.Supplier))
        //                            results[T.Supplier].Add(T);
        //                        else
        //                        {
        //                            List<Tariff> temp = new List<Tariff>();
        //                            temp.Add(T);
        //                            results[T.Supplier] = temp;
        //                            temp = null;
        //                        }
        //                    T = null;
        //                }
        //                conn.Close();
        //                comm.Connection.Dispose();
        //                comm.Dispose();
        //                reader.Dispose();
        //                reader = null;
        //                return results;
        //            }
        //            catch (Exception EX)
        //            {
        //                throw new Exception("MyObjectAssembler_New->GetAllSupplierTarrif(param1,param2,param3):- " + EX.Message);
        //            }

        //        }

        //        public Dictionary<CarrierAccount, List<Tariff>> GetAllCustomerTarrif(DateTime whenEffective, TABS.EffectiveCachedData _CachedData, Dictionary<int, Zone> _OwnZones)
        //        {
        //            //Dictionary<int, Zone> _OwnZones = MyObjectAssembler_New.GetOwnZones(whenEffective);
        //            Dictionary<CarrierAccount, List<Tariff>> results = new Dictionary<CarrierAccount, List<Tariff>>();
        //            try
        //            {
        //                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
        //                conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
        //                SqlCommand comm = new SqlCommand(@"select TariffID ,ZoneID ,CustomerID ,SupplierID ,CallFee ,FirstPeriodRate ,FirstPeriod ,RepeatFirstPeriod ,FractionUnit 
        //                                            ,BeginEffectiveDate ,EndEffectiveDate from Tariff T WITH (NOLOCK) where T.BeginEffectiveDate <= '" + whenEffective.ToString("yyyy-MM-dd") + "' AND (T.EndEffectiveDate IS NULL OR T.EndEffectiveDate >= '" + whenEffective.ToString("yyyy-MM-dd") + "') AND T.SupplierID='SYS'", conn);
        //                comm.Connection = conn;
        //                conn.Open();
        //                SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        //                int index = -1;
        //                int zoneID;
        //                CarrierAccount SYSTEM = GetSYSTEMCarrierAccounts(_CachedData.CarrierProfiles, _CachedData.CarrierGroups);
        //                while (reader.Read())
        //                {
        //                    Tariff T = new Tariff();
        //                    index = -1;
        //                    index++; T.TariffID = int.Parse(reader[index].ToString());
        //                    index++; zoneID = int.Parse(reader[index].ToString());
        //                    T.Zone = _OwnZones.ContainsKey(zoneID) ? _OwnZones[zoneID] : null;
        //                    index++; T.Customer = _CachedData.Customers.ContainsKey(reader[index].ToString()) ? _CachedData.Customers[reader[index].ToString()] : null;
        //                    index++; T.Supplier = SYSTEM;// TABS.CarrierAccount.SYSTEM;
        //                    index++; T.CallFee = reader.IsDBNull(index) ? 0 : decimal.Parse(reader[index].ToString());
        //                    index++; T.FirstPeriodRate = reader.IsDBNull(index) ? 0 : decimal.Parse(reader[index].ToString());
        //                    index++; T.FirstPeriod = reader.IsDBNull(index) ? 0 : int.Parse(reader[index].ToString());
        //                    index++; T.RepeatFirstPeriod = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
        //                    index++; T.FractionUnit = reader.IsDBNull(index) ? 0 : int.Parse(reader[index].ToString());
        //                    index++; T.BeginEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
        //                    index++; T.EndEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
        //                    if (T.Customer != null)
        //                        if (results.ContainsKey(T.Customer))
        //                            results[T.Customer].Add(T);
        //                        else
        //                        {
        //                            List<Tariff> temp = new List<Tariff>();
        //                            temp.Add(T);
        //                            results[T.Customer] = temp;
        //                            temp = null;
        //                        }
        //                    T = null;
        //                }
        //                conn.Close();
        //                comm.Connection.Dispose();
        //                comm.Dispose();
        //                reader.Dispose();
        //                reader = null;
        //                return results;
        //            }
        //            catch (Exception EX)
        //            {
        //                throw new Exception("MyObjectAssembler_New->GetAllCustomerTarrif(param1,param2,param3):- " + EX.Message);
        //            }

        //        }

        //        public List<Rate> GetCustomerRates(CarrierAccount Customer, Zone zone, DateTime whenEffective, TABS.EffectiveCachedData _CachedData, Dictionary<int, Zone> _OwnZones)
        //        {
        //            List<Rate> results = new List<Rate>();
        //            try
        //            {
        //                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
        //                conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
        //                SqlCommand comm = new SqlCommand(@"With PrcelistCTE As (
        //                                              Select P.priceListID,SupplierID,P.CustomerID,P.CurrencyID,P.BeginEffectiveDate,P.EndEffectiveDate from Pricelist P where P.SupplierID='SYS')
        //                                              Select R.RateID,R.ZoneID,R.Rate,R.OffPeakRate,R.WeekendRate,R.Change,R.ServicesFlag,R.BeginEffectiveDate,R.EndEffectiveDate,P.* from Rate R  WITH (NOLOCK,index=IX_Rate_Dates)  inner join PrcelistCTE P   WITH (NOLOCK)  on R.PriceListID=P.PriceListID
        //                                              where P.CustomerID='" + Customer.CarrierAccountID + "' and  R.ZoneID=" + zone.ZoneID + " and R.BeginEffectiveDate <= '" + whenEffective.ToString("yyyy-MM-dd") + "' AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate >= '" + whenEffective.ToString("yyyy-MM-dd") + "') ", conn);


        //                comm.Connection = conn;
        //                SqlDataAdapter da = new SqlDataAdapter(comm);
        //                System.Data.DataTable dt = new System.Data.DataTable();
        //                da.Fill(dt);

        //                int index = -1;
        //                foreach (System.Data.DataRow dr in dt.Rows)
        //                {
        //                    Rate R = new Rate();
        //                    index = -1;
        //                    index++; R.ID = int.Parse(dr[index].ToString());
        //                    index++;
        //                    index++; R.Zone = _OwnZones.ContainsKey(int.Parse(dr[index].ToString())) ? _OwnZones[int.Parse(dr[index].ToString())] : null;
        //                    index++; R.Value = dr.IsNull(index) ? 0 : decimal.Parse(dr[index].ToString());
        //                    index++; R.OffPeakRate = dr.IsNull(index) ? 0 : decimal.Parse(dr[index].ToString());
        //                    index++; R.WeekendRate = dr.IsNull(index) ? 0 : decimal.Parse(dr[index].ToString());
        //                    index++; R.Change = (Change)int.Parse(dr[index].ToString());
        //                    index++; R.ServicesFlag = short.Parse(dr[index].ToString());
        //                    index++; R.BeginEffectiveDate = dr.IsNull(index) ? null : (DateTime?)DateTime.Parse(dr[index].ToString());
        //                    index++; R.EndEffectiveDate = dr.IsNull(index) ? null : (DateTime?)DateTime.Parse(dr[index].ToString());

        //                    PriceList P = new PriceList();
        //                    index++; P.ID = int.Parse(dr[index].ToString());
        //                    index++; //P.Supplier = TABS.CarrierAccount.SYSTEM;
        //                    index++; //P.Customer = Customer;
        //                    //index++; P.Description = dr.IsNull(index) ? "" : dr[index].ToString();
        //                    index++; P.Currency = _CachedData.Currencies.ContainsKey(dr[index].ToString()) ? _CachedData.Currencies[dr[index].ToString()] : null;
        //                    index++; //P.BeginEffectiveDate = dr.IsNull(index) ? null : (DateTime?)DateTime.Parse(dr[index].ToString());
        //                    index++; //P.EndEffectiveDate = dr.IsNull(index) ? null : (DateTime?)DateTime.Parse(dr[index].ToString());
        //                    R.PriceList = P;
        //                    results.Add(R);
        //                    R = null; P = null;
        //                }
        //                conn.Close();
        //                comm.Connection.Dispose();
        //                comm.Dispose();
        //                dt.Dispose();
        //                dt = null;
        //                da.Dispose(); da = null;
        //                return results;
        //            }
        //            catch (Exception EX)
        //            {
        //                throw new Exception("MyObjectAssembler_New->GetCustomerRates(param1,param2,param3,param4,param5,param6,param7):- " + EX.Message);
        //            }
        //        }

        //        public Dictionary<CarrierAccount, List<Rate>> GetAllCustomerRates(DateTime whenEffective, TABS.EffectiveCachedData _CachedData, Dictionary<int, Zone> _OwnZones)
        //        {

        //            //Dictionary<int, Zone> _OwnZones = MyObjectAssembler_New.GetOwnZones(whenEffective);
        //            Dictionary<CarrierAccount, List<Rate>> results = new Dictionary<CarrierAccount, List<Rate>>();

        //            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
        //            conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
        //            SqlCommand comm = new SqlCommand(@"With PrcelistCTE As (
        //                                              Select Pricelistid,P.CustomerID,P.SupplierID,P.CurrencyID from Pricelist P where P.SupplierID='SYS')
        //                                              Select R.RateID , R.PriceListID , R.ZoneID, R.Rate , R.OffPeakRate , R.WeekendRate , R.Change, R.ServicesFlag, R.BeginEffectiveDate , R.EndEffectiveDate , 
        //                                               P.Pricelistid,P.CustomerID,P.SupplierID,P.CurrencyID  from Rate R  WITH (NOLOCK,index=IX_Rate_Dates)  inner join PrcelistCTE P   WITH (NOLOCK)  on R.PriceListID=P.PriceListID
        //                                              where   R.BeginEffectiveDate <= '" + whenEffective.ToString("yyyy-MM-dd") + "' AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate > '" + whenEffective.ToString("yyyy-MM-dd") + "') ", conn);
        //            comm.Connection = conn;
        //            conn.Open();
        //            SqlDataReader dr = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        //            //SqlDataAdapter da = new SqlDataAdapter(comm);
        //            //System.Data.DataTable dt = new System.Data.DataTable();
        //            // da.Fill(dt);
        //            int count = 0;
        //            try
        //            {
        //                int index = -1;
        //                while (dr.Read())
        //                {
        //                    //foreach (System.Data.DataRow dr in dt.Rows)
        //                    //{
        //                    Rate R = new Rate();
        //                    index = -1;
        //                    index++; R.ID = int.Parse(dr[index].ToString());
        //                    index++;
        //                    index++; R.Zone = _OwnZones.ContainsKey(int.Parse(dr[index].ToString())) ? _OwnZones[int.Parse(dr[index].ToString())] : null;
        //                    index++; R.Value = dr.IsDBNull(index) ? 0 : decimal.Parse(dr[index].ToString());
        //                    index++; R.OffPeakRate = dr.IsDBNull(index) ? 0 : decimal.Parse(dr[index].ToString());
        //                    index++; R.WeekendRate = dr.IsDBNull(index) ? 0 : decimal.Parse(dr[index].ToString());
        //                    index++; R.Change = (Change)int.Parse(dr[index].ToString());
        //                    index++; R.ServicesFlag = short.Parse(dr[index].ToString());
        //                    index++; R.BeginEffectiveDate = dr.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(dr[index].ToString());
        //                    index++; R.EndEffectiveDate = dr.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(dr[index].ToString());
        //                    //index++;
        //                    // index++; index++;
        //                    // index++; R.Notes = dr[index].ToString();
        //                    PriceList P = new PriceList();
        //                    index++; P.ID = int.Parse(dr[index].ToString());
        //                    index++; P.Customer = _CachedData.Customers.ContainsKey(dr[index].ToString()) ? _CachedData.Customers[dr[index].ToString()] : null;
        //                    index++; //P.Supplier = TABS.CarrierAccount.SYSTEM;
        //                    index++; P.Currency = _CachedData.Currencies.ContainsKey(dr[index].ToString()) ? _CachedData.Currencies[dr[index].ToString()] : null;
        //                    R.PriceList = P;
        //                    if (P.Customer != null)
        //                        if (results.ContainsKey(P.Customer))
        //                            results[P.Customer].Add(R);
        //                        else
        //                        {
        //                            List<Rate> temp = new List<Rate>();
        //                            temp.Add(R);
        //                            results[P.Customer] = temp;
        //                            temp = null;
        //                        }
        //                    count++;
        //                    R = null; P = null;
        //                }
        //                conn.Close();
        //                comm.Connection.Dispose();
        //                comm.Dispose();
        //                dr.Dispose();
        //                dr = null;
        //                return results;
        //            }
        //            catch (Exception EX)
        //            {
        //                throw new Exception("MyObjectAssembler_New->GetAllCustomerRates(param1,param2,param3):- " + EX.Message);
        //            }

        //        }

        //        public Dictionary<Zone, List<Rate>> GetAllSupplierRatesZones(DateTime whenEffective, TABS.EffectiveCachedData _CachedData, Dictionary<CarrierAccount, List<Zone>> _AllEffectedSuppliersZones)
        //        {

        //            //Dictionary<CarrierAccount, List<Zone>> _DICAllEffectedZones = AllEffectiveSupplierZonesDictionary(whenEffective, _CachedData);
        //            Dictionary<Zone, List<Rate>> results = new Dictionary<Zone, List<Rate>>();
        //            int count = 0;

        //            try
        //            {
        //                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
        //                conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
        //                //                SqlCommand comm = new SqlCommand(@"With PrcelistCTE As (
        //                //                                              Select Pricelistid,P.CustomerID,P.SupplierID,P.CurrencyID from Pricelist P where P.CustomerID='SYS')
        //                //                                              Select * from Rate R  WITH (NOLOCK,index=IX_Rate_Dates)  inner join PrcelistCTE P   WITH (NOLOCK)  on R.PriceListID=P.PriceListID
        //                //                                              where   R.BeginEffectiveDate <= '" + whenEffective.ToString("yyyy-MM-dd") + "' AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate >= '" + whenEffective.ToString("yyyy-MM-dd") + "') ", conn);

        //                //With Time Shift
        //                SqlCommand comm = new SqlCommand(@" With PrcelistCTE As (
        //                                                Select Pricelistid,P.CustomerID,P.SupplierID,P.CurrencyID,CA.GMTTime from Pricelist P
        //                                                inner join CarrierAccount CA  WITH (NOLOCK) on CA.CarrierAccountID=P.CustomerID
        //                                                 where P.CustomerID='SYS'
        //                                                )
        //                                                 Select R.RateID , R.PriceListID , R.ZoneID, R.Rate , R.OffPeakRate , R.WeekendRate , R.Change, R.ServicesFlag, R.BeginEffectiveDate , R.EndEffectiveDate , 
        //                                                        P.Pricelistid,P.CustomerID,P.SupplierID,P.CurrencyID
        //                                                    from Rate R  WITH (NOLOCK,index=IX_Rate_Dates) 
        //                                                  inner join PrcelistCTE P   WITH (NOLOCK)  on R.PriceListID=P.PriceListID
        //                                                     where   R.BeginEffectiveDate <=DATEADD(hh,P.GMTTime,'" + whenEffective.ToString("yyyy-MM-dd") + "') AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate > DATEADD(hh,P.GMTTime,'" + whenEffective.ToString("yyyy-MM-dd") + "')) ", conn);


        //                comm.Connection = conn;
        //                conn.Open();
        //                SqlDataReader dr = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        //                //SqlDataAdapter da = new SqlDataAdapter(comm);
        //                //System.Data.DataTable dt = new System.Data.DataTable();
        //                // da.Fill(dt);
        //                int zoneID;
        //                int index = -1;
        //                //foreach (System.Data.DataRow dr in dt.Rows)
        //                //{
        //                while (dr.Read())
        //                {
        //                    Rate R = new Rate();
        //                    index = -1;
        //                    index++; R.ID = int.Parse(dr[index].ToString());
        //                    index++;
        //                    index++; zoneID = int.Parse(dr[index].ToString());
        //                    index++; R.Value = dr.IsDBNull(index) ? 0 : decimal.Parse(dr[index].ToString());
        //                    index++; R.OffPeakRate = dr.IsDBNull(index) ? 0 : decimal.Parse(dr[index].ToString());
        //                    index++; R.WeekendRate = dr.IsDBNull(index) ? 0 : decimal.Parse(dr[index].ToString());
        //                    index++; R.Change = (Change)int.Parse(dr[index].ToString());
        //                    index++; R.ServicesFlag = short.Parse(dr[index].ToString());
        //                    index++; R.BeginEffectiveDate = dr.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(dr[index].ToString());
        //                    index++; R.EndEffectiveDate = dr.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(dr[index].ToString());

        //                    PriceList P = new PriceList();
        //                    index++; P.ID = int.Parse(dr[index].ToString());
        //                    index++; //P.Customer = TABS.CarrierAccount.SYSTEM;
        //                    index++; P.Supplier = _CachedData.Supplires.ContainsKey(dr[index].ToString()) ? _CachedData.Supplires[dr[index].ToString()] : null;
        //                    if (P.Supplier == null) { count += 1; continue; }
        //                    R.Zone = _AllEffectedSuppliersZones.ContainsKey(P.Supplier) ? _AllEffectedSuppliersZones[P.Supplier].Where(z => z.Equals(zoneID)).FirstOrDefault() : null;
        //                    index++; P.Currency = _CachedData.Currencies.ContainsKey(dr[index].ToString()) ? _CachedData.Currencies[dr[index].ToString()] : null;
        //                    R.PriceList = P;
        //                    if (R.Zone != null)
        //                        if (results.ContainsKey(R.Zone))
        //                            results[R.Zone].Add(R);
        //                        else
        //                        {
        //                            List<Rate> temp = new List<Rate>();
        //                            temp.Add(R);
        //                            results[R.Zone] = temp;
        //                            temp = null;
        //                        }
        //                    R = null; P = null;
        //                }
        //                conn.Close();
        //                comm.Connection.Dispose();
        //                comm.Dispose();
        //                dr.Dispose();
        //                dr = null;
        //                return results;
        //            }
        //            catch (Exception EX)
        //            {
        //                throw new Exception("MyObjectAssembler_New->GetAllSupplierRatesZones(param1,param2,param3):- " + EX.Message);
        //            }
        //        }

        //        public Dictionary<CarrierAccount, List<Commission>> GetAllCustomerCommission(DateTime whenEffective, TABS.EffectiveCachedData _CachedData, Dictionary<int, Zone> _OwnZones)
        //        {
        //            //Dictionary<int, Zone> _OwnZones = MyObjectAssembler_New.GetOwnZones(whenEffective);
        //            Dictionary<CarrierAccount, List<Commission>> results = new Dictionary<CarrierAccount, List<Commission>>();
        //            try
        //            {
        //                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
        //                conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
        //                SqlCommand comm = new SqlCommand(@"select CommissionID,SupplierID,CustomerID,ZoneID,FromRate,ToRate,Percentage ,Amount
        //                         ,BeginEffectiveDate,EndEffectiveDate,IsExtraCharge from Commission C WITH (NOLOCK) where C.BeginEffectiveDate <= '" + whenEffective.ToString("yyyy-MM-dd") + "' AND (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate >= '" + whenEffective.ToString("yyyy-MM-dd") + "') AND C.SupplierID='SYS'", conn);
        //                comm.Connection = conn;
        //                conn.Open();
        //                SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        //                int index = -1;
        //                int zoneID;
        //                CarrierAccount SYSTEM = GetSYSTEMCarrierAccounts(_CachedData.CarrierProfiles, _CachedData.CarrierGroups);
        //                while (reader.Read())
        //                {
        //                    Commission C = new Commission();
        //                    index = -1;
        //                    index++; C.CommissionID = int.Parse(reader[index].ToString());
        //                    index++; C.Supplier = SYSTEM;// TABS.CarrierAccount.SYSTEM;
        //                    index++; C.Customer = _CachedData.Customers.ContainsKey(reader[index].ToString()) ? _CachedData.Customers[reader[index].ToString()] : null;
        //                    index++; zoneID = int.Parse(reader[index].ToString());
        //                    C.Zone = _OwnZones.ContainsKey(zoneID) ? _OwnZones[zoneID] : null;
        //                    index++; C.FromRate = reader.IsDBNull(index) ? 0 : float.Parse(reader[index].ToString());
        //                    index++; C.ToRate = reader.IsDBNull(index) ? 0 : float.Parse(reader[index].ToString());
        //                    index++; C.Percentage = reader.IsDBNull(index) ? 0 : float.Parse(reader[index].ToString());
        //                    index++; C.Amount = reader.IsDBNull(index) ? 0 : decimal.Parse(reader[index].ToString());
        //                    index++; C.BeginEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
        //                    index++; C.EndEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
        //                    index++; C.IsExtraCharge = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
        //                    if (results.ContainsKey(C.Customer))
        //                        results[C.Customer].Add(C);
        //                    else
        //                    {
        //                        List<Commission> temp = new List<Commission>();
        //                        temp.Add(C);
        //                        results[C.Customer] = temp;
        //                        temp = null;
        //                    }
        //                    C = null;
        //                }
        //                reader.Dispose(); reader = null; comm.Connection.Dispose();
        //                conn.Close(); comm.Dispose(); comm = null;
        //                SYSTEM = null;
        //                return results;
        //            }
        //            catch (Exception EX)
        //            {
        //                throw new Exception("MyObjectAssembler_New->GetAllCustomerCommission(param1,param2,param3):- " + EX.Message);
        //            }

        //        }

        //        public Dictionary<CarrierAccount, List<Commission>> GetAllSupplierCommission(DateTime whenEffective, TABS.EffectiveCachedData _CachedData, Dictionary<CarrierAccount, List<Zone>> _AllEffectedSuppliersZone)
        //        {
        //            //Dictionary<CarrierAccount, List<Zone>> _DICAllEffectedZones = AllEffectiveSupplierZonesDictionary(whenEffective,_CachedData);
        //            Dictionary<CarrierAccount, List<Commission>> results = new Dictionary<CarrierAccount, List<Commission>>();
        //            try
        //            {
        //                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
        //                conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
        //                SqlCommand comm = new SqlCommand(@"select CommissionID,SupplierID,CustomerID,ZoneID,FromRate,ToRate,Percentage ,Amount
        //                             ,BeginEffectiveDate,EndEffectiveDate,IsExtraCharge from Commission C WITH (NOLOCK) where C.BeginEffectiveDate <= '" + whenEffective.ToString("yyyy-MM-dd") + "' AND (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate >= '" + whenEffective.ToString("yyyy-MM-dd") + "') AND C.CustomerID='SYS' ", conn);
        //                comm.Connection = conn;
        //                conn.Open();
        //                SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        //                int index = -1;
        //                int zoneID;
        //                CarrierAccount SYSTEM = GetSYSTEMCarrierAccounts(_CachedData.CarrierProfiles, _CachedData.CarrierGroups);
        //                while (reader.Read())
        //                {
        //                    Commission C = new Commission();
        //                    index = -1;
        //                    index++; C.CommissionID = int.Parse(reader[index].ToString());
        //                    index++; C.Supplier = _CachedData.Supplires.ContainsKey(reader[index].ToString()) ? _CachedData.Supplires[reader[index].ToString()] : null;
        //                    index++; C.Customer = SYSTEM; // TABS.CarrierAccount.SYSTEM;
        //                    index++; zoneID = int.Parse(reader[index].ToString());
        //                    if (C.Supplier == null) continue;
        //                    C.Zone = _AllEffectedSuppliersZone.ContainsKey(C.Supplier) ? _AllEffectedSuppliersZone[C.Supplier].Where(z => z.Equals(zoneID)).FirstOrDefault() : null;
        //                    index++; C.FromRate = reader.IsDBNull(index) ? 0 : float.Parse(reader[index].ToString());
        //                    index++; C.ToRate = reader.IsDBNull(index) ? 0 : float.Parse(reader[index].ToString());
        //                    index++; C.Percentage = reader.IsDBNull(index) ? 0 : float.Parse(reader[index].ToString());
        //                    index++; C.Amount = reader.IsDBNull(index) ? 0 : decimal.Parse(reader[index].ToString());
        //                    index++; C.BeginEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
        //                    index++; C.EndEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
        //                    index++; C.IsExtraCharge = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
        //                    if (results.ContainsKey(C.Supplier))
        //                        results[C.Supplier].Add(C);
        //                    else
        //                    {
        //                        List<Commission> temp = new List<Commission>();
        //                        temp.Add(C);
        //                        results[C.Supplier] = temp;
        //                        temp = null;
        //                    }
        //                    C = null;
        //                }
        //                reader.Dispose(); reader = null; comm.Connection.Dispose();
        //                conn.Close(); comm.Dispose(); comm = null;
        //                SYSTEM = null;
        //                return results;
        //            }
        //            catch (Exception EX)
        //            {
        //                throw new Exception("MyObjectAssembler_New->GetAllSupplierCommission(param1,param2,param3):- " + EX.Message);
        //            }

        //        }

        //        public Dictionary<CarrierAccount, List<ToDConsideration>> GetAllSupplierToDConsideration(DateTime whenEffective, TABS.EffectiveCachedData _CachedData, Dictionary<CarrierAccount, List<Zone>> _AllEffectedSupplierZones)
        //        {
        //            //Dictionary<CarrierAccount, List<Zone>> _DICAllEffectedZones = AllEffectiveSupplierZonesDictionary(whenEffective, _CachedData);
        //            Dictionary<CarrierAccount, List<ToDConsideration>> results = new Dictionary<CarrierAccount, List<ToDConsideration>>();
        //            try
        //            {
        //                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
        //                conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
        //                SqlCommand comm = new SqlCommand(@"select ToDConsiderationID,ZoneID ,SupplierID ,CustomerID ,BeginTime ,EndTime
        //                                            ,WeekDay ,HolidayDate ,HolidayName,RateType ,BeginEffectiveDate ,EndEffectiveDate ,IsEffective from ToDConsideration T WITH (NOLOCK) where T.BeginEffectiveDate <= '" + whenEffective.ToString("yyyy-MM-dd") + "' AND (T.EndEffectiveDate IS NULL OR T.EndEffectiveDate >= '" + whenEffective.ToString("yyyy-MM-dd") + "') AND T.CustomerID='SYS' ", conn);
        //                comm.Connection = conn;
        //                conn.Open();
        //                SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        //                int index = -1;
        //                int zoneID;
        //                CarrierAccount SYSTEM = GetSYSTEMCarrierAccounts(_CachedData.CarrierProfiles, _CachedData.CarrierGroups);
        //                while (reader.Read())
        //                {
        //                    ToDConsideration T = new ToDConsideration();
        //                    index = -1;
        //                    index++; T.ToDConsiderationID = int.Parse(reader[index].ToString());
        //                    index++; zoneID = int.Parse(reader[index].ToString());


        //                    index++; T.Supplier = _CachedData.Supplires.ContainsKey(reader[index].ToString()) ? _CachedData.Supplires[reader[index].ToString()] : null;
        //                    index++; T.Customer = SYSTEM; // TABS.CarrierAccount.SYSTEM;
        //                    if (T.Supplier == null) continue;
        //                    T.Zone = _AllEffectedSupplierZones.ContainsKey(T.Supplier) ? _AllEffectedSupplierZones[T.Supplier].Where(z => z.Equals(zoneID)).FirstOrDefault() : null;
        //                    index++; T.BeginTime = reader.IsDBNull(index) ? null : (TimeSpan?)TimeSpan.Parse(reader[index].ToString());
        //                    index++; T.EndTime = reader.IsDBNull(index) ? null : (TimeSpan?)TimeSpan.Parse(reader[index].ToString());
        //                    index++; T.WeekDay = reader.IsDBNull(index) ? null : (DayOfWeek?)int.Parse(reader[index].ToString());
        //                    index++; T.HolidayDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
        //                    index++; T.HolidayName = reader.IsDBNull(index) ? "" : reader[index].ToString();
        //                    index++; T.RateType = reader.IsDBNull(index) ? 0 : (ToDRateType)int.Parse(reader[index].ToString());
        //                    index++; T.BeginEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
        //                    index++; T.EndEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
        //                    index++; index++; // T.IsActive = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
        //                    if (T.Supplier != null)
        //                        if (results.ContainsKey(T.Supplier))
        //                            results[T.Supplier].Add(T);
        //                        else
        //                        {
        //                            List<ToDConsideration> temp = new List<ToDConsideration>();
        //                            temp.Add(T);
        //                            results[T.Supplier] = temp;
        //                            temp = null;
        //                        }
        //                    T = null;
        //                }
        //                reader.Dispose(); reader = null; comm.Connection.Dispose();
        //                conn.Close(); comm.Dispose(); comm = null;
        //                SYSTEM = null;
        //                return results;
        //            }
        //            catch (Exception EX)
        //            {
        //                throw new Exception("MyObjectAssembler_New->GetAllSupplierToDConsideration(param1,param2,param3):- " + EX.Message);
        //            }

        //        }

        //        public Dictionary<CarrierAccount, List<ToDConsideration>> GetAllCustomerToDConsideration(DateTime whenEffective, TABS.EffectiveCachedData _CachedData, Dictionary<int, Zone> _OwnZones)
        //        {
        //            //Dictionary<int, Zone> _OwnZones = MyObjectAssembler_New.GetOwnZones(whenEffective);
        //            Dictionary<CarrierAccount, List<ToDConsideration>> results = new Dictionary<CarrierAccount, List<ToDConsideration>>();
        //            try
        //            {
        //                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
        //                conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
        //                SqlCommand comm = new SqlCommand(@"select ToDConsiderationID,ZoneID ,SupplierID ,CustomerID ,BeginTime ,EndTime
        //                                                ,WeekDay ,HolidayDate ,HolidayName,RateType ,BeginEffectiveDate ,EndEffectiveDate  from ToDConsideration T WITH (NOLOCK) where T.BeginEffectiveDate <= '" + whenEffective.ToString("yyyy-MM-dd") + "' AND (T.EndEffectiveDate IS NULL OR T.EndEffectiveDate >= '" + whenEffective.ToString("yyyy-MM-dd") + "') AND T.SupplierID='SYS' ", conn);
        //                comm.Connection = conn;
        //                conn.Open();
        //                SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        //                int index = -1;
        //                int zoneID;
        //                CarrierAccount SYSTEM = GetSYSTEMCarrierAccounts(_CachedData.CarrierProfiles, _CachedData.CarrierGroups);
        //                while (reader.Read())
        //                {
        //                    ToDConsideration T = new ToDConsideration();
        //                    index = -1;
        //                    index++; T.ToDConsiderationID = int.Parse(reader[index].ToString());
        //                    index++; zoneID = int.Parse(reader[index].ToString());
        //                    T.Zone = _OwnZones.ContainsKey(zoneID) ? _OwnZones[zoneID] : null;

        //                    index++; T.Supplier = SYSTEM; // TABS.CarrierAccount.SYSTEM;
        //                    index++; T.Customer = _CachedData.Customers.ContainsKey(reader[index].ToString()) ? _CachedData.Customers[reader[index].ToString()] : null;

        //                    index++; T.BeginTime = reader.IsDBNull(index) ? null : (TimeSpan?)TimeSpan.Parse(reader[index].ToString());
        //                    index++; T.EndTime = reader.IsDBNull(index) ? null : (TimeSpan?)TimeSpan.Parse(reader[index].ToString());
        //                    index++; T.WeekDay = reader.IsDBNull(index) ? null : (DayOfWeek?)int.Parse(reader[index].ToString());
        //                    index++; T.HolidayDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
        //                    index++; T.HolidayName = reader.IsDBNull(index) ? "" : reader[index].ToString();
        //                    index++; T.RateType = reader.IsDBNull(index) ? 0 : (ToDRateType)int.Parse(reader[index].ToString());
        //                    index++; T.BeginEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
        //                    index++; T.EndEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
        //                    //index++; index++; // T.IsActive = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
        //                    if (T.Customer != null)
        //                        if (results.ContainsKey(T.Customer))
        //                            results[T.Customer].Add(T);
        //                        else
        //                        {
        //                            List<ToDConsideration> temp = new List<ToDConsideration>();
        //                            temp.Add(T);
        //                            results[T.Customer] = temp;
        //                        }
        //                    T = null;
        //                }
        //                reader.Dispose(); reader = null; comm.Connection.Dispose();
        //                conn.Close(); comm.Dispose(); comm = null;
        //                SYSTEM = null;
        //                return results;
        //            }
        //            catch (Exception EX)
        //            {
        //                throw new Exception("MyObjectAssembler_New->GetAllCustomerToDConsideration(param1,param2,param3):- " + EX.Message);
        //            }

        //        }

        //        public Dictionary<Currency, List<CurrencyExchangeRate>> GetExchangeRates()
        //        {
        //            Dictionary<string, Currency> _Currencies = GetCurrencies();
        //            Dictionary<Currency, List<CurrencyExchangeRate>> results = new Dictionary<Currency, List<CurrencyExchangeRate>>();
        //            DateTime FromDate = new DateTime(1995, 1, 1);
        //            DateTime ToDate = new DateTime(DateTime.Now.Year + 50, 1, 1);
        //            try
        //            {

        //                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
        //                conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
        //                SqlCommand comm = new SqlCommand(@"select CurrencyExchangeRateID,CurrencyID,Rate,ExchangeDate,UserID,timestamp from CurrencyExchangeRate C with (NOLOCK) where C.ExchangeDate >= '" + FromDate.ToString("yyyy-MM-dd") + "' and C.ExchangeDate<= '" + ToDate.ToString("yyyy-MM-dd") + "' ", conn);
        //                comm.Connection = conn;
        //                conn.Open();
        //                SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        //                int index = -1;
        //                int count = 0;
        //                while (reader.Read())
        //                {
        //                    CurrencyExchangeRate CurrExch = new CurrencyExchangeRate();
        //                    index = -1;
        //                    index++; CurrExch.CurrencyExchangeRateID = int.Parse(reader[index].ToString());
        //                    index++; CurrExch.Currency = _Currencies.ContainsKey(reader[index].ToString()) ? _Currencies[reader[index].ToString()] : null;
        //                    index++; CurrExch.Rate = reader.IsDBNull(index) ? 0 : float.Parse(reader[index].ToString());
        //                    index++; CurrExch.ExchangeDate = DateTime.Parse(reader[index].ToString());
        //                    if (CurrExch.Currency != null)
        //                        if (results.ContainsKey(CurrExch.Currency))
        //                            results[CurrExch.Currency].Add(CurrExch);
        //                        else
        //                        {
        //                            List<CurrencyExchangeRate> temp = new List<CurrencyExchangeRate>();
        //                            temp.Add(CurrExch);
        //                            results.Add(CurrExch.Currency, temp);
        //                        }
        //                    count++;
        //                    CurrExch = null;
        //                }
        //                reader.Dispose(); reader = null; comm.Connection.Dispose();
        //                conn.Close(); comm.Dispose(); comm = null;
        //                _Currencies = null;
        //                return results;
        //            }
        //            catch (Exception EX)
        //            {
        //                throw new Exception("MyObjectAssembler_New->GetExchangeRates():- " + EX.Message);
        //            }
        //        }


        //        public Dictionary<TABS.CarrierAccount, List<Zone>> AllEffectiveSupplierZonesDictionary(DateTime whenEffective, TABS.EffectiveCachedData _CachedData)
        //        {
        //            try
        //            {
        //                //Dictionary<TABS.CarrierAccount, List<Zone>> result = new Dictionary<CarrierAccount, List<Zone>>();
        //                // List<Zone> list = new List<Zone>();
        //                // Dictionary<int, Zone> zones = new Dictionary<int, Zone>();

        //                Dictionary<CarrierAccount, List<Zone>> _SupplierZones = GetAllSupplierZones(whenEffective, _CachedData.Supplires, _CachedData.CodeGroups);
        //                return _SupplierZones;
        //                //List<object[]> results = GetAllZonesRates(whenEffective, _CachedData.Supplires, _SupplierZones, _CachedData.CodeGroups);//_CachedData.CarrierAccounts


        //                //foreach (object[] tupple in results)
        //                //{
        //                //    Zone zone = (Zone)tupple[0];
        //                //    zones[zone.ZoneID] = zone;
        //                //    Rate rate = (Rate)tupple[1];
        //                //    zone.EffectiveRates = new List<Rate>(new Rate[] { rate });
        //                //    list.Add(zone);
        //                //}

        //                //List<Code> codes = GetAllSupplierCodes(whenEffective, _CachedData.Supplires, _SupplierZones);// _CachedData.CarrierAccounts
        //                //_SupplierZones = null;
        //                //// Initialize zones
        //                //foreach (Zone zone in zones.Values) zone.EffectiveCodes = new List<Code>();

        //                //// Invalid Zones should be put as-non effective
        //                //List<Zone> invalidZones = new List<Zone>();

        //                //// Assign codes for zones
        //                //foreach (Code code in codes)
        //                //{
        //                //    Zone zone;
        //                //    if (!zones.TryGetValue(code.Zone.ZoneID, out zone))
        //                //        invalidZones.Add(code.Zone);
        //                //    else
        //                //        zone.EffectiveCodes.Add(code);
        //                //}


        //                //foreach (Zone Z in list)
        //                //{
        //                //    if (result.ContainsKey(Z.Supplier))
        //                //        result[Z.Supplier].Add(Z);
        //                //    else
        //                //    {
        //                //        List<Zone> temp = new List<Zone>();
        //                //        temp.Add(Z);
        //                //        result[Z.Supplier] = temp;
        //                //        temp = null;
        //                //    }
        //                //}
        //                //results = null; list = null; codes = null; invalidZones = null; zones = null;
        //                //GC.Collect(GC.MaxGeneration);
        //                //GC.WaitForPendingFinalizers();
        //                //return result;
        //            }
        //            catch (Exception Ex)
        //            {
        //                throw new Exception(" --:" + Ex.Message);
        //            }
        //        }

        public List<object[]> GetAllZonesRates(DateTime whenEffective, Dictionary<string, CarrierAccount> _AllSupplires, Dictionary<CarrierAccount, List<Zone>> _dicSupplierZones, Dictionary<string, CodeGroup> _CodeGroups)
        {
            List<object[]> ZoneRatesTupple = new List<object[]>();
            int count = 0;
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();

                    SqlCommand comm = new SqlCommand(@"SELECT Z.ZoneID,Z.CodeGroup,Z.Name,Z.SupplierID,Z.ServicesFlag,Z.IsMobile ,Z.IsProper ,Z.IsSold,
                                                Z.BeginEffectiveDate,Z.EndEffectiveDate,
                                                R.RateID ,R.PriceListID ,R.ZoneID,R.Rate,R.OffPeakRate ,R.WeekendRate,R.Change ,R.ServicesFlag ,R.BeginEffectiveDate ,
                                                R.EndEffectiveDate  
                                                FROM Zone Z  WITH (NOLOCK)  inner join Rate R  WITH (NOLOCK)  on  R.ZoneID = Z.ZoneID inner join PriceList P  WITH (NOLOCK)  on R.PriceListID=P.PriceListID AND P.SupplierID<>'SYS'
                                                WHERE R.BeginEffectiveDate <= '" + whenEffective.ToString("yyyy-MM-dd") + "' AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate >= '" + whenEffective.ToString("yyyy-MM-dd") + "' ) ORDER BY Z.Name", conn);

                    comm.Connection = conn;
                    SqlDataAdapter da = new SqlDataAdapter(comm);
                    System.Data.DataTable dt = new System.Data.DataTable();
                    da.Fill(dt);

                    int index = -1;
                    int zoneID;
                    // DateTime start = DateTime.Now;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            index = -1;
                            Zone Z = new Zone();

                            index++; Z.ZoneID = int.Parse(dt.Rows[i][index].ToString());
                            index++; Z.CodeGroup = _CodeGroups.ContainsKey(dt.Rows[i][index].ToString()) ? _CodeGroups[dt.Rows[i][index].ToString()] : null;
                            index++; Z.Name = dt.Rows[i].IsNull(index) ? "" : dt.Rows[i][index].ToString();
                            index++; Z.Supplier = _AllSupplires.ContainsKey(dt.Rows[i][index].ToString()) ? _AllSupplires[dt.Rows[i][index].ToString()] : null;
                            if (Z.Supplier == null) continue;
                            index++; Z.ServicesFlag = short.Parse(dt.Rows[i][index].ToString());
                            index++; Z.IsMobile = char.Parse(dt.Rows[i][index].ToString()) == 'Y' ? true : false;
                            index++; Z.IsProper = char.Parse(dt.Rows[i][index].ToString()) == 'Y' ? true : false;
                            index++; Z.IsSold = char.Parse(dt.Rows[i][index].ToString()) == 'Y' ? true : false;
                            index++; Z.BeginEffectiveDate = dt.Rows[i].IsNull(index) ? null : (DateTime?)DateTime.Parse(dt.Rows[i][index].ToString());
                            index++; Z.EndEffectiveDate = dt.Rows[i].IsNull(index) ? null : (DateTime?)DateTime.Parse(dt.Rows[i][index].ToString());
                            //index++; index++; Z.User = null;



                            Rate R = new Rate();
                            // index++;
                            index++; R.ID = int.Parse(dt.Rows[i][index].ToString());
                            index++; R.PriceList = null;

                            index++; zoneID = int.Parse(dt.Rows[i][index].ToString()); R.Zone = _dicSupplierZones.ContainsKey(Z.Supplier) ? _dicSupplierZones[Z.Supplier].Where(z => z.Equals(zoneID)).FirstOrDefault() : null;
                            index++; R.Value = dt.Rows[i].IsNull(index) ? 0 : decimal.Parse(dt.Rows[i][index].ToString());
                            index++; R.OffPeakRate = dt.Rows[i].IsNull(index) ? 0 : decimal.Parse(dt.Rows[i][index].ToString());
                            index++; R.WeekendRate = dt.Rows[i].IsNull(index) ? 0 : decimal.Parse(dt.Rows[i][index].ToString());
                            index++; R.Change = (Change)int.Parse(dt.Rows[i][index].ToString());
                            index++; R.ServicesFlag = short.Parse(dt.Rows[i][index].ToString());
                            index++; R.BeginEffectiveDate = dt.Rows[i].IsNull(index) ? null : (DateTime?)DateTime.Parse(dt.Rows[i][index].ToString());
                            index++; R.EndEffectiveDate = dt.Rows[i].IsNull(index) ? null : (DateTime?)DateTime.Parse(dt.Rows[i][index].ToString());
                            //index++; index++; R.User = null;
                            //index++; index++; R.Notes = dt.Rows[i].IsNull(index) ? "" : dt.Rows[i][index].ToString();

                            object[] Obj = new object[2];
                            Obj[0] = Z;
                            Obj[1] = R;
                            if (Z.Supplier != null && R.Zone != null)
                                ZoneRatesTupple.Add(Obj);
                            count++;
                            Z = null; R = null; Obj = null;
                        }
                    }
                    conn.Close();
                    comm.Connection.Dispose();
                    comm.Dispose();
                    dt.Dispose();
                    dt = null;
                    da.Dispose(); da = null;
                }
                //TimeSpan spent = DateTime.Now.Subtract(start);
            }
            catch (Exception e)
            {
                throw new Exception("MyObjectAssembler_New->GetAllZonesRates(param1,param2,param3,param4):- " + e.Message);
            }
            return ZoneRatesTupple;
        }

        public Dictionary<string, CodeGroup> GetCodeGroups()
        {
            Dictionary<string, CodeGroup> results = new Dictionary<string, CodeGroup>();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    SqlCommand comm = new SqlCommand("select Code,Name,UserID from CodeGroup  WITH (NOLOCK) ", conn);
                    comm.Connection = conn;
                    conn.Open();
                    SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    int index = -1;
                    while (reader.Read())
                    {
                        CodeGroup C = new CodeGroup();
                        index = -1;
                        index++; C.Code = reader[index].ToString();
                        index++; C.Name = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.User = null;
                        results[C.Code] = C;
                        C = null;
                    }
                    comm.Connection.Dispose();
                    conn.Close();
                    reader.Dispose();
                    reader = null;
                    comm.Dispose();
                    comm = null;
                }
                return results;
            }
            catch (Exception EX)
            {
                throw new Exception("MyObjectAssembler_New->GetCodeGroups():- " + EX.Message);
            }
        }

        public List<Zone> Get_OwnZones(DateTime when)
        {
            List<Zone> LstOwnZones = new List<Zone>();

            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    SqlCommand comm = new SqlCommand(@"SELECT ZoneID,CodeGroup,Name,SupplierID,ServicesFlag,IsMobile ,IsProper ,IsSold,
                                    BeginEffectiveDate,EndEffectiveDate FROM Zone Z  WITH (NOLOCK)  WHERE  Z.SupplierID ='SYS' AND '" + when.ToString("yyyy-MM-dd") + "' >= Z.BeginEffectiveDate AND (Z.EndEffectiveDate IS NULL OR '" + when.ToString("yyyy-MM-dd") + "' < Z.EndEffectiveDate) ORDER BY Z.Name", conn);
                    comm.Connection = conn;
                    conn.Open();
                    SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    int index = -1;
                    CarrierAccount SYSTEM = GetSYSTEMCarrierAccounts();
                    //Dictionary<string, CodeGroup> _CodeGroups = GetCodeGroups();
                    while (reader.Read())
                    {
                        Zone Z = new Zone();
                        index = -1;
                        index++; Z.ZoneID = int.Parse(reader[index].ToString());
                        index++; //Z.CodeGroup = _CodeGroups[reader[index].ToString()];
                        index++; Z.Name = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; Z.Supplier = SYSTEM;// TABS.CarrierAccount.SYSTEM;
                        index++; //Z.ServicesFlag = short.Parse(reader[index].ToString());
                        index++; Z.IsMobile = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; Z.IsProper = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; Z.IsSold = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; Z.BeginEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; Z.EndEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        //index++; index++; Z.User = null;
                        LstOwnZones.Add(Z);
                        Z = null;
                    }
                    comm.Connection.Dispose();
                    conn.Close();
                    reader.Dispose();
                    reader = null;
                    comm.Dispose();
                    comm = null;
                }
                return LstOwnZones;
            }
            catch (Exception EX)
            {
                throw new Exception("MyObjectAssembler_New->Get_OwnZones(param1):- " + EX.Message);
            }
        }

        public Dictionary<string, List<Zone>> GetAllSupplierIDZones(DateTime when, Dictionary<string, CarrierAccount> _AllSupplier, Dictionary<string, CodeGroup> _CodeGroups)
        {
            Dictionary<string, List<Zone>> dicSuppliedZones = new Dictionary<string, List<Zone>>();
            int count = 0;
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    //SqlCommand comm = new SqlCommand("SELECT * FROM Zone Z  WITH (NOLOCK)  WHERE '" + when.ToString("yyyy-MM-dd") + "' >= Z.BeginEffectiveDate AND (Z.EndEffectiveDate IS NULL OR '" + when.ToString("yyyy-MM-dd") + "' < Z.EndEffectiveDate) ORDER BY Z.Name", conn);
                    //With Time Shift
                    SqlCommand comm = new SqlCommand(@"SELECT Z.ZoneID,Z.CodeGroup,Z.Name,Z.SupplierID,Z.ServicesFlag,Z.IsMobile ,Z.IsProper ,Z.IsSold,
                                        Z.BeginEffectiveDate,Z.EndEffectiveDate FROM Zone Z  WITH (NOLOCK)  inner join CarrierAccount CA  WITH (NOLOCK)  on CA.CarrierAccountID=Z.SupplierID WHERE DATEADD(hh,CA.GMTTime,'" + when.ToString("yyyy-MM-dd") + "' ) >= Z.BeginEffectiveDate AND (Z.EndEffectiveDate IS NULL OR DATEADD(hh,CA.GMTTime,'" + when.ToString("yyyy-MM-dd") + "' ) < Z.EndEffectiveDate) ORDER BY Z.Name", conn);
                    comm.Connection = conn;
                    conn.Open();

                    SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    int index = -1;

                    while (reader.Read())
                    {
                        Zone Z = new Zone();
                        index = -1;
                        index++; Z.ZoneID = int.Parse(reader[index].ToString());
                        index++; Z.CodeGroup = _CodeGroups.ContainsKey(reader[index].ToString()) ? _CodeGroups[reader[index].ToString()] : null;
                        index++; Z.Name = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; Z.Supplier = _AllSupplier.ContainsKey(reader[index].ToString()) == false ? null : _AllSupplier[reader[index].ToString()];
                        index++; Z.ServicesFlag = short.Parse(reader[index].ToString());
                        index++; Z.IsMobile = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; Z.IsProper = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; Z.IsSold = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; Z.BeginEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; Z.EndEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        //index++; index++; Z.User = null;
                        if (Z.Supplier != null)
                            if (!dicSuppliedZones.ContainsKey(Z.Supplier.CarrierAccountID))
                            {
                                List<Zone> temp = new List<Zone>();
                                temp.Add(Z);
                                dicSuppliedZones[Z.Supplier.CarrierAccountID] = temp;
                                temp = null;
                            }
                            else
                                dicSuppliedZones[Z.Supplier.CarrierAccountID].Add(Z);
                        count++;
                        Z = null;
                    }
                    comm.Connection.Dispose();
                    conn.Close();
                    reader.Dispose();
                    reader = null;
                    comm.Dispose();
                    comm = null;
                }
            }
            catch (Exception EX)
            {
                throw new Exception("MyObjectAssembler_New->Get_AllSupplierZones(param1,param2,param3):- " + EX.Message);
            }
            return dicSuppliedZones;
        }


        public Dictionary<CarrierAccount, List<Zone>> GetAllSupplierZones(DateTime when, Dictionary<string, CarrierAccount> _AllSupplier, Dictionary<string, CodeGroup> _CodeGroups)
        {
            Dictionary<CarrierAccount, List<Zone>> dicSuppliedZones = new Dictionary<CarrierAccount, List<Zone>>();
            int count = 0;
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;//System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    //SqlCommand comm = new SqlCommand("SELECT * FROM Zone Z  WITH (NOLOCK)  WHERE '" + when.ToString("yyyy-MM-dd") + "' >= Z.BeginEffectiveDate AND (Z.EndEffectiveDate IS NULL OR '" + when.ToString("yyyy-MM-dd") + "' < Z.EndEffectiveDate) ORDER BY Z.Name", conn);
                    //With Time Shift
                    SqlCommand comm = new SqlCommand(@"SELECT Z.ZoneID,Z.CodeGroup,Z.Name,Z.SupplierID,Z.ServicesFlag,Z.IsMobile ,Z.IsProper ,Z.IsSold,
                                        Z.BeginEffectiveDate,Z.EndEffectiveDate FROM Zone Z  WITH (NOLOCK)  inner join CarrierAccount CA  WITH (NOLOCK)  on CA.CarrierAccountID=Z.SupplierID WHERE DATEADD(hh,CA.GMTTime,'" + when.ToString("yyyy-MM-dd") + "' ) >= Z.BeginEffectiveDate AND (Z.EndEffectiveDate IS NULL OR DATEADD(hh,CA.GMTTime,'" + when.ToString("yyyy-MM-dd") + "' ) < Z.EndEffectiveDate) ORDER BY Z.Name", conn);
                    comm.Connection = conn;
                    conn.Open();

                    SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    int index = -1;

                    while (reader.Read())
                    {
                        Zone Z = new Zone();
                        index = -1;
                        index++; Z.ZoneID = int.Parse(reader[index].ToString());
                        index++; Z.CodeGroup = _CodeGroups.ContainsKey(reader[index].ToString()) ? _CodeGroups[reader[index].ToString()] : null;
                        index++; Z.Name = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; Z.Supplier = _AllSupplier.ContainsKey(reader[index].ToString()) == false ? null : _AllSupplier[reader[index].ToString()];
                        index++; Z.ServicesFlag = short.Parse(reader[index].ToString());
                        index++; Z.IsMobile = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; Z.IsProper = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; Z.IsSold = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; Z.BeginEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; Z.EndEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        //index++; index++; Z.User = null;
                        if (Z.Supplier != null)
                            if (!dicSuppliedZones.ContainsKey(Z.Supplier))
                            {
                                List<Zone> temp = new List<Zone>();
                                temp.Add(Z);
                                dicSuppliedZones[Z.Supplier] = temp;
                                temp = null;
                            }
                            else
                                dicSuppliedZones[Z.Supplier].Add(Z);
                        count++;
                        Z = null;
                    }
                    comm.Connection.Dispose();
                    conn.Close();
                    reader.Dispose();
                    reader = null;
                    comm.Dispose();
                    comm = null;
                }
            }
            catch (Exception EX)
            {
                throw new Exception("MyObjectAssembler_New->Get_AllSupplierZones(param1,param2,param3):- " + EX.Message);
            }
            return dicSuppliedZones;
        }

        public Dictionary<int, Zone> Get_OwnZonesDIC(DateTime when)
        {
            Dictionary<int, Zone> dicOwnZones = new Dictionary<int, Zone>();
            List<Zone> lstownZones = new List<Zone>();

            try
            {

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;//System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    SqlCommand comm = new SqlCommand(@"SELECT ZoneID,CodeGroup,Name,SupplierID,ServicesFlag,IsMobile ,IsProper ,IsSold,
                                    BeginEffectiveDate,EndEffectiveDate FROM Zone Z  WITH (NOLOCK)  WHERE  Z.SupplierID ='SYS' AND '" + when.ToString("yyyy-MM-dd") + "' >= Z.BeginEffectiveDate AND (Z.EndEffectiveDate IS NULL OR '" + when.ToString("yyyy-MM-dd") + "' < Z.EndEffectiveDate) ORDER BY Z.Name", conn);
                    comm.Connection = conn;
                    conn.Open();
                    SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    int index = -1;

                    CarrierAccount SYSTEM = GetSYSTEMCarrierAccounts();
                    //Dictionary<string, CodeGroup> _CodeGroups = GetCodeGroups();


                    while (reader.Read())
                    {
                        Zone Z = new Zone();
                        index = -1;
                        index++; Z.ZoneID = int.Parse(reader[index].ToString());
                        index++; //Z.CodeGroup = _CodeGroups[reader[index].ToString()];
                        index++; Z.Name = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; Z.Supplier = SYSTEM;// TABS.CarrierAccount.SYSTEM;
                        index++; Z.ServicesFlag = short.Parse(reader[index].ToString());
                        index++; Z.IsMobile = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; Z.IsProper = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; Z.IsSold = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; Z.BeginEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; Z.EndEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        //index++; index++; Z.User = null;
                        lstownZones.Add(Z);
                        Z = null;
                    }
                    comm.Connection.Dispose();
                    conn.Close();
                    reader.Dispose();
                    reader = null;
                    comm.Dispose();
                    comm = null;
                    foreach (Zone zone in lstownZones)
                    {
                        dicOwnZones[zone.ZoneID] = zone;
                        zone.EffectiveCodes = new List<Code>();
                    }
                    lstownZones = null;
                }
                return dicOwnZones;
            }
            catch (Exception EX)
            {
                throw new Exception("MyObjectAssembler_New->Get_OwnZonesDIC(param1):- " + EX.Message);
            }
        }

        public List<Code> Get_OwnCodes(DateTime when)
        {
            List<Code> LstOwnCodes = new List<Code>();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;//System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    SqlCommand comm = new SqlCommand(" SELECT  C.ID,C.Code,C.ZoneID,C.BeginEffectiveDate,C.EndEffectiveDate FROM Code C  WITH (NOLOCK)  inner join Zone Z  WITH (NOLOCK)  on C.ZoneID=Z.ZoneID WHERE  Z.SupplierID = 'SYS' AND ((C.BeginEffectiveDate <= '" + when.ToString("yyyy-MM-dd") + "' AND (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate > '" + when.ToString("yyyy-MM-dd") + "')) OR C.BeginEffectiveDate > '" + when.ToString("yyyy-MM-dd") + "')  ORDER BY C.Code", conn);
                    comm.Connection = conn;
                    conn.Open();
                    SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    int index = -1;
                    int zoneID = -1;
                    Dictionary<int, Zone> _OwnZones = Get_OwnZonesDIC(when);
                    while (reader.Read())
                    {
                        Code C = new Code();
                        index = -1;
                        index++; C.ID = int.Parse(reader[index].ToString());
                        index++; C.Value = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; zoneID = int.Parse(reader[index].ToString()); C.Zone = _OwnZones.ContainsKey(zoneID) ? _OwnZones[zoneID] : null;
                        index++; C.BeginEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; C.EndEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        //index++; index++; C.User = null;
                        if (C.Zone != null)
                            LstOwnCodes.Add(C);
                        C = null;
                    }
                    comm.Connection.Dispose();
                    conn.Close();
                    reader.Dispose();
                    reader = null;
                    comm.Dispose();
                    comm = null;
                    _OwnZones = null;
                }
                return LstOwnCodes;
            }
            catch (Exception EX)
            {
                throw new Exception("Get_OwnCodes(param1):- " + EX.Message);
            }
        }

        public List<Code> GetAllSupplierCodes(DateTime whenEffective, Dictionary<string, CarrierAccount> _Suppliers, Dictionary<CarrierAccount, List<Zone>> _SuppliedZones)
        {
            List<Code> LstCodesSuppliers = new List<Code>();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    SqlCommand comm = new SqlCommand(@" SELECT Z.SupplierID,C.ID,C.Code,C.ZoneID,C.BeginEffectiveDate,C.EndEffectiveDate FROM Code C  WITH (NOLOCK)  inner join Zone Z  WITH (NOLOCK)  on C.ZoneID=Z.ZoneID 
                            WHERE  Z.SupplierID<>'SYS' And C.BeginEffectiveDate <= '" + whenEffective.ToString("yyyy-MM-dd") + "' AND (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate >= '" + whenEffective.ToString("yyyy-MM-dd") + "' ) ORDER BY C.Code", conn);

                    comm.Connection = conn;
                    conn.Open();
                    SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    int index = -1;

                    int zoneID;

                    // DateTime start = DateTime.Now;
                    while (reader.Read())
                    {
                        index = -1;
                        index++; CarrierAccount Supplier = !_Suppliers.ContainsKey(reader[index].ToString()) ? null : _Suppliers[reader[index].ToString()];
                        if (Supplier != null)
                        {
                            Code C = new Code();
                            index++; C.ID = int.Parse(reader[index].ToString());
                            index++; C.Value = reader.IsDBNull(index) ? "" : reader[index].ToString();
                            index++; zoneID = int.Parse(reader[index].ToString()); C.Zone = _SuppliedZones.ContainsKey(Supplier) ? _SuppliedZones[Supplier].Where(z => z.Equals(zoneID)).FirstOrDefault() : null;
                            if (C.Zone == null) continue;
                            index++; C.BeginEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                            index++; C.EndEffectiveDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                            //index++; index++; C.User = null;

                            LstCodesSuppliers.Add(C);
                            C = null;
                        }
                        Supplier = null;
                    }
                    comm.Connection.Dispose();
                    conn.Close();
                    reader.Dispose();
                    reader = null;
                    comm.Dispose();
                    comm = null;
                }
                // TimeSpan spent = DateTime.Now.Subtract(start);
            }
            catch (Exception EX)
            {
                throw new Exception("MyObjectAssembler_New->GetAllSupplierCodes(param1,param2,param3):- " + EX.Message);
            }
            return LstCodesSuppliers;
        }

        public Dictionary<string, CarrierAccount> GetAllSuppliers(Dictionary<int, CarrierProfile> _CarrierProfiles, Dictionary<int, CarrierGroup> _CarrierGroups)
        {
            Dictionary<string, CarrierAccount> dicSupplires = new Dictionary<string, CarrierAccount>();

            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    SqlCommand comm = new SqlCommand(@" select CarrierAccountID,ProfileID,ServicesFlag,ActivationStatus,RoutingStatus,AccountType,CustomerPaymentType,SupplierPaymentType,SupplierCreditLimit
      ,BillingCycleFrom,BillingCycleTo,GMTTime,IsTOD,IsDeleted,IsOriginatingZonesEnabled,Notes
      ,NominalCapacityInE1s,UserID,timestamp,CarrierGroupID,RateIncreaseDays,BankReferences,CustomerCreditLimit
      ,IsPassThroughCustomer,IsPassThroughSupplier,RepresentsASwitch,IsAToZ,NameSuffix,SupplierRatePolicy ,CustomerGMTTime,ImportEmail
      ,ImportSubjectCode,IsNettingEnabled ,Services,ConnectionFees,CustomerActivateDate,CustomerDeactivateDate,SupplierActivateDate,SupplierDeactivateDate
      ,InvoiceSerialPattern,CustomerMask,IsCustomerCeiling,IsSupplierCeiling,CarrierGroups,CodeView,IsCustomCodeView,AutomaticInvoiceSettingID,CarrierMask,IsProduct from CarrierAccount  WITH (NOLOCK)  Where (AccountType=2 or AccountType=1) and IsDeleted='N'", conn);



                    comm.Connection = conn;
                    conn.Open();
                    SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    int index = -1;


                    while (reader.Read())
                    {
                        CarrierAccount C = new CarrierAccount();
                        index = -1;
                        index++; C.CarrierAccountID = reader[index].ToString();
                        index++; C.CarrierProfile = _CarrierProfiles[int.Parse(reader[index].ToString())];
                        index++; C.ServicesFlag = short.Parse(reader[index].ToString());
                        index++; //C.ActivationStatus = (ActivationStatus)int.Parse(reader[index].ToString());
                        index++; //C.RoutingStatus = (RoutingStatus)int.Parse(reader[index].ToString());
                        index++; C.AccountType = (AccountType)int.Parse(reader[index].ToString());
                        index++; C.CustomerPaymentType = (PaymentType)int.Parse(reader[index].ToString());
                        index++; C.SupplierPaymentType = (PaymentType)int.Parse(reader[index].ToString());
                        index++; C.SupplierCreditLimit = int.Parse(reader[index].ToString());
                        index++; index++; index++; C.SupplierGMTTime = short.Parse(reader[index].ToString());
                        index++; C.IsTOD = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; C.IsDeleted = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; C.IsOriginatingZonesEnabled = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.Notes = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.NominalCapacityInE1s = int.Parse(reader[index].ToString());
                        index++; C.User = null;
                        index++; index++; C.CarrierGroup = reader.IsDBNull(index) ? null : _CarrierGroups[int.Parse(reader[index].ToString())];
                        index++; C.RateIncreaseDays = reader.IsDBNull(index) ? null : (int?)int.Parse(reader[index].ToString());
                        index++; //C.BankReferences = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.CustomerCreditLimit = int.Parse(reader[index].ToString());
                        index++; C.IsPassThroughCustomer = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; C.IsPassThroughSupplier = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.RepresentsASwitch = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.IsAToZ = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; C.NameSuffix = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.SupplierRatePolicy = (SupplierRatePolicy)int.Parse(reader[index].ToString());
                        index++; C.CustomerGMTTime = short.Parse(reader[index].ToString());
                        index++; //C.ImportEmail = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.ImportSubjectCode = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.IsNettingEnabled = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; index++; //C.ConnectionFees = decimal.Parse(reader[index].ToString());
                        index++; //C.CustomerActivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.CustomerDeactivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.SupplierActivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.SupplierDeactivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.InvoiceSerialPattern = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.CustomerMask = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.IsCustomerCeiling = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.IsSupplierCeiling = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.CarrierGroups = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.CodeView = int.Parse(reader[index].ToString());
                        index++; //C.IsCustomCodeView = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;

                        dicSupplires[C.CarrierAccountID] = C;
                        C = null;
                    }
                    comm.Connection.Dispose();
                    conn.Close();
                    reader.Dispose();
                    reader = null;
                    comm.Dispose();
                    comm = null;
                }
                return dicSupplires;
            }
            catch (Exception EX)
            {
                throw new Exception("MyObjectAssembler_New->GetAllSuppliers(param1,param2):- " + EX.Message);
            }
        }

        public Dictionary<string, CarrierAccount> GetAllCustomers(Dictionary<int, CarrierProfile> _CarrierProfiles, Dictionary<int, CarrierGroup> _CarrierGroups)
        {
            Dictionary<string, CarrierAccount> dicSupplires = new Dictionary<string, CarrierAccount>();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    SqlCommand comm = new SqlCommand(@" select CarrierAccountID,ProfileID,ServicesFlag,ActivationStatus,RoutingStatus,AccountType,CustomerPaymentType,SupplierPaymentType,SupplierCreditLimit
      ,BillingCycleFrom,BillingCycleTo,GMTTime,IsTOD,IsDeleted,IsOriginatingZonesEnabled,Notes
      ,NominalCapacityInE1s,UserID,timestamp,CarrierGroupID,RateIncreaseDays,BankReferences,CustomerCreditLimit
      ,IsPassThroughCustomer,IsPassThroughSupplier,RepresentsASwitch,IsAToZ,NameSuffix,SupplierRatePolicy ,CustomerGMTTime,ImportEmail
      ,ImportSubjectCode,IsNettingEnabled ,Services,ConnectionFees,CustomerActivateDate,CustomerDeactivateDate,SupplierActivateDate,SupplierDeactivateDate
      ,InvoiceSerialPattern,CustomerMask,IsCustomerCeiling,IsSupplierCeiling,CarrierGroups,CodeView,IsCustomCodeView,AutomaticInvoiceSettingID,CarrierMask,IsProduct from CarrierAccount  WITH (NOLOCK)  Where (AccountType=0 or AccountType=1) and IsDeleted='N'", conn);



                    comm.Connection = conn;
                    conn.Open();
                    SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    int index = -1;

                    while (reader.Read())
                    {
                        CarrierAccount C = new CarrierAccount();
                        index = -1;
                        index++; C.CarrierAccountID = reader[index].ToString();
                        index++; C.CarrierProfile = _CarrierProfiles[int.Parse(reader[index].ToString())];
                        index++; C.ServicesFlag = short.Parse(reader[index].ToString());
                        index++; //C.ActivationStatus = (ActivationStatus)int.Parse(reader[index].ToString());
                        index++; //C.RoutingStatus = (RoutingStatus)int.Parse(reader[index].ToString());
                        index++; C.AccountType = (AccountType)int.Parse(reader[index].ToString());
                        index++; C.CustomerPaymentType = (PaymentType)int.Parse(reader[index].ToString());
                        index++; C.SupplierPaymentType = (PaymentType)int.Parse(reader[index].ToString());
                        index++; C.SupplierCreditLimit = int.Parse(reader[index].ToString());
                        index++; index++; index++; C.SupplierGMTTime = short.Parse(reader[index].ToString());
                        index++; C.IsTOD = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; C.IsDeleted = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; C.IsOriginatingZonesEnabled = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.Notes = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.NominalCapacityInE1s = int.Parse(reader[index].ToString());
                        index++; C.User = null;
                        index++; index++; C.CarrierGroup = reader.IsDBNull(index) ? null : _CarrierGroups[int.Parse(reader[index].ToString())];
                        index++; C.RateIncreaseDays = reader.IsDBNull(index) ? null : (int?)int.Parse(reader[index].ToString());
                        index++; //C.BankReferences = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.CustomerCreditLimit = int.Parse(reader[index].ToString());
                        index++; C.IsPassThroughCustomer = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; C.IsPassThroughSupplier = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.RepresentsASwitch = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; C.IsAToZ = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; C.NameSuffix = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.SupplierRatePolicy = (SupplierRatePolicy)int.Parse(reader[index].ToString());
                        index++; C.CustomerGMTTime = short.Parse(reader[index].ToString());
                        index++; //C.ImportEmail = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.ImportSubjectCode = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.IsNettingEnabled = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; index++; //C.ConnectionFees = decimal.Parse(reader[index].ToString());
                        index++; //C.CustomerActivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.CustomerDeactivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.SupplierActivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.SupplierDeactivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.InvoiceSerialPattern = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.CustomerMask = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.IsCustomerCeiling = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.IsSupplierCeiling = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.CarrierGroups = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.CodeView = int.Parse(reader[index].ToString());
                        index++; //C.IsCustomCodeView = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;

                        dicSupplires[C.CarrierAccountID] = C;
                        C = null;
                    }
                    comm.Connection.Dispose();
                    conn.Close();
                    reader.Dispose();
                    reader = null;
                    comm.Dispose();
                    comm = null;
                }
            }
            catch (Exception EX)
            {
                throw new Exception("MyObjectAssembler_New->GetAllCustomers(param1,param2):- " + EX.Message);
            }
            return dicSupplires;
        }

        public Dictionary<string, CarrierAccount> GetAllCarrierAccounts(Dictionary<int, CarrierProfile> _CarrierProfiles, Dictionary<int, CarrierGroup> _CarrierGroups)
        {
            Dictionary<string, CarrierAccount> dicSupplires = new Dictionary<string, CarrierAccount>();

            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    SqlCommand comm = new SqlCommand(@" select CarrierAccountID,ProfileID,ServicesFlag,ActivationStatus,RoutingStatus,AccountType,CustomerPaymentType,SupplierPaymentType,SupplierCreditLimit
      ,BillingCycleFrom,BillingCycleTo,GMTTime,IsTOD,IsDeleted,IsOriginatingZonesEnabled,Notes
      ,NominalCapacityInE1s,UserID,timestamp,CarrierGroupID,RateIncreaseDays,BankReferences,CustomerCreditLimit
      ,IsPassThroughCustomer,IsPassThroughSupplier,RepresentsASwitch,IsAToZ,NameSuffix,SupplierRatePolicy ,CustomerGMTTime,ImportEmail
      ,ImportSubjectCode,IsNettingEnabled ,Services,ConnectionFees,CustomerActivateDate,CustomerDeactivateDate,SupplierActivateDate,SupplierDeactivateDate
      ,InvoiceSerialPattern,CustomerMask,IsCustomerCeiling,IsSupplierCeiling,CarrierGroups,CodeView,IsCustomCodeView,AutomaticInvoiceSettingID,CarrierMask,IsProduct  from CarrierAccount  WITH (NOLOCK)  where IsDeleted='N'", conn);



                    comm.Connection = conn;
                    conn.Open();
                    SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    int index = -1;

                    while (reader.Read())
                    {
                        CarrierAccount C = new CarrierAccount();
                        index = -1;
                        index++; C.CarrierAccountID = reader[index].ToString();
                        index++; C.CarrierProfile = _CarrierProfiles.ContainsKey(int.Parse(reader[index].ToString())) ? _CarrierProfiles[int.Parse(reader[index].ToString())] : null;
                        index++; C.ServicesFlag = short.Parse(reader[index].ToString());
                        index++; C.ActivationStatus = (ActivationStatus)int.Parse(reader[index].ToString());
                        index++; C.RoutingStatus = (RoutingStatus)int.Parse(reader[index].ToString());
                        index++; C.AccountType = (AccountType)int.Parse(reader[index].ToString());
                        index++; C.CustomerPaymentType = (PaymentType)int.Parse(reader[index].ToString());
                        index++; C.SupplierPaymentType = (PaymentType)int.Parse(reader[index].ToString());
                        index++; C.SupplierCreditLimit = int.Parse(reader[index].ToString());
                        index++; index++; index++; C.SupplierGMTTime = short.Parse(reader[index].ToString());
                        index++; C.IsTOD = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; C.IsDeleted = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; C.IsOriginatingZonesEnabled = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.Notes = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.NominalCapacityInE1s = int.Parse(reader[index].ToString());
                        index++; C.User = null;
                        index++; index++; C.CarrierGroup = reader.IsDBNull(index) ? null : _CarrierGroups[int.Parse(reader[index].ToString())];
                        index++; C.RateIncreaseDays = reader.IsDBNull(index) ? null : (int?)int.Parse(reader[index].ToString());
                        index++; C.BankReferences = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.CustomerCreditLimit = int.Parse(reader[index].ToString());
                        index++; C.IsPassThroughCustomer = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; C.IsPassThroughSupplier = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; C.RepresentsASwitch = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; C.IsAToZ = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; C.NameSuffix = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.SupplierRatePolicy = (SupplierRatePolicy)int.Parse(reader[index].ToString());
                        index++; C.CustomerGMTTime = short.Parse(reader[index].ToString());
                        index++; C.ImportEmail = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.ImportSubjectCode = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.IsNettingEnabled = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; index++; C.ConnectionFees = decimal.Parse(reader[index].ToString());
                        index++; C.CustomerActivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; C.CustomerDeactivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; C.SupplierActivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; C.SupplierDeactivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; C.InvoiceSerialPattern = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.CustomerMask = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.IsCustomerCeiling = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.IsSupplierCeiling = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.CarrierGroups = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.CodeView = int.Parse(reader[index].ToString());
                        index++; C.IsCustomCodeView = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;

                        dicSupplires[C.CarrierAccountID] = C;
                        C = null;
                    }
                    comm.Connection.Dispose();
                    conn.Close();
                    reader.Dispose();
                    reader = null;
                    comm.Dispose();
                    comm = null;
                }
                return dicSupplires;
            }
            catch (Exception EX)
            {
                throw new Exception("MyObjectAssembler_New->GetAllCarrierAccounts(param1,param2):- " + EX.Message);
            }
        }

        public Dictionary<int, CarrierProfile> GetAllCarrierProfile()
        {
            Dictionary<int, CarrierProfile> dicProfiles = new Dictionary<int, CarrierProfile>();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    SqlCommand comm = new SqlCommand(@"select 
          ProfileID,Name,CompanyName,CompanyLogo,CompanyLogoName,Address1,Address2,Address3,Country,City,Telephone
         ,Fax,BillingContact,BillingEmail,PricingContact,PricingEmail,SupportContact,SupportEmail,CurrencyID,DuePeriod,PaymentTerms,Tax1
         ,Tax2,IsTaxAffectsCost,TaxFormula,VAT,Services,ConnectionFees,IsDeleted,BankingDetails,UserID,timestamp,RegistrationNumber
        ,EscalationLevel ,Guarantee,CustomerPaymentType,SupplierPaymentType,CustomerCreditLimit,SupplierCreditLimit,IsNettingEnabled,CustomerActivateDate
        ,CustomerDeactivateDate ,SupplierActivateDate,SupplierDeactivateDate ,VatID ,VatOffice ,AccountManagerEmail ,InvoiceByProfile,SMSPhoneNumber 
        from CarrierProfile  WITH (NOLOCK) ", conn);



                    comm.Connection = conn;
                    conn.Open();
                    SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    int index = -1;
                    Dictionary<string, Currency> _Currencies = GetCurrencies();
                    while (reader.Read())
                    {
                        CarrierProfile C = new CarrierProfile();
                        index = -1;
                        index++; C.ProfileID = int.Parse(reader[index].ToString());
                        index++; C.Name = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.CompanyName = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.CompanyLogo = reader.IsDBNull(index) ? null : (byte[])reader[index];
                        index++; //C.CompanyLogoName = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.Address1 = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.Address2 = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.Address3 = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.Country = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.City = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.Telephone = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.Fax = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.BillingContact = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.BillingEmail = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.PricingContact = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.PricingEmail = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.SupportContact = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.SupportEmail = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.Currency = reader.IsDBNull(index) ? null : _Currencies.ContainsKey(reader[index].ToString()) ? _Currencies[reader[index].ToString()] : null;
                        index++; //C.DuePeriod = reader[index].ToString() == "0.00" ? 0 : int.Parse(reader[index].ToString());
                        index++; //C.PaymentTerms = reader[index].ToString() == "0.00" ? 0 : int.Parse(reader[index].ToString());
                        index++; //C.Tax1 = decimal.Parse(reader[index].ToString());
                        index++; //C.Tax2 = decimal.Parse(reader[index].ToString());
                        index++; C.IsTaxAffectsCost = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.TaxFormula = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.VAT = decimal.Parse(reader[index].ToString());
                        index++; index++; index++; C.IsDeleted = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.BankingDetails = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.User = null;
                        index++; index++; //C.RegistrationNumber = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.EscalationLevel = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.Guarantee = reader.IsDBNull(index) ? 0 : (double?)double.Parse(reader[index].ToString());
                        index++; C.CustomerPaymentType = reader.IsDBNull(index) ? (PaymentType)0 : (PaymentType)int.Parse(reader[index].ToString());
                        index++; C.SupplierPaymentType = reader.IsDBNull(index) ? (PaymentType)0 : (PaymentType)int.Parse(reader[index].ToString());
                        index++; C.CustomerCreditLimit = reader.IsDBNull(index) ? 0 : int.Parse(reader[index].ToString());
                        index++; C.SupplierCreditLimit = reader.IsDBNull(index) ? 0 : int.Parse(reader[index].ToString());
                        index++; C.IsNettingEnabled = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.CustomerActivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.CustomerDeactivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.SupplierActivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.SupplierDeactivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; C.VatID = reader[index].ToString();
                        index++; //C.VatOffice = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.AccountManagerEmail = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.InvoiceByProfile = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        dicProfiles[C.ProfileID] = C;
                        C = null;
                    }
                    comm.Connection.Dispose();
                    conn.Close();
                    reader.Dispose();
                    reader = null;
                    comm.Dispose();
                    comm = null;
                    _Currencies = null;
                }
            }
            catch (Exception EX)
            {
                throw new Exception("MyObjectAssembler_New->GetAllCarrierProfile():- " + EX.Message);
            }
            return dicProfiles;
        }

        public Dictionary<string, Currency> GetCurrencies()
        {
            Dictionary<string, Currency> diccurrencies = new Dictionary<string, Currency>();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    SqlCommand comm = new SqlCommand(@"select CurrencyID,Name,IsMainCurrency,IsVisible,LastRate,LastUpdated,UserID,timestamp from Currency  WITH (NOLOCK)  where IsVisible='Y'", conn);



                    comm.Connection = conn;
                    conn.Open();
                    SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    int index = -1;

                    while (reader.Read())
                    {
                        Currency C = new Currency();
                        index = -1;
                        index++; C.Symbol = reader[index].ToString();
                        index++; C.Name = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.IsMainCurrency = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.IsVisible = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.LastRate = reader.IsDBNull(index) ? 0 : (float)float.Parse(reader[index].ToString());
                        index++; //C.LastUpdated = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; index++; C.User = null;
                        diccurrencies[C.Symbol] = C;
                        C = null;
                    }
                    comm.Connection.Dispose(); conn.Close();
                    reader.Dispose(); reader = null;
                    comm.Dispose(); comm = null;
                }
                return diccurrencies;
            }
            catch (Exception EX)
            {
                throw new Exception("MyObjectAssembler_New->GetCurrencies():- " + EX.Message);
            }
        }

        public Dictionary<int, CarrierGroup> GetAllCarrierGroups()
        {
            Dictionary<int, CarrierGroup> dicCarrierGroups = new Dictionary<int, CarrierGroup>();

            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    SqlCommand comm = new SqlCommand(@"select CarrierGroupID,CarrierGroupName,ParentID,ParentPath,Path from CarrierGroup  WITH (NOLOCK) ", conn);



                    comm.Connection = conn;
                    conn.Open();
                    SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    int index = -1;

                    while (reader.Read())
                    {
                        CarrierGroup C = new CarrierGroup();
                        index = -1;
                        index++; C.CarrierGroupID = int.Parse(reader[index].ToString());
                        index++; C.CarrierGroupName = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.ParentGroup = reader.IsDBNull(index) ? null : GetCarrierGroup(int.Parse(reader[index].ToString()));
                        index++; //C.ParentPath = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        dicCarrierGroups[C.CarrierGroupID] = C;
                        C = null;
                    }
                    comm.Connection.Dispose(); conn.Close();
                    reader.Dispose(); reader = null;
                    comm.Dispose(); comm = null;
                }
                return dicCarrierGroups;
            }
            catch (Exception EX)
            {
                throw new Exception("MyObjectAssembler_New->GetAllCarrierGroups():- " + EX.Message);
            }
        }

        public CarrierGroup GetCarrierGroup(int CarrierGroupID)
        {
            CarrierGroup _CarrierGroup = null;
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    SqlCommand comm = new SqlCommand(@"select CarrierGroupID,CarrierGroupName,ParentID,ParentPath,Path  from CarrierGroup  WITH (NOLOCK)   where CarrierGroupID=" + CarrierGroupID, conn);

                    comm.Connection = conn;
                    conn.Open();


                    SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    int index = -1;

                    while (reader.Read())
                    {
                        _CarrierGroup = new CarrierGroup();
                        index = -1;
                        index++; _CarrierGroup.CarrierGroupID = int.Parse(reader[index].ToString());
                        index++; _CarrierGroup.CarrierGroupName = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; _CarrierGroup.ParentGroup = reader.IsDBNull(index) ? null : GetCarrierGroup(int.Parse(reader[index].ToString()));
                        index++; _CarrierGroup.ParentPath = reader.IsDBNull(index) ? "" : reader[index].ToString();
                    }
                    comm.Connection.Dispose(); conn.Close();
                    reader.Dispose(); reader = null;
                    comm.Dispose(); comm = null;
                }
                return _CarrierGroup;
            }
            catch (Exception EX)
            {
                throw new Exception("MyObjectAssembler_New->GetCarrierGroup(param1):- " + EX.Message);
            }
        }

        //        public System.Data.DataTable Get_Cdr_Main(long lastPricedCdrID, int BatchSize, EffectiveAllData _EffectiveAllDataNeeded, out List<DateTime> DistinctAttempts, out Dictionary<long, TABS.PricingTimeShift> _DICTimeShifts)
        //        {
        //            System.Data.DataTable dt = new System.Data.DataTable();
        //            try
        //            {
        //                string QueryString = string.Format(@"SELECT top {0} ID ,Attempt ,Alert
        //      ,Connect ,Disconnect,DurationInSeconds,CustomerID,OurZoneID,OriginatingZoneID,SupplierID ,SupplierZoneID
        //      ,CDPN ,CGPN,ReleaseCode,ReleaseSource
        //      ,SwitchID,SwitchCdrID,Tag ,Extra_Fields,Port_IN,Port_OUT,OurCode,SupplierCode ,CDPNOut,SubscriberID,SIP FROM Billing_CDR_Main BM   WITH (NOLOCK,index=PK_Billing_CDR_Main)  WHERE BM.ID > {1} ORDER BY BM.ID,BM.Attempt", BatchSize, lastPricedCdrID);//string.Format();//PricedCdrID
        //                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
        //                conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
        //                SqlCommand comm = new SqlCommand(QueryString, conn);
        //                comm.Connection = conn;
        //                System.Data.SqlClient.SqlDataAdapter da = new SqlDataAdapter();
        //                da.SelectCommand = comm;
        //                da.Fill(dt);

        //                Dictionary<long, TABS.PricingTimeShift> DicTimeShift = new Dictionary<long, PricingTimeShift>();
        //                string SuplierID = "";
        //                CarrierAccount _Account;

        //                List<DateTime> L = new List<DateTime>();
        //                if (dt.Rows.Count > 0)
        //                    foreach (System.Data.DataRow dr in dt.Rows)
        //                    {
        //                        string Sdate = DateTime.Parse(dr["Attempt"].ToString()).ToString("yyyy-MM-dd");//.ToString()
        //                        L.Add(DateTime.Parse(Sdate));


        //                        SuplierID = dr["SupplierID"].ToString();
        //                        _Account = _EffectiveAllDataNeeded.EffectiveCachedData.Supplires.ContainsKey(SuplierID) ? _EffectiveAllDataNeeded.EffectiveCachedData.Supplires[SuplierID] : null;
        //                        TABS.PricingTimeShift PShift = new PricingTimeShift();
        //                        PShift.BillingCDRMainID = long.Parse(dr["ID"].ToString());
        //                        PShift.Attempt = DateTime.Parse(Sdate);
        //                        if (_Account != null)
        //                        {
        //                            string Attempt = dr["Attempt"].ToString();
        //                            PShift.SupplierAttemptTimeShift = _Account.SupplierGMTTime != 0 ? DateTime.Parse(DateTime.Parse(Attempt).AddMinutes(_Account.SupplierGMTTime).ToString("yyyy-MM-dd")) : PShift.Attempt;
        //                        }
        //                        else
        //                            PShift.SupplierAttemptTimeShift = PShift.Attempt;
        //                        DicTimeShift[PShift.BillingCDRMainID] = PShift;
        //                    }
        //                else
        //                    DistinctAttempts = null;

        //                _DICTimeShifts = DicTimeShift;
        //                //DistinctAttempts = L.Select(D => D).Distinct().ToList();
        //                DistinctAttempts = L.Select(D => D).Distinct().ToList().Union(DicTimeShift.Values.Select(d => d.SupplierAttemptTimeShift).Distinct().ToList()).ToList();
        //                comm.Connection.Dispose(); conn.Close();
        //                da.Dispose(); da = null;
        //                comm.Dispose(); comm = null;
        //                _Account = null;
        //                return dt;
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("MyObjectAssembler_New->Get_Cdr_Main(param1,param2):- " + ex.Message);
        //            }
        //        }

        public decimal? GetSystemParameterNumericValue(string sys_CDR_Pricing_Batch_Size, string sys_CDR_Pricing_CDRID, out decimal? _Resultsys_CDR_Pricing_Batch_Size)
        {
            decimal? Resultsys_CDR_Pricing_CDRID = null;
            decimal? Resultsys_CDR_Pricing_Batch_Size = null;
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;//System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    SqlCommand comm = new SqlCommand(@"select NumericValue from SystemParameter where Name ='" + sys_CDR_Pricing_Batch_Size + "'", conn);

                    comm.Connection = conn;
                    conn.Open();
                    Resultsys_CDR_Pricing_Batch_Size = (decimal?)comm.ExecuteScalar();
                    comm.CommandText = "select NumericValue from SystemParameter where Name ='" + sys_CDR_Pricing_CDRID + "'";
                    Resultsys_CDR_Pricing_CDRID = (decimal?)comm.ExecuteScalar();
                    conn.Close();
                    _Resultsys_CDR_Pricing_Batch_Size = Resultsys_CDR_Pricing_Batch_Size;

                    comm.Connection.Dispose(); conn.Close();
                    comm.Dispose(); comm = null;
                }
                return Resultsys_CDR_Pricing_CDRID;
            }
            catch (Exception EX)
            {
                throw new Exception("MyObjectAssembler_New->GetSystemParameterNumericValue():- " + EX.Message);
            }
        }

        public CarrierAccount GetSYSTEMCarrierAccounts(Dictionary<int, CarrierProfile> _CarrierProfiles, Dictionary<int, CarrierGroup> _CarrierGroups)
        {
            CarrierAccount SystemAccount = null;

            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    SqlCommand comm = new SqlCommand(@" select CarrierAccountID,ProfileID,ServicesFlag,ActivationStatus,RoutingStatus,AccountType,CustomerPaymentType,SupplierPaymentType,SupplierCreditLimit
      ,BillingCycleFrom,BillingCycleTo,GMTTime,IsTOD,IsDeleted,IsOriginatingZonesEnabled,Notes
      ,NominalCapacityInE1s,UserID,timestamp,CarrierGroupID,RateIncreaseDays,BankReferences,CustomerCreditLimit
      ,IsPassThroughCustomer,IsPassThroughSupplier,RepresentsASwitch,IsAToZ,NameSuffix,SupplierRatePolicy ,CustomerGMTTime,ImportEmail
      ,ImportSubjectCode,IsNettingEnabled ,Services,ConnectionFees,CustomerActivateDate,CustomerDeactivateDate,SupplierActivateDate,SupplierDeactivateDate
      ,InvoiceSerialPattern,CustomerMask,IsCustomerCeiling,IsSupplierCeiling,CarrierGroups,CodeView,IsCustomCodeView,AutomaticInvoiceSettingID,CarrierMask,IsProduct  from CarrierAccount  WITH (NOLOCK)  where IsDeleted='N' and carrierAccountID='SYS'", conn);



                    comm.Connection = conn;
                    conn.Open();
                    SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    int index = -1;

                    while (reader.Read())
                    {
                        SystemAccount = new CarrierAccount();
                        index = -1;
                        index++; SystemAccount.CarrierAccountID = reader[index].ToString();
                        index++; //C.CarrierProfile = _CarrierProfiles.ContainsKey(int.Parse(reader[index].ToString())) ? _CarrierProfiles[int.Parse(reader[index].ToString())] : null;
                        index++; //C.ServicesFlag = short.Parse(reader[index].ToString());
                        index++; //C.ActivationStatus = (ActivationStatus)int.Parse(reader[index].ToString());
                        index++; //C.RoutingStatus = (RoutingStatus)int.Parse(reader[index].ToString());
                        index++; //C.AccountType = (AccountType)int.Parse(reader[index].ToString());
                        index++; //C.CustomerPaymentType = (PaymentType)int.Parse(reader[index].ToString());
                        index++; //C.SupplierPaymentType = (PaymentType)int.Parse(reader[index].ToString());
                        index++; //C.SupplierCreditLimit = int.Parse(reader[index].ToString());
                        index++; index++; index++; SystemAccount.SupplierGMTTime = short.Parse(reader[index].ToString());
                        index++; SystemAccount.IsTOD = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; SystemAccount.IsDeleted = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.IsOriginatingZonesEnabled = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.Notes = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.NominalCapacityInE1s = int.Parse(reader[index].ToString());
                        index++; SystemAccount.User = null;
                        index++; index++; SystemAccount.CarrierGroup = reader.IsDBNull(index) ? null : _CarrierGroups[int.Parse(reader[index].ToString())];
                        index++; //C.RateIncreaseDays = reader.IsDBNull(index) ? null : (int?)int.Parse(reader[index].ToString());
                        index++; //C.BankReferences = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.CustomerCreditLimit = int.Parse(reader[index].ToString());
                        index++; //C.IsPassThroughCustomer = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.IsPassThroughSupplier = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.RepresentsASwitch = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.IsAToZ = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; SystemAccount.NameSuffix = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.SupplierRatePolicy = (SupplierRatePolicy)int.Parse(reader[index].ToString());
                        index++; SystemAccount.CustomerGMTTime = short.Parse(reader[index].ToString());
                        index++; //C.ImportEmail = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.ImportSubjectCode = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; SystemAccount.IsNettingEnabled = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; index++; //C.ConnectionFees = decimal.Parse(reader[index].ToString());
                        index++; //C.CustomerActivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.CustomerDeactivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.SupplierActivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.SupplierDeactivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.InvoiceSerialPattern = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.CustomerMask = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; SystemAccount.IsCustomerCeiling = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; SystemAccount.IsSupplierCeiling = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.CarrierGroups = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.CodeView = int.Parse(reader[index].ToString());
                        index++; //C.IsCustomCodeView = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        break;
                    }
                    comm.Connection.Dispose(); conn.Close();
                    reader.Dispose(); reader = null;
                    comm.Dispose(); comm = null;
                }
                return SystemAccount;
            }
            catch (Exception EX)
            {
                throw new Exception("MyObjectAssembler_New->GetAllCarrierAccounts(param1,param2):- " + EX.Message);
            }
        }

        public CarrierAccount GetSYSTEMCarrierAccounts()
        {
            CarrierAccount C = null;

            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
                {
                    conn.ConnectionString = ApplicationConfiguration.hibernateconnection_string;// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                    SqlCommand comm = new SqlCommand(@" select CarrierAccountID,ProfileID,ServicesFlag,ActivationStatus,RoutingStatus,AccountType,CustomerPaymentType,SupplierPaymentType,SupplierCreditLimit
      ,BillingCycleFrom,BillingCycleTo,GMTTime,IsTOD,IsDeleted,IsOriginatingZonesEnabled,Notes
      ,NominalCapacityInE1s,UserID,timestamp,CarrierGroupID,RateIncreaseDays,BankReferences,CustomerCreditLimit
      ,IsPassThroughCustomer,IsPassThroughSupplier,RepresentsASwitch,IsAToZ,NameSuffix,SupplierRatePolicy ,CustomerGMTTime,ImportEmail
      ,ImportSubjectCode,IsNettingEnabled ,Services,ConnectionFees,CustomerActivateDate,CustomerDeactivateDate,SupplierActivateDate,SupplierDeactivateDate
      ,InvoiceSerialPattern,CustomerMask,IsCustomerCeiling,IsSupplierCeiling,CarrierGroups,CodeView,IsCustomCodeView,AutomaticInvoiceSettingID,CarrierMask,IsProduct  from CarrierAccount  WITH (NOLOCK)  where IsDeleted='N' and carrierAccountID='SYS'", conn);



                    comm.Connection = conn;
                    conn.Open();
                    SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    int index = -1;
                    Dictionary<int, CarrierProfile> _CarrierProfiles = GetAllCarrierProfile();
                    Dictionary<int, CarrierGroup> _CarrierGroups = GetAllCarrierGroups();
                    while (reader.Read())
                    {
                        C = new CarrierAccount();
                        index = -1;
                        index++; C.CarrierAccountID = reader[index].ToString();
                        index++; //C.CarrierProfile = _CarrierProfiles.ContainsKey(int.Parse(reader[index].ToString())) ? _CarrierProfiles[int.Parse(reader[index].ToString())] : null;
                        index++; //C.ServicesFlag = short.Parse(reader[index].ToString());
                        index++; //C.ActivationStatus = (ActivationStatus)int.Parse(reader[index].ToString());
                        index++; //C.RoutingStatus = (RoutingStatus)int.Parse(reader[index].ToString());
                        index++; //C.AccountType = (AccountType)int.Parse(reader[index].ToString());
                        index++; //C.CustomerPaymentType = (PaymentType)int.Parse(reader[index].ToString());
                        index++; //C.SupplierPaymentType = (PaymentType)int.Parse(reader[index].ToString());
                        index++; //C.SupplierCreditLimit = int.Parse(reader[index].ToString());
                        index++; index++; index++; C.SupplierGMTTime = short.Parse(reader[index].ToString());
                        index++; C.IsTOD = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; C.IsDeleted = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.IsOriginatingZonesEnabled = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.Notes = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.NominalCapacityInE1s = int.Parse(reader[index].ToString());
                        index++; C.User = null;
                        index++; index++; C.CarrierGroup = reader.IsDBNull(index) ? null : _CarrierGroups[int.Parse(reader[index].ToString())];
                        index++; //C.RateIncreaseDays = reader.IsDBNull(index) ? null : (int?)int.Parse(reader[index].ToString());
                        index++; //C.BankReferences = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.CustomerCreditLimit = int.Parse(reader[index].ToString());
                        index++; //C.IsPassThroughCustomer = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.IsPassThroughSupplier = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.RepresentsASwitch = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; //C.IsAToZ = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; C.NameSuffix = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.SupplierRatePolicy = (SupplierRatePolicy)int.Parse(reader[index].ToString());
                        index++; C.CustomerGMTTime = short.Parse(reader[index].ToString());
                        index++; //C.ImportEmail = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.ImportSubjectCode = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.IsNettingEnabled = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        index++; index++; //C.ConnectionFees = decimal.Parse(reader[index].ToString());
                        index++; //C.CustomerActivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.CustomerDeactivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.SupplierActivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.SupplierDeactivateDate = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                        index++; //C.InvoiceSerialPattern = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.CustomerMask = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.IsCustomerCeiling = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; C.IsSupplierCeiling = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.CarrierGroups = reader.IsDBNull(index) ? "" : reader[index].ToString();
                        index++; //C.CodeView = int.Parse(reader[index].ToString());
                        index++; //C.IsCustomCodeView = reader.IsDBNull(index) ? false : char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                        break;
                    }
                    comm.Connection.Dispose(); conn.Close();
                    reader.Dispose(); reader = null;
                    comm.Dispose(); comm = null;
                }
                return C;
            }
            catch (Exception EX)
            {
                throw new Exception("MyObjectAssembler_New->GetAllCarrierAccounts(param1,param2):- " + EX.Message);
            }
        }
    }
}

