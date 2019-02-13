'use strict';

app.directive('vrCommonTabtilesRuntime', ['UtilsService', 'VRUIUtilsService', 'VRCommon_VRTileService','ColumnWidthEnum',
    function (UtilsService, VRUIUtilsService, VRCommon_VRTileService, ColumnWidthEnum) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TabTilesEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/TabTiles/Templates/TabTilesRuntimeTemplate.html"
        };

        function TabTilesEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var tiles;
            var definitionSettings;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.tiles = [];
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        definitionSettings = payload.definitionSettings;
                        var tabTiles = definitionSettings.TabTiles;
                        if (tabTiles != undefined && tabTiles.length > 0) {
                            for (var i = 0; i < tabTiles.length; i++) {
                                var tabTile = tabTiles[i];
                                if (tabTile != undefined && tabTile.Tiles != undefined && tabTile.Tiles.length > 0) {
                                    var tile = tabTile.Tiles[0];
                                    addTile(tile);
                                }
                            }
                        }
                    }
                    function addTile(tileEntity) {
                        var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        var columnWidthObj = UtilsService.getEnum(ColumnWidthEnum, "value", tileEntity.Settings.NumberOfColumns);
                        var tile = {
                            name: tileEntity.Name,
                            runtimeEditor: tileEntity.Settings.ExtendedSettings.RuntimeEditor,
                            columnWidth: columnWidthObj != undefined ? columnWidthObj.numberOfColumns : undefined,
                            showTitle: tileEntity.ShowTitle
                        };
                        tile.onVRTileDirectiveReady = function (api) {
                            tile.tileAPI = api;
                            tile.isDirectiveLoading = true;
                            
                            tile.isDirectiveLoading = true;
                            var payload = { definitionSettings: tileEntity.Settings.ExtendedSettings };
                            VRUIUtilsService.callDirectiveLoad(tile.tileAPI, payload, loadDirectivePromiseDeferred);
                            loadDirectivePromiseDeferred.promise.then(function () {
                                tile.isDirectiveLoading = false;
                            });
                        };
                        $scope.scopeModel.tiles.push(tile);
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                };
                
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);