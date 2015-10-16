(function (appControllers) {

    "use strict";

    pricingRuleEditorController.$inject = ['$scope', 'WhS_BE_SalePricingRuleAPIService',  'UtilsService', 'VRNotificationService', 'VRNavigationService','WhS_Be_PricingRuleTypeEnum','WhS_BE_CarrierAccountAPIService','WhS_BE_SaleZoneAPIService','WhS_Be_PricingTypeEnum'];

    function pricingRuleEditorController($scope, WhS_BE_SalePricingRuleAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_Be_PricingRuleTypeEnum, WhS_BE_CarrierAccountAPIService, WhS_BE_SaleZoneAPIService, WhS_Be_PricingTypeEnum) {

        var editMode;
        var pricingType;
        var directiveAppendixData;
        var pricingRuleTypeDirectiveAPI;
        var saleZoneGroupSettingsDirectiveAPI;
        var customerGroupSettingsDirectiveAPI;
        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                pricingType = parameters.PricingType;
            }
            console.log(parameters);
            editMode = (pricingType != undefined);
        }

        function defineScope() {

            $scope.SavePricingRule = function () {
                console.log(pricingRuleTypeDirectiveAPI.getData());
                return;
                if (editMode) {
                    return updatePricingRule();
                }
                else {
                    return insertPricingRule();
                }
            };
            $scope.onPricingRuleTypeDirectiveReady = function (api) {
                pricingRuleTypeDirectiveAPI = api;
                api.load();
            }
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.selectedPricingRuleType;

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
                        }).finally(function () {
                            $scope.saleZonesAppendixLoader = false;
                        });
                    }
                }
            }

            $scope.onCustomerGroupSettingsDirectiveReady = function (api) {
                customerGroupSettingsDirectiveAPI = api;
                if (directiveAppendixData != undefined) {
                    tryLoadAppendixDirectives();
                } else {
                    var promise = customerGroupSettingsDirectiveAPI.load();
                    if (promise != undefined)
                    {
                        $scope.customersAppendixLoader = true;
                        promise.catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        }).finally(function () {
                            $scope.customersAppendixLoader = false;
                        });
                    }
                }
            }
            $scope.isCustomerShown = function () {
                if (pricingType.value == WhS_Be_PricingTypeEnum.Sale.value)
                    return true;
                return false;
            }

        }

        function load() {
            $scope.isGettingData = true;

            return UtilsService.waitMultipleAsyncOperations([loadSaleZoneGroupTemplates, loadCustomerGroupTemplates, definePricingRuleTypes]).then(function () {
               
                    $scope.isGettingData = false;
     

            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });

           
        }

        function definePricingRuleTypes() {
            $scope.pricingRuleTypes = [];
            for (var p in WhS_Be_PricingRuleTypeEnum)
                $scope.pricingRuleTypes.push(WhS_Be_PricingRuleTypeEnum[p]);
        }
        function getPricingRule() {
            return WhS_BE_SalePricingRuleAPIService.GetRule(pricingRuleId).then(function (pricingRule) {
                fillScopeFromPricingRuleObj(pricingRule);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });
        }


        function buildPricingRuleObjFromScope() {
            var pricingRule = {
            };

            return pricingRule;
        }

        function fillScopeFromPricingRuleObj(pricingRuleObj) {
        }

        function insertPricingRule() {
            var pricingRuleObject = buildPricingRuleObjFromScope();
            return WhS_BE_SalePricingRuleAPIService.AddRule(pricingRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Pricing Rule", response)) {
                    if ($scope.onPricingRuleAdded != undefined)
                        $scope.onPricingRuleAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updatePricingRule() {
            var pricingRuleObject = buildPricingRuleObjFromScope();
            WhS_BE_SalePricingRuleAPIService.UpdateRule(routeRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Pricing Rule", response)) {
                    if ($scope.onPricingRuleUpdated != undefined)
                        $scope.onPricingRuleUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
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
        function loadSaleZoneGroupSettings() {
            return saleZoneGroupSettingsDirectiveAPI.load();
        }

        function loadCustomerGroupSettings() {
            return customerGroupSettingsDirectiveAPI.load();
        }

      


    }

    appControllers.controller('WhS_BE_PricingRuleEditorController', pricingRuleEditorController);
})(appControllers);
