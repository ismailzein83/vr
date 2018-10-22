using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.DataParser.BinaryParser;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;
using Vanrise.DataParser.Entities.HexTLV;
using Vanrise.Web.Base;


namespace Vanrise.DataParser.Web.Controllers
{

    [RoutePrefix(Constants.ROUTE_PREFIX + "ParserTypeConfigs")]
    [JSONWithTypeAttribute]
    public class ParserTypeConfigsController : BaseAPIController
    {
        ParserTypeConfigsManager _manager = new ParserTypeConfigsManager();

        [HttpGet]
        [Route("GetParserTypeTemplateConfigs")]
        public IEnumerable<ParserTypeExtendedSettingsConfig> GetParserTypeTemplateConfigs()
        {
            return _manager.GetParserTypeTemplateConfigs();
        }

        [HttpGet]
        [Route("GetRecordeReaderTemplateConfigs")]
        public IEnumerable<HexTLVRecordReadersConfig> GetRecordeReaderTemplateConfigs()
        {
            return _manager.GetRecordeReaderTemplateConfigs();
        }

        [HttpGet]
        [Route("GetTagValueParserTemplateConfigs")]
        public IEnumerable<HexTLVTagValueParserConfig> GetTagValueParserTemplateConfigs()
        {
            return _manager.GetTagValueParserTemplateConfigs();
        }


        [HttpGet]
        [Route("GetBinaryRecordReaderTemplateConfigs")]
        public IEnumerable<BinaryRecordReadersConfig> GetBinaryRecordReaderTemplateConfigs()
        {
            return _manager.GetBinaryRecordReaderTemplateConfigs();
        }
        [HttpGet]
        [Route("GetBaseStringParserTemplateConfigs")]
        public IEnumerable<BaseStringConfig> GetBaseStringParserTemplateConfigs()
        {
            return _manager.GetBaseStringParserTemplateConfigs();
        }
        [HttpGet]
        [Route("GetCompositeFieldsReaderTemplateConfigs")]
        public IEnumerable<CompositeFieldsConfig> GetCompositeFieldsReaderTemplateConfigs()
        {
            return _manager.GetCompositeFieldsReaderTemplateConfigs();
        }
        [HttpGet]
        [Route("GetPackageFieldsReaderTemplateConfigs")]
        public IEnumerable<PackageFieldsConfig> GetPackageFieldsReaderTemplateConfigs()
        {
            return _manager.GetPackageFieldsReaderTemplateConfigs();
        }
    }
}
