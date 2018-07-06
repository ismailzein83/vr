"use strict";

app.directive("vrIntegrationAdapterSftp", ['UtilsService', 'VR_Integration_CompressionTypeEnum', 'VR_Integration_CompressionEnum', 'VR_Integration_SshEncryptionAlgorithmEnum', 'VR_Integration_SshHostKeyAlgorithmEnum', 'VR_Integration_SshKeyExchangeAlgorithmEnum', 'VR_Integration_SshMacAlgorithmEnum', 'VR_Integration_SshOptionsEnum','FileCheckCriteriaEnum',
function (UtilsService, VR_Integration_CompressionTypeEnum, VR_Integration_CompressionEnum, VR_Integration_SshEncryptionAlgorithmEnum, VR_Integration_SshHostKeyAlgorithmEnum, VR_Integration_SshKeyExchangeAlgorithmEnum, VR_Integration_SshMacAlgorithmEnum, VR_Integration_SshOptionsEnum, FileCheckCriteriaEnum) {

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
        templateUrl: "/Client/Modules/Integration/Directives/Adapter/Templates/AdapterSFTPTemplate.html"
    };
    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;


        function initializeController() {
            $scope.scopeModel = {};
            $scope.compressionTypes = UtilsService.getArrayEnum(VR_Integration_CompressionTypeEnum);

            $scope.scopeModel.compression = UtilsService.getArrayEnum(VR_Integration_CompressionEnum);
            $scope.scopeModel.sshEncryptionAlgorithm = UtilsService.getArrayEnum(VR_Integration_SshEncryptionAlgorithmEnum);
            $scope.scopeModel.sshHostKeyAlgorithm = UtilsService.getArrayEnum(VR_Integration_SshHostKeyAlgorithmEnum);
            $scope.scopeModel.sshKeyExchangeAlgorithm = UtilsService.getArrayEnum(VR_Integration_SshKeyExchangeAlgorithmEnum);
            $scope.scopeModel.sshMacAlgorithm = UtilsService.getArrayEnum(VR_Integration_SshMacAlgorithmEnum);
            $scope.scopeModel.sshOptions = UtilsService.getArrayEnum(VR_Integration_SshOptionsEnum);

            $scope.actionsAfterImport = [{ value: -1, name: 'No Action' }, { value: 0, name: 'Rename' }, { value: 1, name: 'Delete' }, { value: 2, name: 'Move' }];

            $scope.actionIsRequired = function () {
                return !$scope.basedOnTime;
            };

            $scope.fileCheckCriterias = UtilsService.getArrayEnum(FileCheckCriteriaEnum);

            $scope.onSelectedActionChanged = function (selectedAction) {
                if (selectedAction != undefined && selectedAction.value == -1) {
                    $scope.selectedFileCheckCriteria = undefined;
                }
            };

            defineAPI();
        }

        function defineAPI() {

            var api = {};

            api.getData = function () {

                var extension;
                var sshParameters;
                if ($scope.scopeModel.hasSshParameters) {
                    if ($scope.scopeModel.selectedCompression != undefined) {
                        tryInitializeSshParameters();
                        sshParameters.Compression = $scope.scopeModel.selectedCompression.value;
                    }
                    if ($scope.scopeModel.selectedSshEncryptionAlgorithm != undefined) {
                        tryInitializeSshParameters();
                        sshParameters.SshEncryptionAlgorithm = $scope.scopeModel.selectedSshEncryptionAlgorithm.value;
                    }
                    if ($scope.scopeModel.selectedSshHostKeyAlgorithm != undefined) {
                        tryInitializeSshParameters();
                        sshParameters.SshHostKeyAlgorithm = $scope.scopeModel.selectedSshHostKeyAlgorithm.value;
                    }
                    if ($scope.scopeModel.selectedSshKeyExchangeAlgorithm != undefined) {
                        tryInitializeSshParameters();
                        sshParameters.SshKeyExchangeAlgorithm = $scope.scopeModel.selectedSshKeyExchangeAlgorithm.value;
                    }
                    if ($scope.scopeModel.selectedSshMacAlgorithm != undefined) {
                        tryInitializeSshParameters();
                        sshParameters.SshMacAlgorithm = $scope.scopeModel.selectedSshMacAlgorithm.value;
                    }
                    if ($scope.scopeModel.selectedSshOptions != undefined) {
                        tryInitializeSshParameters();
                        sshParameters.SshOptions = $scope.scopeModel.selectedSshOptions.value;
                    }
                }

                function tryInitializeSshParameters() {
                    if (sshParameters == undefined)
                        sshParameters = {};
                }



                if ($scope.extension.indexOf(".") == 0)
                    extension = $scope.extension;
                else
                    extension = "." + $scope.extension;

                return {
                    $type: "Vanrise.Integration.Adapters.SFTPReceiveAdapter.Arguments.SFTPAdapterArgument, Vanrise.Integration.Adapters.SFTPReceiveAdapter.Arguments",
                    Extension: extension,
                    Mask: $scope.mask == undefined ? "" : $scope.mask,
                    Directory: $scope.directory,
                    ServerIP: $scope.serverIP,
                    Port: $scope.port,
                    UserName: $scope.userName,
                    Password: $scope.password,
                    DirectorytoMoveFile: $scope.directorytoMoveFile,
                    ActionAfterImport: $scope.selectedAction ? $scope.selectedAction.value : undefined,
                    FileCheckCriteria: $scope.selectedFileCheckCriteria != undefined ? $scope.selectedFileCheckCriteria.value : $scope.fileCheckCriterias[0].value,
                    LastImportedFile: $scope.lastImportedFile,
                    CompressedFiles: $scope.compressed,
                    CompressionType: $scope.selectedCompressionType != undefined ? $scope.selectedCompressionType.value : undefined,
                    FileCompletenessCheckInterval: $scope.fileCompletenessCheckInterval,
                    NumberOfFiles: $scope.maxNumberOfFiles,
                    InvalidFilesDirectory: $scope.invalidDirectory,
                    SshParameters: sshParameters
                };
            };
            api.getStateData = function () {
                return null;
            };


            api.load = function (payload) {
                var port;
                if (payload != undefined) {

                    var argumentData = payload.adapterArgument;

                    if (argumentData != null) {
                        $scope.extension = argumentData.Extension;
                        $scope.mask = argumentData.Mask;
                        $scope.directory = argumentData.Directory;
                        $scope.serverIP = argumentData.ServerIP;
                        port = argumentData.Port;
                        $scope.userName = argumentData.UserName;
                        $scope.password = argumentData.Password;
                        $scope.directorytoMoveFile = argumentData.DirectorytoMoveFile;
                        $scope.selectedAction = UtilsService.getItemByVal($scope.actionsAfterImport, argumentData.ActionAfterImport, "value");
                        $scope.selectedFileCheckCriteria = UtilsService.getItemByVal($scope.fileCheckCriterias, argumentData.FileCheckCriteria, "value");
                        $scope.lastImportedFile = argumentData.LastImportedFile;
                        $scope.compressed = argumentData.CompressedFiles;
                        $scope.selectedCompressionType = UtilsService.getEnum(VR_Integration_CompressionTypeEnum, "value", argumentData.CompressionType);
                        $scope.fileCompletenessCheckInterval = argumentData.FileCompletenessCheckInterval;
                        $scope.maxNumberOfFiles = argumentData.NumberOfFiles;
                        $scope.invalidDirectory = argumentData.InvalidFilesDirectory;
                        if (argumentData.SshParameters != undefined) {
                            $scope.scopeModel.hasSshParameters = true;
                            if (argumentData.SshParameters.Compression != undefined)
                                $scope.scopeModel.selectedCompression = UtilsService.getItemByVal($scope.scopeModel.compression, argumentData.SshParameters.Compression, "value");
                            if (argumentData.SshParameters.SshEncryptionAlgorithm != undefined)
                                $scope.scopeModel.selectedSshEncryptionAlgorithm = UtilsService.getItemByVal($scope.scopeModel.sshEncryptionAlgorithm, argumentData.SshParameters.SshEncryptionAlgorithm, "value");
                            if (argumentData.SshParameters.SshHostKeyAlgorithm != undefined)
                                $scope.scopeModel.selectedSshHostKeyAlgorithm = UtilsService.getItemByVal($scope.scopeModel.sshHostKeyAlgorithm, argumentData.SshParameters.SshHostKeyAlgorithm, "value");
                            if (argumentData.SshParameters.SshKeyExchangeAlgorithm != undefined)
                                $scope.scopeModel.selectedSshKeyExchangeAlgorithm = UtilsService.getItemByVal($scope.scopeModel.sshKeyExchangeAlgorithm, argumentData.SshParameters.SshKeyExchangeAlgorithm, "value");
                            if (argumentData.SshParameters.SshMacAlgorithm != undefined)
                                $scope.scopeModel.selectedSshMacAlgorithm = UtilsService.getItemByVal($scope.scopeModel.sshMacAlgorithm, argumentData.SshParameters.SshMacAlgorithm, "value");
                            if (argumentData.SshParameters.SshOptions != undefined)
                                $scope.scopeModel.selectedSshOptions = UtilsService.getItemByVal($scope.scopeModel.sshOptions, argumentData.SshParameters.SshOptions, "value");
                        }

                    }
                }
                if (port == undefined)
                    port = 22;
                $scope.port = port;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
