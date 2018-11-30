(function (appControllers) {

    'use strict';

    SwapDealService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRCommon_ObjectTrackingService', 'WhS_Deal_RouteRuleService'];

    function SwapDealService(VRModalService, VRNotificationService, UtilsService, VRCommon_ObjectTrackingService, WhS_Deal_RouteRuleService) {
        var editorUrl = '/Client/Modules/WhS_Deal/Views/SwapDeal/SwapDealEditor.html';

        var drillDownDefinitions = [];

        function addSwapDeal(onSwapDealAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwapDealAdded = onSwapDealAdded;
            };
            VRModalService.showModal(editorUrl, null, settings);
        }

        function editSwapDeal(dealId, onSwapDealUpdated, isReadOnly, isEditable) {
            var parameters = {
                dealId: dealId,
                isReadOnly: isReadOnly,
                isEditable: isEditable
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwapDealUpdated = onSwapDealUpdated;
                modalScope.viewMode = false;
            };

            VRModalService.showModal(editorUrl, parameters, settings);
        }
        function viewSwapDeal(dealId, viewMode) {
            var parameters = {
                dealId: dealId,
                isReadOnly: true
            };
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                if (viewMode != undefined && viewMode == true) {
                    UtilsService.setContextReadOnly(modalScope);
                    modalScope.viewMode = true;
                }
            };
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
            addDrillDownDefinition(WhS_Deal_RouteRuleService.registerDealSellRouteRuleViewToDeal());
        }

        function registerSwapDealBuyRouteRuleViewToSwapDeal() {
            addDrillDownDefinition(WhS_Deal_RouteRuleService.registerDealBuyRouteRuleViewToDeal());
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