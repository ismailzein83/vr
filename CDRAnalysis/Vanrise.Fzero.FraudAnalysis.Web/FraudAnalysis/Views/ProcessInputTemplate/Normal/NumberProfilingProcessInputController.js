"use strict";

NumberProfilingProcessInputController.$inject = ['$scope', 'UtilsService', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'HourEnum', 'VRValidationService'];

function NumberProfilingProcessInputController($scope, UtilsService, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, HourEnum, VRValidationService) {

    defineScope();

    load();

    function defineScope() {

        var yesterday = new Date();
        yesterday.setDate(yesterday.getDate() - 1);

        $scope.fromDate = yesterday;
        $scope.toDate = new Date();

        $scope.validateTimeRange = function () {
            return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
        }

        $scope.createProcessInputObjects = [];

        $scope.periods = [];
        $scope.selectedPeriod ;


        $scope.hours = [];
        angular.forEach(HourEnum, function (itm) {
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

        $scope.fixedPrefixes = [{ value: "88888888" }];
        $scope.selectedFixedPrefixes = [];
        for (var i = 0; i < $scope.fixedPrefixes.length; i++) {
            $scope.selectedFixedPrefixes.push($scope.fixedPrefixes[i]);
        }

        $scope.prefixLengths = [0, 1, 2, 3];
        $scope.selectedPrefixLength = $scope.prefixLengths[1];

        $scope.createProcessInput.getData = function () {

            $scope.PeakHoursIds = [];
            angular.forEach($scope.selectedPeakHours, function (itm) {
                $scope.PeakHoursIds.push(itm.id)
            });

            var runningDate = new Date($scope.fromDate);
            var selectedToDate = new Date($scope.toDate);

            $scope.createProcessInputObjects.length = 0;


            if ($scope.selectedPeriod.Id == 1)//Hourly
            {
                while (runningDate < selectedToDate) {
                    var fromDate = new Date(runningDate);
                    var toDate = new Date(runningDate.setHours(runningDate.getHours() + 1));



                    $scope.createProcessInputObjects.push({
                        InputArguments: {
                            $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                            FixedPrefixes: $scope.selectedFixedPrefixes != undefined ? UtilsService.getPropValuesFromArray($scope.selectedFixedPrefixes, "value") : null,
                            PrefixLength: $scope.selectedPrefixLength,
                            FromDate: new Date(fromDate),
                            ToDate: new Date(toDate),
                            PeriodId: $scope.selectedPeriod.Id,
                            IncludeWhiteList :  $scope.includeWhiteList,
                            Parameters: { GapBetweenConsecutiveCalls: $scope.gapBetweenConsecutiveCalls, GapBetweenFailedConsecutiveCalls: $scope.gapBetweenFailedConsecutiveCalls, MaxLowDurationCall: $scope.maxLowDurationCall, MinimumCountofCallsinActiveHour: $scope.minCountofCallsinActiveHour, PeakHoursIds: $scope.PeakHoursIds }
                        }
                    });
                    runningDate = new Date(toDate);
                }

            }

            else if ($scope.selectedPeriod.Id == 2) //Daily
            {
                while (runningDate < selectedToDate) {
                    var fromDate = new Date(runningDate);
                    var toDate = new Date(runningDate.setHours(runningDate.getHours() + 24));

                    $scope.createProcessInputObjects.push({
                        InputArguments: {
                            $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                            FixedPrefixes: $scope.selectedFixedPrefixes != undefined ? UtilsService.getPropValuesFromArray($scope.selectedFixedPrefixes, "value") : null,
                            PrefixLength: $scope.selectedPrefixLength,
                            FromDate: new Date(fromDate),
                            ToDate: new Date(toDate),
                            PeriodId: $scope.selectedPeriod.Id,
                            IncludeWhiteList: $scope.includeWhiteList,
                            Parameters: { GapBetweenConsecutiveCalls: $scope.gapBetweenConsecutiveCalls, GapBetweenFailedConsecutiveCalls: $scope.gapBetweenFailedConsecutiveCalls, MaxLowDurationCall: $scope.maxLowDurationCall, MinimumCountofCallsinActiveHour: $scope.minCountofCallsinActiveHour, PeakHoursIds: $scope.PeakHoursIds }
                        }
                    });

                    runningDate = new Date(toDate);
                }


            }

            return $scope.createProcessInputObjects;

        };


    }

    function load() {
        loadPeriods();
    }

    function loadPeriods() {
        return StrategyAPIService.GetPeriods().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.periods.push(itm);
            });
        });
    }

}

appControllers.controller('FraudAnalysis_NumberProfilingProcessInputController', NumberProfilingProcessInputController)



