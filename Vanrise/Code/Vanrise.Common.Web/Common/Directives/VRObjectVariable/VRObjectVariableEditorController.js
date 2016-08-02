﻿(function (appControllers) {

    'use strict';

    VRObjectVariableEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function VRObjectVariableEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var objectVariableEntity;

        var objectTypeSelectiveAPI;
        var objectTypeSelectiveReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                objectVariableEntity = parameters.objectVariable;
            }

            isEditMode = (objectVariableEntity != undefined);
        }
        function defineScope() {

            $scope.scopeModal = {};

            $scope.scopeModal.onObjectTypeSelectiveReady = function (api) {
                objectTypeSelectiveAPI = api;
                objectTypeSelectiveReadyDeferred.resolve();
            };

            $scope.scopeModal.save = function () {
                if (isEditMode)
                    return update();
                else
                    return insert();
            };
            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.isLoading = true;

            //if (isEditMode) {
            //    setObjectVariableEntityFromParameters().then(function () {
            //        loadAllControls().finally(function () {
            //            genericRuleDefinitionEntity = undefined;
            //        });
            //    });
            //}
            //else {
            //    loadAllControls();
            //}

            loadAllControls();
        }

        function setObjectVariableEntityFromParameters() {
            objectVariableEntity = UtilsService.getItemByVal(vrObjectVariableCollection, objectName, 'ObjectName');

            var deferred = UtilsService.createPromiseDeferred();
            deferred.resolve();
            return deferred.promise;
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadObjectTypeSelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((objectVariableEntity != undefined) ? objectVariableEntity.FieldName : null, 'Object Variable') :
                    UtilsService.buildTitleForAddEditor('Object Variable');
            }
            function loadStaticData() {
                if (objectVariableEntity == undefined) {
                    return;
                }
                $scope.scopeModal.objectName = objectVariableEntity.ObjectName;
            }
            function loadObjectTypeSelective() {
                var objectTypeSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                objectTypeSelectiveReadyDeferred.promise.then(function () {
                    var payload;
                    if (objectVariableEntity != undefined) {
                        payload = objectVariableEntity.ObjectType;
                    }

                    VRUIUtilsService.callDirectiveLoad(objectTypeSelectiveAPI, payload, objectTypeSelectiveLoadDeferred);
                });

                return objectTypeSelectiveLoadDeferred.promise;
            }
        }

        function insert() {
            var objectVariable = buildObjectVariableFromScope();
            if ($scope.onObjectVariableAdded != undefined && typeof($scope.onObjectVariableAdded) == 'function') {
                $scope.onObjectVariableAdded(objectVariable);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var objectVariable = buildObjectVariableFromScope();
            if ($scope.onObjectVariableUpdated != undefined && typeof ($scope.onObjectVariableUpdated) == 'function') {
                $scope.onObjectVariableUpdated(objectVariable);
            }
            $scope.modalContext.closeModal();
        }

        function buildObjectVariableFromScope() {
            return {
                ObjectName: $scope.scopeModal.objectName,
                ObjectType: objectTypeSelectiveAPI.getData(),
            };
        }
    }

    appControllers.controller('VRCommon_VRObjectVariableEditorController', VRObjectVariableEditorController);

})(appControllers);
