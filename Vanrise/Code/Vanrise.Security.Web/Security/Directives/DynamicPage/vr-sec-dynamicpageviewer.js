'use strict';
app.directive('vrSecDynamicpageviewer', ['UtilsService', 'TimeDimensionTypeEnum', 'VRModalService', 'PeriodEnum', 'VRValidationService', 'VR_Sec_ViewAPIService', 'VR_Sec_WidgetAPIService', 'ColumnWidthEnum', 'VRUIUtilsService',
    function (UtilsService, TimeDimensionTypeEnum, VRModalService, PeriodEnum, VRValidationService, VR_Sec_ViewAPIService, VR_Sec_WidgetAPIService, ColumnWidthEnum, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ctorWidgetPreview(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) { }
                }
            },
            templateUrl: "/Client/Modules/Security/Directives/DynamicPage/Templates/DynamicPageViewer.html"

        };

        function ctorWidgetPreview(ctrl, $scope) {

            var date;
            var widgetAPI;
            var viewId;
            var timeRangeDirectiveAPI;
            var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var timeDimentionDirectiveAPI;
            var timeDimentionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModal = {};
                $scope.nonSearchable = true;
                $scope.selectedTimeDimension;
                $scope.allWidgets = [];
                $scope.bodyContents = [];
                $scope.summaryWidgets = [];
                $scope.bodyWidgets = [];
                defineTimeDimensionTypes();
                $scope.Search = function () {
                    if (($scope.bodyWidgets != null && $scope.bodyWidgets != undefined) || ($scope.summaryWidgets != null && $scope.summaryWidgets != undefined)) {
                        return refreshData();
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                        if (payload.viewId != undefined) {
                            viewId = payload.viewId;
                            return UtilsService.waitMultipleAsyncOperations([loadAllWidgets, loadViewByID])
                                .then(function () {
                                    if ($scope.defaultPeriod != undefined || $scope.defaultGrouping != undefined) {
                                        $scope.nonSearchable = $scope.defaultPeriod == undefined;
                                        fillDateAndPeriod($scope.defaultPeriod, $scope.defaultGrouping).then(function () {

                                            loadViewWidgets($scope.allWidgets, $scope.bodyContents, $scope.summaryContents);

                                        });
                                    } else {
                                        loadViewWidgets($scope.allWidgets, $scope.bodyContents, $scope.summaryContents);

                                    }
                                });
                        } else {
                            $scope.nonSearchable = payload.selectedPeriod == undefined;
                            var timeDimentionType = payload.selectedTimeDimensionType != undefined ? payload.selectedTimeDimensionType.value : undefined;
                            fillDateAndPeriod(payload.selectedPeriod, timeDimentionType)
                                .then(function () {
                                    return UtilsService.waitMultipleAsyncOperations([loadAllWidgets])
                                        .then(function () {
                                            loadViewWidgets($scope.allWidgets, payload.bodyContents, payload.summaryContents);
                                        });
                                });
                        }
                    }

                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function fillDateAndPeriod(selectedPeriod, selectedTimeDimention) {
                var loadTimeRangePromiseDeferred = UtilsService.createPromiseDeferred();
                var loadTimeDimentionPromiseDeferred = UtilsService.createPromiseDeferred();
                var promises = [];
                if (selectedPeriod != undefined && selectedTimeDimention != undefined) {
                    $scope.onTimeRangeDirectiveReady = function (api) {
                        timeRangeDirectiveAPI = api;
                        timeRangeReadyPromiseDeferred.resolve();
                    };
                    $scope.onTimeDimentionDirectiveReady = function (api) {
                        timeDimentionDirectiveAPI = api;
                        timeDimentionReadyPromiseDeferred.resolve();
                    };
                    timeRangeReadyPromiseDeferred.promise.then(function () {
                        var timeRangePeriod = {
                            period: selectedPeriod
                        };

                        VRUIUtilsService.callDirectiveLoad(timeRangeDirectiveAPI, timeRangePeriod, loadTimeRangePromiseDeferred);

                    });

                    timeDimentionReadyPromiseDeferred.promise.then(function () {
                        var timeDimentionPeriod = {
                            selectedIds: selectedTimeDimention
                        };

                        VRUIUtilsService.callDirectiveLoad(timeDimentionDirectiveAPI, timeDimentionPeriod, loadTimeDimentionPromiseDeferred);

                    });
                } else {
                    loadTimeRangePromiseDeferred.resolve();
                    loadTimeDimentionPromiseDeferred.resolve();
                }

                promises.push(loadTimeRangePromiseDeferred.promise);
                promises.push(loadTimeDimentionPromiseDeferred.promise);

                return UtilsService.waitMultiplePromises(promises);

            }

            function loadAllWidgets() {
                return VR_Sec_WidgetAPIService.GetAllWidgets()
                    .then(function (response) {
                        for (var i = 0; i < response.length; i++)
                            $scope.allWidgets.push(response[i].Entity)
                    });

            }

            function loadViewWidgets(allWidgets, BodyContents, SummaryContents) {

                for (var i = 0; i < BodyContents.length; i++) {
                    addBodyWidget(BodyContents[i]);
                }
                for (var i = 0; i < SummaryContents.length; i++) {
                    addSummaryWidget(SummaryContents[i]);
                }
                function addBodyWidget(bodyContent) {

                    var bodyWidget = UtilsService.getItemByVal(allWidgets, bodyContent.WidgetId, 'Id');
                    var numberOfColumns;
                    for (var td in ColumnWidthEnum)
                        if (bodyContent.NumberOfColumns == ColumnWidthEnum[td].value)
                            numberOfColumns = ColumnWidthEnum[td];

                    if (bodyWidget != null) {
                        bodyWidget.NumberOfColumns = numberOfColumns;
                        bodyWidget.SectionTitle = bodyContent.SectionTitle;
                        if ($scope.nonSearchable) {
                            bodyWidget.DefaultPeriod = bodyContent.DefaultPeriod;
                            bodyWidget.DefaultGrouping = bodyContent.DefaultGrouping;
                        }
                       
                        var filter = {};
                        var widgetPeriod = {
                            title: bodyWidget.SectionTitle,
                            settings: bodyWidget.Setting.Settings,
                        };
                        bodyWidget.onElementReady = function (api) {
                            bodyWidget.API = api;

                            if (!$scope.nonSearchable) {
                                widgetPeriod.filter = GetFilter();
                                bodyWidget.API.load(widgetPeriod);
                            } else {
                                var widgetDate = UtilsService.getPeriod(bodyWidget.DefaultPeriod);
                                var timeDimention = UtilsService.getItemByVal($scope.timeDimensionTypes, bodyWidget.DefaultGrouping, 'value');
                                filter = {
                                    timeDimensionType: timeDimention,
                                    fromDate: widgetDate.from,
                                    toDate: widgetDate.to
                                };
                                widgetPeriod.filter = filter;
                                bodyWidget.API.load(widgetPeriod);
                            }

                            bodyWidget.load = function () {
                                if (!$scope.nonSearchable) {
                                    widgetPeriod.filter = GetFilter();
                                    return api.load(widgetPeriod);
                                } else {
                                    widgetPeriod.filter = filter;
                                    return api.load(filter);
                                }

                            };
                        };
                        $scope.bodyWidgets.push(bodyWidget);
                    }




                   
                }

                function addSummaryWidget(summaryContent) {
                    var summaryWidget = UtilsService.getItemByVal(allWidgets, summaryContent.WidgetId, 'Id');
                    var numberOfColumns;
                    for (var td in ColumnWidthEnum)
                        if (summaryContent.NumberOfColumns == ColumnWidthEnum[td].value)
                            numberOfColumns = ColumnWidthEnum[td];
                    if (summaryWidget != null) {
                        summaryWidget.NumberOfColumns = numberOfColumns;
                        summaryWidget.SectionTitle = summaryContent.SectionTitle;
                        if ($scope.nonSearchable) {
                            summaryWidget.DefaultPeriod = summaryContent.DefaultPeriod;
                            summaryWidget.DefaultGrouping = summaryContent.DefaultGrouping;
                        }


                        var filter = {};
                        var widgetPeriod = {
                            title: summaryWidget.SectionTitle,
                            settings: summaryWidget.Setting.Settings,
                        };
                        summaryWidget.onElementReady = function (api) {

                            summaryWidget.API = api;
                            if (!$scope.nonSearchable) {
                                widgetPeriod.filter = GetFilter();
                                summaryWidget.API.load(widgetPeriod);
                            } else {
                                var widgetDate = UtilsService.getPeriod(summaryWidget.DefaultPeriod);
                                var timeDimention = UtilsService.getItemByVal($scope.timeDimensionTypes, summaryWidget.DefaultGrouping, 'value');
                                filter = {
                                    timeDimensionType: timeDimention,
                                    fromDate: widgetDate.from,
                                    toDate: widgetDate.to
                                };
                                widgetPeriod.filter = filter;
                                summaryWidget.API.load(widgetPeriod);
                            }

                            summaryWidget.load = function () {
                                if (!$scope.nonSearchable) {
                                    widgetPeriod.filter = GetFilter();
                                    return api.load(widgetPeriod);
                                } else {
                                    var widgetDate = UtilsService.getPeriod(summaryWidget.DefaultPeriod);
                                    var timeDimention = UtilsService.getItemByVal($scope.timeDimensionTypes, summaryWidget.DefaultGrouping, 'value');
                                    filter = {
                                        timeDimensionType: timeDimention,
                                        fromDate: widgetDate.from,
                                        toDate: widgetDate.to
                                    };
                                    widgetPeriod.filter = filter;
                                    return api.load(widgetPeriod);
                                }

                            };
                        };

                        $scope.summaryWidgets.push(summaryWidget);
                    }



                }
            }

            function loadViewByID() {
                return VR_Sec_ViewAPIService.GetView(viewId)
                    .then(function (response) {
                        $scope.summaryContents = response.ViewContent.SummaryContents;
                        $scope.bodyContents = response.ViewContent.BodyContents;
                        $scope.defaultPeriod = response.ViewContent.DefaultPeriod;
                        $scope.defaultGrouping = response.ViewContent.DefaultGrouping;
                    });
            }

            function refreshData() {
                var refreshDataOperations = [];
                angular.forEach($scope.bodyWidgets, function (bodyWidget) {
                    if (bodyWidget.API != undefined)
                        refreshDataOperations.push(bodyWidget.load);
                });
                angular.forEach($scope.summaryWidgets, function (summaryWidget) {
                    if (summaryWidget.API != undefined)
                        refreshDataOperations.push(summaryWidget.load);
                });

                return UtilsService.waitMultipleAsyncOperations(refreshDataOperations);
            }

            function defineTimeDimensionTypes() {
                $scope.timeDimensionTypes = [];
                for (var td in TimeDimensionTypeEnum)
                    $scope.timeDimensionTypes.push(TimeDimensionTypeEnum[td]);
            }

            function GetFilter()
            {
                var filter = {
                    timeDimensionType: timeDimentionDirectiveAPI.getSelectedValues(),
                    fromDate: $scope.scopeModal.fromDate,
                    toDate: $scope.scopeModal.toDate
                };
                return filter;
            }

            this.initializeController = initializeController;
            this.defineAPI = defineAPI;
        }

        return directiveDefinitionObject;
    }]);