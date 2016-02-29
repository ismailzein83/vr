(function (appControllers) {

    'use strict';

    StrategyEditorController.$inject = ['$scope', 'StrategyAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'CDRAnalysis_FA_SuspicionLevelEnum', 'VRCommon_HourEnum', 'VRUIUtilsService', 'CDRAnalysis_FA_PeriodEnum', 'CDRAnalysis_FA_ParametersService'];

    function StrategyEditorController($scope, StrategyAPIService, VRModalService, VRNotificationService, VRNavigationService, UtilsService, CDRAnalysis_FA_SuspicionLevelEnum, VRCommon_HourEnum, VRUIUtilsService, CDRAnalysis_FA_PeriodEnum, CDRAnalysis_FA_ParametersService) {
        var isEditMode;

        var periodSelectorAPI;
        var periodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var hourSelectorAPI;
        var hourSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var strategyId;
        var strategyEntity;

        var filters;

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


        function countDecimal(num) {
            var match = ('' + num).match(/(?:\.(\d+))?(?:[eE]([+-]?\d+))?$/);
            if (!match) { return 0; }
            return Math.max(
                 0,
                 // Number of digits right of decimal point.
                 (match[1] ? match[1].length : 0)
                 // Adjust for scientific notation.
                 - (match[2] ? +match[2] : 0));
        }

        function defineScope() {
            $scope.modalScope = {};
            $scope.modalScope.strategyFilters = [];

            // Directives
            $scope.modalScope.onPeriodSelectorReady = function (api) {
                periodSelectorAPI = api;
                periodSelectorReadyDeferred.resolve();
            };
            $scope.modalScope.onPeriodSelectionChanged = function (selectedPeriod) {
                if (selectedPeriod != undefined) {
                    for (var i = 0; i < $scope.modalScope.strategyFilters.length; i++) {
                        setIsSelectedAndIsShownForFilterDataItem($scope.modalScope.strategyFilters[i]);
                    }
                }
            };
            $scope.modalScope.onHourSelectorReady = function (api) {
                hourSelectorAPI = api;
                hourSelectorReadyDeferred.resolve();
            };

            // Strategy parameters
            $scope.modalScope.selectedPeakHours = [];
            $scope.modalScope.gapBetweenConsecutiveCalls = 10;
            $scope.modalScope.gapBetweenFailedConsecutiveCalls = 10;
            $scope.modalScope.maxLowDurationCall = 8;
            $scope.modalScope.minCountofCallsinActiveHour = 5;

            $scope.modalScope.getFilterHint = function (parameter) {
                if (parameter != undefined && parameter != null) {
                    var filters = [];
                    for (var i = 0; i < $scope.modalScope.strategyFilters.length; i++) {
                        var filter = $scope.modalScope.strategyFilters[i];
                        if (filter.parameters != null && filter.parameters.indexOf(parameter) > -1) {
                            filters.push(filter.abbreviation);
                        }
                    }
                    return filters.join(',');
                }
                return null;
            };

            // Suspicion rules
            $scope.modalScope.suspicionRules = [];
            $scope.modalScope.suspicionLevels = UtilsService.getArrayEnum(CDRAnalysis_FA_SuspicionLevelEnum);

            $scope.modalScope.addSuspicionRule = function () {
                var strategyLevelItem = {
                    suspicionLevel: $scope.modalScope.suspicionLevels[0],
                    StrategyLevelCriterias: []
                };

                angular.forEach($scope.modalScope.strategyFilters, function (filter) {
                    var levelCriteriaItem = {
                        filterId: filter.filterId,
                        percentage: 0,
                        upSign: filter.upSign,
                        downSign: filter.downSign
                    };
                    strategyLevelItem.StrategyLevelCriterias.push(levelCriteriaItem);
                });

                $scope.modalScope.suspicionRules.push(strategyLevelItem);
            };
            $scope.modalScope.deleteSuspicionRule = function (rule) {
                var index = $scope.modalScope.suspicionRules.indexOf(rule);
                $scope.modalScope.suspicionRules.splice(index, 1);
            };
            $scope.modalScope.showThresholdHint = function (filter, strategyLevel) {
                if (filter != undefined && strategyLevel != undefined) {
                    var newThreshold;

                    if (countDecimal(filter.threshold) > 0) {

                        newThreshold = (filter.threshold != undefined && strategyLevel.percentage != undefined) ?
                       (parseFloat(filter.threshold) + (parseFloat(strategyLevel.percentage) * parseFloat(filter.threshold) / 100)).toFixed(2) : 'None';

                    }
                    else {

                        newThreshold = (filter.threshold != undefined && strategyLevel.percentage != undefined) ?
                       (parseInt(filter.threshold) + (parseInt(strategyLevel.percentage) * parseInt(filter.threshold) / 100)) : 'None';

                    }
                   
                    return filter.label + ': ' + newThreshold;
                }
                return null;
            };

            // User actions
            $scope.modalScope.saveStrategy = function () {
                if (isEditMode) {
                    return updateStrategy();
                }
                else {
                    return addStrategy();
                }
            };
            $scope.modalScope.close = function () {
                $scope.modalContext.closeModal();
            };

            // Validation functions
            $scope.modalScope.validateStrategyFilters = function () {
                if ($scope.modalScope.strategyFilters.length == 0)
                    return 'No filters found';

                for (var i = 0; i < $scope.modalScope.strategyFilters.length; i++) {
                    if ($scope.modalScope.strategyFilters[i].isSelected)
                        return null;
                }

                return 'No filter(s) selected';
            };
            $scope.modalScope.validateStrategyLevels = function () {
                if ($scope.modalScope.strategyFilters.length == 0) {
                    return 'No filters found';
                }

                if ($scope.modalScope.suspicionRules.length == 0) {
                    return 'No suspicion rules added';
                }

                var filtersToUseCount = 0;
                var filterUsages = [];
                for (var i = 0; i < $scope.modalScope.strategyFilters.length; i++) {
                    var filterUsage = {
                        mustBeUsed: $scope.modalScope.strategyFilters[i].isSelected,
                        isUsed: false
                    };

                    if (filterUsage.mustBeUsed)
                        filtersToUseCount++;

                    filterUsages.push(filterUsage);
                }

                for (var i = 0; i < $scope.modalScope.suspicionRules.length; i++) {
                    var strategyLevel = $scope.modalScope.suspicionRules[i];
                    var aFilterIsUsed = false;

                    for (var j = 0; j < strategyLevel.StrategyLevelCriterias.length; j++) {
                        if ($scope.modalScope.strategyFilters[j].isSelected) {
                            if (!filterUsages[j].isUsed) {
                                filterUsages[j].isUsed = strategyLevel.StrategyLevelCriterias[j].isSelected;
                            }
                            if (!aFilterIsUsed) {
                                aFilterIsUsed = strategyLevel.StrategyLevelCriterias[j].isSelected;
                            }
                        }
                    }

                    if (!aFilterIsUsed) {
                        return 'Some suspicion rules are not using any filters';
                    }
                }

                var usedFiltersCount = 0;
                for (var i = 0; i < filterUsages.length; i++) {
                    if (filterUsages[i].mustBeUsed && filterUsages[i].isUsed) {
                        usedFiltersCount++;
                    }
                }

                if (usedFiltersCount < filtersToUseCount) {
                    return 'Not all selected filters are used';
                }

                return null;
            };
        }

        function load() {
            $scope.modalScope.isLoading = true;

            if (isEditMode) {
                getStrategy().then(function () {
                    loadAllControls().finally(function () {
                        strategyEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.modalScope.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope.modalScope);
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
                    loadSuspicionRulesForEditMode();
                }
                else {
                    loadStrategyFiltersForAddMode();
                }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope.modalScope);
            }).finally(function () {
                $scope.modalScope.isLoading = false;
            });

            function setTitle() {
                $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(strategyEntity ? strategyEntity.Name : null, 'Strategy') : UtilsService.buildTitleForAddEditor('Strategy');
            }
            function loadStaticControls() {
                if (strategyEntity) {
                    $scope.modalScope.name = strategyEntity.Name;
                    $scope.modalScope.description = strategyEntity.Description;
                    $scope.modalScope.isDefault = strategyEntity.IsDefault;
                    $scope.modalScope.isEnabled = strategyEntity.IsEnabled;
                    $scope.modalScope.gapBetweenConsecutiveCalls = strategyEntity.GapBetweenConsecutiveCalls;
                    $scope.modalScope.gapBetweenFailedConsecutiveCalls = strategyEntity.GapBetweenFailedConsecutiveCalls;
                    $scope.modalScope.maxLowDurationCall = strategyEntity.MaxLowDurationCall;
                    $scope.modalScope.minCountofCallsinActiveHour = strategyEntity.MinimumCountofCallsinActiveHour;
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
                        filters = [];

                        for (var i = 0; i < response.length; i++) {
                            var filter = {};

                            filter.filterId = response[i].FilterId,
                            filter.description = response[i].Description,
                            filter.abbreviation = response[i].Abbreviation,
                            filter.label = response[i].Label,
                            filter.minValue = response[i].MinValue,
                            filter.maxValue = response[i].MaxValue,
                            filter.decimalPrecision = response[i].DecimalPrecision,
                            filter.excludeHourly = response[i].ExcludeHourly,
                            filter.toolTip = response[i].ToolTip,
                            filter.upSign = response[i].UpSign,
                            filter.downSign = response[i].DownSign,
                            filter.parameters = response[i].Parameters

                            filters.push(filter);
                        }
                    }
                });
            }
            function loadStrategyFiltersForAddMode() {
                if (filters) {
                    for (var i = 0; i < filters.length; i++) {
                        var strategyFilter = getStrategyFilterDataItemWithCommonProperties(filters[i]);
                        $scope.modalScope.strategyFilters.push(strategyFilter);
                    }
                }
            }
            function loadStrategyFiltersForEditMode() {
                if (filters) {
                    for (var i = 0; i < filters.length; i++) {
                        var strategyFilter = getStrategyFilterDataItemWithCommonProperties(filters[i]);

                        if (strategyEntity != null) {
                            var entityStrategyFilter = UtilsService.getItemByVal(strategyEntity.StrategyFilters, strategyFilter.filterId, 'FilterId');
                            if (entityStrategyFilter != null) {
                                strategyFilter.isSelected = true;
                                strategyFilter.threshold = entityStrategyFilter.Threshold;
                            }
                        }

                        $scope.modalScope.strategyFilters.push(strategyFilter);
                    }
                }
            }
            function getStrategyFilterDataItemWithCommonProperties(filter) {
                var item = {};

                item.filterId = filter.filterId;
                item.description = filter.description;
                item.abbreviation = filter.abbreviation;
                item.label = filter.label;
                item.minValue = filter.minValue;
                item.maxValue = filter.maxValue;
                item.decimalPrecision = filter.decimalPrecision;
                item.excludeHourly = filter.excludeHourly;
                item.toolTip = filter.toolTip;
                item.upSign = filter.upSign;
                item.downSign = filter.downSign;
                item.parameters = filter.parameters;

                if (item.parameters != null && item.parameters.length > 0) {
                    item.hint = 'This filter requires the following parameter(s): ' + item.parameters.join(',');
                    item.hasParameters = true;
                }
                else {
                    item.hint = null;
                    item.hasParameters = false;
                }

                setIsSelectedAndIsShownForFilterDataItem(item);
                return item;
            }
            function loadSuspicionRulesForEditMode() {
                angular.forEach(strategyEntity.StrategyLevels, function (level) {
                    var strategyLevelItem = {
                        suspicionLevel: UtilsService.getItemByVal($scope.modalScope.suspicionLevels, level.SuspicionLevelId, 'value'),
                        StrategyLevelCriterias: []
                    };

                    angular.forEach(filters, function (filter) {
                        var levelCriteriaItem = {
                            filterId: filter.FilterId,
                            upSign: filter.upSign,
                            downSign: filter.downSign,
                            percentage: 0
                        };

                        var existingItem = UtilsService.getItemByVal(level.StrategyLevelCriterias, filter.filterId, 'FilterId');
                        if (existingItem != undefined && existingItem != null) {
                            levelCriteriaItem.isSelected = true;
                            levelCriteriaItem.percentage = parseInt((parseFloat(existingItem.Percentage) * 100) - 100); // The outer parseInt call is used for formatting purposes
                        }
                        strategyLevelItem.StrategyLevelCriterias.push(levelCriteriaItem);
                    });

                    $scope.modalScope.suspicionRules.push(strategyLevelItem);
                });
            }
        }

        function setIsSelectedAndIsShownForFilterDataItem(filter) {
            if (filter.excludeHourly && periodSelectorAPI.getSelectedIds() == CDRAnalysis_FA_PeriodEnum.Hourly.value) {
                filter.isSelected = false;
                filter.isShown = false;
            }
            else {
                filter.isShown = true;
            }
        }

        function addStrategy() {
            var strategyObject = buildStrategyObjFromScope();
            $scope.modalScope.isLoading = true;

            return StrategyAPIService.AddStrategy(strategyObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Strategy', response, 'Name')) {
                    if ($scope.onStrategyAdded != undefined && typeof $scope.onStrategyAdded == 'function')
                        $scope.onStrategyAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope.modalScope);
            }).finally(function () {
                $scope.modalScope.isLoading = false;
            });
        }

        function updateStrategy() {
            var strategyObject = buildStrategyObjFromScope();
            $scope.modalScope.isLoading = true;

            return StrategyAPIService.UpdateStrategy(strategyObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Strategy', response, 'Name')) {
                    if ($scope.onStrategyUpdated != undefined && typeof $scope.onStrategyUpdated == 'function')
                        $scope.onStrategyUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope.modalScope);
            }).finally(function () {
                $scope.modalScope.isLoading = false;
            });
        }

        function buildStrategyObjFromScope() {
            var strategyObject = {
                Id: (strategyId != null) ? strategyId : 0,
                PeriodId: periodSelectorAPI.getSelectedIds(),
                Name: $scope.modalScope.name,
                Description: $scope.modalScope.description,
                IsDefault: $scope.modalScope.isDefault,
                IsEnabled: $scope.modalScope.isEnabled,
                GapBetweenConsecutiveCalls: $scope.modalScope.gapBetweenConsecutiveCalls,
                GapBetweenFailedConsecutiveCalls: $scope.modalScope.gapBetweenFailedConsecutiveCalls,
                MaxLowDurationCall: $scope.modalScope.maxLowDurationCall,
                MinimumCountofCallsinActiveHour: $scope.modalScope.minCountofCallsinActiveHour,
                PeakHours: (periodSelectorAPI.getSelectedIds() == CDRAnalysis_FA_PeriodEnum.Hourly.value) ? [] : $scope.modalScope.selectedPeakHours,
                StrategyFilters: [],
                StrategyLevels: [],
                LastUpdatedOn: new Date()
            };

            angular.forEach($scope.modalScope.strategyFilters, function (filter) {
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

            angular.forEach($scope.modalScope.suspicionRules, function (level) {
                var strategyLevelItem = {
                    SuspicionLevelId: level.suspicionLevel.value,
                    StrategyLevelCriterias: []
                };

                var index = 0;
                angular.forEach(level.StrategyLevelCriterias, function (levelCriteria) {
                    if ($scope.modalScope.strategyFilters[index].isSelected && levelCriteria.isSelected) {
                        var levelCriteriaItem = {
                            FilterId: $scope.modalScope.strategyFilters[index].filterId
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
