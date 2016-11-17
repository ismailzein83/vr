
(function (appControllers) {

    "use strict";
    TariffAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig', 'SecurityService'];

    function TariffAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig, SecurityService) {

        var controllerName = "Tariff";
        

        function GetTariffsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "GetTariffsInfo"), {
                filter: filter
            });
        }      


        return ({
            GetTariffsInfo: GetTariffsInfo,
            
        });
    }

    appControllers.service('NP_IVSwitch_TariffAPIService', TariffAPIService);

})(appControllers);