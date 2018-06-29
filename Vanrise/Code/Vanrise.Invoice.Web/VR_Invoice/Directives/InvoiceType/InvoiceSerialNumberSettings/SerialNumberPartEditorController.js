 (function (appControllers) {

    'use strict';

    serialNumberPartEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function serialNumberPartEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var serialNumberPartEntity;

        var isEditMode;
        var serialNumberPartSettingsAPI;
        var serialNumberPartSettingsReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                serialNumberPartEntity = parameters.serialNumberPartEntity;
            }
            isEditMode = (serialNumberPartEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onPartSettingsReady = function (api) {
                serialNumberPartSettingsAPI = api;
                serialNumberPartSettingsReadyDeferred.resolve();
            };
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateSerialNumberPart() : addeSerialNumberPart();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function builSerialNumberPartObjFromScope() {
                return {
                    VariableName: $scope.scopeModel.variableName,
                    Description: $scope.scopeModel.description,
                    Settings: serialNumberPartSettingsAPI.getData()
                };
            }
            function addeSerialNumberPart() {
                var serialNumberPartObj = builSerialNumberPartObjFromScope();
                if ($scope.onSerialNumberPartAdded != undefined) {
                    $scope.onSerialNumberPartAdded(serialNumberPartObj);
                }
                $scope.modalContext.closeModal();
            }
            function updateSerialNumberPart() {
                var serialNumberPartObj = builSerialNumberPartObjFromScope();
                if ($scope.onSerialNumberPartUpdated != undefined) {
                    $scope.onSerialNumberPartUpdated(serialNumberPartObj);
                }
                $scope.modalContext.closeModal();
            }
        }
        function load() {

            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {
                function setTitle() {
                    if (isEditMode && serialNumberPartEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(serialNumberPartEntity.VariableName, 'Serial Number Part');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Serial Number Part');
                }
                function loadStaticData() {
                    if (serialNumberPartEntity != undefined) {
                        $scope.scopeModel.variableName = serialNumberPartEntity.VariableName;
                        $scope.scopeModel.description = serialNumberPartEntity.Description;

                    }
                }
                function loadSerialNumberPartSettingsDirective() {
                    var serialNumberPartSettingsLoadDeferred = UtilsService.createPromiseDeferred();
                    serialNumberPartSettingsReadyDeferred.promise.then(function () {
                        var serialNumberPartSettingsPayload = { context: getContext() };
                        if (serialNumberPartEntity != undefined)
                            serialNumberPartSettingsPayload.concatenatedPartSettings = serialNumberPartEntity.Settings;
                        VRUIUtilsService.callDirectiveLoad(serialNumberPartSettingsAPI, serialNumberPartSettingsPayload, serialNumberPartSettingsLoadDeferred);
                    });
                    return serialNumberPartSettingsLoadDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSerialNumberPartSettingsDirective]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
        }
        function getContext() {
            return context;
        }

    }
    appControllers.controller('VR_Invoice_SerialNumberPartEditorController', serialNumberPartEditorController);

})(appControllers);