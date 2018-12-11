﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;


namespace TOne.WhS.BusinessEntity.Web
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "PortalAccount")]
    public class PortalAccountController : BaseAPIController
    {
        PortalAccountManager _manager = new PortalAccountManager();

        [HttpGet]
        [Route("GetCarrierProfilePortalAccounts")]
        public List<CarrierProfilePortalAccountDetail> GetCarrierProfilePortalAccounts(int carrierProfileId)
        {
            return _manager.GetCarrierProfilePortalAccounts(carrierProfileId);
        }

        [HttpPost]
        [Route("AddPortalAccount")]
        public object AddPortalAccount(PortalAccountEditorObject portalAccountEditorObject)
        {
            return _manager.AddPortalAccount(portalAccountEditorObject);
        }

        [HttpPost]
        [Route("UpdatePortalAccount")]
        public object UpdatePortalAccount(PortalAccountEditorObject portalAccountEditorObject)
        {
            return _manager.UpdatePortalAccount(portalAccountEditorObject);
        }

        [HttpGet]
        [Route("GetPortalAccount")]
        public CarrierProfilePortalAccount GetPortalAccount(int carrierProfileId, int userId)
        {
            return _manager.GetPortalAccount(carrierProfileId, userId);
        }

        [HttpGet]
        [Route("GetPortalAccountEditorRuntime")]
        public CarrierProfilePortalAccountEditorRuntime GetPortalAccountEditorRuntime(int carrierProfileId, int userId)
        {
            return _manager.GetPortalAccountEditorRuntime(carrierProfileId, userId);
        }

        [HttpPost]
        [Route("ResetPassword")]
        public object ResetPassword(PortalAccountResetPasswordInput resetPasswordInput)
        {
            return _manager.ResetPassword(resetPasswordInput);
        }

        [HttpGet]
        [Route("DisablePortalAccount")]
        public object DisablePortalAccount(int carrierProfileId, int userId)
        {
            return _manager.DisablePortalAccount(carrierProfileId, userId);
        }

        [HttpGet]
        [Route("UnlockPortalAccount")]
        public object UnlockPortalAccount(int carrierProfileId, int userId)
        {
            return _manager.UnlockPortalAccount(carrierProfileId, userId);
        }

        [HttpGet]
        [Route("EnablePortalAccount")]
        public object EnablePortalAccount(int carrierProfileId, int userId)
        {
            return _manager.EnablePortalAccount(carrierProfileId, userId);
        }

    }
}