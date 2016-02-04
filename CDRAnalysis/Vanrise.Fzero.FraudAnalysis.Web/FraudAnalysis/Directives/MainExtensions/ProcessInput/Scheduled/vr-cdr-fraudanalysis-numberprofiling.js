﻿"use strict";

app.directive("vrCdrFraudanalysisNumberprofiling", ["UtilsService", "VRUIUtilsService", "VRNotificationService", "CDRAnalysis_FA_PeriodAPIService", "VRCommon_HourEnum", "CDRAnalysis_FA_PeriodEnum",
function (UtilsService, VRUIUtilsService, VRNotificationService, CDRAnalysis_FA_PeriodAPIService, VRCommon_HourEnum, CDRAnalysis_FA_PeriodEnum) {
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
        templateUrl: "/Client/Modules/FraudAnalysis/Directives/MainExtensions/ProcessInput/Scheduled/Templates/NumberProfilingTemplate.html"
    };


    function DirectiveConstructor($scope, ctrl) {

        this.initializeController = initializeController;
        var periodSelectorAPI;
        var periodSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var createNumberProfileEntity;


        function initializeController() {

            $scope.onPeriodSelectorReady = function (api) {
                periodSelectorAPI = api;
                periodSelectorReadyDeferred.resolve();
            };

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

                var payload = {
                };
                if (createNumberProfileEntity != undefined && createNumberProfileEntity.PeriodId != undefined)
                    payload.selectedIds = createNumberProfileEntity.PeriodId;

                VRUIUtilsService.callDirectiveLoad(periodSelectorAPI, payload, periodSelectorLoadDeferred);
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


                return {
                    $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                    PeriodId: periodSelectorAPI.getSelectedIds(),
                    IncludeWhiteList: false,
                    Parameters: { GapBetweenConsecutiveCalls: $scope.gapBetweenConsecutiveCalls, GapBetweenFailedConsecutiveCalls: $scope.gapBetweenFailedConsecutiveCalls, MaxLowDurationCall: $scope.maxLowDurationCall, MinimumCountofCallsinActiveHour: $scope.minCountofCallsinActiveHour, PeakHoursIds: $scope.PeakHoursIds }
                };

            };

            api.getExpressionsData = function () {

                return { "ScheduleTime": "ScheduleTime" };

            };

            api.load = function (payload) {
                var data;
                if (payload != undefined && payload.data != undefined) {
                    data = payload.data;
                }
                return CDRAnalysis_FA_PeriodAPIService.GetPeriods().then(function (response) {
                    $scope.periods.length = 0;
                    angular.forEach(response, function (itm) {
                        $scope.periods.push(itm);
                    });
                    if (data != undefined) {
                        $scope.isGettingData = true;
                        $scope.gapBetweenConsecutiveCalls = data.Parameters.GapBetweenConsecutiveCalls;
                        $scope.gapBetweenFailedConsecutiveCalls = data.Parameters.GapBetweenFailedConsecutiveCalls;
                        $scope.maxLowDurationCall = data.Parameters.MaxLowDurationCall;
                        $scope.minCountofCallsinActiveHour = data.Parameters.MinimumCountofCallsinActiveHour;
                        $scope.selectedPeriod = UtilsService.getItemByVal($scope.periods, data.PeriodId, "Id");
                        $scope.selectedPeakHours.length = 0;

                        angular.forEach(data.Parameters.PeakHoursIds, function (peakHour) {
                            $scope.selectedPeakHours.push(UtilsService.getItemByVal($scope.hours, peakHour, "id"));
                        });
                    }
                });

            }

            api.load = function (payload) {

                var promises = [];
                if (payload != undefined && payload.data != undefined) {
                    createNumberProfileEntity = payload.data;
                }

                promises.push(loadPeriodSelector());
                return UtilsService.waitMultiplePromises(promises);

            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
