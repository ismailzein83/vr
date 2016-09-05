using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VolumeCommitment")]
    public class VolumeCommitmentController:BaseAPIController
    {
        VolumeCommitmentManager _manager = new VolumeCommitmentManager();

        [HttpPost]
        [Route("GetFilteredVolumeCommitments")]
        public object GetFilteredVolumeCommitments(Vanrise.Entities.DataRetrievalInput<VolumeCommitmentQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredVolumeCommitments(input));
        }

        [HttpPost]
        [Route("AddVolumeCommitment")]
        public Vanrise.Entities.InsertOperationOutput<VolumeCommitmentDetail> AddVolumeCommitment(VolumeCommitment volumeCommitment)
        {
            return _manager.AddVolumeCommitment(volumeCommitment);
        }

        [HttpPost]
        [Route("UpdateVolumeCommitment")]
        public Vanrise.Entities.UpdateOperationOutput<VolumeCommitmentDetail> UpdateVolumeCommitment(VolumeCommitment volumeCommitment)
        {
            return _manager.UpdateVolumeCommitment(volumeCommitment);
        }
        [HttpGet]
        [Route("GetVolumeCommitment")]
        public VolumeCommitment GetVolumeCommitment(int volumeCommitmentId)
        {
            return _manager.GetVolumeCommitment(volumeCommitmentId);
        }
    }
}