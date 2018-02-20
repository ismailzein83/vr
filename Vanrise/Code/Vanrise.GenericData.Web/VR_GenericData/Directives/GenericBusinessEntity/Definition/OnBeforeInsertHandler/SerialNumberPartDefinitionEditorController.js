(function (appControllers) {

    "use strict";

    SerialNumberPartDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function SerialNumberPartDefinitionEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var partDefinition;
        var context;

        var partSettingsAPI;
        var partSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                partDefinition = parameters.partDefinition;
                context = parameters.context;
            }
            isEditMode = (partDefinition != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGenericBeSerialNumberPartSettingsDirectiveReady = function (api) {
                partSettingsAPI = api;
                partSettingsReadyPromiseDeferred.resolve();
            };


            $scope.scopeModel.savePartDefinition = function () {
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

            loadAllControls();

            function loadAllControls() {
                $scope.scopeModel.isLoading = true;
                function setTitle() {
                    if (isEditMode && partDefinition != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(partDefinition.VariableName, 'Part Definition Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Part Definition Editor');
                }

                function loadStaticData() {
                    if (!isEditMode)
                        return;

                    $scope.scopeModel.variableName = partDefinition.VariableName;
                    $scope.scopeModel.variableDescription = partDefinition.VariableDescription;

                }

                function loadSerialNumberPartEditorSettings() {
                    var loadSerialNumberPartEditorSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
                    partSettingsReadyPromiseDeferred.promise.then(function () {
                        var settingsPayload = {
                            settings: partDefinition != undefined && partDefinition.Settings || undefined,
                            context: getContext()
                        };

                        VRUIUtilsService.callDirectiveLoad(partSettingsAPI, settingsPayload, loadSerialNumberPartEditorSettingsPromiseDeferred);
                    });
                    return loadSerialNumberPartEditorSettingsPromiseDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadSerialNumberPartEditorSettings]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }

        }

        function buildPartDefinitionFromScope() {
            return {
                GenericBESerialNumberPartId: partDefinition != undefined ? partDefinition.GenericBESerialNumberPartId : UtilsService.guid(),
                VariableName: $scope.scopeModel.variableName,
                VariableDescription: $scope.scopeModel.variableDescription,
                Settings: partSettingsAPI.getData()
            };
        }

        function insert() {
            var partDefinition = buildPartDefinitionFromScope();
            if ($scope.onGenericBESerialNumberPartAdded != undefined) {
                $scope.onGenericBESerialNumberPartAdded(partDefinition);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            var partDefinition = buildPartDefinitionFromScope();
            if ($scope.onGenericBESerialNumberPartUpdated != undefined) {
                $scope.onGenericBESerialNumberPartUpdated(partDefinition);
            }
            $scope.modalContext.closeModal();
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }
    }

    appControllers.controller('VR_GenericData_SerialNumberPartDefinitionEditorController', SerialNumberPartDefinitionEditorController);
})(appControllers);
