(function (appControllers) {

    'use strict';

    StrategyEditorController.$inject = ['$scope', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'SuspicionLevelEnum', 'HourEnum', 'VRUIUtilsService', 'CDRAnalysis_FA_PeriodEnum', 'CDRAnalysis_FA_ParametersService'];

    function StrategyEditorController($scope, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, SuspicionLevelEnum, HourEnum, VRUIUtilsService, CDRAnalysis_FA_PeriodEnum, CDRAnalysis_FA_ParametersService) {
        var isEditMode;

        var periodSelectorAPI;
        var periodSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        
        var hourSelectorAPI;
        var hourSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var strategyId;
        var strategyEntity;

        var filterDefinitions;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                strategyId = parameters.strategyId;
            }

            isEditMode = strategyId != undefined;
        }

        function defineScope() {
            // Directives
            $scope.onPeriodSelectorReady = function (api) {
                periodSelectorAPI = api;
                periodSelectorReadyDeferred.resolve();
            };
            $scope.onHourSelectorReady = function (api) {
                hourSelectorAPI = api;
                hourSelectorReadyDeferred.resolve();
            };

            // Strategy parameters
            $scope.strategyFilters = [];
            $scope.selectedPeakHours = [];

            $scope.gapBetweenConsecutiveCalls = 10;
            $scope.gapBetweenFailedConsecutiveCalls = 10;
            $scope.maxLowDurationCall = 8;
            $scope.minCountofCallsinActiveHour = 5;

            $scope.getFilterHint = function (parameter) {
                if (parameter && $scope.strategyFilters) {
                    var filters = [];
                    for (var i = 0; i < $scope.strategyFilters.length; i++) {
                        var filter = $scope.strategyFilters[i];
                        if (filter.parameters && filter.parameters.indexOf(parameter) > -1) {
                            filters.push(filter.abbreviation);
                        }
                    }
                    return filters.join(',');
                }
                return null;
            };

            // Suspicion rules
            $scope.strategyLevels = [];
            $scope.suspicionLevels = UtilsService.getArrayEnum(SuspicionLevelEnum);

            $scope.addRule = function () {
                var strategyLevelItem = {
                    suspicionLevel: $scope.suspicionLevels[0],
                    StrategyLevelCriterias: []
                };

                angular.forEach($scope.strategyFilters, function (filter) {

                    var levelCriteriaItem = {
                        filterId: filter.filterId,
                        percentage: 0,
                        upSign: filter.upSign,
                        downSign: filter.downSign
                    };
                    strategyLevelItem.StrategyLevelCriterias.push(levelCriteriaItem);

                });

                $scope.strategyLevels.push(strategyLevelItem);
            };
            $scope.deleteRule = function (rule) {
                var index = $scope.strategyLevels.indexOf(rule);
                $scope.strategyLevels.splice(index, 1);
            };
            $scope.showThresholdHint = function (filter, strategyLevel) {
                if (filter && filter.threshold && strategyLevel && strategyLevel.percentage) {
                    var newThreshold = (parseInt(filter.threshold) + (parseInt(strategyLevel.percentage) * parseInt(filter.threshold) / 100));
                    return filter.label + ': ' + newThreshold;
                }
                return;
            };

            // User actions
            $scope.save = function () {
                if (isEditMode) {
                    return updateStrategy();
                }
                else {
                    return addStrategy();
                }
            };
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            // Validation functions
            $scope.validateStrategyFilters = function () {
                if ($scope.strategyFilters.length == 0)
                    return 'No filters found';

                for (var i = 0; i < $scope.strategyFilters.length; i++) {
                    if ($scope.strategyFilters[i].isSelected)
                        return null;
                }
                return 'No filter(s) selected';
            };
            $scope.validateStrategyLevels = function () {
                if ($scope.strategyFilters.length == 0) {
                    //VRNotificationService.showError('No filters found');
                    return 'No filters found';
                }

                if ($scope.strategyLevels.length == 0) {
                    //VRNotificationService.showError('No strategy levels added');
                    return 'No strategy levels added';
                }

                var filtersToUseCount = 0;
                var filterUsages = [];
                for (var i = 0; i < $scope.strategyFilters.length; i++) {
                    var filterUsage = {
                        mustBeUsed: $scope.strategyFilters[i].isSelected,
                        isUsed: false
                    };

                    if (filterUsage.mustBeUsed)
                        filtersToUseCount++;

                    filterUsages.push(filterUsage);
                }

                for (var i = 0; i < $scope.strategyLevels.length; i++) {
                    var strategyLevel = $scope.strategyLevels[i];

                    for (var j = 0; j < strategyLevel.StrategyLevelCriterias.length; j++) {
                        if ($scope.strategyFilters[j].isSelected) {
                            if (!filterUsages[j].isUsed) {
                                filterUsages[j].isUsed = strategyLevel.StrategyLevelCriterias[j].isSelected;
                            }
                        }
                    }
                }

                var usedFiltersCount = 0;
                for (var i = 0; i < filterUsages.length; i++) {
                    if (filterUsages[i].mustBeUsed && filterUsages[i].isUsed) {
                        usedFiltersCount++;
                    }
                }

                if (usedFiltersCount < filtersToUseCount) {
                    //VRNotificationService.showError('Not all selected filters are used');
                    return 'Not all selected filters are used';
                }

                return null;
            };
        }

        function load() {
            $scope.isLoading = true;
            
            if (isEditMode) {
                getStrategy().then(function () {
                    loadAllControls().finally(function () {
                        strategyEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
                loadAllControls();
            }
        }

        function getStrategy() {
            return StrategyAPIService.GetStrategy(strategyId).then(function (response) {
                strategyEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticControls, loadPeriodSelector, loadHourSelector, loadFilters]).then(function () {
                if (isEditMode) {
                    loadStrategyFiltersForEditMode();
                    loadStrategyLevelsForEditMode();
                }
                else {
                    loadStrategyFiltersForAddMode();
                }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function setTitle() {
                $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(strategyEntity ? strategyEntity.Name : null, 'Strategy') : UtilsService.buildTitleForAddEditor('Strategy');
            }
            function loadStaticControls() {
                if (strategyEntity) {
                    $scope.name = strategyEntity.Name;
                    $scope.description = strategyEntity.Description;
                    $scope.isDefault = strategyEntity.IsDefault;
                    $scope.isEnabled = strategyEntity.IsEnabled;
                    $scope.gapBetweenConsecutiveCalls = strategyEntity.GapBetweenConsecutiveCalls;
                    $scope.gapBetweenFailedConsecutiveCalls = strategyEntity.GapBetweenFailedConsecutiveCalls;
                    $scope.maxLowDurationCall = strategyEntity.MaxLowDurationCall;
                    $scope.minCountofCallsinActiveHour = strategyEntity.MinimumCountofCallsinActiveHour;
                }
            }
            function loadPeriodSelector() {
                var periodSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                periodSelectorReadyDeferred.promise.then(function () {
                    var periodSelectorPayload = strategyEntity ? {
                        selectedIds: strategyEntity.PeriodId
                    } : null;
                    VRUIUtilsService.callDirectiveLoad(periodSelectorAPI, periodSelectorPayload, periodSelectorLoadDeferred);
                });

                return periodSelectorLoadDeferred.promise;
            }
            function loadHourSelector() {
                var hourSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                hourSelectorReadyDeferred.promise.then(function () {
                    var selectedIds;
                    if (strategyEntity != undefined) {
                        if (strategyEntity.PeakHours != null) {
                            selectedIds = [];
                            for (var i = 0; i < strategyEntity.PeakHours.length; i++) {
                                selectedIds.push(strategyEntity.PeakHours[i].Id);
                            }
                        }
                        callDirectiveLoad(selectedIds);
                    }
                    else {
                        CDRAnalysis_FA_ParametersService.getDefaultPeakHourIds().then(function (response) {
                            callDirectiveLoad(response);
                        });
                    }
                });

                return hourSelectorLoadDeferred.promise;

                function callDirectiveLoad(selectedIds) {
                    var hourSelectorPayload = {
                        selectedIds: selectedIds
                    };
                    VRUIUtilsService.callDirectiveLoad(hourSelectorAPI, hourSelectorPayload, hourSelectorLoadDeferred);
                }
            }
            function loadFilters() {
                return StrategyAPIService.GetFilters().then(function (response) {
                    if (response) {
                        filterDefinitions = [];

                        for (var i = 0; i < response.length; i++) {
                            var filterDef = {};

                            filterDef.filterId =  response[i].FilterId,
                            filterDef.description = response[i].Description,
                            filterDef.abbreviation = response[i].Abbreviation,
                            filterDef.label = response[i].Label,
                            filterDef.minValue = response[i].MinValue,
                            filterDef.maxValue = response[i].MaxValue,
                            filterDef.decimalPrecision = response[i].DecimalPrecision,
                            filterDef.excludeHourly = response[i].ExcludeHourly,
                            filterDef.toolTip = response[i].ToolTip,
                            filterDef.upSign = response[i].UpSign,
                            filterDef.downSign = response[i].DownSign,
                            filterDef.parameters = response[i].Parameters

                            filterDefinitions.push(filterDef);
                        }
                    }
                });
            }
            function loadStrategyLevelsForEditMode() {
                angular.forEach(strategyEntity.StrategyLevels, function (level) {
                    var strategyLevelItem = {
                        suspicionLevel: UtilsService.getItemByVal($scope.suspicionLevels, level.SuspicionLevelId, 'value'),
                        StrategyLevelCriterias: []
                    };

                    angular.forEach(filterDefinitions, function (filterDef) {

                        var levelCriteriaItem = {
                            filterId: filterDef.FilterId,
                            upSign: filterDef.upSign,
                            downSign: filterDef.downSign,
                            percentage: 0
                        };

                        var existingItem = UtilsService.getItemByVal(level.StrategyLevelCriterias, filterDef.filterId, 'FilterId');
                        if (existingItem != undefined && existingItem != null) {
                            levelCriteriaItem.isSelected = true;
                            levelCriteriaItem.percentage = ((parseFloat(existingItem.Percentage) * 100) - 100);
                        }
                        strategyLevelItem.StrategyLevelCriterias.push(levelCriteriaItem);
                    });

                    $scope.strategyLevels.push(strategyLevelItem);
                });
            }
            function loadStrategyFiltersForAddMode() {
                if (filterDefinitions) {
                    for (var i = 0; i < filterDefinitions.length; i++) {
                        var strategyFilter = getStrategyFilterDataItemWithCommonProperties(filterDefinitions[i]);
                        $scope.strategyFilters.push(strategyFilter);
                    }
                }
            }
            function loadStrategyFiltersForEditMode() {
                if (filterDefinitions) {
                    for (var i = 0; i < filterDefinitions.length; i++) {
                        var strategyFilter = getStrategyFilterDataItemWithCommonProperties(filterDefinitions[i]);

                        if (strategyEntity != null) {
                            var entityStrategyFilter = UtilsService.getItemByVal(strategyEntity.StrategyFilters, strategyFilter.filterId, 'FilterId');
                            if (entityStrategyFilter != null) {
                                strategyFilter.isSelected = true;
                                strategyFilter.threshold = entityStrategyFilter.Threshold;
                            }
                        }

                        $scope.strategyFilters.push(strategyFilter);
                    }
                }
            }
            function getStrategyFilterDataItemWithCommonProperties(filterDef) {
                var item = {};

                item.filterId = filterDef.filterId;
                item.description = filterDef.description;
                item.abbreviation = filterDef.abbreviation;
                item.label = filterDef.label;
                item.minValue = filterDef.minValue;
                item.maxValue = filterDef.maxValue;
                item.decimalPrecision = filterDef.decimalPrecision;
                item.excludeHourly = filterDef.excludeHourly;
                item.toolTip = filterDef.toolTip;
                item.upSign = filterDef.upSign;
                item.downSign = filterDef.downSign;
                item.parameters = filterDef.parameters;

                if (item.parameters != null && item.parameters.length > 0) {
                    item.hint = 'This filter requires the following parameter(s): ' + item.parameters.join(',');
                    item.hasParameters = true;
                }
                else {
                    item.hint = null;
                    item.hasParameters = false;
                }

                if (item.excludeHourly && periodSelectorAPI.getSelectedIds() == CDRAnalysis_FA_PeriodEnum.Hourly.value) {
                    item.isSelected = false;
                    item.isShown = false;
                }
                else {
                    item.isShown = true;
                }

                return item;
            }
        }

        function addStrategy() {
            var strategyObject = buildStrategyObjFromScope();
            return StrategyAPIService.AddStrategy(strategyObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Strategy', response, 'Name')) {
                    if ($scope.onStrategyAdded != undefined)
                        $scope.onStrategyAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        function updateStrategy() {
            var strategyObject = buildStrategyObjFromScope();
            return StrategyAPIService.UpdateStrategy(strategyObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Strategy', response, 'Name')) {
                    if ($scope.onStrategyUpdated != undefined)
                        $scope.onStrategyUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        function buildStrategyObjFromScope() {
            var strategyObject = {
                Id: (strategyId != null) ? strategyId : 0,
                PeriodId: periodSelectorAPI.getSelectedIds(),
                Name: $scope.name,
                Description: $scope.description,
                IsDefault: $scope.isDefault,
                IsEnabled: $scope.isEnabled,
                GapBetweenConsecutiveCalls: $scope.gapBetweenConsecutiveCalls,
                GapBetweenFailedConsecutiveCalls: $scope.gapBetweenFailedConsecutiveCalls,
                MaxLowDurationCall: $scope.maxLowDurationCall,
                MinimumCountofCallsinActiveHour: $scope.minCountofCallsinActiveHour,
                PeakHours: $scope.selectedPeakHours,
                StrategyFilters: [],
                StrategyLevels: [],
                LastUpdatedOn: new Date()
            };

            angular.forEach($scope.strategyFilters, function (filter) {
                if (filter.isSelected) {
                    var filterItem = {
                        FilterId: filter.filterId,
                        Description: filter.description,
                        Abbreviation: filter.abbreviation,
                        Threshold: filter.threshold
                    };
                    strategyObject.StrategyFilters.push(filterItem);
                }
            });

            angular.forEach($scope.strategyLevels, function (level) {
                var strategyLevelItem = {
                    SuspicionLevelId: level.suspicionLevel.id,
                    StrategyLevelCriterias: []
                };

                var index = 0;
                angular.forEach(level.StrategyLevelCriterias, function (levelCriteria) {
                    if ($scope.strategyFilters[index].isSelected && levelCriteria.isSelected) {
                        var levelCriteriaItem = {
                            FilterId: $scope.strategyFilters[index].filterId
                        };

                        levelCriteriaItem.Percentage = ((parseFloat(levelCriteria.percentage) + 100) / 100);
                        strategyLevelItem.StrategyLevelCriterias.push(levelCriteriaItem);
                    }
                    index++;
                });

                strategyObject.StrategyLevels.push(strategyLevelItem);
            });

            return strategyObject;
        }
    }

    appControllers.controller('CDRAnalysis_FA_StrategyEditorController', StrategyEditorController);

})(appControllers);
