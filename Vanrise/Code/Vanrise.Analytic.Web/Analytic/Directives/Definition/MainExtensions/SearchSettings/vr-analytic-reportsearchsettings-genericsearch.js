(function (app) {

    'use strict';

    ReportsearchsettingsGenericsearch.$inject = ["UtilsService",'VRUIUtilsService'];

    function ReportsearchsettingsGenericsearch(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                customvalidate: '=',
                type: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericsearch = new Genericsearch($scope, ctrl, $attrs);
                genericsearch.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/MainExtensions/SearchSettings/Templates/GenericSearchTemplate.html"

        };
        function Genericsearch($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var groupingDimensionSelectorAPI;
            var groupingDimensionReadyDeferred = UtilsService.createPromiseDeferred();
            var filterDimensionSelectorAPI;
            var filterDimensionReadyDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onGroupingDimensionSelectorDirectiveReady = function (api) {
                    groupingDimensionSelectorAPI = api;
                    groupingDimensionReadyDeferred.resolve();
                }
                $scope.scopeModel.groupingDimensions = [];
                $scope.scopeModel.isValidGroupingDimensions = function () {

                    if ($scope.scopeModel.groupingDimensions.length > 0)
                        return null;
                    return "At least one dimention should be selected.";
                }
                $scope.scopeModel.onSelectGroupingDimensionItem = function (dimension) {
                    var dataItem = {
                        AnalyticItemConfigId: dimension.AnalyticItemConfigId,
                        Title: dimension.Title,
                        Name: dimension.Name,
                        IsSelected: false
                    };
                    $scope.scopeModel.groupingDimensions.push(dataItem);
                }
                $scope.scopeModel.onDeselectGroupingDimensionItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.groupingDimensions, dataItem.AnalyticItemConfigId, 'AnalyticItemConfigId');
                    $scope.scopeModel.groupingDimensions.splice(datasourceIndex, 1);
                }
                $scope.scopeModel.removeGroupingDimension = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedGroupingDimensions, dataItem.AnalyticItemConfigId, 'AnalyticItemConfigId');
                    $scope.scopeModel.selectedGroupingDimensions.splice(index, 1);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.groupingDimensions, dataItem.AnalyticItemConfigId, 'AnalyticItemConfigId');
                    $scope.scopeModel.groupingDimensions.splice(datasourceIndex, 1);
                };


                $scope.scopeModel.filterDimensions = [];
                $scope.scopeModel.onFilterDimensionSelectorDirectiveReady = function (api) {
                    filterDimensionSelectorAPI = api;
                    filterDimensionReadyDeferred.resolve();
                }
                $scope.scopeModel.isValidFilterDimensions = function () {

                    if ($scope.scopeModel.filterDimensions.length > 0)
                        return null;
                    return "At least one dimention should be selected.";
                }
                $scope.scopeModel.onSelectFilterDimensionItem = function (dimension) {
                    var dataItem = {
                        AnalyticItemConfigId: dimension.AnalyticItemConfigId,
                        Title: dimension.Title,
                        Name: dimension.Name,
                        IsRequired: false,
                        onFieldTypeReady: function (api) {
                            dataItem.fieldAPI = api;
                            var setLoader = function (value) { $scope.isLoadingDimensionDirective = value };
                            var payload;
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.fieldAPI, payload, setLoader);
                        }
                    };
                    $scope.scopeModel.filterDimensions.push(dataItem);
                }
                $scope.scopeModel.onDeselectFilterDimensionItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.filterDimensions, dataItem.AnalyticItemConfigId, 'AnalyticItemConfigId');
                    $scope.scopeModel.filterDimensions.splice(datasourceIndex, 1);
                }
                $scope.scopeModel.removeFilterDimension = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedGroupingDimensions, dataItem.AnalyticItemConfigId, 'AnalyticItemConfigId');
                    $scope.scopeModel.selectedGroupingDimensions.splice(index, 1);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.filterDimensions, dataItem.AnalyticItemConfigId, 'AnalyticItemConfigId');
                    $scope.scopeModel.filterDimensions.splice(datasourceIndex, 1);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
             
                    if (payload != undefined && payload.tableIds!=undefined) {
                        var tableIds = payload.tableIds;
                        var promises = [];

                        var loadGroupingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        groupingDimensionReadyDeferred.promise.then(function () {
                            var payloadGroupingDirective = {
                                filter: { TableIds: tableIds }
                            };

                            VRUIUtilsService.callDirectiveLoad(groupingDimensionSelectorAPI, payloadGroupingDirective, loadGroupingDirectivePromiseDeferred);
                        });
                        promises.push(loadGroupingDirectivePromiseDeferred.promise);

                        var loadFilterDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        filterDimensionReadyDeferred.promise.then(function () {
                            var payloadFilterDirective = {
                                filter: { TableIds: tableIds }
                            };

                            VRUIUtilsService.callDirectiveLoad(filterDimensionSelectorAPI, payloadFilterDirective, loadFilterDirectivePromiseDeferred);
                        });
                        promises.push(loadFilterDirectivePromiseDeferred.promise);

                        return UtilsService.waitMultiplePromises(promises);
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var groupingDimensions;
                    if($scope.scopeModel.groupingDimensions  !=undefined && $scope.scopeModel.groupingDimensions.length>0)
                    {
                        groupingDimensions=[];
                        for(var i=0;i<$scope.scopeModel.groupingDimensions.length;i++)
                        {
                            var groupingDimension = $scope.scopeModel.groupingDimensions[i];
                            groupingDimensions.push({
                                DimensionName:groupingDimension.Name,
                                IsSelected:groupingDimension.IsSelected
                            });
                        }
                    }

                    var filterDimensions;
                    if($scope.scopeModel.filterDimensions  !=undefined && $scope.scopeModel.filterDimensions.length>0)
                    {
                        filterDimensions=[];
                        for(var i=0;i<$scope.scopeModel.filterDimensions.length;i++)
                        {
                            var filterDimension = $scope.scopeModel.filterDimensions[i];
                            filterDimensions.push({
                                DimensionName:filterDimension.Name,
                                Title: filterDimension.Title,
                                IsRequired: filterDimension.IsRequired,
                                FieldType:filterDimension.fieldAPI!=undefined?filterDimension.fieldAPI.getData():undefined
                            });
                        }
                    }
                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.SearchSettings.GenericSearchSettings, Vanrise.Analytic.MainExtensions ",
                        GroupingDimensions: groupingDimensions,
                        Filters: filterDimensions
                    }
                    return data;
                }
            }
        }
    }

    app.directive('vrAnalyticReportsearchsettingsGenericsearch', ReportsearchsettingsGenericsearch);

})(app);