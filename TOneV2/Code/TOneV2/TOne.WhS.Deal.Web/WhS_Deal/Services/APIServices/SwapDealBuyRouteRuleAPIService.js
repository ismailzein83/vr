(function (app) {

    'use strict';

    SwapDealBuyRouteRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Deal_ModuleConfig', 'SecurityService'];

    function SwapDealBuyRouteRuleAPIService(BaseAPIService, UtilsService, WhS_Deal_ModuleConfig, SecurityService) {

        var controllerName = 'SwapDealBuyRouteRule';

        function GetFilteredSwapDealBuyRouteRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetFilteredSwapDealBuyRouteRules'), input);
        }

        function GetVRRule(vrRuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetVRRule'), {
                vrRuleId: vrRuleId
            });
        }

        function AddVRRule(swapDealBuyRouteRule) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'AddVRRule'), swapDealBuyRouteRule);
        }

        function UpdateVRRule(swapDealBuyRouteRule) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'AddVRRule'), swapDealBuyRouteRule);
        }


        return ({
            GetFilteredSwapDealBuyRouteRules: GetFilteredSwapDealBuyRouteRules,
            GetVRRule: GetVRRule,
            AddVRRule: AddVRRule,
            UpdateVRRule: UpdateVRRule,
        });
    }

    app.service('WhS_Deal_SwapDealBuyRouteRuleBuyRouteRule', SwapDealBuyRouteRuleAPIService);

})(app);