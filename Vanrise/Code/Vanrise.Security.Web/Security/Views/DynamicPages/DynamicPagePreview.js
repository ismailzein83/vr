DynamicPagePreviewController.$inject = ['$scope', 'ViewAPIService', 'WidgetAPIService', 'BITimeDimensionTypeEnum', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

function DynamicPagePreviewController($scope, ViewAPIService, WidgetAPIService, BITimeDimensionTypeEnum, UtilsService, VRNotificationService, VRNavigationService) {
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
            if ($scope.viewWidgets != null && $scope.viewWidgets != undefined && $scope.viewWidgets.length > 0) {
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
        angular.forEach($scope.viewWidgets, function (viewWidget) {
            refreshDataOperations.push(viewWidget.API.retrieveData);
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
                     loadViewWidgets($scope.allWidgets, $scope.viewContent);
                     $scope.isGettingData = false;
                 });
        }
        else {
            $scope.isGettingData = true;
            UtilsService.waitMultipleAsyncOperations([loadAllWidgets])
                .finally(function () {
                    loadViewWidgets($scope.allWidgets, $scope.$parent.viewContents);
                    $scope.isGettingData = false;
                });

        }   
    }

    function loadViewWidgets(allWidgets, viewContents) {
        for (var i = 0; i < viewContents.length; i++) {
            var viewContent = viewContents[i];
            var value = UtilsService.getItemByVal(allWidgets, viewContent.WidgetId, 'Id');
            if (value != null)
            {
                value.NumberOfColumns = viewContent.NumberOfColumns;
                addElementReady(value);
            }

        }
    }

    function addElementReady(viewWidget) {
        viewWidget.onElementReady = function (api) {
            viewWidget.API = api;
        };
        $scope.viewWidgets.push(viewWidget);
    }

    function loadAllWidgets() {
        return WidgetAPIService.GetAllWidgets().then(function (response) {
            $scope.allWidgets = response;
        });

    }

    function loadViewByID() {
        return ViewAPIService.GetView(viewId).then(function (response) {
            $scope.viewContent = response.Content;
        });
    }

}
appControllers.controller('Security_DynamicPagePreviewController', DynamicPagePreviewController);
