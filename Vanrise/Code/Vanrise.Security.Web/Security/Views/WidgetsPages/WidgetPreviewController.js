WidgetPreviewController.$inject = ['$scope', 'TimeDimensionTypeEnum','PeriodEnum','UtilsService'];

function WidgetPreviewController($scope, TimeDimensionTypeEnum, PeriodEnum, UtilsService) {
    var widgetAPI;
    defineScope();
    load();
    var date;
    function defineScope() {
       
        $scope.selectedPeriod = PeriodEnum.CurrentMonth;
        date = $scope.selectedPeriod.getInterval();
        $scope.fromDate = date.from;
        $scope.toDate = date.to;
        
        defineTimeDimensionTypes();
        $scope.filter = {
            timeDimensionType: $scope.selectedTimeDimensionType,
            fromDate: $scope.fromDate,
            toDate: $scope.toDate
        }
        $scope.periods = UtilsService.getArrayEnum(PeriodEnum);
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

                date = $scope.selectedPeriod.getInterval();
                console.log(date);
                $scope.fromDate = date.from;
                $scope.toDate = date.to;
            }
     
        }
        $scope.customvalidateFrom = function (fromDate) {
            var from = UtilsService.getShortDate(fromDate);
            var to = UtilsService.getShortDate($scope.toDate);
            return UtilsService.validateDates(from, to);
        };
        $scope.customvalidateTo = function (toDate) {
            var from = UtilsService.getShortDate($scope.fromDate);
            var to = UtilsService.getShortDate(toDate);
            return UtilsService.validateDates(from, to);
        };
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
    function load() {
      
        $scope.isGettingData = false;
        
    }

   

}
appControllers.controller('Security_WidgetPreviewController', WidgetPreviewController);
