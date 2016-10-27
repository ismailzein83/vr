(function (app) {

    'use strict';

    StrategyParametersDirective.$inject = ['StrategyAPIService', 'UtilsService', 'VRUIUtilsService','CDRAnalysis_FA_ParametersService'];

    function StrategyParametersDirective(StrategyAPIService, UtilsService, VRUIUtilsService, CDRAnalysis_FA_ParametersService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                label: '@',
                customvalidate: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var strategyParameters = new StrategyParameters($scope, ctrl, $attrs);
                strategyParameters.initializeController();
            },
            controllerAs: "strategyCriteriaCtrl",
            bindToController: true,
            templateUrl: '/Client/Modules/FraudAnalysis/Directives/Strategy/Templates/StrategyParameterTemplate.html'

        };
        function StrategyParameters($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var hourSelectorAPI;
            var hourSelectorReadyDeferred = UtilsService.createPromiseDeferred()
            var strategyParameters;
            var context;
            var filter;
            var selectedIds = [];
            var defaultSelectedIds = [];
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.selectedPeakHours = [];
                $scope.scopeModel.gapBetweenConsecutiveCalls = 10;
                $scope.scopeModel.gapBetweenFailedConsecutiveCalls = 10;
                $scope.scopeModel.maxLowDurationCall = 8;
                $scope.scopeModel.minCountofCallsinActiveHour = 5;

                $scope.scopeModel.showMinimumCountOfCallParameter = false;
                $scope.scopeModel.minimumCountOfCallParameterHint = "";
                $scope.scopeModel.showSelectedPeakHoursParameter = false;
                $scope.scopeModel.selectedPeakHoursParameterHint = "";
                $scope.scopeModel.showGapBetweenConsecutiveCallsParameter = false;
                $scope.scopeModel.gapBetweenConsecutiveCallsParameterHint="";
                $scope.scopeModel.showGapBetweenFailedConsecutiveCallsParameter = false;
                $scope.scopeModel.gapBetweenFailedConsecutiveCallsParameterHint="";
                $scope.scopeModel.showMaxLowDurationCallParameter = false;
                $scope.scopeModel.maxLowDurationCallParameterHint = "";
                $scope.scopeModel.onHourSelectorReady = function (api) {
                    hourSelectorAPI = api;
                    hourSelectorReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        filter = payload.filter;
                        context = payload.context;
                        if (payload.strategyParameters != undefined) {
                            strategyParameters = payload.strategyParameters;
                            $scope.scopeModel.gapBetweenConsecutiveCalls = strategyParameters.GapBetweenConsecutiveCalls;
                            $scope.scopeModel.gapBetweenFailedConsecutiveCalls = strategyParameters.GapBetweenFailedConsecutiveCalls;
                            $scope.scopeModel.maxLowDurationCall = strategyParameters.MaxLowDurationCall;
                            $scope.scopeModel.minCountofCallsinActiveHour = strategyParameters.MinimumCountofCallsinActiveHour;
                            if (strategyParameters.PeakHours != undefined) {
                                selectedIds.length = 0;;
                                for (var i = 0; i < strategyParameters.PeakHours.length; i++) {
                                    selectedIds.push(strategyParameters.PeakHours[i].Id);
                                }
                              promises.push(loadHourSelector(selectedIds));
                            } else {
                                CDRAnalysis_FA_ParametersService.getDefaultPeakHourIds().then(function (response) {
                                    if (response != undefined) {
                                        defaultSelectedIds = response;
                                        selectedIds = defaultSelectedIds
                                    }
                                });
                            }
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setParameterVisibility = function (visibility, parameters)
                {
                    if (parameters != undefined)
                    {
                        for (var parameter in parameters) {
                            switch (parameters[parameter]) {
                                case "Minimum Count of Calls per Hour":
                                    $scope.scopeModel.showMinimumCountOfCallParameter = visibility;
                                    if (context != undefined) {
                                        $scope.scopeModel.minimumCountOfCallParameterHint = context.getFilterHint(parameters[parameter]);
                                    }
                                    break;
                                case "Peak Hours":
                                  if (visibility) {
                                        var hourSelectorPayload = {
                                            selectedIds: selectedIds != undefined ? selectedIds : defaultSelectedIds
                                        };
                                        var setLoaderHourSelector = function (value) { setTimeout(function () { $scope.scopeModel.isLoadingHourlySelector = value; UtilsService.safeApply($scope) }); };
                                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, hourSelectorAPI, hourSelectorPayload, setLoaderHourSelector);
                                        selectedIds = undefined;
                                   }
                                    $scope.scopeModel.showSelectedPeakHoursParameter = visibility;
                                    if (context != undefined) {
                                        $scope.scopeModel.selectedPeakHoursParameterHint = context.getFilterHint(parameters[parameter]);
                                    }
                                    break;
                                case "Gap between Consecutive Calls in Seconds":
                                    $scope.scopeModel.showGapBetweenConsecutiveCallsParameter = visibility;
                                    if (context != undefined) {
                                        $scope.scopeModel.gapBetweenConsecutiveCallsParameterHint = context.getFilterHint(parameters[parameter]);
                                    }
                                    break;
                                case "Gap between Failed Consecutive Calls in Seconds":
                                    $scope.scopeModel.showGapBetweenFailedConsecutiveCallsParameter = visibility;
                                    if (context != undefined) {
                                        $scope.scopeModel.gapBetweenFailedConsecutiveCallsParameterHint = context.getFilterHint(parameters[parameter]);
                                    }
                                    break;
                                case "Maximum Low Duration Call (s)":
                                    $scope.scopeModel.showMaxLowDurationCallParameter = visibility;
                                    if (context != undefined) {
                                        $scope.scopeModel.maxLowDurationCallParameterHint = context.getFilterHint(parameters[parameter]);
                                    }
                                    break;
                            }
                        }
                    }
                }

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        GapBetweenConsecutiveCalls :$scope.scopeModel.showGapBetweenConsecutiveCallsParameter?  $scope.scopeModel.gapBetweenConsecutiveCalls:undefined,
                        GapBetweenFailedConsecutiveCalls : $scope.scopeModel.showGapBetweenFailedConsecutiveCallsParameter? $scope.scopeModel.gapBetweenFailedConsecutiveCalls:undefined,
                        MaxLowDurationCall :$scope.scopeModel.showMaxLowDurationCallParameter? $scope.scopeModel.maxLowDurationCall:undefined,
                        MinimumCountofCallsinActiveHour: $scope.scopeModel.showMinimumCountOfCallParameter?$scope.scopeModel.minCountofCallsinActiveHour:undefined,
                        PeakHours: $scope.scopeModel.showSelectedPeakHoursParameter? $scope.scopeModel.selectedPeakHours:[],
                    };
                    return data;
                }
            }

            function loadHourSelector() {
                var hourSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                hourSelectorReadyDeferred.promise.then(function () {
                    var hourSelectorPayload = {
                        selectedIds: selectedIds
                    };
                    VRUIUtilsService.callDirectiveLoad(hourSelectorAPI, hourSelectorPayload, hourSelectorLoadDeferred);
                });
                return hourSelectorLoadDeferred.promise;
            }
        }
    }

    app.directive('cdranalysisFaStrategyParameters', StrategyParametersDirective);

})(app);