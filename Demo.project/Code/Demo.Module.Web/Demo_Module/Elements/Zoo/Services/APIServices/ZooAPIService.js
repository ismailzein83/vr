(function (appControllers) {

    'use strict';

    zooAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig'];

    function zooAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig) {

        var controller = 'Demo_Zoo';

        function GetFilteredZoos(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetFilteredZoos'), input);
        }

        function GetZoosInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetZoosInfo'), { filter: filter });
        }

        function GetZooById(zooId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetZooById'), { zooId: zooId });
        }

        function AddZoo(zoo) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'AddZoo'), zoo);
        }

        function UpdateZoo(zoo) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'UpdateZoo'), zoo);
        }

        return {
            GetFilteredZoos: GetFilteredZoos,
            GetZoosInfo: GetZoosInfo,
            GetZooById: GetZooById,
            AddZoo: AddZoo,
            UpdateZoo: UpdateZoo
        };
    }

    appControllers.service('Demo_Module_ZooAPIService', zooAPIService);
})(appControllers);