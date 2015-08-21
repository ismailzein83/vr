using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;
using TOne.BusinessEntity.Entities;
using TOne.BusinessEntity.Business;

namespace TOne.Analytics.Business
{
    public class CarrierSummaryStatsManager
    {
        private readonly ICarrierSummaryStatsDataManager _datamanager;
        private readonly CarrierAccountManager _cmanager;
        private readonly CarrierProfileManager _cpmanager;

        public CarrierSummaryStatsManager()
        {
            _datamanager = AnalyticsDataManagerFactory.GetDataManager<ICarrierSummaryStatsDataManager>();
            _cmanager = new CarrierAccountManager();
        }

        public Vanrise.Entities.IDataRetrievalResult<CarrierSummaryStats> GetFilteredCarrierSummaryStats(Vanrise.Entities.DataRetrievalInput<CarrierSummaryStatsQuery> input)
        {
            input.Query.ToDate = input.Query.ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            input.Query.SupplierAmuID = null;
            input.Query.CustomerAmuID = null;
            
            if (input.Query.CarrierType == "True")
                input.Query.CarrierType = "Customer";
            else
                input.Query.CarrierType = "Supplier";
            
            if (input.Query.GroupByProfile == "True")
                input.Query.GroupByProfile = "Y";
            else
                input.Query.GroupByProfile = "N";

            Vanrise.Entities.BigResult<Entities.CarrierSummaryStats> lstCarrierSummaryStats = _datamanager.GetCarrierSummaryStats(input);
            List<CarrierAccount> activeCustomers = new List<CarrierAccount>();
            List<CarrierAccount> activeSuppliers = new List<CarrierAccount>();
            List<CarrierAccount> lstCarriers = _cmanager.GetAllCarrierAccounts().Values.ToList<CarrierAccount>();
            //-----------------------------------------------------------------------------------
            List<CarrierProfile> activeProfile = new List<CarrierProfile>();
            //-----------------------------------------------------------------------------------
            decimal TotalDurations = 0;
            int Attempts = 0;
            decimal Cost = 0;
            decimal Sale = 0;
            decimal Profit = 0;
            decimal ProfitPercentage = 0;
            List<CarrierSummaryStats> lstCarrierSummarySta = lstCarrierSummaryStats.Data.ToList<CarrierSummaryStats>();
            

            if (input.Query.GroupByProfile == "N")
            {
                foreach (CarrierSummaryStats row in lstCarrierSummarySta)
                {
                    if (input.Query.CarrierType == "Customer")
                        if (row.GroupID != null && row.GroupID.ToString() != "")
                        {
                            CarrierAccount account = _cmanager.GetCarrierAccount(row.GroupID);
                            activeCustomers.Add(account);
                            row.GroupName = account.ProfileName;
                        }

                    if (input.Query.CarrierType == "Supplier")
                        if (row.GroupID != null && row.GroupID.ToString() != "")
                        {
                            CarrierAccount account = _cmanager.GetCarrierAccount(row.GroupID);
                            activeSuppliers.Add(account);
                            row.GroupName = account.ProfileName;
                        }

                    TotalDurations += (decimal)row.DurationsInMinutes;
                    Attempts += (int)row.Attempts;
                    Cost += Convert.ToDecimal(row.Cost_Nets);
                    Sale += Convert.ToDecimal(row.Sale_Nets);
                    Profit += Convert.ToDecimal(row.Profit);
                    decimal costAmmount = Convert.ToDecimal(row.Cost_Nets);
                    decimal saleAmmount = Convert.ToDecimal(row.Sale_Nets);

                    try
                    {
                        ProfitPercentage = saleAmmount == 0 ? 0 : ((saleAmmount - costAmmount) / saleAmmount) * 100;
                        row.ProfitPercentage = Math.Round(ProfitPercentage, 2);
                    }
                    catch { }
                }

                if (input.Query.ShowInactive == "True")
                {
                    if (input.Query.CarrierType == "Customer")
                    {
                        foreach (var account in _cmanager.GetAllCarriers(CarrierType.Customer).Except(activeCustomers))
                        {
                            CarrierSummaryStats row = new CarrierSummaryStats();
                            row.GroupName = account.ProfileName;
                            lstCarrierSummarySta.Add(row);
                        }
                    }

                    if (input.Query.CarrierType == "Supplier")
                    {
                        foreach (var account in _cmanager.GetAllCarriers(CarrierType.Supplier).Except(activeSuppliers))
                        {
                            CarrierSummaryStats row = new CarrierSummaryStats();
                            row.GroupName = account.ProfileName;
                            lstCarrierSummarySta.Add(row);
                        }
                    }
                }
            }

            if (input.Query.GroupByProfile == "Y")
            {
                List<int> NEwProfiles = new List<int>();
                foreach (CarrierSummaryStats row in lstCarrierSummarySta)
                {
                    if (row.GroupID != null && row.GroupID.ToString() != "")
                    {
                        int profileId = 0;
                        int.TryParse(row.GroupID, out profileId);
                        CarrierProfile profile = _cpmanager.GetCarrierProfile(profileId);
                        row.GroupName = profile.Name;
                        NEwProfiles.Add(profileId);
                    }
                    
                    TotalDurations += (decimal)row.DurationsInMinutes;
                    Attempts += (int)row.Attempts;
                    Cost += Convert.ToDecimal(row.Cost_Nets);
                    Sale += Convert.ToDecimal(row.Sale_Nets);
                    Profit += Convert.ToDecimal(row.Profit);
                    decimal costAmmount = Convert.ToDecimal(row.Cost_Nets);
                    decimal saleAmmount = Convert.ToDecimal(row.Sale_Nets);
                    
                    try
                    {
                        ProfitPercentage = (saleAmmount - costAmmount) / saleAmmount;
                        row.ProfitPercentage = Math.Round(ProfitPercentage, 2);
                    }
                    catch { }
                }

                if (input.Query.ShowInactive == "True")
                {
                    foreach (CarrierAccount account in _cmanager.GetAllCarriers(CarrierType.Customer).Except(activeCustomers))
                    {
                        if (!NEwProfiles.Contains(account.ProfileId))
                        {
                            CarrierSummaryStats NewRow = new CarrierSummaryStats();
                            NewRow.GroupName = account.ProfileName;
                            lstCarrierSummarySta.Add(NewRow);
                            NEwProfiles.Add(account.ProfileId);
                        }
                    }
                }
            }
            lstCarrierSummaryStats.Data = lstCarrierSummarySta;
           return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input,lstCarrierSummaryStats );
        }
    }
}
