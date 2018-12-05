(function (appControllers) {

    'use strict';

    DealBuyRouteRuleService.$inject = ['VRModalService'];

    function DealBuyRouteRuleService(VRModalService) {

        function addDealBuyRouteRule(dealId, dealBED, dealEED, onDealBuyRouteRuleAdded) {

            var parameters = {
                dealId: dealId,
                dealBED: dealBED,
                dealEED: dealEED
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDealBuyRouteRuleAdded = onDealBuyRouteRuleAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_Deal/Views/RouteRules/DealBuyRouteRuleEditor.html', parameters, settings);
        }

        function editDealBuyRouteRule(dealBuyRouteRuleId, dealId, dealBED, dealEED, onDealBuyRouteRuleUpdated) {

            var parameters = {
                dealBuyRouteRuleId: dealBuyRouteRuleId,
                dealId: dealId,
                dealBED: dealBED,
                dealEED: dealEED
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