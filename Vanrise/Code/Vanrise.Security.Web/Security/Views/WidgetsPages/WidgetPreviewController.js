WidgetPreviewController.$inject = ['$scope', 'TimeDimensionTypeEnum', 'PeriodEnum', 'UtilsService', 'VRValidationService'];

function WidgetPreviewController($scope, TimeDimensionTypeEnum, PeriodEnum, UtilsService, VRValidationService) {
    var widgetAPI;
    defineScope();
    load();
    var date;

    function defineScope() {
        $scope.scopeModal = {};

        $scope.scopeModal.validateDateTime = function () {
            return VRValidationService.validateTimeRange($scope.scopeModal.fromDate, $scope.scopeModal.toDate);
        };

        $scope.scopeModal.selectedPeriod = PeriodEnum.CurrentMonth;
        date = $scope.scopeModal.selectedPeriod.getInterval();
        $scope.scopeModal.fromDate = date.from;
        $scope.scopeModal.toDate = date.to;

        defineTimeDimensionTypes();
        $scope.scopeModal.filter = {
            timeDimensionType: $scope.scopeModal.selectedTimeDimensionType,
            fromDate: $scope.scopeModal.fromDate,
            toDate: $scope.scopeModal.toDate
        };
        $scope.scopeModal.periods = UtilsService.getArrayEnum(PeriodEnum);
        $scope.scopeModal.close = function () {
            $scope.modalContext.closeModal()
        };
        var customize = {
            value: -1,
            description: "Customize"
        };
        $scope.scopeModal.onBlurChanged = function () {
            var from = UtilsService.getShortDate($scope.scopeModal.fromDate);
            var oldFrom = UtilsService.getShortDate(date.from);
            var to = UtilsService.getShortDate($scope.scopeModal.toDate);
            var oldTo = UtilsService.getShortDate(date.to);
            if (from != oldFrom || to != oldTo)
                $scope.scopeModal.selectedPeriod = customize;

        };

        $scope.scopeModal.widget = $scope.$parent.widget;
        if ($scope.scopeModal.widget != null) {
            $scope.scopeModal.widget.SectionTitle = $scope.scopeModal.widget.Name;
        }

        $scope.scopeModal.onElementReady = function (api) {
            widgetAPI = api;
            widgetAPI.retrieveData($scope.scopeModal.filter);
        };

        $scope.scopeModal.Search = function () {
            $scope.scopeModal.filter = {
                timeDimensionType: $scope.scopeModal.selectedTimeDimensionType,
                fromDate: $scope.scopeModal.fromDate,
                toDate: $scope.scopeModal.toDate
            };
            return refreshWidget();
        };
        $scope.scopeModal.periodSelectionChanged = function () {

            if ($scope.scopeModal.selectedPeriod.value != -1) {

                date = $scope.scopeModal.selectedPeriod.getInterval();
                $scope.scopeModal.fromDate = date.from;
                $scope.scopeModal.toDate = date.to;
            }

        }

    }

    function refreshWidget() {
        return widgetAPI.retrieveData($scope.scopeModal.filter);
    }

    function defineTimeDimensionTypes() {
        $scope.scopeModal.timeDimensionTypes = [];
        for (var td in TimeDimensionTypeEnum)
            $scope.scopeModal.timeDimensionTypes.push(TimeDimensionTypeEnum[td]);
        $scope.scopeModal.selectedTimeDimensionType = TimeDimensionTypeEnum.Daily;

    }

    function load() {

        $scope.scopeModal.isGettingData = false;

    }

}
appControllers.controller('VR_Sec_WidgetPreviewController', WidgetPreviewController);