"use strict";

app.directive("vrIntegrationAdapterSftp", ['UtilsService', 'VR_Integration_CompressionTypeEnum',
function (UtilsService, VR_Integration_CompressionTypeEnum) {

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
            $scope.compressionTypes = UtilsService.getArrayEnum(VR_Integration_CompressionTypeEnum);

            $scope.actionsAfterImport = [{ value: 0, name: 'Rename' }, { value: 1, name: 'Delete' }, { value: 2, name: 'Move' }];

            $scope.actionIsRequired = function () {
                return !$scope.basedOnTime;
            };

            defineAPI();
        }

        function defineAPI() {

            var api = {};

            api.getData = function () {

                var extension;

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
                    UserName: $scope.userName,
                    Password: $scope.password,
                    DirectorytoMoveFile: $scope.directorytoMoveFile,
                    ActionAfterImport: $scope.selectedAction ? $scope.selectedAction.value : undefined,
                    LastImportedFile: $scope.lastImportedFile,
                    CompressedFiles: $scope.compressed,
                    CompressionType: $scope.selectedCompressionType != undefined ? $scope.selectedCompressionType.value : undefined,
                    FileCompletenessCheckInterval: $scope.fileCompletenessCheckInterval,
                    NumberOfFiles: $scope.maxNumberOfFiles,
                    InvalidFilesDirectory: $scope.invalidDirectory
                };
            };
            api.getStateData = function () {
                return null;
            };


            api.load = function (payload) {

                if (payload != undefined) {

                    var argumentData = payload.adapterArgument;

                    if (argumentData != null) {
                        $scope.extension = argumentData.Extension;
                        $scope.mask = argumentData.Mask;
                        $scope.directory = argumentData.Directory;
                        $scope.serverIP = argumentData.ServerIP;
                        $scope.userName = argumentData.UserName;
                        $scope.password = argumentData.Password;
                        $scope.directorytoMoveFile = argumentData.DirectorytoMoveFile;
                        $scope.selectedAction = UtilsService.getItemByVal($scope.actionsAfterImport, argumentData.ActionAfterImport, "value");
                        $scope.lastImportedFile = argumentData.LastImportedFile;
                        $scope.compressed = argumentData.CompressedFiles;
                        $scope.selectedCompressionType = UtilsService.getEnum(VR_Integration_CompressionTypeEnum, "value", argumentData.CompressionType);
                        $scope.fileCompletenessCheckInterval = argumentData.FileCompletenessCheckInterval;
                        $scope.maxNumberOfFiles = argumentData.NumberOfFiles;
                        $scope.invalidDirectory = argumentData.InvalidFilesDirectory;
                    }

                }
            };


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
