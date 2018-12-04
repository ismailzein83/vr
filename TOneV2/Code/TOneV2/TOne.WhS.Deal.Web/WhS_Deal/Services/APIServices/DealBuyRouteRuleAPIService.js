(function (app) {

    'use strict';

    DealBuyRouteRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Deal_ModuleConfig', 'SecurityService'];

    function DealBuyRouteRuleAPIService(BaseAPIService, UtilsService, WhS_Deal_ModuleConfig, SecurityService) {

        var controllerName = 'DealBuyRouteRule';

        function GetFilteredDealBuyRouteRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetFilteredDealBuyRouteRules'), input);
        }

        function GetDealBuyRouteRule(dealBuyRouteRuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetDealBuyRouteRule'), {
                dealBuyRouteRuleId: dealBuyRouteRuleId
            });
        }

        function AddDealBuyRouteRule(dealBuyRouteRule) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'AddDealBuyRouteRule'), dealBuyRouteRule);
        }

        function UpdateDealBuyRouteRule(dealBuyRouteRule) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'UpdateDealBuyRouteRule'), dealBuyRouteRule);
        }

        function GetDealBuyRouteRuleExtendedSettingsConfigs() {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetDealBuyRouteRuleExtendedSettingsConfigs'));
        }

        function GetCarrierAccountId(dealId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetCarrierAccountId'), {
                dealId: dealId
            });
        }

        return ({
            GetFilteredDealBuyRouteRules: GetFilteredDealBuyRouteRules,
            GetDealBuyRouteRule: GetDealBuyRouteRule,
            AddDealBuyRouteRule: AddDealBuyRouteRule,
            UpdateDealBuyRouteRule: UpdateDealBuyRouteRule,
            GetDealBuyRouteRuleExtendedSettingsConfigs: GetDealBuyRouteRuleExtendedSettingsConfigs,
            GetCarrierAccountId: GetCarrierAccountId
        });
    }

    app.service('WhS_Deal_BuyRouteRuleAPIService', DealBuyRouteRuleAPIService);
})(app);