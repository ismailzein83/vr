DynamicPagePreviewController.$inject = ['$scope', 'ViewAPIService', 'WidgetAPIService', 'TimeDimensionTypeEnum', 'UtilsService', 'VRNotificationService', 'VRNavigationService','WidgetSectionEnum','PeriodEnum','VRValidationService'];

function DynamicPagePreviewController($scope, ViewAPIService, WidgetAPIService, TimeDimensionTypeEnum, UtilsService, VRNotificationService, VRNavigationService, WidgetSectionEnum, PeriodEnum, VRValidationService) {
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
        $scope.scopeModal = {};
        var date;
        $scope.scopeModal.fromDate;
        $scope.scopeModal.validateDateTime = function () {
            return VRValidationService.validateTimeRange($scope.scopeModal.fromDate, $scope.scopeModal.toDate);
        }
        $scope.scopeModal.nonSearchable = true;
        $scope.scopeModal.toDate;
        $scope.scopeModal.allWidgets = [];
        $scope.scopeModal.viewContent = [];
        $scope.scopeModal.periods = UtilsService.getArrayEnum(PeriodEnum);
        $scope.scopeModal.selectedPeriod;
        $scope.scopeModal.summaryContents = [];
        $scope.scopeModal.bodyContents = [];
        $scope.scopeModal.summaryWidgets = [];
        $scope.scopeModal.bodyWidgets = [];
        $scope.scopeModal.viewWidgets = [];
        var customize = {
            value: -1,
            description: "Customize"
        }
        $scope.scopeModal.onBlurChanged = function () {
            var from = UtilsService.getShortDate($scope.scopeModal.fromDate);
            var oldFrom = UtilsService.getShortDate(date.from);
            var to = UtilsService.getShortDate($scope.scopeModal.toDate);
            var oldTo = UtilsService.getShortDate(date.to);
            if (from != oldFrom || to != oldTo)
                $scope.scopeModal.selectedPeriod = customize;

        }
        $scope.scopeModal.periodSelectionChanged = function () {
            if ($scope.scopeModal.selectedPeriod != undefined && $scope.scopeModal.selectedPeriod.value != -1) {
                date = $scope.scopeModal.selectedPeriod.getInterval();
                $scope.scopeModal.fromDate = date.from;
                $scope.scopeModal.toDate = date.to;
            }
           
        }
        defineTimeDimensionTypes();
      
        
        $scope.scopeModal.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.scopeModal.Search = function () {
            $scope.scopeModal.filter = {
                timeDimensionType: $scope.scopeModal.selectedTimeDimensionType,
                fromDate: $scope.scopeModal.fromDate,
                toDate: $scope.scopeModal.toDate
            }
            if (($scope.scopeModal.bodyWidgets != null && $scope.scopeModal.bodyWidgets != undefined) || ($scope.scopeModal.summaryWidgets != null && $scope.scopeModal.summaryWidgets != undefined)) {
               return refreshData();
            }
            else {
                return loadWidgets();
            }    
        };
       

    }
    
    function defineTimeDimensionTypes() {
        $scope.scopeModal.timeDimensionTypes = [];
        for (var td in TimeDimensionTypeEnum)
            $scope.scopeModal.timeDimensionTypes.push(TimeDimensionTypeEnum[td]);

        $scope.scopeModal.selectedTimeDimensionType = TimeDimensionTypeEnum.Daily;
    }

    function load() {
        loadWidgets();
        $scope.scopeModal.isGettingData = false;

    }
    function fillDateAndPeriod(){
        date = $scope.scopeModal.selectedPeriod.getInterval();
        $scope.scopeModal.fromDate = date.from;
        $scope.scopeModal.toDate = date.to;
        $scope.scopeModal.filter = {
            timeDimensionType: $scope.scopeModal.selectedTimeDimensionType,
            fromDate: $scope.scopeModal.fromDate,
            toDate: $scope.scopeModal.toDate
        }
    }
    function refreshData() {
        var refreshDataOperations = [];
        angular.forEach($scope.scopeModal.bodyWidgets, function (bodyWidget) {
           if(bodyWidget.API!=undefined)
            refreshDataOperations.push(bodyWidget.retrieveData);
        });
        angular.forEach($scope.scopeModal.summaryWidgets, function (summaryWidget) {
            if (summaryWidget.API != undefined)
            refreshDataOperations.push(summaryWidget.retrieveData);
        });
      
        return UtilsService.waitMultipleAsyncOperations(refreshDataOperations)
                  .finally(function () {
                  });
    }

    function loadWidgets() {

        if (viewId != undefined) {
            $scope.scopeModal.isGettingData = true;
          return  UtilsService.waitMultipleAsyncOperations([loadAllWidgets, loadViewByID])
                 .finally(function () {
                     loadViewWidgets($scope.scopeModal.allWidgets, $scope.scopeModal.bodyContents, $scope.scopeModal.summaryContents);
                     $scope.scopeModal.isGettingData = false;
                 });
        }
        else {
            $scope.scopeModal.isGettingData = true;
            if (!$scope.$parent.nonSearchable) {
                $scope.scopeModal.selectedPeriod = $scope.$parent.selectedViewPeriod;
                $scope.scopeModal.selectedTimeDimensionType = $scope.$parent.selectedViewTimeDimensionType;
                fillDateAndPeriod();
                $scope.scopeModal.nonSearchable = false;
            }

          return  UtilsService.waitMultipleAsyncOperations([loadAllWidgets])
                .finally(function () {
                    loadViewWidgets($scope.scopeModal.allWidgets, $scope.$parent.bodyContents, $scope.$parent.summaryContents);
                    $scope.scopeModal.isGettingData = false;
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
                if ($scope.scopeModal.nonSearchable) {
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
                if ($scope.scopeModal.nonSearchable) {
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
            if (!$scope.scopeModal.nonSearchable)
                bodyWidget.API.retrieveData($scope.scopeModal.filter);
            else {
                var widgetDate = UtilsService.getPeriod(bodyWidget.DefaultPeriod);
                var timeDimention = UtilsService.getItemByVal($scope.scopeModal.timeDimensionTypes, bodyWidget.DefaultGrouping, 'value');
                filter = {
                    timeDimensionType: timeDimention,
                    fromDate: widgetDate.from,
                    toDate: widgetDate.to
                }
                bodyWidget.API.retrieveData(filter);
            }
           
            bodyWidget.retrieveData = function () {
                if (!$scope.scopeModal.nonSearchable)
                    return api.retrieveData($scope.scopeModal.filter);
                else
                   return api.retrieveData(filter);
            };
        };
        $scope.scopeModal.bodyWidgets.push(bodyWidget);
    }
    function addSummaryWidget(summaryWidget) {
        summaryWidget.onElementReady = function (api) {
           
            summaryWidget.API = api;
            var filter = {};
            if (!$scope.scopeModal.nonSearchable)
                summaryWidget.API.retrieveData($scope.scopeModal.filter);
            else {
                var widgetDate = UtilsService.getPeriod(summaryWidget.DefaultPeriod);
                var timeDimention = UtilsService.getItemByVal($scope.scopeModal.timeDimensionTypes, summaryWidget.DefaultGrouping, 'value');
                filter = {
                    timeDimensionType: timeDimention,
                    fromDate: widgetDate.from,
                    toDate: widgetDate.to
                }
                summaryWidget.API.retrieveData(filter);
            }
          
            summaryWidget.retrieveData = function () {
                if (!$scope.scopeModal.nonSearchable)
                    return api.retrieveData($scope.scopeModal.filter);
                else
                    return api.retrieveData(filter);

            };
        };
       
        $scope.scopeModal.summaryWidgets.push(summaryWidget);
    }

    function loadAllWidgets() {
        return WidgetAPIService.GetAllWidgets().then(function (response) {
            $scope.scopeModal.allWidgets = response;
        });

    }
    function loadViewByID() {
        return ViewAPIService.GetView(viewId).then(function (response) {
            $scope.scopeModal.summaryContents = response.ViewContent.SummaryContents;
            $scope.scopeModal.bodyContents = response.ViewContent.BodyContents;
            if (response.ViewContent.DefaultPeriod != undefined || response.ViewContent.DefaultGrouping != undefined) {
                $scope.scopeModal.selectedPeriod = UtilsService.getItemByVal($scope.scopeModal.periods, response.ViewContent.DefaultPeriod, 'value');
                $scope.scopeModal.selectedTimeDimensionType = UtilsService.getItemByVal($scope.scopeModal.timeDimensionTypes, response.ViewContent.DefaultGrouping, 'value');
                $scope.scopeModal.nonSearchable = false;
                fillDateAndPeriod();
            }
 
        });
    }
    

}
appControllers.controller('Security_DynamicPagePreviewController', DynamicPagePreviewController);
