(function (appControllers) {

    "use strict";
    cityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function cityAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        function GetFilteredCities(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "City", "GetFilteredCities"), input);
        }
        function GetCitiesInfo(serializedFilter,countryId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "City", "GetCitiesInfo"), {
                serializedFilter: serializedFilter,
                countryId: countryId
            });
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

        function GetCountryIdByCityIds(cityIds) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "City", "GetCountryIdByCityIds"),cityIds);
        }

        return ({
            GetFilteredCities: GetFilteredCities,
            GetCitiesInfo: GetCitiesInfo,
            GetCity: GetCity,
            UpdateCity: UpdateCity,
            AddCity: AddCity,
            GetCountryIdByCityIds: GetCountryIdByCityIds
        });
    }

    appControllers.service('VRCommon_CityAPIService', cityAPIService);

})(appControllers);