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
            HasResetUserPasswordPermission: HasResetUserPasswordPermission
        });

        function GetFilteredUsers(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetFilteredUsers'), input);
        }

        function GetUsersInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetUsersInfo'), {
                filter: filter
            });
        }

        function GetUsers() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetUsersInfo'));
        }

        function GetUserbyId(userId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetUserbyId'), {
                UserId: userId
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

        function CheckUserName(name) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'CheckUserName'), {
                Name: name
            });
        }

        function ResetPassword(resetPasswordInput) {   
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'ResetPassword'), resetPasswordInput);
        }

        function EditUserProfile(userProfileObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'EditUserProfile'), userProfileObject);
        }

        function LoadLoggedInUserProfile() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'LoadLoggedInUserProfile'));
        }

        function HasAddUserPermission() {
            return SecurityService.IsAllowedBySystemActionNames(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['AddUser']));
        }

        function HasUpdateUserPermission() {
            return SecurityService.IsAllowedBySystemActionNames(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['UpdateUser']));
        }

        function HasResetUserPasswordPermission() {
            return SecurityService.IsAllowedBySystemActionNames(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['ResetPassword']));
        }
    }

    appControllers.service('VR_Sec_UserAPIService', UserAPIService);

})(appControllers);