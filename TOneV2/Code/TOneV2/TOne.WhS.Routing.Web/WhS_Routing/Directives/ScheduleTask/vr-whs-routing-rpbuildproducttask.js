"use strict";

app.directive("vrWhsRoutingRpbuildproducttask", ['WhS_Routing_RPRouteAPIService', 'VRNotificationService', 'WhS_Routing_RoutingDatabaseTypeEnum', 'WhS_Routing_RoutingProcessTypeEnum', 'WhS_Routing_SaleZoneRangeOptions', 'UtilsService', 'VRUIUtilsService',
    function (WhS_Routing_RPRouteAPIService, VRNotificationService, WhS_Routing_RoutingDatabaseTypeEnum, WhS_Routing_RoutingProcessTypeEnum, WhS_Routing_SaleZoneRangeOptions, UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var directiveConstructor = new DirectiveConstructor($scope, ctrl);
                directiveConstructor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/ScheduleTask/Templates/RPBuildProductRoutesTaskTemplate.html"
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var defaultPolicy = null;

            var rpSettingsAddBlockedOptions;
             
            function initializeController() {
                $scope.supplierZoneRPOptionPolicies = [];

                $scope.onRoutingProductDatabaseTypeSelectionChanged = function () {
                    $scope.isFuture = $scope.selectedRoutingDatabaseType == WhS_Routing_RoutingDatabaseTypeEnum.Future;
                };

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                };

                $scope.onSelectCheckChanged = function (dataItem) {
                    if (!dataItem.isSelected && dataItem.IsDefault) {
                        dataItem.IsDefault = false;
                        defaultPolicy = null;
                    }
                };

                $scope.onDefaultCheckChanged = function (dataItem) {
                    angular.forEach($scope.supplierZoneRPOptionPolicies, function (item) {
                        if (item.ExtensionConfigurationId != dataItem.ExtensionConfigurationId) {
                            item.IsDefault = false;
                        }
                    });
                    if (dataItem.IsDefault)
                        defaultPolicy = dataItem;
                    else
                        defaultPolicy = null;
                };

                $scope.ValidatePolicies = function () {
                    if ($scope.supplierZoneRPOptionPolicies != undefined && $scope.supplierZoneRPOptionPolicies.length == 0)
                        return "Select at least one policy.";
                    else if (defaultPolicy == null)
                        return "Select at least one default policy.";
                    else if (defaultPolicy != null && !defaultPolicy.isSelected)
                        return "Default policy should be selected.";
                    return null;
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.getExpressionsData = function () {
                    return { "ScheduleTime": "ScheduleTime" };
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.BP.Arguments.RPRoutingProcessInput, TOne.WhS.Routing.BP.Arguments",
                        IsFuture: $scope.isFuture,
                        RoutingDatabaseType: $scope.selectedRoutingDatabaseType.value,
                        RoutingProcessType: WhS_Routing_RoutingProcessTypeEnum.RoutingProductRoute.value,
                        SaleZoneRange: $scope.selectedSaleZoneRange,
                        SupplierZoneRPOptionPolicies: GetSelectedSupplierPolicies(),
                        IncludeBlockedSupplierZones: $scope.includeBlockedSupplierZones,
                        EffectiveAfterInMinutes: $scope.effectiveAfterInMinutes
                    };
                };

                api.load = function (payload) {
                    var promises = [];

                    $scope.routingDatabaseTypes = UtilsService.getArrayEnum(WhS_Routing_RoutingDatabaseTypeEnum);
                    
                    $scope.saleZoneRangeOptions = WhS_Routing_SaleZoneRangeOptions;
                    if (payload != undefined && payload.data != undefined) {
                        $scope.selectedRoutingDatabaseType = UtilsService.getEnum(WhS_Routing_RoutingDatabaseTypeEnum, 'value', payload.data.RoutingDatabaseType);
                        $scope.selectedSaleZoneRange = payload.data.SaleZoneRange;
                        $scope.includeBlockedSupplierZones = payload.data.IncludeBlockedSupplierZones;
                        $scope.effectiveAfterInMinutes = payload.data.EffectiveAfterInMinutes;
                    }
                    else {
                        $scope.selectedRoutingDatabaseType = UtilsService.getEnum(WhS_Routing_RoutingDatabaseTypeEnum, 'value', WhS_Routing_RoutingDatabaseTypeEnum.Current.value);
                        $scope.selectedSaleZoneRange = WhS_Routing_SaleZoneRangeOptions[5];
                    }

                    var getPoliciesPromise = WhS_Routing_RPRouteAPIService.GetPoliciesOptionTemplates();
                    promises.push(getPoliciesPromise);

                    var loadPoliciesDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadPoliciesDeferred.promise);

                    getPoliciesPromise.then(function (response) {
                        var policyLoadPromises = [];

                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                var policy = response[i];
                                extendPolicy(policy);
                                policyLoadPromises.push(policy.directiveLoadDeferred.promise);
                                if (payload != undefined && payload.data != undefined) {
                                    fillPolicy(policy, payload.data.SupplierZoneRPOptionPolicies);
                                }
                                $scope.supplierZoneRPOptionPolicies.push(policy);
                            }
                        }

                        UtilsService.waitMultiplePromises(policyLoadPromises).then(function () {
                            loadPoliciesDeferred.resolve();
                        }).catch(function (error) {
                            loadPoliciesDeferred.reject(error);
                        });
                    });

                    function extendPolicy(policy) {
                        policy.directiveReadyDeferred = UtilsService.createPromiseDeferred();
                        policy.directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        policy.onDirectiveReady = function (api) {
                            policy.directiveAPI = api;
                            policy.directiveReadyDeferred.resolve();
                        };

                        policy.directiveReadyDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(policy.directiveAPI, undefined, policy.directiveLoadDeferred);
                        });
                    };

                    function fillPolicy(policy, supplierZoneRPOptionPolicies) {
                        var matchedPolicy = UtilsService.getItemByVal(supplierZoneRPOptionPolicies, policy.ExtensionConfigurationId, 'ConfigId');
                        if (matchedPolicy != undefined) {
                            policy.IsDefault = matchedPolicy.IsDefault;
                            if (policy.IsDefault) {
                                defaultPolicy = policy;
                            }
                            policy.isSelected = true;
                        }
                    };

                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function GetSelectedSupplierPolicies() {

                var policies = [];

                angular.forEach($scope.supplierZoneRPOptionPolicies, function (item) {
                    if (item.isSelected) {
                        var obj = item.directiveAPI.getData();
                        obj.ConfigId = item.ExtensionConfigurationId;
                        obj.IsDefault = item.IsDefault;
                        policies.push(obj);
                    }
                });
                return policies;
            }
        }

        return directiveDefinitionObject;
    }]);
