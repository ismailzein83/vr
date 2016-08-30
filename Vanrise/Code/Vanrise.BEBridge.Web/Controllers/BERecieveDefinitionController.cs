using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.BEBridge.Business;
using Vanrise.BEBridge.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BEBridge.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "BERecieveDefinition")]
    public class BERecieveDefinitionController : BaseAPIController
    {
        BEReceiveDefinitionManager _manager = new BEReceiveDefinitionManager();
        [HttpGet]
        [Route("GetBERecieveDefinitionsInfo")]
        public object GetBERecieveDefinitionsInfo()
        {
            return _manager.GetBEReceiveDefinitionsInfo();
        }
        [HttpPost]
        [Route("GetFilteredBeReceiveDefinitions")]
        public object GetFilteredBeReceiveDefinitions(DataRetrievalInput<BEReceiveDefinitionQuery> input)
        {
            input.SortByColumnName = "Entity.BEReceiveDefinitionId";
            return GetWebResponse(input, _manager.GetFilteredBeReceiveDefinitions(input));
        }
        [HttpGet]
        [Route("GetReceiveDefinition")]
        public BEReceiveDefinition GetReceiveDefinition(Guid receiveDefinitionId)
        {
            return _manager.GetReceiveDefinition(receiveDefinitionId);
        }
        [HttpPost]
        [Route("UpdateReceiveDefinition")]
        public UpdateOperationOutput<BEReceiveDefinitionDetail> UpdateReceiveDefinition(BEReceiveDefinition beReceiveDefinition)
        {
            return _manager.UpdateReceiveDefinition(beReceiveDefinition);
        }
        [HttpPost]
        [Route("AddReceiveDefinition")]
        public InsertOperationOutput<BEReceiveDefinitionDetail> AddStatusChargingSet(BEReceiveDefinition beReceiveDefinition)
        {
            return _manager.AddReceiveDefinition(beReceiveDefinition);
        }
        [HttpGet]
        [Route("GetSourceReaderExtensionConfigs")]
        public IEnumerable<SourceBeReadersConfig> GetSourceReaderExtensionConfigs()
        {
            return _manager.GetSourceReaderExtensionConfigs();
        }
        [HttpGet]
        [Route("GetTargetSynchronizerExtensionConfigs")]
        public IEnumerable<TargetBESynchronizerConfig> GetTargetSynchronizerExtensionConfigs()
        {
            return _manager.GetTargetSynchronizerExtensionConfigs();
        }
        [HttpGet]
        [Route("GetTargetConvertorExtensionConfigs")]
        public IEnumerable<TargetBEConvertorConfig> GetTargetConvertorExtensionConfigs()
        {
            return _manager.GetTargetConvertorExtensionConfigs();
        }
    }
}