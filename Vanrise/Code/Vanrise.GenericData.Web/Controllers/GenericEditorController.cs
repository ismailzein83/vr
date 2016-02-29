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
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericEditor")]
    public class GenericEditorController:BaseAPIController
    {
        [HttpGet]
        [Route("GetEditor")]
        public GenericEditor GetEditor(int businessEntityId, int dataRecordTypeId)
        {
            GenericEditorManager manager = new GenericEditorManager();
            return manager.GetEditor(businessEntityId, dataRecordTypeId);
        }
        //[HttpGet]
        //[Route("GetEditorRuntime")]
        //public GenericEditorRuntime GetEditorRuntime(int editorId)
        //{
        //    GenericEditorManager manager = new GenericEditorManager();
        //    return manager.GetEditorRuntime(editorId);
        //}
        [HttpGet]
        [Route("GetEditorRuntime")]
        public GenericEditorRuntime GetEditorRuntime(int businessEntityId, int dataRecordTypeId)
        {
            GenericEditorManager manager = new GenericEditorManager();
            return manager.GetEditorRuntime(businessEntityId, dataRecordTypeId);
        }
        [HttpGet]
        [Route("GetEditorRuntimeMock")]
        public GenericEditorRuntime GetEditorRuntimeMock(int editorId)
        {
            GenericEditorManager manager = new GenericEditorManager();
            return manager.GetEditorRuntimeMock(editorId);
        }
        [HttpGet]
        [Route("GetGenericEditorDefinition")]
        public GenericEditorDefinition GetGenericEditorDefinition(int editorId)
        {
            GenericEditorManager manager = new GenericEditorManager();
            return manager.GetGenericEditorDefinition(editorId);
        }
        [Route("UpdateGenericEditor")]
        public Vanrise.Entities.UpdateOperationOutput<GenericEditorDefinitionDetail> UpdateGenericEditor(GenericEditorDefinition genericEditor)
        {
            GenericEditorManager manager = new GenericEditorManager();
            return manager.UpdateGenericEditor(genericEditor);
        }

        [HttpPost]
        [Route("AddGenericEditor")]
        public Vanrise.Entities.InsertOperationOutput<GenericEditorDefinitionDetail> AddGenericEditor(GenericEditorDefinition genericEditor)
        {
            GenericEditorManager manager = new GenericEditorManager();
            return manager.AddGenericEditor(genericEditor);
        }
        [HttpPost]
        [Route("GetFilteredGenericEditorDefinitions")]
        public object GetFilteredGenericEditorDefinitions(Vanrise.Entities.DataRetrievalInput<GenericEditorDefinitionQuery> input)
        {
            GenericEditorManager manager = new GenericEditorManager();
            return GetWebResponse(input, manager.GetFilteredGenericEditorDefinitions(input));
        }
    }
}