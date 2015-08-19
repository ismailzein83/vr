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

            List<CarrierSummaryStats> lstCarrierSummaryStats = _datamanager.GetCarrierSummaryStats(input).Data.ToList<CarrierSummaryStats>();
            List<CarrierAccount> activeCustomers = new List<CarrierAccount>();
            List<CarrierAccount> activeSuppliers = new List<CarrierAccount>();
            //-----------------------------------------------------------------------------------
            List<CarrierProfile> activeProfile = new List<CarrierProfile>();
            //-----------------------------------------------------------------------------------
            decimal TotalDurations = 0;
            int Attempts = 0;
            decimal Cost = 0;
            decimal Sale = 0;
            decimal Profit = 0;
            decimal ProfitPercentage = 0;

            if (input.Query.GroupByProfile == "False")
            {
                foreach (CarrierSummaryStats row in lstCarrierSummaryStats)
                {

                    if (input.Query.CarrierType == "Customer")
                        if (row.GroupID != null && row.GroupID.ToString() != "")
                        {
                            //row["CarrierID"] = row["CustomerID"];

                            //TABS.CarrierAccount account = null;
                            //if (TABS.CarrierAccount.All.ContainsKey("" + row["CustomerID"]))
                            //    account = TABS.CarrierAccount.All["" + row["CustomerID"]];
                            //else
                            //    account = TABS.ObjectAssembler.Get<TABS.CarrierAccount>("" + row["CustomerID"]);

                            //row["Carrier"] = account.NameAsCustomer;

                            CarrierAccount account = _cmanager.GetCarrierAccount(row.GroupID);
                            activeCustomers.Add(account);
                        }

                    if (input.Query.CarrierType == "Supplier")
                        if (row.GroupID != null && row.GroupID.ToString() != "")
                        {
                            //row["CarrierID"] = row["SupplierID"];

                            //TABS.CarrierAccount account = null;
                            //if (TABS.CarrierAccount.All.ContainsKey("" + row["SupplierID"]))
                            //    account = TABS.CarrierAccount.All["" + row["SupplierID"]];
                            //else
                            //    account = TABS.ObjectAssembler.Get<TABS.CarrierAccount>("" + row["SupplierID"]);

                            //row["Carrier"] = account.NameAsSupplier;
                            CarrierAccount account = _cmanager.GetCarrierAccount(row.GroupID);
                            activeSuppliers.Add(account);
                        }

                    TotalDurations += (decimal)row.DurationsInMinutes;
                    Attempts += (int)row.Attempts;
                    Cost += Convert.ToDecimal(row.Cost_Nets);
                    Sale += Convert.ToDecimal(row.Sale_Nets);
                    Profit += Convert.ToDecimal(row.Profit);
                    decimal costAmmount = Convert.ToDecimal(row.Cost_Nets);
                    decimal saleAmmount = Convert.ToDecimal(row.Sale_Nets);
                    decimal duration;

                    try
                    {
                        //ProfitPercentage = saleAmmount == 0 ? 0 : ((saleAmmount - costAmmount) / saleAmmount) * 100;
                        //row.ProfitePercentage = Math.Round(ProfitPercentage, 2);// String.Format("{0:P}", ProfitPercentage);
                    }
                    catch { }
                }

                //if (ChkShowInactive.Checked)
                //{
                //    if (CarrierType.SelectedValue == "Customer")
                //    {
                //        foreach (var account in TABS.CarrierAccount.Customers.Except(activeCustomers))
                //        {
                //            DataRow row = data.NewRow();
                //            row["CarrierID"] = account.CarrierAccountID;
                //            row["Carrier"] = account.NameAsCustomer;
                //            data.Rows.Add(row);
                //        }
                //    }

                //    if (CarrierType.SelectedValue == "Supplier")
                //    {
                //        foreach (var account in TABS.CarrierAccount.Suppliers.Except(activeSuppliers))
                //        {
                //            DataRow row = data.NewRow();
                //            row["CarrierID"] = account.CarrierAccountID;
                //            row["Carrier"] = account.NameAsSupplier;
                //            data.Rows.Add(row);
                //        }
                //    }

                //}
                //rgCarrierSummary.Columns.FindByUniqueName("clmProfile").Visible = false;
                //rgCarrierSummary.Columns.FindByUniqueName("clmCustomer").Visible = true;
                //rgCarrierSummary.Columns.FindByUniqueName("clmZoneDetails").Visible = true;
                //rgCarrierSummary.Columns.FindByUniqueName("clmCarrierDetails").Visible = true;
            }




            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _datamanager.GetCarrierSummaryStats(input));
        }
    }
}
