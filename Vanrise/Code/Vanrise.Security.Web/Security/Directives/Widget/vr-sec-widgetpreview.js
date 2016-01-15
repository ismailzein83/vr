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
            $scope.scopeModal = {};
            $scope.scopeModal.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.scopeModal.fromDate, $scope.scopeModal.toDate);
            }

            $scope.scopeModal.selectedPeriod = PeriodEnum.CurrentMonth;
            date = $scope.scopeModal.selectedPeriod.getInterval();
            $scope.scopeModal.fromDate = date.from;
            $scope.scopeModal.toDate = date.to;

            defineTimeDimensionTypes();
            $scope.scopeModal.filter = {
                timeDimensionType: $scope.scopeModal.selectedTimeDimensionType,
                fromDate: $scope.scopeModal.fromDate,
                toDate: $scope.scopeModal.toDate
            }
            $scope.scopeModal.periods = UtilsService.getArrayEnum(PeriodEnum);

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




            $scope.scopeModal.onElementReady = function (api) {
                widgetAPI = api;
                console.log(api);
                return widgetAPI.retrieveData($scope.scopeModal.filter);
            };

            $scope.scopeModal.Search = function () {
                $scope.scopeModal.filter = {
                    timeDimensionType: $scope.scopeModal.selectedTimeDimensionType,
                    fromDate: $scope.scopeModal.fromDate,
                    toDate: $scope.scopeModal.toDate
                }
                return refreshWidget();
            };
            $scope.scopeModal.periodSelectionChanged = function () {

                if ($scope.scopeModal.selectedPeriod.value != -1) {

                    date = $scope.scopeModal.selectedPeriod.getInterval();
                    $scope.scopeModal.fromDate = date.from;
                    $scope.scopeModal.toDate = date.to;
                }

            }

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                console.log(payload);
                if (payload !=undefined)
                    $scope.scopeModal.widget = payload;
                if ($scope.scopeModal.widget != null) {
                    $scope.scopeModal.widget.SectionTitle = $scope.scopeModal.widget.Name;
                }
                if (widgetAPI !=undefined)
                    return widgetAPI.retrieveData($scope.scopeModal.filter);
            }
            
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function refreshWidget() {
           
            return widgetAPI.retrieveData($scope.scopeModal.filter);
        }
        function defineTimeDimensionTypes() {
            $scope.scopeModal.timeDimensionTypes = [];
            for (var td in TimeDimensionTypeEnum)
                $scope.scopeModal.timeDimensionTypes.push(TimeDimensionTypeEnum[td]);
            $scope.scopeModal.selectedTimeDimensionType = TimeDimensionTypeEnum.Daily;
            console.log($scope.scopeModal.timeDimensionTypes);
        }


        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }




    return directiveDefinitionObject;
}]);

