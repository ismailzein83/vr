using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.Entities;
using Vanrise.Entities;

namespace TOne.BusinessEntity.Business
{
    public class CurrencyManager
    {
        ICurrencyDataManager _dataManager;

        public CurrencyManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<ICurrencyDataManager>();
        }

        public List<Currency> GetCurrencies()
        {
            return _dataManager.GetCurrencies();
        }
    }
}
