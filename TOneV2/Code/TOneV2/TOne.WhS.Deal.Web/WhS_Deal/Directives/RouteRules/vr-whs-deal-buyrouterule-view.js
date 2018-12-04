(function (app) {

    'use strict';

    DealBuyRouteRuleViewDirective.$inject = ['UtilsService', 'WhS_Deal_BuyRouteRuleService'];

    function DealBuyRouteRuleViewDirective(UtilsService, WhS_Deal_BuyRouteRuleService) {
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
            templateUrl: '/Client/Modules/WhS_Deal/Directives/RouteRules/Templates/DealBuyRouteRuleViewTemplate.html'
        };

        function SubAccountsViewCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var dealId;
            var dealBED;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDealBuyRouteRuleGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDealBuyRouteRuleAdded = function () {
                    var onDealBuyRouteRuleAdded = function (addedDealBuyRouteRulet) {
                        gridAPI.onDealBuyRouteRuleAdded(addedDealBuyRouteRulet);
                    };

                    WhS_Deal_BuyRouteRuleService.addDealBuyRouteRule(dealId, dealBED, onDealBuyRouteRuleAdded);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    $scope.scopeModel.isGridLoading = true;

                    if (payload != undefined) {
                        dealId = payload.DealId;
                        dealBED = payload.BED;
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

    app.directive('vrWhsDealBuyrouteruleView', DealBuyRouteRuleViewDirective);

})(app);