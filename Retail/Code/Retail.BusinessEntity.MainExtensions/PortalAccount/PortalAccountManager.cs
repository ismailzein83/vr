using PartnerPortal.CustomerAccess.Entities;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Retail.BusinessEntity.MainExtensions.PortalAccount
{
    public class PortalAccountManager
    {
        static SecurityManager sManager = new SecurityManager();
      
        public PortalAccountSettings GetPortalAccountSettings(Guid accountBEDefinitionId, long accountId, Guid accountViewDefinitionId)
        {
            return new AccountBEManager().GetExtendedSettings<PortalAccountSettings>(accountBEDefinitionId, accountId);
        }
        public PortalAccountDetail GetPortalAccount(Guid accountBEDefinitionId, long accountId, Guid accountViewDefinitionId, int userId)
        {
            var connectionSettings = GetCarrierPortalConnectionSettings(accountBEDefinitionId, accountViewDefinitionId);

            UserStatus userStatus = connectionSettings.Get<UserStatus>(string.Format("/api/PartnerPortal_CustomerAccess/RetailAccountUser/GetUserStatusByUserId?userId={0}", userId));
            var portalAccountSettings = GetPortalAccountSettings(accountBEDefinitionId, accountId, accountViewDefinitionId);
            if (portalAccountSettings.UserId == userId)
                return PortalAccountDetailMapper(userId, portalAccountSettings.Name, portalAccountSettings.Email, portalAccountSettings.TenantId, userStatus);
            if (portalAccountSettings.AdditionalUsers == null)
                portalAccountSettings.AdditionalUsers.ThrowIfNull("Additional Users");
            var additionalPortalSettings = portalAccountSettings.AdditionalUsers.FindRecord(x => x.UserId == userId);
            additionalPortalSettings.ThrowIfNull("additionalPortalSettings");
            return PortalAccountDetailMapper(userId, additionalPortalSettings.Name, additionalPortalSettings.Email, additionalPortalSettings.TenantId, userStatus);
        }

        public List<PortalAccountDetail> GetPortalAccountDetails(Guid accountBEDefinitionId, long accountId, Guid accountViewDefinitionId)
        {
            UserStatusInput userStatusInput = new UserStatusInput();
            List<PortalAccountDetail> portalAccountDetails = new List<PortalAccountDetail>();
            var portalAccountSettings = GetPortalAccountSettings(accountBEDefinitionId, accountId, accountViewDefinitionId);


            if (portalAccountSettings != null)
            {
                if (userStatusInput.UserIds == null)
                    userStatusInput.UserIds = new List<int>();

                userStatusInput.UserIds.Add(portalAccountSettings.UserId);

                if (portalAccountSettings.AdditionalUsers != null)
                {
                    foreach (var additionalAccount in portalAccountSettings.AdditionalUsers)
                    {
                        userStatusInput.UserIds.Add(additionalAccount.UserId);
                    }
                }
                var connectionSettings = GetCarrierPortalConnectionSettings(accountBEDefinitionId, accountViewDefinitionId);

                List<UserDetailInfo> userDetailInfo = connectionSettings.Post<UserStatusInput, List<UserDetailInfo>>("/api/PartnerPortal_CustomerAccess/RetailAccountUser/GetUsersStatus", userStatusInput);
                var primaryUserDetailInfo = userDetailInfo.Find(x => x.UserId == portalAccountSettings.UserId);
                if (primaryUserDetailInfo == null)
                    primaryUserDetailInfo.ThrowIfNull("PrimaryUserInfo");
                var portalAccountDetail = PortalAccountDetailMapper(portalAccountSettings.UserId, portalAccountSettings.Name, portalAccountSettings.Email, portalAccountSettings.TenantId, primaryUserDetailInfo.UserStatus);
                portalAccountDetails.Add(portalAccountDetail);
                if (portalAccountSettings.AdditionalUsers != null)
                {
                    foreach (var poratlAccountSetting in portalAccountSettings.AdditionalUsers)
                    {
                        var additionalUserDetailInfo = userDetailInfo.Find(x => x.UserId == poratlAccountSetting.UserId);
                        if (additionalUserDetailInfo == null)
                            additionalUserDetailInfo.ThrowIfNull("Additional UserInfo");
                        var additionalPortalAccountDetail = PortalAccountDetailMapper(poratlAccountSetting.UserId, poratlAccountSetting.Name, poratlAccountSetting.Email, poratlAccountSetting.TenantId, additionalUserDetailInfo.UserStatus);

                        portalAccountDetails.Add(additionalPortalAccountDetail);
                    }
                }
            }
            return portalAccountDetails;
        }
        public UpdateOperationOutput<PortalAccountDetail> EnbalePortalAccount(Guid accountBEDefinitionId, Guid accountViewDefinitionId, long accountId, int userId)
        {
            UpdateOperationOutput<PortalAccountDetail> updateOperationOutput = new UpdateOperationOutput<PortalAccountDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            var connectionSettings = GetCarrierPortalConnectionSettings(accountBEDefinitionId, accountViewDefinitionId);

            UpdateOperationOutput<UserDetail> userDetail = connectionSettings.Get<UpdateOperationOutput<UserDetail>>(string.Format("/api/PartnerPortal_CustomerAccess/RetailAccountUser/EnableUser?userId={0}", userId));
            switch (userDetail.Result)
            {
                case UpdateOperationResult.Succeeded:

                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                    var portalAccountUpdated = GetPortalAccount(accountBEDefinitionId, accountId, accountViewDefinitionId, userId);
                    updateOperationOutput.UpdatedObject = PortalAccountDetailMapper(portalAccountUpdated.UserId, portalAccountUpdated.Name, portalAccountUpdated.Email, portalAccountUpdated.TenantId, userDetail.UpdatedObject.Status);
                    break;

                case UpdateOperationResult.Failed:
                case UpdateOperationResult.SameExists:

                default: break;
            }
            return updateOperationOutput;
        }
       
        public UpdateOperationOutput<PortalAccountDetail> UnlockPortalAccount(Guid accountBEDefinitionId, Guid accountViewDefinitionId, long accountId, int userId)
        {
            UpdateOperationOutput<PortalAccountDetail> updateOperationOutput = new UpdateOperationOutput<PortalAccountDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            var connectionSettings = GetCarrierPortalConnectionSettings(accountBEDefinitionId, accountViewDefinitionId);

            UpdateOperationOutput<UserDetail> userDetail = connectionSettings.Get<UpdateOperationOutput<UserDetail>>(string.Format("/api/PartnerPortal_CustomerAccess/RetailAccountUser/UnlockPortalAccount?userId={0}", userId));
            switch (userDetail.Result)
            {
                case UpdateOperationResult.Succeeded:

                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                    var portalAccountUpdated = GetPortalAccount(accountBEDefinitionId, accountId, accountViewDefinitionId, userId);
                    updateOperationOutput.UpdatedObject = PortalAccountDetailMapper(portalAccountUpdated.UserId, portalAccountUpdated.Name, portalAccountUpdated.Email, portalAccountUpdated.TenantId, userDetail.UpdatedObject.Status);
                    break;

                case UpdateOperationResult.Failed:
                case UpdateOperationResult.SameExists:

                default: break;
            }
            return updateOperationOutput;
        }
        public UpdateOperationOutput<PortalAccountDetail> DisablePortalAccount(Guid accountBEDefinitionId, Guid accountViewDefinitionId, long accountId, int userId)
        {
            UpdateOperationOutput<PortalAccountDetail> updateOperationOutput = new UpdateOperationOutput<PortalAccountDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            var connectionSettings = GetCarrierPortalConnectionSettings(accountBEDefinitionId, accountViewDefinitionId);
            string actionPath = string.Format("/api/PartnerPortal_CustomerAccess/RetailAccountUser/DisableUser?userId={0}", userId);
            UpdateOperationOutput<UserDetail> userDetail = connectionSettings.Get<UpdateOperationOutput<UserDetail>>(actionPath);
            switch (userDetail.Result)
            {
                case UpdateOperationResult.Succeeded:

                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                    var portalAccountUpdated = GetPortalAccount(accountBEDefinitionId, accountId, accountViewDefinitionId, userId);
                    updateOperationOutput.UpdatedObject = PortalAccountDetailMapper(portalAccountUpdated.UserId, portalAccountUpdated.Name, portalAccountUpdated.Email, portalAccountUpdated.TenantId, userDetail.UpdatedObject.Status);
                    break;

                case UpdateOperationResult.Failed:
                case UpdateOperationResult.SameExists:

                default: break;
            }
            return updateOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<PortalAccountDetail> UpdatePortalAccount(Guid accountBEDefinitionId, long accountId, Guid accountViewDefinitionId,int userId, string name, string email)
        {
            PortalAccountDetail portalAccountDetail = new PortalAccountDetail();
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<PortalAccountDetail>();
            updateOperationOutput.UpdatedObject = null;
            var portalAccountSettings = GetPortalAccountSettings(accountBEDefinitionId, accountId, accountViewDefinitionId);
            var userEmailExists = DoesUserEmailExists(portalAccountSettings, email, userId);
            if (userEmailExists)
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
                return updateOperationOutput;
            }

            var retailAccountToUpdate = new PartnerPortal.CustomerAccess.Entities.RetailAccountToUpdate()
            {
                UserId = userId,
                Name = name,
                Email = email,
            };

            var connectionSettings = GetCarrierPortalConnectionSettings(accountBEDefinitionId, accountViewDefinitionId);

            UpdateOperationOutput<UserDetail> userDetails =
                connectionSettings.Post<PartnerPortal.CustomerAccess.Entities.RetailAccountToUpdate, UpdateOperationOutput<UserDetail>>("/api/PartnerPortal_CustomerAccess/RetailAccountUser/UpdateRetailAccountUser", retailAccountToUpdate);

            updateOperationOutput.Result = userDetails.Result;
            updateOperationOutput.Message = userDetails.Message;

            switch (userDetails.Result)
            {
                case UpdateOperationResult.Succeeded:
                    if (userId == portalAccountSettings.UserId)
                    {
                        portalAccountSettings.Name = name;
                        portalAccountSettings.Email = email;
                        portalAccountDetail = PortalAccountDetailMapper(portalAccountSettings.UserId, portalAccountSettings.Name, portalAccountSettings.Email, portalAccountSettings.TenantId, userDetails.UpdatedObject.Status);
                    }
                    else
                    {
                        foreach (var portalAccountSetting in portalAccountSettings.AdditionalUsers)
                        {
                            if(portalAccountSetting.UserId == userId)
                            {
                                portalAccountSetting.Name = name;
                                portalAccountSetting.Email = email;
                                portalAccountDetail = PortalAccountDetailMapper(portalAccountSetting.UserId, portalAccountSetting.Name, portalAccountSetting.Email, portalAccountSetting.TenantId, userDetails.UpdatedObject.Status);
                            }
                        }
                    }
                    AccountBEManager accountBEManager = new AccountBEManager();

                    bool IsAccountExtendedSettingsUpdated = accountBEManager.UpdateAccountExtendedSetting(accountBEDefinitionId, accountId, portalAccountSettings);
                    if (IsAccountExtendedSettingsUpdated)
                    {
                        accountBEManager.TrackAndLogObjectCustomAction(accountBEDefinitionId, accountId, "Configure Portal Account", string.Format("Email: {0}", portalAccountSettings.Email), portalAccountSettings);
                        updateOperationOutput.UpdatedObject = portalAccountDetail;
                    }
                    else
                    {
                        updateOperationOutput.Result = UpdateOperationResult.Failed;
                        updateOperationOutput.Message = "Update Retail Account has failed";
                    }
                    break;


                case UpdateOperationResult.Failed:
                case UpdateOperationResult.SameExists:
                default: break;

            }

            return updateOperationOutput;
        }
        public Vanrise.Entities.InsertOperationOutput<PortalAccountDetail> AddPortalAccount(Guid accountBEDefinitionId, long accountId, Guid accountViewDefinitionId, string name, string email)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<PortalAccountDetail>();
            insertOperationOutput.InsertedObject = null;
                  var portalAccountSettings = GetPortalAccountSettings(accountBEDefinitionId, accountId, accountViewDefinitionId);
                  var userEmailExists = DoesUserEmailExists(portalAccountSettings, email,null);
            if(userEmailExists)
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
                return insertOperationOutput;
            }

            PortalAccount portalAccount = new AccountBEDefinitionManager().GetAccountViewDefinitionSettings<PortalAccount>(accountBEDefinitionId, accountViewDefinitionId);

            var retailAccount = new PartnerPortal.CustomerAccess.Entities.RetailAccount()
            {
                AccountId = accountId,
                Name = name,
                Email = email,
                TenantId = portalAccount.TenantId,
                AccountBEDefinitionId = accountBEDefinitionId
            };

            var connectionSettings = GetCarrierPortalConnectionSettings(accountBEDefinitionId, accountViewDefinitionId);

            InsertOperationOutput<UserDetail> userDetails =
                connectionSettings.Post<PartnerPortal.CustomerAccess.Entities.RetailAccount, InsertOperationOutput<UserDetail>>("/api/PartnerPortal_CustomerAccess/RetailAccountUser/AddRetailAccountUser", retailAccount);

            insertOperationOutput.Result = userDetails.Result;
            insertOperationOutput.Message = userDetails.Message;

            switch (userDetails.Result)
            {
                case InsertOperationResult.Succeeded:
                    var additionalPortalAccountSettings = new AdditionalPortalAccountSettings
                    {
                        UserId = userDetails.InsertedObject.Entity.UserId,
                        Name = userDetails.InsertedObject.Entity.Name,
                        Email = userDetails.InsertedObject.Entity.Email,
                        TenantId = userDetails.InsertedObject.Entity.TenantId
                    };

              
                    AccountBEManager accountBEManager = new AccountBEManager();
                    if (portalAccountSettings != null)
                    {
                        if (portalAccountSettings.AdditionalUsers == null)
                            portalAccountSettings.AdditionalUsers = new List<AdditionalPortalAccountSettings>();

                        portalAccountSettings.AdditionalUsers.Add(additionalPortalAccountSettings);

                        bool IsAccountExtendedSettingsUpdated = accountBEManager.UpdateAccountExtendedSetting(accountBEDefinitionId, accountId, portalAccountSettings);
                        if (IsAccountExtendedSettingsUpdated)
                        {
                            var portalAccountDetail = PortalAccountDetailMapper(additionalPortalAccountSettings.UserId, additionalPortalAccountSettings.Name, additionalPortalAccountSettings.Email, additionalPortalAccountSettings.TenantId,userDetails.InsertedObject.Status);
                            accountBEManager.TrackAndLogObjectCustomAction(accountBEDefinitionId, accountId, "Configure Portal Account", string.Format("Email: {0}", portalAccountSettings.Email), portalAccountSettings);
                            insertOperationOutput.InsertedObject = portalAccountDetail;
                        }
                        else
                        {
                            insertOperationOutput.Result = InsertOperationResult.Failed;
                            insertOperationOutput.Message = "Update Retail Account has failed";
                        }
                        break;
                    }
                    else
                    {
                        PortalAccountSettings primaryPortalAccountSettings = new PortalAccountSettings()
                        {
                            UserId = userDetails.InsertedObject.Entity.UserId,
                            Name = userDetails.InsertedObject.Entity.Name,
                            Email = userDetails.InsertedObject.Entity.Email,
                            TenantId = userDetails.InsertedObject.Entity.TenantId
                        };
                        bool IsAccountExtendedSettingsUpdated = accountBEManager.UpdateAccountExtendedSetting(accountBEDefinitionId, accountId, primaryPortalAccountSettings);
                        if (IsAccountExtendedSettingsUpdated)
                        {
                            var portalAccountDetail = PortalAccountDetailMapper(primaryPortalAccountSettings.UserId, primaryPortalAccountSettings.Name, primaryPortalAccountSettings.Email, primaryPortalAccountSettings.TenantId, userDetails.InsertedObject.Status);
                            accountBEManager.TrackAndLogObjectCustomAction(accountBEDefinitionId, accountId, "Configure Portal Account", string.Format("Email: {0}", primaryPortalAccountSettings.Email), primaryPortalAccountSettings);
                            insertOperationOutput.InsertedObject = portalAccountDetail;
                        }
                        else
                        {
                            insertOperationOutput.Result = InsertOperationResult.Failed;
                            insertOperationOutput.Message = "Update Retail Account has failed";
                        }
                        break;
                    }
                case InsertOperationResult.Failed:
                case InsertOperationResult.SameExists:
                default: break;

            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<object> ResetPassword(Guid accountBEDefinitionId, long accountId, Guid accountViewDefinitionId, string password,int userId)
        {
            var portalAccountSettings = GetPortalAccountSettings(accountBEDefinitionId, accountId, accountViewDefinitionId);
            var resetPasswordInput = new Vanrise.Security.Entities.ResetPasswordInput()
            {
                UserId = userId,
                Password = password
            };
            if (userId == portalAccountSettings.UserId || (portalAccountSettings.AdditionalUsers != null && portalAccountSettings.AdditionalUsers.Any(item => item.UserId == userId)))
            {
                var connectionSettings = GetCarrierPortalConnectionSettings(accountBEDefinitionId, accountViewDefinitionId);
                return connectionSettings.Post<Vanrise.Security.Entities.ResetPasswordInput, Vanrise.Entities.UpdateOperationOutput<object>>("/api/PartnerPortal_CustomerAccess/RetailAccountUser/ResetPassword", resetPasswordInput);
            }

            throw new Exception();
        }

        #region Mappers 
        public PortalAccountDetail PortalAccountDetailMapper(int userId, string name, string email, int tenantId,UserStatus userStatus)
        {
            return new PortalAccountDetail()
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

        #region Security
        public bool DosesUserHaveViewAccess( Guid accountBEDefinitionId,Guid accountViewDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAccess(userId, accountBEDefinitionId, accountViewDefinitionId, (sec) => sec.ViewRequiredPermission);

        }
        public bool DosesUserHaveConfigureAccess(Guid accountBEDefinitionId, Guid accountViewDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAccess(userId, accountBEDefinitionId, accountViewDefinitionId, (sec) => sec.ConfigureRequiredPermission);

        }
        public bool DosesUserHaveResetPasswordAccess(Guid accountBEDefinitionId, Guid accountViewDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAccess(userId, accountBEDefinitionId, accountViewDefinitionId, (sec) => sec.ResetPasswordRequiredPermission);

        }

        #endregion

        #region Private Methods
        private bool DoesUserHaveAccess(int userId, Guid accountBEDefinitionId, Guid accountViewDefinitionId, Func<AccountViewDefinitionSecurity, Vanrise.Security.Entities.RequiredPermissionSettings> getRequiredPermissionSetting)
        {
            var accountViewDefinitionSettings = new AccountBEDefinitionManager().GetAccountViewDefinitionSettings<PortalAccount>(accountBEDefinitionId, accountViewDefinitionId);
            if (accountViewDefinitionSettings != null && accountViewDefinitionSettings.Security != null && getRequiredPermissionSetting(accountViewDefinitionSettings.Security) != null)
                return sManager.IsAllowed(getRequiredPermissionSetting(accountViewDefinitionSettings.Security), userId);
            else
                return true;
        }
        private VRInterAppRestConnection GetCarrierPortalConnectionSettings(Guid accountBEDefinitionId, Guid accountViewDefinitionId)
        {
            PortalAccount portalAccount = new AccountBEDefinitionManager().GetAccountViewDefinitionSettings<PortalAccount>(accountBEDefinitionId, accountViewDefinitionId);
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(portalAccount.ConnectionId);
            return vrConnection.Settings as VRInterAppRestConnection;
        }
        private bool DoesUserEmailExists (PortalAccountSettings portalAccountSettings,string email,int? userId)
        {
            if (portalAccountSettings == null)
                return false;
            if (portalAccountSettings.Email == null)
                return false;
            if (portalAccountSettings.Email == email && portalAccountSettings.UserId != userId)
                return true;
            if (portalAccountSettings.AdditionalUsers == null)
                return false;
            foreach(var portalSetting in portalAccountSettings.AdditionalUsers )
            {
                if (portalSetting.Email == email && portalSetting.UserId != userId)
                    return true;
            }
            return false;
        }

        #endregion
    }
}
