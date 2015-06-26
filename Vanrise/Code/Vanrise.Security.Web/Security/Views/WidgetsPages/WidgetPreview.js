WidgetPreviewController.$inject = ['$scope', 'BIVisualElementService1', 'BITimeDimensionTypeEnum', 'BIConfigurationAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function WidgetPreviewController($scope, BIVisualElementService1, BITimeDimensionTypeEnum, BIConfigurationAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {
    defineScope();
    load();
    function getVisualElement() {
        $scope.visualElement = $scope.$parent.visualElement;
        console.log($scope.visualElement);
        addTimeToVisualElement();
    }
    function addTimeToVisualElement() {
            $scope.visualElement.settings.fromdate = $scope.fromDate;
            $scope.visualElement.settings.todate = $scope.toDate;
            $scope.visualElement.settings.timedimensiontype = $scope.selectedTimeDimensionType;
    }
    function defineScope() {
        $scope.visualElement;
        $scope.fromDate = "2015-04-01";
        $scope.toDate = "2015-04-30";
        defineTimeDimensionTypes();
        $scope.subViewValue = {};
        $scope.filter = {
            timeDimensionType: $scope.selectedTimeDimensionType,
            fromDate: $scope.fromDate,
            toDate: $scope.toDate
        }
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.chartReady = function (api) {
            $scope.chartAPI = api;

        };
        $scope.Search = function () {
            if ($scope.visualElement != null && $scope.visualElement != undefined && $scope.visualElement.length > 0) {
                console.log("update");
                updateDashboard();
            }
            else {
                getVisualElement();
               
            }
        };

        $scope.chartTopReady = function (api) {
            chartTopAPI = api;
        };
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
        visualElement.API.retrieveData;
        $scope.isGettingData = true;
        return UtilsService.waitMultipleAsyncOperations(visualElement.API.retrieveData)
            .finally(function () {
                $scope.isGettingData = false;
            });
    }





}
appControllers.controller('Security_WidgetPreviewController', WidgetPreviewController);
