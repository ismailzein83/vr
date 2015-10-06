(function (appControllers) {

    "use strict";

    routeRuleEditorController.$inject = ['$scope', 'WhS_BE_RouteRuleAPIService', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_SaleZoneAPIService', 'WhS_BE_CarrierAccountAPIService',
        'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function routeRuleEditorController($scope, WhS_BE_RouteRuleAPIService, WhS_BE_RoutingProductAPIService, WhS_BE_SaleZoneAPIService, WhS_BE_CarrierAccountAPIService,
        UtilsService, VRNotificationService, VRNavigationService) {

            var editMode;
            var routeRuleId;

            var saleZoneGroupSettingsDirectiveAPI;
            var saleZoneGroupSettings;

            var customerGroupSettingsDirectiveAPI;
            var customerGroupSettings;

            var codeCriteriaGroupSettingsDirectiveAPI;
            var codeCriteriaGroupSettings;

            loadParameters();
            defineScope();
            load();

            function loadParameters() {
                var parameters = VRNavigationService.getParameters($scope);

                if (parameters != undefined && parameters != null) {
                    routeRuleId = parameters.routeRuleId;
                }
                editMode = (routeRuleId != undefined);
            }

            function defineScope() {
                $scope.onSaleZoneGroupSettingsDirectiveLoaded = function (api) {
                    saleZoneGroupSettingsDirectiveAPI = api;

                    if (saleZoneGroupSettings != undefined)
                    {
                        saleZoneGroupSettingsDirectiveAPI.setData(saleZoneGroupSettings);
                        saleZoneGroupSettings = undefined;
                    }
                }

                $scope.onCustomerGroupSettingsDirectiveLoaded = function (api) {
                    customerGroupSettingsDirectiveAPI = api;

                    if (customerGroupSettings != undefined) {
                        customerGroupSettingsDirectiveAPI.setData(customerGroupSettings);
                        customerGroupSettings = undefined;
                    }
                }

                $scope.onCodeCriteriaGroupSettingsDirectiveLoaded = function (api) {
                    codeCriteriaGroupSettingsDirectiveAPI = api;

                    if (codeCriteriaGroupSettings != undefined) {
                        codeCriteriaGroupSettingsDirectiveAPI.setData(codeCriteriaGroupSettings);
                        codeCriteriaGroupSettings = undefined;
                    }
                }

                $scope.SaveRouteRule = function () {
                    if (editMode) {
                        return updateRouteRule();
                    }
                    else {
                        return insertRouteRule();
                    }
                };

                //TODO: make the validation below a custom validation
                $scope.addExcludedCode = function () {
                    var codeIsValid = true;

                    if ($scope.excludedCode == undefined || $scope.excludedCode.length == 0) {
                        codeIsValid = false;
                    }
                    else {
                        angular.forEach($scope.excludedCodes, function (item) {
                            if ($scope.excludedCode === item) {
                                codeIsValid = false;
                            }
                        });
                    }

                    if (codeIsValid)
                        $scope.excludedCodes.push($scope.excludedCode);
                }

                $scope.removeExcludedCode = function (codeToRemove) {
                    $scope.excludedCodes.splice($scope.excludedCodes.indexOf(codeToRemove), 1);
                }

                $scope.close = function () {
                    $scope.modalContext.closeModal()
                };

                $scope.routingProductsOnSelectionChanged = function () {
                    if ($scope.selectedSaleZonePackage != undefined) {
                        //Hide the section of customer and code
                    }
                }

                $scope.saleZoneGroupTemplates = [];
                $scope.selectedSaleZoneGroupTemplate = undefined;

                $scope.customerGroupTemplates = [];
                $scope.selectedCustomerGroupTemplate = undefined;

                $scope.codeCriteriaGroupTemplates = [];
                $scope.selectedCodeCriteriaGroupTemplate = undefined;

                $scope.routingProducts = [];
                $scope.selectedRoutingProduct = undefined;

                $scope.excludedCodes = [];
            }

            function load() {
                $scope.isGettingData = true;
                return UtilsService.waitMultipleAsyncOperations([loadRoutingProducts, loadSaleZoneGroupTemplates, loadCustomerGroupTemplates, loadCodeCriteriaGroupTemplates]).then(function () {
                    if (editMode) {
                        getRouteRule();
                    }
                    else {
                        $scope.isGettingData = false;
                    }

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isGettingData = false;
                });
            }

            function getRouteRule() {
                return WhS_BE_RouteRuleAPIService.GetRouteRule(routeRuleId).then(function (routeRule) {
                    fillScopeFromRouteRuleObj(routeRule);

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.isGettingData = false;
                });
            }

            function loadRoutingProducts() {
                return WhS_BE_RoutingProductAPIService.GetRoutingProducts().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.routingProducts.push(item);
                    });
                });
            }

            function loadSaleZoneGroupTemplates() {
                return WhS_BE_SaleZoneAPIService.GetSaleZoneGroupTemplates().then(function (response) {

                    var defSaleZoneSelection = { TemplateConfigID: -1, Name: 'No Filter', Editor: '' };
                    $scope.saleZoneGroupTemplates.push(defSaleZoneSelection);

                    angular.forEach(response, function (item) {
                        $scope.saleZoneGroupTemplates.push(item);
                    });

                    $scope.selectedSaleZoneGroupTemplate = defSaleZoneSelection;
                });
            }

            function loadCustomerGroupTemplates() {
                return WhS_BE_CarrierAccountAPIService.GetCustomerGroupTemplates().then(function (response) {

                    var defCustomerSelection = { TemplateConfigID: -1, Name: 'No Filter', Editor: '' };
                    $scope.customerGroupTemplates.push(defCustomerSelection);

                    angular.forEach(response, function (item) {
                        $scope.customerGroupTemplates.push(item);
                    });

                    $scope.selectedCustomerGroupTemplate = defCustomerSelection;
                });
            }

            function loadCodeCriteriaGroupTemplates() {
                return WhS_BE_RouteRuleAPIService.GetCodeCriteriaGroupTemplates().then(function (response) {

                    var defCodeCriteriaSelection = { TemplateConfigID: -1, Name: 'No Filter', Editor: '' };
                    $scope.codeCriteriaGroupTemplates.push(defCodeCriteriaSelection);

                    angular.forEach(response, function (item) {
                        $scope.codeCriteriaGroupTemplates.push(item);
                    });

                    $scope.selectedCodeCriteriaGroupTemplate = defCodeCriteriaSelection;
                });
            }

            function buildRouteRuleObjFromScope() {
                var routeRule = {
                    RouteRuleId: (routeRuleId != null) ? routeRuleId : 0,
                    RouteCriteria: {
                        RoutingProductId: $scope.selectedRoutingProduct != undefined ? $scope.selectedRoutingProduct.RoutingProductId : null,
                        ExcludedCodes: $scope.excludedCodes,
                        SaleZoneGroupConfigId: $scope.selectedSaleZoneGroupTemplate.TemplateConfigID != -1 ? $scope.selectedSaleZoneGroupTemplate.TemplateConfigID : null,
                        SaleZoneGroupSettings: $scope.selectedSaleZoneGroupTemplate.TemplateConfigID != -1 ? saleZoneGroupSettingsDirectiveAPI.getData() : null,
                        CustomersGroupConfigId: $scope.selectedCustomerGroupTemplate.TemplateConfigID != -1 ? $scope.selectedCustomerGroupTemplate.TemplateConfigID : null,
                        CustomerGroupSettings: $scope.selectedCustomerGroupTemplate.TemplateConfigID != -1 ? customerGroupSettingsDirectiveAPI.getData() : null,
                        CodeCriteriaGroupId: $scope.selectedCodeCriteriaGroupTemplate.TemplateConfigID != -1 ? $scope.selectedCodeCriteriaGroupTemplate.TemplateConfigID : null,
                        CodeCriteriaGroupSettings: $scope.selectedCodeCriteriaGroupTemplate.TemplateConfigID != -1 ? codeCriteriaGroupSettingsDirectiveAPI.getData() : null
                    }
                };

                return routeRule;
            }

            function fillScopeFromRouteRuleObj(routeRuleObj) {

                if (routeRuleObj.RouteCriteria.RoutingProductId != null)
                    $scope.selectedRoutingProduct = UtilsService.getItemByVal($scope.routingProducts, routeRuleObj.RouteCriteria.RoutingProductId, "RoutingProductId");

                angular.forEach(routeRuleObj.RouteCriteria.ExcludedCodes, function (item) {
                    $scope.excludedCodes.push(item);
                });

                if (routeRuleObj.RouteCriteria.SaleZoneGroupConfigId != null)
                {
                    $scope.selectedSaleZoneGroupTemplate = UtilsService.getItemByVal($scope.saleZoneGroupTemplates, routeRuleObj.RouteCriteria.SaleZoneGroupConfigId, "TemplateConfigID");
                    saleZoneGroupSettings = routeRuleObj.RouteCriteria.SaleZoneGroupSettings;
                }

                if (routeRuleObj.RouteCriteria.CustomersGroupConfigId != null) {
                    $scope.selectedCustomerGroupTemplate = UtilsService.getItemByVal($scope.customerGroupTemplates, routeRuleObj.RouteCriteria.CustomersGroupConfigId, "TemplateConfigID");
                    customerGroupSettings = routeRuleObj.RouteCriteria.CustomerGroupSettings;
                }

                if (routeRuleObj.RouteCriteria.CodeCriteriaGroupId != null) {
                    $scope.selectedCodeCriteriaGroupTemplate = UtilsService.getItemByVal($scope.codeCriteriaGroupTemplates, routeRuleObj.RouteCriteria.CodeCriteriaGroupId, "TemplateConfigID");
                    codeCriteriaGroupSettings = routeRuleObj.RouteCriteria.CodeCriteriaGroupSettings;
                }
            }

            function insertRouteRule() {
                var routeRuleObject = buildRouteRuleObjFromScope();
                return WhS_BE_RouteRuleAPIService.AddRouteRule(routeRuleObject)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Route Rule", response)) {
                        if ($scope.onRouteRuleAdded != undefined)
                            $scope.onRouteRuleAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });

            }

            function updateRouteRule() {
                var routeRuleObject = buildRouteRuleObjFromScope();
                WhS_BE_RouteRuleAPIService.UpdateRouteRule(routeRuleObject)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Route Rule", response)) {
                        if ($scope.onRouteRuleUpdated != undefined)
                            $scope.onRouteRuleUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            }

    }

    appControllers.controller('WhS_BE_RouteRuleEditorController', routeRuleEditorController);
})(appControllers);
