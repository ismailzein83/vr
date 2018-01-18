(function (appControllers) {

    "use strict";

    BulkActionWizardController.$inject = ["$scope", 'WhS_Sales_RatePlanAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VRDateTimeService'];

    function BulkActionWizardController($scope, WhS_Sales_RatePlanAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VRDateTimeService) {

        var ownerType;
        var ownerId;
        var ownerSellingNumberPlanId;
        var routingDatabaseId;
        var policyConfigId;
        var numberOfOptions;
        var currencyId;
        var longPrecision;
        var tabsApi;
        var bulkActionContext;
        var pricingSettings;

        var dropdownSectionAPI;
        var dropdownSectionReadyDeferred = UtilsService.createPromiseDeferred();

        var bulkActionSelectiveAPI;
        var bulkActionSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var zoneFilterSelectiveAPI;
        var zoneFilterSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();
        var gridQuery;

        var validationDirectiveAPI;
        var validationDirectiveReadyDeferred;

        var excludedZoneIds;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                ownerType = parameters.ownerType;
                ownerId = parameters.ownerId;
                ownerSellingNumberPlanId = parameters.ownerSellingNumberPlanId;
                gridQuery = parameters.gridQuery;
                routingDatabaseId = parameters.routingDatabaseId;
                policyConfigId = parameters.policyConfigId;
                numberOfOptions = parameters.numberOfOptions;
                currencyId = parameters.currencyId;
                longPrecision = parameters.longPrecision;
                pricingSettings = parameters.pricingSettings;
            }
        }
        function defineScope() {

            $scope.title = "Bulk Action";

            $scope.scopeModel = {};

            $scope.scopeModel.applicableZonesTabObject = { showTab: false };
            $scope.scopeModel.showInvalidTab = false;

            $scope.scopeModel.onDropdownSectionReady = function (api) {
                dropdownSectionAPI = api;
                dropdownSectionReadyDeferred.resolve();
            };

            $scope.scopeModel.onBulkActionSelectiveReady = function (api) {
                bulkActionSelectiveAPI = api;
                bulkActionSelectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onTabsAPIReady = function (api) {
                tabsApi = api;
            };
            $scope.scopeModel.onDropdownSectionCollapsed = function () {
                var actionSummary = bulkActionSelectiveAPI.getSummary();
                if (actionSummary != undefined) {
                    $scope.scopeModel.actionSummaryTitle = actionSummary.title;
                    $scope.scopeModel.actionSummaryBody = actionSummary.body;
                }

                var filterSummary = zoneFilterSelectiveAPI.getSummary();
                if (filterSummary != undefined) {
                    $scope.scopeModel.filterSummaryTitle = filterSummary.title;
                    $scope.scopeModel.filterSummaryBody = filterSummary.body;
                }
            };

            $scope.scopeModel.onZoneFilterSelectiveReady = function (api) {
                zoneFilterSelectiveAPI = api;
                zoneFilterSelectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };

            $scope.scopeModel.onValidationDirectiveReady = function (api) {
                validationDirectiveAPI = api;
                validationDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.evaluate = function () {
                return evaluate();
            };

            $scope.scopeModel.apply = function () {
                return apply();
            };

            $scope.scopeModel.cancel = function () {
                $scope.modalContext.closeModal();
            };

            setContext();
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            UtilsService.waitMultipleAsyncOperations([expandDropdownSection, loadBulkActionSelective, loadZoneFilterSelective]).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function expandDropdownSection() {
            var expandDropdownSectionDeferred = UtilsService.createPromiseDeferred();

            dropdownSectionReadyDeferred.promise.then(function () {
                dropdownSectionAPI.expand().then(function () {
                    expandDropdownSectionDeferred.resolve();
                }).catch(function (error) {
                    expandDropdownSectionDeferred.reject(error);
                });
            });

            return expandDropdownSectionDeferred.promise;
        }
        function loadBulkActionSelective() {
            var bulkActionSelectiveLoadDeferred = UtilsService.createPromiseDeferred();
            bulkActionSelectiveReadyDeferred.promise.then(function () {
                var bulkActionSelectivePayload = {
                    bulkActionContext: bulkActionContext
                };
                VRUIUtilsService.callDirectiveLoad(bulkActionSelectiveAPI, bulkActionSelectivePayload, bulkActionSelectiveLoadDeferred);
            });
            return bulkActionSelectiveLoadDeferred.promise;
        }
        function loadZoneFilterSelective() {
            var zoneFilterSelectiveLoadDeferred = UtilsService.createPromiseDeferred();
            zoneFilterSelectiveReadyDeferred.promise.then(function () {
                var zoneFilterSelectivePayload = {
                    bulkActionContext: bulkActionContext
                };
                VRUIUtilsService.callDirectiveLoad(zoneFilterSelectiveAPI, zoneFilterSelectivePayload, zoneFilterSelectiveLoadDeferred);
            });
            return zoneFilterSelectiveLoadDeferred.promise;
        }

        function evaluate() {

            $scope.scopeModel.isLoading = true;

            var promises = [];
            var bulkActionValidationResult;
            var invalidDataExists;

            var bulkAction;
            var bulkActionZoneFilter;
            var validationDirectiveName;
            setBulkActionVariables();

            $scope.scopeModel.applicableZonesTabObject.showTab = false;
            $scope.scopeModel.showInvalidTab = false;

            var validateBulkActionZonesPromise = validateBulkActionZones();
            promises.push(validateBulkActionZonesPromise);

            var loadValidationDirectiveDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadValidationDirectiveDeferred.promise);

            var loadGridDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadGridDeferred.promise);

            validateBulkActionZonesPromise.then(function () {
                if (invalidDataExists === true) {
                    validationDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
                    $scope.scopeModel.showInvalidTab = true;
                    $scope.scopeModel.validationDirectiveName = validationDirectiveName;

                    loadValidationDirective().then(function () {
                        loadValidationDirectiveDeferred.resolve();
                    }).catch(function (error) {
                        loadValidationDirectiveDeferred.reject(error);
                    });
                }
                else {
                    loadValidationDirectiveDeferred.resolve();
                }

                loadGrid().then(function () {
                    loadGridDeferred.resolve();
                }).catch(function (error) {
                    loadGridDeferred.reject(error);
                });
            });

            function setBulkActionVariables() {
                bulkAction = bulkActionSelectiveAPI.getData();
                validationDirectiveName = bulkActionSelectiveAPI.getValidationDirectiveName();
                bulkActionZoneFilter = zoneFilterSelectiveAPI.getData();
            }
            function validateBulkActionZones() {
                var bulkActionZoneValidationInput =
                {
                    BulkAction: bulkAction,
                    BulkActionZoneFilter: bulkActionZoneFilter,
                    EffectiveOn: UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime()),
                    RoutingDatabaseId: routingDatabaseId,
                    PolicyConfigId: policyConfigId,
                    NumberOfOptions: numberOfOptions,
                    CurrencyId: currencyId
                };

                if (bulkActionContext != undefined) {
                    bulkActionZoneValidationInput.OwnerType = bulkActionContext.ownerType;
                    bulkActionZoneValidationInput.OwnerId = bulkActionContext.ownerId;
                    bulkActionZoneValidationInput.CostCalculationMethods = bulkActionContext.costCalculationMethods;
                }

                return WhS_Sales_RatePlanAPIService.ValidateBulkActionZones(bulkActionZoneValidationInput).then(function (response) {
                    bulkActionValidationResult = response;
                    if (response != undefined) {
                        invalidDataExists = response.InvalidDataExists;
                        excludedZoneIds = response.ExcludedZoneIds;
                    }
                });
            }
            function loadValidationDirective() {
                var validationDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                validationDirectiveReadyDeferred.promise.then(function () {
                    validationDirectiveReadyDeferred = undefined;
                    var validationDirectivePayload = {
                        bulkActionValidationResult: bulkActionValidationResult,
                        bulkAction: bulkActionContext.getSelectedBulkAction(),
                        pricingSettings: pricingSettings,
                        longPrecision: longPrecision
                    };
                    VRUIUtilsService.callDirectiveLoad(validationDirectiveAPI, validationDirectivePayload, validationDirectiveLoadDeferred);
                });

                return validationDirectiveLoadDeferred.promise;
            }
            function loadGrid() {
                var gridLoadDeferred = UtilsService.createPromiseDeferred();

                gridReadyDeferred.promise.then(function () {
                    extendGridQuery();
                    gridAPI.load(gridQuery).then(function () {
                        gridLoadDeferred.resolve();
                    }).catch(function (error) {
                        gridLoadDeferred.reject(error);
                    });
                });

                function extendGridQuery() {
                    gridQuery.BulkAction = bulkAction;

                    if (gridQuery.Filter == null)
                        gridQuery.Filter = {};

                    gridQuery.Filter.ExcludedZoneIds = excludedZoneIds;
                    gridQuery.Filter.BulkActionFilter = bulkActionZoneFilter;

                    if (gridQuery.context == undefined) {
                        gridQuery.context = {};
                    }
                    gridQuery.context.showApplicableZonesTab = function (value) {
                        $scope.scopeModel.applicableZonesTabObject.showTab = value;
                        $scope.scopeModel.isApplyButtonDisabled = !value;
                    };
                }

                return gridLoadDeferred.promise;
            }

            return UtilsService.waitMultiplePromises(promises).finally(function () {

                $scope.scopeModel.isLoading = false;
                dropdownSectionAPI.collapse();

                var selectedTabIndex;

                if ($scope.scopeModel.applicableZonesTabObject.showTab == true) {
                    selectedTabIndex = 0;
                }
                else if ($scope.scopeModel.showInvalidTab == true) {
                    selectedTabIndex = 1;
                }

                if (selectedTabIndex != undefined) {
                    tabsApi.setTabSelected(selectedTabIndex);

                    if (invalidDataExists && selectedTabIndex != 1)
                        VRNotificationService.showWarning("Invalid data exists. Please check the 'Invalid Zones' tab");
                }
            });
        }
        function apply() {
            $scope.scopeModel.isLoading = true;

            var bulkActionApplicationInput =
                {
                    OwnerType: gridQuery.OwnerType,
                    OwnerId: gridQuery.OwnerId,
                    CurrencyId: gridQuery.CurrencyId,
                    RoutingDatabaseId: gridQuery.RoutingDatabaseId,
                    PolicyConfigId: gridQuery.PolicyConfigId,
                    NumberOfOptions: gridQuery.NumberOfOptions,
                    CostCalculationMethods: gridQuery.CostCalculationMethods,
                    BulkAction: gridQuery.BulkAction,
                    EffectiveOn: gridQuery.EffectiveOn,
                    ExcludedZoneIds: excludedZoneIds,
                    BulkActionCorrectedData: (validationDirectiveAPI != undefined) ? validationDirectiveAPI.getData() : null
                };
            if (gridQuery.Filter != null) {
                bulkActionApplicationInput.BulkActionFilter = gridQuery.Filter.BulkActionFilter;
            }

            return WhS_Sales_RatePlanAPIService.ApplyBulkActionToDraft(bulkActionApplicationInput).then(function () {
                if ($scope.onBulkActionAppliedToDraft != undefined)
                    $scope.onBulkActionAppliedToDraft();
                $scope.modalContext.closeModal();
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setContext() {
            bulkActionContext = {};
            bulkActionContext.ownerType = ownerType;
            bulkActionContext.ownerId = ownerId;
            bulkActionContext.ownerSellingNumberPlanId = ownerSellingNumberPlanId;
            bulkActionContext.longPrecision = longPrecision;
            if (gridQuery != undefined)
                bulkActionContext.costCalculationMethods = gridQuery.CostCalculationMethods;
            bulkActionContext.getSelectedBulkAction = function () {
                return bulkActionSelectiveAPI.getData();
            };
            bulkActionContext.requireEvaluation = function () {
                $scope.scopeModel.isApplyButtonDisabled = true;
            };
        }
    }

    appControllers.controller("WhS_Sales_BulkActionWizardController", BulkActionWizardController);

})(appControllers);