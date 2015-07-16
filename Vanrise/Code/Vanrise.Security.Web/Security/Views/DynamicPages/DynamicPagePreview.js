DynamicPagePreviewController.$inject = ['$scope', 'ViewAPIService', 'WidgetAPIService', 'BITimeDimensionTypeEnum', 'UtilsService', 'VRNotificationService', 'VRNavigationService','WidgetSectionEnum','BIPeriodEnum'];

function DynamicPagePreviewController($scope, ViewAPIService, WidgetAPIService, BITimeDimensionTypeEnum, UtilsService, VRNotificationService, VRNavigationService, WidgetSectionEnum, BIPeriodEnum) {
    var viewId;
    loadParameters();
    defineScope();
    load();
    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters!=null && parameters.viewId != undefined) {
            viewId = parameters.viewId;
        }
    }

    function defineScope() {
        definePeriods();
        var date;
        $scope.fromDate;
        $scope.toDate;
        $scope.allWidgets = [];
        $scope.viewContent = [];
        $scope.selectedPeriod;
        $scope.summaryContents = [];
        $scope.bodyContents = [];
        $scope.summaryWidgets = [];
        $scope.bodyWidgets = [];
        $scope.viewWidgets = [];
        $scope.onValueChanged = function () {
            var customize = {
                value: -1,
                description: "Customize"
            }
            $scope.selectedPeriod = customize;
        }
        $scope.periodSelectionChanged = function () {
            var date = getPeriod($scope.selectedPeriod.value);
            $scope.fromDate = date.from;
            $scope.toDate = date.to;
        }
        defineTimeDimensionTypes();
      
        
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.Search = function () {
            $scope.filter = {
                timeDimensionType: $scope.selectedTimeDimensionType,
                fromDate: $scope.fromDate,
                toDate: $scope.toDate
            }
            if (($scope.bodyWidgets != null && $scope.bodyWidgets != undefined) || ($scope.summaryWidgets != null && $scope.summaryWidgets != undefined)) {
               return refreshData();
            }
            else {
                return loadWidgets();
            }    
        };
        $scope.customvalidateFrom = function (fromDate) {
            return validateDates(fromDate, $scope.toDate);
        };
        $scope.customvalidateTo = function (toDate) {
            return validateDates($scope.fromDate, toDate);
        };
        

    }
    
    function validateDates(fromDate, toDate) {
        if (fromDate == undefined || toDate == undefined)
            return null;
        var from = new Date(fromDate);
        var to = new Date(toDate);
        if (from.getTime() > to.getTime())
            return "Start should be before end";
        else
            return null;
    }
    function defineTimeDimensionTypes() {
        $scope.timeDimensionTypes = [];
        for (var td in BITimeDimensionTypeEnum)
            $scope.timeDimensionTypes.push(BITimeDimensionTypeEnum[td]);

        $scope.selectedTimeDimensionType = $.grep($scope.timeDimensionTypes, function (t) {
            return t == BITimeDimensionTypeEnum.Daily;
        })[0];
    }

    function load() {
       
     

        loadWidgets();
        $scope.isGettingData = false;

    }
    function fillDateAndPeriod(){
        date = getPeriod($scope.selectedPeriod.value);
        $scope.fromDate = date.from;
        $scope.toDate = date.to;
        $scope.filter = {
            timeDimensionType: $scope.selectedTimeDimensionType,
            fromDate: $scope.fromDate,
            toDate: $scope.toDate
        }
    }
    function refreshData() {
        var refreshDataOperations = [];
        angular.forEach($scope.bodyWidgets, function (bodyWidget) {

            refreshDataOperations.push(bodyWidget.retrieveData);
        });
        angular.forEach($scope.summaryWidgets, function (summaryWidget) {
            refreshDataOperations.push(summaryWidget.retrieveData);
       });
        return UtilsService.waitMultipleAsyncOperations(refreshDataOperations)
                  .finally(function () {
                  });
    }

    function loadWidgets() {

        if (viewId != undefined) {
            $scope.isGettingData = true;
          return  UtilsService.waitMultipleAsyncOperations([loadAllWidgets, loadViewByID])
                 .finally(function () {
                     fillDateAndPeriod();
                  
                     loadViewWidgets($scope.allWidgets, $scope.bodyContents, $scope.summaryContents);
                     $scope.isGettingData = false;
                 });
        }
        else {
            $scope.isGettingData = true;
            $scope.selectedPeriod = $scope.$parent.selectedPeriod;
            
            $scope.selectedTimeDimensionType = $scope.$parent.selectedTimeDimensionType;
          return  UtilsService.waitMultipleAsyncOperations([loadAllWidgets])
                .finally(function () {
                    fillDateAndPeriod();
                    loadViewWidgets($scope.allWidgets, $scope.$parent.bodyContents, $scope.$parent.summaryContents);
                    $scope.isGettingData = false;
                });

        }   
    }

    function loadViewWidgets(allWidgets, BodyContents, SummaryContents) {
        for (var i = 0; i < BodyContents.length; i++) {
            var bodyContent = BodyContents[i];
            var value = UtilsService.getItemByVal(allWidgets, bodyContent.WidgetId, 'Id');
            if (value != null)
            {
                value.NumberOfColumns = bodyContent.NumberOfColumns;
                value.SectionTitle = bodyContent.SectionTitle;
                addBodyWidget(value);
            }
            

        }
     for (var i = 0; i < SummaryContents.length; i++) {
            var summaryContent = SummaryContents[i];
            var value = UtilsService.getItemByVal(allWidgets, summaryContent.WidgetId, 'Id');
            if (value != null) {
                value.NumberOfColumns = summaryContent.NumberOfColumns;
                value.SectionTitle = summaryContent.SectionTitle;
                addSummaryWidget(value);
            }


     }
    
    }

    function addBodyWidget(bodyWidget) {
        bodyWidget.onElementReady = function (api) {
            bodyWidget.API = api;
            bodyWidget.API.retrieveData($scope.filter);
            bodyWidget.retrieveData = function () {
                return api.retrieveData($scope.filter);
            };
        };
        $scope.bodyWidgets.push(bodyWidget);
    }
    function addSummaryWidget(summaryWidget) {
        summaryWidget.onElementReady = function (api) {
            summaryWidget.API = api;
            summaryWidget.API.retrieveData($scope.filter);
            summaryWidget.retrieveData = function () {
                return api.retrieveData($scope.filter);
            };
        };
        $scope.summaryWidgets.push(summaryWidget);
    }

    function loadAllWidgets() {
        return WidgetAPIService.GetAllWidgets().then(function (response) {
            $scope.allWidgets = response;
        });

    }

    function loadViewByID() {
        return ViewAPIService.GetView(viewId).then(function (response) {
            $scope.summaryContents = response.ViewContent.SummaryContents;
            $scope.bodyContents = response.ViewContent.BodyContents;
            $scope.selectedPeriod = UtilsService.getItemByVal($scope.periods, response.ViewContent.DefaultPeriod, 'value');
            console.log($scope.selectedPeriod)
            $scope.selectedTimeDimensionType = UtilsService.getItemByVal($scope.timeDimensionTypes, response.ViewContent.DefaultGrouping, 'value');
        });
    }
    function getPeriod(periodType) {
        switch (periodType) {
            case BIPeriodEnum.LastYear.value: return getLastYearInterval();
            case BIPeriodEnum.LastMonth.value: return getLastMonthInterval();
            case BIPeriodEnum.LastWeek.value: return getLastWeekInterval();
            case BIPeriodEnum.Yesterday.value: return getYesterdayInterval();
            case BIPeriodEnum.Today.value: return getTodayInterval();
            case BIPeriodEnum.CurrentWeek.value: return getCurrentWeekInterval();
            case BIPeriodEnum.CurrentMonth.value: return getCurrentMonthInterval();
            case BIPeriodEnum.CurrentYear.value: return getCurrentYearInterval();
        }
    }
    function getCurrentYearInterval() {
        var date = new Date();
        var interval = {
            from: new Date(date.getFullYear(), 0, 1),
            to: new Date(),
        }
        return interval;
    }
    function getCurrentWeekInterval() {
        var thisWeek = new Date(new Date().getTime() - 60 * 60 * 24 * 1000)
        var day = thisWeek.getDay();
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
        var lastSunday = new Date(beforeOneWeek.setDate(diffToMonday + 6));
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
            from: new Date(date.getFullYear(), date.getMonth() - 1, 1),
            to: new Date(date.getFullYear(), date.getMonth(), 0),
        }
        return interval;
    }
    function getLastYearInterval() {
        var date = new Date();
        var interval = {
            from: new Date(date.getFullYear() - 1, 0, 1),
            to: new Date(date.getFullYear() - 1, 11, 31)
        }
        return interval;
    }
    function definePeriods() {
        $scope.periods = [];
        for (var p in BIPeriodEnum)
            $scope.periods.push(BIPeriodEnum[p]);
      //  $scope.selectedPeriod = $scope.periods[0];
    }
}
appControllers.controller('Security_DynamicPagePreviewController', DynamicPagePreviewController);
