(function (appControllers) {

    "use strict";
    cityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function cityAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        function GetFilteredCities(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "City", "GetFilteredCities"), input);
        }
        function GetAllCities() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "City", "GetAllCities"));
        }
        function GetCity(cityId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "City", "GetCity"), {
                cityId: cityId
            });
        }
        function UpdateCity(cityObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "City", "UpdateCity"), cityObject);
        }
        function AddCity(cityObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "City", "AddCity"), cityObject);
        }
        return ({
            GetFilteredCities: GetFilteredCities,
            GetAllCities: GetAllCities,
            GetCity: GetCity,
            UpdateCity: UpdateCity,
            AddCity: AddCity
        });
    }

    appControllers.service('VRCommon_CityAPIService', cityAPIService);

})(appControllers);