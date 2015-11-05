(function (appControllers) {

    "use strict";
    rateTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function rateTypeAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetFilteredRateTypes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "RateType", "GetFilteredRateTypes"), input);
        }
        function GetAllRateTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "RateType", "GetAllRateTypes"));
        }
        function GetRateType(rateTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "RateType", "GetRateType"), {
                rateTypeId: rateTypeId
            });

        }
        function UpdateRateType(rateTypeObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "RateType", "UpdateRateType"), rateTypeObject);
        }
        function AddRateType(rateTypeObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "RateType", "AddRateType"), rateTypeObject);
        }
        return ({
            GetFilteredRateTypes: GetFilteredRateTypes,
            GetAllRateTypes: GetAllRateTypes,
            GetRateType: GetRateType,
            UpdateRateType: UpdateRateType,
            AddRateType: AddRateType
        });
    }

    appControllers.service('WhS_BE_RateTypeAPIService', rateTypeAPIService);

})(appControllers);