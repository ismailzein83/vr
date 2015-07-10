DynamicPagePreviewController.$inject = ['$scope', 'ViewAPIService', 'WidgetAPIService', 'BITimeDimensionTypeEnum', 'UtilsService', 'VRNotificationService', 'VRNavigationService','WidgetSectionEnum'];

function DynamicPagePreviewController($scope, ViewAPIService, WidgetAPIService, BITimeDimensionTypeEnum, UtilsService, VRNotificationService, VRNavigationService, WidgetSectionEnum) {
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
        $scope.allWidgets = [];
        $scope.viewContent = [];

        $scope.summaryContents = [];
        $scope.bodyContents = [];
        $scope.summaryWidgets = [];
        $scope.bodyWidgets = [];
        $scope.viewWidgets = [];
        $scope.fromDate = "2015-04-01";
        $scope.toDate = "2015-04-30";
        defineTimeDimensionTypes();
        $scope.filter = {
            timeDimensionType: $scope.selectedTimeDimensionType,
            fromDate:$scope.fromDate,
            toDate:$scope.toDate 
        }
        
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.Search = function () {
            if (($scope.bodyWidgets != null && $scope.bodyWidgets != undefined) || ($scope.summaryWidgets != null && $scope.summaryWidgets != undefined)) {
                updateDashboard();
            }
            else {
                getData();
            }    
        };
        getData();
        
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
        $scope.isGettingData = false;

    }
    
    function updateDashboard() {
        $scope.filter = {
            timeDimensionType: $scope.selectedTimeDimensionType,
            fromDate: $scope.fromDate,
            toDate: $scope.toDate
        }
        var refreshDataOperations = [];
        angular.forEach($scope.bodyWidgets, function (bodyWidget) {
            refreshDataOperations.push(bodyWidget.API.retrieveData);
        });
        angular.forEach($scope.summaryWidgets, function (summaryWidget) {
            refreshDataOperations.push(summaryWidget.API.retrieveData);
        });
        $scope.isGettingData = true;
        return UtilsService.waitMultipleAsyncOperations(refreshDataOperations)
            .finally(function () {
                $scope.isGettingData = false;
            });
        
    }

    function getData() {

        if (viewId != undefined) {
            $scope.isGettingData = true;
            UtilsService.waitMultipleAsyncOperations([loadAllWidgets, loadViewByID])
                 .finally(function () {
                     loadViewWidgets($scope.allWidgets, $scope.bodyContents, $scope.summaryContents);
                     $scope.isGettingData = false;
                 });
        }
        else {
            $scope.isGettingData = true;
            UtilsService.waitMultipleAsyncOperations([loadAllWidgets])
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
                addBodyWidget(value);
            }
            

        }
    
     for (var i = 0; i < SummaryContents.length; i++) {
            var summaryContent = SummaryContents[i];
            var value = UtilsService.getItemByVal(allWidgets, summaryContent.WidgetId, 'Id');
            if (value != null) {
                value.NumberOfColumns = summaryContent.NumberOfColumns;
                addSummaryWidget(value);
            }


     }
    
    }

    function addBodyWidget(bodyWidget) {
        bodyWidget.onElementReady = function (api) {
            bodyWidget.API = api;
        };
        $scope.bodyWidgets.push(bodyWidget);
        console.log($scope.bodyWidgets);
    }
    function addSummaryWidget(summaryWidget) {
        summaryWidget.onElementReady = function (api) {
            
            summaryWidget.API = api;
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
            
           
        });
    }

}
appControllers.controller('Security_DynamicPagePreviewController', DynamicPagePreviewController);
