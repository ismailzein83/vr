'use strict';

app.directive('vrCommonTabtilesDefinition', ['UtilsService', 'VRUIUtilsService', 'VRCommon_VRTileService',
    function (UtilsService, VRUIUtilsService, VRCommon_VRTileService) {

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
            templateUrl: "/Client/Modules/Common/Directives/TabTiles/Templates/TabTilesDefinitionTemplate.html"
        };

        function TabTilesEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one tab tile.";
                };
                ctrl.addTabTile = function () {
                    var onTabTileAdded = function (tabTile) {
                        ctrl.datasource.push({
                            Name : tabTile.Name,
                            Tiles: [tabTile]
                        });
                    };
                    var disableColumnWidth = true;
                    VRCommon_VRTileService.addVRTile(onTabTileAdded, ctrl.datasource, disableColumnWidth);
                };
                ctrl.removeTabTile = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var tabTiles;
                    if (payload != undefined && payload.tileExtendedSettings != undefined) {
                        tabTiles = payload.tileExtendedSettings.TabTiles;
                    }
                    if (tabTiles != undefined) {
                        for (var i = 0; i < tabTiles.length; i++) {
                            var tabTile = tabTiles[i];
                            ctrl.datasource.push(tabTile);
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    var tabTiles = [];
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var tabTile = ctrl.datasource[i];
                            tabTiles.push(tabTile);
                        }
                    }
                    return {
                        $type: 'Vanrise.Common.MainExtensions.VRTile.TabsTiles,Vanrise.Common.MainExtensions',
                        TabTiles: tabTiles,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);


            }
            function defineMenuActions() {
                var defaultMenuActions = [{
                    name: "Edit",
                    clicked: editTabTile
                }];
                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editTabTile(tabTileObj) {
                 var onTabTileUpdated = function (tabTile) {
                    var index = ctrl.datasource.indexOf(tabTileObj);
                    var tabtile = {
                        Name: tabTileObj.Name,
                        Tiles: [tabTile]
                    };
                     ctrl.datasource[index] = tabtile; 
                };
                var disableColumnWidth = true;
                var obj = tabTileObj.Tiles[0];
                VRCommon_VRTileService.editVRTile(obj, onTabTileUpdated, ctrl.datasource, disableColumnWidth);
            }
        }
    }]);