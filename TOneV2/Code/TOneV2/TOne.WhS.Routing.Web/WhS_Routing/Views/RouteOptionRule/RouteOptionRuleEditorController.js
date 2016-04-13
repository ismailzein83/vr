(function (appControllers) {

    "use strict";

    routeOptionRuleEditorController.$inject = ['$scope', 'WhS_Routing_RouteOptionRuleAPIService', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_SaleZoneAPIService', 'WhS_BE_CarrierAccountAPIService',
        'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_Routing_RouteRuleCriteriaTypeEnum'];

    function routeOptionRuleEditorController($scope, WhS_Routing_RouteOptionRuleAPIService, WhS_BE_RoutingProductAPIService, WhS_BE_SaleZoneAPIService, WhS_BE_CarrierAccountAPIService,
        UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_Routing_RouteRuleCriteriaTypeEnum) {

        var isEditMode;

        var routeOptionRuleId;
        var routingProductId;
        var sellingNumberPlanId;

        var routeOptionRuleEntity;

        var saleZoneGroupSettingsAPI;
        var saleZoneGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var codeCriteriaGroupSettingsAPI;
        var codeCriteriaGroupSettingsReadyPromiseDeferred;

        var customerGroupSettingsAPI;
        var customerGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var routeOptionRuleSettingsAPI;
        var routeOptionRuleSettingsReadyPromiseDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {

                routeOptionRuleId = parameters.routeOptionRuleId;
                routingProductId = parameters.routingProductId;
                sellingNumberPlanId = parameters.sellingNumberPlanId;
            }
            isEditMode = (routeOptionRuleId != undefined);
        }

        function defineScope() {

            $scope.hasSaveOptionRulePermission = function () {
                if (isEditMode)
                    return WhS_Routing_RouteOptionRuleAPIService.HasUpdateRulePermission();
                else
                    return WhS_Routing_RouteOptionRuleAPIService.HasAddRulePermission();
            }

            $scope.scopeModal = {}
            $scope.scopeModal.onSaleZoneGroupSettingsDirectiveReady = function (api) {
                saleZoneGroupSettingsAPI = api;
                saleZoneGroupSettingsReadyPromiseDeferred.resolve();
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

            $scope.scopeModal.onRouteOptionRuleSettingsDirectiveReady = function (api) {
                routeOptionRuleSettingsAPI = api;
                var setLoader = function (value) { $scope.scopeModal.isLoadingRouteOptionRuleSettings = value };

                var routeOptionRuleSettingsPayload = {
                    SupplierFilterSettings: { RoutingProductId: routingProductId }
                }

                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, routeOptionRuleSettingsAPI, routeOptionRuleSettingsPayload, setLoader, routeOptionRuleSettingsReadyPromiseDeferred);
            }

            $scope.scopeModal.SaveRouteOptionRule = function () {
                if (isEditMode) {
                    return updateRouteOptionRule();
                }
                else {
                    return insertRouteOptionRule();
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

            $scope.scopeModal.onRouteOptionRuleCriteriaTypeSelectionChanged = function () {

                if ($scope.scopeModal.selectedRouteOptionRuleCriteriaType == undefined)
                    return;

                if ($scope.scopeModal.selectedRouteOptionRuleCriteriaType == WhS_Routing_RouteRuleCriteriaTypeEnum.SaleZone) {
                    $scope.scopeModal.showSaleZoneSection = $scope.scopeModal.showCustomerSection = $scope.scopeModal.showExcludedCodeSection = true;
                    $scope.scopeModal.showIncludedCodeSection = false;
                }
                else {
                    $scope.scopeModal.showIncludedCodeSection = $scope.scopeModal.showCustomerSection = $scope.scopeModal.showExcludedCodeSection = true;
                    $scope.scopeModal.showSaleZoneSection = false;
                }
            }

            $scope.scopeModal.saleZoneGroupTemplates = [];
            $scope.scopeModal.customerGroupTemplates = [];
            $scope.scopeModal.codeCriteriaGroupTemplates = [];
            $scope.scopeModal.routeOptionRuleSettingsTemplates = [];

            $scope.scopeModal.excludedCodes = [];

            $scope.scopeModal.beginEffectiveDate = new Date();
            $scope.scopeModal.endEffectiveDate = undefined;
        }

        function load() {
            $scope.scopeModal.isLoading = true;

            if (isEditMode) {
                $scope.title = "Edit Route Option OptionRule";
                getRouteRule().then(function () {
                    loadAllControls()
                        .finally(function () {
                            routeOptionRuleEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            else {
                $scope.title = "New Route Option Rule";
                loadAllControls();
            }
        }

        function displaySectionsBasedOnParameters() {
            $scope.scopeModal.showSaleZoneSection = routingProductId != undefined;
            $scope.scopeModal.showRouteOptionRuleTypeFilterSection = $scope.scopeModal.showCustomerSection = $scope.scopeModal.showExcludedCodeSection = $scope.scopeModal.showIncludedCodeSection = !$scope.scopeModal.showSaleZoneSection;
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([displaySectionsBasedOnParameters, editScopeTitle, loadFilterBySection, loadSaleZoneGroupSection, loadCustomerGroupSection,
                loadCodeCriteriaGroupSection, loadRouteOptionRuleSettingsSection, loadStaticSection])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.scopeModal.isLoading = false;
               });
        }

        function getRouteOptionRule() {
            return WhS_Routing_RouteOptionRuleAPIService.GetOptionRule(routeOptionRuleId).then(function (routeOptionRule) {
                routeOptionRuleEntity = routeOptionRule;
                routingProductId = routeOptionRuleEntity.Criteria != null ? routeOptionRuleEntity.Criteria.RoutingProductId : undefined;
            });
        }

        function editScopeTitle() {
            if (routingProductId == undefined)
                return;

            return WhS_BE_RoutingProductAPIService.GetRoutingProduct(routingProductId).then(function (response) {
                $scope.title += " of " + response.Name;
            });
        }

        function loadFilterBySection() {
            if (!$scope.scopeModal.showRouteOptionRuleTypeFilterSection)
                return;

            $scope.scopeModal.routeOptionRuleCriteriaTypes = UtilsService.getArrayEnum(WhS_Routing_RouteRuleCriteriaTypeEnum);

            if (routeOptionRuleEntity != undefined && routeOptionRuleEntity.Criteria.CodeCriteriaGroupSettings != null)
                $scope.scopeModal.selectedRouteOptionRuleCriteriaType = UtilsService.getEnum(WhS_Routing_RouteRuleCriteriaTypeEnum, 'value', WhS_Routing_RouteRuleCriteriaTypeEnum.Code.value);
            else
                $scope.scopeModal.selectedRouteOptionRuleCriteriaType = UtilsService.getEnum(WhS_Routing_RouteRuleCriteriaTypeEnum, 'value', WhS_Routing_RouteRuleCriteriaTypeEnum.SaleZone.value);
        }

        function loadSaleZoneGroupSection() {
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

                var saleZoneGroupPayload;

                if (routeOptionRuleEntity != undefined) {
                    saleZoneGroupPayload.sellingNumberPlanId = routeOptionRuleEntity.Criteria.SaleZoneGroupSettings != undefined ? routeOptionRuleEntity.Criteria.SaleZoneGroupSettings.SellingNumberPlanId : undefined;
                    saleZoneGroupPayload.saleZoneGroupSettings = routeOptionRuleEntity.Criteria.SaleZoneGroupSettings
                }

                saleZoneGroupSettingsReadyPromiseDeferred = undefined;
                VRUIUtilsService.callDirectiveLoad(saleZoneGroupSettingsAPI, saleZoneGroupPayload, saleZoneGroupSettingsLoadPromiseDeferred);
            });
            return UtilsService.waitMultiplePromises(promises);
        }

        function loadCodeCriteriaGroupSection() {
            var promises = [];
            var codeCriteriaGroupPayload;

            if (routeOptionRuleEntity != undefined && routeOptionRuleEntity.Criteria.CodeCriteriaGroupSettings != null)
                codeCriteriaGroupPayload = routeOptionRuleEntity.Criteria.CodeCriteriaGroupSettings;

            var loadCodeCriteriaGroupTemplatesPromise = WhS_Routing_RouteOptionRuleAPIService.GetCodeCriteriaGroupTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.scopeModal.codeCriteriaGroupTemplates.push(item);
                });

                if (codeCriteriaGroupPayload != undefined)
                    $scope.scopeModal.selectedCodeCriteriaGroupTemplate = UtilsService.getItemByVal($scope.scopeModal.codeCriteriaGroupTemplates, codeCriteriaGroupPayload.ConfigId, "TemplateConfigID");
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
                if (routeOptionRuleEntity != undefined && routeOptionRuleEntity.Criteria.CustomerGroupSettings != null)
                    customerGroupPayload = routeOptionRuleEntity.Criteria.CustomerGroupSettings;

                customerGroupSettingsReadyPromiseDeferred = undefined;
                VRUIUtilsService.callDirectiveLoad(customerGroupSettingsAPI, customerGroupPayload, customerGroupSettingsLoadPromiseDeferred);
            });


            return UtilsService.waitMultiplePromises(promises);
        }

        function loadRouteOptionRuleSettingsSection() {
            var promises = [];
            var routeOptionRuleSettingsPayload;

            if (routeOptionRuleEntity != undefined && routeOptionRuleEntity.Settings != null) {
                routeOptionRuleSettingsPayload = {
                    SupplierFilterSettings: { RoutingProductId: routingProductId },
                    RouteOptionRuleSettings: routeOptionRuleEntity.Settings
                }
            }

            var loadRouteOptionRuleSettingsTemplatesPromise = WhS_Routing_RouteOptionRuleAPIService.GetRouteOptionRuleSettingsTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.scopeModal.routeOptionRuleSettingsTemplates.push(item);
                });

                if (routeOptionRuleSettingsPayload != undefined)
                    $scope.scopeModal.selectedrouteOptionRuleSettingsTemplate = UtilsService.getItemByVal($scope.scopeModal.routeOptionRuleSettingsTemplates, routeOptionRuleSettingsPayload.RouteOptionRuleSettings.ConfigId, "TemplateConfigID");
            });

            promises.push(loadRouteOptionRuleSettingsTemplatesPromise);

            if (routeOptionRuleSettingsPayload != undefined) {
                routeOptionRuleSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                var routeOptionRuleSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(routeOptionRuleSettingsLoadPromiseDeferred.promise);

                routeOptionRuleSettingsReadyPromiseDeferred.promise.then(function () {
                    routeOptionRuleSettingsReadyPromiseDeferred = undefined;
                    VRUIUtilsService.callDirectiveLoad(routeOptionRuleSettingsAPI, routeOptionRuleSettingsPayload, routeOptionRuleSettingsLoadPromiseDeferred);
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function loadStaticSection() {
            if (routeOptionRuleEntity == undefined)
                return;

            $scope.scopeModal.beginEffectiveDate = routeOptionRuleEntity.BeginEffectiveTime;
            $scope.scopeModal.endEffectiveDate = routeOptionRuleEntity.EndEffectiveTime;

            if (routeOptionRuleEntity.Criteria != null) {

                angular.forEach(routeOptionRuleEntity.Criteria.ExcludedCodes, function (item) {
                    $scope.scopeModal.excludedCodes.push(item);
                });
            }
        }

        function buildRouteOptionRuleObjFromScope() {

            var routeOptionRule = {
                OptionRuleId: (routeOptionRuleId != null) ? routeOptionRuleId : 0,
                Criteria: {
                    RoutingProductId: routingProductId,
                    ExcludedCodes: $scope.scopeModal.excludedCodes,
                    SaleZoneGroupSettings: saleZoneGroupSettingsAPI.getData(),//VRUIUtilsService.getSettingsFromDirective($scope, saleZoneGroupSettingsAPI, 'selectedSaleZoneGroupTemplate'),
                    CustomerGroupSettings: customerGroupSettingsAPI.getData(),
                    CodeCriteriaGroupSettings: VRUIUtilsService.getSettingsFromDirective($scope.scopeModal, codeCriteriaGroupSettingsAPI, 'selectedCodeCriteriaGroupTemplate')
                },
                Settings: VRUIUtilsService.getSettingsFromDirective($scope.scopeModal, routeOptionRuleSettingsAPI, 'selectedrouteOptionRuleSettingsTemplate'),
                BeginEffectiveTime: $scope.scopeModal.beginEffectiveDate,
                EndEffectiveTime: $scope.scopeModal.endEffectiveDate
            };

            return routeOptionRule;
        }

        function insertRouteOptionRule() {
            var routeRuleObject = buildRouteOptionRuleObjFromScope();
            return WhS_Routing_RouteOptionRuleAPIService.AddRule(routeRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Route Option Rule", response)) {
                    if ($scope.onRouteOptionRuleAdded != undefined)
                        $scope.onRouteOptionRuleAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateRouteOptionRule() {
            var routeRuleObject = buildRouteOptionRuleObjFromScope();
            WhS_Routing_RouteOptionRuleAPIService.UpdateRule(routeRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Route Option Rule", response)) {
                    if ($scope.onRouteOptionRuleUpdated != undefined)
                        $scope.onRouteOptionRuleUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('WhS_Routing_RouteOptionRuleEditorController', routeOptionRuleEditorController);
})(appControllers);
