WidgetPreviewController.$inject = ['$scope', 'TimeDimensionTypeEnum','PeriodEnum','UtilsService'];

function WidgetPreviewController($scope, TimeDimensionTypeEnum, PeriodEnum, UtilsService) {
    var widgetAPI;
    defineScope();
    load();
    var date;
    function defineScope() {
        definePeriods();
        date = UtilsService.getPeriod($scope.selectedPeriod.value);
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
        var customize = {
            value: -1,
            description: "Customize"
        }
        $scope.onBlurChanged = function () {
            var from = UtilsService.getShortDate($scope.fromDate);
            var oldFrom = UtilsService.getShortDate(date.from);
            var to = UtilsService.getShortDate($scope.toDate);
            var oldTo = UtilsService.getShortDate(date.to);
            if (from != oldFrom || to != oldTo)
                $scope.selectedPeriod = customize;

        }
   
        
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
               return refreshWidget();
        };
        $scope.periodSelectionChanged = function () {

            if ($scope.selectedPeriod.value != -1) {

                date = UtilsService.getPeriod($scope.selectedPeriod.value);
                console.log(date);
                $scope.fromDate = date.from;
                $scope.toDate = date.to;
            }
     
        }
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
    function refreshWidget() {
      return  widgetAPI.retrieveData($scope.filter);
    }

    function defineTimeDimensionTypes() {
        $scope.timeDimensionTypes = [];
        for (var td in TimeDimensionTypeEnum)
            $scope.timeDimensionTypes.push(TimeDimensionTypeEnum[td]);
        $scope.selectedTimeDimensionType =  TimeDimensionTypeEnum.Daily;
     
    }
    function definePeriods() {
        $scope.periods = [];
        for (var p in PeriodEnum)
            $scope.periods.push(PeriodEnum[p]);
        $scope.selectedPeriod = PeriodEnum.CurrentMonth;
    }
    function load() {
      
        $scope.isGettingData = false;
        
    }

   

}
appControllers.controller('Security_WidgetPreviewController', WidgetPreviewController);
