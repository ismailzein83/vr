using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.DevTools.Entities;
using Vanrise.DevTools.Business;
namespace Vanrise.DevTools.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Columns")]
    [JSONWithTypeAttribute]
    public class ColumnsController : BaseAPIController
    {
        VRGeneratedScriptColumnsManager columnsManager = new VRGeneratedScriptColumnsManager();


        [HttpGet]
        [Route("GetColumnsInfo")]
        public IEnumerable<VRGeneratedScriptColumnsInfo> GetColumnsInfo(string filter = null)
        {
            VRGeneratedScriptColumnsInfoFilter columnsInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<VRGeneratedScriptColumnsInfoFilter>(filter) : null;
            return columnsManager.GetColumnsInfo(columnsInfoFilter);
        }
        [HttpGet]

        [Route("GetGeneratedScriptItemTableSettingsConfigs")]
        public IEnumerable<GeneratedScriptItemTableSettingsConfig> GetGeneratedScriptItemTableSettingsConfigs()
        {
            return columnsManager.GetGeneratedScriptItemTableSettingsConfigs();
        }
        [HttpGet]

        [Route("GetGeneratedScriptVariableSettingsConfigs")]
        public IEnumerable<GeneratedScriptVariableSettingsConfig> GetGeneratedScriptVariableSettingsConfigs()
        {
            return columnsManager.GetGeneratedScriptVariableSettingsConfigs();
        }
        
        [HttpPost]
        [Route("Validate")]
        public List<GeneratedScriptItemValidatorOutput> Validate(GeneratedScriptItemTables generatedScriptItemTables)
        {
            return columnsManager.Validate(generatedScriptItemTables);
        }

        [HttpPost]
        [Route("GenerateQueries")]
        public string GenerateQueries(GeneratedScriptItem generatedScriptItem)
        {
            return columnsManager.GenerateQueries(generatedScriptItem);
        }

        [HttpGet]
        [Route("GetGeneratedQueries")]
        public string GetGeneratedQueries(string jsonScripts, GeneratedScriptType type)
        {
            return columnsManager.GetGeneratedQueries(jsonScripts, type);
        }

        [HttpGet]
        [Route("GenerateQueriesFromTextFile")]
        public string GenerateQueriesFromTextFile(string filePath, GeneratedScriptType type)
        {
            return columnsManager.GenerateQueriesFromTextFile(filePath, type);
        }
    }
}