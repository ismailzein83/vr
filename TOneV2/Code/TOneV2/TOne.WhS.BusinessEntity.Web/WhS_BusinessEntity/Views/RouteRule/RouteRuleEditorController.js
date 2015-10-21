(function (appControllers) {

    "use strict";

    routeRuleEditorController.$inject = ['$scope', 'WhS_BE_RouteRuleAPIService', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_SaleZoneAPIService', 'WhS_BE_CarrierAccountAPIService',
        'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_Be_RouteRuleCriteriaTypeEnum'];

    function routeRuleEditorController($scope, WhS_BE_RouteRuleAPIService, WhS_BE_RoutingProductAPIService, WhS_BE_SaleZoneAPIService, WhS_BE_CarrierAccountAPIService,
        UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_Be_RouteRuleCriteriaTypeEnum) {

        var editMode;
        var routeRuleId;

        var appendixDirectiveData;

        var routingProductDirectiveAPI;
        var saleZoneGroupSettingsDirectiveAPI;
        var customerGroupSettingsDirectiveAPI;
        var codeCriteriaGroupSettingsDirectiveAPI;
        var routeRuleSettingsDirectiveAPI;

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

                if (appendixDirectiveData != undefined)
                    tryLoadAppendixDirectives();
                else
                    VRUIUtilsService.loadDirective($scope, saleZoneGroupSettingsDirectiveAPI, 'saleZonesAppendixLoader');
            }

            $scope.onCustomerGroupSettingsDirectiveReady = function (api) {
                customerGroupSettingsDirectiveAPI = api;

                if (appendixDirectiveData != undefined)
                    tryLoadAppendixDirectives();
                else
                    VRUIUtilsService.loadDirective($scope, customerGroupSettingsDirectiveAPI, 'customersAppendixLoader');
            }

            $scope.onCodeCriteriaGroupSettingsDirectiveReady = function (api) {
                codeCriteriaGroupSettingsDirectiveAPI = api;

                if (appendixDirectiveData != undefined)
                    tryLoadAppendixDirectives();
                else
                    VRUIUtilsService.loadDirective($scope, codeCriteriaGroupSettingsDirectiveAPI, 'codeCriteriaAppendixLoader');
            }

            $scope.onRouteRuleSettingsDirectiveReady = function (api) {
                routeRuleSettingsDirectiveAPI = api;

                if (appendixDirectiveData != undefined)
                    tryLoadAppendixDirectives();
                else
                    VRUIUtilsService.loadDirective($scope, routeRuleSettingsDirectiveAPI, 'routeRuleSettingsAppendixLoader');
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
                    {
                        $scope.selectedSaleZonePackageId = routingProductDirectiveAPI.getData().SaleZonePackageId;
                        $scope.showSaleZoneSection = true;
                        $scope.showRouteRuleCriteriaTypes = $scope.showCustomerSection = $scope.showExecludedCodeSection = $scope.showIncludedCodeSection = false;
                    }
                    else
                    {
                        $scope.showSaleZoneSection = false;
                        $scope.showRouteRuleCriteriaTypes = $scope.showCustomerSection = $scope.showExecludedCodeSection = $scope.showIncludedCodeSection = true;
                    }
                }
                else {
                    $scope.selectedSaleZonePackageId = undefined;
                }
            }

            $scope.onRouteRuleCriteriaTypeSelectionChanged = function () {
                if ($scope.selectedRouteRuleCriteriaType == WhS_Be_RouteRuleCriteriaTypeEnum.SaleZone)
                {
                    $scope.showSaleZoneSection = $scope.showCustomerSection = $scope.showExecludedCodeSection = true;
                    $scope.showIncludedCodeSection = false;
                }
                else
                {
                    $scope.showIncludedCodeSection = $scope.showCustomerSection = $scope.showExecludedCodeSection = true;
                    $scope.showSaleZoneSection = false;
                }
            }

            $scope.saleZoneGroupTemplates = [];
            $scope.customerGroupTemplates = [];
            $scope.codeCriteriaGroupTemplates = [];
            $scope.routingProducts = [];
            $scope.routeRuleSettingsTemplates = [];

            $scope.excludedCodes = [];

            $scope.beginEffectiveDate = new Date();
            $scope.endEffectiveDate = undefined;
        }

        function load() {
            $scope.isGettingData = true;

            if (routingProductDirectiveAPI == undefined)
                return;

            loadEnums();

            return UtilsService.waitMultipleAsyncOperations([routingProductDirectiveAPI.load, loadSaleZoneGroupTemplates, loadCustomerGroupTemplates, loadCodeCriteriaGroupTemplates, loadRouteRuleSettingsTemplates]).then(function () {
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

        function loadEnums()
        {
            $scope.routeRuleCriteriaTypes = UtilsService.getArrayEnum(WhS_Be_RouteRuleCriteriaTypeEnum);
            $scope.selectedRouteRuleCriteriaType = UtilsService.getEnum(WhS_Be_RouteRuleCriteriaTypeEnum, 'value', 'SaleZone');
        }

        function getRouteRule() {
            return WhS_BE_RouteRuleAPIService.GetRule(routeRuleId).then(function (routeRule) {
                appendixDirectiveData = routeRule;
                fillScopeFromRouteRuleObj(routeRule);
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

        function loadRouteRuleSettingsTemplates()
        {
            return WhS_BE_RouteRuleAPIService.GetRouteRuleSettingsTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.routeRuleSettingsTemplates.push(item);
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

            if ($scope.selectedrouteRuleSettingsTemplate != undefined) {
                if (routeRuleSettingsDirectiveAPI == undefined)
                    return;

                loadOperations.push(routeRuleSettingsDirectiveAPI.load);
                setDirectivesDataOperations.push(setRouteRuleSettingsDirective);
            }

            UtilsService.waitMultipleAsyncOperations(loadOperations).then(function () {

                setAppendixDirectives();

            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });

            function setAppendixDirectives() {
                UtilsService.waitMultipleAsyncOperations(setDirectivesDataOperations).then(function () {

                    appendixDirectiveData = undefined;

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.isGettingData = false;
                });
            }

            function setSaleZoneGroupSettingsDirective() {
                return saleZoneGroupSettingsDirectiveAPI.setData(appendixDirectiveData.Criteria.SaleZoneGroupSettings);
            }

            function setCustomerGroupSettingsDirective() {
                return customerGroupSettingsDirectiveAPI.setData(appendixDirectiveData.Criteria.CustomerGroupSettings);
            }

            function setCodeCriteriaGroupSettingsDirective() {
                return codeCriteriaGroupSettingsDirectiveAPI.setData(appendixDirectiveData.Criteria.CodeCriteriaGroupSettings);
            }

            function setRouteRuleSettingsDirective() {
                return routeRuleSettingsDirectiveAPI.setData(appendixDirectiveData.Settings);
            }

        }

        function buildRouteRuleObjFromScope() {
            var routeRule = {
                RuleId: (routeRuleId != null) ? routeRuleId : 0,
                Criteria: {
                    RoutingProductId: routingProductDirectiveAPI.getData() != undefined ? routingProductDirectiveAPI.getData().RoutingProductId : null,
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
            if (routeRuleObj.Criteria != null) {
                if (routeRuleObj.Criteria.RoutingProductId != null)
                    routingProductDirectiveAPI.setData(routeRuleObj.Criteria.RoutingProductId);

                angular.forEach(routeRuleObj.Criteria.ExcludedCodes, function (item) {
                    $scope.excludedCodes.push(item);
                });

                if (routeRuleObj.Criteria.CodeCriteriaGroupSettings != null)
                    $scope.selectedRouteRuleCriteriaType = UtilsService.getEnum(WhS_Be_RouteRuleCriteriaTypeEnum, 'value', 'Code');                    

                if (routeRuleObj.Criteria.SaleZoneGroupSettings != null)
                    $scope.selectedSaleZoneGroupTemplate = UtilsService.getItemByVal($scope.saleZoneGroupTemplates, routeRuleObj.Criteria.SaleZoneGroupSettings.ConfigId, "TemplateConfigID");

                if (routeRuleObj.Criteria.CustomerGroupSettings != null)
                    $scope.selectedCustomerGroupTemplate = UtilsService.getItemByVal($scope.customerGroupTemplates, routeRuleObj.Criteria.CustomerGroupSettings.ConfigId, "TemplateConfigID");

                if (routeRuleObj.Criteria.CodeCriteriaGroupSettings != null)
                    $scope.selectedCodeCriteriaGroupTemplate = UtilsService.getItemByVal($scope.codeCriteriaGroupTemplates, routeRuleObj.Criteria.CodeCriteriaGroupSettings.ConfigId, "TemplateConfigID");
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
