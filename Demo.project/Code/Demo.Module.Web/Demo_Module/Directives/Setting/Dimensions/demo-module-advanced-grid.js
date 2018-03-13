"use strict";
app.directive("demoModuleAdvancedGrid", ["UtilsService", "VRNotificationService", "Demo_Module_AdvancedGridService",
    function (UtilsService, VRNotificationService, Demo_Module_AdvancedGridService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
           
                var ctrl = this;
                var advancedGrid = new AdvancedGrid($scope, ctrl, $attrs);
                advancedGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_Module/Directives/Setting/Dimensions/Templates/AdvancedDimensionsGridTemplate.html"
        };
        function AdvancedGrid($scope, ctrl, $attrs) {

            var gridAPI;
            //var context;
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [];
                
                ctrl.addGridItem = function () {                  
                    var onGridItemAdded = function (gridItem) {
                        
                        ctrl.datasource.push(gridItem);

                    };
                    Demo_Module_AdvancedGridService.addGridItem(onGridItemAdded, undefined);
                };
                ctrl.removeGridItem = function (gridItem) {
                    var index = ctrl.datasource.indexOf(gridItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            };

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editGridItem
                }];

                $scope.gridMenuActions = function (gridItem) {
                    return defaultMenuActions;
                };
            };
            function editGridItem(gridObject) {
                var onGridItemUpdated = function (gridItem) {
                    var index = ctrl.datasource.indexOf(gridObject);
                    ctrl.datasource[index] = gridItem;
                };
                Demo_Module_AdvancedGridService.editGridItem(gridObject, onGridItemUpdated, undefined);
            };
            function defineAPI() {
                var api = {};
                api.getData = function () {
                    var gridItems;
                    if (ctrl.datasource != undefined && ctrl.datasource != null) {
                        gridItems = [];

                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            gridItems.push(currentItem);
                        }
                    }
                    return gridItems;
                };
                api.load = function (payload) {
                  
                    if (payload != undefined) {                       
                       // context = payload.context;
                        
                        for (var i = 0; i < payload.length; i++) {
                            var gridItem = payload[i];
                            ctrl.datasource.push( gridItem );
                        }
                        
                    }
                };

                if (ctrl.onReady != null) {
                  
                    ctrl.onReady(api);
                }
            };
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            };
        };
        return directiveDefinitionObject;
    }]);