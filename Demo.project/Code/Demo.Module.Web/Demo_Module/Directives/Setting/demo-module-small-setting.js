"use strict";
app.directive("demoModuleSmallSetting" , ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var smallSetting = new SmallSetting($scope, ctrl, $attrs);
                smallSetting.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_Module/Directives/Setting/Templates/SmallSettingTemplate.html"
        };
        function SmallSetting($scope, ctrl, $attrs) {

            var dimensionsDirectiveAPI;
            var dimensionsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();

                $scope.scopeModel.onDimensionsDirectiveReady = function (api) {
                    dimensionsDirectiveAPI = api;
                    dimensionsDirectiveReadyDeferred.resolve();
                    
                }
                                         
            };
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                   
                    var promises = [];
                    var smallSettingEntity;

                    if (payload != undefined && payload != null) {
                        
                        smallSettingEntity = payload;
                        $scope.scopeModel.size = smallSettingEntity.Size != undefined ? smallSettingEntity.Size : undefined;
                        $scope.scopeModel.location = smallSettingEntity.Location != undefined ? smallSettingEntity.Location : undefined;

                        
                    }

                function loadDirective() {

                    var dimensionsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    dimensionsDirectiveReadyDeferred.promise.then(function () {
                                              
                        var directivePayload = smallSettingEntity != undefined ? smallSettingEntity.Dimensions : undefined;
                        VRUIUtilsService.callDirectiveLoad(dimensionsDirectiveAPI, directivePayload, dimensionsDirectiveLoadDeferred);
                    });

                    return dimensionsDirectiveLoadDeferred.promise;
                }
                    var loadDirectivePromise = loadDirective();
                    promises.push(loadDirectivePromise);
                    return UtilsService.waitMultiplePromises(promises);
                };
        
                api.getData = function () {                 
                    
                    return {
                        $type: "Demo.Module.Entities.SmallBranch,Demo.Module.Entities",
                        Size: $scope.scopeModel.size,
                        Location: $scope.scopeModel.location,
                        Dimensions: dimensionsDirectiveAPI.getData()
                    };                   
                    
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
]);