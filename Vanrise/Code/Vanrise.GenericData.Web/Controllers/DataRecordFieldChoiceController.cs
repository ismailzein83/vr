using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataRecordFieldChoice")]
    public class DataRecordFieldChoiceController:BaseAPIController
    {
        DataRecordFieldChoiceManager _manager = new DataRecordFieldChoiceManager();

        [HttpPost]
        [Route("GetFilteredDataRecordFieldChoices")]
        public object GetFilteredDataRecordFieldChoices(Vanrise.Entities.DataRetrievalInput<DataRecordFieldChoiceQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredDataRecordFieldChoices(input));
        }

        [HttpGet]
        [Route("GetDataRecordFieldChoicesInfo")]
        public IEnumerable<DataRecordFieldChoiceInfo> GetDataRecordFieldChoicesInfo()
        {
            return _manager.GetDataRecordFieldChoicesInfo();
        }

        [HttpGet]
        [Route("GetDataRecordFieldChoice")]
        public DataRecordFieldChoice GetDataRecordFieldChoice(Guid dataRecordFieldChoiceId)
        {
            return _manager.GeDataRecordFieldChoice(dataRecordFieldChoiceId);
        }

        [HttpPost]
        [Route("AddDataRecordFieldChoice")]
        public Vanrise.Entities.InsertOperationOutput<DataRecordFieldChoiceDetail> AddDataRecordFieldChoice(DataRecordFieldChoice dataRecordFieldChoice)
        {
            return _manager.AddDataRecordFieldChoice(dataRecordFieldChoice);
        }

        [HttpPost]
        [Route("UpdateDataRecordFieldChoice")]
        public Vanrise.Entities.UpdateOperationOutput<DataRecordFieldChoiceDetail> UpdateDataRecordFieldChoice(DataRecordFieldChoice dataRecordFieldChoice)
        {
            return _manager.UpdateDataRecordFieldChoice(dataRecordFieldChoice);
        }
    }
}