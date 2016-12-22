using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountDefinition")]
    public class AccountDefinitionController : BaseAPIController
    {
        AccountDefinitionManager _manager = new AccountDefinitionManager();

        [HttpGet]
        [Route("GetAccountViewDefinitionSettingsConfigs")]
        public IEnumerable<AccountViewDefinitionSettingsConfig> GetAccountViewDefinitionSettingsConfigs()
        {
            return _manager.GetAccountViewDefinitionSettingsConfigs();
        }

        [HttpGet]
        [Route("GetAccountGridColumnAttributes")]
        public List<DataRecordGridColumnAttribute> GetAccountGridColumnAttributes()
        {
            return _manager.GetAccountGridColumnAttributes();
        }

        [HttpGet]
        [Route("GetAccountViewDefinitions")]
        public List<AccountViewDefinition> GetAccountViewDefinitions()
        {
            return _manager.GetAccountViewDefinitions();
        }
        [HttpGet]
        [Route("GetAccountViewDefinitionsByAccount")]
        public List<AccountViewDefinition> GetAccountViewDefinitionsByAccount(long accountId)
        {
            return _manager.GetAccountViewDefinitionsByAccount(accountId);
        }
    }
}