
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
        function DeleteUser(Id) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'DeleteUser'), {
                Id: Id
            });
        }
        function GetDemoModuleCityInfos(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetDemoModuleCityInfos"), {
                filter: filter
            });
        };
        return ({
            GetFilteredUsers: GetFilteredUsers,
            UpdateUser: UpdateUser,
            AddUser: AddUser,
            GetUser: GetUser,
            DeleteUser: DeleteUser
        });
    }


    appControllers.service('Demo_Module_UserAPIService', UserAPIService);
})(appControllers);