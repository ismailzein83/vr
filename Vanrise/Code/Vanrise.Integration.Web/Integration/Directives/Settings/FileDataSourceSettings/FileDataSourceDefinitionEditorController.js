(function (appControllers) {

    "use strict";

    fileDataSourceDefinitionEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Integration_DataSourceSettingAPIService'];

    function fileDataSourceDefinitionEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Integration_DataSourceSettingAPIService) {

        var editMode;
        var isSingleInsert;
        var fileDataSourceDefinitionEntity;

        var fileDelayCheckerDirectiveAPI;
        var fileDelayCheckerDirectiveReadyPromiseDeferred;

        var fileMissingCheckerDirectiveAPI;
        var fileMissingCheckerDirectiveReadyPromiseDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                fileDataSourceDefinitionEntity = parameters.fileDataSourceDefinitionEntity;
                isSingleInsert = parameters.isSingleInsert;
            }

            editMode = (fileDataSourceDefinitionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDuplicateSettingsSwitchChanged = function () {
                $scope.scopeModel.duplicateCheckInterval = "2.00:00:00";
            };

            $scope.scopeModel.onFileDelayCheckerDirectiveReady = function (api) {
                fileDelayCheckerDirectiveAPI = api;

                var setLoader = function (value) {
                    $scope.scopeModel.isFileDelayCheckerDirectiveLoading = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, fileDelayCheckerDirectiveAPI, undefined, setLoader, fileDelayCheckerDirectiveReadyPromiseDeferred);
            };

            $scope.scopeModel.onFileMissingCheckerDirectiveReady = function (api) {
                fileMissingCheckerDirectiveAPI = api;

                var setLoader = function (value) {
                    $scope.scopeModel.isFileMissingCheckerDirectiveLoading = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, fileMissingCheckerDirectiveAPI, undefined, setLoader, fileMissingCheckerDirectiveReadyPromiseDeferred);
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

            loadAllControls().catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            }).finally(function () {
                fileDataSourceDefinitionEntity = undefined;
            });
        }

        function loadAllControls() {

            function setTitle() {
                if (editMode && fileDataSourceDefinitionEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(fileDataSourceDefinitionEntity.Name, "File Import Exception Setting", $scope);
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("File Import Exception Setting");
            }

            function loadStaticData() {
                if (fileDataSourceDefinitionEntity != undefined) {
                    $scope.scopeModel.name = fileDataSourceDefinitionEntity.Name;

                    if (fileDataSourceDefinitionEntity.DuplicateCheckInterval != undefined) {
                        $scope.scopeModel.duplicateCheckInterval = fileDataSourceDefinitionEntity.DuplicateCheckInterval;
                        $scope.scopeModel.showDuplicateSettings = true;
                    }
                }
            }

            function loadFileDelayCheckerDirective() {
                if (fileDataSourceDefinitionEntity == undefined || fileDataSourceDefinitionEntity.FileDelayChecker == undefined)
                    return;

                fileDelayCheckerDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                $scope.scopeModel.showDelaySettings = true;

                var fileDelayCheckerLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                fileDelayCheckerDirectiveReadyPromiseDeferred.promise.then(function () {
                    fileDelayCheckerDirectiveReadyPromiseDeferred = undefined;

                    var payload = {
                        fileDelayChecker: fileDataSourceDefinitionEntity.FileDelayChecker
                    };
                    VRUIUtilsService.callDirectiveLoad(fileDelayCheckerDirectiveAPI, payload, fileDelayCheckerLoadPromiseDeferred);
                });

                return fileDelayCheckerLoadPromiseDeferred.promise;
            }

            function loadFileMissingCheckerDirective() {
                if (fileDataSourceDefinitionEntity == undefined || fileDataSourceDefinitionEntity.FileMissingChecker == undefined)
                    return;

                fileMissingCheckerDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                $scope.scopeModel.showMissingSettings = true;

                var fileMissingCheckerLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                fileMissingCheckerDirectiveReadyPromiseDeferred.promise.then(function () {
                    fileMissingCheckerDirectiveReadyPromiseDeferred = undefined;

                    var payload = { fileMissingChecker: fileDataSourceDefinitionEntity.FileMissingChecker };

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
                FileDataSourceDefinitionId: fileDataSourceDefinitionEntity != undefined ? fileDataSourceDefinitionEntity.FileDataSourceDefinitionId : UtilsService.guid(),
                Name: $scope.scopeModel.name,
                DuplicateCheckInterval: $scope.scopeModel.showDuplicateSettings ? $scope.scopeModel.duplicateCheckInterval : undefined,
                FileDelayChecker: fileDelayCheckerDirectiveAPI != undefined && $scope.scopeModel.showDelaySettings ? fileDelayCheckerDirectiveAPI.getData() : undefined,
                FileMissingChecker: fileMissingCheckerDirectiveAPI != undefined && $scope.scopeModel.showMissingSettings ? fileMissingCheckerDirectiveAPI.getData() : undefined
            };
            return obj;
        }

        function insertFileDataSourceDefinition() {

            var fileDataSourceDefinitionObject = buildFileDataSourceDefinitionObjFromScope();

            if (isSingleInsert) {
                $scope.scopeModel.isLoading = true;
                return VR_Integration_DataSourceSettingAPIService.AddFileDataSourceDefinition(fileDataSourceDefinitionObject).then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("File Import Exception Setting", response, "Name")) {
                        if ($scope.onFileDataSourceDefinitionAdded != undefined)
                            $scope.onFileDataSourceDefinitionAdded(fileDataSourceDefinitionObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                if ($scope.onFileDataSourceDefinitionAdded != undefined)
                    $scope.onFileDataSourceDefinitionAdded(fileDataSourceDefinitionObject);
                $scope.modalContext.closeModal();
            }
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