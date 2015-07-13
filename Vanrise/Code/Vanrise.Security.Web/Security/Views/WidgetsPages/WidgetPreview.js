WidgetPreviewController.$inject = ['$scope', 'TimeDimensionTypeEnum','PeriodEnum'];

function WidgetPreviewController($scope, TimeDimensionTypeEnum, PeriodEnum) {
    var widgetAPI;
    defineScope();
    load();

    function defineScope() {
        definePeriods();
        var date = getPeriod($scope.selectedPeriod);
       $scope.fromDate = date.from;
       $scope.toDate = date.to;
        defineTimeDimensionTypes();
        $scope.filter = {
            timeDimensionType: $scope.selectedTimeDimensionType,
            fromDate: $scope.fromDate,
            toDate: $scope.toDate
        }
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.widget = $scope.$parent.widget;
        $scope.widget.SectionTitle = $scope.widget.Name;
        $scope.onElementReady = function (api) {
            widgetAPI = api;
            widgetAPI.retrieveData($scope.filter);
        };
       
        $scope.Search = function () {
            $scope.filter = {
                timeDimensionType: $scope.selectedTimeDimensionType,
                fromDate: $scope.fromDate,
                toDate: $scope.toDate
            }
                updateDashboard();
        };
        $scope.periodSelectionChanged = function () {
            var date = getPeriod($scope.selectedPeriod);
            $scope.fromDate = date.from;
            $scope.toDate = date.to;
        }
    }

    function getPeriod(periodType) {
        console.log(periodType);
        switch (periodType.value) {
            case PeriodEnum.LastYear.value: return getLastYearInterval();
            case PeriodEnum.LastMonth.value: return getLastMonthInterval();
            case PeriodEnum.LastWeek.value: return getLastWeekInterval();
            case PeriodEnum.Yesterday.value: return getYesterdayInterval();
            case PeriodEnum.Today.value: return getTodayInterval();
            case PeriodEnum.CurrentWeek.value: return getCurrentWeekInterval();
            case PeriodEnum.CurrentMonth.value: return getCurrentMonthInterval();
            case PeriodEnum.CurrentYear.value: return getCurrentYearInterval();
        }
    }
    function getCurrentYearInterval() {
        var date = new Date();
        var interval = {
            from: new Date(date.getFullYear(),0, 1),
            to: new Date(),
        }
        return interval;
    }
    function getCurrentWeekInterval() {
        var thisWeek = new Date(new Date().getTime() - 60 * 60 * 24 * 1000)
        var day = thisWeek.getDay();
        console.log(day);
        var LastMonday;
        if (day === 0) {
            LastMonday = new Date();
        }
        else {
            var diffToMonday = thisWeek.getDate() - day + (day === 0 ? -6 : 1);
            var LastMonday = new Date(thisWeek.setDate(diffToMonday));
        }
        
      
        var interval = {
            from: LastMonday,
            to: new Date(),
        }
        return interval;
    }
    function getLastWeekInterval() {
        var beforeOneWeek = new Date(new Date().getTime() - 60 * 60 * 24 * 7 * 1000)
        var day = beforeOneWeek.getDay();

        var diffToMonday = beforeOneWeek.getDate() - day + (day === 0 ? -6 : 1);
        var beforeLastMonday = new Date(beforeOneWeek.setDate(diffToMonday));
        var lastSunday = new Date(beforeOneWeek.setDate(diffToMonday+6));
        var interval = {
            from: beforeLastMonday,
            to: lastSunday,
        }
        return interval;
    }
    function getCurrentMonthInterval() {
        var date = new Date();
        var interval = {
            from: new Date(date.getFullYear(), date.getMonth(), 1),
            to: new Date(),
        }
        return interval;
    }
    function getTodayInterval() {
        var date = new Date();
        var interval = {
            from: date,
            to: date
        }
        return interval;
    }
    function getYesterdayInterval() {
        var date = new Date();
        date.setDate(date.getDate() - 1);
        var interval = {
            from: date,
            to: date,
        }
        return interval;
    }
    function getLastMonthInterval() {
        var date = new Date();
        var interval = {
            from: new Date(date.getFullYear(), date.getMonth()-1, 1),
            to: new Date(date.getFullYear(), date.getMonth(), 0),
        }
        return interval;
    }
    function getLastYearInterval() {
        var date = new Date();
        var interval = {
            from: new Date(date.getFullYear()-1, 0, 1),
            to: new Date(date.getFullYear()-1, 11, 31)
        }
        return interval;
    }
    function defineTimeDimensionTypes() {
        $scope.timeDimensionTypes = [];
        for (var td in TimeDimensionTypeEnum)
            $scope.timeDimensionTypes.push(TimeDimensionTypeEnum[td]);

        $scope.selectedTimeDimensionType = $.grep($scope.timeDimensionTypes, function (t) {
            return t == TimeDimensionTypeEnum.Daily;
        })[0];
    }
    function definePeriods() {
        $scope.periods = [];
        for (var p in PeriodEnum)
            $scope.periods.push(PeriodEnum[p]);
        $scope.selectedPeriod = $scope.periods[0];
       // console.log($scope.selectedPeriod);
    }
    
    function load() {
      
        $scope.isGettingData = false;
        
    }

   

}
appControllers.controller('Security_WidgetPreviewController', WidgetPreviewController);
