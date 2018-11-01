using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Security.Web.Controllers;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class PortalAccountManager
    {
        static CarrierProfileManager _carrierProfileManager = new CarrierProfileManager();
        static Guid _carrierPortalConnectionId = new Guid("b50314a1-7b2b-4465-917d-5a8e60bfa09e");

        public List<CarrierProfilePortalAccount> GetCarrierProfilePortalAccounts(int carrierProfileId)
        {
            var portalAccountSettings =  _carrierProfileManager.GetExtendedSettings<PortalAccountSettings>(carrierProfileId);
            if (portalAccountSettings != null)
                return portalAccountSettings.CarrierProfilePortalAccounts;
            return null;
        }

        public Vanrise.Entities.InsertOperationOutput<CarrierProfilePortalAccountDetail> AddPortalAccount(PortalAccountEditorObject portalAccountEditorObject)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CarrierProfilePortalAccountDetail>();
            insertOperationOutput.InsertedObject = null;
            UserManager userManager = new UserManager();
            InsertOperationOutput<UserDetail> userDetails = userManager.AddRemoteUser(_carrierPortalConnectionId, new UserToAdd
            {
                TenantId = 1,
                Email = portalAccountEditorObject.Entity.Email,
                Name = portalAccountEditorObject.Entity.Name,
            });
            insertOperationOutput.Result = userDetails.Result;
            insertOperationOutput.Message = userDetails.Message;
            switch (userDetails.Result)
            {
                case InsertOperationResult.Succeeded:
                    var portalAccountSettings = _carrierProfileManager.GetExtendedSettings<PortalAccountSettings>(portalAccountEditorObject.CarrierProfileId);
                    if (portalAccountSettings == null)
                    {
                        portalAccountSettings = new PortalAccountSettings
                        {
                            CarrierProfilePortalAccounts = new List<CarrierProfilePortalAccount>()
                        };
                    }
                    else if (portalAccountSettings.CarrierProfilePortalAccounts == null)
                    {
                        portalAccountSettings.CarrierProfilePortalAccounts = new List<CarrierProfilePortalAccount>();
                    }
                      
                    portalAccountSettings.CarrierProfilePortalAccounts.Add(new CarrierProfilePortalAccount
                    {
                        CarrierAccounts = portalAccountEditorObject.Entity.CarrierAccounts,
                        Email = portalAccountEditorObject.Entity.Email,
                        Name = portalAccountEditorObject.Entity.Name,
                        TenantId = 1,
                        Type = portalAccountEditorObject.Entity.Type,
                        UserId = userDetails.InsertedObject.Entity.UserId
                    });
                    _carrierProfileManager.UpdateCarrierProfileExtendedSetting<PortalAccountSettings>(portalAccountEditorObject.CarrierProfileId, portalAccountSettings);
                    insertOperationOutput.InsertedObject = new CarrierProfilePortalAccountDetail
                    {
                        Email = portalAccountEditorObject.Entity.Email,
                        Name = portalAccountEditorObject.Entity.Name,
                        TenantId = 1,
                        UserId = userDetails.InsertedObject.Entity.UserId,
                    };
                    break;
            }
            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<CarrierProfilePortalAccountDetail> UpdatePortalAccount(PortalAccountEditorObject portalAccountEditorObject)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CarrierProfilePortalAccountDetail>();
            updateOperationOutput.UpdatedObject = null;
            var connectionSettings = GetCarrierPortalConnectionSettings();
            var portalAccountSettings = _carrierProfileManager.GetExtendedSettings<PortalAccountSettings>(portalAccountEditorObject.CarrierProfileId);
            var carrierProfilePortalAccount = portalAccountSettings.CarrierProfilePortalAccounts.FindRecord(x => x.UserId == portalAccountEditorObject.Entity.UserId);

            if (!CheckUserInfoChange(carrierProfilePortalAccount, portalAccountEditorObject.Entity))
            {
                updateOperationOutput.UpdatedObject = UpdateCarrierProfileExtendedSettings(portalAccountSettings, carrierProfilePortalAccount, portalAccountEditorObject.Entity, portalAccountEditorObject.CarrierProfileId);
                return updateOperationOutput;
            }
            User user = connectionSettings.Get<User>(string.Format("/api/VR_Sec/Users/GetUserbyId?userId={0}", portalAccountEditorObject.Entity.UserId));
            UpdateOperationOutput<UserDetail> userDetails = connectionSettings.Post<UserToUpdate, UpdateOperationOutput<UserDetail>>("/api/VR_Sec/Users/UpdateUser", new UserToUpdate
            {
                UserId = user.UserId,
                Name = portalAccountEditorObject.Entity.Name,
                Email = portalAccountEditorObject.Entity.Email,
                TenantId = user.TenantId,
                Description = user.Description,
                EnabledTill = user.EnabledTill,
                ExtendedSettings = user.ExtendedSettings,
                SecurityProviderId = user.SecurityProviderId
            });

            updateOperationOutput.Result = userDetails.Result;
            updateOperationOutput.Message = userDetails.Message;
            switch (userDetails.Result)
            {
                case UpdateOperationResult.Succeeded:
                   
                    updateOperationOutput.UpdatedObject = UpdateCarrierProfileExtendedSettings(portalAccountSettings, carrierProfilePortalAccount, portalAccountEditorObject.Entity, portalAccountEditorObject.CarrierProfileId);
                    break;
            }
            return updateOperationOutput;
        }

        public CarrierProfilePortalAccount GetPortalAccount(int carrierProfileId, int userId)
        {
            PortalAccountSettings portalAccountSettings = _carrierProfileManager.GetExtendedSettings<PortalAccountSettings>(carrierProfileId);
            if(portalAccountSettings.CarrierProfilePortalAccounts!= null && portalAccountSettings.CarrierProfilePortalAccounts.Count > 0)
            {
                return portalAccountSettings.CarrierProfilePortalAccounts.FindRecord(x => x.UserId == userId);
            }
            return null;
        }

        public Vanrise.Entities.UpdateOperationOutput<object> ResetPassword(PortalAccountResetPasswordInput resetPasswordInput)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();
            updateOperationOutput.UpdatedObject = null;
            var connectionSettings = GetCarrierPortalConnectionSettings();
            UpdateOperationOutput<object> userDetails = connectionSettings.Post<ResetPasswordInput, UpdateOperationOutput<object>>("/api/VR_Sec/Users/ResetPassword", new ResetPasswordInput
            {
                UserId = resetPasswordInput.UserId,
                Password = resetPasswordInput.Password
            });
            updateOperationOutput.Result = userDetails.Result;
            updateOperationOutput.Message = userDetails.Message;
            return updateOperationOutput;
        }

        private CarrierProfilePortalAccountDetail UpdateCarrierProfileExtendedSettings(PortalAccountSettings portalAccountSettings, CarrierProfilePortalAccount oldCarrierProfilePortalAccount, CarrierProfilePortalAccount newCarrierProfilePortalAccount, int carrierProfileId)
        {
            portalAccountSettings.CarrierProfilePortalAccounts.Remove(oldCarrierProfilePortalAccount);
            portalAccountSettings.CarrierProfilePortalAccounts.Add(new CarrierProfilePortalAccount
            {
                CarrierAccounts = newCarrierProfilePortalAccount.CarrierAccounts,
                Email = newCarrierProfilePortalAccount.Email,
                Name = newCarrierProfilePortalAccount.Name,
                TenantId = 1,
                Type = newCarrierProfilePortalAccount.Type,
                UserId = newCarrierProfilePortalAccount.UserId
            });
            _carrierProfileManager.UpdateCarrierProfileExtendedSetting<PortalAccountSettings>(carrierProfileId, portalAccountSettings);
            return new CarrierProfilePortalAccountDetail
            {
                Email = newCarrierProfilePortalAccount.Email,
                Name = newCarrierProfilePortalAccount.Name,
                TenantId = 1,
                UserId = newCarrierProfilePortalAccount.UserId
            };
        }
        private bool CheckUserInfoChange(CarrierProfilePortalAccount oldPortalAccount, CarrierProfilePortalAccount newPortalAccount)
        {
            if (oldPortalAccount.Email != newPortalAccount.Email || oldPortalAccount.Name != newPortalAccount.Name)
                return true;
            return false;
        }
        private VRInterAppRestConnection GetCarrierPortalConnectionSettings()
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection> (_carrierPortalConnectionId);
            return vrConnection.Settings as VRInterAppRestConnection;
        }
    }
}
