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
        [Route("GetEditorRuntimeMock")]
        public GenericEditorRuntime GetEditorRuntimeMock(int editorId)
        {
            GenericEditorManager manager = new GenericEditorManager();
            return manager.GetEditorRuntimeMock(editorId);
        }
    }
}