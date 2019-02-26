"use strict";

app.directive("vrIntegrationAdapterFtp", ['UtilsService', 'VR_Integration_CompressionTypeEnum', 'VRUIUtilsService', 'FileCheckCriteriaEnum', 'VRNotificationService',
    function (UtilsService, VR_Integration_CompressionTypeEnum, VRUIUtilsService, FileCheckCriteriaEnum, VRNotificationService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var directiveConstructor = new DirectiveConstructor($scope, ctrl);
                directiveConstructor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/Integration/Directives/Adapter/Templates/AdapterFTPTemplate.html"
        };
        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            var fileDataSourceDefinitionsSelectorAPI;
            var fileDataSourceDefinitionsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.compressionTypes = UtilsService.getArrayEnum(VR_Integration_CompressionTypeEnum);

                $scope.scopeModel.actionsAfterImport = [{ value: -1, name: 'No Action' }, { value: 0, name: 'Rename' }, { value: 1, name: 'Delete' }, { value: 2, name: 'Move' }];

                $scope.scopeModel.actionIsRequired = function () {
                    return !$scope.basedOnTime;
                };

                $scope.scopeModel.fileCheckCriterias = UtilsService.getArrayEnum(FileCheckCriteriaEnum);

                $scope.scopeModel.onSelectedActionChanged = function (selectedAction) {
                    if (selectedAction != undefined && selectedAction.value == -1) {
                        $scope.selectedFileCheckCriteria = undefined;
                    }
                };

                $scope.scopeModel.onFileDataSourceDefinitionsSelectorReady = function (api) {
                    fileDataSourceDefinitionsSelectorAPI = api;
                    fileDataSourceDefinitionsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onFileDataSourceDefinitionSelectionChanged = function () {
                    var beforeSelection = $scope.scopeModel.selectedFileDataSourceDefinition == undefined ? false : $scope.scopeModel.selectedFileDataSourceDefinition;
                    $scope.scopeModel.selectedFileDataSourceDefinition = fileDataSourceDefinitionsSelectorAPI != undefined && fileDataSourceDefinitionsSelectorAPI.getSelectedIds() != undefined ?
                        true : false;
                    if (beforeSelection && !$scope.scopeModel.selectedFileDataSourceDefinition)
                        $scope.scopeModel.duplicatedFilesDirectory = undefined;
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var argumentData;

                    if (payload != undefined) {
                        argumentData = payload.adapterArgument;

                        if (argumentData != null) {
                            $scope.scopeModel.extension = argumentData.Extension;
                            $scope.scopeModel.mask = argumentData.Mask;
                            $scope.scopeModel.directory = argumentData.Directory;
                            $scope.scopeModel.serverIP = argumentData.ServerIP;
                            $scope.scopeModel.userName = argumentData.UserName;
                            $scope.scopeModel.password = argumentData.Password;
                            $scope.scopeModel.directorytoMoveFile = argumentData.DirectorytoMoveFile;
                            $scope.scopeModel.selectedAction = UtilsService.getItemByVal($scope.scopeModel.actionsAfterImport, argumentData.ActionAfterImport, "value");
                            $scope.scopeModel.selectedFileCheckCriteria = UtilsService.getItemByVal($scope.scopeModel.fileCheckCriterias, argumentData.FileCheckCriteria, "value");
                            $scope.scopeModel.lastImportedFile = argumentData.LastImportedFile;
                            $scope.scopeModel.compressed = argumentData.CompressedFiles;
                            $scope.scopeModel.selectedCompressionType = UtilsService.getEnum(VR_Integration_CompressionTypeEnum, "value", argumentData.CompressionType);
                            $scope.scopeModel.fileCompletenessCheckInterval = argumentData.FileCompletenessCheckInterval;
                            $scope.scopeModel.maxNumberOfFiles = argumentData.NumberOfFiles;
                            $scope.scopeModel.invalidDirectory = argumentData.InvalidFilesDirectory;
                            $scope.scopeModel.duplicatedFilesDirectory = argumentData.DuplicatedFilesDirectory;
                        }
                    }

                    var loadFileDataSourceDefinitionsSelectorPromise = loadFileDataSourceDefinitionsSelector();
                    promises.push(loadFileDataSourceDefinitionsSelectorPromise);

                    return UtilsService.waitMultiplePromises(promises).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    }).finally(function () {
                        $scope.isLoading = false;
                    });


                    function loadFileDataSourceDefinitionsSelector() {

                        var fileDataSourceDefinitionsSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        fileDataSourceDefinitionsSelectorReadyPromiseDeferred.promise.then(function () {

                            var payload;

                            if (argumentData != undefined && argumentData != null)
                                payload = { selectedIds: argumentData.FileDataSourceDefinitionId };

                            VRUIUtilsService.callDirectiveLoad(fileDataSourceDefinitionsSelectorAPI, payload, fileDataSourceDefinitionsSelectorLoadPromiseDeferred);
                        });

                        return fileDataSourceDefinitionsSelectorLoadPromiseDeferred.promise;
                    }
                };

                api.getData = function () {

                    var extension;
                    if ($scope.scopeModel.extension.indexOf(".") == 0)
                        extension = $scope.scopeModel.extension;
                    else
                        extension = "." + $scope.scopeModel.extension;

                    return {
                        $type: "Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments",
                        Extension: extension,
                        Mask: $scope.scopeModel.mask == undefined ? "" : $scope.scopeModel.mask,
                        Directory: $scope.scopeModel.directory,
                        ServerIP: $scope.scopeModel.serverIP,
                        UserName: $scope.scopeModel.userName,
                        Password: $scope.scopeModel.password,
                        DirectorytoMoveFile: $scope.scopeModel.directorytoMoveFile,
                        ActionAfterImport: $scope.scopeModel.selectedAction ? $scope.scopeModel.selectedAction.value : undefined,
                        FileCheckCriteria: $scope.scopeModel.selectedFileCheckCriteria != undefined ? $scope.scopeModel.selectedFileCheckCriteria.value : $scope.scopeModel.fileCheckCriterias[0].value,
                        LastImportedFile: $scope.scopeModel.lastImportedFile,
                        CompressedFiles: $scope.scopeModel.compressed,
                        CompressionType: $scope.scopeModel.selectedCompressionType != undefined ? $scope.scopeModel.selectedCompressionType.value : undefined,
                        FileCompletenessCheckInterval: $scope.scopeModel.fileCompletenessCheckInterval,
                        NumberOfFiles: $scope.scopeModel.maxNumberOfFiles,
                        InvalidFilesDirectory: $scope.scopeModel.invalidDirectory,
                        FileDataSourceDefinitionId: fileDataSourceDefinitionsSelectorAPI != undefined ? fileDataSourceDefinitionsSelectorAPI.getSelectedIds() : undefined,
                        DuplicatedFilesDirectory: fileDataSourceDefinitionsSelectorAPI != undefined ? $scope.scopeModel.duplicatedFilesDirectory : undefined
                    };
                };

                api.getStateData = function () {
                    return null;
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
