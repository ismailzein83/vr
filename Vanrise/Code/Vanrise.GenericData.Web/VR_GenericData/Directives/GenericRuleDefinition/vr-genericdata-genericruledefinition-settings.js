"use strict";

app.directive("vrGenericdataGenericruledefinitionSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new GenericTRuleDefinitionSettings($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericRuleDefinition/Templates/GenericRuleDefinitionSettings.html'

    };

    function GenericTRuleDefinitionSettings($scope, ctrl) {
        var objectDirectiveAPI;
        var objectDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var criteriaDirectiveAPI;
        var criteriaDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var settingsDirectiveAPI;
        var settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var viewPermissionAPI;
        var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var addPermissionAPI;
        var addPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var editPermissionAPI;
        var editPermissionReadyDeferred = UtilsService.createPromiseDeferred();
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onObjectDirectiveReady = function (api) {
                objectDirectiveAPI = api;
                objectDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onCriteriaDirectiveReady = function (api) {
                criteriaDirectiveAPI = api;
                criteriaDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onSettingsDirectiveReady = function (api) {
                settingsDirectiveAPI = api;
                settingsDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onViewRequiredPermissionReady = function (api) {
                viewPermissionAPI = api;
                viewPermissionReadyDeferred.resolve();
            };
            $scope.scopeModel.onAddRequiredPermissionReady = function (api) {
                addPermissionAPI = api;
                addPermissionReadyDeferred.resolve();
            };
            $scope.scopeModel.onEditRequiredPermissionReady = function (api) {
                editPermissionAPI = api;
                editPermissionReadyDeferred.resolve();
            };
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            var objects;
            var criteriaDefinition;
            var settingsDefinition;
            var security;
            var settingsTypeName;
            api.load = function (payload) {
                var promises = [];
                if (payload != undefined) {
                    objects = payload.objects;
                    criteriaDefinition = payload.criteriaDefinition;
                    settingsTypeName = payload.settingsTypeName;
                    settingsDefinition = payload.settingsDefinition;
                    security = payload.security;
                }
            
                var loadObjectDirectivePromise = loadObjectDirective();
                promises.push(loadObjectDirectivePromise);
                function loadObjectDirective() {
                    var objectDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    
                    objectDirectiveReadyDeferred.promise.then(function () {
                        var objectDirectivePayload;

                        if (objects != null) {

                            objectDirectivePayload = {
                                objects: objects
                            };
                        }

                        VRUIUtilsService.callDirectiveLoad(objectDirectiveAPI, objectDirectivePayload, objectDirectiveLoadDeferred);
                    });
                  
                    return objectDirectiveLoadDeferred.promise;
                }

                var loadCriteriaDirectivePromise = loadCriteriaDirective();
                promises.push(loadCriteriaDirectivePromise);
                function loadCriteriaDirective() {
                    var criteriaDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    criteriaDirectiveReadyDeferred.promise.then(function () {
                        var criteriaDirectivePayload = { context: buildContext() };

                        if (criteriaDefinition != null) {
                            criteriaDirectivePayload.GenericRuleDefinitionCriteriaFields = criteriaDefinition.Fields
                        }

                        VRUIUtilsService.callDirectiveLoad(criteriaDirectiveAPI, criteriaDirectivePayload, criteriaDirectiveLoadDeferred);
                    });

                    return criteriaDirectiveLoadDeferred.promise;
                }

                var loadSettingsDirectivePromise = loadSettingsDirective();
                promises.push(loadSettingsDirectivePromise);
                function loadSettingsDirective() {
                    var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    settingsDirectiveReadyDeferred.promise.then(function () {
                        var payload;
                        if (settingsDefinition != null)
                            payload = settingsDefinition;
                        else if (settingsTypeName != undefined)
                            payload = { settingsTypeName: settingsTypeName };
                        VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, payload, settingsDirectiveLoadDeferred);
                    });

                    return settingsDirectiveLoadDeferred.promise;
                }

                var loadViewRequiredPermissionPromise = loadViewRequiredPermission();
                promises.push(loadViewRequiredPermissionPromise);
                function loadViewRequiredPermission() {
                    var viewPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                    viewPermissionReadyDeferred.promise.then(function () {
                        var payload;

                        if (security != undefined && security.ViewRequiredPermission != null) {
                            payload = {
                                data: security.ViewRequiredPermission
                            };
                        }

                        VRUIUtilsService.callDirectiveLoad(viewPermissionAPI, payload, viewPermissionLoadDeferred);
                    });

                    return viewPermissionLoadDeferred.promise;
                }

                var loadAddRequiredPermissionPromise = loadAddRequiredPermission();
                promises.push(loadAddRequiredPermissionPromise);
                function loadAddRequiredPermission() {
                    var addPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                    addPermissionReadyDeferred.promise.then(function () {
                        var payload;

                        if (security != undefined && security.AddRequiredPermission != null) {
                            payload = {
                                data: security.AddRequiredPermission
                            };
                        }

                        VRUIUtilsService.callDirectiveLoad(addPermissionAPI, payload, addPermissionLoadDeferred);
                    });

                    return addPermissionLoadDeferred.promise;
                }

                var loadEditRequiredPermissionPromise = loadEditRequiredPermission();
                promises.push(loadEditRequiredPermissionPromise);
                function loadEditRequiredPermission() {
                    var editPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                    editPermissionReadyDeferred.promise.then(function () {
                        var payload;

                        if (security != undefined && security.EditRequiredPermission != null) {
                            payload = {
                                data: security.EditRequiredPermission
                            };
                        }

                        VRUIUtilsService.callDirectiveLoad(editPermissionAPI, payload, editPermissionLoadDeferred);
                    });

                    return editPermissionLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
               
            };
            api.getData = function () {
                var genericRuleDefinitionSettings = {};
                genericRuleDefinitionSettings.criteriaDefinition = criteriaDirectiveAPI.getData();
                genericRuleDefinitionSettings.settingsDefinition = settingsDirectiveAPI.getData();
                genericRuleDefinitionSettings.objects = objectDirectiveAPI.getData();
                genericRuleDefinitionSettings.security = {
                    ViewRequiredPermission: viewPermissionAPI.getData(),
                    AddRequiredPermission: addPermissionAPI.getData(),
                    EditRequiredPermission: editPermissionAPI.getData()
                };
                return genericRuleDefinitionSettings;
            };
            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }

        function buildContext() {

            var context = {
                getObjectVariables: function () { return objectDirectiveAPI.getData(); }
            };
            return context;
        }
    }




    return directiveDefinitionObject;

}]);