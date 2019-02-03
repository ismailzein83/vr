'use strict';

app.directive('vrCommonTileFigurestileQueries', ['UtilsService', 'VRUIUtilsService', 'VRCommon_VRTileService','VRCommon_VRTileAPIService',
    function (UtilsService, VRUIUtilsService, VRCommon_VRTileService, VRCommon_VRTileAPIService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FigurestileQueries(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/Common/Directives/VRTile/Templates/FiguresTileQueriesTemplate.html'
        };

        function FigurestileQueries(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

           

            function initializeController() {

                ctrl.itemsToDisplayDatasource = [];
                ctrl.itemsToDisplay = [];

                ctrl.datasource = [];
                ctrl.isValid = function () {

                };
                ctrl.onSelectedItemChange = function (value) {
                    console.log("value on select. change");
                    console.log(value);
                    if (value != undefined) {
                        var existingItem = UtilsService.getItemByVal(ctrl.itemsToDisplayDatasource, value.QueryId, 'QueryId');
                        if (existingItem != undefined && existingItem.Name == value.Name)
                            return;
                        ctrl.itemsToDisplayDatasource.push({
                            Name: value.Name,
                            Title: value.Title,
                            FiguresTileQueryId: value.QueryId,
                            QueryName : value.QueryName
                        });
                        console.log("ctrl.itemsToDisplayDatasource");
                        console.log(ctrl.itemsToDisplayDatasource);
                    }
                }
                ctrl.addQuery = function () {
                    var onFigureTileQueryAdded = function (query) {
                        console.log("figureTileQuery on add");
                        console.log(query)
                        ctrl.datasource.push(query);
                        return loadItemsToDisplaySelector();
                    }
                    VRCommon_VRTileService.addFiguresTileQuery(onFigureTileQueryAdded,ctrl.datasource);
                };

                ctrl.removeQuery = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                ctrl.removeItem = function (dataItem) {
                    var index = ctrl.itemsToDisplayDatasource.indexOf(dataItem);
                    ctrl.itemsToDisplayDatasource.splice(index, 1);
                };

                defineMenuActions();
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        var queries = payload.queries;
                        var itemsToDisplay = payload.itemsToDisplay;

                        for (var i = 0; i < queries.length; i++) {
                            var query = queries[i];
                            ctrl.datasource.push(query);
                        }
                        for (var i = 0; i < itemsToDisplay.length; i++) {
                            var item = itemsToDisplay[i];
                            ctrl.itemsToDisplayDatasource.push(item);
                        }
                    }
                    promises.push(loadItemsToDisplaySelector());
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    var queries = [];
                    var itemsToDisplay = [];
                    if (ctrl.datasource != undefined) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var query = {
                                FiguresTileQueryId: ctrl.datasource[i].FiguresTileQueryId,
                                Name: ctrl.datasource[i].Name,
                                Settings: ctrl.datasource[i].Settings
                            };
                            queries.push(query);
                        }
                    }
                    if (ctrl.itemsToDisplayDatasource != undefined) {
                        for (var i = 0; i < ctrl.itemsToDisplayDatasource.length; i++) {
                            var dataItem = ctrl.itemsToDisplayDatasource[i];
                            var item = {
                                FiguresTileQueryId: ctrl.itemsToDisplayDatasource[i].FiguresTileQueryId,
                                Name: ctrl.itemsToDisplayDatasource[i].Name,
                                Title: dataItem.Title,
                                QueryName: ctrl.itemsToDisplayDatasource[i].QueryName
                            };
                            itemsToDisplay.push(item);
                        }
                    }
                    console.log("itemsToDisplay");
                    console.log(itemsToDisplay);
                    return {
                        queries: queries,
                        itemsToDisplay: itemsToDisplay
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);


            }
            function defineMenuActions() {
                var defaultMenuActions = [{
                    name: "Edit",
                    clicked: editFigureTileQuery
                }];
                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editFigureTileQuery(obj) {
                console.log("edit obj")
                console.log(obj);
                var onFigureTileQueryUpdated = function (figureTileQuery) {
                    console.log("figureTileQuery on edit");
                    console.log(figureTileQuery)
                    var index = ctrl.datasource.indexOf(obj);
                    ctrl.datasource[index] = figureTileQuery;
                    return loadItemsToDisplaySelector();
                };
                VRCommon_VRTileService.editFiguresTileQuery(obj, onFigureTileQueryUpdated, ctrl.datasource);
            }
            function loadItemsToDisplaySelector() {
                ctrl.itemsToDisplay = [];
                var queryInput = {
                    Queries: ctrl.datasource
                };
                return VRCommon_VRTileAPIService.GetFiguresTileItemsToDiplayInfo(queryInput).then(function (response) {
                    if (response != undefined) {
                        var figureItems = response;
                        for (var i = 0; i < figureItems.length; i++) {
                            var figureItem = figureItems[i];
                            figureItem.displayName = figureItem.Name + " (" + figureItem.QueryName + ")";
                            ctrl.itemsToDisplay.push(figureItem);
                        }
                    }
                });
            }
        }
    }]);