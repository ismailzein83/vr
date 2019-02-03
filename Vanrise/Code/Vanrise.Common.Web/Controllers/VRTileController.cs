using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRTile")]
    [JSONWithTypeAttribute]

    public class VRTileController : BaseAPIController
    {
        VRTileManager _manager = new VRTileManager();

        [HttpGet]
        [Route("GetTileExtendedSettingsConfigs")]
        public IEnumerable<VRTileExtendedSettingsConfig> GetTileExtendedSettingsConfigs()
        {
            return _manager.GetTileExtendedSettingsConfigs();
        }

        [HttpGet]
        [Route("GetFiguresTilesDefinitionSettingsConfigs")]
        public IEnumerable<FiguresTileQueryDefinitionSettingsConfig> GetFiguresTilesDefinitionSettingsConfigs()
        {
            return _manager.GetFiguresTilesDefinitionSettingsConfigs();
        }
        [HttpPost]
        [Route("GetFiguresTileItemsToDiplayInfo")]
        public IEnumerable<FiguresTileSchemaInfo> GetFiguresTileItemsToDiplayInfo(FiguresTileQueriesInput queriesInput)
        {
            return _manager.GetFiguresTileItemsToDiplayInfo(queriesInput);
        }
        [HttpPost]
        [Route("GetFigureItemsValue")]
        public IEnumerable<FigureItemValue> GetFigureItemsValue(FigureStyleInput figureStyleInput)
        {
            return _manager.GetFigureItemsValue(figureStyleInput.Queries,figureStyleInput.ItemsToDisplay);
        }
        

    }
    public class FigureStyleInput
    {
        public List<FiguresTileQuery> Queries { get; set; }
        public List<FiguresTileDisplayItem> ItemsToDisplay { get; set; }
    }
   
}