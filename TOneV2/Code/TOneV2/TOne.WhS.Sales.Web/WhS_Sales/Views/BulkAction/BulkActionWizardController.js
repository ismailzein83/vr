﻿(function (appControllers) {

    "use strict";

    BulkActionWizardController.$inject = ["$scope", 'WhS_Sales_RatePlanAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function BulkActionWizardController($scope, WhS_Sales_RatePlanAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var ownerType;
        var ownerId;
        var ownerSellingNumberPlanId;
        var routingDatabaseId;
        var policyConfigId;
        var numberOfOptions;
        var currencyId;

        var bulkActionContext;

        var actionStepAPI;
        var actionStepReadyDeferred = UtilsService.createPromiseDeferred();

        var filterStepAPI;
        var filterStepReadyDeferred = UtilsService.createPromiseDeferred();

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
            }
        }
        function defineScope() {

            $scope.title = "Bulk Action";

            $scope.scopeModel = {};
            $scope.scopeModel.showInvalidTab = false;

            $scope.scopeModel.onActionStepReady = function (api) {
                actionStepAPI = api;
                actionStepReadyDeferred.resolve();
            };

            $scope.scopeModel.onFilterStepReady = function (api) {
                filterStepAPI = api;
                filterStepReadyDeferred.resolve();
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
            UtilsService.waitMultipleAsyncOperations([loadActionStep, loadFilterStep]).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadActionStep() {
            var actionStepLoadDeferred = UtilsService.createPromiseDeferred();
            actionStepReadyDeferred.promise.then(function () {
                var actionStepPayload = {
                    bulkActionContext: bulkActionContext
                };
                VRUIUtilsService.callDirectiveLoad(actionStepAPI, actionStepPayload, actionStepLoadDeferred);
            });
            return actionStepLoadDeferred.promise;
        }
        function loadFilterStep() {
            var filterStepLoadDeferred = UtilsService.createPromiseDeferred();
            filterStepReadyDeferred.promise.then(function () {
                var filterStepPayload = {
                    bulkActionContext: bulkActionContext
                };
                VRUIUtilsService.callDirectiveLoad(filterStepAPI, filterStepPayload, filterStepLoadDeferred);
            });
            return filterStepLoadDeferred.promise;
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

            $scope.scopeModel.showInvalidTab = false;

            var validateBulkActionZonesPromise = validateBulkActionZones();
            promises.push(validateBulkActionZonesPromise);

            var loadValidationDirectiveDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadValidationDirectiveDeferred.promise);

            var loadGridDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadGridDeferred.promise);

            validateBulkActionZonesPromise.then(function ()
            {
                if (invalidDataExists === true)
                {
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
                var actionStepData = actionStepAPI.getData();
                if (actionStepData != undefined)
                    bulkAction = actionStepData.bulkAction;

                validationDirectiveName = actionStepAPI.getValidationDirectiveName();

                var filterStepData = filterStepAPI.getData();
                if (filterStepData != undefined)
                    bulkActionZoneFilter = filterStepData.zoneFilter;
            }
            function validateBulkActionZones() {
                var bulkActionZoneValidationInput =
                {
                    BulkAction: bulkAction,
                    BulkActionZoneFilter: bulkActionZoneFilter,
                    EffectiveOn: UtilsService.getDateFromDateTime(new Date()),
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
                        bulkActionValidationResult: bulkActionValidationResult
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
                    gridQuery.context.showBulkActionTabs = function (value) {
                        $scope.scopeModel.showTabs = value;
                        $scope.scopeModel.isApplyButtonDisabled = !value;
                    };
                }

                return gridLoadDeferred.promise;
            }

            return UtilsService.waitMultiplePromises(promises).finally(function () {
                $scope.scopeModel.isLoading = false;
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
                    ExcludedZoneIds: excludedZoneIds
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
            bulkActionContext.getSelectedBulkAction = function () {
                var actionStepData = actionStepAPI.getData();
                return (actionStepData != undefined) ? actionStepData.bulkAction : undefined;
            };
            bulkActionContext.requireEvaluation = function () {
                $scope.scopeModel.isApplyButtonDisabled = true;
            };
            if (gridQuery != undefined)
                bulkActionContext.costCalculationMethods = gridQuery.CostCalculationMethods;
        }
    }

    appControllers.controller("WhS_Sales_BulkActionWizardController", BulkActionWizardController);

})(appControllers);