using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Web.Base;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Deal.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "VolCommitmentDeal")]
    public class VolCommitmentDealController : Vanrise.Web.Base.BaseAPIController
    {
        VolCommitmentDealManager _manager = new VolCommitmentDealManager();

        [HttpPost]
        [Route("GetFilteredVolCommitmentDeals")]
        public object GetFilteredVolCommitmentDeals(Vanrise.Entities.DataRetrievalInput<VolCommitmentDealQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredVolCommitmentDeals(input));
        }
        [HttpGet]
        [Route("GetVolumeCommitmentHistoryDetailbyHistoryId")]
        public DealDefinition GetVolumeCommitmentHistoryDetailbyHistoryId(int volumeCommitmentHistoryId)
        {
            return _manager.GetVolumeCommitmentHistoryDetailbyHistoryId(volumeCommitmentHistoryId);
        }
        [HttpGet]
        [Route("GetDeal")]
        public DealDefinition GetDeal(int dealId)
        {
            return _manager.GetDeal(dealId);
        }

        [HttpPost]
        [Route("UpdateDeal")]
        public Vanrise.Entities.UpdateOperationOutput<DealDefinitionDetail> UpdateDeal(DealDefinition deal)
        {
            return _manager.UpdateDeal(deal);
        }
        [HttpPost]
        [Route("AddDeal")]
        public Vanrise.Entities.InsertOperationOutput<DealDefinitionDetail> AddDeal(DealDefinition deal)
        {
            return _manager.AddDeal(deal);
        }
        [HttpGet]
        [Route("GetSaleRateEvaluatorConfigurationTemplateConfigs")]
        public IEnumerable<DealSaleRateEvaluatorConfig> GetSaleRateEvaluatorConfigurationTemplateConfigs()
        {
            ConfigManager configManager = new ConfigManager();
            return configManager.GetSaleRateEvaluatorConfigurationTemplateConfigs();
        }

        [HttpGet]
        [Route("GetSupplierRateEvaluatorConfigurationTemplateConfigs")]
        public IEnumerable<DealSupplierRateEvaluatorConfig> GetSupplierRateEvaluatorConfigurationTemplateConfigs()
        {
            ConfigManager configManager = new ConfigManager();
            return configManager.GetSupplierRateEvaluatorConfigurationTemplateConfigs();
        }

        [HttpGet]
        [Route("ReoccurDeal")]
        public InsertDealOperationOutput<List<DealDefinitionDetail>> ReoccurDeal(int dealId, int reoccuringNumber, ReoccuringType reoccuringType)
        {
            return _manager.ReoccurDeal(dealId, reoccuringNumber, reoccuringType);
        }

    }
}