(function (appControllers) {

    "use strict";

    routeRuleEditorController.$inject = ['$scope', 'WhS_Routing_RouteRuleAPIService', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_SaleZoneAPIService', 'WhS_BE_CarrierAccountAPIService',
        'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_Routing_RouteRuleCriteriaTypeEnum'];

    function routeRuleEditorController($scope, WhS_Routing_RouteRuleAPIService, WhS_BE_RoutingProductAPIService, WhS_BE_SaleZoneAPIService, WhS_BE_CarrierAccountAPIService,
        UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_Routing_RouteRuleCriteriaTypeEnum) {

        var isEditMode;

        var routeRuleId;
        var routingProductId;
        var sellingNumberPlanId;

        var routeRuleEntity;
        var productRouteEntity;

        var saleZoneGroupSettingsAPI;
        var saleZoneGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var codeCriteriaGroupSettingsAPI;
        var codeCriteriaGroupSettingsReadyPromiseDeferred;

        var customerGroupSettingsAPI;
        var customerGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var routeRuleSettingsAPI;
        var routeRuleSettingsReadyPromiseDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {

                routeRuleId = parameters.routeRuleId;
                routingProductId = parameters.routingProductId;
                sellingNumberPlanId = parameters.sellingNumberPlanId;
            }
            isEditMode = (routeRuleId != undefined);
        }

        function defineScope() {

            $scope.hasSaveRulePermission = function () {
                if (isEditMode)
                    return WhS_Routing_RouteRuleAPIService.HasUpdateRulePermission();
                else
                    return WhS_Routing_RouteRuleAPIService.HasAddRulePermission();
            }

            $scope.scopeModal = {}
            $scope.scopeModal.onSaleZoneGroupSettingsDirectiveReady = function (api) {
                saleZoneGroupSettingsAPI = api;
                var saleZoneGroupPayload = {
                    sellingNumberPlanId: sellingNumberPlanId != undefined ? sellingNumberPlanId : undefined,
                    saleZoneFilterSettings: { RoutingProductId: routingProductId },
                };
                var setLoader = function (value) { $scope.scopeModal.isLoadingSellingNumberPlan = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneGroupSettingsAPI, saleZoneGroupPayload, setLoader, saleZoneGroupSettingsReadyPromiseDeferred);
            }

            $scope.scopeModal.onCodeCriteriaGroupSettingsDirectiveReady = function (api) {
                codeCriteriaGroupSettingsAPI = api;
                var setLoader = function (value) { $scope.scopeModal.isLoadingCustomersSection = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, codeCriteriaGroupSettingsAPI, undefined, setLoader, codeCriteriaGroupSettingsReadyPromiseDeferred);
            }

            $scope.scopeModal.onCustomerGroupSettingsDirectiveReady = function (api) {
                customerGroupSettingsAPI = api;
                customerGroupSettingsReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.onRouteRuleSettingsDirectiveReady = function (api) {
                routeRuleSettingsAPI = api;
                var setLoader = function (value) { $scope.scopeModal.isLoadingRouteRuleSettings = value };

                var routeRuleSettingsPayload = {
                    SupplierFilterSettings: { RoutingProductId: routingProductId }
                }

                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, routeRuleSettingsAPI, routeRuleSettingsPayload, setLoader, routeRuleSettingsReadyPromiseDeferred);
            }

            $scope.scopeModal.SaveRouteRule = function () {
                if (isEditMode) {
                    return updateRouteRule();
                }
                else {
                    return insertRouteRule();
                }
            };

            //TODO: make the validation below a custom validation
            $scope.scopeModal.addExcludedCode = function () {
                var codeIsValid = true;

                if ($scope.scopeModal.excludedCode == undefined || $scope.scopeModal.excludedCode.length == 0) {
                    codeIsValid = false;
                }
                else {
                    angular.forEach($scope.scopeModal.excludedCodes, function (item) {
                        if ($scope.scopeModal.excludedCode === item) {
                            codeIsValid = false;
                        }
                    });
                }

                if (codeIsValid)
                    $scope.scopeModal.excludedCodes.push($scope.scopeModal.excludedCode);
            }

            $scope.scopeModal.removeExcludedCode = function (codeToRemove) {
                $scope.scopeModal.excludedCodes.splice($scope.scopeModal.excludedCodes.indexOf(codeToRemove), 1);
            }

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModal.onRouteRuleCriteriaTypeSelectionChanged = function () {

                if ($scope.scopeModal.selectedRouteRuleCriteriaType == undefined)
                    return;

                if ($scope.scopeModal.selectedRouteRuleCriteriaType == WhS_Routing_RouteRuleCriteriaTypeEnum.SaleZone) {
                    $scope.scopeModal.showSaleZoneSection = $scope.scopeModal.showCustomerSection = $scope.scopeModal.showExcludedCodeSection = true;
                    $scope.scopeModal.showIncludedCodeSection = false;
                  
                }
                else {
                    $scope.scopeModal.showIncludedCodeSection = $scope.scopeModal.showCustomerSection = $scope.scopeModal.showExcludedCodeSection = true;
                    $scope.scopeModal.selectedCodeCriteriaGroupTemplate = undefined;
                    $scope.scopeModal.showSaleZoneSection = false;
                }
            }

            $scope.scopeModal.saleZoneGroupTemplates = [];
            $scope.scopeModal.customerGroupTemplates = [];
            $scope.scopeModal.codeCriteriaGroupTemplates = [];
            $scope.scopeModal.routeRuleSettingsTemplates = [];

            $scope.scopeModal.excludedCodes = [];

            $scope.scopeModal.beginEffectiveDate = new Date();
            $scope.scopeModal.endEffectiveDate = undefined;

            $scope.scopeModal.validateDates = function (date) {
                return UtilsService.validateDates($scope.scopeModal.beginEffectiveDate, $scope.scopeModal.endEffectiveDate);
            }
        }

        function load() {
            $scope.scopeModal.isLoading = true;

            if (isEditMode) {
                $scope.title = "Edit Route Rule";
                getRouteRule().then(function () {
                    loadAllControls()
                        .finally(function () {
                            routeRuleEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            else {
                $scope.title = "New Route Rule";
                loadAllControls();
            }
        }

        function displaySectionsBasedOnParameters() {
            $scope.scopeModal.showSaleZoneSection = routingProductId != undefined;
            $scope.scopeModal.showRouteRuleTypeFilterSection = $scope.scopeModal.showCustomerSection = $scope.scopeModal.showExcludedCodeSection = $scope.scopeModal.showIncludedCodeSection = !$scope.scopeModal.showSaleZoneSection;
        }

        function loadAllControls() {
            var promises = [];
            if (routingProductId != undefined) {
                var getProductRoutePromise = getProductRoute();
                promises.push(getProductRoutePromise);
                var loadDataPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadDataPromiseDeferred.promise);

                getProductRoutePromise.then(function () {
                    loadData().then(function () {
                        loadDataPromiseDeferred.resolve();
                    }).catch(function (error) {
                        loadDataPromiseDeferred.reject(error);
                    });
                });
            }
            else {
                promises.push(loadData());                
            }
            return UtilsService.waitMultiplePromises(promises);
        }

        function loadData() {
            return UtilsService.waitMultipleAsyncOperations([displaySectionsBasedOnParameters, editScopeTitle, loadFilterBySection, loadSaleZoneGroupSection, loadCustomerGroupSection,
                loadCodeCriteriaGroupSection, loadRouteRuleSettingsSection, loadStaticSection])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModal.isLoading = false;
                });
        }

        function getRouteRule() {
            return WhS_Routing_RouteRuleAPIService.GetRule(routeRuleId).then(function (routeRule) {
                $scope.scopeModal.routeRuleName = routeRule != null ? routeRule.Name : '';
                routeRuleEntity = routeRule;
                routingProductId = routeRuleEntity.Criteria != null ? routeRuleEntity.Criteria.RoutingProductId : undefined;
            });
        }

        function getProductRoute() {
            if (routingProductId != undefined)
                return WhS_BE_RoutingProductAPIService.GetRoutingProduct(routingProductId).then(function (response) {
                    productRouteEntity = response;
                    sellingNumberPlanId = productRouteEntity.SellingNumberPlanId;
                });
        }

        function editScopeTitle() {
            if (routingProductId == undefined)
                return;

            $scope.title += " of " + productRouteEntity.Name;
        }

        function loadFilterBySection() {
            if (!$scope.scopeModal.showRouteRuleTypeFilterSection)
                return;

            $scope.scopeModal.routeRuleCriteriaTypes = UtilsService.getArrayEnum(WhS_Routing_RouteRuleCriteriaTypeEnum);
            if (routeRuleEntity != undefined && routeRuleEntity.Criteria.CodeCriteriaGroupSettings != undefined)
                $scope.scopeModal.selectedRouteRuleCriteriaType = UtilsService.getEnum(WhS_Routing_RouteRuleCriteriaTypeEnum, 'value', WhS_Routing_RouteRuleCriteriaTypeEnum.Code.value);
            else
                $scope.scopeModal.selectedRouteRuleCriteriaType = UtilsService.getEnum(WhS_Routing_RouteRuleCriteriaTypeEnum, 'value', WhS_Routing_RouteRuleCriteriaTypeEnum.SaleZone.value);
        }

        function loadSaleZoneGroupSection() {
         
            if (routeRuleEntity == undefined || routeRuleEntity.Criteria == undefined || routeRuleEntity.Criteria.SaleZoneGroupSettings == undefined)
            {
                saleZoneGroupSettingsReadyPromiseDeferred = undefined;
                return;
            }
            var promises = [];

            if (saleZoneGroupSettingsReadyPromiseDeferred == undefined)
                saleZoneGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleZoneGroupSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            promises.push(saleZoneGroupSettingsLoadPromiseDeferred.promise);

            saleZoneGroupSettingsReadyPromiseDeferred.promise.then(function () {
                var saleZoneGroupPayload = {
                    sellingNumberPlanId: sellingNumberPlanId != undefined ? sellingNumberPlanId : undefined,
                    saleZoneFilterSettings: { RoutingProductId: routingProductId },
                };

                if (routeRuleEntity != undefined) {
                    saleZoneGroupPayload.sellingNumberPlanId = sellingNumberPlanId != undefined ? sellingNumberPlanId : routeRuleEntity.Criteria.SaleZoneGroupSettings != undefined ? routeRuleEntity.Criteria.SaleZoneGroupSettings.SellingNumberPlanId : undefined;
                    saleZoneGroupPayload.saleZoneGroupSettings = routeRuleEntity.Criteria.SaleZoneGroupSettings
                }
                saleZoneGroupSettingsReadyPromiseDeferred = undefined;
                VRUIUtilsService.callDirectiveLoad(saleZoneGroupSettingsAPI, saleZoneGroupPayload, saleZoneGroupSettingsLoadPromiseDeferred);
            });
            return UtilsService.waitMultiplePromises(promises);
        }

        function loadCodeCriteriaGroupSection() {
            var promises = [];
            var codeCriteriaGroupPayload;

            if (routeRuleEntity != undefined && routeRuleEntity.Criteria.CodeCriteriaGroupSettings != null)
                codeCriteriaGroupPayload = routeRuleEntity.Criteria.CodeCriteriaGroupSettings;

            var loadCodeCriteriaGroupTemplatesPromise = WhS_Routing_RouteRuleAPIService.GetCodeCriteriaGroupTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.scopeModal.codeCriteriaGroupTemplates.push(item);
                });

                if (codeCriteriaGroupPayload != undefined)
                    $scope.scopeModal.selectedCodeCriteriaGroupTemplate = UtilsService.getItemByVal($scope.scopeModal.codeCriteriaGroupTemplates, codeCriteriaGroupPayload.ConfigId, "ExtensionConfigurationId");
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
            if (customerGroupSettingsReadyPromiseDeferred == undefined)
                customerGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var customerGroupSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            promises.push(customerGroupSettingsLoadPromiseDeferred.promise);

            customerGroupSettingsReadyPromiseDeferred.promise.then(function () {
                var customerGroupPayload;
                if (routeRuleEntity != undefined && routeRuleEntity.Criteria.CustomerGroupSettings != null)
                    customerGroupPayload = routeRuleEntity.Criteria.CustomerGroupSettings;

                customerGroupSettingsReadyPromiseDeferred = undefined;
                VRUIUtilsService.callDirectiveLoad(customerGroupSettingsAPI, customerGroupPayload, customerGroupSettingsLoadPromiseDeferred);
            });


            return UtilsService.waitMultiplePromises(promises);
        }

        function loadRouteRuleSettingsSection() {
            var promises = [];
            var routeRuleSettingsPayload;

            if (routeRuleEntity != undefined && routeRuleEntity.Settings != null) {
                routeRuleSettingsPayload = {
                    SupplierFilterSettings: { RoutingProductId: routingProductId },
                    RouteRuleSettings: routeRuleEntity.Settings
                }
            }

            var loadRouteRuleSettingsTemplatesPromise = WhS_Routing_RouteRuleAPIService.GetRouteRuleSettingsTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.scopeModal.routeRuleSettingsTemplates.push(item);
                });

                if (routeRuleSettingsPayload != undefined)
                    $scope.scopeModal.selectedrouteRuleSettingsTemplate = UtilsService.getItemByVal($scope.scopeModal.routeRuleSettingsTemplates, routeRuleSettingsPayload.RouteRuleSettings.ConfigId, "ExtensionConfigurationId");
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

        function loadStaticSection() {
            if (routeRuleEntity == undefined)
                return;

            $scope.scopeModal.beginEffectiveDate = routeRuleEntity.BeginEffectiveTime;
            $scope.scopeModal.endEffectiveDate = routeRuleEntity.EndEffectiveTime;

            if (routeRuleEntity.Criteria != null) {

                angular.forEach(routeRuleEntity.Criteria.ExcludedCodes, function (item) {
                    $scope.scopeModal.excludedCodes.push(item);
                });
            }
        }

        function buildRouteRuleObjFromScope() {

            var routeRule = {
                RuleId: (routeRuleId != null) ? routeRuleId : 0,
                Name: $scope.scopeModal.routeRuleName,
                Criteria: {
                    RoutingProductId: routingProductId,
                    ExcludedCodes: $scope.scopeModal.excludedCodes,
                    SaleZoneGroupSettings:$scope.scopeModal.showSaleZoneSection? saleZoneGroupSettingsAPI.getData():undefined,//VRUIUtilsService.getSettingsFromDirective($scope, saleZoneGroupSettingsAPI, 'selectedSaleZoneGroupTemplate'),
                    CustomerGroupSettings: customerGroupSettingsAPI.getData(),
                    CodeCriteriaGroupSettings: $scope.scopeModal.showIncludedCodeSection? VRUIUtilsService.getSettingsFromDirective($scope.scopeModal, codeCriteriaGroupSettingsAPI, 'selectedCodeCriteriaGroupTemplate'):undefined
                },
                Settings: VRUIUtilsService.getSettingsFromDirective($scope.scopeModal, routeRuleSettingsAPI, 'selectedrouteRuleSettingsTemplate'),
                BeginEffectiveTime: $scope.scopeModal.beginEffectiveDate,
                EndEffectiveTime: $scope.scopeModal.endEffectiveDate
            };

            return routeRule;
        }

        function insertRouteRule() {
            var routeRuleObject = buildRouteRuleObjFromScope();
            return WhS_Routing_RouteRuleAPIService.AddRule(routeRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Route Rule", response, "Name")) {
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
            WhS_Routing_RouteRuleAPIService.UpdateRule(routeRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Route Rule", response, "Name")) {
                    if ($scope.onRouteRuleUpdated != undefined)
                        $scope.onRouteRuleUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('WhS_Routing_RouteRuleEditorController', routeRuleEditorController);
})(appControllers);
