(function (appControllers) {

    'use strict';

    DealBuyRouteRuleService.$inject = ['VRModalService'];

    function DealBuyRouteRuleService(VRModalService) {

        function addDealBuyRouteRule(dealId, dealBED, onDealBuyRouteRuleAdded) {

            var parameters = {
                dealId: dealId,
                dealBED: dealBED
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDealBuyRouteRuleAdded = onDealBuyRouteRuleAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_Deal/Views/RouteRules/DealBuyRouteRuleEditor.html', parameters, settings);
        }

        function editDealBuyRouteRule(dealBuyRouteRuleId, dealId, dealBED, onDealBuyRouteRuleUpdated) {

            var parameters = {
                dealBuyRouteRuleId: dealBuyRouteRuleId,
                dealId: dealId,
                dealBED: dealBED
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDealBuyRouteRuleUpdated = onDealBuyRouteRuleUpdated;
            };

            VRModalService.showModal('/Client/Modules/WhS_Deal/Views/RouteRules/DealBuyRouteRuleEditor.html', parameters, settings);
        }

        return {
            addDealBuyRouteRule: addDealBuyRouteRule,
            editDealBuyRouteRule: editDealBuyRouteRule
        };
    }

    appControllers.service('WhS_Deal_BuyRouteRuleService', DealBuyRouteRuleService);

})(appControllers);