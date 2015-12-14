'use strict';
app.directive('vrWhsRoutingRprouteDetails', ['UtilsService', 'WhS_Routing_RPRouteAPIService', 'WhS_Routing_RPRouteService', 'VRUIUtilsService',
    function (UtilsService, WhS_Routing_RPRouteAPIService, WhS_Routing_RPRouteService, VRUIUtilsService) {

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
            var gridAPI;
            var rpRouteDetail;
            var routingDatabaseId;
            var policies;
            var defaultPolicyId;
            var rpRoutePolicyAPI;
            var rpRoutePolicyReadyPromiseDeffered = UtilsService.createPromiseDeferred();

            function initializeController() {
                ctrl.rpRouteOptions = [];

                $scope.onRPRoutePolicySelectorReady = function (api) {
                    rpRoutePolicyAPI = api;
                    rpRoutePolicyReadyPromiseDeffered.resolve();
                };

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_Routing_RPRouteAPIService.GetFilteredRPRouteOptions(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                $scope.onPolicySelectItem = function (selectedItem) {
                    if (rpRouteDetail == undefined)
                        return;

                    var query = {
                        RoutingDatabaseId: routingDatabaseId,
                        PolicyOptionConfigId: selectedItem.TemplateConfigID,
                        RoutingProductId: rpRouteDetail.RoutingProductId,
                        SaleZoneId: rpRouteDetail.SaleZoneId
                    };

                    if (gridAPI != undefined)
                        return gridAPI.retrieveData(query);
                };

                $scope.openRouteOptionSupplier = function (dataItem) {
                    WhS_Routing_RPRouteService.viewRPRouteOptionSupplier(routingDatabaseId, rpRouteDetail.RoutingProductId, rpRouteDetail.SaleZoneId, dataItem.Entity.SupplierId, dataItem.SupplierName);
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        rpRouteDetail = payload.rpRouteDetail;
                        routingDatabaseId = payload.routingDatabaseId;
                        policies = payload.filteredPolicies;
                        defaultPolicyId = payload.defaultPolicyId
                    }

                    var loadRPRoutePolicyPromiseDeferred = UtilsService.createPromiseDeferred();

                    rpRoutePolicyReadyPromiseDeffered.promise.then(function () {
                        var payload = {
                            filteredIds: policies,
                            selectedId: defaultPolicyId
                        };
                        VRUIUtilsService.callDirectiveLoad(rpRoutePolicyAPI, payload, loadRPRoutePolicyPromiseDeferred);
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