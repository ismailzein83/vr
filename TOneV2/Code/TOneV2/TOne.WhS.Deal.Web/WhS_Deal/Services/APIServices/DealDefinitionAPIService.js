(function (app) {

    'use strict';

    DealDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Deal_ModuleConfig', 'SecurityService'];

    function DealDefinitionAPIService(BaseAPIService, UtilsService, WhS_Deal_ModuleConfig, SecurityService) {
        var controllerName = 'SwapDeal';

        function GetDealDefinitionInfo(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetDealDefinitionInfo'), input);
        }
        
        return ({
            GetDealDefinitionInfo: GetDealDefinitionInfo
        });
    }

    app.service('WhS_Deal_DealDefinitionAPIService', DealDefinitionAPIService);

})(app);