using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Summary description for PricelistHelper
/// </summary>
namespace TABS.Addons.Utilities
{
    public class PricelistHelper
    {
        protected TABS.PriceListType PricelistType { get; set; }
        protected TABS.PriceList SavedPricelist { get; set; }

        public PricelistHelper(TABS.PriceList savedPricelist, TABS.PriceListType pricelistType)
        {
            SavedPricelist = savedPricelist;
            PricelistType = pricelistType;
        }

        public TABS.PriceList GetPricelist()
        {
            if (SavedPricelist.Customer.Equals(TABS.CarrierAccount.SYSTEM)) return null;

            var CarrierRates = GetRates();

            Dictionary<string, List<TABS.Code>> currenctAndFutureCodes = GetCodes();

            TABS.PriceList dummyPricelist = new TABS.PriceList();

            dummyPricelist.BeginEffectiveDate = SavedPricelist.BeginEffectiveDate;
            dummyPricelist.EndEffectiveDate = SavedPricelist.EndEffectiveDate;
            dummyPricelist.Currency = SavedPricelist.Currency;
            dummyPricelist.Description = SavedPricelist.Description;
            dummyPricelist.Customer = SavedPricelist.Customer;
            dummyPricelist.Supplier = SavedPricelist.Supplier;
            dummyPricelist.User = SavedPricelist.User != null ? SavedPricelist.User : dummyPricelist.User;
            Dictionary<TABS.Zone, TABS.RateBase> rates = new Dictionary<TABS.Zone, TABS.RateBase>();

            foreach (TABS.Rate rate in CarrierRates.Values)
            {
                TABS.Zone dummyZone = new TABS.Zone();
                dummyZone.BeginEffectiveDate = rate.Zone.BeginEffectiveDate;
                dummyZone.EndEffectiveDate = rate.Zone.EndEffectiveDate;
                dummyZone.Name = rate.Zone.Name;
                dummyZone.Supplier = rate.Zone.Supplier;
                dummyZone.ServicesFlag = rate.Zone.ServicesFlag;
                dummyZone.EffectiveRates = null;
                dummyZone.EffectiveCodes = null;
                dummyZone.EffectiveSpecialRequests = new List<TABS.SpecialRequest>();
                dummyZone.EffectiveRouteBlocks = new List<TABS.RouteBlock>();
                dummyZone.EffectiveTariffs = new List<TABS.Tariff>();
                dummyZone.EffectiveToDConsiderations = new List<TABS.ToDConsideration>();
                dummyZone.EffectiveCommissions = new List<TABS.Commission>();

                TABS.Rate DummyRate = new TABS.Rate();
                DummyRate.PriceList = dummyPricelist;
                DummyRate.BeginEffectiveDate = rate.BeginEffectiveDate;
                DummyRate.EndEffectiveDate = rate.EndEffectiveDate;
                DummyRate.ServicesFlag = rate.ServicesFlag;
                DummyRate.Zone = dummyZone;
                DummyRate.Value = rate.Value;
                DummyRate.OffPeakRate = rate.OffPeakRate;
                DummyRate.WeekendRate = rate.WeekendRate;
                DummyRate.Supplier = rate.Supplier;
                DummyRate.Customer = rate.Customer;
                DummyRate.Notes = rate.Notes;
                DummyRate.Change = SavedPricelist.Rates.Values.Any(r => r.Zone.Name.Equals(dummyZone.Name)) ? rate.Change : TABS.Change.None;

                DummyRate.EffectiveCodes = currenctAndFutureCodes.ContainsKey(dummyZone.Name) ? currenctAndFutureCodes[dummyZone.Name] : new List<TABS.Code>();

                rates[dummyZone] = DummyRate;
            }

            try
            {
                dummyPricelist.Rates = rates.Values
                                      .ToList()
                                      .OrderBy(r => r.Zone.Name)
                                      .ThenBy(r => r.EffectiveCodes[0]).ToDictionary(r => r.Zone);
            }
            catch
            {
                dummyPricelist.Rates = rates.Values.ToList()
                                      .OrderBy(r => r.Zone.Name)
                                      .ToDictionary(r => r.Zone);
            }


            if (PricelistType == TABS.PriceListType.Rate_Change_Pricelist)
                return SavedPricelist;
            else
                return dummyPricelist;
        }
        protected Dictionary<string, List<TABS.Code>> GetCodes()
        {
            IList<TABS.Code> codes = TABS.DataConfiguration.CurrentSession
                        .CreateQuery(string.Format(@"FROM Code C 
                            WHERE 
                            (C.Zone.Supplier = :supplier)  
                            AND ((C.BeginEffectiveDate <= :when AND (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate >= :when)) OR C.BeginEffectiveDate >= :when)"))
                         .SetParameter("when", DateTime.Now)
                         .SetParameter("supplier", TABS.CarrierAccount.SYSTEM)
                         .List<TABS.Code>();

            Dictionary<string, List<TABS.Code>> result = new Dictionary<string, List<TABS.Code>>();

            foreach (TABS.Code code in codes)
            {
                TABS.Zone zone = code.Zone;
                if (!result.ContainsKey(zone.Name))
                    result[zone.Name] = new List<TABS.Code>();

                result[zone.Name].Add(code);
            }
            return result;
        }


        protected Dictionary<TABS.Zone, TABS.Rate> GetRates()
        {
            Dictionary<TABS.Zone, TABS.Rate> result = new Dictionary<TABS.Zone, TABS.Rate>();

            var codegroups = SavedPricelist.Rates.Keys.Select(z => z.CodeGroup.Code);

            IList<TABS.Rate> rates = TABS.DataConfiguration.CurrentSession
                       .CreateQuery(string.Format(@"FROM Rate R 
                          WHERE     
                                R.PriceList.Supplier = :Supplier 
                            AND R.PriceList.Customer = :Customer
                            AND ((R.BeginEffectiveDate <= :when AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate >= :when)) OR R.BeginEffectiveDate >= :when)"))
                         .SetParameter("Supplier", SavedPricelist.Supplier)
                        .SetParameter("Customer", SavedPricelist.Customer)
                        .SetParameter("when", DateTime.Now)
                        .List<TABS.Rate>();

            foreach (TABS.Rate rate in rates)
            {
                if (!result.ContainsKey(rate.Zone))
                    result[rate.Zone] = rate;
                else
                {
                    if (rate.EndEffectiveDate == null)
                        result[rate.Zone] = rate;
                }
            }

            foreach (var rate in SavedPricelist.Rates.Values)
            {
                if (!result.ContainsKey(rate.Zone))
                    result[rate.Zone] = (TABS.Rate)rate;
            }

            if (PricelistType == TABS.PriceListType.Country_Pricelist)
                return result.Values.Where(r => codegroups.Contains(r.Zone.CodeGroup.Code)).ToDictionary(r => r.Zone);
            else
                return result;
        }


        #region ADO functions For PriceList


        public List<TABS.Rate> GetRatesADO(string SupplierID, string CustomerID, DateTime effectivedate, bool IsCountry, string Countries)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat(@"
                     ;WITH 
                     MyZone AS 
                     (
	                    SELECT  z.Zoneid,z.Name,z.CodeGroup 
	                    FROM Zone z with(nolock)
	                    WHERE z.SupplierID='{0}'
                    )
                     ,MyPricelist AS 
                     (
	                    SELECT p.SupplierID,p.PriceListId,p.BeginEffectiveDate,p.CurrencyID ,p.CustomerID
	                    FROM PriceList p with(nolock)
	                    WHERE p.CustomerID='{1}' and p.SupplierID='{0}'
                    ) 
                    ,MyRate AS 
                    (
	                    SELECT * FROM rate r with(nolock) 
	                    WHERE r.BeginEffectiveDate <= '{2}'  AND 
	                    (r.EndEffectiveDate IS NULL OR r.EndEffectiveDate >'{2}')
                         AND EXISTS (SELECT p.PriceListId FROM MyPricelist P WHERE p.PriceListID=r.PriceListID
                        )
                     )
                SELECT			
				
				z.ZoneID,z.CodeGroup,z.Name as ZoneName,r.RateID,r.ServicesFlag,r.Rate,p.CurrencyID,r.OffPeakRate,r.WeekendRate,r.Change,
                r.BeginEffectiveDate AS RateBeginEffectiveDate
                ,r.EndEffectiveDate AS RateEndEffectiveDate
                ,c.Code
                ,c.BeginEffectiveDate AS CodeBeginEffectiveDate
                ,c.EndEffectiveDate AS CodeEndEffectiveDate
                ,p.PriceListID,cu.LastRate,cu.IsMainCurrency,p.BeginEffectiveDate AS PriceListBeginEffectiveDate,u.ID as userID,u.Name as UserName
                ,p.CustomerID,p.SupplierID 
			    INTO #result1
                FROM code c 
                    INNER JOIN MyRate r on c.ZoneID=r.ZoneID
                    INNER JOIN MyZone z on z.ZoneID=r.ZoneID
                    INNER JOIN MyPricelist p on p.PriceListID=r.PriceListID
                    INNER JOIN Currency cu with(nolock) on cu.CurrencyID=p.CurrencyID
                    INNER JOIN [User] u with(nolock) on r.UserID= u.ID
                    
                    WHERE 
                        c.BeginEffectiveDate <= '{2}' AND 
                     (c.EndEffectiveDate IS NULL OR c.EndEffectiveDate >'{2}')",
 SupplierID, CustomerID, effectivedate.ToString("yyyy-MM-dd"));
                  
            if(IsCountry) sb.AppendFormat(" AND Z.CodeGroup IN ({0}) ",Countries);
            sb.Append(@"
               SELECT  * from #result1 r 
               LEFT JOIN Tariff t ON r.ZoneID=t.ZoneID AND r.CustomerID = t.CustomerID AND r.SupplierID = t.SupplierID
               ORDER BY ZoneName ");
                       

            System.Data.SqlClient.SqlConnection connection = (System.Data.SqlClient.SqlConnection)TABS.BusinessEntities.RateBO.GetOpenedConnection();
            System.Data.IDataReader reader = TABS.BusinessEntities.RateBO.ExecuteReader(connection, sb.ToString());
            Dictionary<int, Zone> ZonesByID = new Dictionary<int, Zone>();
            int index;
            Dictionary<int, List<Code>> ListOfCodes = new Dictionary<int, List<Code>>();
            List<Rate> Test = new List<Rate>();
            if (reader != null)
                while (reader.Read())
                {
                    int ZoneID = reader.GetInt32(0);
                    Zone Z;
                    Code code = new Code();

                    int codeOrdinal = reader.GetOrdinal("Code");
                    int codeBEDOrdinal = reader.GetOrdinal("CodeBeginEffectiveDate");
                    int codeEEDOrdinal = reader.GetOrdinal("CodeEndEffectiveDate");

                    if (!ZonesByID.TryGetValue(ZoneID, out Z))
                    {

                        index = -1;

                        Rate rate = new Rate();
                        Zone zone = new Zone();
                        PriceList priceList = new PriceList();
                        rate.PriceList = new TABS.PriceList();
                        rate.PriceList.Currency = new Currency();
                        rate.Zone = zone;
                        Z = rate.Zone;
                        index++; rate.Zone.ZoneID = reader.GetInt32(index);
                        ZonesByID[rate.Zone.ZoneID] = rate.Zone;
                        index++; rate.Zone.CodeGroup = TABS.CodeGroup.All[reader[index].ToString()];
                        index++; rate.Zone.Name = reader[index].ToString();
                        rate.EffectiveCodes = new List<Code>();
                        rate.Zone.EffectiveRate = rate;
                        index++; rate.ID = reader.GetInt64(index);
                        index++; rate.ServicesFlag = short.Parse(reader[index].ToString());
                        index++; rate.Value = reader.GetDecimal(index);
                        index++; rate.PriceList.Currency.Symbol = reader[index].ToString();
                        index++;
                        if (!reader.IsDBNull(index))
                            rate.OffPeakRate = (decimal)reader.GetDecimal(index);
                        else
                            rate.OffPeakRate = null;
                        index++;
                        if (!reader.IsDBNull(index))
                            rate.WeekendRate = (decimal)reader.GetDecimal(index);
                        else
                            rate.WeekendRate = null;

                        rate.Change = new TABS.Change();
                        index++;
                        string change = reader[index].ToString();
                        rate.Change = (TABS.Change)Enum.Parse(typeof(TABS.Change), change);

                        index++;
                        if (!reader.IsDBNull(index)) rate.BeginEffectiveDate = (DateTime)reader.GetDateTime(index); else rate.BeginEffectiveDate = null;
                        index++;
                        if (!reader.IsDBNull(index)) rate.EndEffectiveDate = (DateTime)reader.GetDateTime(index); else rate.EndEffectiveDate = null;
                        index++; //code.Value = reader[index].ToString();
                        index++; //if (!reader.IsDBNull(index)) code.BeginEffectiveDate = (DateTime)reader.GetDateTime(index); else code.BeginEffectiveDate = null;
                        index++; //if (!reader.IsDBNull(index)) code.EndEffectiveDate = (DateTime)reader.GetDateTime(index); else code.EndEffectiveDate = null;
                        index++; rate.PriceList.ID = reader.GetInt32(index);
                        index++;
                        index++;
                        index++; if (!reader.IsDBNull(index)) rate.PriceList.BeginEffectiveDate = (DateTime)reader.GetDateTime(index); else rate.PriceList.BeginEffectiveDate = null;
                        index++; index++; index++; index++;

                        rate.Zone.BeginEffectiveDate = rate.BeginEffectiveDate;
                        rate.Zone.EndEffectiveDate = rate.EndEffectiveDate;
                        rate.Zone.ServicesFlag = rate.Zone.ServicesFlag;
                        // rate.Zone.EffectiveRates = null;
                        // rate.Zone.EffectiveCodes = null;
                        rate.Zone.EffectiveSpecialRequests = new List<TABS.SpecialRequest>();
                        rate.Zone.EffectiveRouteBlocks = new List<TABS.RouteBlock>();
                        rate.Zone.EffectiveTariffs = new List<TABS.Tariff>();
                        rate.Zone.EffectiveToDConsiderations = new List<TABS.ToDConsideration>();
                        rate.Zone.EffectiveCommissions = new List<TABS.Commission>();

                        rate.Zone.EffectiveTariffs = new List<Tariff>();
                        index++; if (!reader.IsDBNull(index))
                        {
                            Tariff tariff = new Tariff();
                            tariff.Zone = rate.Zone;
                            tariff.Customer = SavedPricelist.Customer;
                            tariff.Supplier = SavedPricelist.Supplier;
                            tariff.TariffID = (int)reader.GetInt64(index);
                            index++; if (!reader.IsDBNull(index)) tariff.Zone.ZoneID = reader.GetInt32(index);
                            index++; if (!reader.IsDBNull(index)) tariff.Customer.CarrierAccountID = reader[index].ToString(); else tariff.Customer.CarrierAccountID = null;
                            index++; if (!reader.IsDBNull(index)) tariff.Supplier.CarrierAccountID = reader[index].ToString(); else tariff.Supplier.CarrierAccountID = null;
                            index++; if (!reader.IsDBNull(index)) tariff.CallFee = (decimal)reader.GetDecimal(index);
                            index++; if (!reader.IsDBNull(index)) tariff.FirstPeriodRate = reader.GetDecimal(index);
                            index++; if (!reader.IsDBNull(index)) tariff.FirstPeriod = reader.GetByte(index);
                            index++; if (!reader.IsDBNull(index)) tariff.RepeatFirstPeriod = reader[index].ToString() == "Y" ? true : false;
                            index++; if (!reader.IsDBNull(index)) tariff.FractionUnit = reader.GetByte(index);
                            index++; if (!reader.IsDBNull(index)) tariff.BeginEffectiveDate = (DateTime)reader.GetDateTime(index); else tariff.BeginEffectiveDate = null;
                            index++; if (!reader.IsDBNull(index)) tariff.EndEffectiveDate = (DateTime)reader.GetDateTime(index); else tariff.EndEffectiveDate = null;
                            index++;
                            index++;// tariff.User=
                            rate.Zone.EffectiveTariffs.Add(tariff);
                        }
                        //rate.Zone.EffectiveCodes.Add(code);
                        Test.Add(rate);
                    }

                    code.Value = reader[codeOrdinal].ToString();
                    if (!reader.IsDBNull(codeBEDOrdinal)) code.BeginEffectiveDate = (DateTime)reader.GetDateTime(codeBEDOrdinal); else code.BeginEffectiveDate = null;
                    if (!reader.IsDBNull(codeEEDOrdinal)) code.EndEffectiveDate = (DateTime)reader.GetDateTime(codeEEDOrdinal); else code.EndEffectiveDate = null;
                    Z.EffectiveRate.EffectiveCodes.Add(code);
                }
            return Test;
        }

        public PriceList GetPriceListADO(string SupplierID, string CustomerID, DateTime effectivedate, bool IsCountry, string Countries)
        {
            if (SavedPricelist.Customer.Equals(TABS.CarrierAccount.SYSTEM)) return null;
            List<Rate> AllRates = GetRatesADO(SupplierID, CustomerID, effectivedate,  IsCountry,  Countries);
            Dictionary<TABS.Zone, TABS.RateBase> FinalResult = AllRates.ToDictionary(r => r.Zone, val => val as RateBase);

            TABS.PriceList dummyPricelist = new TABS.PriceList();

            dummyPricelist.ID = SavedPricelist.ID;
            dummyPricelist.BeginEffectiveDate = SavedPricelist.BeginEffectiveDate;
            dummyPricelist.EndEffectiveDate = SavedPricelist.EndEffectiveDate;
            dummyPricelist.Currency = SavedPricelist.Currency;
            dummyPricelist.Description = SavedPricelist.Description;
            dummyPricelist.Customer = SavedPricelist.Customer;
            dummyPricelist.Supplier = SavedPricelist.Supplier;
            dummyPricelist.User = SavedPricelist.User != null ? SavedPricelist.User : dummyPricelist.User;

            try
            {
                dummyPricelist.Rates = FinalResult.Values
                                      .ToList()
                                      .OrderBy(r => r.Zone.Name)
                                      .ThenBy(r => r.EffectiveCodes[0]).ToDictionary(r => r.Zone);
            }
            catch
            {
                dummyPricelist.Rates = FinalResult.Values.ToList()
                                      .OrderBy(r => r.Zone.Name)
                                      .ToDictionary(r => r.Zone);
            }

            FinalResult = null;
            if (PricelistType == TABS.PriceListType.Rate_Change_Pricelist)
                return SavedPricelist;
            else
                return dummyPricelist;
            //  return Result;

        }

        #endregion
    }
}