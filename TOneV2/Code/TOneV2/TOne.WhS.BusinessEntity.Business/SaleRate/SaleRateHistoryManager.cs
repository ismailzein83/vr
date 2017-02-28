using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateHistoryManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<SellingProductZoneRateHistoryRecordDetail> GetFilteredSellingProductZoneRateHistoryRecords(Vanrise.Entities.DataRetrievalInput<SellingProductZoneRateHistoryQuery> input)
        {
            return Vanrise.Common.Business.BigDataManager.Instance.RetrieveData(input, new SellingProductZoneRateHistoryRequestHandler());
        }

        #endregion

        #region Private Classes

        private class SellingProductZoneRateHistoryRequestHandler : Vanrise.Common.Business.BigDataRequestHandler<SellingProductZoneRateHistoryQuery, SellingProductZoneRateHistoryRecord, SellingProductZoneRateHistoryRecordDetail>
        {
            public override SellingProductZoneRateHistoryRecordDetail EntityDetailMapper(SellingProductZoneRateHistoryRecord entity)
            {
                return new SellingProductZoneRateHistoryRecordDetail()
                {
                    Entity = entity
                };
            }

            public override IEnumerable<SellingProductZoneRateHistoryRecord> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SellingProductZoneRateHistoryQuery> input)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
