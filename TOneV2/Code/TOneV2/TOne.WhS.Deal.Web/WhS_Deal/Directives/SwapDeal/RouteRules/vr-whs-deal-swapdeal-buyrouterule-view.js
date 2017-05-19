(function (app) {

    'use strict';

    SwapDealBuyRouteRuleViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'WhS_Deal_SwapDealBuyRouteRuleService'];

    function SwapDealBuyRouteRuleViewDirective(UtilsService, VRNotificationService, WhS_Deal_SwapDealBuyRouteRuleService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SubAccountsViewCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDeal/RouteRules/Templates/SwapDealBuyRouteRuleViewTemplate.html'
        };

        function SubAccountsViewCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var swapDealId;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSwapDealBuyRouteRuleReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onSwapDealBuyRouteRuleAdded = function () {
                    var onSwapDealBuyRouteRuleAdded = function (addedSubcAccount) {
                        gridAPI.onSwapDealBuyRouteRuleAdded(addedSubcAccount);
                    };

                    WhS_Deal_SwapDealBuyRouteRuleService.addSwapDealBuyRouteRule(swapDealId, onSwapDealBuyRouteRuleAdded);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    $scope.scopeModel.isGridLoading = true;

                    if (payload != undefined) {
                        swapDealId = payload.swapDealId;
                    }

                    return gridAPI.load(buildGridPayload(payload)).then(function () {
                        $scope.scopeModel.isGridLoading = false;
                    });
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildGridPayload(loadPayload) {
                return loadPayload;
            }
        }
    }

    app.directive('vrWhsDealSwapdealBuyrouteruleView', SwapDealBuyRouteRuleViewDirective);

})(app);