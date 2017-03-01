
(function (appControllers) {

    'use strict';

    cityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];

    function cityAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = 'City';

        function GetCity(Id) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetCity'), 
                {Id:Id}
                );
        }

        function GetFilteredCities(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetFilteredCities'), input);
        }

        function AddCity(cityObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddCity"), cityObject);
        }
        function UpdateCity(cityObject) { 
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateCity"), cityObject);
        }
        return ({
            GetFilteredCities: GetFilteredCities,
            UpdateCity:UpdateCity,
            AddCity: AddCity,
            GetCity: GetCity
        });
    }

    
    appControllers.service('Demo_Module_CityAPIService', cityAPIService);
})(appControllers);