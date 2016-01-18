'use strict';
app.directive('vrSecDynamicpageviewer', ['UtilsService', 'TimeDimensionTypeEnum', 'VRModalService', 'PeriodEnum', 'VRValidationService','VR_Sec_ViewAPIService','WidgetAPIService','ColumnWidthEnum',
    function (UtilsService, TimeDimensionTypeEnum, VRModalService, PeriodEnum, VRValidationService, VR_Sec_ViewAPIService, WidgetAPIService, ColumnWidthEnum) {

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
                    pre: function ($scope, iElem, iAttrs, ctrl) {
                    }
                }
            },
            templateUrl: "/Client/Modules/Security/Directives/DynamicPage/Templates/DynamicPageViewer.html"

        };

        function ctorWidgetPreview(ctrl, $scope) {

            var date;
            var widgetAPI;
            var viewId;
            function initializeController() {
               
                var date;
                $scope.fromDate;
                $scope.validateDateTime = function () {
                    return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
                }
                $scope.nonSearchable = true;
                $scope.toDate;
                $scope.allWidgets = [];
                $scope.viewContent = [];
                $scope.periods = UtilsService.getArrayEnum(PeriodEnum);
                $scope.selectedPeriod;
                $scope.summaryContents = [];
                $scope.bodyContents = [];
                $scope.summaryWidgets = [];
                $scope.bodyWidgets = [];
                $scope.viewWidgets = [];
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
                $scope.periodSelectionChanged = function () {
                    if ($scope.selectedPeriod != undefined && $scope.selectedPeriod.value != -1) {
                        date = $scope.selectedPeriod.getInterval();
                        $scope.fromDate = date.from;
                        $scope.toDate = date.to;

                    }

                }
                defineTimeDimensionTypes();


         
                $scope.Search = function () {

                    $scope.filter = {
                        timeDimensionType: $scope.selectedTimeDimensionType,
                        fromDate: $scope.fromDate,
                        toDate: $scope.toDate
                    }

                    if (($scope.bodyWidgets != null && $scope.bodyWidgets != undefined) || ($scope.summaryWidgets != null && $scope.summaryWidgets != undefined)) {
                        return refreshData();
                    }
                    else {
                        return loadWidgets();
                    }
                };





             
                defineAPI();
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if(payload !=undefined)
                    {
                        if (payload.viewId != undefined) {
                            viewId = payload.viewId
                            return UtilsService.waitMultipleAsyncOperations([loadAllWidgets, loadViewByID])
                                   .finally(function () {
                                       loadViewWidgets($scope.allWidgets, $scope.bodyContents, $scope.summaryContents);
                                   });
                        }
                        else {
                            $scope.selectedPeriod = payload.selectedViewPeriod;
                            $scope.selectedTimeDimensionType = payload.selectedViewTimeDimensionType;
                            fillDateAndPeriod();
                            $scope.nonSearchable = false;
                            return UtilsService.waitMultipleAsyncOperations([loadAllWidgets])
                              .finally(function () {
                                  loadViewWidgets($scope.allWidgets, payload.bodyContents, payload.summaryContents);
                              });
                        }
                    }
                    
                    
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function refreshWidget() {

                return widgetAPI.retrieveData($scope.filter);
            }

            function fillDateAndPeriod() {
                date = $scope.selectedPeriod.getInterval();
                $scope.fromDate = date.from;
                $scope.toDate = date.to;
                $scope.filter = {
                    timeDimensionType: $scope.selectedTimeDimensionType,
                    fromDate: $scope.fromDate,
                    toDate: $scope.toDate
                }
            }

            function loadAllWidgets() {
                return WidgetAPIService.GetAllWidgets().then(function (response) {
                    for (var i = 0; i < response.length; i++)
                        $scope.allWidgets.push(response[i].Entity)
                  //  $scope.allWidgets = response;
                });

            }

            function loadViewWidgets(allWidgets, BodyContents, SummaryContents) {
                console.log(allWidgets);
                
                for (var i = 0; i < BodyContents.length; i++) {
                    var bodyContent = BodyContents[i];
                    var value = UtilsService.getItemByVal(allWidgets, bodyContent.WidgetId, 'Id');
                    console.log(value);
                    var numberOfColumns;
                    for (var td in ColumnWidthEnum)
                        if (bodyContent.NumberOfColumns == ColumnWidthEnum[td].value)
                            numberOfColumns = ColumnWidthEnum[td]

                    if (value != null) {
                        value.NumberOfColumns = numberOfColumns;
                        value.SectionTitle = bodyContent.SectionTitle;
                        if ($scope.nonSearchable) {
                            value.DefaultPeriod = bodyContent.DefaultPeriod;
                            value.DefaultGrouping = bodyContent.DefaultGrouping;
                        }
                        console.log(value);
                        addBodyWidget(value);
                    }
                   


                }
                for (var i = 0; i < SummaryContents.length; i++) {
                    var summaryContent = SummaryContents[i];

                    var value = UtilsService.getItemByVal(allWidgets, summaryContent.WidgetId, 'Id');
                    var numberOfColumns;
                    for (var td in ColumnWidthEnum)
                        if (summaryContent.NumberOfColumns == ColumnWidthEnum[td].value)
                            numberOfColumns = ColumnWidthEnum[td]
                    if (value != null) {
                        value.NumberOfColumns = numberOfColumns;
                        value.SectionTitle = summaryContent.SectionTitle;
                        if ($scope.nonSearchable) {
                            value.DefaultPeriod = summaryContent.DefaultPeriod;
                            value.DefaultGrouping = summaryContent.DefaultGrouping;
                        }

                        addSummaryWidget(value);
                    }

                }

            }

            function addBodyWidget(bodyWidget) {
                bodyWidget.onElementReady = function (api) {
                    bodyWidget.API = api;
                    var filter = {};
                    if (!$scope.nonSearchable)
                        bodyWidget.API.retrieveData($scope.filter);
                    else {
                        var widgetDate = UtilsService.getPeriod(bodyWidget.DefaultPeriod);
                        var timeDimention = UtilsService.getItemByVal($scope.timeDimensionTypes, bodyWidget.DefaultGrouping, 'value');
                        filter = {
                            timeDimensionType: timeDimention,
                            fromDate: widgetDate.from,
                            toDate: widgetDate.to
                        }
                        bodyWidget.API.retrieveData(filter);
                    }

                    bodyWidget.retrieveData = function () {
                        if (!$scope.nonSearchable)
                            return api.retrieveData($scope.filter);
                        else
                            return api.retrieveData(filter);
                    };
                };
                $scope.bodyWidgets.push(bodyWidget);
                console.log($scope.bodyWidgets);
            }

            function addSummaryWidget(summaryWidget) {
                summaryWidget.onElementReady = function (api) {

                    summaryWidget.API = api;
                    var filter = {};
                    if (!$scope.nonSearchable)
                        summaryWidget.API.retrieveData($scope.filter);
                    else {
                        var widgetDate = UtilsService.getPeriod(summaryWidget.DefaultPeriod);
                        var timeDimention = UtilsService.getItemByVal($scope.timeDimensionTypes, summaryWidget.DefaultGrouping, 'value');
                        filter = {
                            timeDimensionType: timeDimention,
                            fromDate: widgetDate.from,
                            toDate: widgetDate.to
                        }
                        summaryWidget.API.retrieveData(filter);
                    }

                    summaryWidget.retrieveData = function () {
                        if (!$scope.nonSearchable)
                            return api.retrieveData($scope.filter);
                        else
                            return api.retrieveData(filter);

                    };
                };

                $scope.summaryWidgets.push(summaryWidget);
            }

            function loadViewByID() {
                return VR_Sec_ViewAPIService.GetView(viewId).then(function (response) {
                    $scope.summaryContents = response.ViewContent.SummaryContents;
                    $scope.bodyContents = response.ViewContent.BodyContents;
                    if (response.ViewContent.DefaultPeriod != undefined || response.ViewContent.DefaultGrouping != undefined) {
                        $scope.selectedPeriod = UtilsService.getItemByVal($scope.periods, response.ViewContent.DefaultPeriod, 'value');
                        $scope.selectedTimeDimensionType = UtilsService.getItemByVal($scope.timeDimensionTypes, response.ViewContent.DefaultGrouping, 'value');
                        $scope.nonSearchable = false;
                        fillDateAndPeriod();
                    }

                });
            }

            function refreshData() {
                var refreshDataOperations = [];
                angular.forEach($scope.bodyWidgets, function (bodyWidget) {
                    if (bodyWidget.API != undefined)
                        refreshDataOperations.push(bodyWidget.retrieveData);
                });
                angular.forEach($scope.summaryWidgets, function (summaryWidget) {
                    if (summaryWidget.API != undefined)
                        refreshDataOperations.push(summaryWidget.retrieveData);
                });

                return UtilsService.waitMultipleAsyncOperations(refreshDataOperations)
                          .finally(function () {
                          });
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

