﻿(function (appControllers) {

    'use strict';

    ExecutionFlowAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig'];

    function ExecutionFlowAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig) {
        return ({
            GetFilteredUsers: GetFilteredUsers,
            GetExecutionFlowDefinitions: GetExecutionFlowDefinitions,
            GetUsers: GetUsers,
            GetUserbyId: GetUserbyId,
            GetMembers: GetMembers,
            AddUser: AddUser,
            UpdateUser: UpdateUser,
            CheckUserName: CheckUserName,
            ResetPassword: ResetPassword,
            EditUserProfile: EditUserProfile,
            LoadLoggedInUserProfile: LoadLoggedInUserProfile
        });

        function GetFilteredUsers(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Users', 'GetFilteredUsers'), input);
        }

        function GetExecutionFlowDefinitions(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'ExecutionFlow', 'GetFilteredExecutionFlowDefinitions'), {
                filter: filter
            });
        }

        function GetUsers() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Users', 'GetUsersInfo'));
        }

        function GetUserbyId(userId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Users', 'GetUserbyId'), {
                UserId: userId
            });
        }

        function GetMembers(groupId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Users', 'GetMembers'), {
                groupId: groupId
            });
        }

        function AddUser(user) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Users', 'AddUser'), user);
        }

        function UpdateUser(user) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Users', 'UpdateUser'), user);
        }

        function CheckUserName(name) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Users', 'CheckUserName'), {
                Name: name
            });
        }

        function ResetPassword(resetPasswordInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Users', 'ResetPassword'), resetPasswordInput);
        }

        function EditUserProfile(userProfileObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Users', 'EditUserProfile'), userProfileObject);
        }

        function LoadLoggedInUserProfile() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Users', 'LoadLoggedInUserProfile'));
        }
    }

    appControllers.service('ExecutionFlowAPIService', ExecutionFlowAPIService);

})(appControllers);
