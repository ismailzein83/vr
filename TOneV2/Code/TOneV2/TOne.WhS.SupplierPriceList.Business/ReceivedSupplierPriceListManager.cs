using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ReceivedSupplierPricelistManager
    {

        #region Public Methods
        public IDataRetrievalResult<ReceivedPricelistDetail> GetFilteredReceivedPricelists(DataRetrievalInput<ReceivedPricelistQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new ReceivedPricelistRequestHandler());
        }

        #endregion

        #region Private Classes

        private class ReceivedPricelistRequestHandler : BigDataRequestHandler<ReceivedPricelistQuery, ReceivedPricelist, ReceivedPricelistDetail>
        {

            public override ReceivedPricelistDetail EntityDetailMapper(ReceivedPricelist entity)
            {
                return new ReceivedPricelistDetail()
                {
                    ReceivedPricelist = entity,
                    SupplierName = new CarrierAccountManager().GetCarrierAccountName(entity.SupplierId),
                    StatusDescription = Vanrise.Common.Utilities.GetEnumDescription(entity.Status),
                    PriceListTypeDescription = entity.PricelistType.HasValue ? Vanrise.Common.Utilities.GetEnumDescription(entity.PricelistType.Value) : null,
                };
            }

            public override IEnumerable<ReceivedPricelist> RetrieveAllData(DataRetrievalInput<ReceivedPricelistQuery> input)
            {
                IReceivedPricelistManager receivedPricelistDataManager = SupPLDataManagerFactory.GetDataManager<IReceivedPricelistManager>();
                return receivedPricelistDataManager.GetFilteredReceivedPricelists(input.Query);
            }
        }

        #endregion
    }
}
