(function (appControllers) {

    'use strict';

    UserAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig', 'SecurityService'];

    function UserAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig, SecurityService) {
        var controllerName = 'Users';

        return ({
            GetFilteredUsers: GetFilteredUsers,
            GetUsersInfo: GetUsersInfo,
            GetUsers: GetUsers,
            GetUserbyId: GetUserbyId,
            GetMembers: GetMembers,
            AddUser: AddUser,
            UpdateUser: UpdateUser,
            CheckUserName: CheckUserName,
            ResetPassword: ResetPassword,
            EditUserProfile: EditUserProfile,
            LoadLoggedInUserProfile: LoadLoggedInUserProfile,
            HasAddUserPermission: HasAddUserPermission,
            HasUpdateUserPermission: HasUpdateUserPermission,
            HasResetUserPasswordPermission: HasResetUserPasswordPermission,
            ActivatePassword: ActivatePassword,
            ForgotPassword: ForgotPassword,
            DisableUser: DisableUser,
            EnableUser: EnableUser,
            UnlockUser: UnlockUser,
            GetUserHistoryDetailbyHistoryId: GetUserHistoryDetailbyHistoryId,
            GetUserLanguageId: GetUserLanguageId,
            UpdateMyLanguage: UpdateMyLanguage
        });

        function GetFilteredUsers(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetFilteredUsers'), input);
        }



        function GetUsersInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetUsersInfo'), {
                filter: filter
            });
        }
        function GetUserLanguageId() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetUserLanguageId'));
     
        }
        function UpdateMyLanguage(languageInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'UpdateMyLanguage'), languageInput);
        }

        function GetUsers() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetUsersInfo'));
        }

        function GetUserbyId(userId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetUserbyId'), {
                UserId: userId
            });
        }

        function GetUserHistoryDetailbyHistoryId(userHistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetUserHistoryDetailbyHistoryId'), {
                userHistoryId: userHistoryId
            });
        }

        function GetMembers(groupId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetMembers'), {
                groupId: groupId
            });
        }

        function AddUser(user) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'AddUser'), user);
        }

        function UpdateUser(user) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'UpdateUser'), user);
        }

        function DisableUser(user) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'DisableUser'), user);
        }

        function EnableUser(user) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'EnableUser'), user);
        }

        function UnlockUser(user) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'UnlockUser'), user);
        }

        function CheckUserName(name) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'CheckUserName'), {
                Name: name
            });
        }

        function ResetPassword(resetPasswordInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'ResetPassword'), resetPasswordInput);
        }

        function ForgotPassword(forgotPasswordInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'ForgotPassword'), forgotPasswordInput);
        }

        function ActivatePassword(activatePasswordInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'ActivatePassword'), activatePasswordInput);
        }

        function EditUserProfile(userProfileObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'EditUserProfile'), userProfileObject);
        }

        function LoadLoggedInUserProfile() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'LoadLoggedInUserProfile'));
        }

        function HasAddUserPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['AddUser']));
        }

        function HasUpdateUserPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['UpdateUser']));
        }

        function HasResetUserPasswordPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['ResetPassword']));
        }
    }

    appControllers.service('VR_Sec_UserAPIService', UserAPIService);

})(appControllers);