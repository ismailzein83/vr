(function (appControllers) {

    "use strict";

    rateTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function rateTypeAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = "RateType";

        function GetFilteredRateTypes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredRateTypes"), input);
        }

        function GetAllRateTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetAllRateTypes"));
        }

        function GetRateType(rateTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetRateType"), {
                rateTypeId: rateTypeId
            });
        }

        function UpdateRateType(rateTypeObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "UpdateRateType"), rateTypeObject);
        }

        function AddRateType(rateTypeObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "AddRateType"), rateTypeObject);
        }

        function HasUpdateRateTypePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UpdateRateType']));
        }

        function HasAddRateTypePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['AddRateType']));
        }
        function GetRateTypeHistoryDetailbyHistoryId(rateTypeHistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetRateTypeHistoryDetailbyHistoryId'), {
                rateTypeHistoryId: rateTypeHistoryId
            });
        }
        return ({
            GetFilteredRateTypes: GetFilteredRateTypes,
            GetAllRateTypes: GetAllRateTypes,
            GetRateType: GetRateType,
            UpdateRateType: UpdateRateType,
            AddRateType: AddRateType,
            HasAddRateTypePermission: HasAddRateTypePermission,
            HasUpdateRateTypePermission: HasUpdateRateTypePermission,
            GetRateTypeHistoryDetailbyHistoryId: GetRateTypeHistoryDetailbyHistoryId
        });
    }

    appControllers.service('VRCommon_RateTypeAPIService', rateTypeAPIService);

})(appControllers);