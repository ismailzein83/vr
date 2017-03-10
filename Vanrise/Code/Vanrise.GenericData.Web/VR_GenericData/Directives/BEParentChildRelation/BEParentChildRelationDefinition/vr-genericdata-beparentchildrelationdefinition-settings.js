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

            var parentBEDefinitionSelectorAPI;
            var parentBEDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var childBEDefinitionSelectorAPI;
            var childBEDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var viewPermissionAPI;
            var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var addPermissionAPI;
            var addPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onParentBEDefinitionSelectorReady = function (api) {
                    parentBEDefinitionSelectorAPI = api;
                    parentBEDefinitionSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onChildBEDefinitionSelectorReady = function (api) {
                    childBEDefinitionSelectorAPI = api;
                    childBEDefinitionSelectorReadyDeferred.resolve();
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

                    //Loading ParentBEDefinition selector
                    var parentBEDefinitionSelectorLoadPromise = getParentBEDefinitionSelectorLoadPromise();
                    promises.push(parentBEDefinitionSelectorLoadPromise);

                    //Loading ChildBEDefinition selector
                    var childBEDefinitionSelectorLoadPromise = getChildBEDefinitionSelectorLoadPromise();
                    promises.push(childBEDefinitionSelectorLoadPromise);

                    var loadViewRequiredPermissionPromise = loadViewRequiredPermission();
                    promises.push(loadViewRequiredPermissionPromise);

                    var loadAddRequiredPermissionPromise = loadAddRequiredPermission();
                    promises.push(loadAddRequiredPermissionPromise);

                    //Loading ChildFilterFQTN
                    $scope.scopeModel.childFilterFQTN = beParentChildRelationDefinitionSettings ? beParentChildRelationDefinitionSettings.ChildFilterFQTN : undefined;


                    function getParentBEDefinitionSelectorLoadPromise() {
                        var parentBEDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        parentBEDefinitionSelectorReadyDeferred.promise.then(function () {
                            var payload = {
                                selectedIds: beParentChildRelationDefinitionSettings != undefined ? beParentChildRelationDefinitionSettings.ParentBEDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(parentBEDefinitionSelectorAPI, payload, parentBEDefinitionSelectorLoadDeferred);
                        });

                        return parentBEDefinitionSelectorLoadDeferred.promise;
                    }
                    function getChildBEDefinitionSelectorLoadPromise() {
                        var childBEDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        childBEDefinitionSelectorReadyDeferred.promise.then(function () {
                            var payload = {
                                selectedIds: beParentChildRelationDefinitionSettings != undefined ? beParentChildRelationDefinitionSettings.ChildBEDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(childBEDefinitionSelectorAPI, payload, childBEDefinitionSelectorLoadDeferred);
                        });

                        return childBEDefinitionSelectorLoadDeferred.promise;
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

                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "Vanrise.GenericData.Entities.BEParentChildRelationDefinitionSettings, Vanrise.GenericData.Entities",
                            ParentBEDefinitionId: parentBEDefinitionSelectorAPI.getSelectedIds(),
                            ChildBEDefinitionId: childBEDefinitionSelectorAPI.getSelectedIds(),
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
