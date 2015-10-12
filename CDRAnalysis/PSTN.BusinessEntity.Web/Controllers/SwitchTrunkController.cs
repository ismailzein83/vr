﻿using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Web.Controllers
{
    public class SwitchTrunkController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredSwitchTrunks(Vanrise.Entities.DataRetrievalInput<SwitchTrunkQuery> input)
        {
            SwitchTrunkManager manager = new SwitchTrunkManager();
            return GetWebResponse(input, manager.GetFilteredSwitchTrunks(input));
        }

        [HttpGet]
        public SwitchTrunkDetail GetSwitchTrunkByID(int trunkID)
        {
            SwitchTrunkManager manager = new SwitchTrunkManager();
            return manager.GetSwitchTrunkByID(trunkID);
        }

        [HttpPost]
        public List<SwitchTrunkInfo> GetTrunksBySwitchIds(TrunkFilter trunkFilterObj)
        {
            SwitchTrunkManager manager = new SwitchTrunkManager();
            return manager.GetTrunksBySwitchIds(trunkFilterObj);
        }

        [HttpGet]
        public List<SwitchTrunkInfo> GetSwitchTrunks()
        {
            SwitchTrunkManager manager = new SwitchTrunkManager();
            return manager.GetSwitchTrunks();
        }

        [HttpPost]
        public InsertOperationOutput<SwitchTrunkDetail> AddSwitchTrunk(SwitchTrunk trunkObject)
        {
            SwitchTrunkManager manager = new SwitchTrunkManager();
            return manager.AddSwitchTrunk(trunkObject);
        }

        [HttpPost]
        public UpdateOperationOutput<SwitchTrunkDetail> UpdateSwitchTrunk(SwitchTrunk trunkObject)
        {
            SwitchTrunkManager manager = new SwitchTrunkManager();
            return manager.UpdateSwitchTrunk(trunkObject);
        }

        [HttpGet]
        public DeleteOperationOutput<object> DeleteSwitchTrunk(int trunkID, int? linkedToTrunkID)
        {
            SwitchTrunkManager manager = new SwitchTrunkManager();
            return manager.DeleteSwitchTrunk(trunkID, linkedToTrunkID);
        }
    }
}