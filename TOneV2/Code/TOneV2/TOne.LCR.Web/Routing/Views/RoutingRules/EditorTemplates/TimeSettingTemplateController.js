var TimeSettingController = function ($scope, UtilsService, DaysOfWeekEnum) {

    defineScope();

    function defineScope() {
        $scope.FromTime = "";
        $scope.ToTime = "";

        $scope.daysOfWeek = [];
        $scope.selectedDaysOfWeek = [];


        $scope.subViewConnector.getTimeSettings = function () {
            return {
                Days: $scope.getSelectedDays(),
                FromTime: $scope.FromTime,
                ToTime: $scope.ToTime,
                BeginEffectiveDate: $scope.BED,
                EndEffectiveDate: $scope.EED
            }
        }
        $scope.getSelectedDays = function () {
            var selectedDays = [];
            $.each($scope.selectedDaysOfWeek, function (i, value) {
                selectedDays.push(i);
            });
            return selectedDays;
        }

        $scope.subViewConnector.load = function () {
            loadDaysOfWeek();
            if ($scope.routeRule != null && $scope.routeRule != undefined && $scope.routeRule.TimeExecutionSetting != null) {
                //TODO Fill Time Settings
                $scope.BED = $scope.routeRule.TimeExecutionSetting.BeginEffectiveDate;
                $scope.EED = $scope.routeRule.TimeExecutionSetting.EndEffectiveDate;
                $scope.FromTime = $scope.routeRule.TimeExecutionSetting.FromTime;
                $scope.ToTime = $scope.routeRule.TimeExecutionSetting.ToTime;
                loadSelectedDays($scope.routeRule.TimeExecutionSetting.Days);
            }

        }

    }


    function loadDaysOfWeek() {
        for (var prop in DaysOfWeekEnum) {
            $scope.daysOfWeek.push(DaysOfWeekEnum[prop]);
        }
    }

    function loadSelectedDays(days) {
        if (days != undefined)
            $.each(days, function (i, value) {
                var existobj = UtilsService.getItemByVal($scope.daysOfWeek, value, 'value');
                if (existobj != null)
                    $scope.selectedDaysOfWeek.push(existobj);
            });
    }
}

TimeSettingController.$inject = ['$scope', 'UtilsService', 'DaysOfWeekEnum'];
appControllers.controller('RoutingRules_TimeSettingTemplateController', TimeSettingController)