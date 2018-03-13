"use strict";
app.directive("demoModuleAdvancedDimensions", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var advancedDimensions = new AdvancedDimensions($scope, ctrl, $attrs);
                advancedDimensions.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_Module/Directives/Setting/Dimensions/Templates/AdvancedDimensionsTemplate.html"
        };
        function AdvancedDimensions($scope, ctrl, $attrs) {

            var gridAPI;
            var gridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel = {};
                defineAPI();

                $scope.scopeModel.onAdvancedGridReady = function (api) {
                   
                    gridAPI = api;
                    gridReadyPromiseDeferred.resolve();
                }
            };
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined && payload != null) {
                        $scope.scopeModel.width = payload.Width != null ? payload.Width : null;
                        $scope.scopeModel.length = payload.Length != null ? payload.Length : null;
                        $scope.scopeModel.height = payload.Height != null ? payload.Height : null;
                    }
                    var loadGridPromise = loadAdvancedGrid();

                    var promises = [];
                    promises.push(loadGridPromise);

                    function loadAdvancedGrid() {
                       
                        var gridLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        gridReadyPromiseDeferred.promise.then(function () {
                            
                            var gridPayload;                    
                            if (payload != undefined) {
                                gridPayload = (payload.GridItems != undefined) ? payload.GridItems : undefined;                                
                            }
                            VRUIUtilsService.callDirectiveLoad(gridAPI, gridPayload, gridLoadPromiseDeferred);
                        });
                        return gridLoadPromiseDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };
                api.getData = function () {
                    return {
                        $type: "Demo.Module.Entities.Advanced,Demo.Module.Entities",
                        Width: $scope.scopeModel.width,
                        Length: $scope.scopeModel.length,
                        Height: $scope.scopeModel.height,
                        GridItems: gridAPI.getData()
                    };
                };
                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                    
                }
            }
        }
        return directiveDefinitionObject;
    }
]);