'use strict';

app.directive('vrCommonTileFigurestileQueries', ['UtilsService', 'VRUIUtilsService', 'VRCommon_VRTileService', 'VRCommon_VRTileAPIService','VRNotificationService',
    function (UtilsService, VRUIUtilsService, VRCommon_VRTileService, VRCommon_VRTileAPIService, VRNotificationService) {

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
                    if (ctrl.datasource.length > 0)
                        return null;
                    else return "1 Item at least should be added";
                };
                ctrl.editFiguresTileQuery = function (obj) {
                    var onFigureTileQueryUpdated = function (figureTileQuery) {
                        var index = ctrl.datasource.indexOf(obj);
                        ctrl.datasource[index] = figureTileQuery;
                        var queryInput = {
                            Queries: ctrl.datasource
                        };
                        ctrl.itemsToDisplayLoading = true;
                        updateItemsToDisplayGridData().then(function () {
                            ctrl.itemsToDisplayLoading = false;
                        });
                    };
                    var clearItemsToDisplayDataSource = function () {
                        ctrl.itemsToDisplayDatasource = [];
                    };
                    VRCommon_VRTileService.editFiguresTileQuery(obj, onFigureTileQueryUpdated, ctrl.datasource, clearItemsToDisplayDataSource);
                };
                ctrl.onSelectedItemChange = function (value) {
                    if (value != undefined) {
                        var existingItem = UtilsService.getItemByVal(ctrl.itemsToDisplayDatasource, value.QueryId, 'QueryId');
                        if (existingItem != undefined && existingItem.Name == value.Name)
                            return;
                        ctrl.itemsToDisplayDatasource.push({
                            Name: value.Name,
                            Title: value.Title,
                            FiguresTileQueryId: value.QueryId,
                            QueryName: value.QueryName
                        });
                    }
                };
                ctrl.addQuery = function () {
                    var onFigureTileQueryAdded = function (query) {
                        if (query != undefined) {
                            ctrl.datasource.push(query);
                            var queryInput = {
                                Queries: ctrl.datasource
                            };
                            ctrl.itemsToDisplayLoading = true;
                            updateItemsToDisplayGridData().then(function () {
                                ctrl.itemsToDisplayLoading = false;
                            });
                        }
                    };
                    VRCommon_VRTileService.addFiguresTileQuery(onFigureTileQueryAdded, ctrl.datasource);
                };

                ctrl.removeQuery = function (dataItem) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = ctrl.datasource.indexOf(dataItem);
                            ctrl.datasource.splice(index, 1);
                            ctrl.itemsToDisplayLoading = true;
                            updateItemsToDisplayGridData().then(function () {
                                ctrl.itemsToDisplayLoading = false;
                            });
                        }
                    });

                };
                ctrl.removeItem = function (dataItem) {
                    var index = ctrl.itemsToDisplayDatasource.indexOf(dataItem);
                    ctrl.itemsToDisplayDatasource.splice(index, 1);
                };

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
                                    QueryName: ctrl.itemsToDisplayDatasource[i].QueryName,
                                    HideAtRuntime: dataItem.HideAtRuntime
                                };
                                itemsToDisplay.push(item);
                        }
                    }
                    return {
                        queries: queries,
                        itemsToDisplay: itemsToDisplay
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);


            }

            function loadItemsToDisplaySelector() {
                ctrl.itemsToDisplay = [];

            }
            function updateItemsToDisplayGridData() {
                ctrl.itemsToDisplayLoading = true;
                var queryInput = {
                    Queries: ctrl.datasource
                };
                
                return VRCommon_VRTileAPIService.GetQuerySchemaItems(queryInput).then(function (response) {
                    if (response != undefined) {
                        var figureItems = response;
                        if (figureItems != undefined && figureItems.length > 0) {
                            for (var i = 0; i < figureItems.length; i++) {
                                var figureItemExists = false;
                                var figureItem = figureItems[i];

                                var dataSourceItem = UtilsService.getItemByVal(ctrl.itemsToDisplayDatasource, figureItem.Name, 'Name');
                                if (dataSourceItem != undefined) {
                                    if (figureItem.QueryName == dataSourceItem.QueryName)
                                        figureItemExists = true;
                                }
                                if (!figureItemExists) {
                                    ctrl.itemsToDisplayDatasource.push({
                                        Name: figureItem.Name,
                                        Title: figureItem.Title,
                                        FiguresTileQueryId: figureItem.QueryId,
                                        QueryName: figureItem.QueryName
                                    });
                                }
                            }
                            if (ctrl.itemsToDisplayDatasource != undefined && ctrl.itemsToDisplayDatasource.length > 0) {
                                var updatedItemsToDiplay = [];
                                for (var j = 0; j < ctrl.itemsToDisplayDatasource.length; j++) {
                                    var itemTodisplay = ctrl.itemsToDisplayDatasource[j];
                                    if (itemTodisplay != undefined) {
                                        var figureItemValue = UtilsService.getItemByVal(figureItems, itemTodisplay.Name, 'Name');
                                        if (figureItemValue != null && figureItemValue.QueryName == itemTodisplay.QueryName ) {
                                            updatedItemsToDiplay.push(itemTodisplay);
                                        }
                                    }
                                }
                                ctrl.itemsToDisplayDatasource = updatedItemsToDiplay;
                            }
                        }
                        else {
                            ctrl.itemsToDisplayDatasource = [];
                        }

                    }
                });

            }
        }
    }]);