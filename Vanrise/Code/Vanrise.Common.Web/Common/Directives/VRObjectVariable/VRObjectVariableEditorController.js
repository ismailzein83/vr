(function (appControllers) {

    'use strict';

    VRObjectVariableEditorController.$inject = ['$scope', 'VR_GenericData_MappingRuleStructureBehaviorTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function VRObjectVariableEditorController($scope, VR_GenericData_MappingRuleStructureBehaviorTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var criteriaFieldName;
        var objectVariableEntity;
        //var criteriaFields; // criteriaFields are passed for validation

        var objectTypeSelectiveAPI;
        var objectTypeSelectiveReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                objectVariable = parameters.objectVariable;
                //vrObjectVariableCollection = UtilsService.cloneObject(parameters.VRObjectVariableCollection, true);
            }

            isEditMode = (objectVariable != undefined);
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

            $scope.validateCriteriaField = function () {
                var fieldName = ($scope.fieldName != undefined) ? $scope.fieldName.toUpperCase() : null;

                if (isEditMode && (fieldName == criteriaFieldName))
                    return null;
                else if (UtilsService.getItemIndexByVal(criteriaFields, fieldName, 'FieldName') > -1) {
                    return 'Another criteria field with the same name already exists';
                }
                return null;
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
                $type: "Vanrise.Entities.VRObject.VRObjectVariable, Vanrise.Entities",
                ObjectName: $scope.scopeModal.objectName,
                ObjectType: objectTypeSelectiveAPI.getData(),
            };
        }
    }

    appControllers.controller('VRCommon_VRObjectVariableEditorController', VRObjectVariableEditorController);

})(appControllers);