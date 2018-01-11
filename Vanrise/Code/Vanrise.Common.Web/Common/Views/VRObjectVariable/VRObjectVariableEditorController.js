(function (appControllers) {

    'use strict';

    VRObjectVariableEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function VRObjectVariableEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var objectVariableEntity;
        var objectVariables; // objectVariables are passed for validation

        var objectTypeDefinitionSelectorAPI;
        var objectTypeDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                objectVariableEntity = parameters.objectVariable;
                objectVariables = parameters.objectVariables;
            }

            isEditMode = (objectVariableEntity != undefined);
        }
        function defineScope() {

            $scope.scopeModal = {};

            $scope.scopeModal.onObjectTypeDefinitionSelectorReady = function (api) {
                objectTypeDefinitionSelectorAPI = api;
                objectTypeDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.validateObjectVariables = function () {
                if ($scope.scopeModal.objectName == undefined || objectVariables == undefined)
                    return null;

                //for EditMode
                if (objectVariableEntity != undefined && objectVariableEntity.ObjectName == $scope.scopeModal.objectName)
                    return null;

                for (var i = 0; i < objectVariables.length; i++) {
                    var object = objectVariables[i];
                    if ($scope.scopeModal.objectName.toLowerCase() == object.ObjectName.toLowerCase())
                        return 'Same Objet Name Exists';
                }
                return null;
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
            loadAllControls();
        }

        function loadAllControls() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadObjectTypeDefinitionSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((objectVariableEntity != undefined) ? objectVariableEntity.ObjectName : null, 'Object Variable') :
                    UtilsService.buildTitleForAddEditor('Object Variable');
            }
            function loadStaticData() {
                if (objectVariableEntity == undefined) {
                    return;
                }
                $scope.scopeModal.objectName = objectVariableEntity.ObjectName;
            }
            function loadObjectTypeDefinitionSelector() {
                var objectTypeDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                objectTypeDefinitionSelectorReadyDeferred.promise.then(function () {
                    var payload;
                    if (objectVariableEntity != undefined) {
                        payload = { selectedIds: objectVariableEntity.VRObjectTypeDefinitionId };
                    }

                    VRUIUtilsService.callDirectiveLoad(objectTypeDefinitionSelectorAPI, payload, objectTypeDefinitionSelectorLoadDeferred);
                });

                return objectTypeDefinitionSelectorLoadDeferred.promise;
            }
        }

        function insert() {
            var objectVariable = buildObjectVariableFromScope();
            if ($scope.onObjectVariableAdded != undefined && typeof ($scope.onObjectVariableAdded) == 'function') {
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
                VRObjectTypeDefinitionId: objectTypeDefinitionSelectorAPI.getSelectedIds(),
            };
        }
    }

    appControllers.controller('VRCommon_VRObjectVariableEditorController', VRObjectVariableEditorController);

})(appControllers);
