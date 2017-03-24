using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Data;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;
using TOne.WhS.Deal.Entities.Settings;
using System.ComponentModel;

namespace TOne.WhS.Deal.Business
{
    public class SwapDealManager: BaseDealManager
    {

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<DealDefinitionDetail> GetFilteredSwapDeals(Vanrise.Entities.DataRetrievalInput<SwapDealQuery> input)
        {
            var cachedEntities = this.GetCachedSwapDeals();
            Func<DealDefinition, bool> filterExpression = (deal) =>
            {
                if (input.Query.Name != null && !deal.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.CarrierAccountIds != null && !input.Query.CarrierAccountIds.Contains((deal.Settings as SwapDealSettings).CarrierAccountId))
                    return false;

                return true;
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedEntities.ToBigResult(input, filterExpression, DealDeinitionDetailMapper));
        }
        
        public SwapDealSettingData GetSwapDealSettingData()
        {
            var settingManager = new SettingManager();
            Setting setting = settingManager.GetSettingByType(Constants.SwapDealSettings);
            if (setting == null)
                throw new NullReferenceException("setting");
            if (setting.Data == null)
                throw new NullReferenceException("setting.Data");
            var swapDealAnalysisSettingData = setting.Data as SwapDealSettingData;
            if (swapDealAnalysisSettingData == null)
                throw new NullReferenceException("swapDealAnalysisSettingData");
            return swapDealAnalysisSettingData;
        }
        public override DealDefinitionDetail DealDeinitionDetailMapper(DealDefinition deal)
        {
            SwapDealDetail detail = new SwapDealDetail()
            {
                Entity = deal,
            };

            SwapDealSettings settings = deal.Settings as SwapDealSettings;
            int carrierAccountId = settings.CarrierAccountId;
            detail.CarrierAccountName = new CarrierAccountManager().GetCarrierAccountName(carrierAccountId);
            detail.TypeDescription = Utilities.GetEnumAttribute<DealType, DescriptionAttribute>(settings.DealType).Description;
            detail.ContractDescription = Utilities.GetEnumAttribute<DealContract, DescriptionAttribute>(settings.DealContract).Description;
            detail.IsEffective = settings.BeginDate <= DateTime.Now.Date && settings.EndDate >= DateTime.Now.Date;
            detail.BuyingAmount = settings.Outbounds.Sum(x => x.Volume * x.Rate);
            detail.BuyingVolume = settings.Outbounds.Sum(x => x.Volume);
            detail.SellingAmount = settings.Inbounds.Sum(x => x.Volume * x.Rate);
            detail.SellingVolume = settings.Inbounds.Sum(x => x.Volume);

            return detail;
        }
        #endregion

        #region Mappers      

       
        #endregion
    }
}
