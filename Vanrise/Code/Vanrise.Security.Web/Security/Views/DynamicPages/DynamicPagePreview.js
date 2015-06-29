DynamicPagePreviewController.$inject = ['$scope', '$routeParams', 'DynamicPagesAPIService','WidgetAPIService', 'BITimeDimensionTypeEnum', 'UtilsService', 'VRNotificationService'];

function DynamicPagePreviewController($scope, $routeParams, DynamicPagesAPIService,WidgetAPIService, BITimeDimensionTypeEnum, UtilsService, VRNotificationService) {
    var viewId;
    loadParameters();
    defineScope();
    load();
    
    function loadParameters() {
        if ($routeParams.params != undefined) {
            viewId = JSON.parse($routeParams.params).viewId;
        }
    }
    function defineScope() {
        $scope.allWidgets = [];
        $scope.content = [];
        $scope.widgets = [];
        $scope.fromDate = "2015-04-01";
        $scope.toDate = "2015-04-30";
        defineTimeDimensionTypes();
        $scope.subViewValue = {};
        $scope.filter = {
            timeDimensionType: $scope.selectedTimeDimensionType,
            fromDate:$scope.fromDate,
            toDate:$scope.toDate 
        }
        
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.Search = function () {
            if ($scope.widgets != null && $scope.widgets != undefined && $scope.widgets.length > 0) {
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
        angular.forEach($scope.widgets, function (widget) {
            refreshDataOperations.push(widget.API.retrieveData);
        });
        $scope.isGettingData = true;
        return UtilsService.waitMultipleAsyncOperations(refreshDataOperations)
            .finally(function () {
                $scope.isGettingData = false;
            });
        
    }
    function getData() {
        if (viewId != undefined) {
            UtilsService.waitMultipleAsyncOperations([loadAllWidgets, loadViewByID])
                 .finally(function () {
                     getWidgets($scope.allWidgets, $scope.content);
                     $scope.isGettingData = false;
                 });
        }
        else {
            UtilsService.waitMultipleAsyncOperations([loadAllWidgets])
                .finally(function () {
                    getWidgets($scope.allWidgets, $scope.$parent.Contents);
                    $scope.isGettingData = false;
                });

        }   
    }
    function getWidgets(allWidgets, content) {
        for (var i = 0; i < content.length; i++) {
            for (var j = 0; j < allWidgets.length; j++) {
                if (allWidgets[j].Id == content[i].WidgetId) {
                    allWidgets[j].NumberOfColumns = content[i].NumberOfColumns;
                    AddElementReady(allWidgets[j]);
                   
                   
                }
            }
        }
    }
    function AddElementReady(widget ) {
        var widgetElement = widget;
        widgetElement.onElementReady = function (api) {
            widgetElement.API = api;
        };
        $scope.widgets.push(widgetElement);
    }
 
    function loadAllWidgets() {
        return WidgetAPIService.GetAllWidgets().then(function (response) {
            $scope.allWidgets = response;
        });

    }
    function loadViewByID() {
        return DynamicPagesAPIService.GetView(viewId).then(function (response) {
            $scope.content = response.Content;
        });
    }

}
appControllers.controller('Security_DynamicPagePreviewController', DynamicPagePreviewController);
