"use strict";

app.directive("vrWhsRoutingRpbuildproduct", [ 'WhS_Routing_RPRouteAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'WhS_Routing_RoutingDatabaseTypeEnum', 'WhS_Routing_RoutingProcessTypeEnum', 'WhS_Routing_CodePrefixOptions', 'WhS_Routing_SaleZoneRangeOptions',
    function (WhS_Routing_RPRouteAPIService, VRNotificationService, UtilsService, VRUIUtilsService, WhS_Routing_RoutingDatabaseTypeEnum, WhS_Routing_RoutingProcessTypeEnum, WhS_Routing_CodePrefixOptions, WhS_Routing_SaleZoneRangeOptions) {
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
            }
        },
        templateUrl: "/Client/Modules/WhS_Routing/Directives/ProcessInput/Templates/RPBuildProductRoutesProcessTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        var gridAPI;
        this.initializeController = initializeController;
        var defaultPolicy = null;
        function initializeController() {
            $scope.supplierZoneRPOptionPolicies = [];

            $scope.onRoutingProductDatabaseTypeSelectionChanged = function () {
                $scope.isFuture = $scope.selectedRoutingDatabaseType == WhS_Routing_RoutingDatabaseTypeEnum.Future;
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
            }

            $scope.onSelectCheckChanged = function (dataItem) {
                if (!dataItem.isSelected && dataItem.IsDefault) {
                    dataItem.IsDefault = false;
                    defaultPolicy = null;
                }
            }

            $scope.onDefaultCheckChanged = function (dataItem) {
                angular.forEach($scope.supplierZoneRPOptionPolicies, function (item) {
                    if (item.TemplateConfigID != dataItem.TemplateConfigID) {
                        item.IsDefault = false;
                    }
                });
                if (dataItem.IsDefault)
                    defaultPolicy = dataItem;
                else
                    defaultPolicy = null;
            }

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
            api.getData = function () {
                return {
                    InputArguments: {
                        $type: "TOne.WhS.Routing.BP.Arguments.RPRoutingProcessInput, TOne.WhS.Routing.BP.Arguments",
                        EffectiveOn: !$scope.isFuture ? $scope.effectiveOn : null,
                        IsFuture: $scope.isFuture,
                        RoutingDatabaseType: $scope.selectedRoutingDatabaseType.value,
                        RoutingProcessType: WhS_Routing_RoutingProcessTypeEnum.RoutingProductRoute.value,
                        CodePrefixLength: $scope.selectedCodePrefixOption,
                        SaleZoneRange: $scope.selectedSaleZoneRange,
                        SupplierZoneRPOptionPolicies: GetSelectedSupplierPolicies()
                    }
                };
               
            };

            api.load = function (payload) {

                $scope.routingDatabaseTypes = UtilsService.getArrayEnum(WhS_Routing_RoutingDatabaseTypeEnum);
                $scope.selectedRoutingDatabaseType = UtilsService.getEnum(WhS_Routing_RoutingDatabaseTypeEnum, 'value', WhS_Routing_RoutingDatabaseTypeEnum.Current.value);

                $scope.codePrefixOptions = WhS_Routing_CodePrefixOptions;
                $scope.selectedCodePrefixOption = WhS_Routing_CodePrefixOptions[1];

                $scope.saleZoneRangeOptions = WhS_Routing_SaleZoneRangeOptions;
                $scope.selectedSaleZoneRange = WhS_Routing_SaleZoneRangeOptions[0];

              

                var loadPoliciesPromiseDeffered = UtilsService.createPromiseDeferred();
                WhS_Routing_RPRouteAPIService.GetPoliciesOptionTemplates().then(function (response) {

                    var promises = [];
                    angular.forEach(response, function (itm) {
                        promises.push(setPoliciesDirectives(itm));
                        $scope.supplierZoneRPOptionPolicies.push(itm);
                    });

                    UtilsService.waitMultiplePromises(promises).then(function () {
                        loadPoliciesPromiseDeffered.resolve();
                    }).catch(function (error) {
                        loadPoliciesPromiseDeffered.reject();
                    });

                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });

                function setPoliciesDirectives(poilicy) {

                    poilicy.rpSupplierPoliciesReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    poilicy.onDirectiveReady = function (api) {
                        poilicy.rpSupplierPoliciesDirectiveApi = api;
                        poilicy.rpSupplierPoliciesReadyPromiseDeferred.resolve();
                    }
                    poilicy.rpSupplierPoliciesLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    poilicy.rpSupplierPoliciesReadyPromiseDeferred.promise.then(function () {

                        VRUIUtilsService.callDirectiveLoad(poilicy.rpSupplierPoliciesDirectiveApi, undefined, poilicy.rpSupplierPoliciesLoadPromiseDeferred);
                    });

                    return poilicy.rpSupplierPoliciesLoadPromiseDeferred.promise;
                }


                return loadPoliciesPromiseDeffered.promise;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function GetSelectedSupplierPolicies() {

            var policies = [];

            angular.forEach($scope.supplierZoneRPOptionPolicies, function (item) {
                if (item.isSelected) {
                    var obj = item.rpSupplierPoliciesDirectiveApi.getData();
                    obj.ConfigId = item.TemplateConfigID;
                    obj.IsDefault = item.IsDefault;
                    policies.push(obj);
                }
            });
            return policies;
        }
    }

    return directiveDefinitionObject;
}]);
