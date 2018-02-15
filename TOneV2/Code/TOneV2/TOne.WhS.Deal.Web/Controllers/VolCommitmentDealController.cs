using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Web.Base;
using Vanrise.Entities;
using TOne.WhS.Deal.Entities.Settings;

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
        [Route("GetDealSaleRateEvaluator")]
        public IEnumerable<DealSaleRateEvaluatorConfig> GetDealSaleRateEvaluator()
        {
            ConfigManager configManager = new ConfigManager();
            return configManager.GetDealSaleRateEvaluator();
        }

        [HttpGet]
        [Route("GetDealSupplierRateEvaluator")]
        public IEnumerable<DealSupplierRateEvaluatorConfig> GetDealSupplierRateEvaluator()
        {
            ConfigManager configManager = new ConfigManager();
            return configManager.GetDealSupplierRateEvaluator();
        }

    }
}