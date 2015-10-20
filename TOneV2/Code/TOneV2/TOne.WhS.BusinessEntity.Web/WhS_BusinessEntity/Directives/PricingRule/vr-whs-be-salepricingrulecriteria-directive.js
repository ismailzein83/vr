'use strict';
app.directive('vrWhsBeSalepricingrulecriteria', ['WhS_BE_SalePricingRuleAPIService', 'UtilsService', '$compile','WhS_BE_SaleZoneAPIService','WhS_BE_CarrierAccountAPIService','VRNotificationService','VRUIUtilsService',
function (WhS_BE_SalePricingRuleAPIService, UtilsService, $compile, WhS_BE_SaleZoneAPIService, WhS_BE_CarrierAccountAPIService, VRNotificationService, VRUIUtilsService) {

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
        var directiveAppendixData ;

        function initializeController() {
            $scope.onCustomerGroupSettingsDirectiveReady = function (api) {
                customerGroupSettingsDirectiveAPI = api;
                if (directiveAppendixData != undefined) {
                    tryLoadAppendixDirectives();
                } else {
                    $scope.customersAppendixLoader;
                    VRUIUtilsService.loadDirective($scope, customerGroupSettingsDirectiveAPI, $scope.customersAppendixLoader);
                }

            }
            $scope.onSaleZoneGroupSettingsDirectiveReady = function (api) {
                saleZoneGroupSettingsDirectiveAPI = api;

                if (directiveAppendixData != undefined) {
                    tryLoadAppendixDirectives();
                } else {
                    $scope.saleZonesAppendixLoader;
                    VRUIUtilsService.loadDirective($scope, saleZoneGroupSettingsDirectiveAPI, $scope.saleZonesAppendixLoader);
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
                var saleZoneGroupSettings;
                if (saleZoneGroupSettingsDirectiveAPI != undefined) {
                    saleZoneGroupSettings = saleZoneGroupSettingsDirectiveAPI.getData();
                    saleZoneGroupSettings.ConfigId = $scope.selectedSaleZoneGroupTemplate.TemplateConfigID;
                }
                   
                var customerGroupSettings;
                if (customerGroupSettingsDirectiveAPI != undefined)
                {
                    customerGroupSettings = customerGroupSettingsDirectiveAPI.getData();
                    customerGroupSettings.ConfigId = $scope.selectedCustomerGroupTemplate.TemplateConfigID;
                }
                var obj={
                    SaleZoneGroupSettings: saleZoneGroupSettings,
                    CustomerGroupSettings: customerGroupSettings
                    }
            return obj;
            }

            api.setData = function (criteria) {
                if (criteria.SaleZoneGroupSettings!=null)
                    $scope.selectedSaleZoneGroupTemplate = UtilsService.getItemByVal($scope.saleZoneGroupTemplates, criteria.SaleZoneGroupSettings.ConfigId, "TemplateConfigID")
                if (criteria.CustomerGroupSettings != null)
                  $scope.selectedCustomerGroupTemplate = UtilsService.getItemByVal($scope.customerGroupTemplates, criteria.CustomerGroupSettings.ConfigId, "TemplateConfigID")
                directiveAppendixData = {
                    SaleZoneGroupSettings: criteria.SaleZoneGroupSettings,
                    CustomerGroupSettings: criteria.CustomerGroupSettings
                };
                tryLoadAppendixDirectives();
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
            if ($scope.selectedCustomerGroupTemplate != undefined) {
                if (customerGroupSettingsDirectiveAPI == undefined)
                    return;

                loadOperations.push(customerGroupSettingsDirectiveAPI.load);

                setDirectivesDataOperations.push(setCustomerGroupSettingsDirective);
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
            function setSaleZoneGroupSettingsDirective() {
                return saleZoneGroupSettingsDirectiveAPI.setData(directiveAppendixData.SaleZoneGroupSettings);
            }

            function setCustomerGroupSettingsDirective() {
                return customerGroupSettingsDirectiveAPI.setData(directiveAppendixData.CustomerGroupSettings);
            }
        }

        

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);