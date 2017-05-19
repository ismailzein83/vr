(function (appControllers) {

    'use strict';

    SwapDealBuyRouteRuleService.$inject = ['VRModalService'];

    function SwapDealBuyRouteRuleService(VRModalService) {

        function addSwapDealBuyRouteRule(onSwapDealBuyRouteRuleAdded) {

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwapDealBuyRouteRuleAdded = onSwapDealBuyRouteRuleAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_Deal/Views/SwapDeal/RouteRules/SwapDealBuyRouteRuleEditor.html', null, settings);
        }

        function editSwapDealBuyRouteRule(swapDealBuyRouteRuleId, onSwapDealBuyRouteRuleUpdated) {

            var parameters = {
                swapDealBuyRouteRuleId: swapDealBuyRouteRuleId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwapDealBuyRouteRuleUpdated = onSwapDealBuyRouteRuleUpdated;
            };

            VRModalService.showModal('/Client/Modules/WhS_Deal/Views/SwapDeal/RouteRules/SwapDealBuyRouteRuleEditor.html', parameters, settings);
        }


        return {
            addSwapDealBuyRouteRule: addSwapDealBuyRouteRule,
            editSwapDealBuyRouteRule: editSwapDealBuyRouteRule
        };
    }

    appControllers.service('WhS_Deal_SwapDealBuyRouteRuleService', SwapDealBuyRouteRuleService);

})(appControllers);