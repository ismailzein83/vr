﻿DynamicPagePreviewController.$inject = ['$scope', 'ViewAPIService', 'WidgetAPIService', 'TimeDimensionTypeEnum', 'UtilsService', 'VRNotificationService', 'VRNavigationService','WidgetSectionEnum','PeriodEnum'];

function DynamicPagePreviewController($scope, ViewAPIService, WidgetAPIService, TimeDimensionTypeEnum, UtilsService, VRNotificationService, VRNavigationService, WidgetSectionEnum, PeriodEnum) {
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
        $scope.nonSearchable = true;
        $scope.toDate;
        $scope.allWidgets = [];
        $scope.viewContent = [];
        $scope.selectedPeriod;
        $scope.summaryContents = [];
        $scope.bodyContents = [];
        $scope.summaryWidgets = [];
        $scope.bodyWidgets = [];
        $scope.viewWidgets = [];
        var customize = {
            value: -1,
            description: "Customize"
        }
        $scope.onBlurChanged = function () {
            var from = formatMMDDYYYY($scope.fromDate);
            var oldFrom = formatMMDDYYYY(date.from);
            var to = formatMMDDYYYY($scope.toDate);
            var oldTo = formatMMDDYYYY(date.to);
            if (from != oldFrom || to != oldTo)
                $scope.selectedPeriod = customize;

        }
        $scope.periodSelectionChanged = function () {
            if ($scope.selectedPeriod != undefined && $scope.selectedPeriod.value != -1) {
                date = UtilsService.getPeriod($scope.selectedPeriod.value);
                $scope.fromDate = date.from;
                $scope.toDate = date.to;
            }
           
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
        for (var td in TimeDimensionTypeEnum)
            $scope.timeDimensionTypes.push(TimeDimensionTypeEnum[td]);

        $scope.selectedTimeDimensionType= TimeDimensionTypeEnum.Daily;
    }

    function load() {
        loadWidgets();
        $scope.isGettingData = false;

    }
    function fillDateAndPeriod(){
        date = UtilsService.getPeriod($scope.selectedPeriod.value);
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
           if(bodyWidget.API!=undefined)
            refreshDataOperations.push(bodyWidget.retrieveData);
        });
        angular.forEach($scope.summaryWidgets, function (summaryWidget) {
            if (summaryWidget.API != undefined)
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
                     loadViewWidgets($scope.allWidgets, $scope.bodyContents, $scope.summaryContents);
                     $scope.isGettingData = false;
                 });
        }
        else {
            $scope.isGettingData = true;
            if (!$scope.$parent.nonSearchable) {
                $scope.selectedPeriod = $scope.$parent.selectedViewPeriod;
                $scope.selectedTimeDimensionType = $scope.$parent.selectedViewTimeDimensionType;
                fillDateAndPeriod();
                $scope.nonSearchable = false;
            }

          return  UtilsService.waitMultipleAsyncOperations([loadAllWidgets])
                .finally(function () {
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
                if ($scope.nonSearchable) {
                    value.DefaultPeriod = bodyContent.DefaultPeriod;
                    value.DefaultGrouping = bodyContent.DefaultGrouping;
                }
                addBodyWidget(value);
            }
            

        }
     for (var i = 0; i < SummaryContents.length; i++) {
            var summaryContent = SummaryContents[i];
            var value = UtilsService.getItemByVal(allWidgets, summaryContent.WidgetId, 'Id');
            if (value != null) {
                value.NumberOfColumns = summaryContent.NumberOfColumns;
                value.SectionTitle = summaryContent.SectionTitle;
                if ($scope.nonSearchable) {
                    value.DefaultPeriod = summaryContent.DefaultPeriod;
                    value.DefaultGrouping = summaryContent.DefaultGrouping;
                }
                
                addSummaryWidget(value);
            }

     }
    
    }

    function addBodyWidget(bodyWidget) {
        bodyWidget.onElementReady = function (api) {
            bodyWidget.API = api;
            var filter = {};
            if (!$scope.nonSearchable)
               bodyWidget.API.retrieveData( $scope.filter);
            else {
                var widgetDate = UtilsService.getPeriod(bodyWidget.DefaultPeriod);
                var timeDimention = UtilsService.getItemByVal($scope.timeDimensionTypes, bodyWidget.DefaultGrouping, 'value');
                filter = {
                    timeDimensionType: timeDimention,
                    fromDate: widgetDate.from,
                    toDate: widgetDate.to
                }
                bodyWidget.API.retrieveData(filter);
            }
           
            bodyWidget.retrieveData = function () {
                if (!$scope.nonSearchable)
                    return api.retrieveData($scope.filter);
                else
                   return api.retrieveData(filter);
            };
        };
        $scope.bodyWidgets.push(bodyWidget);
    }
    function addSummaryWidget(summaryWidget) {
        summaryWidget.onElementReady = function (api) {
           
            summaryWidget.API = api;
            var filter = {};
            if (!$scope.nonSearchable)
                summaryWidget.API.retrieveData( $scope.filter);
            else {
                var widgetDate = UtilsService.getPeriod(summaryWidget.DefaultPeriod);
                var timeDimention = UtilsService.getItemByVal($scope.timeDimensionTypes, summaryWidget.DefaultGrouping, 'value');
                filter = {
                    timeDimensionType: timeDimention,
                    fromDate: widgetDate.from,
                    toDate: widgetDate.to
                }
                summaryWidget.API.retrieveData(filter);
            }
          
            summaryWidget.retrieveData = function () {
                if (!$scope.nonSearchable)
                    return api.retrieveData($scope.filter);
                else
                    return api.retrieveData(filter);

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
            if (response.ViewContent.DefaultPeriod != undefined || response.ViewContent.DefaultGrouping != undefined) {
                $scope.selectedPeriod = UtilsService.getItemByVal($scope.periods, response.ViewContent.DefaultPeriod, 'value');
                $scope.selectedTimeDimensionType = UtilsService.getItemByVal($scope.timeDimensionTypes, response.ViewContent.DefaultGrouping, 'value');
                $scope.nonSearchable = false;
                fillDateAndPeriod();
            }
 
        });
    }
    
    function definePeriods() {
        $scope.periods = [];
        for (var p in PeriodEnum)
            $scope.periods.push(PeriodEnum[p]);
      // $scope.selectedPeriod = $scope.periods[0];
    }
}
appControllers.controller('Security_DynamicPagePreviewController', DynamicPagePreviewController);
