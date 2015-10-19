'use strict';
app.directive('vrWhsBeSalepricingrulecriteria', ['WhS_BE_SalePricingRuleAPIService', 'UtilsService', '$compile','WhS_BE_SaleZoneAPIService','WhS_BE_CarrierAccountAPIService','VRNotificationService',
function (WhS_BE_SalePricingRuleAPIService, UtilsService, $compile, WhS_BE_SaleZoneAPIService,WhS_BE_CarrierAccountAPIService,VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var beSalePricingRuleObject = new beSalePricingRule(ctrl, $scope, $attrs);
            beSalePricingRuleObject.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingRule/Templates/SalePricingRuleCriteriaTemplate.html"

    };


    function beSalePricingRule(ctrl, $scope, $attrs) {
        var saleZoneGroupSettingsDirectiveAPI;
        var customerGroupSettingsDirectiveAPI;
        var directiveAppendixData;

        function initializeController() {
            $scope.onCustomerGroupSettingsDirectiveReady = function (api) {
                customerGroupSettingsDirectiveAPI = api;
                if (directiveAppendixData != undefined) {
                    tryLoadAppendixDirectives();
                } else {
                    var promise = customerGroupSettingsDirectiveAPI.load();
                    if (promise != undefined) {
                        $scope.customersAppendixLoader = true;
                        promise.catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                            $scope.customersAppendixLoader = false;
                        }).finally(function () {
                            $scope.customersAppendixLoader = false;
                        });
                    }
                }

            }
            $scope.onSaleZoneGroupSettingsDirectiveReady = function (api) {
                saleZoneGroupSettingsDirectiveAPI = api;

                if (directiveAppendixData != undefined) {
                    tryLoadAppendixDirectives();
                } else {
                    var promise = saleZoneGroupSettingsDirectiveAPI.load();
                    if (promise != undefined) {
                        $scope.saleZonesAppendixLoader = true;
                        promise.catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                            $scope.saleZonesAppendixLoader = false;
                        }).finally(function () {
                           
                            $scope.saleZonesAppendixLoader = false;
                        });
                    }
                }
            }

            UtilsService.waitMultipleAsyncOperations([loadSaleZoneGroupTemplates, loadCustomerGroupTemplates]).then(function () {
                defineAPI();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj={
                    SaleZoneGroupSettings:saleZoneGroupSettingsDirectiveAPI.getData(),
                    CustomerGroupSettings:customerGroupSettingsDirectiveAPI.getData()
                    }
            return obj;
            }

            api.setData = function (selectedIds) {

                
            }
            api.load = function () {
            }
            
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadSaleZoneGroupTemplates() {
            $scope.saleZoneGroupTemplates = [];
            return WhS_BE_SaleZoneAPIService.GetSaleZoneGroupTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.saleZoneGroupTemplates.push(item);
                });
            });
        }

        function loadCustomerGroupTemplates() {
            $scope.customerGroupTemplates = [];
            return WhS_BE_CarrierAccountAPIService.GetCustomerGroupTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.customerGroupTemplates.push(item);
                });
            });
        }

        function tryLoadAppendixDirectives() {
            var loadOperations = [];
            var setDirectivesDataOperations = [];

            if ($scope.selectedSaleZoneGroupTemplate != undefined) {
                if (saleZoneGroupSettingsDirectiveAPI == undefined)
                    return;
                loadOperations.push(saleZoneGroupSettingsDirectiveAPI.load);

                setDirectivesDataOperations.push(setSaleZoneGroupSettingsDirective);
            }
            if ($scope.selectedSupplierGroupTemplate != undefined) {
                if (supplierGroupSettingsDirectiveAPI == undefined)
                    return;

                loadOperations.push(supplierGroupSettingsDirectiveAPI.load);

                setDirectivesDataOperations.push(setSupplierGroupSettingsDirective);
            }

            UtilsService.waitMultipleAsyncOperations(loadOperations).then(function () {

                setAppendixDirectives();

            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });

            function setAppendixDirectives() {
                UtilsService.waitMultipleAsyncOperations(setDirectivesDataOperations).then(function () {
                    directiveAppendixData = undefined;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.isGettingData = false;
                });
            }
        }

        function setSaleZoneGroupSettingsDirective() {
            return saleZoneGroupSettingsDirectiveAPI.setData(appendixData.RouteCriteria.SaleZoneGroupSettings);
        }

        function setCustomerGroupSettingsDirective() {
            return customerGroupSettingsDirectiveAPI.setData(appendixData.RouteCriteria.CustomerGroupSettings);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);