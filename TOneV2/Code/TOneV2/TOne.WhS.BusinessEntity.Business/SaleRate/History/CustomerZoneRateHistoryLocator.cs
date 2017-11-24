using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerZoneRateHistoryLocator
    {
        #region Fields
        private int _customerId;
        private CustomerZoneRateHistoryReader _reader;
        private SaleRateManager _saleRateManager = new SaleRateManager();
        #endregion

        #region Constructors
        public CustomerZoneRateHistoryLocator(int customerId, CustomerZoneRateHistoryReader reader)
        {
            _customerId = customerId;
            _reader = reader;
        }
        #endregion

        public IEnumerable<SaleRateHistoryRecord> GetSaleRateHistory(string zoneName, int countryId, int? rateTypeId, int currencyId)
        {
            return RateHistoryUtilities.GetSaleRateHistory(_customerId, zoneName, countryId, rateTypeId, currencyId, _reader);
        }
        public SaleRateHistoryRecord GetSaleRateHistoryRecord(string zoneName, int countryId, int? rateTypeId, int currencyId, DateTime effectiveOn)
        {
            return RateHistoryUtilities.GetSaleRateHistoryRecord(_customerId, zoneName, countryId, rateTypeId, currencyId, effectiveOn, _reader);
        }
    }
}
