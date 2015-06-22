var TimeSettingController = function ($scope, UtilsService, DaysOfWeekEnum) {

    defineScope();
    load();
    function defineScope() {

        $scope.FromTime = "";
        $scope.ToTime = "";

        $scope.daysOfWeek = [];
        $scope.selectedDaysOfWeek = [];

        $scope.subViewTimeSettingConnector.getData = function () {
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

        $scope.subViewTimeSettingConnector.setData = function (data) {
            $scope.subViewTimeSettingConnector.data = data;
            loadForm();
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

    function load() {
        loadDaysOfWeek();
        loadForm();
    }

    function loadForm() {
        var data = $scope.subViewTimeSettingConnector.data;
        if (data != null) {
            //TODO Fill Time Settings
            $scope.BEDDate = data.BeginEffectiveDate;
            $scope.EEDDate = data.EndEffectiveDate;
            $scope.FromTime = data.FromTime;
            $scope.ToTime = data.ToTime;
            loadSelectedDays(data.Days);
        }
    }
}

TimeSettingController.$inject = ['$scope', 'UtilsService', 'DaysOfWeekEnum'];
appControllers.controller('RoutingRules_TimeSettingTemplateController', TimeSettingController)