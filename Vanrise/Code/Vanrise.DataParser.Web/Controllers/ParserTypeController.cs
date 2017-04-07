using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;
using Vanrise.Web.Base;

namespace Vanrise.DataParser.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ParserType")]
    [JSONWithTypeAttribute]
    public class ParserTypeController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredParserTypes")]
        public object GetFilteredParserTypes(Vanrise.Entities.DataRetrievalInput<ParserTypeQuery> input)
        {
            ParserTypeManager manager = new ParserTypeManager();
            return GetWebResponse(input, manager.GetFilteredParserTypes(input));
        }

        [HttpGet]
        [Route("GetParserType")]
        public ParserType GetParserType(Guid parserTypeId)
        {
            ParserTypeManager manager = new ParserTypeManager();
            return manager.GetParserType(parserTypeId);
        }

        [HttpPost]
        [Route("AddParserType")]
        public Vanrise.Entities.InsertOperationOutput<ParserTypeDetail> AddParserType(ParserType parserType)
        {
            ParserTypeManager manager = new ParserTypeManager();
            return manager.AddParserType(parserType);
        }

        [HttpPost]
        [Route("UpdateParserType")]
        public Vanrise.Entities.UpdateOperationOutput<ParserTypeDetail> UpdateParserType(ParserType parserType)
        {
            ParserTypeManager manager = new ParserTypeManager();
            return manager.UpdateParserType(parserType);
        }
    }
}