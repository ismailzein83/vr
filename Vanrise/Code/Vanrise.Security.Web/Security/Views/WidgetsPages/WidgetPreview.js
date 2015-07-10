WidgetPreviewController.$inject = ['$scope', 'BITimeDimensionTypeEnum'];

function WidgetPreviewController($scope, BITimeDimensionTypeEnum) {
    var widgetAPI;
    defineScope();
    load();

    function defineScope() {
        $scope.fromDate = "2015-04-01";
        $scope.toDate = "2015-04-30";
        defineTimeDimensionTypes();
        $scope.filter = {
            timeDimensionType: $scope.selectedTimeDimensionType,
            fromDate: $scope.fromDate,
            toDate: $scope.toDate
        }
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.widget = $scope.$parent.widget;
        $scope.onElementReady = function (api) {
            widgetAPI = api;
            updateDashboard();
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
        if (widgetAPI == undefined)
            return;
        $scope.isGettingData = true;
        return widgetAPI.retrieveData()
            .finally(function () {
                $scope.isGettingData = false;
            });
    }

}
appControllers.controller('Security_WidgetPreviewController', WidgetPreviewController);
