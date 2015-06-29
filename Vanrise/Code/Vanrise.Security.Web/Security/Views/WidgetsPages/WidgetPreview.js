WidgetPreviewController.$inject = ['$scope', 'BIVisualElementService1', 'BITimeDimensionTypeEnum', 'BIConfigurationAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function WidgetPreviewController($scope, BIVisualElementService1, BITimeDimensionTypeEnum, BIConfigurationAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {
    var widgetAPI;
    defineScope();
    load();
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
        $scope.visualElement = $scope.$parent.visualElement;
        $scope.onElementReady = function (api) {
            widgetAPI = api;
        };
        $scope.Search = function () {
                updateDashboard();
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

        $scope.isGettingData = true;
        return widgetAPI.retrieveData()
            .finally(function () {
                $scope.isGettingData = false;
            });
    }

}
appControllers.controller('Security_WidgetPreviewController', WidgetPreviewController);
