(function (app) {

    'use strict';

    ReportsearchsettingsGenericsearch.$inject = ["UtilsService", 'VRUIUtilsService'];

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
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/MainExtensions/AnalyticReport/History/SearchSettings/Templates/GenericSearchTemplate.html"

        };
        function Genericsearch($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var groupingDimensionSelectorAPI;
            var groupingDimensionReadyDeferred = UtilsService.createPromiseDeferred();

            var filterDimensionSelectorAPI;
            var filterDimensionReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.groupingDimensions = [];
                $scope.scopeModel.filterDimensions = [];

                $scope.scopeModel.onGroupingDimensionSelectorDirectiveReady = function (api) {
                    groupingDimensionSelectorAPI = api;
                    groupingDimensionReadyDeferred.resolve();
                };
                $scope.scopeModel.onFilterDimensionSelectorDirectiveReady = function (api) {
                    filterDimensionSelectorAPI = api;
                    filterDimensionReadyDeferred.resolve();
                };

                $scope.scopeModel.onSelectFilterDimensionItem = function (dimension) {
                    var dataItem = {
                        AnalyticItemConfigId: dimension.AnalyticItemConfigId,
                        Title: dimension.Title,
                        Name: dimension.Name,
                        IsRequired: false
                    };
                    $scope.scopeModel.filterDimensions.push(dataItem);
                };
                $scope.scopeModel.onDeselectFilterDimensionItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.filterDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.filterDimensions.splice(datasourceIndex, 1);
                };

                $scope.scopeModel.onSelectGroupingDimensionItem = function (dimension) {
                    var dataItem = {

                        Id: dimension.AnalyticItemConfigId,
                        Title: dimension.Title,
                        Name: dimension.Name,
                        IsSelected: false
                    };
                    $scope.scopeModel.groupingDimensions.push(dataItem);
                };
                $scope.scopeModel.onDeselectGroupingDimensionItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.groupingDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.groupingDimensions.splice(datasourceIndex, 1);
                };

                $scope.scopeModel.removeGroupingDimension = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedGroupingDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.selectedGroupingDimensions.splice(index, 1);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.groupingDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.groupingDimensions.splice(datasourceIndex, 1);
                };
                $scope.scopeModel.removeFilterDimension = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedFilterDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.selectedFilterDimensions.splice(index, 1);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.filterDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.filterDimensions.splice(datasourceIndex, 1);
                };
                $scope.scopeModel.isValidGroupingDimensions = function () {

                    if ($scope.scopeModel.groupingDimensions.length > 0 || !$scope.scopeModel.isRequiredGroupingDimensions)
                        return null;
                    return "At least one dimension should be selected.";
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.tableIds != undefined) {
                        var promises = [];

                        var selectedGroupingIds;
                        var selectedFilterIds;

                        var tableIds = payload.tableIds;

                        if (payload.searchSettings != undefined) {
                            $scope.scopeModel.isRequiredGroupingDimensions = payload.searchSettings.IsRequiredGroupingDimensions;
                            $scope.scopeModel.showCurrency = payload.searchSettings.ShowCurrency;
                            if (payload.searchSettings.GroupingDimensions != undefined && payload.searchSettings.GroupingDimensions.length > 0) {
                                selectedGroupingIds = [];
                                for (var i = 0 ; i < payload.searchSettings.GroupingDimensions.length; i++) {
                                    var groupingDimension = payload.searchSettings.GroupingDimensions[i];
                                    selectedGroupingIds.push(groupingDimension.DimensionName);
                                    $scope.scopeModel.groupingDimensions.push({
                                        Name: groupingDimension.DimensionName,
                                        Title: groupingDimension.DimensionName,
                                        IsSelected: groupingDimension.IsSelected
                                    });
                                }
                            }

                            if (payload.searchSettings.Filters != undefined && payload.searchSettings.Filters.length > 0) {

                                selectedFilterIds = [];
                                for (var i = 0 ; i < payload.searchSettings.Filters.length; i++) {
                                    var filterDimension = payload.searchSettings.Filters[i];
                                    selectedFilterIds.push(filterDimension.DimensionName);
                                    var dataItem = {
                                        Name: filterDimension.DimensionName,
                                        Title: filterDimension.Title,
                                        IsRequired: filterDimension.IsRequired,
                                    };
                                    $scope.scopeModel.filterDimensions.push(dataItem);
                                }
                            }

                            if (payload.searchSettings.Legends) {
                                $scope.scopeModel.legendHeader = payload.searchSettings.Legends[0].Header;
                                $scope.scopeModel.legendContent = payload.searchSettings.Legends[0].Content;
                            }
                        }

                        var loadGroupingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        groupingDimensionReadyDeferred.promise.then(function () {
                            var payloadGroupingDirective = {
                                filter: { TableIds: tableIds, HideIsRequiredFromParent: true },
                                selectedIds: selectedGroupingIds
                            };

                            VRUIUtilsService.callDirectiveLoad(groupingDimensionSelectorAPI, payloadGroupingDirective, loadGroupingDirectivePromiseDeferred);
                        });
                        promises.push(loadGroupingDirectivePromiseDeferred.promise);

                        var loadFilterDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        filterDimensionReadyDeferred.promise.then(function () {
                            var payloadFilterDirective = {
                                filter: { TableIds: tableIds, HideIsRequiredFromParent: true },
                                selectedIds: selectedFilterIds
                            };

                            VRUIUtilsService.callDirectiveLoad(filterDimensionSelectorAPI, payloadFilterDirective, loadFilterDirectivePromiseDeferred);
                        });
                        promises.push(loadFilterDirectivePromiseDeferred.promise);

                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function getData() {

                    var groupingDimensions;
                    if ($scope.scopeModel.groupingDimensions != undefined && $scope.scopeModel.groupingDimensions.length > 0) {
                        groupingDimensions = [];
                        for (var i = 0; i < $scope.scopeModel.groupingDimensions.length; i++) {
                            var groupingDimension = $scope.scopeModel.groupingDimensions[i];
                            groupingDimensions.push({
                                DimensionName: groupingDimension.Name,
                                IsSelected: groupingDimension.IsSelected
                            });
                        }
                    }

                    var filterDimensions;
                    if ($scope.scopeModel.filterDimensions != undefined && $scope.scopeModel.filterDimensions.length > 0) {
                        filterDimensions = [];
                        for (var i = 0; i < $scope.scopeModel.filterDimensions.length; i++) {
                            var filterDimension = $scope.scopeModel.filterDimensions[i];
                            filterDimensions.push({
                                DimensionName: filterDimension.Name,
                                Title: filterDimension.Title,
                                IsRequired: filterDimension.IsRequired,
                            });
                        }
                    }

                    var legends;
                    if ($scope.scopeModel.legendHeader != undefined && $scope.scopeModel.legendContent != undefined)
                        legends = [{ Header: $scope.scopeModel.legendHeader, Content: $scope.scopeModel.legendContent }];

                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.History.SearchSettings.GenericSearchSettings, Vanrise.Analytic.MainExtensions ",
                        GroupingDimensions: groupingDimensions,
                        Filters: filterDimensions,
                        Legends: legends,
                        IsRequiredGroupingDimensions: $scope.scopeModel.isRequiredGroupingDimensions,
                        ShowCurrency: $scope.scopeModel.showCurrency
                    };

                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticReportsearchsettingsGenericsearch', ReportsearchsettingsGenericsearch);

})(app);