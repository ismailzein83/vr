'use strict';

app.directive('vrWhsDealBuyrouteruleGrid', ['VRNotificationService', 'VRUIUtilsService', 'WhS_Deal_BuyRouteRuleAPIService', 'WhS_Deal_BuyRouteRuleService',
    function (VRNotificationService, VRUIUtilsService, WhS_Deal_BuyRouteRuleAPIService, WhS_Deal_BuyRouteRuleService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctro = new DealBuyRouteRuleGridCtor($scope, ctrl, $attrs);
                ctro.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Deal/Directives/RouteRules/Templates/DealBuyRouteRuleGridTemplate.html'
        };

        function DealBuyRouteRuleGridCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            var dealBED;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dealBuyRouteRules = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_Deal_BuyRouteRuleAPIService.GetFilteredDealBuyRouteRules(dataRetrievalInput).then(function (response) {
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
                    dealBED = query.BED;
                    return gridAPI.retrieveData(query);
                };

                api.onDealBuyRouteRuleAdded = function (addedDealBuyRouteRule) {
                    gridAPI.itemAdded(addedDealBuyRouteRule);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editDealBuyRouteRule,
                    //haspermission: hasEditDealBuyRouteRulePermission
                });
            }
            function editDealBuyRouteRule(dealBuyRouteRuleItem) {
                var onDealBuyRouteRuleUpdated = function (updatedDealBuyRouteRule) {
                    gridAPI.itemUpdated(updatedDealBuyRouteRule);
                };

                WhS_Deal_BuyRouteRuleService.editDealBuyRouteRule(dealBuyRouteRuleItem.VRRuleId, dealBuyRouteRuleItem.DealId, dealBED, onDealBuyRouteRuleUpdated);
            }
            //function hasEditDealBuyRouteRulePermission() {
            //    return VRCommon_DealBuyRouteRuleAPIService.HasEditDealBuyRouteRulePermission();
            //}
        }
    }]);
