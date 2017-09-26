"use strict";

app.directive("vrCdrFraudanalysisNumberprofilingManual", ["UtilsService", "VRCommon_HourEnum", "VRValidationService", "CDRAnalysis_FA_PeriodEnum", "VRUIUtilsService", "VRDateTimeService",
    function (UtilsService, VRCommon_HourEnum, VRValidationService, CDRAnalysis_FA_PeriodEnum, VRUIUtilsService, VRDateTimeService) {
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
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: "/Client/Modules/FraudAnalysis/Directives/MainExtensions/ProcessInput/Normal/Templates/NumberProfilingManualTemplate.html"
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            $scope.toDate = VRDateTimeService.getNowDateTime();
            $scope.fromDate = VRDateTimeService.getNowDateTime();
            $scope.toDateHour = VRDateTimeService.getNowDateTime();
            $scope.fromDateHour = VRDateTimeService.getNowDateTime();
            $scope.showDateHour = false;

            var periodSelectorAPI;
            var periodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.onPeriodSelectorReady = function (api) {
                    periodSelectorAPI = api;
                    periodSelectorReadyDeferred.resolve();
                };

                $scope.onPeriodSelectionChanged = function (selectedPeriod) {
                    if (selectedPeriod != undefined) {
                        $scope.showDateHour = (selectedPeriod.Id == CDRAnalysis_FA_PeriodEnum.Hourly.value);
                        $scope.toDate = VRDateTimeService.getNowDateTime();
                        $scope.fromDate = VRDateTimeService.getNowDateTime();
                        $scope.toDateHour = VRDateTimeService.getNowDateTime();
                        $scope.fromDateHour = VRDateTimeService.getNowDateTime();
                    }
                };


                $scope.validateDateRange = function () {
                    return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
                };

                $scope.validateHourRange = function () {
                    return VRValidationService.validateTimeRange($scope.fromDateHour, $scope.toDateHour);
                };

                $scope.createProcessInputObjects = [];


                $scope.hours = [];
                angular.forEach(VRCommon_HourEnum, function (itm) {
                    $scope.hours.push({ id: itm.id, name: itm.name })
                });

                $scope.gapBetweenConsecutiveCalls = 10;
                $scope.gapBetweenFailedConsecutiveCalls = 10;
                $scope.maxLowDurationCall = 8;
                $scope.minCountofCallsinActiveHour = 5;
                $scope.selectedPeakHours = [];

                angular.forEach($scope.hours, function (itm) {
                    if (itm.id >= 12 && itm.id <= 17)
                        $scope.selectedPeakHours.push(itm);
                });


                defineAPI();
            }

            function loadPeriodSelector() {
                var periodSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                periodSelectorReadyDeferred.promise.then(function () {
                    var selectorPayload = { selectedIds: CDRAnalysis_FA_PeriodEnum.Daily.value };
                    VRUIUtilsService.callDirectiveLoad(periodSelectorAPI, selectorPayload, periodSelectorLoadDeferred);
                });

                return periodSelectorLoadDeferred.promise;
            }

            function defineAPI() {

                var api = {};
                api.getData = function () {

                    $scope.PeakHoursIds = [];
                    angular.forEach($scope.selectedPeakHours, function (itm) {
                        $scope.PeakHoursIds.push(itm.id)
                    });


                    $scope.createProcessInputObjects.length = 0;


                    if (periodSelectorAPI.getSelectedIds() == CDRAnalysis_FA_PeriodEnum.Hourly.value)//Hourly
                    {
                        var runningDate = new Date($scope.fromDateHour);
                        while (runningDate < $scope.toDateHour) {
                            var fromDate = new Date(runningDate);
                            var toDate = new Date(runningDate.setHours(runningDate.getHours() + 1));



                            $scope.createProcessInputObjects.push({
                                InputArguments: {
                                    $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                                    FromDate: new Date(fromDate),
                                    ToDate: new Date(toDate),
                                    PeriodId: periodSelectorAPI.getSelectedIds(),
                                    IncludeWhiteList: $scope.includeWhiteList,
                                    Parameters: { GapBetweenConsecutiveCalls: $scope.gapBetweenConsecutiveCalls, GapBetweenFailedConsecutiveCalls: $scope.gapBetweenFailedConsecutiveCalls, MaxLowDurationCall: $scope.maxLowDurationCall, MinimumCountofCallsinActiveHour: $scope.minCountofCallsinActiveHour, PeakHoursIds: $scope.PeakHoursIds }
                                }
                            });
                            runningDate = new Date(toDate);
                        }

                    }

                    else if (periodSelectorAPI.getSelectedIds() == CDRAnalysis_FA_PeriodEnum.Daily.value)//Daily
                    {
                        var runningDate = new Date($scope.fromDate);
                        while (runningDate < $scope.toDate) {
                            var fromDate = new Date(runningDate);
                            var toDate = new Date(runningDate.setHours(runningDate.getHours() + 24));

                            $scope.createProcessInputObjects.push({
                                InputArguments: {
                                    $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                                    FromDate: new Date(fromDate),
                                    ToDate: new Date(toDate),
                                    PeriodId: periodSelectorAPI.getSelectedIds(),
                                    IncludeWhiteList: $scope.includeWhiteList,
                                    Parameters: { GapBetweenConsecutiveCalls: $scope.gapBetweenConsecutiveCalls, GapBetweenFailedConsecutiveCalls: $scope.gapBetweenFailedConsecutiveCalls, MaxLowDurationCall: $scope.maxLowDurationCall, MinimumCountofCallsinActiveHour: $scope.minCountofCallsinActiveHour, PeakHoursIds: $scope.PeakHoursIds }
                                }
                            });

                            runningDate = new Date(toDate);
                        }


                    }

                    return $scope.createProcessInputObjects;

                };

                api.load = function (payload) {
                    var promises = [];
                    promises.push(loadPeriodSelector());
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
