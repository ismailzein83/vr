(function (appControllers) {

    "use strict";
    cityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function cityAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'City';

        function GetFilteredCities(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredCities"), input);
        }
        function GetCitiesInfo(serializedFilter,countryId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetCitiesInfo"), {
                serializedFilter: serializedFilter,
                countryId: countryId
            });
        }
        function GetCity(cityId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetCity"), {
                cityId: cityId
            });
        }
        function UpdateCity(cityObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "UpdateCity"), cityObject);
        }

        function HasEditCityPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UpdateCity']));
        }

        function AddCity(cityObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "AddCity"), cityObject);
        }

        function HasAddCityPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['AddCity']));
        }

        function GetCountryIdByCityIds(cityIds) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetCountryIdByCityIds"), cityIds);
        }

        return ({
            HasAddCityPermission: HasAddCityPermission,
            GetFilteredCities: GetFilteredCities,
            GetCitiesInfo: GetCitiesInfo,
            GetCity: GetCity,
            UpdateCity: UpdateCity,
            HasEditCityPermission:HasEditCityPermission,
            AddCity: AddCity,
            GetCountryIdByCityIds: GetCountryIdByCityIds
        });
    }

    appControllers.service('VRCommon_CityAPIService', cityAPIService);

})(appControllers);