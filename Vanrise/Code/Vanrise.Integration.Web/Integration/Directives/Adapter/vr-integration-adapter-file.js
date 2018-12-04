"use strict";

app.directive("vrIntegrationAdapterFile", ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

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
            templateUrl: "/Client/Modules/Integration/Directives/Adapter/Templates/AdapterFileTemplate.html"
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            var fileDataSourceDefinitionsSelectorAPI;
            var fileDataSourceDefinitionsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.actionsAfterImport = [{ value: -1, name: 'No Action' }, { value: 0, name: 'Rename' }, { value: 1, name: 'Delete' }, { value: 2, name: 'Move' }];

                $scope.scopeModel.onFileDataSourceDefinitionsSelectorReady = function (api) {
                    fileDataSourceDefinitionsSelectorAPI = api;
                    fileDataSourceDefinitionsSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    if (payload != undefined) {

                        var argumentData = payload.adapterArgument;

                        if (argumentData != null) {
                            $scope.scopeModel.extension = argumentData.Extension;
                            $scope.scopeModel.mask = argumentData.Mask;
                            $scope.scopeModel.directory = argumentData.Directory;
                            $scope.scopeModel.directorytoMoveFile = argumentData.DirectorytoMoveFile;
                            $scope.scopeModel.selectedAction = UtilsService.getItemByVal($scope.scopeModel.actionsAfterImport, argumentData.ActionAfterImport, "value");
                        }
                    }

                    var loadFileDataSourceDefinitionsSelectorPromise = loadFileDataSourceDefinitionsSelector();
                    promises.push(loadFileDataSourceDefinitionsSelectorPromise);

                    return UtilsService.waitMultiplePromises(promises)
                        .catch(function (error) {
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
                        $type: "Vanrise.Integration.Adapters.FileReceiveAdapter.Arguments.FileAdapterArgument, Vanrise.Integration.Adapters.FileReceiveAdapter.Arguments",
                        Extension: extension,
                        Mask: $scope.scopeModel.mask == undefined ? "" : $scope.scopeModel.mask,
                        Directory: $scope.scopeModel.directory,
                        DirectorytoMoveFile: $scope.scopeModel.directorytoMoveFile,
                        ActionAfterImport: $scope.scopeModel.selectedAction.value,
                        FileDataSourceDefinitionId: fileDataSourceDefinitionsSelectorAPI != undefined ? fileDataSourceDefinitionsSelectorAPI.getSelectedIds() : undefined
                    };
                };

                api.getStateData = function () {
                    return null;
                };

                if (ctrl.onReady != null && typeof ctrl.onReady == "function")
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);