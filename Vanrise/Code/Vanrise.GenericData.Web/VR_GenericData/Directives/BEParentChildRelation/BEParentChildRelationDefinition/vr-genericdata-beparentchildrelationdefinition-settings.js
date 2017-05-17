'use strict';

app.directive('vrGenericdataBeparentchildrelationdefinitionSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var beParentChildRelationDefinitionSettings = new BEParentChildRelationDefinitionSettings($scope, ctrl, $attrs);
                beParentChildRelationDefinitionSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BEParentChildRelation/BEParentChildRelationDefinition/Templates/BEParentChildRelationDefinitionSettingsTemplate.html'
        };

        function BEParentChildRelationDefinitionSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var parentBEDefinitionRuntimeSelectorSettingsAPI;
            var parentBEDefinitionRuntimeSelectorSettingsReadyDeferred = UtilsService.createPromiseDeferred();

            var childBEDefinitionRuntimeSelectorSettingsAPI;
            var childBEDefinitionRuntimeSelectorSettingsReadyDeferred = UtilsService.createPromiseDeferred();


            var viewPermissionAPI;
            var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var addPermissionAPI;
            var addPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onParentBEDefinitionRuntimeSelectorSettingsReady = function (api) {
                    parentBEDefinitionRuntimeSelectorSettingsAPI = api;
                    parentBEDefinitionRuntimeSelectorSettingsReadyDeferred.resolve();
                };
                $scope.scopeModel.onChildBEDefinitionRuntimeSelectorSettingsReady = function (api) {
                    childBEDefinitionRuntimeSelectorSettingsAPI = api;
                    childBEDefinitionRuntimeSelectorSettingsReadyDeferred.resolve();
                };

                $scope.scopeModel.onViewRequiredPermissionReady = function (api) {
                    viewPermissionAPI = api;
                    viewPermissionReadyDeferred.resolve();
                };

                $scope.scopeModel.onAddRequiredPermissionReady = function (api) {
                    addPermissionAPI = api;
                    addPermissionReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var beParentChildRelationDefinitionSettings;
                    var security;

                    if (payload != undefined) {
                        var beParentChildRelationDefinitionEntity = payload.componentType;

                        if (beParentChildRelationDefinitionEntity != undefined) {
                            $scope.scopeModel.name = beParentChildRelationDefinitionEntity.Name;
                            beParentChildRelationDefinitionSettings = beParentChildRelationDefinitionEntity.Settings;
                            security = beParentChildRelationDefinitionSettings && beParentChildRelationDefinitionSettings.Security || undefined;
                        }
                    }

                    //Loading ParentBEDefinitionRuntimeSelectorSettings selector
                    var parentBEDefinitionRuntimeSelectorSettingsLoadPromise = getParentBEDefinitionRuntimeSelectorSettingsLoadPromise();
                    promises.push(parentBEDefinitionRuntimeSelectorSettingsLoadPromise);

                    //Loading ChildBEDefinitionRuntimeSelectorSettings selector
                    var childBEDefinitionRuntimeSelectorSettingsLoadPromise = getChildBEDefinitionRuntimeSelectorSettingsLoadPromise();
                    promises.push(childBEDefinitionRuntimeSelectorSettingsLoadPromise);

                    //Loading ChildFilterFQTN
                    $scope.scopeModel.childFilterFQTN = beParentChildRelationDefinitionSettings ? beParentChildRelationDefinitionSettings.ChildFilterFQTN : undefined;

                    //Loading ViewRequiredPermission directive
                    var loadViewRequiredPermissionPromise = loadViewRequiredPermission();
                    promises.push(loadViewRequiredPermissionPromise);

                    //Loading AddRequiredPermission directive
                    var loadAddRequiredPermissionPromise = loadAddRequiredPermission();
                    promises.push(loadAddRequiredPermissionPromise);


                    function getParentBEDefinitionRuntimeSelectorSettingsLoadPromise() {
                        var parentBEDefinitionRuntimeSelectorSettingsLoadDeferred = UtilsService.createPromiseDeferred();

                        parentBEDefinitionRuntimeSelectorSettingsReadyDeferred.promise.then(function () {
                            var parentBEDefinitionRuntimeSelectorSettingsPayload = {
                                beDefinitionId: beParentChildRelationDefinitionSettings != undefined ? beParentChildRelationDefinitionSettings.ParentBEDefinitionId : undefined,
                                beRuntimeSelectorFilter: beParentChildRelationDefinitionSettings != undefined ? beParentChildRelationDefinitionSettings.ParentBERuntimeSelectorFilter : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(parentBEDefinitionRuntimeSelectorSettingsAPI, parentBEDefinitionRuntimeSelectorSettingsPayload, parentBEDefinitionRuntimeSelectorSettingsLoadDeferred);
                        });

                        return parentBEDefinitionRuntimeSelectorSettingsLoadDeferred.promise;
                    }
                    function getChildBEDefinitionRuntimeSelectorSettingsLoadPromise() {
                        var childBEDefinitionRuntimeSelectorSettingsLoadDeferred = UtilsService.createPromiseDeferred();

                        childBEDefinitionRuntimeSelectorSettingsReadyDeferred.promise.then(function () {
                            var childBEDefinitionRuntimeSelectorSettingsPayload = {
                                beDefinitionId: beParentChildRelationDefinitionSettings != undefined ? beParentChildRelationDefinitionSettings.ChildBEDefinitionId : undefined,
                                beRuntimeSelectorFilter: beParentChildRelationDefinitionSettings != undefined ? beParentChildRelationDefinitionSettings.ChildBERuntimeSelectorFilter : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(childBEDefinitionRuntimeSelectorSettingsAPI, childBEDefinitionRuntimeSelectorSettingsPayload, childBEDefinitionRuntimeSelectorSettingsLoadDeferred);
                        });

                        return childBEDefinitionRuntimeSelectorSettingsLoadDeferred.promise;
                    }

                    function loadViewRequiredPermission() {
                        var viewSettingPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        viewPermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: security && security.ViewRequiredPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(viewPermissionAPI, dataPayload, viewSettingPermissionLoadDeferred);
                        });
                        return viewSettingPermissionLoadDeferred.promise;
                    }
                    function loadAddRequiredPermission() {
                        var addPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        addPermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: security && security.AddRequiredPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(addPermissionAPI, dataPayload, addPermissionLoadDeferred);
                        });
                        return addPermissionLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var parentBEDefinitionId, parentBERuntimeSelectorFilter;
                    var parentBEDefinitionRuntimeSelectorSettings = parentBEDefinitionRuntimeSelectorSettingsAPI.getData();
                    if (parentBEDefinitionRuntimeSelectorSettings != undefined) {
                        parentBEDefinitionId = parentBEDefinitionRuntimeSelectorSettings.beDefinitionId;
                        parentBERuntimeSelectorFilter = parentBEDefinitionRuntimeSelectorSettings.beRuntimeSelectorFilter;
                    }

                    var childBEDefinitionId, childBERuntimeSelectorFilter;
                    var childBEDefinitionRuntimeSelectorSettings = childBEDefinitionRuntimeSelectorSettingsAPI.getData();
                    if (childBEDefinitionRuntimeSelectorSettings != undefined) {
                        childBEDefinitionId = childBEDefinitionRuntimeSelectorSettings.beDefinitionId;
                        childBERuntimeSelectorFilter = childBEDefinitionRuntimeSelectorSettings.beRuntimeSelectorFilter;
                    }

                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "Vanrise.GenericData.Entities.BEParentChildRelationDefinitionSettings, Vanrise.GenericData.Entities",
                            ParentBEDefinitionId: parentBEDefinitionId,
                            ParentBERuntimeSelectorFilter: parentBERuntimeSelectorFilter,
                            ChildBEDefinitionId: childBEDefinitionId,
                            ChildBERuntimeSelectorFilter: childBERuntimeSelectorFilter,
                            ChildFilterFQTN: $scope.scopeModel.childFilterFQTN,
                            Security: {
                                ViewRequiredPermission: viewPermissionAPI.getData(),
                                AddRequiredPermission: addPermissionAPI.getData()
                            }
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
