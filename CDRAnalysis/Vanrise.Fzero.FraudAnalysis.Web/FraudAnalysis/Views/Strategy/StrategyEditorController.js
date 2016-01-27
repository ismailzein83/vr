(function (appControllers) {

    'use strict';

    StrategyEditorController.$inject = ['$scope', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'SuspicionLevelEnum', 'HourEnum', 'VRUIUtilsService'];

    function StrategyEditorController($scope, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, SuspicionLevelEnum, HourEnum, VRUIUtilsService) {
        var isEditMode;

        var periodSelectorAPI;
        var periodSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        
        var strategyId;
        var strategyEntity;

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
            // ?
            $scope.filterDefinitions = [];

            // Directives
            $scope.onPeriodSelectorReady = function (api) {
                periodSelectorAPI = api;
                periodSelectorReadyDeferred.resolve();
            };
            
            // Strategy filters
            $scope.showSwitch = function (filter) {
                if (filter.excludeHourly && periodSelectorAPI.getSelectedIds() == 1) {
                    filter.isSelected = false;
                    return false;
                }
                return true;
            };
            $scope.showParametersHint = function (item) {
                if (item.parameters != undefined) {
                    if (item.parameters.length > 0)
                        return 'This filter requires the following parameter(s): ' + item.parameters.join(',');
                }
                return;
            };
            $scope.hasParameters = function (item) {
                if (item.parameters != undefined) {
                    return (item.parameters.length > 0);
                }
                return;
            };
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
                return;
            };

            // Strategy parameters
            $scope.hours = UtilsService.getArrayEnum(HourEnum);
            $scope.strategyFilters = [];
            $scope.selectedPeakHours = [];

            $scope.gapBetweenConsecutiveCalls = 10;
            $scope.gapBetweenFailedConsecutiveCalls = 10;
            $scope.maxLowDurationCall = 8;
            $scope.minCountofCallsinActiveHour = 5;

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

            // Junk code
            //$scope.hasFilters = function (parameter) {
            //    var found = false;
            //    if ($scope.strategyFilters.length > 0) {
            //        angular.forEach($scope.strategyFilters, function (filter) {
            //            if (filter.parameters.indexOf(parameter) > -1 && filter.isSelected) {
            //                found = true;
            //            }
            //        });
            //    }
            //    return found;
            //}
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticControls, loadPeriodSelector, loadFilters]).then(function () {
                if (isEditMode) {
                    loadPeakHoursForEditMode();
                    setFiltersForEditMode();
                }
                else {
                    loadPeakHoursForAddMode();
                    setFiltersForAddMode();
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
                    
                    loadStrategyLevels();
                }

                function loadStrategyLevels() {
                    angular.forEach(strategyEntity.StrategyLevels, function (level) {
                        var strategyLevelItem = {
                            suspicionLevel: UtilsService.getItemByVal($scope.suspicionLevels, level.SuspicionLevelId, 'value'),
                            StrategyLevelCriterias: []
                        };

                        angular.forEach($scope.filterDefinitions, function (filterDef) {

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
            function loadFilters() {
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

            function setFiltersForAddMode() {
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
            function setFiltersForEditMode() {
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

                    var existingItem = UtilsService.getItemByVal(strategyEntity.StrategyFilters, filterDef.filterId, 'FilterId');

                    if (existingItem != undefined && existingItem != null) {
                        filterItem.isSelected = true;
                        filterItem.threshold = existingItem.Threshold;
                    }

                    $scope.strategyFilters.push(filterItem);
                });
            }
            function loadPeakHoursForAddMode() {
                for (var i = 0; i < $scope.hours; i++) {
                    if ($scope.hours[i].id >= 12 && $scope.hours[i].id <= 17) {
                        $scope.selectedPeakHours.push($scope.hours[i]);
                    }
                }
            }
            function loadPeakHoursForEditMode() {
                if (strategyEntity.PeakHours) {
                    for (var i = 0; i < strategyEntity.PeakHours.length; i++) {
                        $scope.selectedPeakHours.push({
                            id: strategyEntity.PeakHours[i].Id,
                            name: strategyEntity.PeakHours[i].Name
                        });
                    }
                }
            }
        }

        function addStrategy() {
            var strategyObject = buildStrategyObjFromScope();

            if (isValid(strategyObject)) {
                return StrategyAPIService.AddStrategy(strategyObject)
              .then(function (response) {
                  if (VRNotificationService.notifyOnItemAdded('Strategy', response, 'Name')) {
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
                                      if (VRNotificationService.notifyOnItemUpdated('Strategy', response, 'Name')) {
                                          if ($scope.onStrategyUpdated != undefined)
                                              $scope.onStrategyUpdated(response.UpdatedObject);
                                          $scope.modalContext.closeModal();
                                      }
                                  }).catch(function (error) {
                                      VRNotificationService.notifyException(error, $scope);
                                  });
            }
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
                VRNotificationService.showError('At least one filter should be specified in a strategy. ');
                return false;

            }

            if (countStrategyLevels == 0) {
                VRNotificationService.showError('At least one rule with filter(s) should be specified in a strategy. ');
                return false;

            }


            return true;
        }
    }

    appControllers.controller('CDRAnalysis_FA_StrategyEditorController', StrategyEditorController);

})(appControllers);
