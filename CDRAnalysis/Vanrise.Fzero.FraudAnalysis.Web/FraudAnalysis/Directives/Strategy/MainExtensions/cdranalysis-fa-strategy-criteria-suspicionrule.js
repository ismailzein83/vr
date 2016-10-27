"use strict";

app.directive("cdranalysisFaStrategyCriteriaSuspicionrule", ["StrategyAPIService", "CDRAnalysis_FA_PeriodEnum", "CDRAnalysis_FA_SuspicionLevelEnum","UtilsService", function (StrategyAPIService, CDRAnalysis_FA_PeriodEnum, CDRAnalysis_FA_SuspicionLevelEnum, UtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "criteriaCtrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: "/Client/Modules/FraudAnalysis/Directives/Strategy/MainExtensions/Templates/SuspicionRuleStrategyTemplate.html"

    };
    function DirectiveConstructor($scope, ctrl) {


        this.initializeController = initializeController;

        var filters;
        var filter;
        var context;
        var strategyEntity;
        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.strategyFilters = [];
            $scope.scopeModel.suspicionRules = [];
            $scope.scopeModel.suspicionLevels = UtilsService.getArrayEnum(CDRAnalysis_FA_SuspicionLevelEnum);
            $scope.scopeModel.validateStrategyFilters = function () {
                if ($scope.scopeModel.strategyFilters.length == 0)
                    return 'No filters found';

                for (var i = 0; i < $scope.scopeModel.strategyFilters.length; i++) {
                    if ($scope.scopeModel.strategyFilters[i].isSelected)
                        return null;
                }

                return 'No filter(s) selected';
            };
            $scope.scopeModel.validateStrategyLevels = function () {
                if ($scope.scopeModel.strategyFilters.length == 0) {
                    return 'No filters found';
                }

                if ($scope.scopeModel.suspicionRules.length == 0) {
                    return 'No suspicion rules added';
                }

                var filtersToUseCount = 0;
                var filterUsages = [];
                for (var i = 0; i < $scope.scopeModel.strategyFilters.length; i++) {
                    var filterUsage = {
                        mustBeUsed: $scope.scopeModel.strategyFilters[i].isSelected,
                        isUsed: false
                    };

                    if (filterUsage.mustBeUsed)
                        filtersToUseCount++;

                    filterUsages.push(filterUsage);
                }

                for (var i = 0; i < $scope.scopeModel.suspicionRules.length; i++) {
                    var strategyLevel = $scope.scopeModel.suspicionRules[i];
                    var aFilterIsUsed = false;

                    for (var j = 0; j < strategyLevel.StrategyLevelCriterias.length; j++) {
                        if ($scope.scopeModel.strategyFilters[j].isSelected) {
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
                    return 'Not all selected filters are used in suspicion rules';
                }

                return null;
            };
            $scope.scopeModel.addSuspicionRule = function () {
                var strategyLevelItem = {
                    suspicionLevel: $scope.scopeModel.suspicionLevels[0],
                    StrategyLevelCriterias: []
                };

                angular.forEach($scope.scopeModel.strategyFilters, function (filter) {
                    var levelCriteriaItem = {
                        filterId: filter.filterId,
                        percentage: 0,
                        upSign: filter.upSign,
                        downSign: filter.downSign
                    };
                    strategyLevelItem.StrategyLevelCriterias.push(levelCriteriaItem);
                });

                $scope.scopeModel.suspicionRules.push(strategyLevelItem);
            };
            $scope.scopeModel.deleteSuspicionRule = function (rule) {
                var index = $scope.scopeModel.suspicionRules.indexOf(rule);
                $scope.scopeModel.suspicionRules.splice(index, 1);
            };
            $scope.scopeModel.showThresholdHint = function (filter, strategyLevel) {
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
            $scope.scopeModel.onFilterSelected = function(dataItem)
            {
                if (context != undefined)
                {
                    context.setParameterVisibility(dataItem.isSelected, dataItem.parameters);
                }
            }

            defineAPI();
        }

        function defineAPI() {


            var api = {};
            api.getData = function () {
                var strategyFilters = [];
                angular.forEach($scope.scopeModel.strategyFilters, function (filter) {
                    if (filter.isSelected) {
                        var filterItem = {
                            FilterId: filter.filterId,
                            Description: filter.description,
                            Abbreviation: filter.abbreviation,
                            Threshold: filter.threshold
                        };
                        strategyFilters.push(filterItem);
                    }
                });
                var strategyLevels = [];
                angular.forEach($scope.scopeModel.suspicionRules, function (level) {
                    var strategyLevelItem = {
                        SuspicionLevelId: level.suspicionLevel.value,
                        StrategyLevelCriterias: []
                    };

                    var index = 0;
                    angular.forEach(level.StrategyLevelCriterias, function (levelCriteria) {
                        if ($scope.scopeModel.strategyFilters[index].isSelected && levelCriteria.isSelected) {
                            var levelCriteriaItem = {
                                FilterId: $scope.scopeModel.strategyFilters[index].filterId
                            };

                            levelCriteriaItem.Percentage = ((parseFloat(levelCriteria.percentage) + 100) / 100);
                            strategyLevelItem.StrategyLevelCriterias.push(levelCriteriaItem);
                        }
                        index++;
                    });
                    strategyLevels.push(strategyLevelItem);
                });
                return {
                    $type: "Vanrise.Fzero.FraudAnalysis.MainExtensions.SuspicionRuleStrategySettingsCriteria, Vanrise.Fzero.FraudAnalysis.MainExtensions",
                    StrategyLevels:strategyLevels,
                    StrategyFilters:strategyFilters
                }
            };

            api.getFilterHint = function(parameter)
            {
                if (parameter != undefined) {
                    var filters = [];
                    for (var i = 0; i < $scope.scopeModel.strategyFilters.length; i++) {
                        var filter = $scope.scopeModel.strategyFilters[i];
                        if (filter.parameters != null && filter.parameters.indexOf(parameter) > -1) {
                            filters.push(filter.abbreviation);
                        }
                    }
                    return filters.join(',');
                }
                return null;
            }

            api.load = function (payload) {
                if (payload)
                {
                    strategyEntity = payload.strategyCriteria;
                    filter = payload.filter;
                    context = payload.context;
                }
                $scope.scopeModel.strategyFilters.length = 0;
                $scope.scopeModel.suspicionRules.length = 0;

                var promises = [];
                var promiseDeffered = UtilsService.createPromiseDeferred();
                promises.push(promiseDeffered.promise);
                loadFilters(filter).then(function ()
                {
                    UtilsService.waitMultipleAsyncOperations([loadStrategyFilters, loadSuspicionRules]).then(function () {
                        promiseDeffered.resolve();
                    }).catch(function (error) {
                        promiseDeffered.reject(error);
                    });
                }).catch(function(error){
                    promiseDeffered.reject(error);
                });
             
                function loadSuspicionRules() {
                    if (strategyEntity)
                    {
                        angular.forEach(strategyEntity.StrategyLevels, function (level) {
                            var strategyLevelItem = {
                                suspicionLevel: UtilsService.getItemByVal($scope.scopeModel.suspicionLevels, level.SuspicionLevelId, 'value'),
                                StrategyLevelCriterias: []
                            };

                            angular.forEach(filters, function (filter) {
                                var levelCriteriaItem = {
                                    filterId: filter.FilterId,
                                    upSign: filter.UpSign,
                                    downSign: filter.DownSign,
                                    percentage: 0
                                };

                                var existingItem = UtilsService.getItemByVal(level.StrategyLevelCriterias, filter.FilterId, 'FilterId');
                                if (existingItem != undefined && existingItem != null) {
                                    levelCriteriaItem.isSelected = true;
                                    levelCriteriaItem.percentage = parseInt((parseFloat(existingItem.Percentage) * 100) - 100); // The outer parseInt call is used for formatting purposes
                                }
                                strategyLevelItem.StrategyLevelCriterias.push(levelCriteriaItem);
                            });

                            $scope.scopeModel.suspicionRules.push(strategyLevelItem);
                        });
                    }
                    
                }
                function loadFilters(filter) {
                    return StrategyAPIService.GetFilters(UtilsService.serializetoJson(filter)).then(function (response) {
                        filters = response;
                    });
                }
                function loadStrategyFilters() {
                    if (filters) {
                        for (var i = 0; i < filters.length; i++) {
                            var strategyFilter = getStrategyFilterDataItemWithCommonProperties(filters[i]);

                            if (strategyEntity != undefined) {
                                var entityStrategyFilter = UtilsService.getItemByVal(strategyEntity.StrategyFilters, strategyFilter.filterId, 'FilterId');
                                if (entityStrategyFilter != null) {
                                    strategyFilter.isSelected = true;
                                    strategyFilter.threshold = entityStrategyFilter.Threshold;
                                }
                            }
                            $scope.scopeModel.strategyFilters.push(strategyFilter);
                            $scope.scopeModel.onFilterSelected(strategyFilter);
                        }
                    }
                }
                return UtilsService.waitMultiplePromises(promises);
            }

            function getStrategyFilterDataItemWithCommonProperties(filter) {
                var item = {};

                item.filterId = filter.FilterId;
                item.description = filter.Description;
                item.abbreviation = filter.Abbreviation;
                item.label = filter.Label;
                item.minValue = filter.MinValue;
                item.maxValue = filter.MaxValue;
                item.decimalPrecision = filter.DecimalPrecision;
                item.excludeHourly = filter.ExcludeHourly;
                item.toolTip = filter.ToolTip;
                item.upSign = filter.UpSign;
                item.downSign = filter.DownSign;
                item.parameters = filter.Parameters;
                item.isShown = true;
                if (item.parameters != null && item.parameters.length > 0) {
                    item.hint = 'This filter requires the following parameter(s): ' + item.parameters.join(',');
                    item.hasParameters = true;
                }
                else {
                    item.hint = null;
                    item.hasParameters = false;
                }

                //setIsSelectedAndIsShownForFilterDataItem(item);
                return item;
            }

           
            if (ctrl.onReady != null)
                ctrl.onReady(api);
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
    }

    return directiveDefinitionObject;
}]);
