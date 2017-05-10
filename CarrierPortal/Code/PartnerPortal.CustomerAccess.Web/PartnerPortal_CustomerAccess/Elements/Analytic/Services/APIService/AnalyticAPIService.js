(function (appControllers) {

    'use strict';

    AnalyticAPIServiceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PartnerPortal_CustomerAccess_ModuleConfig', 'SecurityService'];

    function AnalyticAPIServiceAPIService(BaseAPIService, UtilsService, PartnerPortal_CustomerAccess_ModuleConfig, SecurityService) {
        var controllerName = 'Analytic';

        function GetAnalyticTileInfo(analyticDefinitionSettings) {
            return BaseAPIService.post(UtilsService.getServiceURL(PartnerPortal_CustomerAccess_ModuleConfig.moduleName, controllerName, "GetAnalyticTileInfo"), analyticDefinitionSettings);
        };
        return {
            GetAnalyticTileInfo: GetAnalyticTileInfo,
        };
    }

    appControllers.service('PartnerPortal_CustomerAccess_AnalyticAPIService', AnalyticAPIServiceAPIService);

})(appControllers);