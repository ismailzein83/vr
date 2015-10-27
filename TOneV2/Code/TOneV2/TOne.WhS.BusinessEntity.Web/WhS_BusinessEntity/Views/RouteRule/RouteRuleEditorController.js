(function (appControllers) {

    "use strict";

    routeRuleEditorController.$inject = ['$scope', 'WhS_BE_RouteRuleAPIService', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_SaleZoneAPIService', 'WhS_BE_CarrierAccountAPIService',
        'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_Be_RouteRuleCriteriaTypeEnum'];

    function routeRuleEditorController($scope, WhS_BE_RouteRuleAPIService, WhS_BE_RoutingProductAPIService, WhS_BE_SaleZoneAPIService, WhS_BE_CarrierAccountAPIService,
        UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_Be_RouteRuleCriteriaTypeEnum) {

        var isEditMode;
        var routeRuleId;
        var routeRuleEntity;

        var routingProductAPI;
        var routingProductReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneGroupSettingsAPI;
        var saleZoneGroupSettingsReadyPromiseDeferred;

        var codeCriteriaGroupSettingsAPI;
        var codeCriteriaGroupSettingsReadyPromiseDeferred;

        var customerGroupSettingsAPI;
        var customerGroupSettingsReadyPromiseDeferred;

        var routeRuleSettingsAPI;
        var routeRuleSettingsReadyPromiseDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                routeRuleId = parameters.routeRuleId;
            }
            isEditMode = (routeRuleId != undefined);
        }

        function defineScope() {

            $scope.onRoutingProductDirectiveReady = function (api) {
                routingProductAPI = api;
                routingProductReadyPromiseDeferred.resolve();
            }

            $scope.onSaleZoneGroupSettingsDirectiveReady = function (api) {
                saleZoneGroupSettingsAPI = api;
                var setLoader = function (value) { $scope.isLoadingSaleZonesSection = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneGroupSettingsAPI, undefined, setLoader, saleZoneGroupSettingsReadyPromiseDeferred);
            }

            $scope.onCodeCriteriaGroupSettingsDirectiveReady = function (api) {
                codeCriteriaGroupSettingsAPI = api;
                var setLoader = function (value) { $scope.isLoadingCustomersSection = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, codeCriteriaGroupSettingsAPI, undefined, setLoader, codeCriteriaGroupSettingsReadyPromiseDeferred);
            }

            $scope.onCustomerGroupSettingsDirectiveReady = function (api) {
                customerGroupSettingsAPI = api;
                var setLoader = function (value) { $scope.isLoadingCodeCriteriaSection = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, customerGroupSettingsAPI, undefined, setLoader, customerGroupSettingsReadyPromiseDeferred);
            }

            $scope.onRouteRuleSettingsDirectiveReady = function (api) {
                routeRuleSettingsAPI = api;
                var setLoader = function (value) { $scope.isLoadingRouteRuleSettings = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, routeRuleSettingsAPI, undefined, setLoader, routeRuleSettingsReadyPromiseDeferred);
            }

            $scope.SaveRouteRule = function () {
                if (isEditMode) {
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
                if (routingProductAPI != undefined) {
                    var routingProductObj = routingProductAPI.getData();
                    if (routingProductObj != undefined)
                    {
                        $scope.selectedSellingNumberPlanId = routingProductAPI.getData().SellingNumberPlanId;
                        $scope.showSaleZoneSection = true;
                        $scope.showRouteRuleCriteriaTypes = $scope.showCustomerSection = $scope.showExcludedCodeSection = $scope.showIncludedCodeSection = false;
                    }
                    else
                    {
                        $scope.showSaleZoneSection = false;
                        $scope.showRouteRuleCriteriaTypes = $scope.showCustomerSection = $scope.showExcludedCodeSection = $scope.showIncludedCodeSection = true;
                    }
                }
                else {
                    $scope.selectedSellingNumberPlanId = undefined;
                }
            }

            $scope.onRouteRuleCriteriaTypeSelectionChanged = function () {
                if ($scope.selectedRouteRuleCriteriaType == WhS_Be_RouteRuleCriteriaTypeEnum.SaleZone)
                {
                    $scope.showSaleZoneSection = $scope.showCustomerSection = $scope.showExcludedCodeSection = true;
                    $scope.showIncludedCodeSection = false;
                }
                else
                {
                    $scope.showIncludedCodeSection = $scope.showCustomerSection = $scope.showExcludedCodeSection = true;
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
            $scope.isLoading = true;

            loadEnums();

            if (isEditMode) {
                getRouteRule().then(function () {
                    loadAllControls()
                        .finally(function () {
                            routeRuleEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadRoutingProductDirective, loadSaleZoneGroupSection, loadCustomerGroupSection, loadCodeCriteriaGroupSection, loadRouteRuleSettingsSection])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.isLoading = false;
               });
        }

        function loadEnums()
        {
            $scope.routeRuleCriteriaTypes = UtilsService.getArrayEnum(WhS_Be_RouteRuleCriteriaTypeEnum);
            $scope.selectedRouteRuleCriteriaType = UtilsService.getEnum(WhS_Be_RouteRuleCriteriaTypeEnum, 'value', 'SaleZone');
        }

        function getRouteRule() {
            return WhS_BE_RouteRuleAPIService.GetRule(routeRuleId).then(function (routeRule) {
                routeRuleEntity = routeRule;
                fillScopeFromRouteRuleObj(routeRule);
            });
        }

        function loadRoutingProductDirective() {

            var loadRoutingProductPromiseDeferred = UtilsService.createPromiseDeferred();

            routingProductReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload;
                    if (routeRuleEntity != undefined && routeRuleEntity.Criteria.RoutingProductId != null)
                        directivePayload = routeRuleEntity.Criteria.RoutingProductId;
                    VRUIUtilsService.callDirectiveLoad(routingProductAPI, directivePayload, loadRoutingProductPromiseDeferred);
                });

            return loadRoutingProductPromiseDeferred.promise;
        }

        function loadSaleZoneGroupSection() {

            var promises = [];
            var saleZoneGroupPayload;

            if (routeRuleEntity != undefined && routeRuleEntity.Criteria.SaleZoneGroupSettings != null)
                saleZoneGroupPayload = routeRuleEntity.Criteria.SaleZoneGroupSettings;
            
            var loadSaleZoneGroupTemplatesPromise = WhS_BE_SaleZoneAPIService.GetSaleZoneGroupTemplates()
                .then(function (response) {
                   
                    angular.forEach(response, function (item) {
                        $scope.saleZoneGroupTemplates.push(item);
                    });

                    if (saleZoneGroupPayload != undefined)
                        $scope.selectedSaleZoneGroupTemplate = UtilsService.getItemByVal($scope.saleZoneGroupTemplates, saleZoneGroupPayload.ConfigId, "TemplateConfigID");
                });

            promises.push(loadSaleZoneGroupTemplatesPromise);

            if (saleZoneGroupPayload != undefined) {
                saleZoneGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                var saleZoneGroupSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(saleZoneGroupSettingsLoadPromiseDeferred.promise);

                saleZoneGroupSettingsReadyPromiseDeferred.promise.then(function () {
                    saleZoneGroupSettingsReadyPromiseDeferred = undefined;
                    VRUIUtilsService.callDirectiveLoad(saleZoneGroupSettingsAPI, saleZoneGroupPayload, saleZoneGroupSettingsLoadPromiseDeferred);
                });
            }
            return UtilsService.waitMultiplePromises(promises);
        }

        function loadCodeCriteriaGroupSection() {
            var promises = [];
            var codeCriteriaGroupPayload;

            if (routeRuleEntity != undefined && routeRuleEntity.Criteria.CodeCriteriaGroupSettings != null)
                codeCriteriaGroupPayload = routeRuleEntity.Criteria.CodeCriteriaGroupSettings;

            var loadCodeCriteriaGroupTemplatesPromise = WhS_BE_RouteRuleAPIService.GetCodeCriteriaGroupTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.codeCriteriaGroupTemplates.push(item);
                });

                if(codeCriteriaGroupPayload != undefined)
                    $scope.selectedCodeCriteriaGroupTemplate = UtilsService.getItemByVal($scope.codeCriteriaGroupTemplates, codeCriteriaGroupPayload.ConfigId, "TemplateConfigID");
            });

            promises.push(loadCodeCriteriaGroupTemplatesPromise);

            if (codeCriteriaGroupPayload != undefined) {
                codeCriteriaGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                var codeCriteriaGroupSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(codeCriteriaGroupSettingsLoadPromiseDeferred.promise);

                codeCriteriaGroupSettingsReadyPromiseDeferred.promise.then(function () {
                    codeCriteriaGroupSettingsReadyPromiseDeferred = undefined;
                    VRUIUtilsService.callDirectiveLoad(codeCriteriaGroupSettingsAPI, codeCriteriaGroupPayload, codeCriteriaGroupSettingsLoadPromiseDeferred);
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function loadCustomerGroupSection() {
            var promises = [];
            var customerGroupPayload;

            if (routeRuleEntity != undefined && routeRuleEntity.Criteria.CustomerGroupSettings != null)
                customerGroupPayload = routeRuleEntity.Criteria.CustomerGroupSettings;

            var loadCustomerGroupTemplatesPromise = WhS_BE_CarrierAccountAPIService.GetCustomerGroupTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.customerGroupTemplates.push(item);
                });

                if(customerGroupPayload != undefined)
                    $scope.selectedCustomerGroupTemplate = UtilsService.getItemByVal($scope.customerGroupTemplates, customerGroupPayload.ConfigId, "TemplateConfigID");
            });

            promises.push(loadCustomerGroupTemplatesPromise);

            if (customerGroupPayload != undefined) {
                customerGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                var customerGroupSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(customerGroupSettingsLoadPromiseDeferred.promise);

                customerGroupSettingsReadyPromiseDeferred.promise.then(function () {
                    customerGroupSettingsReadyPromiseDeferred = undefined;
                    VRUIUtilsService.callDirectiveLoad(customerGroupSettingsAPI, customerGroupPayload, customerGroupSettingsLoadPromiseDeferred);
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function loadRouteRuleSettingsSection()
        {
            var promises = [];
            var routeRuleSettingsPayload;

            if (routeRuleEntity != undefined && routeRuleEntity.Settings != null)
                routeRuleSettingsPayload = routeRuleEntity.Settings;

            var loadRouteRuleSettingsTemplatesPromise = WhS_BE_RouteRuleAPIService.GetRouteRuleSettingsTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.routeRuleSettingsTemplates.push(item);
                });

                if(routeRuleSettingsPayload != undefined)
                    $scope.selectedrouteRuleSettingsTemplate = UtilsService.getItemByVal($scope.routeRuleSettingsTemplates, routeRuleSettingsPayload.ConfigId, "TemplateConfigID");
            });

            promises.push(loadRouteRuleSettingsTemplatesPromise);

            if (routeRuleSettingsPayload != undefined) {
                routeRuleSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                var routeRuleSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(routeRuleSettingsLoadPromiseDeferred.promise);

                routeRuleSettingsReadyPromiseDeferred.promise.then(function () {
                    routeRuleSettingsReadyPromiseDeferred = undefined;
                    VRUIUtilsService.callDirectiveLoad(routeRuleSettingsAPI, routeRuleSettingsPayload, routeRuleSettingsLoadPromiseDeferred);
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function buildRouteRuleObjFromScope() {
            var routeRule = {
                RuleId: (routeRuleId != null) ? routeRuleId : 0,
                Criteria: {
                    RoutingProductId: routingProductAPI.getData() != undefined ? routingProductAPI.getData().RoutingProductId : null,
                    ExcludedCodes: $scope.excludedCodes,
                    SaleZoneGroupSettings: VRUIUtilsService.getSettingsFromDirective($scope, saleZoneGroupSettingsAPI, 'selectedSaleZoneGroupTemplate'),
                    CustomerGroupSettings: VRUIUtilsService.getSettingsFromDirective($scope, customerGroupSettingsAPI, 'selectedCustomerGroupTemplate'),
                    CodeCriteriaGroupSettings: VRUIUtilsService.getSettingsFromDirective($scope, codeCriteriaGroupSettingsAPI, 'selectedCodeCriteriaGroupTemplate')
                },
                Settings: VRUIUtilsService.getSettingsFromDirective($scope, routeRuleSettingsAPI, 'selectedrouteRuleSettingsTemplate'),
                BeginEffectiveTime: $scope.beginEffectiveDate,
                EndEffectiveTime: $scope.endEffectiveDate
            };

            return routeRule;
        }

        function fillScopeFromRouteRuleObj(routeRuleObj) {
            if (routeRuleObj.Criteria != null) {

                angular.forEach(routeRuleObj.Criteria.ExcludedCodes, function (item) {
                    $scope.excludedCodes.push(item);
                });

                if (routeRuleObj.Criteria.CodeCriteriaGroupSettings != null)
                    $scope.selectedRouteRuleCriteriaType = UtilsService.getEnum(WhS_Be_RouteRuleCriteriaTypeEnum, 'value', 'Code');                    
            }
            
            $scope.beginEffectiveDate = routeRuleObj.BeginEffectiveTime;
            $scope.endEffectiveDate = routeRuleObj.EndEffectiveTime;
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
    }

    appControllers.controller('WhS_BE_RouteRuleEditorController', routeRuleEditorController);
})(appControllers);
