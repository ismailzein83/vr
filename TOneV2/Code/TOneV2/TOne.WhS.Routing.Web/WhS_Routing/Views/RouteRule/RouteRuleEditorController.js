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

        var routeRuleSettingsTypeSelectorAPI;
        var routeRuleSettingsTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel = {}
            $scope.scopeModel.showCriteriaSection = false;
            $scope.scopeModel.showSettingsSection = false;
            $scope.scopeModel.beginEffectiveDate = new Date();
            $scope.scopeModel.endEffectiveDate = undefined;

            $scope.scopeModel.saleZoneGroupTemplates = [];
            $scope.scopeModel.codeCriteriaGroupTemplates = [];
            $scope.scopeModel.customerGroupTemplates = [];
            $scope.scopeModel.excludedCodes = [];
            $scope.scopeModel.routeRuleSettingsTemplates = [];

            $scope.scopeModel.onRouteRuleSettingsTypeSelectorReady = function (api) {
                routeRuleSettingsTypeSelectorAPI = api;
                routeRuleSettingsTypeSelectorReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.onSaleZoneGroupSettingsDirectiveReady = function (api) {
                saleZoneGroupSettingsAPI = api;
                var saleZoneGroupPayload = {
                    sellingNumberPlanId: sellingNumberPlanId != undefined ? sellingNumberPlanId : undefined,
                    saleZoneFilterSettings: { RoutingProductId: routingProductId },
                };
                var setLoader = function (value) { $scope.scopeModel.isLoadingSellingNumberPlan = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneGroupSettingsAPI, saleZoneGroupPayload, setLoader, saleZoneGroupSettingsReadyPromiseDeferred);
            }
            $scope.scopeModel.onCodeCriteriaGroupSettingsDirectiveReady = function (api) {
                codeCriteriaGroupSettingsAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingCustomersSection = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, codeCriteriaGroupSettingsAPI, undefined, setLoader, codeCriteriaGroupSettingsReadyPromiseDeferred);
            }
            $scope.scopeModel.onCustomerGroupSettingsDirectiveReady = function (api) {
                customerGroupSettingsAPI = api;
                customerGroupSettingsReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.onRouteRuleSettingsDirectiveReady = function (api) {
                routeRuleSettingsAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingRouteRuleSettings = value };

                var routeRuleSettingsPayload = {
                    SupplierFilterSettings: { RoutingProductId: routingProductId }
                }

                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, routeRuleSettingsAPI, routeRuleSettingsPayload, setLoader, routeRuleSettingsReadyPromiseDeferred);
            }

            $scope.scopeModel.onRouteRuleCriteriaTypeSelectionChanged = function () {

                if ($scope.scopeModel.selectedRouteRuleCriteriaType == undefined)
                    return;

                if ($scope.scopeModel.selectedRouteRuleCriteriaType == WhS_Routing_RouteRuleCriteriaTypeEnum.SaleZone) {
                    $scope.scopeModel.showSaleZoneSection = $scope.scopeModel.showCustomerSection = $scope.scopeModel.showExcludedCodeSection = true;
                    $scope.scopeModel.showIncludedCodeSection = false;

                }
                else {
                    $scope.scopeModel.showIncludedCodeSection = $scope.scopeModel.showCustomerSection = $scope.scopeModel.showExcludedCodeSection = true;
                    $scope.scopeModel.selectedCodeCriteriaGroupTemplate = undefined;
                    $scope.scopeModel.showSaleZoneSection = false;
                }
            }
            $scope.scopeModel.onRouteRuleSettingsTypeSelectionChanged = function () {

                var _selectedItem = routeRuleSettingsTypeSelectorAPI.getSelectedIds();
                if (_selectedItem != undefined) {

                    $scope.scopeModel.showCriteriaSection = $scope.scopeModel.showSettingsSection = true;

                    ////Reloading Criteria Tab section
                    //$scope.scopeModel.selectedCodeCriteriaGroupTemplate = undefined;
                    //$scope.scopeModel.excludedCodes = [];
                    //reloadFilterBySection();
                    //reloadSaleZoneGroupSection();
                    //reloadCustomerGroupSection();
                }

                function reloadFilterBySection() {
                    if (!$scope.scopeModel.showRouteRuleTypeFilterSection)
                        return;

                    $scope.scopeModel.routeRuleCriteriaTypes = UtilsService.getArrayEnum(WhS_Routing_RouteRuleCriteriaTypeEnum);
                    if (routeRuleEntity != undefined && routeRuleEntity.Criteria.CodeCriteriaGroupSettings != undefined)
                        $scope.scopeModel.selectedRouteRuleCriteriaType = UtilsService.getEnum(WhS_Routing_RouteRuleCriteriaTypeEnum, 'value', WhS_Routing_RouteRuleCriteriaTypeEnum.Code.value);
                    else
                        $scope.scopeModel.selectedRouteRuleCriteriaType = UtilsService.getEnum(WhS_Routing_RouteRuleCriteriaTypeEnum, 'value', WhS_Routing_RouteRuleCriteriaTypeEnum.SaleZone.value);
                }
                function reloadSaleZoneGroupSection() {
                    if (saleZoneGroupSettingsAPI == undefined)
                        return;

                    var saleZoneGroupSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    var saleZoneGroupPayload = {
                        sellingNumberPlanId: sellingNumberPlanId != undefined ? sellingNumberPlanId : undefined,
                        saleZoneFilterSettings: { RoutingProductId: routingProductId }
                    }
                    VRUIUtilsService.callDirectiveLoad(saleZoneGroupSettingsAPI, saleZoneGroupPayload, saleZoneGroupSettingsLoadPromiseDeferred);

                    return saleZoneGroupSettingsLoadPromiseDeferred.promise;
                }
                function reloadCustomerGroupSection() {
                    if (customerGroupSettingsAPI == undefined)
                        return;

                    var customerGroupSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    var customerGroupPayload;
                    VRUIUtilsService.callDirectiveLoad(customerGroupSettingsAPI, customerGroupPayload, customerGroupSettingsLoadPromiseDeferred);

                    return customerGroupSettingsLoadPromiseDeferred.promise;
                }
            }

            //TODO: make the validation below a custom validation
            $scope.scopeModel.addExcludedCode = function () {
                var codeIsValid = true;

                if ($scope.scopeModel.excludedCode == undefined || $scope.scopeModel.excludedCode.length == 0) {
                    codeIsValid = false;
                }
                else {
                    angular.forEach($scope.scopeModel.excludedCodes, function (item) {
                        if ($scope.scopeModel.excludedCode === item) {
                            codeIsValid = false;
                        }
                    });
                }

                if (codeIsValid)
                    $scope.scopeModel.excludedCodes.push($scope.scopeModel.excludedCode);
            }
            $scope.scopeModel.removeExcludedCode = function (codeToRemove) {
                $scope.scopeModel.excludedCodes.splice($scope.scopeModel.excludedCodes.indexOf(codeToRemove), 1);
            }

            $scope.scopeModel.SaveRouteRule = function () {
                if (isEditMode) {
                    return updateRouteRule();
                }
                else {
                    return insertRouteRule();
                }
            };

            $scope.hasSaveRulePermission = function () {
                if (isEditMode)
                    return WhS_Routing_RouteRuleAPIService.HasUpdateRulePermission();
                else
                    return WhS_Routing_RouteRuleAPIService.HasAddRulePermission();
            }

            $scope.scopeModel.validateDates = function (date) {
                return UtilsService.validateDates($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
            }

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                $scope.title = "Edit Route Rule";
                getRouteRule().then(function () {
                    loadAllControls()
                        .finally(function () {
                            routeRuleEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                $scope.title = "New Route Rule";
                loadAllControls();
            }
        }

        function getRouteRule() {
            return WhS_Routing_RouteRuleAPIService.GetRule(routeRuleId).then(function (routeRule) {
                $scope.scopeModel.routeRuleName = routeRule != null ? routeRule.Name : '';
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
            var loadDataPromiseDeferred = UtilsService.createPromiseDeferred();

            var loadRouteRuleSettingsSectionPromise = loadRouteRuleSettingsSection();
            loadRouteRuleSettingsSectionPromise.then(function () {

                UtilsService.waitMultipleAsyncOperations([displaySectionsBasedOnParameters, editScopeTitle, loadFilterBySection, loadSaleZoneGroupSection, loadCustomerGroupSection,
                    loadCodeCriteriaGroupSection, loadStaticSection])
                    .catch(function (error) {
                        loadDataPromiseDeferred.reject(error);
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    })
                    .finally(function () {
                        loadDataPromiseDeferred.resolve();
                        $scope.scopeModel.isLoading = false;
                    });
            }).catch(function (error) {
                loadDataPromiseDeferred.reject(error);
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });

            return loadDataPromiseDeferred.promise;
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

            //loading RouteRuleSettingsTypeSelector
            var loadRouteRuleSettingsTemplatesPromise = UtilsService.createPromiseDeferred();
            promises.push(loadRouteRuleSettingsTemplatesPromise.promise);

            var routeRuleSettingsTypePayload = {};
            if (routeRuleEntity != undefined && routeRuleEntity.Settings != undefined) {
                routeRuleSettingsTypePayload.selectedIds = routeRuleEntity.Settings.ConfigId;
            };
            routeRuleSettingsTypeSelectorReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(routeRuleSettingsTypeSelectorAPI, routeRuleSettingsTypePayload, loadRouteRuleSettingsTemplatesPromise);
            });


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
        function displaySectionsBasedOnParameters() {
            $scope.scopeModel.showSaleZoneSection = routingProductId != undefined;
            $scope.scopeModel.showRouteRuleTypeFilterSection = $scope.scopeModel.showCustomerSection = $scope.scopeModel.showExcludedCodeSection = $scope.scopeModel.showIncludedCodeSection = !$scope.scopeModel.showSaleZoneSection;
        }
        function editScopeTitle() {
            if (routingProductId == undefined)
                return;

            $scope.title += " of " + productRouteEntity.Name;
        }
        function loadFilterBySection() {
            if (!$scope.scopeModel.showRouteRuleTypeFilterSection)
                return;

            $scope.scopeModel.routeRuleCriteriaTypes = UtilsService.getArrayEnum(WhS_Routing_RouteRuleCriteriaTypeEnum);
            if (routeRuleEntity != undefined && routeRuleEntity.Criteria.CodeCriteriaGroupSettings != undefined)
                $scope.scopeModel.selectedRouteRuleCriteriaType = UtilsService.getEnum(WhS_Routing_RouteRuleCriteriaTypeEnum, 'value', WhS_Routing_RouteRuleCriteriaTypeEnum.Code.value);
            else
                $scope.scopeModel.selectedRouteRuleCriteriaType = UtilsService.getEnum(WhS_Routing_RouteRuleCriteriaTypeEnum, 'value', WhS_Routing_RouteRuleCriteriaTypeEnum.SaleZone.value);
        }
        function loadSaleZoneGroupSection() {

            if (routeRuleEntity == undefined || routeRuleEntity.Criteria == undefined || routeRuleEntity.Criteria.SaleZoneGroupSettings == undefined) {
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
                    $scope.scopeModel.codeCriteriaGroupTemplates.push(item);
                });

                if (codeCriteriaGroupPayload != undefined)
                    $scope.scopeModel.selectedCodeCriteriaGroupTemplate = UtilsService.getItemByVal($scope.scopeModel.codeCriteriaGroupTemplates, codeCriteriaGroupPayload.ConfigId, "ExtensionConfigurationId");
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
        function loadStaticSection() {
            if (routeRuleEntity == undefined)
                return;

            $scope.scopeModel.beginEffectiveDate = routeRuleEntity.BeginEffectiveTime;
            $scope.scopeModel.endEffectiveDate = routeRuleEntity.EndEffectiveTime;

            if (routeRuleEntity.Criteria != null) {

                angular.forEach(routeRuleEntity.Criteria.ExcludedCodes, function (item) {
                    $scope.scopeModel.excludedCodes.push(item);
                });
            }
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

        function buildRouteRuleObjFromScope() {

            var routeRule = {
                RuleId: (routeRuleId != null) ? routeRuleId : 0,
                Name: $scope.scopeModel.routeRuleName,
                Criteria: {
                    RoutingProductId: routingProductId,
                    ExcludedCodes: $scope.scopeModel.excludedCodes,
                    SaleZoneGroupSettings: $scope.scopeModel.showSaleZoneSection ? saleZoneGroupSettingsAPI.getData() : undefined,//VRUIUtilsService.getSettingsFromDirective($scope, saleZoneGroupSettingsAPI, 'selectedSaleZoneGroupTemplate'),
                    CustomerGroupSettings: customerGroupSettingsAPI.getData(),
                    CodeCriteriaGroupSettings: $scope.scopeModel.showIncludedCodeSection ? VRUIUtilsService.getSettingsFromDirective($scope.scopeModel, codeCriteriaGroupSettingsAPI, 'selectedCodeCriteriaGroupTemplate') : undefined
                },
                Settings: VRUIUtilsService.getSettingsFromDirective($scope.scopeModel, routeRuleSettingsAPI, 'selectedRouteRuleSettingsTemplate'),
                BeginEffectiveTime: $scope.scopeModel.beginEffectiveDate,
                EndEffectiveTime: $scope.scopeModel.endEffectiveDate
            };

            return routeRule;
        }
    }

    appControllers.controller('WhS_Routing_RouteRuleEditorController', routeRuleEditorController);
})(appControllers);
