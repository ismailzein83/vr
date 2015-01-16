using System;

namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Currency Exchange Rates", "Get the Currency Exchange Rates automaticly.")]
    public class CurrencyExchangeUpdater : RunnableBase
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("TABS.Addons.Runnables.CurrencyExchangeUpdater");
        public override void Run()
        {
            log.Info("Currency Exchange Updater requested to Run");
            log.Info("Currency Exchange Updater is currently unavailable");


            System.Data.DataTable DATA = new System.Data.DataTable();
            try
            {
                DATA = Utilities.CurrencyExchangeRateGetter.CurrencyExchangeUpdate.UpdateCurrencyExchangeRates();
                if (DATA != null)
                    log.Info("Currency Exchange Updater finished");
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Currency Exchange Updater failed to Run : {0}", ex.ToString());
            }

        }

        public override string Status { get { return string.Empty; } }
    }
}
