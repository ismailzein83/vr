(function (appControllers) {

    "use strict";

    CompositeRecordConditionDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function CompositeRecordConditionDefinitionEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var compositeRecordConditionDefinition;

        var compositeRecordConditionDefinitionSettingsDirectiveAPI;
        var compositeRecordConditionDefinitionSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                compositeRecordConditionDefinition = parameters.compositeRecordConditionDefinition;
            }

            isEditMode = (compositeRecordConditionDefinition != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onCompositeRecordConditionDefinitionSettingsDirectiveReady = function (api) {
                compositeRecordConditionDefinitionSettingsDirectiveAPI = api;
                compositeRecordConditionDefinitionSettingsDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return updateCompositeRecordConditionDefinition();
                }
                else {
                    return insertCompositeRecordConditionDefinition();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCompositeRecordConditionDefinitionSettingsDirective])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }
        function setTitle() {

            $scope.title =
                isEditMode ? UtilsService.buildTitleForUpdateEditor(compositeRecordConditionDefinition.Name, 'Composite Record Condition') : UtilsService.buildTitleForAddEditor('Composite Record Condition');
        }
        function loadStaticData() {
            if (compositeRecordConditionDefinition != undefined)
                $scope.scopeModel.name = compositeRecordConditionDefinition.Name;
        }
        function loadCompositeRecordConditionDefinitionSettingsDirective() {
            var loadCompositeRecordConditionDefinitionSettingsDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            compositeRecordConditionDefinitionSettingsDirectiveReadyDeferred.promise.then(function () {
                var compositeRecordConditionDefinitionSettingsDirectivePayload;
                if (compositeRecordConditionDefinition != undefined) {
                    console.log(compositeRecordConditionDefinition);
                    compositeRecordConditionDefinitionSettingsDirectivePayload = {
                        compositeRecordConditionDefinitionSetting: compositeRecordConditionDefinition,
                        compositeRecordConditionDefinitionSettingId: compositeRecordConditionDefinition.CompositeRecordConditionDefinitionId
                    };
                }

                VRUIUtilsService.callDirectiveLoad(compositeRecordConditionDefinitionSettingsDirectiveAPI, compositeRecordConditionDefinitionSettingsDirectivePayload, loadCompositeRecordConditionDefinitionSettingsDirectivePromiseDeferred);
            });

            return loadCompositeRecordConditionDefinitionSettingsDirectivePromiseDeferred.promise;
        }

        function insertCompositeRecordConditionDefinition() {
            var compositeRecordConditionDefinitionObj = buildCompositeRecordConditionDefinitionObjFromScope();

            if ($scope.onCompositeRecordConditionDefinitionAdded != undefined)
                $scope.onCompositeRecordConditionDefinitionAdded(compositeRecordConditionDefinitionObj);

            $scope.modalContext.closeModal();
        }
        function updateCompositeRecordConditionDefinition() {
            var compositeRecordConditionDefinitionObj = buildCompositeRecordConditionDefinitionObjFromScope();

            if ($scope.onCompositeRecordConditionDefinitionUpdated != undefined)
                $scope.onCompositeRecordConditionDefinitionUpdated(compositeRecordConditionDefinitionObj);

            $scope.modalContext.closeModal();
        }
        function buildCompositeRecordConditionDefinitionObjFromScope() {
            return {
                CompositeRecordConditionDefinitionId: compositeRecordConditionDefinition != undefined ? compositeRecordConditionDefinition.CompositeRecordConditionDefinitionId : UtilsService.guid(),
                Name: $scope.scopeModel.name,
                Settings: compositeRecordConditionDefinitionSettingsDirectiveAPI.getData()
            };
        }
    }

    appControllers.controller('VR_GenericData_CompositeRecordConditionDefinitionEditorController', CompositeRecordConditionDefinitionEditorController);
})(appControllers);
