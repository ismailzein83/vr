//using System;
//using System.Collections.Generic;
//using System.Web.Http;
//using Vanrise.GenericData.Entities;
//using Vanrise.Web.Base;

//namespace Retail.BusinessEntity.MainExtensions.PackageTypes
//{
//    [JSONWithTypeAttribute]
//    [RoutePrefix(Constants.ROUTE_PREFIX + "VolumePackage")]
//    public class VolumePackageController : BaseAPIController
//    {
//        [HttpGet]
//        [Route("GetCompositeRecordConditionResolvedDataList")]
//        public List<CompositeRecordConditionResolvedData> GetCompositeRecordConditionResolvedDataList(Guid volumePackageDefinitionId, Guid volumePackageDefinitionItemId)
//        {
//            return new VolumePackageManager().GetCompositeRecordConditionResolvedDataList(volumePackageDefinitionId, volumePackageDefinitionItemId);
//        }
//    }
//}