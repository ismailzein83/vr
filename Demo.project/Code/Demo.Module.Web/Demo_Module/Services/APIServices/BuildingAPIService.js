(function (appControllers) {

    'use strict';

    BuildingAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig'];

    function BuildingAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig) {

        var controller = 'Building';

        function GetFilteredBuildings(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredBuildings"), input);
        }

        function GetBuildingById(Id) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetBuildingById'),
                { buildingId: Id }
                );
        }


        function GetBuildingsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetBuildingsInfo"));
        }

        function AddBuilding(building) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddBuilding"), building);
        }
        function UpdateBuilding(building) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateBuilding"), building);
        }
        function DeleteBuilding(Id) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'DeleteBuilding'), {
                buildingId: Id
            });
        }

        return ({
            GetBuildingById: GetBuildingById,
            GetBuildingsInfo: GetBuildingsInfo,
            GetFilteredBuildings: GetFilteredBuildings,
            AddBuilding: AddBuilding,
            UpdateBuilding: UpdateBuilding,
            DeleteBuilding: DeleteBuilding,
        });
    }


    appControllers.service('Demo_Module_BuildingAPIService', BuildingAPIService);
})(appControllers);