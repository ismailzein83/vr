var NumberProfilingProcessInput_Scheduled = function ($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, HourEnum) {
    var pageLoaded = false;
    defineScope();
    load();

    function defineScope() {

        $scope.processInputArguments = [];

        $scope.periods = [];
        $scope.selectedPeriod = "";



        $scope.hours = [];
        angular.forEach(HourEnum, function (itm) {
            $scope.hours.push({ id: itm.id, name: itm.name })
        });

        $scope.selectedPeakHours = [];


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
                Parameters: { GapBetweenConsecutiveCalls: $scope.gapBetweenConsecutiveCalls, GapBetweenFailedConsecutiveCalls: $scope.gapBetweenFailedConsecutiveCalls, MaxLowDurationCall: $scope.maxLowDurationCall, MinimumCountofCallsinActiveHour: $scope.minCountofCallsinActiveHour, PeakHoursIds: $scope.PeakHoursIds }
            };
        };

    };

    function load() {


        $scope.periods.length = 0;
        $scope.periods = [];
        StrategyAPIService.GetPeriods().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.periods.push(itm);
            });

            if ($scope.schedulerTaskAction.processInputArguments.data == undefined)
                return;

            var data = $scope.schedulerTaskAction.processInputArguments.data;

            if (data != null) {
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
            $scope.isGettingData = false;


        }).catch(function (error) {
            $scope.isGettingData = false;
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });





    }

    function loadPeriods() {
        return StrategyAPIService.GetPeriods().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.periods.push(itm);
            });
        });
    }
}

NumberProfilingProcessInput_Scheduled.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'HourEnum'];
appControllers.controller('FraudAnalysis_NumberProfilingProcessInput_Scheduled', NumberProfilingProcessInput_Scheduled)



