(function (appControllers) {
    'use strict';

    manufactoryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig'];

    function manufactoryAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig) {

        var moduleName = Demo_Module_ModuleConfig.moduleName;
        var controller = 'Demo_Manufactory';

        function GetFilteredManufactories(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(moduleName, controller, "GetFilteredManufactories"), input);
        }

        function GetManufactoryById(manufactoryId) {
            return BaseAPIService.get(UtilsService.getServiceURL(moduleName, controller, "GetManufactoryById"), {
                manufactoryId: manufactoryId
            });
        }

        function AddManufactory(manufactory) {
            return BaseAPIService.post(UtilsService.getServiceURL(moduleName, controller, "InsertManufactory"), manufactory);
        }

        function UpdateManufactory(manufactory) {
            return BaseAPIService.post(UtilsService.getServiceURL(moduleName, controller, "UpdateManufactory"), manufactory);
        }

        function GetManufactoriesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(moduleName, controller, "GetManufactoriesInfo"), {
                filter: filter
            });
        }

        return {
            GetFilteredManufactories: GetFilteredManufactories,
            GetManufactoryById: GetManufactoryById,
            AddManufactory: AddManufactory,
            UpdateManufactory: UpdateManufactory,
            GetManufactoriesInfo: GetManufactoriesInfo
        };
    }

    appControllers.service('Demo_Module_ManufactoryAPIService', manufactoryAPIService);
})(appControllers);