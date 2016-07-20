'use strict';
app.directive('vrWhsRoutingRprouteDetails', ['UtilsService', 'WhS_Routing_RPRouteAPIService', 'WhS_Routing_RPRouteService', 'VRUIUtilsService', 'VRNotificationService',
function (UtilsService, WhS_Routing_RPRouteAPIService, WhS_Routing_RPRouteService, VRUIUtilsService, VRNotificationService) {

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
            var gridReadyDeferred = UtilsService.createPromiseDeferred();
            var rpRouteDetail;
            var routingDatabaseId;
            var policies;
            var defaultPolicyId;
            var rpRoutePolicyAPI;
            var rpRoutePolicyReadyPromiseDeffered = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.selectedPolicy;
                ctrl.rpRouteOptions = [];

                $scope.onRPRoutePolicySelectorReady = function (api) {
                    rpRoutePolicyAPI = api;
                    rpRoutePolicyReadyPromiseDeffered.resolve();
                };

                $scope.onPolicySelectItem = function (selectedItem) {
                    loadGrid(selectedItem.ExtensionConfigurationId);
                };

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    gridReadyDeferred.resolve();
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_Routing_RPRouteAPIService.GetFilteredRPRouteOptions(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                $scope.openRouteOptionSupplier = function (dataItem) {
                    WhS_Routing_RPRouteService.viewRPRouteOptionSupplier(routingDatabaseId, rpRouteDetail.RoutingProductId, rpRouteDetail.SaleZoneId, dataItem.Entity.SupplierId, dataItem.SupplierName);
                }

                UtilsService.waitMultiplePromises([rpRoutePolicyReadyPromiseDeffered.promise, gridReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    setGlobalVairables(payload);

                    var promises = [];

                    var loadPolicySelectorPromise = loadPolicySelector();
                    promises.push(loadPolicySelectorPromise);

                    var loadGridDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadGridDeferred.promise);

                    loadPolicySelectorPromise.then(function () {
                        UtilsService.safeApply($scope);

                        loadGrid(defaultPolicyId).then(function () {
                            loadGridDeferred.resolve();
                        }).catch(function () {
                            loadGridDeferred.reject();
                        });
                    });

                    return UtilsService.waitMultiplePromises(promises);

                    function setGlobalVairables(payload) {
                        if (payload != undefined) {
                            rpRouteDetail = payload.rpRouteDetail;
                            routingDatabaseId = payload.routingDatabaseId;
                            policies = payload.filteredPolicies;
                            defaultPolicyId = payload.defaultPolicyId
                        }
                    }

                    function loadPolicySelector() {
                        var loadPolicySelectorDeferred = UtilsService.createPromiseDeferred();

                        var policySelectorPayload = {
                            filter: {
                                RoutingDatabaseId: routingDatabaseId
                            },
                            selectedIds: defaultPolicyId
                        };

                        VRUIUtilsService.callDirectiveLoad(rpRoutePolicyAPI, policySelectorPayload, loadPolicySelectorDeferred);
                        return loadPolicySelectorDeferred.promise;
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadGrid(policyOptionConfigId) {
                var query = null;

                if (rpRouteDetail) {
                    query = {
                        RoutingDatabaseId: routingDatabaseId,
                        PolicyOptionConfigId: policyOptionConfigId, // $scope.selectedPolicy is != undefined since the policy selector is loaded before the grid
                        RoutingProductId: rpRouteDetail.RoutingProductId,
                        SaleZoneId: rpRouteDetail.SaleZoneId
                    };
                }
                
                return gridAPI.retrieveData(query);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);