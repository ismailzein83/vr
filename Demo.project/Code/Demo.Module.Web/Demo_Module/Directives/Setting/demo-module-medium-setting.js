"use strict";
app.directive("demoModuleMediumSetting", ["UtilsService", "VRUIUtilsService",
function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var mediumSetting = new MediumSetting($scope, ctrl, $attrs);
                mediumSetting.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_Module/Directives/Setting/Templates/MediumSettingTemplate.html"
        };
        function MediumSetting($scope, ctrl, $attrs) {

            var dimensionsDirectiveAPI;
            var dimensionsDirectiveReadyDeferred= UtilsService.createPromiseDeferred();

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
                    var mediumSettingEntity;
                    
                    if (payload != undefined && payload != null) {
                       
                        mediumSettingEntity = payload;
                        $scope.scopeModel.size = mediumSettingEntity.Size != undefined ? mediumSettingEntity.Size : null;
                        $scope.scopeModel.location = mediumSettingEntity.Location != undefined ? mediumSettingEntity.Location : null;
                        $scope.scopeModel.employees = mediumSettingEntity.Employees != undefined ? mediumSettingEntity.Employees : null;
                        
                    }

                    function loadDirective() {

                        var dimensionsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        dimensionsDirectiveReadyDeferred.promise.then(function () {
                            
                          
                            var directivePayload = mediumSettingEntity != undefined ? mediumSettingEntity.Dimensions : undefined;
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
                        $type: "Demo.Module.Entities.MediumBranch,Demo.Module.Entities",
                        Size: $scope.scopeModel.size,
                        Location: $scope.scopeModel.location,
                        Employees: $scope.scopeModel.employees,
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