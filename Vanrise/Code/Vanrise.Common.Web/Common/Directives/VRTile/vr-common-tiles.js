'use strict';

app.directive('vrCommonTiles', ['UtilsService', 'VRUIUtilsService', 'VRCommon_VRTileService',
    function (UtilsService, VRUIUtilsService, VRCommon_VRTileService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BankDetailsSettingsEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/VRTile/Templates/VRTilesGridTemplate.html"
        };

        function BankDetailsSettingsEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one tile.";
                };
                ctrl.addVRTile = function () {
                    var onVRTileAdded = function (vrTile) {
                        ctrl.datasource.push({ Entity: vrTile });
                    };
                    VRCommon_VRTileService.addVRTile(onVRTileAdded, ctrl.datasource);
                };
                ctrl.removeVRTile = function (dataItem) {
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

                    var vrTilesPayload;
                    if (payload != undefined && payload.tiles != undefined) {
                        vrTilesPayload = payload.tiles;
                    }
                    if (vrTilesPayload != undefined) {
                        for (var i = 0; i < vrTilesPayload.length; i++) {
                            var vrTile = vrTilesPayload[i];
                            ctrl.datasource.push({ Entity: vrTile });
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    var vrTiles;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        vrTiles = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            vrTiles.push(currentItem.Entity);
                        }
                    }
                    return vrTiles;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);


            }
            function defineMenuActions() {
                var defaultMenuActions = [{
                    name: "Edit",
                    clicked: editVRTile
                }];
                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editVRTile(vrTileObj) {
                var onVRTileUpdated = function (vrTile) {
                    var index = ctrl.datasource.indexOf(vrTileObj);
                    ctrl.datasource[index] = { Entity: vrTile };
                };
                VRCommon_VRTileService.editVRTile(vrTileObj.Entity, onVRTileUpdated, ctrl.datasource);
            }
        }
    }]);