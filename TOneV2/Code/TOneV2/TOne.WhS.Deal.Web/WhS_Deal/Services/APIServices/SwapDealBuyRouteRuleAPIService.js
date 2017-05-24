(function (app) {

    'use strict';

    SwapDealBuyRouteRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Deal_ModuleConfig', 'SecurityService'];

    function SwapDealBuyRouteRuleAPIService(BaseAPIService, UtilsService, WhS_Deal_ModuleConfig, SecurityService) {

        var controllerName = 'SwapDealBuyRouteRule';

        function GetFilteredSwapDealBuyRouteRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetFilteredSwapDealBuyRouteRules'), input);
        }

        function GetSwapDealBuyRouteRule(swapDealBuyRouteRuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetSwapDealBuyRouteRule'), {
                swapDealBuyRouteRuleId: swapDealBuyRouteRuleId
            });
        }

        function AddSwapDealBuyRouteRule(swapDealBuyRouteRule) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'AddSwapDealBuyRouteRule'), swapDealBuyRouteRule);
        }

        function UpdateSwapDealBuyRouteRule(swapDealBuyRouteRule) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'UpdateSwapDealBuyRouteRule'), swapDealBuyRouteRule);
        }

        function GetSwapDealBuyRouteRuleExtendedSettingsConfigs() {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetSwapDealBuyRouteRuleExtendedSettingsConfigs'));
        }


        return ({
            GetFilteredSwapDealBuyRouteRules: GetFilteredSwapDealBuyRouteRules,
            GetSwapDealBuyRouteRule: GetSwapDealBuyRouteRule,
            AddSwapDealBuyRouteRule: AddSwapDealBuyRouteRule,
            UpdateSwapDealBuyRouteRule: UpdateSwapDealBuyRouteRule,
            GetSwapDealBuyRouteRuleExtendedSettingsConfigs: GetSwapDealBuyRouteRuleExtendedSettingsConfigs
        });
    }

    app.service('WhS_Deal_SwapDealBuyRouteRuleAPIService', SwapDealBuyRouteRuleAPIService);

})(app);