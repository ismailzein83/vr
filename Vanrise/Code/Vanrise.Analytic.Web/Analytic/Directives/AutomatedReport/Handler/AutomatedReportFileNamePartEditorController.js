(function (appControllers) {

    'use strict';

    fileNamePartEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function fileNamePartEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var fileNamePartEntity;

        var isEditMode;
        var fileNamePartSettingsAPI;
        var fileNamePartSettingsReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load(); 

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                fileNamePartEntity = parameters.fileNamePartEntity;
            }
            isEditMode = (fileNamePartEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onPartSettingsReady = function (api) {
                fileNamePartSettingsAPI = api;
                fileNamePartSettingsReadyDeferred.resolve();
            };
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateFileNamePart() : addFileNamePart();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function buildFileNamePartObjFromScope() {
                return {
                    VariableName: $scope.scopeModel.variableName,
                    Description: $scope.scopeModel.description,
                    Settings: fileNamePartSettingsAPI.getData()
                };
            }
            function addFileNamePart() {
                var fileNamePartObj = buildFileNamePartObjFromScope();
                if ($scope.onFileNamePartAdded != undefined) {
                    $scope.onFileNamePartAdded(fileNamePartObj);
                }
                $scope.modalContext.closeModal();
            }
            function updateFileNamePart() {
                var fileNamePartObj = buildFileNamePartObjFromScope();
                if ($scope.onFileNamePartUpdated != undefined) {
                    $scope.onFileNamePartUpdated(fileNamePartObj);
                }
                $scope.modalContext.closeModal();
            }
        }
        function load() {

            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {
                function setTitle() {
                    if (isEditMode && fileNamePartEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(fileNamePartEntity.VariableName, 'File Name Part');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('File Name Part');
                }
                function loadStaticData() {
                    if (fileNamePartEntity != undefined) {
                        $scope.scopeModel.variableName = fileNamePartEntity.VariableName;
                        $scope.scopeModel.description = fileNamePartEntity.Description;

                    }
                }
                function loadFileNamePartSettingsDirective() {
                    var fileNamePartSettingsLoadDeferred = UtilsService.createPromiseDeferred();
                    fileNamePartSettingsReadyDeferred.promise.then(function () {
                        var fileNamePartSettingsPayload = { context: getContext() };
                        if (fileNamePartEntity != undefined)
                            fileNamePartSettingsPayload.concatenatedPartSettings = fileNamePartEntity.Settings;
                        VRUIUtilsService.callDirectiveLoad(fileNamePartSettingsAPI, fileNamePartSettingsPayload, fileNamePartSettingsLoadDeferred);
                    });
                    return fileNamePartSettingsLoadDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFileNamePartSettingsDirective]).then(function () {

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
    appControllers.controller('VR_Analytic_AutomatedReportFileNamePartEditorController', fileNamePartEditorController);

})(appControllers);