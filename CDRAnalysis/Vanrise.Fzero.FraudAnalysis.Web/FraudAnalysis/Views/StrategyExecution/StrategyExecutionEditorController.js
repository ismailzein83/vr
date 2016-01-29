"use strict";

StrategyExecutionEditorController.$inject = ['$scope', 'StrategyExecutionAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'CDRAnalysis_FA_SuspicionLevelEnum', 'VRCommon_HourEnum'];

function StrategyExecutionEditorController($scope, StrategyExecutionAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, CDRAnalysis_FA_SuspicionLevelEnum, VRCommon_HourEnum) {

    var editMode;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.strategyId = undefined;

        if (parameters != undefined && parameters != null)
            $scope.strategyId = parameters.strategyId;

        if ($scope.strategyId != undefined)
            editMode = true;
        else
            editMode = false;
    }

    function defineScope() {

        $scope.gapBetweenConsecutiveCalls = 10;
        $scope.gapBetweenFailedConsecutiveCalls = 10;
        $scope.maxLowDurationCall = 8;
        $scope.minCountofCallsinActiveHour = 5;

        $scope.selectedPeriod;
        $scope.periods = [];
        $scope.strategyFilters = [];
        $scope.selectedPeakHours = [];
        $scope.filterDefinitions = [];

        StrategyExecutionEditorController.isFilterTabShown = true;
        StrategyExecutionEditorController.isLevelsTabShow = false;

        $scope.suspicionLevels = [];
        angular.forEach(CDRAnalysis_FA_SuspicionLevelEnum, function (itm) {
            $scope.suspicionLevels.push({ id: itm.value, name: itm.description })
        });

        $scope.hours = [];
        angular.forEach(VRCommon_HourEnum, function (itm) {
            $scope.hours.push({ id: itm.id, name: itm.name })
        });

        angular.forEach($scope.hours, function (itm) {
            if (itm.id >= 12 && itm.id <= 17)
                $scope.selectedPeakHours.push(itm);
        });

        $scope.strategyLevels = [];

        $scope.AddSuspicionLevel = function () {
            var strategyLevelItem = {
                suspicionLevel: $scope.suspicionLevels[0],
                StrategyExecutionLevelCriterias: []
            };

            angular.forEach($scope.strategyFilters, function (filter) {

                var levelCriteriaItem = {
                    filterId: filter.filterId,
                    percentage: 0,
                    upSign: filter.upSign,
                    downSign: filter.downSign
                };
                strategyLevelItem.StrategyExecutionLevelCriterias.push(levelCriteriaItem);

            });

            $scope.strategyLevels.push(strategyLevelItem);
        };

        $scope.SaveStrategyExecution = function () {
            if (editMode) {

                return updateStrategyExecution();
            }
            else {
                return addStrategyExecution();
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.deleteRule = function (rule) {
            var index = $scope.strategyLevels.indexOf(rule);
            $scope.strategyLevels.splice(index, 1);
        }


        $scope.hasParameters = function (item) {
            if (item.parameters != undefined) {
                return (item.parameters.length > 0);
            }
            else
                return;
        }


        $scope.showParametersHint = function (item) {
            if (item.parameters != undefined) {
                if (item.parameters.length > 0)
                    return "This filter requires the following parameter(s): " + item.parameters.join(',');
            }
            else
                return;
        }


        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return $scope.strategyFilters;
        }


        $scope.hasFilters = function (parameter) {
            var found = false;
            if ($scope.strategyFilters.length > 0) {
                angular.forEach($scope.strategyFilters, function (filter) {
                    if (filter.parameters.indexOf(parameter) > -1 && filter.isSelected) {
                        found = true;
                    }
                });
            }
            return found;
        }


        $scope.showFiltersHint = function (parameter) {
            var filters = [];

            if (parameter != undefined) {
                if ($scope.strategyFilters.length > 0) {
                    angular.forEach($scope.strategyFilters, function (filter) {
                        if (filter.parameters != undefined)
                            if (filter.parameters.indexOf(parameter) > -1) {
                                filters.push(filter.abbreviation);
                            }
                    });
                    return filters.join(',');
                }
            }
            else
                return;
        }

        $scope.showThresholdHint = function (filter, strategyLevel) {
            if (filter != undefined) {
                var newThreshold = (parseInt(filter.threshold) + (parseInt(strategyLevel.percentage) * parseInt(filter.threshold) / 100));
                return filter.label + ': ' + newThreshold;
            }
            else
                return;
        }
    }

    function load() {
        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadFilters, loadPeriods])
        .then(function () {
            if (editMode) {
                getStrategyExecution().finally(function () {
                    $scope.isGettingData = false;
                });
            }
            else {
                loadFiltersForAddMode();
                $scope.isGettingData = false;
            }

        })
        .catch(function (error) {
            $scope.isGettingData = false;
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
    }

    function getStrategyExecution() {

        return StrategyExecutionAPIService.GetStrategyExecution($scope.strategyId)
           .then(function (response) {
               fillScopeFromStrategyExecutionObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }

    function buildStrategyExecutionObjFromScope() {
        var strategyObject = {
            Id: ($scope.strategyId != null) ? $scope.strategyId : 0,
            PeriodId: $scope.selectedPeriod.Id,
            Name: $scope.name,
            Description: $scope.description,
            IsDefault: $scope.isDefault,
            IsEnabled: $scope.isEnabled,
            GapBetweenConsecutiveCalls: $scope.gapBetweenConsecutiveCalls,
            GapBetweenFailedConsecutiveCalls: $scope.gapBetweenFailedConsecutiveCalls,
            MaxLowDurationCall: $scope.maxLowDurationCall,
            MinimumCountofCallsinActiveHour: $scope.minCountofCallsinActiveHour,
            PeakHours: $scope.selectedPeakHours,
            StrategyExecutionFilters: [],
            StrategyExecutionLevels: [],
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
                strategyObject.StrategyExecutionFilters.push(filterItem);
            }
        });

        angular.forEach($scope.strategyLevels, function (level) {
            var strategyLevelItem = {
                SuspicionLevelId: level.suspicionLevel.id,
                StrategyExecutionLevelCriterias: []
            };

            var index = 0;
            angular.forEach(level.StrategyExecutionLevelCriterias, function (levelCriteria) {
                if ($scope.strategyFilters[index].isSelected && levelCriteria.isSelected) {
                    var levelCriteriaItem = {
                        FilterId: $scope.strategyFilters[index].filterId
                    };

                    levelCriteriaItem.Percentage = ((parseFloat(levelCriteria.percentage) + 100) / 100);
                    strategyLevelItem.StrategyExecutionLevelCriterias.push(levelCriteriaItem);
                }
                index++;
            });

            strategyObject.StrategyExecutionLevels.push(strategyLevelItem);
        });

        return strategyObject;
    }

    function loadFiltersForAddMode() {
        angular.forEach($scope.filterDefinitions, function (filterDef) {
            var filterItem = {
                filterId: filterDef.filterId,
                description: filterDef.description,
                abbreviation: filterDef.abbreviation,
                label: filterDef.label,
                minValue: filterDef.minValue,
                maxValue: filterDef.maxValue,
                decimalPrecision: filterDef.decimalPrecision,
                excludeHourly: filterDef.excludeHourly,
                toolTip: filterDef.toolTip,
                upSign: filterDef.upSign,
                downSign: filterDef.downSign,
                parameters: filterDef.parameters
            };
            $scope.strategyFilters.push(filterItem);
        });
    }

    function fillScopeFromStrategyExecutionObj(strategyObject) {
        $scope.name = strategyObject.Name;
        $scope.description = strategyObject.Description;
        $scope.isDefault = strategyObject.IsDefault;
        $scope.isEnabled = strategyObject.IsEnabled;
        $scope.gapBetweenConsecutiveCalls = strategyObject.GapBetweenConsecutiveCalls;
        $scope.gapBetweenFailedConsecutiveCalls = strategyObject.GapBetweenFailedConsecutiveCalls;
        $scope.maxLowDurationCall = strategyObject.MaxLowDurationCall;
        $scope.minCountofCallsinActiveHour = strategyObject.MinimumCountofCallsinActiveHour;
        $scope.selectedPeriod = UtilsService.getItemByVal($scope.periods, strategyObject.PeriodId, "Id");
        $scope.selectedPeakHours.length = 0;
        angular.forEach(strategyObject.PeakHours, function (peakHour) {

            var peakHourItem = {
                id: peakHour.Id,
                name: peakHour.Name
            };

            $scope.selectedPeakHours.push(peakHourItem);
        });

        $scope.isGettingFilters = true;
        angular.forEach($scope.filterDefinitions, function (filterDef) {

            var filterItem = {
                filterId: filterDef.filterId,
                description: filterDef.description,
                abbreviation: filterDef.abbreviation,
                label: filterDef.label,
                minValue: filterDef.minValue,
                maxValue: filterDef.maxValue,
                decimalPrecision: filterDef.decimalPrecision,
                excludeHourly: filterDef.excludeHourly,
                toolTip: filterDef.toolTip,
                upSign: filterDef.upSign,
                downSign: filterDef.downSign,
                parameters: filterDef.parameters
            };


            var existingItem = UtilsService.getItemByVal(strategyObject.StrategyExecutionFilters, filterDef.filterId, "FilterId");
            if (existingItem != undefined && existingItem != null) {
                filterItem.isSelected = true;
                filterItem.threshold = existingItem.Threshold;
            }
            $scope.strategyFilters.push(filterItem);

        });

        $scope.isGettingSuspicionLevels = true;
        angular.forEach(strategyObject.StrategyExecutionLevels, function (level) {

            var strategyLevelItem = {
                suspicionLevel: UtilsService.getItemByVal($scope.suspicionLevels, level.SuspicionLevelId, "id"),
                StrategyExecutionLevelCriterias: []
            };


            angular.forEach($scope.filterDefinitions, function (filterDef) {

                var levelCriteriaItem = {
                    filterId: filterDef.FilterId,
                    upSign: filterDef.upSign,
                    downSign: filterDef.downSign,
                    percentage: 0
                };

                var existingItem = UtilsService.getItemByVal(level.StrategyExecutionLevelCriterias, filterDef.filterId, "FilterId");
                if (existingItem != undefined && existingItem != null) {
                    levelCriteriaItem.isSelected = true;
                    levelCriteriaItem.percentage = ((parseFloat(existingItem.Percentage) * 100) - 100);
                }
                strategyLevelItem.StrategyExecutionLevelCriterias.push(levelCriteriaItem);
            });

            $scope.strategyLevels.push(strategyLevelItem);

        });
    }

    function isValid(strategyObject) {
        var countStrategyExecutionFilters = 0;
        var countStrategyExecutionLevels = 0;


        angular.forEach(strategyObject.StrategyExecutionFilters, function (itm) {
            countStrategyExecutionFilters++;
        });


        angular.forEach(strategyObject.StrategyExecutionLevels, function (level) {
            angular.forEach(level.StrategyExecutionLevelCriterias, function (itm) {
                countStrategyExecutionLevels++;
            });
        });




        if (countStrategyExecutionFilters == 0) {
            VRNotificationService.showError("At least one filter should be specified in a strategy. ");
            return false;

        }

        if (countStrategyExecutionLevels == 0) {
            VRNotificationService.showError("At least one rule with filter(s) should be specified in a strategy. ");
            return false;

        }


        return true;
    }

    function addStrategyExecution() {
        var strategyObject = buildStrategyExecutionObjFromScope();

        if (isValid(strategyObject)) {
            return StrategyExecutionAPIService.AddStrategyExecution(strategyObject)
          .then(function (response) {
              if (VRNotificationService.notifyOnItemAdded("StrategyExecution", response, "Name")) {
                  if ($scope.onStrategyExecutionAdded != undefined)
                      $scope.onStrategyExecutionAdded(response.InsertedObject);
                  $scope.modalContext.closeModal();
              }
          }).catch(function (error) {
              VRNotificationService.notifyException(error, $scope);
          });
        }
    }

    function updateStrategyExecution() {
        var strategyObject = buildStrategyExecutionObjFromScope();

        if (isValid(strategyObject)) {
            return StrategyExecutionAPIService.UpdateStrategyExecution(strategyObject)
                              .then(function (response) {
                                  if (VRNotificationService.notifyOnItemUpdated("StrategyExecution", response, "Name")) {
                                      if ($scope.onStrategyExecutionUpdated != undefined)
                                          $scope.onStrategyExecutionUpdated(response.UpdatedObject);
                                      $scope.modalContext.closeModal();
                                  }
                              }).catch(function (error) {
                                  VRNotificationService.notifyException(error, $scope);
                              });
        }
    }

    function loadFilters() {
        var index = 0;
        return StrategyExecutionAPIService.GetFilters().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.filterDefinitions.push({
                    filterId: itm.FilterId,
                    description: itm.Description,
                    abbreviation: itm.Abbreviation,
                    label: itm.Label,
                    minValue: itm.MinValue,
                    maxValue: itm.MaxValue,
                    decimalPrecision: itm.DecimalPrecision,
                    excludeHourly: itm.ExcludeHourly,
                    toolTip: itm.ToolTip,
                    upSign: itm.UpSign,
                    downSign: itm.DownSign,
                    parameters: itm.Parameters
                });
            });
        });
    }

    function loadPeriods() {
        return StrategyExecutionAPIService.GetPeriods().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.periods.push(itm);
            });

            $scope.selectedPeriod = $scope.periods[0]; // Mohamad
        });
    }

    StrategyExecutionEditorController.viewVisibilityChanged = function () {

        isFilterTabShown = !isFilterTabShown;
        isLevelsTabShow = !isLevelsTabShow;
    };

    $scope.showSwitch = function (filter) {

        if (filter.excludeHourly && $scope.selectedPeriod.Id == 1) {
            filter.isSelected = false;
            return false;
        }
        else
            return true;
    }
}

appControllers.controller('FraudAnalysis_StrategyExecutionEditorController', StrategyExecutionEditorController);
