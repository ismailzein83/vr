using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Demo.Module.Entities;
using Demo.Module.Business;
using Vanrise.Entities;


namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Building")]
    [JSONWithTypeAttribute]
    public class Demo_Module_BuildingController : BaseAPIController
    {
    //    BranchManager branchManager = new BranchManager();
       

    //    [HttpPost]
    //    [Route("AddBranch")]
    //    public InsertOperationOutput<BranchDetails> AddBranch(Branch branch)
    //    {
    //        return branchManager.AddBranch(branch);
    //    }

    //    [HttpPost]
    //    [Route("GetFilteredBranches")]
    //    public object GetFilteredBranches(DataRetrievalInput<BranchQuery> input)
    //    {
    //        return GetWebResponse(input, branchManager.GetFilteredBranches(input));
    //    }

    //    [HttpGet]
    //    [Route("GetBranchById")]
    //    public Branch GetBranchById(int branchId)
    //    {
    //        return branchManager.GetBranchById(branchId);
    //    }


    //    [HttpPost]
    //    [Route("UpdateBranch")]
    //    public UpdateOperationOutput<BranchDetails> UpdateBranch(Branch branch)
    //    {
    //        return branchManager.UpdateBranch(branch);
    //    }
    }
}
