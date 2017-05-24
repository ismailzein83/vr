'use strict';

app.directive('vrWhsDealSwapdealBuyrouteruleGrid', ['VRNotificationService', 'VRUIUtilsService', 'WhS_Deal_SwapDealBuyRouteRuleAPIService', 'WhS_Deal_SwapDealBuyRouteRuleService',
    function (VRNotificationService, VRUIUtilsService, WhS_Deal_SwapDealBuyRouteRuleAPIService, WhS_Deal_SwapDealBuyRouteRuleService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctro = new SwapDealBuyRouteRuleGridCtor($scope, ctrl, $attrs);
                ctro.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDeal/RouteRules/Templates/SwapDealBuyRouteRuleGridTemplate.html'
        };

        function SwapDealBuyRouteRuleGridCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.swapDealBuyRouteRules = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_Deal_SwapDealBuyRouteRuleAPIService.GetFilteredSwapDealBuyRouteRules(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onSwapDealBuyRouteRuleAdded = function (addedSwapDealBuyRouteRule) {
                    gridAPI.itemAdded(addedSwapDealBuyRouteRule);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editSwapDealBuyRouteRule,
                    //haspermission: hasEditSwapDealBuyRouteRulePermission
                });
            }
            function editSwapDealBuyRouteRule(swapDealBuyRouteRuleItem) {
                var onSwapDealBuyRouteRuleUpdated = function (updatedSwapDealBuyRouteRule) {
                    gridAPI.itemUpdated(updatedSwapDealBuyRouteRule);
                };

                WhS_Deal_SwapDealBuyRouteRuleService.editSwapDealBuyRouteRule(swapDealBuyRouteRuleItem.VRRuleId, swapDealBuyRouteRuleItem.SwapDealId, onSwapDealBuyRouteRuleUpdated);
            }
            //function hasEditSwapDealBuyRouteRulePermission() {
            //    return VRCommon_SwapDealBuyRouteRuleAPIService.HasEditSwapDealBuyRouteRulePermission();
            //}
        }
    }]);
