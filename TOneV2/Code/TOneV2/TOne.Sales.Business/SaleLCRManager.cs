using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using TOne.Sales.Data;
using TOne.Sales.Entities;

namespace TOne.Sales.Business
{
    public class SaleLCRManager
    {
        public bool SavePriceList(string customerId, List<SaleLcrRate> saleLcrRates, string currencyId, bool sendEmail)
        {
            RateManager rateManager = new RateManager();
            PriceListManager priceListManager = new PriceListManager();

            PriceList priceList = new PriceList()
            {
                BeginEffectiveDate = DateTime.Now,
                CurrencyId = currencyId,
                CustomerId = customerId,
                Description = "Created Price List From Mobile.",
                SupplierId = "SYS",
                EndEffectiveDate = null
            };
            int priceListId = 0;
            priceListManager.SavePriceList(priceList, out priceListId);
            if (priceListId > 0)
            {
                rateManager.UpdateRateEED(MapEndedRates(saleLcrRates), customerId);
                rateManager.SaveRates(MapNewRates(saleLcrRates, priceListId, currencyId, customerId));
                IStateBackupDataManager stateBackupManager = SalesDataManagerFactory.GetDataManager<IStateBackupDataManager>();
                StateBackup stateBackup = stateBackupManager.Create(StateBackupType.Customer, customerId);
                int stateBackupId = 0;
                stateBackupManager.Save(stateBackup, out stateBackupId);
            }
            return true;
        }

        private List<Rate> MapNewRates(List<SaleLcrRate> saleLcrRates, int priceListId, string currencyId, string customerId)
        {
            List<Rate> rates = new List<Rate>();
            foreach (SaleLcrRate lcrRate in saleLcrRates)
            {
                Rate rate = new Rate()
                {
                    PriceListId = priceListId,
                    BeginEffectiveDate = lcrRate.OldRate <= lcrRate.NewRate ? DateTime.Now : DateTime.Now.AddDays(7),
                    EndEffectiveDate = null,
                    CurrencyID = customerId,
                    CustomerId = customerId,
                    SupplierId = "SYS",
                    NormalRate = lcrRate.NewRate,
                    ServicesFlag = lcrRate.ServiceFlag,
                    Change = lcrRate.OldRate <= lcrRate.NewRate ? BusinessEntity.Entities.Change.Decrease : Change.Increase,
                    ZoneId = lcrRate.ZoneId,
                    OffPeakRate = 0,
                    WeekendRate = 0
                };
                rates.Add(rate);
            }
            return rates;
        }

        List<Rate> MapEndedRates(List<SaleLcrRate> saleLcrRates)
        {
            List<Rate> rates = new List<Rate>();
            foreach (SaleLcrRate lcrRate in saleLcrRates)
            {
                Rate rate = new Rate()
                {
                    PriceListId = lcrRate.PriceListId,
                    EndEffectiveDate = lcrRate.OldRate <= lcrRate.NewRate ? DateTime.Now : DateTime.Now.AddDays(7),
                    ZoneId = lcrRate.ZoneId
                };
                rates.Add(rate);
            }
            return rates;
        }
    }
}
