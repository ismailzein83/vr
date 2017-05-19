'use strict';

app.directive('vrWhsDealSwapdealanalysisRouterulecriteria', ['UtilsService', 'VRUIUtilsService', 'WhS_Routing_RouteRuleAPIService', 'WhS_Routing_RouteRuleCriteriaTypeEnum',
    function (UtilsService, VRUIUtilsService, WhS_Routing_RouteRuleAPIService, WhS_Routing_RouteRuleCriteriaTypeEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new RouteRuleCriteriaCtor(ctrl, $scope);
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
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_Deal/Directives/Extensions/SwapDeal/RouteRuleCriteria/Templates/SwapDealRouteRuleCriteriaDirective.html';
            }
        };

        function RouteRuleCriteriaCtor(ctrl, $scope) {
            var isLinkedRouteRule;
            var routingProductId;
            var routeRuleCriteria;
            var sellingNumberPlanId;
            var linkedCode;

            //var saleZoneGroupSettingsAPI;
            //var saleZoneGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            //var codeCriteriaGroupSettingsAPI;
            //var codeCriteriaGroupSettingsReadyPromiseDeferred;

            //var customerGroupSettingsAPI;
            //var customerGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }


            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    $scope.scopeModel.disableCriteria = isLinkedRouteRule = payload.isLinkedRouteRule;
                    routingProductId = payload.routingProductId;
                    sellingNumberPlanId = payload.sellingNumberPlanId;
                    routeRuleCriteria = payload.routeRuleCriteria;
                    linkedCode = payload.linkedCode;

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Deal.MainExtensions.SwapDeal.RouteRuleCriteria.SwapDealRouteRuleCriteria, TOne.WhS.Deal.MainExtensions",
                        SwapDealId: 12
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);