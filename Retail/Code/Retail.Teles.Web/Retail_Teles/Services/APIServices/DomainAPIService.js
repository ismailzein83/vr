(function (appControllers) {

    "use strict";
    DomainAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Teles_ModuleConfig'];

    function DomainAPIService(BaseAPIService, UtilsService, Retail_Teles_ModuleConfig) {

        var controllerName = "TelesDomain";
        function GetDomainsInfo(vrConnectionId,serializedFilter)
        {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "GetDomainsInfo"), {
                vrConnectionId: vrConnectionId,
                serializedFilter: serializedFilter,
            });
        }
        return ({
            GetDomainsInfo: GetDomainsInfo,
        });
    }

    appControllers.service('Retail_Teles_DomainAPIService', DomainAPIService);

})(appControllers);