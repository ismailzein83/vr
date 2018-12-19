(function (appControllers) {

    "use strict";

    CompositeRecordConditionDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function CompositeRecordConditionDefinitionEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var compositeRecordConditionDefinition;
        var conditionRecordNames;
        var conditionRecordTitles;

        var compositeRecordConditionDefinitionSettingsDirectiveAPI;
        var compositeRecordConditionDefinitionSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                compositeRecordConditionDefinition = parameters.CompositeRecordConditionDefinition;
                conditionRecordNames = parameters.conditionRecordNames;
                conditionRecordTitles = parameters.conditionRecordTitles;
            }

            isEditMode = (compositeRecordConditionDefinition != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onCompositeRecordConditionDefinitionSettingsDirectiveReady = function (api) {
                compositeRecordConditionDefinitionSettingsDirectiveAPI = api;
                compositeRecordConditionDefinitionSettingsDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.isNameValid = function () {
                var conditionRecordName = $scope.scopeModel.name != undefined ? $scope.scopeModel.name.toLowerCase() : undefined;
                if (isEditMode && conditionRecordName == compositeRecordConditionDefinition.Name.toLowerCase())
                    return null;

                for (var i = 0; i < conditionRecordNames.length; i++) {
                    if (conditionRecordName == conditionRecordNames[i].toLowerCase())
                        return 'Name already exists';
                }
                return null;
            };

            $scope.scopeModel.isTitleValid = function () {
                var conditionRecordTitle = $scope.scopeModel.title != undefined ? $scope.scopeModel.title.toLowerCase() : undefined;
                if (isEditMode && conditionRecordTitle == compositeRecordConditionDefinition.Title.toLowerCase())
                    return null;

                for (var i = 0; i < conditionRecordTitles.length; i++) {
                    if (conditionRecordTitle == conditionRecordTitles[i].toLowerCase())
                        return 'Title already exists';
                }
                return null;
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

            function setTitle() {
                $scope.title =
                    isEditMode ? UtilsService.buildTitleForUpdateEditor(compositeRecordConditionDefinition.Name, 'Composite Record Condition') : UtilsService.buildTitleForAddEditor('Composite Record Condition');
            }
            function loadStaticData() {
                if (compositeRecordConditionDefinition != undefined) {
                    $scope.scopeModel.name = compositeRecordConditionDefinition.Name;
                    $scope.scopeModel.title = compositeRecordConditionDefinition.Title;
                }
            }
            function loadCompositeRecordConditionDefinitionSettingsDirective() {
                var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                compositeRecordConditionDefinitionSettingsDirectiveReadyDeferred.promise.then(function () {

                    var payload;
                    if (compositeRecordConditionDefinition != undefined) {
                        payload = {
                            compositeRecordConditionDefinitionSetting: compositeRecordConditionDefinition.Settings,
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(compositeRecordConditionDefinitionSettingsDirectiveAPI, payload, loadDirectivePromiseDeferred);
                });

                return loadDirectivePromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCompositeRecordConditionDefinitionSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
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
                Name: $scope.scopeModel.name,
                Title: $scope.scopeModel.title,
                Settings: compositeRecordConditionDefinitionSettingsDirectiveAPI.getData()
            };
        }
    }

    appControllers.controller('VR_GenericData_CompositeRecordConditionDefinitionEditorController', CompositeRecordConditionDefinitionEditorController);
})(appControllers);
