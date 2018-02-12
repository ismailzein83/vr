'use strict';

app.directive('vrWhsRoutingRprouteDetails', ['UtilsService', 'WhS_Routing_RPRouteAPIService', 'WhS_Routing_RPRouteService', 'WhS_Routing_SupplierStatusEnum', 'VRUIUtilsService', 'VRNotificationService',
    function (UtilsService, WhS_Routing_RPRouteAPIService, WhS_Routing_RPRouteService, WhS_Routing_SupplierStatusEnum, VRUIUtilsService, VRNotificationService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new rpRouteDetailsCtor(ctrl, $scope);
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
            templateUrl: "/Client/Modules/WhS_Routing/Directives/RPRoute/Templates/RPRouteDetailTemplate.html"
        };

        function rpRouteDetailsCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var rpRouteDetail;
            var routingDatabaseId;
            var policies;
            var defaultPolicyId;
            var customerId;
            var showInSystemCurrency;
            var currencyId;
            var includeBlockedSuppliers;

            var gridAPI;
            var gridReadyDeferred = UtilsService.createPromiseDeferred();

            var rpRoutePolicyAPI;
            var rpRoutePolicyReadyPromiseDeffered = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.selectedPolicy;
                $scope.rpRouteOptions = [];

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

                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                extendRPRouteOptionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);

                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                $scope.openRouteOptionSupplier = function (dataItem) {
                    WhS_Routing_RPRouteService.viewRPRouteOptionSupplier(routingDatabaseId, rpRouteDetail.RoutingProductId, rpRouteDetail.SaleZoneId, dataItem.SupplierId, currencyId);
                };

                //$scope.getRowStyle = function (dataItem) {
                //    var rowStyle;
                //    //if (dataItem.SupplierStatus == WhS_Routing_SupplierStatusEnum.PartialActive.value)
                //    //    rowStyle = { CssClass: "bg-warning" }
                //    if (dataItem.SupplierStatus == WhS_Routing_SupplierStatusEnum.Block.value)
                //        rowStyle = { CssClass: "bg-danger" };
                //    return rowStyle;
                //};

                UtilsService.waitMultiplePromises([rpRoutePolicyReadyPromiseDeffered.promise, gridReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        rpRouteDetail = payload.rpRouteDetail;
                        routingDatabaseId = payload.routingDatabaseId;
                        policies = payload.filteredPolicies;
                        defaultPolicyId = payload.defaultPolicyId;
                        customerId = payload.customerId;
                        showInSystemCurrency = payload.showInSystemCurrency;
                        currencyId = payload.currencyId;
                        includeBlockedSuppliers = payload.includeBlockedSuppliers;
                    }

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

                    function loadPolicySelector() {
                        var loadPolicySelectorDeferred = UtilsService.createPromiseDeferred();

                        var policySelectorPayload = {
                            filter: { RoutingDatabaseId: routingDatabaseId },
                            selectedIds: defaultPolicyId
                        };
                        VRUIUtilsService.callDirectiveLoad(rpRoutePolicyAPI, policySelectorPayload, loadPolicySelectorDeferred);

                        return loadPolicySelectorDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

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
                        SaleZoneId: rpRouteDetail.SaleZoneId,
                        CustomerId: customerId,
                        ShowInSystemCurrency: showInSystemCurrency,
                        includeBlockedSuppliers: includeBlockedSuppliers
                    };
                }

                return gridAPI.retrieveData(query);
            }

            function extendRPRouteOptionObject(rpRouteOption) {
                if (rpRouteOption.ConvertedSupplierRate == 0) {
                    rpRouteOption.ConvertedSupplierRate = 'N/A';
                }
                rpRouteOption.SupplierStatusDescription = UtilsService.getEnumDescription(WhS_Routing_SupplierStatusEnum, rpRouteOption.SupplierStatus);
                rpRouteOption.IsBlocked = rpRouteOption.SupplierStatus == WhS_Routing_SupplierStatusEnum.Block.value;
            }
        }

        return directiveDefinitionObject;
    }]);