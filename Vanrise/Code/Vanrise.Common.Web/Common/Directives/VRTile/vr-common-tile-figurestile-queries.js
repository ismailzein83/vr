//'use strict';

//app.directive('vrCommonTileFigurestileQueries', ['UtilsService', 'VRUIUtilsService', 'VRCommon_VRTileService',
//    function (UtilsService, VRUIUtilsService, VRCommon_VRTileService) {

//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new FigurestileQueries(ctrl, $scope, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            compile: function (element, attrs) {

//            },
//            templateUrl: '/Client/Modules/Common/Directives/VRTile/Templates/FiguresTileQueriesTemplate.html'
//        };

//        function FigurestileQueries(ctrl, $scope, $attrs) {
//            this.initializeController = initializeController;

//            var gridAPI;

//            function initializeController() {
//                ctrl.datasource = [];
//                ctrl.isValid = function () {
                    
//                };
//                ctrl.addQuery = function () {
                   
//                };
//                ctrl.removeQuery = function (dataItem) {
                
//                };
//                defineMenuActions();
//                defineAPI();
//            }
//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var promises = [];

                    
//                    return UtilsService.waitMultiplePromises(promises);

//                };

//                api.getData = function () {
                  
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);


//            }
//            function defineMenuActions() {
//                var defaultMenuActions = [{
//                    name: "Edit",
//                    clicked: editFigureTileQuery
//                }];
//                $scope.gridMenuActions = function (dataItem) {
//                    return defaultMenuActions;
//                };
//            }

//            function editFigureTileQuery(obj) {
//                var onVRTileUpdated = function (figureTileQuery) {
//                    var index = ctrl.datasource.indexOf(obj);
//                    ctrl.datasource[index] = { Entity: figureTileQuery };
//                };
//                VRCommon_VRTileService.editVRTile(vrTileObj.Entity, onVRTileUpdated, ctrl.datasource);
//            }
//        }
//    }]);