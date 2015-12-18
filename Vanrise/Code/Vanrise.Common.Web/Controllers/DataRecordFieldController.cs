using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business.GenericDataRecord;
using Vanrise.Entities.GenericDataRecord;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "DefineCDRFields")]
    [JSONWithTypeAttribute]
    public class DataRecordFieldController:BaseAPIController
    {
        //[HttpPost]
        //[Route("GetFilteredDataRecordFields")]
        //public object GetFilteredDataRecordFields(Vanrise.Entities.DataRetrievalInput<DataRecordFieldQuery> input)
        //{
        //    DataRecordFieldManager manager = new DataRecordFieldManager();
        //    return GetWebResponse(input, manager.GetFilteredDataRecordFields(input));
        //}

        //[HttpPost]
        //[Route("UpdateDataRecordField")]
        //public Vanrise.Entities.UpdateOperationOutput<DataRecordFieldDetail> UpdateDataRecordField(DataRecordField dataRecordField)
        //{
        //    DataRecordFieldManager manager = new DataRecordFieldManager();
        //    return manager.UpdateDataRecordField(dataRecordField);
        //}

        //[HttpPost]
        //[Route("AddDataRecordField")]
        //public Vanrise.Entities.InsertOperationOutput<DataRecordFieldDetail> AddDataRecordField(DataRecordField dataRecordField)
        //{
        //    DataRecordFieldManager manager = new DataRecordFieldManager();
        //    return manager.AddDataRecordField(dataRecordField);
        //}

        //[HttpGet]
        //[Route("GetDataRecordFieldTypeTemplates")]
        //public List<Vanrise.Entities.TemplateConfig> GetDataRecordFieldTypeTemplates()
        //{
        //    DataRecordFieldManager manager = new DataRecordFieldManager();
        //    return manager.GetDataRecordFieldTypeTemplates();
        //}

        //[HttpGet]
        //[Route("DeleteDataRecordField")]
        //public Vanrise.Entities.DeleteOperationOutput<DataRecordFieldDetail> DeleteDataRecordField(int dataRecordFieldId)
        //{
        //    DataRecordFieldManager manager = new DataRecordFieldManager();
        //    return manager.DeleteDataRecordField(dataRecordFieldId);
        //}
        //[HttpGet]
        //[Route("GetDataRecordField")]
        //public DataRecordField GetDataRecordField(int cdrFieldId)
        //{
        //    DataRecordFieldManager manager = new DataRecordFieldManager();
        //    return manager.GetDataRecordField(cdrFieldId);
        //}
    }
}