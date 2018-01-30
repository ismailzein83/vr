(function (appControllers) {

    "use strict";

    routeOptionRuleEditorController.$inject = ['$scope', 'WhS_Routing_RouteOptionRuleAPIService', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_SaleZoneAPIService', 'WhS_BE_CarrierAccountAPIService',
        'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_Routing_RouteRuleCriteriaTypeEnum', 'WhS_Routing_RouteRuleAPIService', 'VRDateTimeService'];

    function routeOptionRuleEditorController($scope, WhS_Routing_RouteOptionRuleAPIService, WhS_BE_RoutingProductAPIService, WhS_BE_SaleZoneAPIService, WhS_BE_CarrierAccountAPIService,
        UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_Routing_RouteRuleCriteriaTypeEnum, WhS_Routing_RouteRuleAPIService, VRDateTimeService) {

        var isLinkedRouteOptionRule;
        var linkedRouteOptionRuleInput;
        var linkedCode;

        var isViewHistoryMode;
        var isEditMode;
        var context;

        var routeRuleId;
        var routingProductId;
        var sellingNumberPlanId;

        var routeOptionRuleEntity;
        var settingsEditorRuntime;

        var routeOptionRuleSettingsTypeSelectorAPI;
        var routeOptionRuleSettingsTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneGroupSettingsAPI;
        var saleZoneGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var codeCriteriaGroupSettingsAPI;
        var codeCriteriaGroupSettingsReadyPromiseDeferred;

        var customerGroupSettingsAPI;
        var customerGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var routeOptionRuleSettingsAPI;
        var routeOptionRuleSettingsReadyPromiseDeferred;

        var suppliersWithZonesGroupSettingsAPI;
        var suppliersWithZonesGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var excludedDestinationsDirectiveAPI;
        var excludedDestinationsDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                context = parameters.context;
                routeRuleId = parameters.routeRuleId;
                routingProductId = parameters.routingProductId;
                sellingNumberPlanId = parameters.sellingNumberPlanId;
                linkedRouteOptionRuleInput = parameters.linkedRouteOptionRuleInput;
                isLinkedRouteOptionRule = parameters.isLinkedRouteRule;
                linkedCode = parameters.linkedCode;
            }

            isEditMode = routeRuleId != undefined && (linkedRouteOptionRuleInput == undefined);
            isViewHistoryMode = (context != undefined && context.historyId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.disableCriteria = isLinkedRouteOptionRule;
            $scope.scopeModel.showCriteriaSection = false;
            $scope.scopeModel.showSettingsSection = false;
            $scope.scopeModel.beginEffectiveDate = VRDateTimeService.getCurrentDateWithoutMilliseconds();
            $scope.scopeModel.endEffectiveDate = undefined;

            $scope.scopeModel.saleZoneGroupTemplates = [];
            $scope.scopeModel.codeCriteriaGroupTemplates = [];
            $scope.scopeModel.customerGroupTemplates = [];
            //$scope.scopeModel.excludedCodes = [];
            $scope.scopeModel.routeOptionRuleSettingsTemplates = [];

            $scope.scopeModel.onRouteOptionRuleSettingsTypeSelectorReady = function (api) {
                routeOptionRuleSettingsTypeSelectorAPI = api;
                routeOptionRuleSettingsTypeSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onSuppliersWithZonesGroupSettingsDirectiveReady = function (api) {
                suppliersWithZonesGroupSettingsAPI = api;
                suppliersWithZonesGroupSettingsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onSaleZoneGroupSettingsDirectiveReady = function (api) {
                saleZoneGroupSettingsAPI = api;
                var saleZoneGroupPayload = {
                    sellingNumberPlanId: sellingNumberPlanId != undefined ? sellingNumberPlanId : undefined,
                    saleZoneFilterSettings: { RoutingProductId: routingProductId }
                };
                var setLoader = function (value) { setTimeout(function () { $scope.scopeModel.isLoadingSaleZoneGroup = value; UtilsService.safeApply($scope); }); };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneGroupSettingsAPI, saleZoneGroupPayload, setLoader, saleZoneGroupSettingsReadyPromiseDeferred);
            };
            $scope.scopeModel.onCodeCriteriaGroupSettingsDirectiveReady = function (api) {
                codeCriteriaGroupSettingsAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingCustomersSection = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, codeCriteriaGroupSettingsAPI, undefined, setLoader, codeCriteriaGroupSettingsReadyPromiseDeferred);
            };
            $scope.scopeModel.onCustomerGroupSettingsDirectiveReady = function (api) {
                customerGroupSettingsAPI = api;
                customerGroupSettingsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onRouteOptionRuleSettingsDirectiveReady = function (api) {
                routeOptionRuleSettingsAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingRouteOptionRuleSettings = value; };

                var routeOptionRuleSettingsPayload = {
                    SupplierFilterSettings: { RoutingProductId: routingProductId }
                };

                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, routeOptionRuleSettingsAPI, routeOptionRuleSettingsPayload, setLoader, routeOptionRuleSettingsReadyPromiseDeferred);
            };
            $scope.scopeModel.onExcludedDestinationsDirectiveReady = function (api) {
                excludedDestinationsDirectiveAPI = api;
                excludedDestinationsDirectiveReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onRouteOptionRuleCriteriaTypeSelectionChanged = function () {

                if ($scope.scopeModel.selectedRouteOptionRuleCriteriaType == undefined)
                    return;

                if ($scope.scopeModel.selectedRouteOptionRuleCriteriaType == WhS_Routing_RouteRuleCriteriaTypeEnum.SaleZone) {
                    $scope.scopeModel.showSaleZoneSection = $scope.scopeModel.showCustomerSection = $scope.scopeModel.showExcludedCodeSection = true;
                    $scope.scopeModel.showIncludedCodeSection = false;
                }
                else {
                    $scope.scopeModel.selectedCodeCriteriaGroupTemplate = undefined;
                    $scope.scopeModel.showIncludedCodeSection = $scope.scopeModel.showCustomerSection = $scope.scopeModel.showExcludedCodeSection = true;
                    $scope.scopeModel.showSaleZoneSection = false;
                }
            };
            $scope.scopeModel.onRouteOptionRuleSettingsTypeSelectionChanged = function () {

                var _selectedItem = routeOptionRuleSettingsTypeSelectorAPI.getSelectedIds();
                if (_selectedItem != undefined) {

                    $scope.scopeModel.showCriteriaSection = $scope.scopeModel.showSettingsSection = true;
                    //$scope.scopeModel.selectedCodeCriteriaGroupTemplate = undefined;
                    //$scope.scopeModel.excludedCodes = [];
                    //reloadSuppliersWithZonesGroupSection();
                    //reloadFilterBySection();
                    //reloadSaleZoneGroupSection();
                    //reloadCustomerGroupSection();
                }

                function reloadSuppliersWithZonesGroupSection() {
                    if (suppliersWithZonesGroupSettingsAPI == undefined)
                        return;

                    var suppliersWithZonesGroupSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    var payload = {
                        sellingNumberPlanId: sellingNumberPlanId != undefined ? sellingNumberPlanId : undefined,
                        saleZoneFilterSettings: { RoutingProductId: routingProductId },
                    };
                    VRUIUtilsService.callDirectiveLoad(suppliersWithZonesGroupSettingsAPI, payload, suppliersWithZonesGroupSettingsLoadPromiseDeferred);

                    return suppliersWithZonesGroupSettingsLoadPromiseDeferred.promise;
                }
                function reloadFilterBySection() {
                    if (!$scope.scopeModel.showRouteOptionRuleTypeFilterSection)
                        return;

                    $scope.scopeModel.routeOptionRuleCriteriaTypes = UtilsService.getArrayEnum(WhS_Routing_RouteRuleCriteriaTypeEnum);
                    if (routeOptionRuleEntity != undefined && routeOptionRuleEntity.Criteria.CodeCriteriaGroupSettings != undefined)
                        $scope.scopeModel.selectedRouteOptionRuleCriteriaType = UtilsService.getEnum(WhS_Routing_RouteRuleCriteriaTypeEnum, 'value', WhS_Routing_RouteRuleCriteriaTypeEnum.Code.value);
                    else
                        $scope.scopeModel.selectedRouteOptionRuleCriteriaType = UtilsService.getEnum(WhS_Routing_RouteRuleCriteriaTypeEnum, 'value', WhS_Routing_RouteRuleCriteriaTypeEnum.SaleZone.value);
                }
                function reloadSaleZoneGroupSection() {
                    if (saleZoneGroupSettingsAPI == undefined)
                        return;

                    var saleZoneGroupSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    var saleZoneGroupPayload = {
                        sellingNumberPlanId: sellingNumberPlanId != undefined ? sellingNumberPlanId : undefined,
                        saleZoneFilterSettings: { RoutingProductId: routingProductId }
                    };
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
            };

            //TODO: make the validation below a custom validation
            //$scope.scopeModel.addExcludedCode = function () {
            //    var codeIsValid = true;

            //    if ($scope.scopeModel.excludedCode == undefined || $scope.scopeModel.excludedCode.length == 0) {
            //        codeIsValid = false;
            //    }
            //    else {
            //        angular.forEach($scope.scopeModel.excludedCodes, function (item) {
            //            if ($scope.scopeModel.excludedCode === item) {
            //                codeIsValid = false;
            //            }
            //        });
            //    }

            //    if (codeIsValid)
            //        $scope.scopeModel.excludedCodes.push($scope.scopeModel.excludedCode);
            //};
            //$scope.scopeModel.removeExcludedCode = function (codeToRemove) {
            //    $scope.scopeModel.excludedCodes.splice($scope.scopeModel.excludedCodes.indexOf(codeToRemove), 1);
            //};
            //$scope.scopeModel.validateExcludedCodes = function () {
            //    if (isLinkedRouteOptionRule) {
            //        if ($scope.scopeModel.excludedCodes != null && $scope.scopeModel.excludedCodes.length > 0) {
            //            for (var x = 0; x < $scope.scopeModel.excludedCodes.length; x++) {
            //                var currentExcludedCode = $scope.scopeModel.excludedCodes[x];
            //                if (linkedCode == currentExcludedCode) {
            //                    return linkedCode + ' cannot be excluded.';
            //                }
            //            }
            //        }
            //    }
            //    return null;
            //};

            $scope.scopeModel.SaveRouteOptionRule = function () {
                if (isEditMode) {
                    return updateRouteOptionRule();
                }
                else {
                    return insertRouteOptionRule();
                }
            };

            $scope.hasSaveRulePermission = function () {
                if (isEditMode)
                    return WhS_Routing_RouteOptionRuleAPIService.HasUpdateRulePermission();
                else
                    return WhS_Routing_RouteOptionRuleAPIService.HasAddRulePermission();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                $scope.title = "Edit Route Option Rule";
                getRouteOptionRule().then(function () {
                    loadAllControls().finally(function () {
                        routeOptionRuleEntity = undefined;
                    });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else if (isLinkedRouteOptionRule) {
                $scope.title = "New Route Option Rule";
                buildLinkedRouteOptionRule().then(function () {
                    loadAllControls()
                        .finally(function () {
                            routeRuleEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else if (isViewHistoryMode) {
                geRouteOptionRuleHistory().then(function () {
                    loadAllControls().finally(function () {
                        routeOptionRuleEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                $scope.title = "New Route Option Rule";
                loadAllControls();
            }
        }

        function geRouteOptionRuleHistory() {
            return WhS_Routing_RouteOptionRuleAPIService.GetRouteOptionRuleHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                routeOptionRuleEntity = response;
            });
        }
        function getRouteOptionRule() {
            return WhS_Routing_RouteOptionRuleAPIService.GetRuleEditorRuntime(routeRuleId).then(function (routeOptionRule) {
                routeOptionRuleEntity = routeOptionRule.Entity;
                settingsEditorRuntime = routeOptionRule.SettingsEditorRuntime;

                $scope.scopeModel.routeOptionRuleName = routeOptionRuleEntity != null ? routeOptionRuleEntity.Name : '';
                routingProductId = routeOptionRuleEntity.Criteria != null ? routeOptionRuleEntity.Criteria.RoutingProductId : undefined;
            });
        }

        function loadAllControls() {
            var loadAllControlsPromiseDeferred = UtilsService.createPromiseDeferred();

            var loadRouteOptionRuleSettingsSectionPromise = loadRouteOptionRuleSettingsSection();
            loadRouteOptionRuleSettingsSectionPromise.then(function () {
                UtilsService.waitMultipleAsyncOperations([displaySectionsBasedOnParameters, editScopeTitle, loadFilterBySection, loadSaleZoneGroupSection,
                    loadCustomerGroupSection, loadSuppliersWithZonesGroupSection, loadCodeCriteriaGroupSection, loadStaticSection, loadExcludedDestinationsDirective])
                    .catch(function (error) {
                        loadAllControlsPromiseDeferred.reject(error);
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    })
                    .finally(function () {
                        loadAllControlsPromiseDeferred.resolve();
                        $scope.scopeModel.isLoading = false;
                    });
            }).catch(function (error) {
                loadAllControlsPromiseDeferred.reject(error);
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });

            return loadAllControlsPromiseDeferred.promise;
        }
        function loadRouteOptionRuleSettingsSection() {
            var promises = [];
            var routeOptionRuleSettingsPayload;

            if (routeOptionRuleEntity != undefined && routeOptionRuleEntity.Settings != null) {
                routeOptionRuleSettingsPayload = {
                    SupplierFilterSettings: { RoutingProductId: routingProductId },
                    RouteOptionRuleSettings: routeOptionRuleEntity.Settings,
                    SettingsEditorRuntime: settingsEditorRuntime
                };
            }

            //loading RouteOptionRuleSettingsTypeSelector
            var loadRouteOptionRuleSettingsTemplatesPromise = UtilsService.createPromiseDeferred();
            promises.push(loadRouteOptionRuleSettingsTemplatesPromise.promise);

            var routeOptionRuleSettingsTypePayload = {};
            if (routeOptionRuleEntity != undefined && routeOptionRuleEntity.Settings != undefined) {
                routeOptionRuleSettingsTypePayload.selectedIds = routeOptionRuleEntity.Settings.ConfigId;
            }
            routeOptionRuleSettingsTypeSelectorReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(routeOptionRuleSettingsTypeSelectorAPI, routeOptionRuleSettingsTypePayload, loadRouteOptionRuleSettingsTemplatesPromise);
            });


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
        function displaySectionsBasedOnParameters() {
            $scope.scopeModel.showSaleZoneSection = routingProductId != undefined;
            $scope.scopeModel.showRouteOptionRuleTypeFilterSection = $scope.scopeModel.showCustomerSection = $scope.scopeModel.showExcludedCodeSection = $scope.scopeModel.showIncludedCodeSection = !$scope.scopeModel.showSaleZoneSection;
        }
        function editScopeTitle() {
            if (routingProductId == undefined)
                return;

            return WhS_BE_RoutingProductAPIService.GetRoutingProduct(routingProductId).then(function (response) {
                $scope.title += " of " + response.Name;
            });
        }
        function loadFilterBySection() {
            if (!$scope.scopeModel.showRouteOptionRuleTypeFilterSection)
                return;

            $scope.scopeModel.routeOptionRuleCriteriaTypes = UtilsService.getArrayEnum(WhS_Routing_RouteRuleCriteriaTypeEnum);

            if (routeOptionRuleEntity != undefined && routeOptionRuleEntity.Criteria.CodeCriteriaGroupSettings != undefined)
                $scope.scopeModel.selectedRouteOptionRuleCriteriaType = UtilsService.getEnum(WhS_Routing_RouteRuleCriteriaTypeEnum, 'value', WhS_Routing_RouteRuleCriteriaTypeEnum.Code.value);
            else
                $scope.scopeModel.selectedRouteOptionRuleCriteriaType = UtilsService.getEnum(WhS_Routing_RouteRuleCriteriaTypeEnum, 'value', WhS_Routing_RouteRuleCriteriaTypeEnum.SaleZone.value);
        }
        function loadSaleZoneGroupSection() {
            if (routeOptionRuleEntity == undefined || routeOptionRuleEntity.Criteria == undefined || routeOptionRuleEntity.Criteria.SaleZoneGroupSettings == undefined) {
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
                    saleZoneFilterSettings: { RoutingProductId: routingProductId }
                };


                if (routeOptionRuleEntity != undefined) {
                    saleZoneGroupPayload.sellingNumberPlanId = routeOptionRuleEntity.Criteria.SaleZoneGroupSettings != undefined ? routeOptionRuleEntity.Criteria.SaleZoneGroupSettings.SellingNumberPlanId : undefined;
                    saleZoneGroupPayload.saleZoneGroupSettings = routeOptionRuleEntity.Criteria.SaleZoneGroupSettings;
                }

                saleZoneGroupSettingsReadyPromiseDeferred = undefined;
                VRUIUtilsService.callDirectiveLoad(saleZoneGroupSettingsAPI, saleZoneGroupPayload, saleZoneGroupSettingsLoadPromiseDeferred);
            });
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
        function loadSuppliersWithZonesGroupSection() {
            var promises = [];

            if (suppliersWithZonesGroupSettingsReadyPromiseDeferred == undefined)
                suppliersWithZonesGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var suppliersWithZonesGroupSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            promises.push(suppliersWithZonesGroupSettingsLoadPromiseDeferred.promise);

            suppliersWithZonesGroupSettingsReadyPromiseDeferred.promise.then(function () {

                var payload = {
                    sellingNumberPlanId: sellingNumberPlanId != undefined ? sellingNumberPlanId : undefined,
                    saleZoneFilterSettings: { RoutingProductId: routingProductId },
                };

                if (routeOptionRuleEntity != undefined) {
                    payload.SuppliersWithZonesGroupSettings = routeOptionRuleEntity.Criteria.SuppliersWithZonesGroupSettings;
                }

                suppliersWithZonesGroupSettingsReadyPromiseDeferred = undefined;
                VRUIUtilsService.callDirectiveLoad(suppliersWithZonesGroupSettingsAPI, payload, suppliersWithZonesGroupSettingsLoadPromiseDeferred);
            });
            return UtilsService.waitMultiplePromises(promises);
        }
        function loadCodeCriteriaGroupSection() {
            var promises = [];
            var codeCriteriaGroupPayload;

            if (routeOptionRuleEntity != undefined && routeOptionRuleEntity.Criteria.CodeCriteriaGroupSettings != null)
                codeCriteriaGroupPayload = routeOptionRuleEntity.Criteria.CodeCriteriaGroupSettings;

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
        function loadStaticSection() {
            if (routeOptionRuleEntity == undefined)
                return;

            $scope.scopeModel.beginEffectiveDate = routeOptionRuleEntity.BeginEffectiveTime;
            $scope.scopeModel.endEffectiveDate = routeOptionRuleEntity.EndEffectiveTime;

            //if (routeOptionRuleEntity.Criteria != null) {

            //    angular.forEach(routeOptionRuleEntity.Criteria.ExcludedCodes, function (item) {
            //        $scope.scopeModel.excludedCodes.push(item);
            //    });
            //}
        }
        function loadExcludedDestinationsDirective() {
            var loadExcludedDestinationsDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            excludedDestinationsDirectiveReadyPromiseDeferred.promise.then(function () {

                var excludedDestinationsDirectivePayload = {
                    excludedDestinations: (routeOptionRuleEntity && routeOptionRuleEntity.Criteria) ? routeOptionRuleEntity.Criteria.ExcludedDestinations : undefined,
                    isLinkedRouteRule: isLinkedRouteOptionRule,
                    linkedCode: linkedCode
                };
                VRUIUtilsService.callDirectiveLoad(excludedDestinationsDirectiveAPI, excludedDestinationsDirectivePayload, loadExcludedDestinationsDirectivePromiseDeferred);
            });

            return loadExcludedDestinationsDirectivePromiseDeferred.promise;
        }

        function insertRouteOptionRule() {
            var routeRuleObject = buildRouteOptionRuleObjFromScope();

            return WhS_Routing_RouteOptionRuleAPIService.AddRule(routeRuleObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Route Option Rule", response, "Name")) {
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

            WhS_Routing_RouteOptionRuleAPIService.UpdateRule(routeRuleObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Route Option Rule", response, "Name")) {
                    if ($scope.onRouteOptionRuleUpdated != undefined)
                        $scope.onRouteOptionRuleUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        function buildLinkedRouteOptionRule() {
            return WhS_Routing_RouteOptionRuleAPIService.BuildLinkedRouteOptionRule(linkedRouteOptionRuleInput).then(function (routeOptionRule) {
                routeOptionRuleEntity = routeOptionRule;
            });
        }
        function buildRouteOptionRuleObjFromScope() {

            var routeOptionRule = {
                RuleId: (routeRuleId != null) ? routeRuleId : 0,
                Name: $scope.scopeModel.routeOptionRuleName,
                Criteria: {
                    RoutingProductId: routingProductId,
                    //ExcludedCodes: $scope.scopeModel.excludedCodes,
                    ExcludedDestinations: excludedDestinationsDirectiveAPI.getData(),
                    SaleZoneGroupSettings: $scope.scopeModel.showSaleZoneSection ? saleZoneGroupSettingsAPI.getData() : undefined, //VRUIUtilsService.getSettingsFromDirective($scope, saleZoneGroupSettingsAPI, 'selectedSaleZoneGroupTemplate'),
                    CustomerGroupSettings: customerGroupSettingsAPI.getData(),
                    CodeCriteriaGroupSettings: $scope.scopeModel.showIncludedCodeSection ? VRUIUtilsService.getSettingsFromDirective($scope.scopeModel, codeCriteriaGroupSettingsAPI, 'selectedCodeCriteriaGroupTemplate') : undefined,
                    SuppliersWithZonesGroupSettings: suppliersWithZonesGroupSettingsAPI.getData()
                },
                Settings: VRUIUtilsService.getSettingsFromDirective($scope.scopeModel, routeOptionRuleSettingsAPI, 'selectedRouteOptionRuleSettingsTemplate'),
                BeginEffectiveTime: $scope.scopeModel.beginEffectiveDate,
                EndEffectiveTime: $scope.scopeModel.endEffectiveDate
            };
            return routeOptionRule;
        }
    }

    appControllers.controller('WhS_Routing_RouteOptionRuleEditorController', routeOptionRuleEditorController);
})(appControllers);
