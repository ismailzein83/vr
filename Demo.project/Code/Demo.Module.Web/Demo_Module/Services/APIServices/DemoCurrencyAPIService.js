(function (appControllers) {
    "use strict";
    demoCurrencyAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function demoCurrencyAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "DemoCurrency";


        function GetDemoCurrencyById(demoCurrencyId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetDemoCurrencyById"),
                {
                    demoCurrencyId: demoCurrencyId
                });
        }

        function GetDemoCurrenciesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetDemoCurrenciesInfo"), { filter: filter });
        };


        return {
            GetDemoCurrencyById: GetDemoCurrencyById,
            GetDemoCurrenciesInfo: GetDemoCurrenciesInfo,
        };
    };
    appControllers.service("Demo_Module_DemoCurrencyAPIService", demoCurrencyAPIService);

})(appControllers);