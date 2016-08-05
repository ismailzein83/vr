"use strict";

RPBuildProductRoutesProcessController.$inject = ['$scope', 'WhS_Routing_RPRouteAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService',
    'WhS_Routing_RoutingDatabaseTypeEnum', 'WhS_Routing_RoutingProcessTypeEnum', 'WhS_Routing_CodePrefixOptions', 'WhS_Routing_SaleZoneRangeOptions'];

function RPBuildProductRoutesProcessController($scope, WhS_Routing_RPRouteAPIService, VRNotificationService, UtilsService, VRUIUtilsService,
    WhS_Routing_RoutingDatabaseTypeEnum, WhS_Routing_RoutingProcessTypeEnum, WhS_Routing_CodePrefixOptions, WhS_Routing_SaleZoneRangeOptions) {

    var gridAPI;

    defineScope();

    load();

    function defineScope() {
        $scope.defaultPolicy = null;
        $scope.supplierZoneRPOptionPolicies = [];
        $scope.onRoutingProductDatabaseTypeSelectionChanged = function () {
            $scope.isFuture = $scope.selectedRoutingDatabaseType == WhS_Routing_RoutingDatabaseTypeEnum.Future;
        }

        $scope.createProcessInput.getData = function () {
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

        $scope.onGridReady = function (api) {
            gridAPI = api;
        }
    }

    function load() {

        return UtilsService.waitMultipleAsyncOperations([loadStaticData, loadSupplierPolicies]).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        }).finally(function () {
            $scope.isLoadingFilterData = false;
        });
    }

    function loadStaticData() {
        $scope.routingDatabaseTypes = UtilsService.getArrayEnum(WhS_Routing_RoutingDatabaseTypeEnum);
        $scope.selectedRoutingDatabaseType = UtilsService.getEnum(WhS_Routing_RoutingDatabaseTypeEnum, 'value', WhS_Routing_RoutingDatabaseTypeEnum.Current.value);

        $scope.codePrefixOptions = WhS_Routing_CodePrefixOptions;
        $scope.selectedCodePrefixOption = WhS_Routing_CodePrefixOptions[0];

        $scope.saleZoneRangeOptions = WhS_Routing_SaleZoneRangeOptions;
        $scope.selectedSaleZoneRange = WhS_Routing_SaleZoneRangeOptions[0];

        if (!$scope.isFuture)
            $scope.effectiveOn = new Date();
    }

    function loadSupplierPolicies() {
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
        return loadPoliciesPromiseDeffered.promise;
    }

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

    $scope.onSelectCheckChanged = function (dataItem) {
        if (!dataItem.isSelected && dataItem.IsDefault) {
            dataItem.IsDefault = false;
            $scope.defaultPolicy = null;
        }
    }

    $scope.onDefaultCheckChanged = function (dataItem) {
        angular.forEach($scope.supplierZoneRPOptionPolicies, function (item) {
            if (item.TemplateConfigID != dataItem.TemplateConfigID) {
                item.IsDefault = false;
            }
        });
        if (dataItem.IsDefault)
            $scope.defaultPolicy = dataItem;
        else
            $scope.defaultPolicy = null;
    }

    $scope.ValidatePolicies = function () {
        if ($scope.supplierZoneRPOptionPolicies != undefined && $scope.supplierZoneRPOptionPolicies.length == 0)
            return "Select at least one policy.";
        else if ($scope.defaultPolicy == null)
            return "Select at least one default policy.";
        else if ($scope.defaultPolicy != null && !$scope.defaultPolicy.isSelected)
            return "Default policy should be selected.";
        return null;
    };

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

appControllers.controller('WhS_Routing_RPBuildProductRoutesProcessController', RPBuildProductRoutesProcessController)