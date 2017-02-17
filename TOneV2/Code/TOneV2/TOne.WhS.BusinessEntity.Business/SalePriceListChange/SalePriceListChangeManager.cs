using System.Linq;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SalePriceListChangeManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SalePricelistRateChange> GetFilteredPricelistRateChanges(Vanrise.Entities.DataRetrievalInput<SalePriceListChangeQuery> input)
        {
            ISalePriceListChangeDataManager dataManager =
                BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            var salePriceListRateChanges =
                dataManager.GetFilteredSalePricelistRateChanges(input.Query.PriceListId, input.Query.Countries)
                    .ToDictionary(c => c.ZoneName, c => c);
            return DataRetrievalManager.Instance.ProcessResult(input,
                salePriceListRateChanges.ToBigResult(input, null, SalePricelistRateChangeDetailMapper));
        }
        public Vanrise.Entities.IDataRetrievalResult<SalePricelistCodeChange> GetFilteredPricelistCodeChanges(Vanrise.Entities.DataRetrievalInput<SalePriceListChangeQuery> input)
        {
            ISalePriceListChangeDataManager dataManager =
                BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            var salePriceListRateChanges =
                dataManager.GetFilteredSalePricelistCodeChanges(input.Query.PriceListId, input.Query.Countries)
                    .ToDictionary(c => c.Code, c => c);
            return DataRetrievalManager.Instance.ProcessResult(input,
                salePriceListRateChanges.ToBigResult(input, null, SalePricelistCodeChangeDetailMapper));
        }
        #region Mapper
        private SalePricelistRateChange SalePricelistRateChangeDetailMapper(SalePricelistRateChange salePricelistRateChange)
        {
            return salePricelistRateChange;
        }
        private SalePricelistCodeChange SalePricelistCodeChangeDetailMapper(SalePricelistCodeChange salePricelistCodeChange)
        {
            return salePricelistCodeChange;
        }
        #endregion

    }
}
