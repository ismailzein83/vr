
(function (appControllers) {

    "use strict";
    DomainAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig', 'SecurityService'];

    function DomainAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig, SecurityService) {

        var controllerName = "Domain";


        function GetDomainsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "GetDomainsInfo"), {
                filter: filter
            });
        }


        return ({
            GetDomainsInfo: GetDomainsInfo,

        });
    }

    appControllers.service('NP_IVSwitch_DomainAPIService', DomainAPIService);

})(appControllers);