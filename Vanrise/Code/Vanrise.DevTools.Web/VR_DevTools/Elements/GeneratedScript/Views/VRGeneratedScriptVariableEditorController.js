(appControllers); (function (appControllers) {
    "use strict";
    generatedScriptVariableEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Devtools_GeneratedScriptService'];

    function generatedScriptVariableEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Devtools_GeneratedScriptService) {

        var isEditMode;
        var variableEntity = {};

        var variableTypeDirectiveApi;
        var variableTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var variableSettingsDirectiveApi;
        var variableSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var connectionStringId;
        $scope.scopeModel = {};
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                variableEntity = parameters.variable;
                connectionStringId = parameters.connectionStringId;
            }
            isEditMode = (variableEntity != undefined);
        }

        function defineScope() {

            $scope.scopeModel.saveGeneratedScriptVariable = function () {

                if (isEditMode)
                    return updateGeneratedScriptVariable();
                else
                    return insertGeneratedScriptVariable();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onVariableTypeDirectiveReady = function (api) {
                variableTypeDirectiveApi = api;
                variableTypeReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onVariableSettingsDirectiveReady = function (api) {
                variableSettingsDirectiveApi = api;
                variableSettingsReadyPromiseDeferred.resolve();
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                loadAllControls().finally(function () {
                });
            }
            else
                loadAllControls();
        }

        function loadAllControls() {
            function loadVariableTypeDirective() {
                var variableTypeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                variableTypeReadyPromiseDeferred.promise.then(function () {
                    var variableTypePayload;
                    if (isEditMode) {
                        variableTypePayload = { selectedIds: variableEntity.Type, size: variableEntity.Size, precision: variableEntity.Precision };
                    }
                    VRUIUtilsService.callDirectiveLoad(variableTypeDirectiveApi, variableTypePayload, variableTypeLoadPromiseDeferred);
                });
                return variableTypeLoadPromiseDeferred.promise;
            }

            function loadVariableSettingsDirective() {
                var variableSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                variableSettingsReadyPromiseDeferred.promise.then(function () {
                    var  variableSettingsPayload = {
                            settings: variableEntity!=undefined? variableEntity.Settings:undefined,
                            isEditMode: isEditMode,
                            connectionStringId: connectionStringId
                        };

                    VRUIUtilsService.callDirectiveLoad(variableSettingsDirectiveApi, variableSettingsPayload, variableSettingsLoadPromiseDeferred);
                });

                return variableSettingsLoadPromiseDeferred.promise;
            }

            function loadStaticData() {
                if (variableEntity != undefined) {
                    $scope.scopeModel.variableName = variableEntity.Name;
                }
            }

            function setTitle() {
                if (isEditMode && variableEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor("", "Variable");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Variable");
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadVariableTypeDirective,  loadVariableSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function buildGeneratedScriptVariableObjectFromScope() {
            var typeInfo = variableTypeDirectiveApi.getSelectedIds();
            return {
                Entity: {
                    Id: UtilsService.guid(),
                    Name: $scope.scopeModel.variableName,
                    Type: typeInfo.Type,
                    Size: typeInfo.Size,
                    Precision: typeInfo.Precision,
                    Settings: variableSettingsDirectiveApi.getData()
                }
            };
        }

        function insertGeneratedScriptVariable() {
            $scope.scopeModel.isLoading = true;
            if ($scope.onGeneratedScriptVariableAdded != undefined) {
                $scope.onGeneratedScriptVariableAdded(buildGeneratedScriptVariableObjectFromScope());
            }
            $scope.modalContext.closeModal();
            $scope.scopeModel.isLoading = true;
        }

        function updateGeneratedScriptVariable() {
            $scope.scopeModel.isLoading = true;
            if ($scope.onGeneratedScriptVariableUpdated != undefined) {
                $scope.onGeneratedScriptVariableUpdated(buildGeneratedScriptVariableObjectFromScope());
            }
            $scope.modalContext.closeModal();
            $scope.scopeModel.isLoading = true;

        }

    }
    appControllers.controller('VR_Devtools_GeneratedScriptVariableEditorController', generatedScriptVariableEditorController);
})(appControllers);