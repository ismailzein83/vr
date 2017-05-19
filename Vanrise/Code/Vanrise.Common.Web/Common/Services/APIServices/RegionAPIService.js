(function (appControllers) {

    "use strict";
    regionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function regionAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'Region';

        function GetFilteredRegions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredRegions"), input);
        }

        function GetRegionsInfo(countryId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetRegionsInfo"), {
                countryId: countryId
            });
        }

        function GetRegion(regionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetRegion"), {
                regionId: regionId
            });
        }

        function UpdateRegion(regionObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "UpdateRegion"), regionObject);
        }

        function HasEditRegionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UpdateRegion']));
        }

        function AddRegion(regionObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "AddRegion"), regionObject);
        }

        function HasAddRegionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['AddRegion']));
        }
        function GetRegionHistoryDetailbyHistoryId(regionHistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetRegionHistoryDetailbyHistoryId'), {
                regionHistoryId: regionHistoryId
            });
        }

        return ({
            HasAddRegionPermission: HasAddRegionPermission,
            GetFilteredRegions: GetFilteredRegions,
            GetRegionsInfo: GetRegionsInfo,
            GetRegion: GetRegion,
            UpdateRegion: UpdateRegion,
            HasEditRegionPermission:HasEditRegionPermission,
            AddRegion: AddRegion,
            GetRegionHistoryDetailbyHistoryId: GetRegionHistoryDetailbyHistoryId
        });
    }

    appControllers.service('VRCommon_RegionAPIService', regionAPIService);

})(appControllers);