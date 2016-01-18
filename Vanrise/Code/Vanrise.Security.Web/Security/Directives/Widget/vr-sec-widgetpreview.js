'use strict';
app.directive('vrSecWidgetpreview', ['UtilsService', 'TimeDimensionTypeEnum', 'VRModalService', 'PeriodEnum','VRValidationService',
    function (UtilsService, TimeDimensionTypeEnum, VRModalService, PeriodEnum, VRValidationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var ctor = new ctorWidgetPreview(ctrl, $scope);
            ctor.initializeController();


            //$scope.openReportEntityModal = function (item) {

            //    BIUtilitiesService.openEntityReport(item.EntityType, item.EntityId, item.EntityName);

            //}

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
                }
            }
        },
        templateUrl: "/Client/Modules/Security/Directives/Widget/Templates/WidgetPreview.html"

    };

    function ctorWidgetPreview(ctrl, $scope) {

        var date;
        var widgetAPI;
       
        function initializeController() {
            $scope.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            }

            $scope.selectedPeriod = PeriodEnum.CurrentMonth;
            date = $scope.selectedPeriod.getInterval();
            $scope.fromDate = date.from;
            $scope.toDate = date.to;

            defineTimeDimensionTypes();
            $scope.filter = {
                timeDimensionType: $scope.selectedTimeDimensionType,
                fromDate: $scope.fromDate,
                toDate: $scope.toDate
            }
            $scope.periods = UtilsService.getArrayEnum(PeriodEnum);

            var customize = {
                value: -1,
                description: "Customize"
            }
            $scope.onBlurChanged = function () {
                var from = UtilsService.getShortDate($scope.fromDate);
                var oldFrom = UtilsService.getShortDate(date.from);
                var to = UtilsService.getShortDate($scope.toDate);
                var oldTo = UtilsService.getShortDate(date.to);
                if (from != oldFrom || to != oldTo)
                    $scope.selectedPeriod = customize;

            }




            $scope.onElementReady = function (api) {
                widgetAPI = api;
                return widgetAPI.retrieveData($scope.filter);
            };

            $scope.Search = function () {
                $scope.filter = {
                    timeDimensionType: $scope.selectedTimeDimensionType,
                    fromDate: $scope.fromDate,
                    toDate: $scope.toDate
                }
                return refreshWidget();
            };
            $scope.periodSelectionChanged = function () {

                if ($scope.selectedPeriod.value != -1) {

                    date = $scope.selectedPeriod.getInterval();
                    $scope.fromDate = date.from;
                    $scope.toDate = date.to;
                }

            }

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                if (payload !=undefined)
                    $scope.widget = payload;
                if ($scope.widget != null) {
                    $scope.widget.SectionTitle = $scope.widget.Name;
                }
                if (widgetAPI !=undefined)
                    return widgetAPI.retrieveData($scope.filter);
            }
            
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function refreshWidget() {
           
            return widgetAPI.retrieveData($scope.filter);
        }
        function defineTimeDimensionTypes() {
            $scope.timeDimensionTypes = [];
            for (var td in TimeDimensionTypeEnum)
                $scope.timeDimensionTypes.push(TimeDimensionTypeEnum[td]);
            $scope.selectedTimeDimensionType = TimeDimensionTypeEnum.Daily;
        }


        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }




    return directiveDefinitionObject;
}]);

