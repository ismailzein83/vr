using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using NHibernate.Criterion;
using System.Linq;

namespace TABS
{
    public partial class ObjectAssembler
    {
        public static object SyncRoot = new object();
        public static string PricelistCodeKey = "Vanrise200900*";

        /// <summary>
        /// Gets the current Context-Sesitive Session. See <see cref="DataConfiguration.CurrentSession" />
        /// </summary>
        public static NHibernate.ISession CurrentSession { get { return DataConfiguration.CurrentSession; } }

        #region Pricelists and Rates

        /// <summary>
        /// Get the current (effective) supply pricelist for the given carrier account. 
        /// Note: The prices in this price list *may* be not effective, while prices from previous pricelist(s)
        /// may still be effective for this carrier account.
        /// </summary>
        /// <param name="account">The carrier account for which to get the pricelist</param>
        /// <returns>The effective pricelist or Pricelist.None if none.</returns>
        public static PriceList GetEffectivePricelist(CarrierAccount supplier, DateTime when)
        {
            PriceList result = CurrentSession.CreateCriteria(typeof(PriceList))
                .Add(Expression.Eq("Supplier", supplier))
                .Add(Expression.Le("BeginEffectiveDate", when))
                .Add
                (
                    Expression.Or(
                        Expression.Ge("EndEffectiveDate", when),
                        new NullExpression("EndEffectiveDate")
                    )
                )
                .AddOrder(new Order("BeginEffectiveDate", false))
                .SetMaxResults(1)
                .UniqueResult<PriceList>();
            if (result == null) result = PriceList.None;
            return result;
        }

        /// <summary>
        /// Get the Price list Import options for the given supplier. 
        /// </summary>
        /// <param name="supplier">The Supplier for which to check for available import option</param>
        /// <returns>If there is an import option, return it otherwise it would be PricelistImportOption.None</returns>
        public static PricelistImportOption GetPricelistImportOption(CarrierAccount supplier)
        {
            PricelistImportOption result = PricelistImportOption.None;
            PricelistImportOption option = CurrentSession.CreateCriteria(typeof(PricelistImportOption))
                .Add(Expression.Eq("Supplier", supplier))
                .UniqueResult<PricelistImportOption>();
            if (option != null) result = option;
            return result;
        }

        /// <summary>
        /// Get all Custom Time Zone info 
        /// </summary>
        /// <returns></returns>
        public static List<CustomTimeZoneInfo> GetAllTimeZones()
        {
            string HQL = @"SELECT C FROM CustomTimeZoneInfo C";
            IList<CustomTimeZoneInfo> list = CurrentSession.CreateQuery(HQL)
                    .List<CustomTimeZoneInfo>();

            return list.ToList();
        }


        /// <summary>
        /// Get the effective rates from supplied by given supplier to given customer
        /// </summary>        
        /// <returns>a map of Zone-Rate objects...</returns>
        public static Dictionary<Zone, Rate> GetEffectiveRates(CarrierAccount supplier, CarrierAccount customer, DateTime when)
        {
            string HQL = @"SELECT C, R FROM Rate R, Code C 
                                WHERE 
                                        R.PriceList.Supplier = :Supplier 
                                    AND R.PriceList.Customer = :Customer 
                                    AND R.BeginEffectiveDate <= :when
                                    AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate > :when)
                                    AND C.Zone = R.Zone
                                    AND C.Zone.BeginEffectiveDate <= :when
                                    AND (C.Zone.EndEffectiveDate IS NULL OR C.Zone.EndEffectiveDate > :when)
                                    AND C.BeginEffectiveDate <= :when
                                    AND (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate > :when)
                            ORDER BY R.Zone.Name, C.Value";

            IList<object[]> list = CurrentSession.CreateQuery(HQL)
                    .SetParameter("Supplier", supplier)
                    .SetParameter("Customer", customer)
                    .SetParameter("when", when)
                    .List<object[]>();

            Dictionary<Zone, Rate> result = new Dictionary<Zone, Rate>();
            foreach (object[] codeRate in list)
            {
                TABS.Code code = (TABS.Code)codeRate[0];
                TABS.Rate rate = (TABS.Rate)codeRate[1];
                TABS.Zone zone = code.Zone;
                if (!result.ContainsKey(zone))
                {
                    result[zone] = rate;
                    zone.EffectiveCodes = new List<TABS.Code>();
                }
                zone.EffectiveCodes.Add(code);
            }

            return result;
        }



        /// <summary>
        /// Get the last effective rates for a given customer excluding rates fro a givin pricelist
        /// </summary>        
        /// <returns>a map of Zone-Rate objects... </returns>
        public static Dictionary<Zone, Rate> GetLastEffectiveRatesBefore(PriceList pricelist)
        {
            string HQL = @"SELECT R2 FROM Rate R1, Rate R2 
                                WHERE 
                                       R1.PriceList = :pricelist
                                    AND R2.PriceList.ID != R1.PriceList.ID
                                    AND R2.PriceList.Customer = R1.PriceList.Customer
                                    AND R1.Zone = R2.Zone   
                                    AND R2.BeginEffectiveDate <= R1.BeginEffectiveDate
                            ORDER BY R2.Zone.Name, R2.BeginEffectiveDate ASC";

            IList<TABS.Rate> list = TABS.DataConfiguration.CurrentSession.CreateQuery(HQL)
                        .SetParameter("pricelist", pricelist)
                        .List<TABS.Rate>();

            Dictionary<Zone, Rate> result = new Dictionary<Zone, Rate>();
            foreach (Rate rate in list)
                result[rate.Zone] = rate;
            return result;
        }


        /// <summary>
        /// Get the CarrierGroups Related to such CarrierAccount
        /// </summary>        
        /// <returns>a map of int-CarrierGroup objects... </returns>
        public static Dictionary<int, CarrierGroup> GetCarrierGroups(CarrierAccount CarrierAccount)
        {

            char[] Saperator = { ',' };
            Dictionary<int, CarrierGroup> lstcarrierGroups = new Dictionary<int, CarrierGroup>();
            CarrierGroup carrierGroup;


            if (CarrierAccount.CarrierGroups == null || CarrierAccount.CarrierGroups == "") return null;
            if (CarrierAccount.CarrierGroups.ToString().Split(Saperator).Length == 0) return null;


            foreach (string CarrierGroupID in CarrierAccount.CarrierGroups.ToString().Split(Saperator))
            {
                carrierGroup = CarrierGroup.All[int.Parse(CarrierGroupID)];
                lstcarrierGroups[carrierGroup.CarrierGroupID] = carrierGroup;
            }


            return lstcarrierGroups;
        }





        /// <summary>
        /// Get the effective (supplied) rates from the given carrier account
        /// </summary>
        /// <param name="account">The account for which to get the rates</param>
        /// <returns>an map of Zone-Rate objects...</returns>
        public static Dictionary<Zone, Rate> GetEffectiveSupplyRates(CarrierAccount supplier, DateTime when)
        {
            return GetEffectiveRates(supplier, CarrierAccount.SYSTEM, when);
        }

        /// <summary>
        /// get the effective sales rates for a specific account 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="when"></param>
        /// <returns></returns>
        public static Dictionary<Zone, Rate> GetEffectiveSaleRates(CarrierAccount customer, DateTime when)
        {
            return GetEffectiveRates(CarrierAccount.SYSTEM, customer, when);
        }

        /// <summary>
        /// Get the non-effective Supplier pricelists 
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public static IList<PriceList> GetNonEffectiveSupplierPricelist(CarrierAccount supplier)
        {
            return CurrentSession.CreateCriteria(typeof(PriceList))
                     .Add(Expression.Eq("Supplier", supplier))
                     .Add(Expression.Eq("Customer", CarrierAccount.SYSTEM))
                     .Add(Expression.Or(
                            Expression.Gt("BeginEffectiveDate", DateTime.Now),
                            Expression.Le("EndEffectiveDate", DateTime.Now)
                            )
                        )
                     .List<PriceList>();
        }


        /// <summary>
        /// Get the non-effective Customer pricelists
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static IList<PriceList> GetNonEffectiveCustomerPricelist(CarrierAccount customer)
        {
            return CurrentSession.CreateCriteria(typeof(PriceList))
                     .Add(Expression.Eq("Customer", customer))
                     .Add(Expression.Eq("Supplier", CarrierAccount.SYSTEM))
                     .Add(Expression.Or(
                            Expression.Gt("BeginEffectiveDate", DateTime.Now),
                            Expression.Le("EndEffectiveDate", DateTime.Now)
                            )
                        )
                     .List<PriceList>();
        }

        /// <summary>
        /// Get the rates defined for the given Rate Planer
        /// </summary>
        /// <param name="priceList">The price list for which the rates are returned</param>
        /// <returns>The list of rates</returns>
        public static IList<PlaningRate> GetPlaningRates(RatePlan rateplan)
        {
            return CurrentSession.CreateCriteria(typeof(PlaningRate))
                .Add(Expression.Eq("RatePlan", rateplan))
                .List<PlaningRate>();
        }

        /// <summary>
        /// Get the rates defined for the given pricelist
        /// </summary>
        /// <param name="priceList">The price list for which the rates are returned</param>
        /// <returns>The list of rates</returns>
        public static Dictionary<Zone, RateBase> GetRates(PriceListBase priceList)
        {
            // if this is a rate plan...
            if (priceList is RatePlan) return GetRates((RatePlan)priceList);

            // Get rates for this Pricelist
            Dictionary<Zone, RateBase> result = new Dictionary<Zone, RateBase>();
            // The zones dictionary
            Dictionary<int, Zone> zones = new Dictionary<int, Zone>();

            // Get the Rate, Zone and Codes...
            // The Codes are selected based on the effective date of the rate! 
            IList<object[]> list = CurrentSession.CreateQuery(
                    @"SELECT Z, R, C FROM Rate R, Zone Z, Code C 
                        WHERE 
                                R.PriceList=:PriceList
                            AND R.Zone = Z 
                            AND Z = C.Zone
                            AND R.BeginEffectiveDate >= C.BeginEffectiveDate
                            AND (C.EndEffectiveDate IS NULL OR R.EndEffectiveDate IS NULL OR C.EndEffectiveDate >= R.EndEffectiveDate)
                            ORDER BY Z.Name, C.Value
                            ")
                            .SetParameter("PriceList", priceList)
                            .List<object[]>();

            // Get zone, rate and code
            foreach (object[] tupple in list)
            {
                Zone zone = (Zone)tupple[0];
                Rate rate = (Rate)tupple[1];
                Code code = (Code)tupple[2];
                Zone previous;
                if (!zones.TryGetValue(zone.ZoneID, out previous))
                {
                    result[zone] = rate;
                    zones[zone.ZoneID] = zone;
                    zone.EffectiveCodes = new List<Code>();
                    zone.EffectiveCodes.Add(code);
                }
                else
                {
                    previous.EffectiveCodes.Add(code);
                }
            }
            return result;
        }

        /// <summary>
        /// Get the planning rates defined for the given rate plan
        /// </summary>
        /// <param name="priceList">The price list for which the rates are returned</param>
        /// <returns>The list of rates</returns>
        public static Dictionary<Zone, RateBase> GetRates(RatePlan ratePlan)
        {
            Dictionary<Zone, RateBase> result = new Dictionary<Zone, RateBase>();
            // Get the rates defined for the given rate plan
            IList<PlaningRate> rates = CurrentSession.CreateCriteria(typeof(PlaningRate))
                    .Add(Expression.Eq("RatePlan", ratePlan))
                    .List<PlaningRate>();
            foreach (PlaningRate rate in rates)
                if (result.Keys.Contains(rate.Zone) == false)
                    result.Add(rate.Zone, rate);
            return result;
        }

        /// <summary>
        /// Get the current (effective) pricelist for the given Supplier.
        /// Note: The prices in this price list *may* be not effective, while prices from previous pricelist(s) 
        /// may still be effective for this supplier.
        /// </summary>        
        /// <returns>The effective pricelist or Pricelist.None if none.</returns>
        public static PriceList GetEffectiveSupplierPricelist(CarrierAccount supplier)
        {
            PriceList result = CurrentSession.CreateCriteria(typeof(PriceList))
                .Add(Expression.Eq("Customer", CarrierAccount.SYSTEM))
                .Add(Expression.Eq("Supplier", supplier))
                .Add(Expression.Le("BeginEffectiveDate", DateTime.Now))
                .Add
                 (
                    Expression.Or(
                        Expression.Ge("EndEffectiveDate", DateTime.Now),
                        new NullExpression("EndEffectiveDate")
                    )
                 )
                .UniqueResult<PriceList>();
            if (result == null) result = PriceList.None;
            return result;
        }

        /// <summary>
        /// Get the current (effective) pricelist(s) for the given Customer (Custom pricelists).
        /// Note: The rates in these lists *may* not all be effective, while rates from previous 
        /// pricelist(s) may still be effective.
        /// </summary>
        /// <param name="customer">The customer for which to get the pricelists</param>
        /// <returns>The effective pricelists if none.</returns>
        public static PriceList GetEffectiveCustomerPricelist(CarrierAccount customer)
        {
            PriceList result = CurrentSession.CreateCriteria(typeof(PriceList))
                    .Add(Expression.Eq("Customer", customer))
                    .Add(Expression.Le("BeginEffectiveDate", DateTime.Now))
                    .Add
                    (
                        Expression.Or(
                            Expression.Ge("EndEffectiveDate", DateTime.Now),
                            new NullExpression("EndEffectiveDate")
                        )
                    )
                    .UniqueResult<PriceList>();

            if (result == null) result = PriceList.None;

            return result;
        }
        /// <summary>
        /// Get the carriers that belongs to a specific group
        /// </summary>
        /// <returns>A list of Carrier accounts</returns>
        public static IList<CarrierAccount> GetAccountsofGroup(CarrierGroup group)
        {
            //return CurrentSession
            //        .CreateCriteria(typeof(CarrierAccount))
            //        .Add(Expression.Eq("CarrierGroup", group))
            //        .List<CarrierAccount>();
            return CurrentSession
                .CreateCriteria(typeof(CarrierAccount))
                .List<CarrierAccount>()
                .Where(c => !string.IsNullOrEmpty(c.CarrierGroups) && c.CarrierGroups.Split(',').ToList().Contains(group.CarrierGroupID.ToString()))
                .ToList();
        }

        /// <summary>
        /// Get Customers that belongs to specific group
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static List<CarrierAccount> GetAccountsofSetting(AutoInvoiceSetting setting)
        {
            return CurrentSession
                .CreateCriteria(typeof(CarrierAccount))
                //.Add(Expression.Eq("AutoInvoiceSetting", setting))
                .Add(Expression.Eq("Setting", setting))
                .List<CarrierAccount>().ToList();
        }

        /// <summary>
        /// Get all the connection types 
        /// </summary>
        /// <returns>A list of ConnectionTypes for this account</returns>
        public static IList<CarrierAccountConnection> GetCarrierAccountConnections(CarrierAccount account)
        {
            return CurrentSession
                    .CreateCriteria(typeof(CarrierAccountConnection))
                    .Add(Expression.Eq("CarrierAccount", account))
                    .List<CarrierAccountConnection>();
        }

        /// <summary>
        /// Get all pricelists provided by the given supplier
        /// </summary>
        /// <param name="supplier">The supplier carrier account</param>
        /// <returns>A list of pricelists supplied by this account</returns>
        public static IList<PriceList> GetSalePricelists(CarrierAccount customer, bool newestFirst)
        {
            IList<PriceList> results = new List<PriceList>();
            NHibernate.ICriteria criteria = CurrentSession
                .CreateCriteria(typeof(PriceList))
                .Add(Expression.Eq("Supplier", TABS.CarrierAccount.SYSTEM))
                .Add(Expression.Eq("Customer", customer));
            if (newestFirst)
            {
                criteria = criteria
                        .AddOrder(new Order("BeginEffectiveDate", false))
                        .AddOrder(new Order("ID", false));
            }
            results = criteria.List<PriceList>();
            return results;
        }

        /// <summary>
        /// Get all pricelists provided by the given supplier
        /// </summary>
        /// <param name="supplier">The supplier carrier account</param>
        /// <returns>A list of pricelists supplied by this account</returns>
        public static IList<PriceList> GetSalePricelists(CarrierAccount customer, bool newestFirst, int pageindex, int pagesize, out int recordCount)
        {
            IList<PriceList> results = new List<PriceList>();
            NHibernate.ICriteria criteria = CurrentSession
                .CreateCriteria(typeof(PriceList))
                .Add(Expression.Eq("Supplier", TABS.CarrierAccount.SYSTEM))
                .Add(Expression.Eq("Customer", customer));
            if (newestFirst)
            {
                criteria = criteria
                        .AddOrder(new Order("BeginEffectiveDate", false))
                        .AddOrder(new Order("ID", false));
            }
            recordCount = criteria.List<PriceList>().Count;
            results = criteria.SetFirstResult(pagesize * (pageindex - 1)).SetMaxResults(pagesize).List<PriceList>();
            return results;
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------
        public static System.Data.IDataReader GetSalePriceListADO(CarrierAccount customer, bool newestFirst, int From, int To, string WhereCondition, string ColumnName)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat(@"
                        ;with MyPricelist AS 
						(
	                    SELECT p.PriceListID,p.Description,p.SourceFileName,p.UserID,p.BeginEffectiveDate,Name
	                    FROM PriceList p with(nolock)
                        INNER JOIN [User] u with(nolock) on P.UserID= u.ID
	                    WHERE p.SupplierID='SYS'  AND p.CustomerID='{0}'
						  ) 
						SELECT *,ROW_NUMBER() OVER(ORDER BY {4}) AS RowNumber INTO #RESULT FROM MyPricelist {3}
                        SELECT COUNT(*) FROM #RESULT 
                        SELECT * FROM #RESULT WHERE RowNumber BETWEEN {1} AND {2} ORDER BY {4} 
                        ",
                            customer.CarrierAccountID, From, To, WhereCondition, ColumnName);
            System.Data.SqlClient.SqlConnection connection = (System.Data.SqlClient.SqlConnection)TABS.BusinessEntities.RateBO.GetOpenedConnection();
            System.Data.IDataReader reader = TABS.BusinessEntities.RateBO.ExecuteReader(connection, sb.ToString());
            return reader;
        }

        public static byte[] GetExportedSheet(int PriceListID)
        {
            byte[] mdata = new byte[10];
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat(@"
                        SELECT SourceFileBytes FROM PriceListData WHERE PriceListID= {0}
                        ",
                            PriceListID);
            System.Data.SqlClient.SqlConnection connection = (System.Data.SqlClient.SqlConnection)TABS.BusinessEntities.RateBO.GetOpenedConnection();
            System.Data.IDataReader reader = TABS.BusinessEntities.RateBO.ExecuteReader(connection, sb.ToString());
            while (reader.Read()) { mdata = (byte[])reader[0]; }
            return mdata;
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Get all pricelists provided by the given supplier
        /// </summary>
        /// <param name="supplier">The supplier carrier account</param>
        /// <returns>A list of pricelists supplied by this account</returns>
        public static IList<PriceList> GetSupplierPricelists(CarrierAccount supplier, bool newestFirst)
        {
            IList<PriceList> results = new List<PriceList>();
            NHibernate.ICriteria criteria = CurrentSession
                .CreateCriteria(typeof(PriceList))
                .Add(Expression.Eq("Supplier", supplier));
            if (newestFirst)
            {
                criteria = criteria
                        .AddOrder(new Order("BeginEffectiveDate", false))
                        .AddOrder(new Order("ID", false));
            }
            results = criteria.List<PriceList>();
            return results;
        }
        //------------------------------------------------> Bug# 2893 <-----------------------------------------------------------------
        /*
         * 
         *this function has been added For custom paging
         *similar for the function above but plus we are 
         *sending the page size and page index as parameter
         *and getting the record counts as output
         * 
         */
        public static IList<PriceList> GetSupplierPricelists(CarrierAccount supplier, bool newestFirst, int PageIndex, int PageSize, out int RecordsCount)
        {
            NHibernate.ICriteria criteriaCount = CurrentSession
              .CreateCriteria(typeof(PriceList))
              .Add(Expression.Eq("Supplier", supplier));

            NHibernate.ICriteria criteria = CurrentSession
                .CreateCriteria(typeof(PriceList))
                .Add(Expression.Eq("Supplier", supplier));
            if (newestFirst)
            {
                criteria = criteria
                        .AddOrder(new Order("BeginEffectiveDate", false))
                        .AddOrder(new Order("ID", false));
            }
            RecordsCount = criteriaCount.SetProjection(Projections.RowCount()).UniqueResult<int>();
            return criteria.SetFirstResult(PageSize * (PageIndex - 1)).SetMaxResults(PageSize).List<PriceList>();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static System.Data.IDataReader GetSupplierPricelistsADO(CarrierAccount supplier, bool newestFirst, int From, int To, string WhereCondition, string ColumnName)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat(@"
                        ;with MyPricelist AS 
						(
	                    SELECT p.PriceListID,p.Description,p.SourceFileName,p.UserID,p.BeginEffectiveDate,Name
	                    FROM PriceList p with(nolock)
                        INNER JOIN [User] u with(nolock) on P.UserID= u.ID
	                    WHERE p.SupplierID='{0}' 
						  ) 
						SELECT *,ROW_NUMBER() OVER(ORDER BY {4}) AS RowNumber INTO #RESULT FROM MyPricelist {3}
                        SELECT COUNT(*) FROM #RESULT 
                        SELECT * FROM #RESULT WHERE RowNumber BETWEEN {1} AND {2} ORDER BY {4} 
                        ",
                            supplier.CarrierAccountID, From, To, WhereCondition, ColumnName);
            System.Data.SqlClient.SqlConnection connection = (System.Data.SqlClient.SqlConnection)TABS.BusinessEntities.RateBO.GetOpenedConnection();
            System.Data.IDataReader reader = TABS.BusinessEntities.RateBO.ExecuteReader(connection, sb.ToString());
            return reader;
        }
        //------------------------------------------------------------------------------------------------------------

        public static Dictionary<int, CodeChangeLog> GetCodeChangeLogs()
        {
            Dictionary<int, CodeChangeLog> results = new Dictionary<int, CodeChangeLog>();
            NHibernate.ICriteria criteria = CurrentSession
                .CreateCriteria(typeof(CodeChangeLog))
                       .AddOrder(new Order("Updated", false))
                        .AddOrder(new Order("ID", false));

            var list = criteria.List<CodeChangeLog>();
            return list.ToDictionary(c => c.ID);
        }

        public static IList<CodeChangeLog> GetCodeChangeLogsCustom(int PageIndex, int PageSize, out int RecordsCount)
        {
            NHibernate.ICriteria criteria = CurrentSession
                           .CreateCriteria(typeof(CodeChangeLog))
                                  .AddOrder(new Order("Updated", false));

            NHibernate.ICriteria CountCriteria = CurrentSession
                           .CreateCriteria(typeof(CodeChangeLog));

            RecordsCount = CountCriteria.SetProjection(NHibernate.Criterion.Projections.RowCount()).UniqueResult<int>();

            var results = criteria.SetFirstResult(PageSize * (PageIndex - 1)).SetMaxResults(PageSize).List<CodeChangeLog>();

            return results;
        }


        #endregion Pricelists

        /// <summary>
        /// Get the main currency of the system.
        /// </summary>
        /// <returns>The main currency object</returns>
        public static Currency GetMainCurrency()
        {
            return CurrentSession.CreateCriteria(typeof(Currency))
                    .Add(Expression.Eq("_IsMainCurrency", "Y"))
                    .UniqueResult<Currency>();
        }
        /// <summary>
        /// get all the pricing templates  
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, PricingTemplate> GetAllPricingTemplate()
        {
            IList<PricingTemplate> list = GetList<PricingTemplate>();
            Dictionary<int, PricingTemplate> result = new Dictionary<int, PricingTemplate>(list.Count);
            foreach (PricingTemplate item in list)
            {
                result.Add(item.PricingTemplateId, item);
            }
            return result;
        }

        public static Dictionary<int, PricingTemplatePlan> GetAllPricingTemplatePlans()
        {
            IList<PricingTemplatePlan> list = GetList<PricingTemplatePlan>();
            Dictionary<int, PricingTemplatePlan> result = new Dictionary<int, PricingTemplatePlan>(list.Count);
            foreach (PricingTemplatePlan item in list)
            {
                result.Add(item.PricingTemplatePlanId, item);
            }
            return result;
        }

        /// <summary>
        /// get the Pricing Template Plans for a specified Pricing Template
        public static IList<PricingTemplatePlan> GetPricingTemplatePlans(PricingTemplate pricingtemplate)
        {
            return CurrentSession.CreateCriteria(typeof(PricingTemplatePlan))
                .Add(Expression.Eq("PricingTemplate", pricingtemplate))
                .AddOrder(new Order("Priority", false))
                .List<PricingTemplatePlan>();
        }

        /// <summary>
        /// Get all CarrierGroups.
        /// </summary>
        /// <returns>A dictionnary  of Carrier Groups</returns>
        public static Dictionary<int, CarrierGroup> GetAllCarrierGroups()
        {
            Dictionary<int, CarrierGroup> result = new Dictionary<int, CarrierGroup>();
            IList<CarrierGroup> list = GetList<CarrierGroup>();
            foreach (CarrierGroup group in list)
                result.Add(group.CarrierGroupID, group);
            return result;
        }


        /// <summary>
        /// Get all currencies of the system.
        /// </summary>
        /// <returns>A Dictionary for the currencies with the currency symbol as the key</returns>
        public static Dictionary<string, Currency> GetAllCurrencies()
        {
            IList<Currency> currencies = GetList<Currency>();
            Dictionary<string, Currency> result = new Dictionary<string, Currency>(currencies.Count);
            foreach (Currency currency in currencies)
            {
                result.Add(currency.Symbol, currency);
                if (currency.IsMainCurrency) Currency.Main = currency;
            }
            return result;
        }

        /// <summary>
        /// Get all Additional Services of the system.
        /// </summary>
        /// <returns>A Dictionary for the additional services with the id as the key</returns>
        public static Dictionary<short, FlaggedService> GetAllFlaggedServices()
        {
            Dictionary<short, FlaggedService> result = new Dictionary<short, FlaggedService>();
            IList<FlaggedService> list = GetList<FlaggedService>();
            foreach (FlaggedService service in list)
                result.Add(service.FlaggedServiceID, service);
            return result;
        }

        /// <summary>
        /// Get all the lookup types of the system.
        /// </summary>
        /// <returns>A dictionary of lookup types with the lookup type name as the key</returns>
        public static Dictionary<string, Lookups.LookupType> GetAllLookupTypes()
        {
            IList<Lookups.LookupType> list = GetList<Lookups.LookupType>();
            Dictionary<string, Lookups.LookupType> result = new Dictionary<string, Lookups.LookupType>(list.Count);
            foreach (Lookups.LookupType item in list)
            {
                result.Add(item.Name, item);
            }
            return result;
        }

        /// <summary>
        /// Get all the Tickets in the system.
        /// </summary>
        /// <returns>a  dictionary of Fault Tickets key: FaultTicketID primary key</returns>
        public static Dictionary<int, FaultTicket> GetFaultTickets()
        {
            IList<FaultTicket> list = GetList<FaultTicket>();
            Dictionary<int, FaultTicket> result = new Dictionary<int, FaultTicket>(list.Count);
            foreach (FaultTicket item in list)
            {
                result.Add(item.FaultTicketID, item);
            }
            return result;
        }

        /// <summary>
        /// get ticket history (a list of ticket history for a specific ticket)
        /// </summary>
        /// <param name="ticket">the specified fault ticket </param>
        /// <returns></returns>
        public static IList<FaultTicketUpdate> GetFaultTicketUpdates(FaultTicket ticket)
        {
            return CurrentSession.CreateCriteria(typeof(FaultTicketUpdate))
                .Add(Expression.Eq("FaultTicket", ticket))
                .AddOrder(new Order("UpdateDate", true))
                .List<FaultTicketUpdate>();
        }

        /// <summary>
        /// Update all currencies
        /// </summary>
        internal static void UpdateAllCurrencies()
        {
            using (NHibernate.ITransaction transaction = CurrentSession.BeginTransaction())
            {
                foreach (Currency currency in Currency.All.Values)
                    CurrentSession.Update(currency);
                transaction.Commit();
            }
        }

        /// <summary>
        /// get the current and future effective codes of a specified zone at a specified date
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="when"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static IList<Code> GetCurrentandFutureEffectiveCodes(Zone zone, DateTime when, NHibernate.ISession session)
        {
            IList<TABS.Code> codes = session
                   .CreateQuery(@"FROM Code C 
                            WHERE 
                                C.Zone = :zone
                            AND ((C.BeginEffectiveDate <= :when AND (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate >= :when)) OR C.BeginEffectiveDate >= :when)
                            ORDER BY C.Value")
                    .SetParameter("when", when)
                    .SetParameter("zone", zone)
                    .List<TABS.Code>();

            return codes;
        }

        /// <summary>
        /// Return the effective codes for a given zone at a given time using the open session
        /// </summary>
        /// <param name="zone">The zone for which to get the codes</param>
        /// <param name="when">The time at which we are looking for effective codes</param>
        /// <param name="session">The open session to use</param>
        /// <returns>A List of codes that are effective in this zone</returns>
        public static IList<Code> GetEffectiveCodes(Zone zone, DateTime when, NHibernate.ISession session)
        {
            return session.CreateCriteria(typeof(Code))
                .Add(Expression.Eq("Zone", zone))
                .Add(Expression.Le("BeginEffectiveDate", when))
                .Add(Expression.Or(
                            Expression.Gt("EndEffectiveDate", when),
                            new NullExpression("EndEffectiveDate")
                        )
                    )
                .AddOrder(new Order("Value", true))
                .List<Code>();
        }

        public static IList<Code> GetCodes(Zone zone, NHibernate.ISession session)
        {
            return session.CreateCriteria(typeof(Code))
                .Add(Expression.Eq("Zone", zone))
                .AddOrder(new Order("Value", true))
                .AddOrder(new Order("BeginEffectiveDate", true))
                .List<Code>();
        }

        public static IList<Code> GetCodes(Zone zone)
        {
            return GetCodes(zone, CurrentSession);
        }



        /// <summary>
        /// Get the effective rate for a given zone...
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="when"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static IList<Rate> GetEffectiveRates(Zone zone, DateTime when, NHibernate.ISession session)
        {
            IList<Rate> result = session.CreateCriteria(typeof(Rate))
                .Add(Expression.Eq("Zone", zone))
                .Add(Expression.Le("BeginEffectiveDate", when))
                .Add(Expression.Or(
                            Expression.Gt("EndEffectiveDate", when),
                            new NullExpression("EndEffectiveDate")
                        )
                    )
                .List<Rate>();
            if (result == null)
                result = new List<Rate>(new Rate[] { Rate.None });
            return result;
        }

        /// <summary>
        /// Gets the effective rate for a zone (at a given time)
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="when"></param>
        /// <returns></returns>
        public static IList<Rate> GetEffectiveRates(Zone zone, DateTime when)
        {
            return GetEffectiveRates(zone, when, CurrentSession);
        }

        /// <summary>
        /// Return the effective codes for a given zone at a given time
        /// </summary>
        /// <param name="zone">The zone for which to get the codes</param>
        /// <param name="when">The time at which we are looking for effective codes</param>
        /// <returns>A List of codes that are effective in this zone</returns>
        public static IList<Code> GetEffectiveCodes(Zone zone, DateTime when)
        {
            IList<Code> codes = GetEffectiveCodes(zone, when, CurrentSession);
            if (codes == null) codes = new List<Code>();
            return codes;
        }

        /// <summary>
        /// Return the current and future effective codes for a given zone at a given time
        /// </summary>
        /// <param name="zone">The zone for which to get the codes</param>
        /// <param name="when">The time at which we are looking for effective codes</param>
        /// <returns>A List of codes that are effective in this zone</returns>
        public static IList<Code> GetCurrentandFutureEffectiveCodes(Zone zone, DateTime when)
        {
            IList<Code> codes = GetCurrentandFutureEffectiveCodes(zone, when, CurrentSession);
            if (codes == null) codes = new List<Code>();
            return codes;
        }

        /// <summary>
        /// Return the special requests for a given zone at a given time
        /// </summary>
        /// <param name="zone">The zone for which to get the special requests</param>
        /// <param name="when">The time in which we are looking</param>
        /// <returns>A List of special requests that are effective in this zone at the time given</returns>
        public static IList<SpecialRequest> GetSpecialRequests(Zone zone, DateTime when)
        {
            return CurrentSession.CreateCriteria(typeof(SpecialRequest))
                .Add(Expression.Eq("Zone", zone))
                .Add(Expression.Le("BeginEffectiveDate", when))
                .Add(Expression.Or(
                            Expression.Gt("EndEffectiveDate", when),
                            new NullExpression("EndEffectiveDate")
                        )
                    )
                .List<SpecialRequest>();
        }

        /// <summary>
        /// Return the rate of the  Specified customer or supplier and speciefied by Zone
        /// </summary>
        /// <param name="Customer"></param>
        /// <param name="Supplier"></param>
        /// <param name="zone"></param>
        /// <param name="when"></param>
        /// <returns></returns>
        public static IList<Rate> GetRateHistory(CarrierAccount Customer, CarrierAccount Supplier, Zone zone, DateTime when)
        {
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                IList<Rate> list =
                    session.CreateQuery(@"SELECT R FROM Rate R  
                                WHERE 
                                    R.PriceList.Customer = :Customer 
                                    AND R.PriceList.Supplier = :Supplier
                                    AND R.Zone=:Zone
                                    AND :when <= R.BeginEffectiveDate 
                                    ORDER BY R.BeginEffectiveDate DESC, R.ID DESC")
                        .SetParameter("Supplier", Supplier)
                        .SetParameter("Customer", Customer)
                        .SetParameter("Zone", zone)
                        .SetParameter("when", when)
                        .List<Rate>();
                return list;
            }
        }

        /// <summary>
        /// Return our Future-Effective Zone-Code (s) 
        /// </summary>
        public static List<DTO.DTO_ZoneCode> GetFutureEffectiveZonesAndCodes(TABS.CarrierAccount supplier)
        {
            IList<Code> zonesCodes = null;
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                string hql = "SELECT C FROM Code C WHERE C.Zone.Supplier = :supplier AND (C.Zone.BeginEffectiveDate > :when or C.BeginEffectiveDate > :when)";
                zonesCodes = session.CreateQuery(hql)
                                .SetParameter("when", DateTime.Now)
                                .SetParameter("supplier", supplier)
                                .List<Code>();
            }
            return DTO.DTO_ZoneCode.Get(zonesCodes);
        }

        ///<summary>
        ///return our Future -    End Effective Zone-Code (s) 
        /// </summary>
        public static List<DTO.DTO_ZoneCode> GetEndedLaterZonesAndCodes(TABS.CarrierAccount supplier)
        {
            IList<Code> zonesCodes = null;
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                string hql = "SELECT C FROM Code C WHERE C.Zone.Supplier = :supplier AND (C.Zone.EndEffectiveDate > :when or C.EndEffectiveDate > :when)";
                zonesCodes = session.CreateQuery(hql)
                             .SetParameter("when", DateTime.Now)
                             .SetParameter("supplier", supplier)
                             .List<Code>();
            }
            return DTO.DTO_ZoneCode.Get(zonesCodes);
        }

        /// <summary>
        /// Return our effective zones and future codes,
        /// the zones we supply.
        /// </summary>
        /// <param name="when">The time at which the zones were effective</param>
        /// <returns>A dictionary (key is zone id) of our own Zones</returns>
        public static Dictionary<int, Zone> GetCurrentAndFutureZones(DateTime when)
        {
            Dictionary<int, Zone> zones = new Dictionary<int, Zone>(2000);
            IList<Zone> list = TABS.DataConfiguration.CurrentSession
                        .CreateQuery(@"FROM Zone Z 
                                        WHERE Z.Supplier = :Supplier 
                                        AND ((Z.BeginEffectiveDate < :when AND (Z.EndEffectiveDate IS NULL OR Z.EndEffectiveDate > :when)) OR Z.BeginEffectiveDate > :when)")
                                   .SetParameter("when", when)
                                   .SetParameter("Supplier", TABS.CarrierAccount.SYSTEM)
                        .List<TABS.Zone>();

            return list.ToDictionary(z => z.ZoneID);
        }

        /// <summary>
        /// Return our effective zones defined for the system (Zones with no supplier defined),
        /// the zones we supply.
        /// </summary>
        /// <param name="when">The time at which the zones were effective</param>
        /// <returns>A dictionary (key is zone id) of our own Zones</returns>
        public static Dictionary<int, Zone> GetOwnZones(DateTime when)
        {
            Dictionary<int, Zone> zones = new Dictionary<int, Zone>(2000);
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                IList<Zone> zoneList =
                    session.CreateQuery(
                        @"SELECT Z FROM TABS.Zone Z 
                                WHERE 
                                    Z.Supplier = :Supplier
                                    AND :when >= Z.BeginEffectiveDate 
                                    AND (Z.EndEffectiveDate IS NULL OR :when < Z.EndEffectiveDate)                                                                    
                                ORDER BY Z.Name")
                        .SetParameter("Supplier", CarrierAccount.SYSTEM)
                        .SetParameter("when", when)
                        .List<Zone>();

                foreach (Zone zone in zoneList)
                {
                    zones[zone.ZoneID] = zone;
                    zone.EffectiveCodes = new List<Code>();
                }

                //AND (C.EndEffectiveDate IS NULL OR :when < C.EndEffectiveDate) 
                //              AND :when >= C.BeginEffectiveDate  

                IList<Code> codes = session.CreateQuery(
                    @"SELECT C FROM Code C                                     
                                WHERE 
                                    C.Zone.Supplier = :Supplier
                                   AND ((C.BeginEffectiveDate <= :when AND (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate > :when)) OR C.BeginEffectiveDate > :when)                                   
                                ORDER BY C.Value")
                        .SetParameter("Supplier", CarrierAccount.SYSTEM)
                        .SetParameter("when", when)
                        .List<Code>();

                foreach (Code code in codes)
                    if (code.Zone != null)
                    {
                        Zone zone = null;
                        if (zones.TryGetValue(code.Zone.ZoneID, out zone))
                            zone.EffectiveCodes.Add(code);
                    }
            }
            return zones;
        }


        public static Dictionary<int, Zone> GetFutureOwnZonesAndCodes(DateTime when)
        {
            Dictionary<int, Zone> zones = new Dictionary<int, Zone>(2000);
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                IList<Zone> zoneList =
                    session.CreateQuery(
                        @"SELECT Z FROM TABS.Zone Z 
                                WHERE 
                                    Z.Supplier = :Supplier
                                    AND :when < Z.BeginEffectiveDate 
                                    AND (Z.EndEffectiveDate IS NULL OR :when < Z.EndEffectiveDate)                                                                    
                                ORDER BY Z.Name")
                        .SetParameter("Supplier", CarrierAccount.SYSTEM)
                        .SetParameter("when", when)
                        .List<Zone>();

                foreach (Zone zone in zoneList)
                {
                    zones[zone.ZoneID] = zone;
                    zone.EffectiveCodes = new List<Code>();
                }

                //AND (C.EndEffectiveDate IS NULL OR :when < C.EndEffectiveDate) 
                //              AND :when >= C.BeginEffectiveDate  

                IList<Code> codes = session.CreateQuery(
                    @"SELECT C FROM Code C                                     
                                WHERE 
                                    C.Zone.Supplier = :Supplier
                                   AND ((C.BeginEffectiveDate > :when AND (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate > :when)))                                   
                                ORDER BY C.Value")
                        .SetParameter("Supplier", CarrierAccount.SYSTEM)
                        .SetParameter("when", when)
                        .List<Code>();

                foreach (Code code in codes)
                    if (code.Zone != null)
                    {
                        Zone zone = null;
                        if (zones.TryGetValue(code.Zone.ZoneID, out zone))
                            zone.EffectiveCodes.Add(code);
                    }
            }
            return zones;
        }


        /// <summary>
        /// Returns our own effective zones and codes from parameter when and on.
        /// </summary>
        /// <param name="when">The time at which the zones were effective</param>
        /// <returns>Dictionary<ZoneID,Own Zones></returns>
        public static Dictionary<int, Zone> GetOwnZonesIncludeEffCodes(DateTime when)
        {
            Dictionary<int, Zone> zones = new Dictionary<int, Zone>(2000);
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                IList<Zone> zoneList =
                    session.CreateQuery(
                        @"SELECT Z FROM TABS.Zone Z 
                                WHERE 
                                    Z.Supplier = :Supplier
                                    AND :when >= Z.BeginEffectiveDate 
                                    AND (Z.EndEffectiveDate IS NULL OR :when < Z.EndEffectiveDate)                                                                    
                                ORDER BY Z.Name")
                        .SetParameter("Supplier", CarrierAccount.SYSTEM)
                        .SetParameter("when", when)
                        .List<Zone>();

                foreach (Zone zone in zoneList)
                {
                    zones[zone.ZoneID] = zone;
                    zone.EffectiveCodes = new List<Code>();
                }

                //AND (C.EndEffectiveDate IS NULL OR :when < C.EndEffectiveDate) 
                //              AND :when >= C.BeginEffectiveDate  

                IList<Code> codes = session.CreateQuery(
                    @"SELECT C FROM Code C                                     
                                WHERE 
                                    C.Zone.Supplier = :Supplier
                                   AND ((C.BeginEffectiveDate < :when AND (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate > :when)) OR C.BeginEffectiveDate >= :when)                                   
                                ORDER BY C.Value")
                        .SetParameter("Supplier", CarrierAccount.SYSTEM)
                        .SetParameter("when", when)
                        .List<Code>();

                foreach (Code code in codes)
                    if (code.Zone != null)
                    {
                        Zone zone = null;
                        if (zones.TryGetValue(code.Zone.ZoneID, out zone))
                            zone.EffectiveCodes.Add(code);
                    }
            }
            return zones;
        }




        /// <summary>
        /// get all zones and codes for a specific carrier account 
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
        public static IList<TABS.Zone> GetAllZonesAndCodes(TABS.CarrierAccount carrier)
        {
            List<TABS.Zone> list = new List<TABS.Zone>();

            Dictionary<int, TABS.Zone> zones = new Dictionary<int, TABS.Zone>();

            IList<object[]> results = TABS.ObjectAssembler.CurrentSession.CreateQuery(
                        @"SELECT Z, R FROM Zone Z, Rate R
                            WHERE 
                                    R.Zone = Z
                                AND R.PriceList.Supplier = :supplier
                                ORDER BY Z.Name")
                   .SetParameter("supplier", carrier)
                   .List<object[]>();

            foreach (object[] tupple in results)
            {
                TABS.Zone zone = (TABS.Zone)tupple[0];
                zones[zone.ZoneID] = zone;
                TABS.Rate rate = (TABS.Rate)tupple[1];
                zone.EffectiveRates = new List<TABS.Rate>(new TABS.Rate[] { rate });
                list.Add(zone);
            }

            // Now get the zone code
            IList<TABS.Code> codes = TABS.ObjectAssembler.CurrentSession.CreateQuery(
                        @"SELECT C FROM Code C, Zone Z
                            WHERE C.Zone = Z
                                AND Z.Supplier = :supplier
                            ORDER BY C.Value")
                   .SetParameter("supplier", carrier)
                   .List<TABS.Code>();

            // Initialize zones
            foreach (TABS.Zone zone in zones.Values) zone.EffectiveCodes = new List<TABS.Code>();

            // Invalid Zones should be put as-non effective
            List<TABS.Zone> invalidZones = new List<TABS.Zone>();

            // Assign codes for zones
            foreach (TABS.Code code in codes)
            {
                TABS.Zone zone;
                if (!zones.TryGetValue(code.Zone.ZoneID, out zone))
                    invalidZones.Add(code.Zone);
                else
                    zone.EffectiveCodes.Add(code);
            }

            return list;
        }


        /// <summary>
        /// Return effective zones defined for the given carrier account
        /// </summary>
        /// <param name="when">The time at which the zones were effective</param>
        /// <returns>A list of supplier zones</returns>
        public static IList<Zone> GetZones(CarrierAccount supplier, DateTime whenEffective)
        {
            List<Zone> list = new List<Zone>();

            Dictionary<int, Zone> zones = new Dictionary<int, Zone>();

            IList<object[]> results = CurrentSession.CreateQuery(
                        @"SELECT Z, R FROM Zone Z, Rate R
                            WHERE 
                                    R.Zone = Z
                                AND R.PriceList.Supplier = :supplier
                                AND R.BeginEffectiveDate <= :whenEffective 
                                AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate >= :whenEffective)
                                ORDER BY Z.Name")
                   .SetParameter("supplier", supplier)
                   .SetParameter("whenEffective", whenEffective)
                   .List<object[]>();

            foreach (object[] tupple in results)
            {
                Zone zone = (Zone)tupple[0];
                zones[zone.ZoneID] = zone;
                Rate rate = (Rate)tupple[1];
                zone.EffectiveRates = new List<Rate>(new Rate[] { rate });
                list.Add(zone);
            }

            // Now get the zone code
            IList<Code> codes = CurrentSession.CreateQuery(
                        @"SELECT C FROM Code C, Zone Z
                            WHERE C.Zone = Z
                                AND Z.Supplier = :supplier
                                AND C.BeginEffectiveDate <= :whenEffective 
                                AND (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate >= :whenEffective) 
                            ORDER BY C.Value")
                   .SetParameter("supplier", supplier)
                   .SetParameter("whenEffective", whenEffective)
                   .List<Code>();

            // Initialize zones
            foreach (Zone zone in zones.Values) zone.EffectiveCodes = new List<Code>();

            // Invalid Zones should be put as-non effective
            List<Zone> invalidZones = new List<Zone>();

            // Assign codes for zones
            foreach (Code code in codes)
            {
                Zone zone;
                if (!zones.TryGetValue(code.Zone.ZoneID, out zone))
                    invalidZones.Add(code.Zone);
                else
                    zone.EffectiveCodes.Add(code);
            }

            return list;
        }

        public static Dictionary<int, Zone> GetZones()
        {
            Dictionary<int, Zone> zones = new Dictionary<int, Zone>(2000);
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                IList<Zone> zoneList =
                    session.CreateQuery(
                        @"SELECT Z FROM TABS.Zone Z 
                                WHERE 
                                    Z.Supplier = :Supplier
                                    AND (Z.EndEffectiveDate IS NOT NULL)                                                                    
                                ORDER BY Z.Name")
                        .SetParameter("Supplier", CarrierAccount.SYSTEM)
                        .List<Zone>();

                foreach (Zone zone in zoneList)
                {
                    zones[zone.ZoneID] = zone;
                    zone.EffectiveCodes = new List<Code>();
                }
            }
            return zones;
        }

        /// <summary>
        /// Return Effective Tariffs for a Selected Zone
        /// </summary>
        /// <param name="when">the time of effective Tariffs</param>
        /// <returns>list of effective Tariffs for the selected zone </returns>
        public static IList<Tariff> GetZoneTariffs(Zone zone, DateTime when)
        {
            if (zone.ZoneID == 0) return new List<Tariff>();

            IList<Tariff> list = CurrentSession.CreateCriteria(typeof(Tariff))
                .Add(Expression.Eq("Zone", zone))
                .Add(Expression.Le("BeginEffectiveDate", when))
                .Add(Expression.Or(
                            Expression.Ge("EndEffectiveDate", when),
                            new NullExpression("EndEffectiveDate")
                        )
                    )
                .AddOrder(new Order("BeginEffectiveDate", false))
                .List<Tariff>();

            return list;
        }
        /// <summary>
        /// Return Effective Tariffs 
        /// </summary>
        /// <param name="when">the time of effective Tariffs</param>
        /// <returns>list of Tariffs</returns>
        public static IList<Tariff> GetTariffs(CarrierAccount supplier, CarrierAccount customer, DateTime when)
        {
            NHibernate.ICriteria criteria = CurrentSession.CreateCriteria(typeof(Tariff))
                         .Add(Expression.Le("BeginEffectiveDate", when))
                        .Add(Expression.Or(
                         Expression.Ge("EndEffectiveDate", when),
                         new NullExpression("EndEffectiveDate")
                     )
                 );
            if (customer != null) criteria = criteria.Add(Expression.Eq("Customer", customer));
            if (supplier != null) criteria = criteria.Add(Expression.Eq("Supplier", supplier));
            criteria.AddOrder(new Order("BeginEffectiveDate", false));
            return criteria.List<Tariff>();
        }
        /// <summary>
        /// get the effective tod consideration fro a specific zne   
        /// </summary>
        /// <param name="when">The time at which the ToD Considerations were effective</param>
        /// <returns>A list of ToD Considerations</returns>
        public static IList<ToDConsideration> GetEffectiveToDConsiderations(Zone zone, DateTime when)
        {
            IList<ToDConsideration> list = CurrentSession.CreateCriteria(typeof(ToDConsideration))
                .Add(Expression.Eq("Zone", zone))
                .Add(Expression.Le("BeginEffectiveDate", when))
                .Add(Expression.Or(
                            Expression.Ge("EndEffectiveDate", when),
                            new NullExpression("EndEffectiveDate")
                        )
                    )
                .AddOrder(new Order("BeginEffectiveDate", false))
                .List<ToDConsideration>();

            return list;
        }
        /// <summary>
        /// Return effective ToD Considerations for the given carrier account (as supplier) 
        /// if supplier is null the method will return our own ToD considerations 
        /// they are rturned by effective date desc  
        /// </summary>
        /// <param name="when">The time at which the ToD Considerations were effective</param>
        /// <returns>A list of ToD Considerations</returns>
        public static IList<ToDConsideration> GetToDConsiderations(CarrierAccount supplier, CarrierAccount customer, DateTime when)
        {
            NHibernate.ICriteria criteria = CurrentSession.CreateCriteria(typeof(ToDConsideration))
                            .Add(Expression.Le("BeginEffectiveDate", when))
                           .Add(Expression.Or(
                            Expression.Ge("EndEffectiveDate", when),
                            new NullExpression("EndEffectiveDate")
                        )
                    )
                    .AddOrder(new Order("BeginEffectiveDate", false));


            if (customer != null) criteria = criteria.Add(Expression.Eq("Customer", customer));
            if (supplier != null) criteria = criteria.Add(Expression.Eq("Supplier", supplier));

            return criteria.List<ToDConsideration>();
        }

        public static IList<ToDConsideration> GetToDConsiderationsFrom(CarrierAccount supplier, CarrierAccount customer, DateTime from)
        {
            NHibernate.ICriteria criteria = CurrentSession.CreateCriteria(typeof(ToDConsideration))
                           .Add(Expression.Or(
                            Expression.Ge("EndEffectiveDate", from),
                            new NullExpression("EndEffectiveDate")
                        )
                    )
                    .AddOrder(new Order("BeginEffectiveDate", false));
            if (customer != null) criteria = criteria.Add(Expression.Eq("Customer", customer));
            if (supplier != null) criteria = criteria.Add(Expression.Eq("Supplier", supplier));
            return criteria.List<ToDConsideration>();
        }

        /// </summary>
        /// <param name="supplier"></param>
        /// <param name="when"></param>
        /// <returns></returns>
        public static IList<Commission> GetSupplierCommissions(CarrierAccount supplier, DateTime when)
        {
            return CurrentSession.CreateCriteria(typeof(Commission))
               .Add(Expression.Eq("Supplier", supplier))
               .Add(Expression.Le("BeginEffectiveDate", when))
               .Add(Expression.Or(
                           Expression.Ge("EndEffectiveDate", when),
                           new NullExpression("EndEffectiveDate")
                       )
                   )
               .AddOrder(new Order("BeginEffectiveDate", false))
               .List<Commission>();

        }

        public static IList<Commission> GetSupplierCommissionsFrom(CarrierAccount supplier, DateTime from)
        {
            return CurrentSession.CreateCriteria(typeof(Commission))
               .Add(Expression.Eq("Supplier", supplier))
               .Add(Expression.Or(
                           Expression.Ge("EndEffectiveDate", from),
                           new NullExpression("EndEffectiveDate")
                       )
                   )
               .AddOrder(new Order("BeginEffectiveDate", false))
               .List<Commission>();
        }

        /// </summary>
        /// <param name="Zone"></param>
        /// <param name="when"></param>
        /// <returns></returns>
        public static IList<Commission> GetEffectiveSalesCommisions(Zone zone, DateTime when)
        {

            IList<Commission> list = CurrentSession.CreateCriteria(typeof(Commission))
                .Add(Expression.Eq("Zone", zone))
                .Add(Expression.Le("BeginEffectiveDate", when))
                .Add(Expression.Or(
                            Expression.Ge("EndEffectiveDate", when),
                            new NullExpression("EndEffectiveDate")
                        )
                    )
                .AddOrder(new Order("BeginEffectiveDate", false))
                .List<Commission>();

            return list;
        }

        public static IList<Commission> GetSalesCommisions(DateTime when)
        {
            return CurrentSession.CreateCriteria(typeof(Commission))
                .Add(Expression.Eq("Supplier", CarrierAccount.SYSTEM))
                .Add(Expression.Le("BeginEffectiveDate", when))
                .Add(Expression.Or(
                            Expression.Ge("EndEffectiveDate", when),
                            new NullExpression("EndEffectiveDate")
                        )
                    )
                .AddOrder(new Order("BeginEffectiveDate", false))
                .List<Commission>();
        }

        /// <summary>
        /// Get all the PrepaidPostpaidOptions
        /// </summary>
        /// <returns>The list of PrepaidPostpaidOptions</returns>
        public static Dictionary<string, Dictionary<decimal, PrepaidPostpaidOptions>> GetPrepaidPostpaidOptions(bool IsCustomer, bool IsPrepaid)
        {
            Dictionary<string, Dictionary<decimal, PrepaidPostpaidOptions>> Options = new Dictionary<string, Dictionary<decimal, PrepaidPostpaidOptions>>();
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                IList<PrepaidPostpaidOptions> list = session.CreateCriteria(typeof(PrepaidPostpaidOptions))
                        .Add(Expression.Eq("IsCustomer", IsCustomer))
                        .Add(Expression.Eq("IsPrepaid", IsPrepaid))
                        .List<PrepaidPostpaidOptions>();
                foreach (PrepaidPostpaidOptions prepaidPostpaidOptions in list)
                {
                    if (!Options.ContainsKey(prepaidPostpaidOptions.CarrierID))
                        Options[prepaidPostpaidOptions.CarrierID] = new Dictionary<decimal, PrepaidPostpaidOptions>();
                    Options[prepaidPostpaidOptions.CarrierID].Add((IsPrepaid) ? prepaidPostpaidOptions.Amount : prepaidPostpaidOptions.Percentage, prepaidPostpaidOptions);
                    Options[prepaidPostpaidOptions.CarrierID].Values.OrderBy(p => (IsPrepaid) ? p.Amount : prepaidPostpaidOptions.Percentage);
                }
            }
            return Options;
        }

        public static PrepaidAmount GetPrepaidAmount(string CarrierType, object Carrier)
        {
            PrepaidAmount tmp = null;
            string hql = @"SELECT pa FROM PrepaidAmount pa WHERE 1 = 1";

            if (Carrier.GetType() == typeof(CarrierAccount))
            {
                if (CarrierType == "Customer") hql += " AND CustomerID = :ID";
                else hql += " AND SupplierID = :ID";
            }

            if (Carrier.GetType() == typeof(CarrierProfile))
            {
                if (CarrierType == "Customer") hql += " AND CustomerProfileID = :ID";
                else hql += " AND SupplierProfileID = :ID";
            }
            hql += " AND Type = :type";



            IList<PrepaidAmount> PrepaidAmounts = CurrentSession.CreateQuery(hql)
                .SetParameter("ID", (Carrier.GetType() == typeof(CarrierProfile)) ? (Carrier as CarrierProfile).ProfileID.ToString() : (Carrier as CarrierAccount).CarrierAccountID)
                .SetParameter("type", TABS.AmountType.Payment)
                .List<PrepaidAmount>();
            if (PrepaidAmounts.Count > 0) tmp = PrepaidAmounts[0];
            return tmp;
        }

        public static PostpaidAmount GetPostpaidAmount(string CarrierType, object Carrier)
        {
            PostpaidAmount tmp = null;
            string hql = @"SELECT pa FROM PostpaidAmount pa WHERE 1 = 1";

            if (Carrier.GetType() == typeof(CarrierAccount))
            {
                if (CarrierType == "Customer") hql += " AND CustomerID = :ID";
                else hql += " AND SupplierID = :ID";
            }

            if (Carrier.GetType() == typeof(CarrierProfile))
            {
                if (CarrierType == "Customer") hql += " AND CustomerProfileID = :ID";
                else hql += " AND SupplierProfileID = :ID";
            }
            hql += " AND Type = :type";

            IList<PostpaidAmount> PostpaidAmounts = CurrentSession.CreateQuery(hql)
                .SetParameter("ID", (Carrier.GetType() == typeof(CarrierProfile)) ? (Carrier as CarrierProfile).ProfileID.ToString() : (Carrier as CarrierAccount).CarrierAccountID)
                .SetParameter("type", TABS.AmountType.Payment)
                .List<PostpaidAmount>();
            if (PostpaidAmounts.Count > 0) tmp = PostpaidAmounts[0];
            return tmp;
        }

        /// <summary>
        /// Get all the carrier profiles
        /// </summary>
        /// <returns>The list of carrier profiles</returns>
        public static Dictionary<int, CarrierProfile> GetCarrierProfiles()
        {
            Dictionary<int, CarrierProfile> profiles = new Dictionary<int, CarrierProfile>();
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                IList<CarrierProfile> list = session.CreateCriteria(typeof(CarrierProfile))
                        .Add(Expression.Eq("_IsDeleted", "N"))
                        .List<CarrierProfile>();
                foreach (CarrierProfile profile in list)
                    profiles[profile.ProfileID] = profile;
            }
            return profiles;
        }

        /// <summary>
        /// Get all the system switches
        /// </summary>
        /// <returns>A dictionary of switches (key is switch id)</returns>
        public static Dictionary<int, Switch> GetSwitches()
        {
            Dictionary<int, Switch> switches = new Dictionary<int, Switch>();
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                IList<Switch> list = session.CreateCriteria(typeof(Switch))
                        .AddOrder(new Order("Name", true))
                        .List<Switch>();
                foreach (Switch sw in list)
                    switches[sw.SwitchID] = sw;
            }
            return switches;
        }


        /// <summary>
        /// Get All Route overrides from database 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<CarrierAccount, List<RouteOverride>> GetRouteOverrides()
        {
            Dictionary<CarrierAccount, List<RouteOverride>> overrides = new Dictionary<CarrierAccount, List<RouteOverride>>();
            lock (typeof(RouteOverride))
            {
                using (NHibernate.ISession session = DataConfiguration.OpenSession())
                {
                    DateTime start = DateTime.Now;

                    // var routeOverrides = session.CreateCriteria(typeof(RouteOverride)).List<RouteOverride>();
                    var routeOverrides = DataHelper.LoadRouteOverrides();
                    TimeSpan spent = DateTime.Now.Subtract(start);

                    foreach (var routeOverride in routeOverrides)
                    {
                        // remove inactive/blocked carriers
                        var customer = routeOverride.Customer;
                        var isBlocked = (
                            !customer.isActive
                            ||
                            customer.RoutingStatus == RoutingStatus.Blocked
                            ||
                            customer.RoutingStatus == RoutingStatus.BlockedInbound
                            )
                             ;
                        //if (isBlocked) continue;

                        routeOverride.IsCustomerBlockedInactive = isBlocked;
                        if (!overrides.ContainsKey(routeOverride.Customer))
                            overrides[routeOverride.Customer] = new List<RouteOverride>();

                        overrides[routeOverride.Customer].Add(routeOverride);
                    }
                }
            }
            return overrides;
        }
        /// <summary>
        /// Clean override from inactive/blocked suppliers
        /// </summary>
        /// <param name="routeOverride"></param>
        /// <returns></returns>
        public static TABS.RouteOverride GetActiveOveeride(TABS.RouteOverride routeOverride)
        {
            TABS.RouteOverride result = new RouteOverride();


            return result;
        }

        /// <summary>
        /// Get all the Carrier Accounts
        /// </summary>
        /// <returns>A dictionary of carrier accounts based on the the carrier account ID</returns>
        public static Dictionary<string, CarrierAccount> GetCarrierAccounts()
        {
            // Make sure flagged services are loaded
            int serviceCount = FlaggedService.All.Count;

            Dictionary<string, CarrierAccount> accounts = new Dictionary<string, CarrierAccount>();
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                IList<CarrierAccount> list = session.CreateCriteria(typeof(CarrierAccount))
                        .Add(Expression.Eq("_IsDeleted", "N"))
                        .CreateCriteria("CarrierProfile")
                        .AddOrder(new Order("Name", true))
                        .List<CarrierAccount>();
                foreach (CarrierAccount account in list)
                {
                    account.CarrierProfile = CarrierProfile.All[account.CarrierProfile.ProfileID];
                    accounts[account.CarrierAccountID] = account;
                }
            }
            return accounts;
        }

        public static Dictionary<int, AutoInvoiceSetting> GetAutoInvoiceSetting()
        {

            Dictionary<int, AutoInvoiceSetting> lst = new Dictionary<int, AutoInvoiceSetting>();
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                IList<AutoInvoiceSetting> list = session.CreateCriteria(typeof(AutoInvoiceSetting))
                        .List<AutoInvoiceSetting>();

                foreach (var setting in list)
                {
                    lst[setting.SettingID] = setting;
                }
                return lst;
            }
        }

        public static List<AutoInvoiceLogger> GetAutoInvoiceLogger()
        {

            List<AutoInvoiceLogger> lst = new List<AutoInvoiceLogger>();
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                IList<AutoInvoiceLogger> list = session.CreateCriteria(typeof(AutoInvoiceLogger))
                        .List<AutoInvoiceLogger>();

                foreach (var item in list)
                {
                    lst.Add(item);
                }
                return lst;
            }
        }

        /// <summary>
        /// Create the System Carrier Account (IT SHOULD NOT BE CALLED UNLESS ACCOUNT DOES NOT EXIST!!!!)
        /// </summary>
        /// <returns></returns>
        internal static CarrierAccount CreateSystemCarrierAccount()
        {
            CarrierProfile profile = new CarrierProfile();
            profile.Name = "SYSTEM";
            profile.User = null;
            CarrierAccount account = new CarrierAccount();
            account.User = null;
            account.CarrierProfile = profile;
            account.CarrierAccountID = CarrierAccount.SystemAccountID;
            account.CarrierProfile.Currency = TABS.Currency.Main;
            account.ActivationStatus = ActivationStatus.Active;
            account.AccountType = AccountType.Exchange;
            account.IsTOD = true;
            account.RoutingStatus = RoutingStatus.Enabled;
            Exception ex;
            Save(profile, out ex);
            Save(account, out ex);
            if (ex != null) throw ex;
            return account;
        }

        /// <summary>
        /// Create the Blocked Carrier Account (IT SHOULD NOT BE CALLED UNLESS ACCOUNT DOES NOT EXIST!!!!)
        /// </summary>
        /// <returns></returns>
        internal static CarrierAccount CreateBlockedCarrierAccount()
        {
            CarrierProfile profile = new CarrierProfile();
            profile.Name = "Blocked Account";
            profile.CompanyName = "Blocked Account";
            profile.User = null;
            CarrierAccount account = new CarrierAccount();
            account.User = null;
            account.CarrierProfile = profile;
            account.CarrierAccountID = CarrierAccount.BlockedAccountID;
            account.ActivationStatus = ActivationStatus.Active;
            account.AccountType = AccountType.Termination;
            account.CarrierProfile.Currency = TABS.Currency.Main;
            account.IsTOD = true;
            account.RoutingStatus = RoutingStatus.Enabled;
            Exception ex;
            Save(profile, out ex);
            Save(account, out ex);
            if (ex != null) throw ex;
            return account;
        }

        /// <summary>
        /// Create Default Time Zone Info
        /// </summary>
        /// <returns></returns>
        internal static CustomTimeZoneInfo CreateCustomTimeZoneInfo()
        {
            CustomTimeZoneInfo info = new CustomTimeZoneInfo();
            info.BaseUtcOffset = 0;
            info.DisplayName = "Default System Time";
            Exception ex;
            Save(info, out ex);
            if (ex != null) throw ex;
            return info;
        }

        /// <summary>
        /// Get all the Code Groups
        /// </summary>
        /// <returns>A dictionary of code groups based on the code as a key</returns>
        public static Dictionary<string, CodeGroup> GetCodeGroups()
        {
            IList<CodeGroup> list = GetList<CodeGroup>();
            Dictionary<string, CodeGroup> codeGroups = new Dictionary<string, CodeGroup>(list.Count);
            foreach (CodeGroup codeGroup in list)
                codeGroups[codeGroup.Code] = codeGroup;
            return codeGroups;
        }

        /// <summary>
        /// Get the carrier accounts owned by a carrier profile
        /// </summary>
        /// <param name="profile">The profile which the accounts are bound to</param>
        /// <returns>The list of carrier accounts</returns>
        public static IList<CarrierAccount> GetCarrierAccounts(CarrierProfile profile)
        {
            List<CarrierAccount> accounts = new List<CarrierAccount>();
            foreach (CarrierAccount account in CarrierAccount.All.Values)
                if (account.CarrierProfile.Equals(profile))
                    accounts.Add(account);
            return accounts;
        }

        /// <summary>
        /// Get the documents associated to a carrier profile
        /// </summary>
        /// <param name="profile">The profile owning the documents</param>
        /// <returns>The list of carrier documents</returns>
        public static IList<CarrierDocument> GetCarrierDocuments(CarrierProfile profile)
        {
            return CurrentSession.CreateCriteria(typeof(CarrierDocument))
                    .Add(Expression.Eq("CarrierProfile", profile))
                    .List<CarrierDocument>();
        }


        /// <summary>
        /// Get all the special requests associated to route change header
        /// </summary>
        /// <param name="header"></param>
        /// <param name="when"></param>
        /// <returns>The list of special requests</returns>
        public static IList<SpecialRequest> GetSpecialRequests(RouteChangeHeader header)
        {
            return CurrentSession.CreateCriteria(typeof(SpecialRequest))
                    .Add(Expression.Eq("RouteChangeHeader", header))
                    .AddOrder(new Order("BeginEffectiveDate", false))
                    .List<SpecialRequest>();
        }


        /// <summary>
        /// Get all the route blocks associated to route change header
        /// </summary>
        /// <param name="header"></param>
        /// <param name="when"></param>
        /// <returns>The list of route blocks</returns>
        public static IList<RouteBlock> GetRouteBlocks(RouteChangeHeader header)
        {
            return CurrentSession.CreateCriteria(typeof(RouteBlock))
                    .Add(Expression.Eq("RouteChangeHeader", header))
                    .AddOrder(new Order("BeginEffectiveDate", false))
                    .List<RouteBlock>();
        }

        #region Billing

        public static Billing_Invoice GetBillingInvoice(int invoiceID)
        {
            Billing_Invoice result = CurrentSession.CreateCriteria(typeof(Billing_Invoice))
                .Add(Expression.Eq("InvoiceID", invoiceID))
                .SetMaxResults(1)
                .UniqueResult<Billing_Invoice>();
            if (result == null) result = null;
            return result;

        }

        public static IList<Billing_Invoice> GetBillingInvoices(CarrierAccount customer, CarrierAccount supplier, DateTime from, DateTime to)
        {
            return GetBillingInvoices(CurrentSession, customer, supplier, from, to);
        }
        /// <summary>
        /// get a list of billing invoices for a specific customer between two dates   
        /// </summary>
        /// <param name="customer">the selected customer</param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <returns>a list of invoices </returns>
        public static IList<Billing_Invoice> GetBillingInvoices(NHibernate.ISession session, CarrierAccount customer, CarrierAccount supplier, DateTime from, DateTime to)
        {
            StringBuilder hql = new StringBuilder(@"SELECT B FROM Billing_Invoice B WHERE 1=1 ");

            if (supplier != null) hql.Append(" AND B.Supplier = :supplier ");
            if (customer != null) hql.Append(" AND B.Customer = :customer ");

            hql.Append(@" AND (B.BeginDate BETWEEN :from AND :to
                                     OR B.EndDate  BETWEEN :from AND :to ) 
                                ORDER BY B.IssueDate DESC");
            IList<Billing_Invoice> results = null;
            //using (session)
            //{in case of Open Session Must be Disposed 
            NHibernate.IQuery query = session.CreateQuery(hql.ToString());

            if (supplier != null) query.SetParameter("supplier", supplier);
            if (customer != null) query.SetParameter("customer", customer);

            results = query
                  .SetParameter("from", from)
                  .SetParameter("to", to)
                  .List<Billing_Invoice>();
            // }
            return results;
        }
        //-------------------------------------------------------------------For Custom Paging-----------------------------------------------------
        public static IList<Billing_Invoice> GetBillingInvoices(CarrierAccount customer, CarrierAccount supplier, DateTime from, DateTime to, int pageIndex, int pageSize, out int recordCount)
        {
            return GetBillingInvoices(CurrentSession, customer, supplier, from, to, pageIndex, pageSize, out recordCount);
        }
        public static IList<Billing_Invoice> GetBillingInvoices(NHibernate.ISession session, CarrierAccount customer, CarrierAccount supplier, DateTime from, DateTime to, int PageIndex, int PageSize, out int RecordCount)
        {
            StringBuilder hql = new StringBuilder(@"SELECT B FROM Billing_Invoice B WHERE 1=1 ");

            if (supplier != null) hql.Append(" AND B.Supplier = :supplier ");
            if (customer != null) hql.Append(" AND B.Customer = :customer ");

            hql.Append(@" AND (B.BeginDate BETWEEN :from AND :to
                                     OR B.EndDate  BETWEEN :from AND :to ) 
                                ORDER BY B.IssueDate DESC");
            IList<Billing_Invoice> results = null;
            //using (session)
            //{in case of Open Session Must be Disposed 
            NHibernate.IQuery query = session.CreateQuery(hql.ToString());

            if (supplier != null) query.SetParameter("supplier", supplier);
            if (customer != null) query.SetParameter("customer", customer);

            // }
            RecordCount = query
                  .SetParameter("from", from)
                  .SetParameter("to", to)
                  .List<Billing_Invoice>().Count;

            results = query
                  .SetParameter("from", from)
                  .SetParameter("to", to).SetFirstResult(PageSize * (PageIndex - 1)).SetMaxResults(PageSize).List<Billing_Invoice>();
            return results;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// get the out carrier details of a specific billing invoice 
        /// </summary>
        /// <param name="BillingInvoice"></param>
        /// <returns>a list of out carrier details for the selected billing invoice  </returns>
        public static IList<Billing_Invoice_Cost> GetBillingInvoiceCosts(Billing_Invoice BillingInvoice)
        {
            IList<Billing_Invoice_Cost> list = CurrentSession.CreateCriteria(typeof(Billing_Invoice_Cost))
                .Add(Expression.Eq("Billing_Invoice", BillingInvoice))
                .AddOrder(new Order("Supplier", true))
                .AddOrder(new Order("FromDate", true))
                .List<Billing_Invoice_Cost>();

            return list;
        }
        /// <summary>
        /// get the details of a specific billing invoice 
        /// </summary>
        /// <param name="BillingInvoice"></param>
        /// <returns>a list of details for the selected billing invoice  </returns>
        public static IList<Billing_Invoice_Detail> GetBillingInvoiceDetails(Billing_Invoice BillingInvoice)
        {
            IList<Billing_Invoice_Detail> list = CurrentSession.CreateCriteria(typeof(Billing_Invoice_Detail))
                .Add(Expression.Eq("Billing_Invoice", BillingInvoice))
                .AddOrder(new Order("Destination", true))
                .AddOrder(new Order("FromDate", true))
                .List<Billing_Invoice_Detail>();

            return list;
        }
        #endregion

    }
}