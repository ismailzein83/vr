using NP.IVSwitch.Data;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;


namespace NP.IVSwitch.Business
{
    public class TariffManager
    {
        #region Public Methods
        public Tariff GetTariff(int tariffId)
        {
            Dictionary<int, Tariff> cachedTariff = this.GetCachedTariff();
            return cachedTariff.GetRecord(tariffId);
        }

        public IEnumerable<TariffInfo> GetTariffsInfo(TariffFilter filter)
        {
            Func<Tariff, bool> filterExpression = null;

            return this.GetCachedTariff().MapRecords(TariffInfoMapper, filterExpression).OrderBy(x => x.TariffName);
        }     
 
         

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ITariffDataManager _dataManager = IVSwitchDataManagerFactory.GetDataManager<ITariffDataManager>();
            protected override bool IsTimeExpirable { get { return true; } }

        }
        #endregion

        #region Private Methods

        Dictionary<int, Tariff> GetCachedTariff()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetTariff",
                () =>
                {
                    ITariffDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<ITariffDataManager>();
                    return dataManager.GetTariffs().ToDictionary(x => x.TariffId, x => x);
                });
        }

        #endregion

        #region Mappers

        public TariffInfo TariffInfoMapper(Tariff tariff)
        {
            TariffInfo tariffInfo = new TariffInfo()
            {
                TariffId = tariff.TariffId,
                TariffName = tariff.TariffName,

            };
            return tariffInfo;
        }

        #endregion
    }
}
