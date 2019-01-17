using Demo.Module.Business;
using Demo.Module.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Demo_Branch")]
    [JSONWithTypeAttribute]
    public class Demo_Module_BranchController : BaseAPIController
    {
        BranchManager _branchManager = new BranchManager();
        
        [HttpPost]
        [Route("GetFilteredBranches")]
        public object GetFilteredBranches(DataRetrievalInput<BranchQuery> input)
        {
            return GetWebResponse(input, _branchManager.GetFilteredBranches(input));
        }

        [HttpGet]
        [Route("GetBranchById")]
        public Branch GetBranchById(int branchId)
        {
            return _branchManager.GetBranchById(branchId);
        }

        [HttpPost]
        [Route("AddBranch")]
        public InsertOperationOutput<BranchDetail> AddBranch(Branch branch)
        {
            return _branchManager.AddBranch(branch);
        }

        [HttpPost]
        [Route("UpdateBranch")]
        public UpdateOperationOutput<BranchDetail> UpdateBranch(Branch branch)
        {
            return _branchManager.UpdateBranch(branch);
        }

        [HttpGet]
        [Route("GetBranchTypeConfigs")]
        public IEnumerable<BranchTypeConfig> GetBranchTypeConfigs()
        {
            return _branchManager.GetBranchTypeConfigs();
        }
        [HttpGet]
        [Route("GetDepartmentSettingsConfigs")]
        public IEnumerable<DepartmentSettingsConfig> GetDepartmentSettingsConfigs()
        {
            return _branchManager.GetDepartmentSettingsConfigs();
        }

        [HttpGet]
        [Route("GetEmployeeSettingsConfigs")]
        public IEnumerable<EmployeeSettingsConfig> GetEmployeeSettingsConfigs()
        {
            return _branchManager.GetEmployeeSettingsConfigs();
        }
    }
}