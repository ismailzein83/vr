using Demo.Module.Business;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "PageDefinition")]
    [JSONWithTypeAttribute]
    public class PageDefinitionController : BaseAPIController
    {
        PageDefinitionManager pageDefinitionManager = new PageDefinitionManager();
        [HttpPost]
        [Route("GetFilteredPageDefinitions")]
        public object GetFilteredPageDefinitions(DataRetrievalInput<PageDefinitionQuery> input)
        {
            return GetWebResponse(input, pageDefinitionManager.GetFilteredPageDefinitions(input));
        }

        [HttpGet]
        [Route("GetPageDefinitionById")]
        public PageDefinition GetPageDefinitionById(int pageDefinitionId)
        {
            return pageDefinitionManager.GetPageDefinitionById(pageDefinitionId);
        }


        [HttpPost]
        [Route("UpdatePageDefinition")]
        public UpdateOperationOutput<PageDefinitionDetails> UpdatePageDefinition(PageDefinition pageDefinition)
        {
            return pageDefinitionManager.UpdatePageDefinition(pageDefinition);
        }

        [HttpPost]
        [Route("AddPageDefinition")]
        public InsertOperationOutput<PageDefinitionDetails> AddPageDefinition(PageDefinition pageDefinition)
        {
            return pageDefinitionManager.AddPageDefinition(pageDefinition);
        }

        [HttpGet]
        [Route("GetPageDefinitionsInfo")]
        public IEnumerable<PageDefinitionInfo> SpecialitiesInfo(string filter = null)
        {
            return pageDefinitionManager.GetPageDefinitionsInfo();
        }
        [HttpGet]

        [Route("GetFieldTypeConfigs")]
        public IEnumerable<FieldTypeConfig> GetFieldTypeConfigs()
        {
            return pageDefinitionManager.GetFieldTypeConfigs();
        }

        [HttpGet]

        [Route("GetSubviewConfigs")]
        public IEnumerable<SubViewConfig> GetSubviewConfigs()
        {
            return pageDefinitionManager.GetSubViewConfigs();
        }

        [HttpGet]

        [Route("GetPageDefinitionConfigs")]
        public IEnumerable<PageDefinitionConfig> GetPageDefinitionConfigs()
        {
            return pageDefinitionManager.GetPageDefinitionConfigs();
        }

    }
}