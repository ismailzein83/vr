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
    [RoutePrefix(Constants.ROUTE_PREFIX + "DevProject")]
    [JSONWithTypeAttribute]
    public class VRDevProjectController : BaseAPIController
    {
        VRDevProjectManager _manager = new VRDevProjectManager();

        [HttpGet]
        [Route("GetVRDevProjectsInfo")]
        public IEnumerable<VRDevProjectInfo> GetVRDevProjectsInfo(string filter = null)
        {
            VRDevProjectInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<VRDevProjectInfoFilter>(filter) : null;
            return _manager.GetVRDevProjectsInfo(deserializedFilter);
        }
        [HttpGet]
        [Route("TryCompileDevProject")]
        public VRDevProjectCompilationOutput TryCompileDevProject(Guid devProjectId, BuildOptionEnum buildOption)
        {
            VRDevProjectCompilationOutput compilationOutput = new VRDevProjectCompilationOutput() { ErrorMessages = new List<string>() };
            CSharpCompilationOutput cSharpCompilationOutput;

            switch (buildOption)
            {
                case BuildOptionEnum.Build:
                    compilationOutput.Result = _manager.TryCompileDevProject(devProjectId, out cSharpCompilationOutput);
                    if (cSharpCompilationOutput.ErrorMessages != null)
                        compilationOutput.ErrorMessages = cSharpCompilationOutput.ErrorMessages;
                    return compilationOutput;

                default: return compilationOutput;
            }
        }
    }
    public class VRDevProjectCompilationOutput
    {
        public List<string> ErrorMessages { get; set; }

        public bool Result { get; set; }
    }
    public enum BuildOptionEnum { Build = 0, BuildWithDependencies = 1 }

}