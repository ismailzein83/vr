'use strict';
app.directive('vrSecDynamicpageviewer', ['UtilsService', 'TimeDimensionTypeEnum', 'VRModalService', 'PeriodEnum', 'VRValidationService','VR_Sec_ViewAPIService','WidgetAPIService','ColumnWidthEnum','VRUIUtilsService',
    function (UtilsService, TimeDimensionTypeEnum, VRModalService, PeriodEnum, VRValidationService, VR_Sec_ViewAPIService, WidgetAPIService, ColumnWidthEnum, VRUIUtilsService) {

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
            var timeRangeDirectiveAPI;
            var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var timeDimentionDirectiveAPI;
            var timeDimentionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.nonSearchable = true;
                $scope.allWidgets = [];
                $scope.bodyContents = [];
                $scope.summaryWidgets = [];
                $scope.bodyWidgets = [];
                defineTimeDimensionTypes();
                $scope.Search = function () {
                    var obj = timeRangeDirectiveAPI.getData();
                    $scope.filter = {
                        timeDimensionType: $scope.selectedTimeDimension,
                        fromDate: new Date(obj.fromDate),
                        toDate: new Date(obj.toDate)
                    }

                    if (($scope.bodyWidgets != null && $scope.bodyWidgets != undefined) || ($scope.summaryWidgets != null && $scope.summaryWidgets != undefined)) {
                        return refreshData();
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if(payload != undefined)
                    {
                        if (payload.viewId != undefined) {
                            viewId = payload.viewId
                            return UtilsService.waitMultipleAsyncOperations([loadAllWidgets, loadViewByID])
                                   .finally(function () {
                                       loadViewWidgets($scope.allWidgets, $scope.bodyContents, $scope.summaryContents);
                                   });
                        }
                        else {
                            $scope.nonSearchable = payload.selectedPeriod == undefined;
                            fillDateAndPeriod(payload.selectedPeriod, payload.selectedTimeDimensionType).then(function ()
                            {
                                if (!$scope.nonSearchable)
                                {
                                    var obj = timeRangeDirectiveAPI.getData();
                                    $scope.filter = {
                                        timeDimensionType: $scope.selectedTimeDimension,
                                        fromDate: obj.fromDate,
                                        toDate: obj.toDate
                                    }
                                }
                              
                                return UtilsService.waitMultipleAsyncOperations([loadAllWidgets])
                                  .finally(function () {
                                      loadViewWidgets($scope.allWidgets, payload.bodyContents, payload.summaryContents);
                                  });
                            });                 
                        }
                    }
                    
                    
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function fillDateAndPeriod(selectedPeriod,selectedTimeDimention) {
                var loadTimeRangePromiseDeferred = UtilsService.createPromiseDeferred();
                var loadTimeDimentionPromiseDeferred = UtilsService.createPromiseDeferred();
                var promises = [];
                if (selectedPeriod != undefined && selectedTimeDimention != undefined) {
                    $scope.onTimeRangeDirectiveReady = function (api) {
                        timeRangeDirectiveAPI = api;
                        timeRangeReadyPromiseDeferred.resolve();
                    }
                    $scope.onTimeDimentionDirectiveReady= function (api) {
                        timeDimentionDirectiveAPI = api;
                        timeDimentionReadyPromiseDeferred.resolve();
                    }
                    timeRangeReadyPromiseDeferred.promise.then(function () {
                        var timeRangePeriod = {
                            period: selectedPeriod
                        };

                        VRUIUtilsService.callDirectiveLoad(timeRangeDirectiveAPI, timeRangePeriod, loadTimeRangePromiseDeferred);

                    });

                    timeDimentionReadyPromiseDeferred.promise.then(function () {
                        var timeDimentionPeriod = {
                            period: selectedTimeDimention.value
                        };

                        VRUIUtilsService.callDirectiveLoad(timeDimentionDirectiveAPI, timeDimentionPeriod, loadTimeDimentionPromiseDeferred);

                    });
                }
                else
                {
                    loadTimeRangePromiseDeferred.resolve();
                    loadTimeDimentionPromiseDeferred.resolve();
                }
                   
                promises.push(loadTimeRangePromiseDeferred.promise);
                promises.push(loadTimeDimentionPromiseDeferred.promise);

                return UtilsService.waitMultiplePromises(promises);
               
            }

            function loadAllWidgets() {
                return WidgetAPIService.GetAllWidgets().then(function (response) {
                    for (var i = 0; i < response.length; i++)
                        $scope.allWidgets.push(response[i].Entity)
                });

            }

            function loadViewWidgets(allWidgets, BodyContents, SummaryContents) {
                
                for (var i = 0; i < BodyContents.length; i++) {
                    var bodyContent = BodyContents[i];
                    var value = UtilsService.getItemByVal(allWidgets, bodyContent.WidgetId, 'Id');
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

                var filter = {};
                var widgetPeriod = {
                    title: bodyWidget.SectionTitle,
                    settings: bodyWidget.Setting.Settings,
                };
                bodyWidget.onElementReady = function (api) {
                    bodyWidget.API = api;
                   
                    if (!$scope.nonSearchable)
                    {
                        widgetPeriod.filter = $scope.filter;
                        bodyWidget.API.load(widgetPeriod);
                    }
                       
                    else {
                        var widgetDate = UtilsService.getPeriod(bodyWidget.DefaultPeriod);
                        var timeDimention = UtilsService.getItemByVal($scope.timeDimensionTypes, bodyWidget.DefaultGrouping, 'value');
                        filter = {
                            timeDimensionType: timeDimention,
                            fromDate: widgetDate.from,
                            toDate: widgetDate.to
                        }
                        widgetPeriod.filter = filter;
                        bodyWidget.API.load(widgetPeriod);
                    }

                    bodyWidget.load = function () {
                        if (!$scope.nonSearchable)
                        {
                            widgetPeriod.filter = $scope.filter;
                            return api.load(widgetPeriod);
                        }
                           
                        else
                        {
                            widgetPeriod.filter = filter;
                            return api.load(filter);
                        }
                            
                    };
                };
                $scope.bodyWidgets.push(bodyWidget);
            }

            function addSummaryWidget(summaryWidget) {
                var filter = {};
                var widgetPeriod = {
                    title: summaryWidget.SectionTitle,
                    settings: summaryWidget.Setting.Settings,
                };
                summaryWidget.onElementReady = function (api) {

                    summaryWidget.API = api;
                    if (!$scope.nonSearchable){
                         widgetPeriod.filter= $scope.filter;
                        summaryWidget.API.load(widgetPeriod);
                    }
                       
                    else {
                        var widgetDate = UtilsService.getPeriod(summaryWidget.DefaultPeriod);
                        var timeDimention = UtilsService.getItemByVal($scope.timeDimensionTypes, summaryWidget.DefaultGrouping, 'value');
                        filter = {
                            timeDimensionType: timeDimention,
                            fromDate: widgetDate.from,
                            toDate: widgetDate.to
                        }
                        widgetPeriod.filter= filter;
                        summaryWidget.API.load(widgetPeriod);
                    }

                    summaryWidget.load = function () {
                        if (!$scope.nonSearchable)
                        {
                            widgetPeriod.filter = $scope.filter;
                            return api.load(widgetPeriod);
                        }     
                        else
                        {
                            var widgetDate = UtilsService.getPeriod(summaryWidget.DefaultPeriod);
                            var timeDimention = UtilsService.getItemByVal($scope.timeDimensionTypes, summaryWidget.DefaultGrouping, 'value');
                            filter = {
                                timeDimensionType: timeDimention,
                                fromDate: widgetDate.from,
                                toDate: widgetDate.to
                            }
                            widgetPeriod.filter = filter;
                            return api.load(widgetPeriod);
                        }
                            

                    };
                };

                $scope.summaryWidgets.push(summaryWidget);
            }

            function loadViewByID() {
                return VR_Sec_ViewAPIService.GetView(viewId).then(function (response) {
                    $scope.summaryContents = response.ViewContent.SummaryContents;
                    $scope.bodyContents = response.ViewContent.BodyContents;
                    if (response.ViewContent.DefaultPeriod != undefined || response.ViewContent.DefaultGrouping != undefined) {
                        $scope.nonSearchable = response.ViewContent.DefaultPeriod == undefined;
                        fillDateAndPeriod(response.ViewContent.DefaultPeriod, response.ViewContent.DefaultGrouping).then(function () {
                            if (!$scope.nonSearchable) {
                                var obj = timeRangeDirectiveAPI.getData();
                                $scope.filter = {
                                    timeDimensionType: $scope.selectedTimeDimension,
                                    fromDate: obj.fromDate,
                                    toDate: obj.toDate
                                }
                            }
                            
                        });
                    }

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

            this.initializeController = initializeController;
            this.defineAPI = defineAPI;
        }




        return directiveDefinitionObject;
    }]);

