﻿(function (appControllers) {

    "use strict";

    VRObjectTypeDefinitionEditorController.$inject = ['$scope', 'VRCommon_VRObjectTypeDefinitionAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function VRObjectTypeDefinitionEditorController($scope, VRCommon_VRObjectTypeDefinitionAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var isEditMode;

        var context;

        var vrObjectTypeDefinitionId;
        var vrObjectTypeDefinitionEntity;

        var objectTypeSelectiveAPI;
        var objectTypeSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var propertyDirectiveAPI;
        var propertyDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                vrObjectTypeDefinitionId = parameters.vrObjectTypeDefinitionId;
            }

            isEditMode = (vrObjectTypeDefinitionId != undefined);
        }
        function defineScope() {
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

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getVRObjectTypeDefinition().then(function () {
                        loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getVRObjectTypeDefinition() {
            return VRCommon_VRObjectTypeDefinitionAPIService.GetVRObjectTypeDefinition(vrObjectTypeDefinitionId).then(function (response) {
                vrObjectTypeDefinitionEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadObjectTypeSelective]).then(function () {
                loadPropertyDirective().then(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var vrObjectTypeDefinitionName = (vrObjectTypeDefinitionEntity != undefined) ? vrObjectTypeDefinitionEntity.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vrObjectTypeDefinitionName, 'Object Type Definition');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Object Type Definition');
                }
            }
            function loadStaticData() {
                if (vrObjectTypeDefinitionEntity == undefined)
                    return;
                $scope.scopeModel.name = vrObjectTypeDefinitionEntity.Name;
            }
            function loadObjectTypeSelective() {
                var objectTypeSelectiveLoadDeferred = UtilsService.createPromiseDeferred();
                objectTypeSelectiveReadyDeferred.promise.then(function () {
                    var payload = {};
                    payload.context = buildObjectTypeContext();
                    if (vrObjectTypeDefinitionEntity != undefined) {
                        payload.objectType = vrObjectTypeDefinitionEntity.Settings.ObjectType;
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
                    if (vrObjectTypeDefinitionEntity != undefined && vrObjectTypeDefinitionEntity.Settings != undefined) {
                        payload.properties = vrObjectTypeDefinitionEntity.Settings.Properties;
                    }

                    VRUIUtilsService.callDirectiveLoad(propertyDirectiveAPI, payload, propertyDirectiveLoadDeferred);
                });

                return propertyDirectiveLoadDeferred.promise;
            }
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return VRCommon_VRObjectTypeDefinitionAPIService.AddVRObjectTypeDefinition(buildVRObjectTypeDefinitionObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('VRObjectTypeDefinition', response, 'Name')) {
                    if ($scope.onVRObjectTypeDefinitionAdded != undefined)
                        $scope.onVRObjectTypeDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function update() {
            $scope.scopeModel.isLoading = true;
            return VRCommon_VRObjectTypeDefinitionAPIService.UpdateVRObjectTypeDefinition(buildVRObjectTypeDefinitionObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('VRObjectTypeDefinition', response, 'Name')) {
                    if ($scope.onVRObjectTypeDefinitionUpdated != undefined) {
                        $scope.onVRObjectTypeDefinitionUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
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
        function buildVRObjectTypeDefinitionObjFromScope() {

            return {
                VRObjectTypeDefinitionId: vrObjectTypeDefinitionEntity != undefined ? vrObjectTypeDefinitionEntity.VRObjectTypeDefinitionId : undefined,
                Name: $scope.scopeModel.name,
                Settings: {
                    ObjectType: objectTypeSelectiveAPI.getData(),
                    Properties: propertyDirectiveAPI.getData()
                }
            };
        }
    }

    appControllers.controller('VRCommon_VRObjectTypeDefinitionEditorController', VRObjectTypeDefinitionEditorController);

})(appControllers);