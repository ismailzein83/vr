using System;
using System.Web.Http;
using Vanrise.Web.Base;
using Demo.Module.Business;
using Vanrise.Entities;
using Demo.Module.Entities;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Demo_ZooSection") ]
    [JSONWithTypeAttribute]
    public class ZooSectionController : BaseAPIController
    {
        //ZooSectionManager _zooSectionManager = new ZooSectionManager();

        //[HttpPost]
        //[Route("GetFilteredZooSections")]
        //public object GetFilteredZooSections(DataRetrievalInput<ZooSectionQuery> input)
        //{
        //    return GetWebResponse(input, _zooSectionManager.GetFilteredZooSections(input));
        //}

        //[HttpPost]
        //[Route("AddZooSection")]
        //public InsertOperationOutput<ZooSectionDetail> AddZooSection(ZooSection zooSection)
        //{
        //    return _zooSectionManager.AddZooSection(zooSection);
        //}

        //[HttpPost]
        //[Route("UpdateZooSection")]
        //public UpdateOperationOutput<ZooSectionDetail> UpdateZooSection(ZooSection zooSection)
        //{
        //    return _zooSectionManager.UpdateZooSection(zooSection);
        //}

    }
}