(function (appControllers) {

    "use strict";

    fileDataSourceDefinitionEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function fileDataSourceDefinitionEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var editMode;
        var fileDataSourceDefinitionEntity;

        var fileDelayCheckerDirectiveAPI;
        var fileDelayCheckerDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var fileMissingCheckerDirectiveAPI;
        var fileMissingCheckerDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                fileDataSourceDefinitionEntity = parameters.fileDataSourceDefinitionEntity;
            }

            editMode = (fileDataSourceDefinitionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onFileDelayCheckerDirectiveReady = function (api) {
                fileDelayCheckerDirectiveAPI = api;
                fileDelayCheckerDirectiveReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onFileMissingCheckerDirectiveReady = function (api) {
                fileMissingCheckerDirectiveAPI = api;
                fileMissingCheckerDirectiveReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.saveDataSourceSettings = function () {
                if (editMode)
                    return updateFileDataSourceDefinition();
                else
                    return insertFileDataSourceDefinition();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;

            if (editMode) {
                loadAllControls().finally(function () {
                    fileDataSourceDefinitionEntity = undefined;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls().finally(function () {
                    fileDataSourceDefinitionEntity = undefined;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
        }

        function loadAllControls() {

            function setTitle() {
                if (editMode && fileDataSourceDefinitionEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(fileDataSourceDefinitionEntity.Name, "File Data Source Definition");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("File Data Source Definition");
            }

            function loadStaticData() {
                if (fileDataSourceDefinitionEntity == undefined)
                    return;
                $scope.scopeModel.name = fileDataSourceDefinitionEntity.Name;
                $scope.scopeModel.checkingDuplicateFilesMaxPeriod = fileDataSourceDefinitionEntity.CheckingDuplicateFilesMaxPeriod;
            }

            function loadFileDelayCheckerDirective() {
                var fileDelayCheckerLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                fileDelayCheckerDirectiveReadyPromiseDeferred.promise.then(function () {
                    var payload;
                    if (fileDataSourceDefinitionEntity != undefined && fileDataSourceDefinitionEntity.FileDelayChecker != undefined) {
                        payload = { fileDelayChecker: fileDataSourceDefinitionEntity.FileDelayChecker };
                    }
                    VRUIUtilsService.callDirectiveLoad(fileDelayCheckerDirectiveAPI, payload, fileDelayCheckerLoadPromiseDeferred);
                });

                return fileDelayCheckerLoadPromiseDeferred.promise;
            }

            function loadFileMissingCheckerDirective() {
                var fileMissingCheckerLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                fileMissingCheckerDirectiveReadyPromiseDeferred.promise.then(function () {
                    var payload;
                    if (fileDataSourceDefinitionEntity != undefined && fileDataSourceDefinitionEntity.FileMissingChecker != undefined) {
                        payload = { fileMissingChecker: fileDataSourceDefinitionEntity.FileMissingChecker };
                    }
                    VRUIUtilsService.callDirectiveLoad(fileMissingCheckerDirectiveAPI, payload, fileMissingCheckerLoadPromiseDeferred);
                });

                return fileMissingCheckerLoadPromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFileDelayCheckerDirective, loadFileMissingCheckerDirective])
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    }).finally(function () {
                        $scope.isLoading = false;
                    });
        }

        function buildFileDataSourceDefinitionObjFromScope() {
            var obj = {
                Name: $scope.scopeModel.name,
                CheckingDuplicateFilesMaxPeriod: $scope.scopeModel.checkingDuplicateFilesMaxPeriod,
                FileDelayChecker: fileDelayCheckerDirectiveAPI != undefined ? fileDelayCheckerDirectiveAPI.getData() : undefined,
                FileMissingChecker: fileMissingCheckerDirectiveAPI != undefined ? fileMissingCheckerDirectiveAPI.getData() : undefined
            };
            return obj;
        }

        function insertFileDataSourceDefinition() {
            var fileDataSourceDefinitionObject = buildFileDataSourceDefinitionObjFromScope();
            if ($scope.onFileDataSourceDefinitionAdded != undefined)
                $scope.onFileDataSourceDefinitionAdded(fileDataSourceDefinitionObject);
            $scope.modalContext.closeModal();
        }

        function updateFileDataSourceDefinition() {
            var fileDataSourceDefinitionObject = buildFileDataSourceDefinitionObjFromScope();
            if ($scope.onFileDataSourceDefinitionUpdated != undefined)
                $scope.onFileDataSourceDefinitionUpdated(fileDataSourceDefinitionObject);
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('VR_Integration_FileDataSourceDefinitionEditorController', fileDataSourceDefinitionEditorController);
})(appControllers);