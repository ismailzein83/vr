"use strict";

NumberProfilingProcessInput_Scheduled.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRCommon_HourEnum', 'VRUIUtilsService'];

function NumberProfilingProcessInput_Scheduled($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, VRCommon_HourEnum, VRUIUtilsService) {
    var prefixDirectiveAPI;
    var prefixReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    defineScope();

    load();

    function defineScope() {
        $scope.selectedPrefixIds;
        $scope.isEditMode = false;
        $scope.processInputArguments = [];

        $scope.periods = [];
        $scope.selectedPeriod;



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


        $scope.schedulerTaskAction.rawExpressions.getData = function () {
            return { "ScheduleTime": "ScheduleTime" };
        };



        $scope.schedulerTaskAction.processInputArguments.getData = function () {

            $scope.PeakHoursIds = [];
            angular.forEach($scope.selectedPeakHours, function (itm) {
                $scope.PeakHoursIds.push(itm.id)
            });


            return {
                $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                PeriodId: $scope.selectedPeriod.Id,
                IncludeWhiteList: false,
                FixedPrefixes: prefixDirectiveAPI.getSelectedIds(),
                PrefixLength: $scope.selectedPrefixLength,
                Parameters: { GapBetweenConsecutiveCalls: $scope.gapBetweenConsecutiveCalls, GapBetweenFailedConsecutiveCalls: $scope.gapBetweenFailedConsecutiveCalls, MaxLowDurationCall: $scope.maxLowDurationCall, MinimumCountofCallsinActiveHour: $scope.minCountofCallsinActiveHour, PeakHoursIds: $scope.PeakHoursIds }
            };
        };

        $scope.onPrefixDirectiveReady = function (api) {
            prefixDirectiveAPI = api;
            prefixReadyPromiseDeferred.resolve();
        }

        $scope.prefixLengths = [0, 1, 2, 3];
        $scope.selectedPrefixLength = $scope.prefixLengths[1];

    };

    function load() {
        $scope.isGettingData = true;

        $scope.periods.length = 0;
        $scope.periods = [];
        StrategyAPIService.GetPeriods().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.periods.push(itm);
            });

            var data = $scope.schedulerTaskAction.processInputArguments.data;

            if (data != null) {
                $scope.isEditMode = true;
                $scope.selectedPrefixIds = data.FixedPrefixes;
                $scope.selectedPrefixLength = data.PrefixLength;

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
            getPrefixesInfo();
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        }).finally(function () {
            $scope.isGettingData = false;
        });

    }

    function getPrefixesInfo() {
        var prefixLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        prefixReadyPromiseDeferred.promise
            .then(function () {
                var directivePayload = {};
                if (!$scope.isEditMode)
                    directivePayload.selectAll = true;
                directivePayload.selectedIds = $scope.selectedPrefixIds;

                VRUIUtilsService.callDirectiveLoad(prefixDirectiveAPI, directivePayload, prefixLoadPromiseDeferred);
            });
        return prefixLoadPromiseDeferred.promise;
    }


}

appControllers.controller('FraudAnalysis_NumberProfilingProcessInput_Scheduled', NumberProfilingProcessInput_Scheduled)



