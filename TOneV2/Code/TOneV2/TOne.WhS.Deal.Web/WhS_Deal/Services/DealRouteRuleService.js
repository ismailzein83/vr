(function (appControllers) {

    'use strict';

    DealRouteRuleService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService'];

    function DealRouteRuleService(VRModalService, VRNotificationService, UtilsService) {

        function registerDealSellRouteRuleViewToDeal() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Sell Route Rules";
            drillDownDefinition.directive = "vr-whs-routing-routerule-view";

            drillDownDefinition.loadDirective = function (directiveAPI, dealItem) {
                dealItem.DealSellRouteRuleGridAPI = directiveAPI;

                var query = {
                    hideCustomerColumn: true,
                    hideIncludedCodesColumn: true,
                    Filters: []
                };
                var filter = {
                    $type: 'TOne.WhS.Deal.MainExtensions.RouteRule.DealRouteRuleDefinitionFilter, TOne.WhS.Deal.MainExtensions',
                    DealId: dealItem.Entity.DealId
                };
                query.Filters.push(filter);

                var payload = {
                    defaultRouteRuleValues: {
                        selectedCriteria: '0ce291eb-790f-4b24-9dc1-512d457546c5',
                        criteria: { dealId: dealItem.Entity.DealId }
                    },
                    query: query
                };
                return dealItem.DealSellRouteRuleGridAPI.load(payload);
            };

            return drillDownDefinition;
        }

        function registerDealBuyRouteRuleViewToDeal() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Buy Route Rules";
            drillDownDefinition.directive = "vr-whs-deal-buyrouterule-view";

            drillDownDefinition.loadDirective = function (directiveAPI, dealItem) {
                dealItem.DealBuyRouteRuleGridAPI = directiveAPI;

                var query = {
                    DealId: dealItem.Entity.DealId,
                    BED: dealItem.Entity.Settings.RealBED,
                    EED: dealItem.Entity.Settings.RealEED
                };
                return dealItem.DealBuyRouteRuleGridAPI.load(query);
            };

            return drillDownDefinition;
        }
               
        return {
            registerDealSellRouteRuleViewToDeal: registerDealSellRouteRuleViewToDeal,
            registerDealBuyRouteRuleViewToDeal: registerDealBuyRouteRuleViewToDeal
        };
    }

    appControllers.service('WhS_Deal_RouteRuleService', DealRouteRuleService);

})(appControllers);