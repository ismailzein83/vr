using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.MainExtensions.PackageTypes
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "VolumePackageDefinition")]
    public class VolumePackageDefinitionController : BaseAPIController
    {
        [HttpGet]
        [Route("GetCompositeRecordConditionResolvedDataList")]
        public List<CompositeRecordConditionResolvedData> GetCompositeRecordConditionResolvedDataList(Guid volumePackageDefinitionId, Guid volumePackageDefinitionItemId)
        {
            return new VolumePackageDefinitionManager().GetCompositeRecordConditionResolvedDataList(volumePackageDefinitionId, volumePackageDefinitionItemId);
        }
    }
}