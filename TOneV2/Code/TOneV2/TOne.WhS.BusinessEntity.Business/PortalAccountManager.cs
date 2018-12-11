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

        #region Public Methods
        public List<CarrierProfilePortalAccountDetail> GetCarrierProfilePortalAccounts(int carrierProfileId)
        {
            UsersStatusesInput userStatusInput = new UsersStatusesInput();
            List<CarrierProfilePortalAccountDetail> portalAccountDetails = new List<CarrierProfilePortalAccountDetail>();
            var portalAccountSettings =  _carrierProfileManager.GetExtendedSettings<PortalAccountSettings>(carrierProfileId);
            if (portalAccountSettings != null && portalAccountSettings.CarrierProfilePortalAccounts!=null && portalAccountSettings.CarrierProfilePortalAccounts.Count>0)
            {
                if (userStatusInput.UserIds == null)
                    userStatusInput.UserIds = new List<int>();
                foreach(var portalAccount in portalAccountSettings.CarrierProfilePortalAccounts)
                {
                    userStatusInput.UserIds.Add(portalAccount.UserId);
                }
                var connectionSettings = GetCarrierPortalConnectionSettings();
                List<UserInfo> userDetailInfo = connectionSettings.Post<UsersStatusesInput, List<UserInfo>>("/api/VR_Sec/Users/GetUsersStatus", userStatusInput);
                foreach(var userDetail in userDetailInfo)
                {
                    var portalAccount = portalAccountSettings.CarrierProfilePortalAccounts.FindRecord(x => x.UserId == userDetail.UserId);
                    if(portalAccount.UserStatus!=userDetail.Status)
                        UpdateCarrierProfileExtendedSettings(portalAccountSettings, portalAccount, portalAccount, carrierProfileId, userDetail.Status);
                    var accountDetail = CarrierProfilePortalAccountDetailMapper(portalAccount.UserId, portalAccount.Name, portalAccount.Email, portalAccount.TenantId, userDetail.Status);
                    portalAccountDetails.Add(accountDetail);
                }
            }
            return portalAccountDetails;
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
                GroupIds=portalAccountEditorObject.GroupIds
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
                        UserId = userDetails.InsertedObject.Entity.UserId,
                        UserStatus = UserStatus.Active
                    });
                    _carrierProfileManager.UpdateCarrierProfileExtendedSetting<PortalAccountSettings>(portalAccountEditorObject.CarrierProfileId, portalAccountSettings);
                    insertOperationOutput.InsertedObject = CarrierProfilePortalAccountDetailMapper(userDetails.InsertedObject.Entity.UserId, portalAccountEditorObject.Entity.Name, portalAccountEditorObject.Entity.Email, 1, UserStatus.Active);
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

            if (!CheckUserInfoChange(carrierProfilePortalAccount, portalAccountEditorObject))
            {
                UpdateCarrierProfileExtendedSettings(portalAccountSettings, carrierProfilePortalAccount, portalAccountEditorObject.Entity, portalAccountEditorObject.CarrierProfileId, portalAccountEditorObject.Entity.UserStatus);
                updateOperationOutput.UpdatedObject = CarrierProfilePortalAccountDetailMapper(portalAccountEditorObject.Entity.UserId, portalAccountEditorObject.Entity.Name, portalAccountEditorObject.Entity.Email, 1, portalAccountEditorObject.Entity.UserStatus);
                return updateOperationOutput;
            }
            User user = connectionSettings.Get<User>(string.Format("/api/VR_Sec/Users/GetUserbyId?userId={0}", portalAccountEditorObject.Entity.UserId));
            UpdateOperationOutput<UserDetail> userDetails = connectionSettings.Post<UserToUpdate, UpdateOperationOutput<UserDetail>>("/api/VR_Sec/Users/UpdateUser", new UserToUpdate
            {
                UserId = user.UserId,
                Name = portalAccountEditorObject.Entity.Name,
                Email = portalAccountEditorObject.Entity.Email,
                GroupIds=portalAccountEditorObject.GroupIds,
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
                    UpdateCarrierProfileExtendedSettings(portalAccountSettings, carrierProfilePortalAccount, portalAccountEditorObject.Entity, portalAccountEditorObject.CarrierProfileId, portalAccountEditorObject.Entity.UserStatus);
                    var portalAccount = GetPortalAccount(portalAccountEditorObject.CarrierProfileId, portalAccountEditorObject.Entity.UserId);
                    updateOperationOutput.UpdatedObject = CarrierProfilePortalAccountDetailMapper(portalAccount.UserId, portalAccount.Name, portalAccount.Email, 1, portalAccount.UserStatus);
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
        public CarrierProfilePortalAccountEditorRuntime GetPortalAccountEditorRuntime(int carrierProfileId, int userId)
        {
            return new CarrierProfilePortalAccountEditorRuntime
            {
                CarrierProfilePortalAccount = GetPortalAccount(carrierProfileId, userId),
                GroupIds = new RestGroupManager().GetRemoteAssignedUserGroups(_carrierPortalConnectionId, userId)
            };
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
        public UpdateOperationOutput<CarrierProfilePortalAccountDetail> DisablePortalAccount(int carrierProfileId, int userId)
        {
            UpdateOperationOutput<CarrierProfilePortalAccountDetail> updateOperationOutput = new UpdateOperationOutput<CarrierProfilePortalAccountDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            var connectionSettings = GetCarrierPortalConnectionSettings();
            var portalAccountSettings = _carrierProfileManager.GetExtendedSettings<PortalAccountSettings>(carrierProfileId);
            UpdateOperationOutput<UserDetail> userDetail = connectionSettings.Post<UserDisableInput, UpdateOperationOutput<UserDetail>>("/api/VR_Sec/Users/DisableUser", new UserDisableInput()
            {
                UserId = userId
            });
            if (userDetail.Result == UpdateOperationResult.Succeeded)
            {
                updateOperationOutput.Result = userDetail.Result;
                var portalAccount = GetPortalAccount(carrierProfileId, userId);
                UpdateCarrierProfileExtendedSettings(portalAccountSettings, portalAccount, portalAccount, carrierProfileId, userDetail.UpdatedObject.Status);
                updateOperationOutput.UpdatedObject = CarrierProfilePortalAccountDetailMapper(portalAccount.UserId, portalAccount.Name, portalAccount.Email, portalAccount.TenantId, userDetail.UpdatedObject.Status);
            }
            return updateOperationOutput;
        }
        public UpdateOperationOutput<CarrierProfilePortalAccountDetail> UnlockPortalAccount(int carrierProfileId, int userId)
        {
            UpdateOperationOutput<CarrierProfilePortalAccountDetail> updateOperationOutput = new UpdateOperationOutput<CarrierProfilePortalAccountDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            var connectionSettings = GetCarrierPortalConnectionSettings();
            var portalAccountSettings = _carrierProfileManager.GetExtendedSettings<PortalAccountSettings>(carrierProfileId);
            UserManager userManager = new UserManager();
            UpdateOperationOutput<UserDetail> userDetail = connectionSettings.Get<UpdateOperationOutput<UserDetail>>(string.Format("/api/VR_Sec/Users/UnlockRemoteUser?userId={0}", userId));
            if (userDetail.Result == UpdateOperationResult.Succeeded)
            {
                updateOperationOutput.Result = userDetail.Result;
                var portalAccountUpdated = GetPortalAccount(carrierProfileId, userId);
                UpdateCarrierProfileExtendedSettings(portalAccountSettings, portalAccountUpdated, portalAccountUpdated, carrierProfileId, userDetail.UpdatedObject.Status);
                updateOperationOutput.UpdatedObject = CarrierProfilePortalAccountDetailMapper(portalAccountUpdated.UserId, portalAccountUpdated.Name, portalAccountUpdated.Email, portalAccountUpdated.TenantId, userDetail.UpdatedObject.Status);
            }
            return updateOperationOutput;
        }
        public UpdateOperationOutput<CarrierProfilePortalAccountDetail> EnablePortalAccount(int carrierProfileId, int userId)
        {
            UpdateOperationOutput<CarrierProfilePortalAccountDetail> updateOperationOutput = new UpdateOperationOutput<CarrierProfilePortalAccountDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            var connectionSettings = GetCarrierPortalConnectionSettings();
            var portalAccountSettings = _carrierProfileManager.GetExtendedSettings<PortalAccountSettings>(carrierProfileId);
            UpdateOperationOutput<UserDetail> userDetail = connectionSettings.Post<UserEnableInput, UpdateOperationOutput<UserDetail>>("/api/VR_Sec/Users/EnableUser", new UserEnableInput()
            {
                UserId = userId
            });
            if (userDetail.Result == UpdateOperationResult.Succeeded)
            {
                updateOperationOutput.Result = userDetail.Result;
                var portalAccountUpdated = GetPortalAccount(carrierProfileId, userId);
                UpdateCarrierProfileExtendedSettings(portalAccountSettings, portalAccountUpdated, portalAccountUpdated, carrierProfileId, userDetail.UpdatedObject.Status);
                updateOperationOutput.UpdatedObject = CarrierProfilePortalAccountDetailMapper(portalAccountUpdated.UserId, portalAccountUpdated.Name, portalAccountUpdated.Email,portalAccountUpdated.TenantId, userDetail.UpdatedObject.Status);
            }
            return updateOperationOutput;
        }

     
        #endregion
        #region Private Methods
        private void UpdateCarrierProfileExtendedSettings(PortalAccountSettings portalAccountSettings, CarrierProfilePortalAccount oldCarrierProfilePortalAccount, CarrierProfilePortalAccount newCarrierProfilePortalAccount, int carrierProfileId, UserStatus userStatus)
        {
            portalAccountSettings.CarrierProfilePortalAccounts.Remove(oldCarrierProfilePortalAccount);
            portalAccountSettings.CarrierProfilePortalAccounts.Add(new CarrierProfilePortalAccount
            {
                CarrierAccounts = newCarrierProfilePortalAccount.CarrierAccounts,
                Email = newCarrierProfilePortalAccount.Email,
                Name = newCarrierProfilePortalAccount.Name,
                TenantId = 1,
                Type = newCarrierProfilePortalAccount.Type,
                UserId = newCarrierProfilePortalAccount.UserId,
                UserStatus = userStatus
            });
            _carrierProfileManager.UpdateCarrierProfileExtendedSetting<PortalAccountSettings>(carrierProfileId, portalAccountSettings);
        }
        private bool CheckUserInfoChange(CarrierProfilePortalAccount oldPortalAccount, PortalAccountEditorObject newPortalAccountObject)
        {
          
            if (oldPortalAccount.Email != newPortalAccountObject.Entity.Email || oldPortalAccount.Name != newPortalAccountObject.Entity.Name )
                return true;

            var oldPortalAccountGroupIds = new RestGroupManager().GetRemoteAssignedUserGroups(_carrierPortalConnectionId, oldPortalAccount.UserId);

            int oldCount = 0;
            if(oldPortalAccountGroupIds != null)
                oldCount = oldPortalAccountGroupIds.Count;

            int newCount = 0;
            if (newPortalAccountObject != null && newPortalAccountObject.GroupIds != null)
                newCount = newPortalAccountObject.GroupIds.Count;

            if (oldCount != newCount)
                return true;

            if(oldCount > 0 && newCount > 0  && oldPortalAccountGroupIds.Any(x => !newPortalAccountObject.GroupIds.Any(y => y == x)))
                return true;
           
            return false;
        }
        private VRInterAppRestConnection GetCarrierPortalConnectionSettings()
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection> (_carrierPortalConnectionId);
            return vrConnection.Settings as VRInterAppRestConnection;
        }
        #endregion
        #region Mappers 
        public CarrierProfilePortalAccountDetail CarrierProfilePortalAccountDetailMapper(int userId, string name, string email,int tenantId, UserStatus userStatus)
        {
            return new CarrierProfilePortalAccountDetail()
            {
                UserId = userId,
                Name = name,
                Email = email,
                TenantId = tenantId,
                UserStatus = userStatus,
                UserStatusDescription = Utilities.GetEnumDescription(userStatus)
            };
        }
        #endregion
    }
}
