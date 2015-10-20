(function (appControllers) {

    "use strict";

    routeRuleEditorController.$inject = ['$scope', 'WhS_BE_RouteRuleAPIService', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_SaleZoneAPIService', 'WhS_BE_CarrierAccountAPIService',
        'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function routeRuleEditorController($scope, WhS_BE_RouteRuleAPIService, WhS_BE_RoutingProductAPIService, WhS_BE_SaleZoneAPIService, WhS_BE_CarrierAccountAPIService,
        UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var editMode;
        var routeRuleId;

        var directiveAppendixData;

        var routingProductDirectiveAPI;
        var saleZoneGroupSettingsDirectiveAPI;
        var customerGroupSettingsDirectiveAPI;
        var codeCriteriaGroupSettingsDirectiveAPI;

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

            $scope.onRoutingProductDirectiveReady = function (api) {
                routingProductDirectiveAPI = api;
                load();
            }

            $scope.onSaleZoneGroupSettingsDirectiveReady = function (api) {
                saleZoneGroupSettingsDirectiveAPI = api;

                if (directiveAppendixData != undefined)
                    tryLoadAppendixDirectives();
                else
                    VRUIUtilsService.loadDirective($scope, saleZoneGroupSettingsDirectiveAPI, 'saleZonesAppendixLoader');
            }

            $scope.onCustomerGroupSettingsDirectiveReady = function (api) {
                customerGroupSettingsDirectiveAPI = api;

                if (directiveAppendixData != undefined)
                    tryLoadAppendixDirectives();
                else
                    VRUIUtilsService.loadDirective($scope, customerGroupSettingsDirectiveAPI, 'customersAppendixLoader');
            }

            $scope.onCodeCriteriaGroupSettingsDirectiveReady = function (api) {
                codeCriteriaGroupSettingsDirectiveAPI = api;

                if (directiveAppendixData != undefined)
                    tryLoadAppendixDirectives();
                else
                    VRUIUtilsService.loadDirective($scope, codeCriteriaGroupSettingsDirectiveAPI, 'codeCriteriaAppendixLoader');
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

            $scope.onRoutingProductSelectionChanged = function () {
                if (routingProductDirectiveAPI != undefined) {
                    var routingProductObj = routingProductDirectiveAPI.getData();
                    if (routingProductObj != undefined)
                        $scope.selectedSaleZonePackageId = routingProductDirectiveAPI.getData().SaleZonePackageId;
                }
                else {
                    $scope.selectedSaleZonePackageId = undefined;
                }
            }

            $scope.saleZoneGroupTemplates = [];
            $scope.customerGroupTemplates = [];
            $scope.codeCriteriaGroupTemplates = [];
            $scope.routingProducts = [];

            $scope.excludedCodes = [];

            $scope.beginEffectiveDate = new Date();
            $scope.endEffectiveDate = undefined;
        }

        function load() {
            $scope.isGettingData = true;

            if (routingProductDirectiveAPI == undefined)
                return;

            return UtilsService.waitMultipleAsyncOperations([routingProductDirectiveAPI.load, loadSaleZoneGroupTemplates, loadCustomerGroupTemplates, loadCodeCriteriaGroupTemplates]).then(function () {
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
            return WhS_BE_RouteRuleAPIService.GetRule(routeRuleId).then(function (routeRule) {
                fillScopeFromRouteRuleObj(routeRule);
                directiveAppendixData = routeRule;
                tryLoadAppendixDirectives();

            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });
        }

        function loadSaleZoneGroupTemplates() {
            return WhS_BE_SaleZoneAPIService.GetSaleZoneGroupTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.saleZoneGroupTemplates.push(item);
                });
            });
        }

        function loadCustomerGroupTemplates() {
            return WhS_BE_CarrierAccountAPIService.GetCustomerGroupTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.customerGroupTemplates.push(item);
                });
            });
        }

        function loadCodeCriteriaGroupTemplates() {
            return WhS_BE_RouteRuleAPIService.GetCodeCriteriaGroupTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.codeCriteriaGroupTemplates.push(item);
                });
            });
        }

        function tryLoadAppendixDirectives() {
            
            var loadOperations = [];
            var setDirectivesDataOperations = [];

            if($scope.selectedSaleZoneGroupTemplate != undefined)
            {
                if(saleZoneGroupSettingsDirectiveAPI == undefined)
                    return;

                loadOperations.push(saleZoneGroupSettingsDirectiveAPI.load);
                setDirectivesDataOperations.push(setSaleZoneGroupSettingsDirective);
            }

            if($scope.selectedCustomerGroupTemplate != undefined)
            {
                if(customerGroupSettingsDirectiveAPI == undefined)
                    return;

                loadOperations.push(customerGroupSettingsDirectiveAPI.load);
                setDirectivesDataOperations.push(setCustomerGroupSettingsDirective);
            }
            
            if($scope.selectedCodeCriteriaGroupTemplate != undefined)
            {
                if(codeCriteriaGroupSettingsDirectiveAPI == undefined)
                    return;

                loadOperations.push(codeCriteriaGroupSettingsDirectiveAPI.load);
                setDirectivesDataOperations.push(setCodeCriteriaGroupSettingsDirective);
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
                return saleZoneGroupSettingsDirectiveAPI.setData(directiveAppendixData.RouteCriteria.SaleZoneGroupSettings);
            }

            function setCustomerGroupSettingsDirective() {
                return customerGroupSettingsDirectiveAPI.setData(directiveAppendixData.RouteCriteria.CustomerGroupSettings);
            }

            function setCodeCriteriaGroupSettingsDirective() {
                return codeCriteriaGroupSettingsDirectiveAPI.setData(directiveAppendixData.RouteCriteria.CodeCriteriaGroupSettings);
            }
        }

        function buildRouteRuleObjFromScope() {
            var routeRule = {
                RuleId: (routeRuleId != null) ? routeRuleId : 0,
                Criteria: {
                    RoutingProductId: $scope.selectedRoutingProduct != undefined ? $scope.selectedRoutingProduct.RoutingProductId : null,
                    ExcludedCodes: $scope.excludedCodes,
                    SaleZoneGroupSettings: getSaleZoneGroupSettings(),
                    CustomerGroupSettings: getCustomersGroupSettings(),
                    CodeCriteriaGroupSettings: getCodeCriteriaGroupSettings()
                },
                BeginEffectiveTime: $scope.beginEffectiveDate,
                EndEffectiveTime: $scope.endEffectiveDate
            };

            return routeRule;
        }

        function fillScopeFromRouteRuleObj(routeRuleObj) {
            if (routeRuleObj.RouteCriteria != null) {
                if (routeRuleObj.RouteCriteria.RoutingProductId != null)
                    routingProductDirectiveAPI.setData(routeRuleObj.RouteCriteria.RoutingProductId);

                angular.forEach(routeRuleObj.RouteCriteria.ExcludedCodes, function (item) {
                    $scope.excludedCodes.push(item);
                });

                if (routeRuleObj.RouteCriteria.SaleZoneGroupSettings != null)
                    $scope.selectedSaleZoneGroupTemplate = UtilsService.getItemByVal($scope.saleZoneGroupTemplates, routeRuleObj.RouteCriteria.SaleZoneGroupSettings.ConfigId, "TemplateConfigID");

                if (routeRuleObj.RouteCriteria.CustomerGroupSettings != null)
                    $scope.selectedCustomerGroupTemplate = UtilsService.getItemByVal($scope.customerGroupTemplates, routeRuleObj.RouteCriteria.CustomerGroupSettings.ConfigId, "TemplateConfigID");

                if (routeRuleObj.RouteCriteria.CodeCriteriaGroupSettings != null)
                    $scope.selectedCodeCriteriaGroupTemplate = UtilsService.getItemByVal($scope.codeCriteriaGroupTemplates, routeRuleObj.RouteCriteria.CodeCriteriaGroupSettings.ConfigId, "TemplateConfigID");
            }
            
            $scope.beginEffectiveDate = routeRuleObj.BeginEffectiveTime;
            $scope.endEffectiveDate = routeRuleObj.endEffectiveTime;
        }

        function insertRouteRule() {
            var routeRuleObject = buildRouteRuleObjFromScope();
            return WhS_BE_RouteRuleAPIService.AddRule(routeRuleObject)
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
            WhS_BE_RouteRuleAPIService.UpdateRule(routeRuleObject)
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

        function getSaleZoneGroupSettings() {
            if ($scope.selectedSaleZoneGroupTemplate != undefined) {
                var settings = saleZoneGroupSettingsDirectiveAPI.getData();
                settings.ConfigId = $scope.selectedSaleZoneGroupTemplate.TemplateConfigID;
                return settings;
            }
            else
                return null;
        }

        function getCustomersGroupSettings() {
            if ($scope.selectedCustomerGroupTemplate != undefined) {
                var settings = customerGroupSettingsDirectiveAPI.getData();
                settings.ConfigId = $scope.selectedCustomerGroupTemplate.TemplateConfigID;
                return settings;
            }
            else
                return null;
        }

        function getCodeCriteriaGroupSettings() {
            if ($scope.selectedCodeCriteriaGroupTemplate != undefined) {
                var settings = codeCriteriaGroupSettingsDirectiveAPI.getData();
                settings.ConfigId = $scope.selectedCodeCriteriaGroupTemplate.TemplateConfigID;
                return settings;
            }
            else
                return null;
        }

    }

    appControllers.controller('WhS_BE_RouteRuleEditorController', routeRuleEditorController);
})(appControllers);
