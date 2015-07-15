testpage1.$inject = ['$scope', 'TimeDimensionTypeEnum', 'PeriodEnum'];

function testpage1($scope, TimeDimensionTypeEnum, PeriodEnum) {
    var widgetAPI;
    defineScope();
    load();

    function defineScope() {
        definePeriods();
        var date = getPeriod($scope.selectedPeriod);
        $scope.fromDate = date.from;
        $scope.toDate = date.to;

    }

    }
    function load() {

        $scope.isGettingData = false;

    }



}
appControllers.controller('testpage1', testpage1);
