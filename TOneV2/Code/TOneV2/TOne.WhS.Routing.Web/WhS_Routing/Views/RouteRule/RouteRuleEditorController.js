(function (appControllers) {

    "use strict";

    routeRuleEditorController.$inject = ['$scope', 'WhS_Routing_RouteRuleAPIService', 'WhS_BE_RoutingProductAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VRDateTimeService', 'WhS_Routing_SupplierZoneDetailsAPIService'];

    function routeRuleEditorController($scope, WhS_Routing_RouteRuleAPIService, WhS_BE_RoutingProductAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VRDateTimeService, WhS_Routing_SupplierZoneDetailsAPIService) {

        var isLinkedRouteRule;
        var customerRouteData;
        var linkedRouteRuleInput;
        var isEditMode;
        var isViewHistoryMode;
        var routeRuleId;
        var routingProductId;
        var sellingNumberPlanId;
        var defaultRouteRuleValues;
        var context;
        var minBED;
        var maxEED;

        var routeRuleEntity;
        var supplierZoneDetails;
        var productRouteEntity;

        var routeRuleSettingsTypeSelectorAPI;
        var routeRuleSettingsTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var routeRuleCriteriaSelectorAPI;
        var routeRuleCriteriaSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var routeRuleSettingsAPI;
        var routeRuleSettingsReadyPromiseDeferred;

        var routeRuleCriteriaAPI;
        var routeRuleCriteriaReadyPromiseDeferred;


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
                linkedRouteRuleInput = parameters.linkedRouteRuleInput;
                isLinkedRouteRule = parameters.isLinkedRouteRule;
                customerRouteData = parameters.customerRouteData;
                defaultRouteRuleValues = parameters.defaultRouteRuleValues;
            }

            isEditMode = routeRuleId != undefined && (linkedRouteRuleInput == undefined);
            isViewHistoryMode = (context != undefined && context.historyId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.disableCriteria = isLinkedRouteRule;
            $scope.scopeModel.showCriteriaSection = false;
            $scope.scopeModel.showSettingsSection = false;
            $scope.scopeModel.beginEffectiveDate = VRDateTimeService.getCurrentDateWithoutMilliseconds();
            $scope.scopeModel.endEffectiveDate = undefined;

            $scope.scopeModel.routeRuleSettingsTemplates = [];

            $scope.scopeModel.onRouteRuleSettingsTypeSelectorReady = function (api) {
                routeRuleSettingsTypeSelectorAPI = api;
                routeRuleSettingsTypeSelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onRouteRuleCriteriaSelectorReady = function (api) {
                routeRuleCriteriaSelectorAPI = api;
                routeRuleCriteriaSelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onRouteRuleSettingsDirectiveReady = function (api) {
                routeRuleSettingsAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingRouteRuleSettings = value; };

                var routeRuleSettingsPayload = {
                    SupplierFilterSettings: { RoutingProductId: routingProductId },
                    supplierZoneDetails: supplierZoneDetails,
                    customerRouteData: customerRouteData
                };

                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, routeRuleSettingsAPI, routeRuleSettingsPayload, setLoader, routeRuleSettingsReadyPromiseDeferred);
            };

            $scope.scopeModel.onRouteRuleCriteriaDirectiveReady = function (api) {
                routeRuleCriteriaAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingRouteRuleCriteria = value; };
                var routeRuleCriteriaPayload = {
                    isLinkedRouteRule: isLinkedRouteRule,
                    routingProductId: routingProductId,
                    sellingNumberPlanId: sellingNumberPlanId,
                    linkedCode: customerRouteData != undefined ? customerRouteData.code : undefined,
                    defaultCriteriaValues: defaultRouteRuleValues != undefined ? defaultRouteRuleValues.criteria : undefined,
                    routeRuleCriteriaContext: buildRouteRuleCriteriaContext(),
                    linkedCustomerId: customerRouteData != undefined ? customerRouteData.CustomerId : undefined
                };

                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, routeRuleCriteriaAPI, routeRuleCriteriaPayload, setLoader, routeRuleCriteriaReadyPromiseDeferred);
            };

            $scope.scopeModel.onRouteRuleSettingsTypeSelectionChanged = function () {

                var _selectedItem = routeRuleSettingsTypeSelectorAPI.getSelectedIds();
                if (_selectedItem != undefined) {
                    $scope.scopeModel.showSettingsSection = true;
                }
            };

            $scope.scopeModel.onRouteRuleCriteriaSelectionChanged = function () {

                var _selectedItem = routeRuleCriteriaSelectorAPI.getSelectedIds();
                if (_selectedItem != undefined) {
                    $scope.scopeModel.showCriteriaSection = true;
                }
            };

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
            };

            //$scope.scopeModel.validateDates = function (date) {
            //    if ($scope.scopeModel.beginEffectiveDate != undefined && $scope.scopeModel.endEffectiveDate != undefined) {
            //        var validationResult = UtilsService.validateDates($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
            //        if (validationResult != null)
            //            return validationResult;
            //    }
            //}

            $scope.scopeModel.validateBED = function () {
                if ($scope.scopeModel.beginEffectiveDate != undefined && $scope.scopeModel.endEffectiveDate != undefined) {
                    var validationResult = UtilsService.validateDates($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
                    if (validationResult != null)
                        return validationResult;
                }

                var bed = $scope.scopeModel.beginEffectiveDate;
                if (bed != undefined && !(bed instanceof Date))
                    bed = UtilsService.createDateFromString(bed);

                if (minBED != undefined && bed != undefined && bed < minBED) {
                    return 'BED should be greater than ' + UtilsService.dateToServerFormat(minBED);
                }
                return null;
            };

            $scope.scopeModel.validateEED = function () {
                if ($scope.scopeModel.beginEffectiveDate != undefined && $scope.scopeModel.endEffectiveDate != undefined) {
                    var validationResult = UtilsService.validateDates($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
                    if (validationResult != null)
                        return validationResult;
                }

                var eed = $scope.scopeModel.endEffectiveDate;
                if (eed != undefined && !(eed instanceof Date))
                    eed = UtilsService.createDateFromString(eed);

                if (maxEED != undefined && (eed == undefined || eed > maxEED)) {
                    return 'EED should be less than ' + UtilsService.dateToServerFormat(maxEED);
                }
                return null;
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor("", "Route Rule", $scope);
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
            else if (isLinkedRouteRule) {
                $scope.title = "New Route Rule";
                buildLinkedRouteRule().then(function () {
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
                getRouteRuleHistory().then(function () {
                    loadAllControls().finally(function () {
                        routeRuleEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });

            }
            else {
                $scope.title = "New Route Rule";
                loadAllControls();
            }
        }

        function getRouteRuleHistory() {
            return WhS_Routing_RouteRuleAPIService.GetRouteRuleHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                routeRuleEntity = response;

            });
        }

        function getRouteRule() { 
            return WhS_Routing_RouteRuleAPIService.GetRule(routeRuleId).then(function (routeRule) {
                if (routeRule != undefined) {
                    $scope.scopeModel.routeRuleName = routeRule != null ? routeRule.Name : '';
                    $scope.scopeModel.routeRuleReason = routeRule != null ? routeRule.Reason : '';

                    routeRuleEntity = routeRule;
                    routingProductId = routeRuleEntity.Criteria != null ? routeRuleEntity.Criteria.RoutingProductId : undefined;
                }
                else {
                    VRNotificationService.showWarning("Matching Rule has been deleted!!");
                    $scope.modalContext.closeModal();
                }
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

        function getProductRoute() {
            if (routingProductId != undefined)
                return WhS_BE_RoutingProductAPIService.GetRoutingProduct(routingProductId).then(function (response) {
                    productRouteEntity = response;
                    sellingNumberPlanId = productRouteEntity.SellingNumberPlanId;
                });
        }

        function loadData() {
            var loadDataPromiseDeferred = UtilsService.createPromiseDeferred();

            var loadSupplierZoneDetailsPromise = loadSupplierZoneDetails();

            loadSupplierZoneDetailsPromise.then(function () {
                var loadRouteRuleSettingsSectionPromise = loadRouteRuleSettingsSection();
                var loadRouteRuleCriteriaSectionPromise = loadRouteRuleCriteriaSection();

                UtilsService.waitMultiplePromises([loadRouteRuleSettingsSectionPromise, loadRouteRuleCriteriaSectionPromise]).then(function () {
                    UtilsService.waitMultipleAsyncOperations([editScopeTitle, loadStaticSection])
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
            }).catch(function (error) {
                loadDataPromiseDeferred.reject(error);
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });

            return loadDataPromiseDeferred.promise;
        }

        function loadSupplierZoneDetails() {
            var promises = [];
            if (customerRouteData != undefined) {
                var loadSupplierZoneDetailsPromise = WhS_Routing_SupplierZoneDetailsAPIService.GetSupplierZoneDetailsByCode(customerRouteData.code).then(function (response) {
                    supplierZoneDetails = response;
                });
                promises.push(loadSupplierZoneDetailsPromise);
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function loadRouteRuleSettingsSection() {
            var promises = [];
            var routeRuleSettingsPayload;

            if (routeRuleEntity != undefined && routeRuleEntity.Settings != null) {
                routeRuleSettingsPayload = {
                    SupplierFilterSettings: { RoutingProductId: routingProductId },
                    RouteRuleSettings: routeRuleEntity.Settings
                };
            }

            //loading RouteRuleSettingsTypeSelector
            var loadRouteRuleSettingsTemplatesPromise = UtilsService.createPromiseDeferred();
            promises.push(loadRouteRuleSettingsTemplatesPromise.promise);

            var routeRuleSettingsTypePayload = {};
            if (routeRuleEntity != undefined && routeRuleEntity.Settings != undefined) {
                routeRuleSettingsTypePayload.selectedIds = routeRuleEntity.Settings.ConfigId;
            }
            routeRuleSettingsTypeSelectorReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(routeRuleSettingsTypeSelectorAPI, routeRuleSettingsTypePayload, loadRouteRuleSettingsTemplatesPromise);
            });

            if (routeRuleSettingsPayload != undefined) {
                routeRuleSettingsPayload.supplierZoneDetails = supplierZoneDetails;
                routeRuleSettingsPayload.customerRouteData = customerRouteData;

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

        function loadRouteRuleCriteriaSection() {
            var promises = [];
            var routeRuleCriteriaPayload;

            if (routeRuleEntity != undefined && routeRuleEntity.Criteria != null) {
                routeRuleCriteriaPayload = {
                    RouteRuleCriteria: routeRuleEntity.Criteria,
                    isLinkedRouteRule: isLinkedRouteRule,
                    routingProductId: routingProductId,
                    sellingNumberPlanId: sellingNumberPlanId,
                    routeRuleCriteria: routeRuleEntity.Criteria,
                    linkedCode: customerRouteData != undefined ? customerRouteData.code : undefined,
                    routeRuleCriteriaContext: buildRouteRuleCriteriaContext(),
                    linkedCustomerId: customerRouteData != undefined ? customerRouteData.CustomerId : undefined
                };
            }

            var loadRouteRuleCriteriaTemplatesPromise = UtilsService.createPromiseDeferred();
            promises.push(loadRouteRuleCriteriaTemplatesPromise.promise);

            var routeRuleCriteriaTypePayload = {};
            if (routeRuleEntity != undefined && routeRuleEntity.Criteria != undefined) {
                routeRuleCriteriaTypePayload.selectedIds = routeRuleEntity.Criteria.ConfigId;
            }
            else if (defaultRouteRuleValues != undefined && defaultRouteRuleValues.selectedCriteria != undefined) {
                routeRuleCriteriaTypePayload.selectedIds = defaultRouteRuleValues.selectedCriteria;
            }
            routeRuleCriteriaSelectorReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(routeRuleCriteriaSelectorAPI, routeRuleCriteriaTypePayload, loadRouteRuleCriteriaTemplatesPromise);
            });

            if (routeRuleCriteriaPayload != undefined) {
                routeRuleCriteriaReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                var routeRuleCriteriaLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(routeRuleCriteriaLoadPromiseDeferred.promise);

                routeRuleCriteriaReadyPromiseDeferred.promise.then(function () {
                    routeRuleCriteriaReadyPromiseDeferred = undefined;
                    VRUIUtilsService.callDirectiveLoad(routeRuleCriteriaAPI, routeRuleCriteriaPayload, routeRuleCriteriaLoadPromiseDeferred);
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function editScopeTitle() {
            if (routingProductId == undefined)
                return;

            $scope.title += " of " + productRouteEntity.Name;
        }

        function loadStaticSection() {
            if (routeRuleEntity == undefined)
                return;

            $scope.scopeModel.beginEffectiveDate = routeRuleEntity.BeginEffectiveTime;
            $scope.scopeModel.endEffectiveDate = routeRuleEntity.EndEffectiveTime;
        }

        function insertRouteRule() {
            var routeRuleObject = buildRouteRuleObjFromScope();

            return WhS_Routing_RouteRuleAPIService.AddRule(routeRuleObject).then(function (response) {
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

            return WhS_Routing_RouteRuleAPIService.UpdateRule(routeRuleObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Route Rule", response, "Name")) {
                    if ($scope.onRouteRuleUpdated != undefined)
                        $scope.onRouteRuleUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        function buildLinkedRouteRule() {
            return WhS_Routing_RouteRuleAPIService.BuildLinkedRouteRule(linkedRouteRuleInput).then(function (routeRule) {
                routeRuleEntity = routeRule;
            });
        }

        function buildRouteRuleCriteriaContext() {
            var routeRuleCriteriaContext = {
                setTimeSettings: function (bed, eed) {
                    if (bed != undefined)
                        $scope.scopeModel.beginEffectiveDate = minBED = UtilsService.createDateFromString(bed);

                    if (eed != undefined)
                        $scope.scopeModel.endEffectiveDate = maxEED = UtilsService.createDateFromString(eed);
                }
            };

            return routeRuleCriteriaContext;
        }

        function buildRouteRuleObjFromScope() {
            var routeRule = {
                RuleId: (routeRuleId != null) ? routeRuleId : 0,
                Name: $scope.scopeModel.routeRuleName,
                Reason: $scope.scopeModel.routeRuleReason,
                Criteria: routeRuleCriteriaAPI.getData(),
                Settings: VRUIUtilsService.getSettingsFromDirective($scope.scopeModel, routeRuleSettingsAPI, 'selectedRouteRuleSettingsTemplate'),
                BeginEffectiveTime: $scope.scopeModel.beginEffectiveDate,
                EndEffectiveTime: $scope.scopeModel.endEffectiveDate
            };
            return routeRule;
        }
    }

    appControllers.controller('WhS_Routing_RouteRuleEditorController', routeRuleEditorController);
})(appControllers);
