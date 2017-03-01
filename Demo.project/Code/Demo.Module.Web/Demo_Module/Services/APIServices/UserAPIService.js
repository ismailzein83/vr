
(function (appControllers) {

    'use strict';

    UserAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];

    function UserAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = 'User';

        function GetUser(Id) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetUser'),
                { Id: Id }
                );
        }

        function GetFilteredUsers(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetFilteredUsers'), input);
        }

        function AddUser(userObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddUser"), userObject);
        }
        function UpdateUser(userObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateUser"), userObject);
        }
        return ({
            GetFilteredUsers: GetFilteredUsers,
            UpdateUser: UpdateUser,
            AddUser: AddUser,
            GetUser: GetUser
        });
    }


    appControllers.service('Demo_Module_UserAPIService', UserAPIService);
})(appControllers);