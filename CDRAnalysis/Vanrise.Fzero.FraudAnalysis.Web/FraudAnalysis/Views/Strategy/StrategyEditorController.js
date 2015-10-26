"use strict";

StrategyEditorController.$inject = ['$scope', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'SuspicionLevelEnum', 'HourEnum'];

function StrategyEditorController($scope, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, SuspicionLevelEnum, HourEnum) {

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

        StrategyEditorController.isFilterTabShown = true;
        StrategyEditorController.isLevelsTabShow = false;

        $scope.suspicionLevels = [];
        angular.forEach(SuspicionLevelEnum, function (itm) {
            $scope.suspicionLevels.push({ id: itm.value, name: itm.description })
        });

        $scope.hours = [];
        angular.forEach(HourEnum, function (itm) {
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

        $scope.SaveStrategy = function () {
            if (editMode) {

                return updateStrategy();
            }
            else {
                return addStrategy();
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
            return (item.parameters.length > 0);
        }


        $scope.showParametersHint = function (item) {
            if (item.parameters.length > 0)
                return "This filter requires the following parameter(s)" + item.parameters.join();
        }

        
    }

    function load() {

        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadFilters, loadPeriods])
        .then(function () {
            if (editMode) {
                getStrategy().finally(function () {
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

    function getStrategy() {

        return StrategyAPIService.GetStrategy($scope.strategyId)
           .then(function (response) {
               fillScopeFromStrategyObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }

    function buildStrategyObjFromScope() {
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
                    //console.log(levelCriteriaItem.percentage)

                    strategyLevelItem.StrategyLevelCriterias.push(levelCriteriaItem);
                }

                index++;

            });

            strategyObject.StrategyLevels.push(strategyLevelItem);


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
                downSign: filterDef.downSign
            };
            $scope.strategyFilters.push(filterItem);
        });
    }

    function fillScopeFromStrategyObj(strategyObject) {
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


            var existingItem = UtilsService.getItemByVal(strategyObject.StrategyFilters, filterDef.filterId, "FilterId");
            if (existingItem != undefined && existingItem != null) {
                filterItem.isSelected = true;
                filterItem.threshold = existingItem.Threshold;
            }
            $scope.strategyFilters.push(filterItem);

        });

        $scope.isGettingSuspicionLevels = true;
        angular.forEach(strategyObject.StrategyLevels, function (level) {

            var strategyLevelItem = {
                suspicionLevel: UtilsService.getItemByVal($scope.suspicionLevels, level.SuspicionLevelId, "id"),
                StrategyLevelCriterias: []
            };


            angular.forEach($scope.filterDefinitions, function (filterDef) {

                var levelCriteriaItem = {
                    filterId: filterDef.FilterId,
                    upSign: filterDef.upSign,
                    downSign: filterDef.downSign,
                    percentage :0
                };

                var existingItem = UtilsService.getItemByVal(level.StrategyLevelCriterias, filterDef.filterId, "FilterId");
                if (existingItem != undefined && existingItem != null) {
                    levelCriteriaItem.isSelected = true;
                    levelCriteriaItem.percentage = ((parseFloat(existingItem.Percentage) * 100) - 100);
                }
                strategyLevelItem.StrategyLevelCriterias.push(levelCriteriaItem);
            });

            $scope.strategyLevels.push(strategyLevelItem);

        });
    }

    function isValid(strategyObject) {
        var countStrategyFilters = 0;
        var countStrategyLevels = 0;


        angular.forEach(strategyObject.StrategyFilters, function (itm) {
            countStrategyFilters++;
        });


        angular.forEach(strategyObject.StrategyLevels, function (level) {
            angular.forEach(level.StrategyLevelCriterias, function (itm) {
                    countStrategyLevels++;
            });
        });

        


        if (countStrategyFilters == 0) {
            VRNotificationService.showError("At least one filter should be specified in a strategy. ");
            return false;

        }

        if (countStrategyLevels == 0) {
            VRNotificationService.showError("At least one rule with filter(s) should be specified in a strategy. ");
            return false;

        }

       
        return true;
    }

    function addStrategy() {
        var strategyObject = buildStrategyObjFromScope();

        if (isValid(strategyObject)) {
            return StrategyAPIService.AddStrategy(strategyObject)
          .then(function (response) {
              if (VRNotificationService.notifyOnItemAdded("Strategy", response)) {
                  if ($scope.onStrategyAdded != undefined)
                      $scope.onStrategyAdded(response.InsertedObject);
                  $scope.modalContext.closeModal();
              }
          }).catch(function (error) {
              VRNotificationService.notifyException(error, $scope);
          });
        }
    }

    function updateStrategy() {
        var strategyObject = buildStrategyObjFromScope();

        if (isValid(strategyObject)) {
            return StrategyAPIService.UpdateStrategy(strategyObject)
                              .then(function (response) {
                                  if (VRNotificationService.notifyOnItemUpdated("Strategy", response, "Name")) {
                                      if ($scope.onStrategyUpdated != undefined)
                                          $scope.onStrategyUpdated(response.UpdatedObject);
                                      $scope.modalContext.closeModal();
                                  }
                              }).catch(function (error) {
                                  VRNotificationService.notifyException(error, $scope);
                              });
        }
    }

    function loadFilters() {
        var index = 0;
        return StrategyAPIService.GetFilters().then(function (response) {
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
        return StrategyAPIService.GetPeriods().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.periods.push(itm);
            });

            $scope.selectedPeriod = $scope.periods[0]; // Mohamad
        });
    }

    StrategyEditorController.viewVisibilityChanged = function () {

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

appControllers.controller('FraudAnalysis_StrategyEditorController', StrategyEditorController);
