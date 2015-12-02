'use strict';
app.directive('vrWhsRoutingRprouteDetails', ['UtilsService', 'WhS_Routing_RPRouteAPIService', 'VRUIUtilsService',
    function (UtilsService, WhS_Routing_RPRouteAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                ctrl.policyOptions = [];

                var ctor = new rpRouteDetailsCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/RPRoute/Templates/RPRouteDetailTemplate.html"
        };

        function rpRouteDetailsCtor(ctrl, $scope) {
            var rpRouteDetail;
            var routingDatabaseId;

            var rpRoutePolicyAPI;
            var rpRoutePolicyReadyPromiseDeffered = UtilsService.createPromiseDeferred();

            function initializeController() {
                ctrl.rpRouteOptions = [];

                $scope.onRPRoutePolicySelectorReady = function (api) {
                    rpRoutePolicyAPI = api;
                    rpRoutePolicyReadyPromiseDeffered.resolve();
                };

                $scope.onPolicySelectItem = function (selectedItem) {
                    if (rpRouteDetail == undefined)
                        return;
                    
                    WhS_Routing_RPRouteAPIService.GetRouteOptionDetails(routingDatabaseId, selectedItem.TemplateConfigID, rpRouteDetail.RoutingProductId, rpRouteDetail.SaleZoneId).then(function (response) {
                        angular.forEach(response, function (item) {
                            ctrl.rpRouteOptions.push(item);
                        });
                    });
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined)
                    {
                        rpRouteDetail = payload.rpRouteDetail;
                        routingDatabaseId = payload.routingDatabaseId;
                    }

                    var loadRPRoutePolicyPromiseDeferred = UtilsService.createPromiseDeferred();

                    rpRoutePolicyReadyPromiseDeffered.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(rpRoutePolicyAPI, undefined, loadRPRoutePolicyPromiseDeferred);
                    });

                    return loadRPRoutePolicyPromiseDeferred.promise;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);