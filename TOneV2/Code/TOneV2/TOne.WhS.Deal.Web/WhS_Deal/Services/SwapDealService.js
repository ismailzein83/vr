(function (appControllers) {

    'use strict';

    SwapDealService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRCommon_ObjectTrackingService'];

    function SwapDealService(VRModalService, VRNotificationService, UtilsService, VRCommon_ObjectTrackingService) {
        var editorUrl = '/Client/Modules/WhS_Deal/Views/SwapDeal/SwapDealEditor.html';

        var drillDownDefinitions = [];

        function addSwapDeal(onSwapDealAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwapDealAdded = onSwapDealAdded;
            };
            VRModalService.showModal(editorUrl, null, settings);
        }

        function editSwapDeal(dealId, onSwapDealUpdated, isReadOnly) {
            var parameters = {
                dealId: dealId,
                isReadOnly: isReadOnly
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwapDealUpdated = onSwapDealUpdated;
            };

            VRModalService.showModal(editorUrl, parameters, settings);
        }
        function viewSwapDeal(dealId) {
            var parameters = {
                dealId: dealId,
                isReadOnly: true
            };

            var settings = {};
            VRModalService.showModal(editorUrl, parameters, settings);
        }

        function getEntityUniqueName() {
            return "WhS_Deal_SwapDeal";
        }

        function registerObjectTrackingDrillDownToSwapDeal() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";

            drillDownDefinition.loadDirective = function (directiveAPI, swapDealItem) {
                swapDealItem.objectTrackingGridAPI = directiveAPI;

                var query = {
                    ObjectId: swapDealItem.Entity.DealId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return swapDealItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);
        }

        function registerSwapDealSellRouteRuleViewToSwapDeal() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Sell Route Rules";
            drillDownDefinition.directive = "vr-whs-routing-routerule-view";

            drillDownDefinition.loadDirective = function (directiveAPI, swapDealItem) {
                swapDealItem.SwapDealSellRouteRuleGridAPI = directiveAPI;

                var query = {
                    hideCustomerColumn: true,
                    hideIncludedCodesColumn: true,
                    Filters: []
                };
                var filter = {
                    $type: 'TOne.WhS.Deal.MainExtensions.SwapDeal.SwapDealRouteRuleDefinitionFilter, TOne.WhS.Deal.MainExtensions',
                    SwapDealId: swapDealItem.Entity.DealId
                };
                query.Filters.push(filter);

                var payload = {
                    defaultRouteRuleValues: {
                        selectedCriteria: '0ce291eb-790f-4b24-9dc1-512d457546c5',
                        criteria: { swapDealId: swapDealItem.Entity.DealId }
                    },
                    query: query
                };
                return swapDealItem.SwapDealSellRouteRuleGridAPI.load(payload);
            };

            addDrillDownDefinition(drillDownDefinition);
        }

        function registerSwapDealBuyRouteRuleViewToSwapDeal() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Buy Route Rules";
            drillDownDefinition.directive = "vr-whs-deal-swapdeal-buyrouterule-view";

            drillDownDefinition.loadDirective = function (directiveAPI, swapDealItem) {
                swapDealItem.SwapDealBuyRouteRuleGridAPI = directiveAPI;

                var query = {
                    swapDealId: swapDealItem.Entity.DealId
                };
                return swapDealItem.SwapDealBuyRouteRuleGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);
        }

        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        function viewHistorySwapDeal(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {                
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal(editorUrl, modalParameters, modalSettings);
        }

        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "WhS_Deal_SwapDealManager_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistorySwapDeal(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }

        function analyzeSwapDeal(onSwapDealAnalyzed) {
            var parameters = null;

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onSwapDealAnalyzed = onSwapDealAnalyzed;
            };

            VRModalService.showModal('/Client/Modules/WhS_Deal/Views/SwapDealAnalysis/SwapDealAnalysis.html', parameters, settings);
        }


        return {
            addSwapDeal: addSwapDeal,
            editSwapDeal: editSwapDeal,
            registerObjectTrackingDrillDownToSwapDeal: registerObjectTrackingDrillDownToSwapDeal,
            registerSwapDealSellRouteRuleViewToSwapDeal: registerSwapDealSellRouteRuleViewToSwapDeal,
            registerSwapDealBuyRouteRuleViewToSwapDeal: registerSwapDealBuyRouteRuleViewToSwapDeal,
            getDrillDownDefinition: getDrillDownDefinition,
            registerHistoryViewAction: registerHistoryViewAction,
            analyzeSwapDeal: analyzeSwapDeal,
            viewSwapDeal: viewSwapDeal
        };
    }

    appControllers.service('WhS_Deal_SwapDealService', SwapDealService);

})(appControllers);