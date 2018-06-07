(function (appControllers) {
    "use strict";
    buildingAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function buildingAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Building";

        function GetFilteredBuildings(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredBuildings"), input);
        }
        function GetBuildingById(buildingId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetBuildingById"),
                {
                    buildingId: buildingId
                });
        }

        function UpdateBuilding(building) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateBuilding"), building);
        }
        function AddBuilding(building) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddBuilding"), building);
        };
        function GetBuildingsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetBuildingsInfo"), { filter: filter });
        };

        return {
            GetFilteredBuildings: GetFilteredBuildings,
            GetBuildingById: GetBuildingById,
            UpdateBuilding: UpdateBuilding,
            AddBuilding: AddBuilding,
            GetBuildingsInfo: GetBuildingsInfo
        };
    };
    appControllers.service("Demo_Module_BuildingAPIService", buildingAPIService);

})(appControllers);