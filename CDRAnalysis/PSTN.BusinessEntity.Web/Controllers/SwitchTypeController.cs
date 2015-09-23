using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Web.Controllers
{
    public class SwitchTypeController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredSwitchTypes(Vanrise.Entities.DataRetrievalInput<SwitchTypeQuery> input)
        {
            SwitchTypeManager manager = new SwitchTypeManager();
            return GetWebResponse(input, manager.GetFilteredSwitchTypes(input));
        }

        [HttpGet]
        public SwitchType GetSwitchTypeByID(int switchTypeID)
        {
            SwitchTypeManager manager = new SwitchTypeManager();
            return manager.GetSwitchTypeByID(switchTypeID);
        }

        [HttpPost]
        public InsertOperationOutput<SwitchType> AddSwitchType(SwitchType switchTypeObject)
        {
            SwitchTypeManager manager = new SwitchTypeManager();
            return manager.AddSwitchType(switchTypeObject);
        }

        [HttpPost]
        public UpdateOperationOutput<SwitchType> UpdateSwitchType(SwitchType switchTypeObject)
        {
            SwitchTypeManager manager = new SwitchTypeManager();
            return manager.UpdateSwitchType(switchTypeObject);
        }

        [HttpGet]
        public DeleteOperationOutput<object> DeleteSwitchType(int switchTypeID)
        {
            SwitchTypeManager manager = new SwitchTypeManager();
            return manager.DeleteSwitchType(switchTypeID);
        }
    }
}