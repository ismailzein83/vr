DynamicPagePreviewController.$inject = ['$scope', '$routeParams','DynamicPagesManagementAPIService', 'BIVisualElementService1', 'BITimeDimensionTypeEnum', 'BIConfigurationAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function DynamicPagePreviewController($scope, $routeParams,DynamicPagesManagementAPIService, BIVisualElementService1, BITimeDimensionTypeEnum, BIConfigurationAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {
    loadParameters();
    defineScope();
    load();
    var viewId;
    function loadParameters() {
        if ($routeParams.params!=undefined)
            viewId = JSON.parse($routeParams.params).viewId;//.viewId;
          //  VRNavigationService.getParameters($scope);
      //  $scope.visualElements = 
        
      // console.log($scope.visualElements);
    }
    function getVisualElements() {
        console.log("out");
        console.log(viewId);
        if (viewId != undefined) {
            console.log(viewId);
            return DynamicPagesManagementAPIService.GetView(viewId).then(function (response) {
                $scope.visualElements = response.Content;
                addTimeToVisualElements();
            });
        }
        else {
            $scope.visualElements = $scope.$parent.visualElements;
            addTimeToVisualElements();
        }
    }
    function addTimeToVisualElements(){
        for (var i = 0; i < $scope.visualElements.length; i++) {
            $scope.visualElements[i].settings.fromdate = $scope.fromDate;
            $scope.visualElements[i].settings.todate = $scope.toDate;
            $scope.visualElements[i].settings.timedimensiontype = $scope.selectedTimeDimensionType;

        }
    }
    function defineScope() {
        $scope.visualElements = [];
        $scope.fromDate = "2015-04-01";
        $scope.toDate = "2015-04-30";
        defineTimeDimensionTypes();
        $scope.subViewValue = {};
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.save = function () {
        };
        $scope.chartReady = function (api) {
            $scope.chartAPI = api;

        };
   
        $scope.Search = function () {
            if ($scope.visualElements != null && $scope.visualElements != undefined && $scope.visualElements.length>0) {
                console.log("update");
                updateDashboard();
            }
            else {
                
            getVisualElements();
           

            }

            
            
        };
        $scope.chartTopReady = function (api) {
            chartTopAPI = api;
            // updateChart();
        };
        //$scope.addVisualElement = function () {
        //    $scope.subViewValue = $scope.subViewValue.getValue();
        //    var visualElement = {
        //        settings: $scope.subViewValue,
        //        directive: $scope.selectedWidget.directiveName,
        //        numberOfColumns: $scope.selectedNumberOfColumns.value
        //    };

        //    visualElement.onElementReady = function (api) {
        //        visualElement.API = api;
        //    };
        //    $scope.visualElements.push(visualElement);
        //    console.log(visualElement.settings.timedimensiontype);
        //    $scope.selectedWidget = null;

        //};
        //$scope.removeVisualElement = function (visualElement) {
        //    $scope.visualElements.splice($scope.visualElements.indexOf(visualElement), 1);
        //};

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
        var refreshDataOperations = [];
        angular.forEach($scope.visualElements, function (visualElement) {
            refreshDataOperations.push(visualElement.API.retrieveData);
        });
        $scope.isGettingData = true;
        return UtilsService.waitMultipleAsyncOperations(refreshDataOperations)
            .finally(function () {
                $scope.isGettingData = false;
            });
    }





}
appControllers.controller('Security_DynamicPagePreviewController', DynamicPagePreviewController);
