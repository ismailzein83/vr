'use strict';
app.directive('vrSecWidgetpreview', ['UtilsService', 'TimeDimensionTypeEnum', 'VRModalService', 'PeriodEnum', 'VRValidationService', 'VRUIUtilsService',
    function (UtilsService, TimeDimensionTypeEnum, VRModalService, PeriodEnum, VRValidationService, VRUIUtilsService) {

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
                    pre: function ($scope, iElem, iAttrs, ctrl) { }
                };
            },
            templateUrl: "/Client/Modules/Security/Directives/Widget/Templates/WidgetPreview.html"

        };

        function ctorWidgetPreview(ctrl, $scope) {

            var widgetAPI;
            var widgetReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var timeRangeDirectiveAPI;
            var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var timeDimentionDirectiveAPI;
            var timeDimentionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.onTimeRangeDirectiveReady = function (api) {
                    timeRangeDirectiveAPI = api;
                    timeRangeReadyPromiseDeferred.resolve();
                };
                $scope.onTimeDimentionDirectiveReady = function (api) {
                    timeDimentionDirectiveAPI = api;
                    timeDimentionReadyPromiseDeferred.resolve();
                };

                $scope.onElementReady = function (api) {
                    widgetAPI = api;
                    widgetReadyPromiseDeferred.resolve();
                };

                $scope.Search = function () {
                    return refreshWidget();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {

                    if (payload != undefined)
                        $scope.widget = payload;
                    if ($scope.widget != null) {
                        $scope.widget.SectionTitle = $scope.widget.Name;
                    }

                    var loadTimeRangePromiseDeferred = UtilsService.createPromiseDeferred();
                    var loadTimeDimentionPromiseDeferred = UtilsService.createPromiseDeferred();
                    var promises = [];

                    timeRangeReadyPromiseDeferred.promise.then(function () {
                        var timeRangePeriod = {
                            period: PeriodEnum.CurrentMonth.value
                        };

                        VRUIUtilsService.callDirectiveLoad(timeRangeDirectiveAPI, timeRangePeriod, loadTimeRangePromiseDeferred);

                    });

                    timeDimentionReadyPromiseDeferred.promise.then(function () {
                        var timeDimentionPeriod = {
                            selectedIds: TimeDimensionTypeEnum.Daily.value
                        };

                        VRUIUtilsService.callDirectiveLoad(timeDimentionDirectiveAPI, timeDimentionPeriod, loadTimeDimentionPromiseDeferred);

                    });
                    var loadwidgetPromiseDeferred = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultiplePromises(promises)
                        .then(function () {
                            UtilsService.safeApply($scope);
                            widgetReadyPromiseDeferred.promise.then(function () {
                                var widgetPeriod = {
                                    filter: getFilter(),
                                    title: $scope.widget.SectionTitle,
                                    settings: $scope.widget.Setting.settings,

                                };
                                widgetReadyPromiseDeferred = undefined;
                                VRUIUtilsService.callDirectiveLoad(widgetAPI, widgetPeriod, loadwidgetPromiseDeferred);

                            });

                        });
                    return loadwidgetPromiseDeferred.promise;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function refreshWidget() {
                var widgetPeriod = {
                    filter: getFilter(),
                    title: $scope.widget.SectionTitle,
                    settings: $scope.widget.Setting.settings,

                };
                return widgetAPI.load(widgetPeriod);

            }
            function getFilter()
            {
                return {
                    timeDimensionType: $scope.selectedTimeDimension,
                    fromDate: $scope.fromDate,
                    toDate: $scope.toDate
                };
            }
            this.initializeController = initializeController;
            this.defineAPI = defineAPI;
        }

        return directiveDefinitionObject;
    }]);