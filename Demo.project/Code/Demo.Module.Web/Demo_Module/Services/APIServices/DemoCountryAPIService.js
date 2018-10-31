(function (appControllers) {
    "use strict";
   demoCountryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
   function demoCountryAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "DemoCountry";


        function GetDemoCountryById(demoCountryId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetDemoCountryById"),
                {
                    demoCountryId: demoCountryId
                });
        }

        function GetDemoCountriesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetDemoCountriesInfo"), { filter: filter });
        };


        return {
            GetDemoCountryById: GetDemoCountryById,
            GetDemoCountriesInfo: GetDemoCountriesInfo,
        };
    };
   appControllers.service("Demo_Module_DemoCountryAPIService", demoCountryAPIService);

})(appControllers);