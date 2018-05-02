"use strict";

app.directive("vrCommonObjecttypedefinitionSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new ObjectTypeDefinitionSettings($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/Common/Directives/VRObjectTypeDefinition/Templates/VRObjectTypeDefinitionSettings.html'
    };

    function ObjectTypeDefinitionSettings($scope, ctrl) {
       
        var objectTypeSelectiveAPI;
        var objectTypeSelectiveReadyDeferred = UtilsService.createPromiseDeferred();
        var propertyDirectiveAPI;
        var propertyDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.onObjectTypeSelectiveReady = function (api) {
                objectTypeSelectiveAPI = api;
                objectTypeSelectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onObjectTypeSelectionChanged = function () {
                if (propertyDirectiveAPI != undefined) {

                    var setLoader = function (value) {
                        $scope.scopeModel.isLoading = value;
                    };
                    var payload = {};
                    payload.context = buildPropertyContext();
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, propertyDirectiveAPI, payload, setLoader);
                }
            };
            $scope.scopeModel.onPropertyDirectiveReady = function (api) {
                propertyDirectiveAPI = api;
                propertyDirectiveReadyDeferred.resolve();
            };
           
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            var vrObjectTypeDefinitionSettings;
            api.load = function (payload) {
                if (payload != undefined) {
                    vrObjectTypeDefinitionSettings = payload.vrObjectTypeDefinitionSettings;
                }
                function loadObjectTypeSelective() {
                    var objectTypeSelectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    objectTypeSelectiveReadyDeferred.promise.then(function () {
                        var payload = {};
                        payload.context = buildObjectTypeContext();
                        if (vrObjectTypeDefinitionSettings != undefined) {
                            payload.objectType = vrObjectTypeDefinitionSettings.ObjectType;
                        }
                        VRUIUtilsService.callDirectiveLoad(objectTypeSelectiveAPI, payload, objectTypeSelectiveLoadDeferred);
                    });
                    return objectTypeSelectiveLoadDeferred.promise;
                }
                function loadPropertyDirective() {
                    var propertyDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    propertyDirectiveReadyDeferred.promise.then(function () {
                        var payload = {};
                        payload.context = buildPropertyContext();
                        if (vrObjectTypeDefinitionSettings != undefined) {
                            payload.properties = vrObjectTypeDefinitionSettings.Properties;
                        }

                        VRUIUtilsService.callDirectiveLoad(propertyDirectiveAPI, payload, propertyDirectiveLoadDeferred);
                    });

                    return propertyDirectiveLoadDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadObjectTypeSelective]).then(function () { loadPropertyDirective().then(function () {
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });})
                  .catch(function (error) {
                      VRNotificationService.notifyExceptionWithClose(error, $scope);
                  }).finally(function () {

                  });

            };
            api.getData = function () {
                 var Settings= {
                        ObjectType: objectTypeSelectiveAPI.getData(),
                        Properties: propertyDirectiveAPI.getData()
                };
                 return Settings;
            };
            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
            
        }
        function buildPropertyContext() {
            var context = {
                getObjectType: function () { return objectTypeSelectiveAPI.getData(); }
            };
            return context;
        }
        function buildObjectTypeContext() {

            var context = {
                canDefineProperties: function (canDefineProperties) {
                    $scope.scopeModel.canDefineProperties = canDefineProperties;
                }
            };
            return context;
        }
    }




    return directiveDefinitionObject;

}]);