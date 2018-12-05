(appControllers); (function (appControllers) {
    "use strict";
    generatedScriptDesignEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Tools_GeneratedScriptService', 'VR_Tools_ColumnsAPIService'];

    function generatedScriptDesignEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Tools_GeneratedScriptService, VR_Tools_ColumnsAPIService) {

        var isEditMode;
        var designEntity = {};

        var connectionStringDirectiveApi;
        var connectionStringReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var connectionStringSelectedPromiseDeferred;

        var tableDirectiveApi;
        var tableReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var tableSelectedPromiseDeferred;

        var schemaDirectiveApi;
        var schemaReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var schemaSelectedPromiseDeferred;

        var generatedScriptSettingsDirectiveApi;
        var generatedScriptSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
        var generatedScriptSettingsSelectedPromiseDeferred;
       
        $scope.scopeModel = {};
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                designEntity = parameters.design;
            }
            isEditMode = (designEntity != undefined);
            if (isEditMode) designEntity = designEntity.Entity;
        }

        function defineScope() {

            $scope.scopeModel.saveGeneratedScriptDesign = function () {
                if (isEditMode)
                    return updateGeneratedScriptDesign();
                else
                    return insertGeneratedScriptDesign();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
    
            $scope.scopeModel.onConnectionStringDirectiveReady = function (api) {
                connectionStringDirectiveApi = api;
                connectionStringReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onSchemaDirectiveReady = function (api) {
                schemaDirectiveApi = api;
                schemaReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onTableDirectiveReady = function (api) {
                tableDirectiveApi = api;
                tableReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onGeneratedScriptSettingsDirectiveReady = function (api) {
                generatedScriptSettingsDirectiveApi = api;
                generatedScriptSettingsPromiseDeferred.resolve();
            };

            $scope.scopeModel.onConnectionStringChanged = function (value) {
                if (connectionStringDirectiveApi != undefined) {
                    if (value != undefined) {
                        if (connectionStringSelectedPromiseDeferred != undefined) {
                            connectionStringSelectedPromiseDeferred.resolve();
                        }
                        else {
                            if (generatedScriptSettingsDirectiveApi != undefined)
                                generatedScriptSettingsDirectiveApi.clear();

                            var schemaPayload = {
                                filter: {
                                    ConnectionId: connectionStringDirectiveApi.getSelectedIds()
                                }
                            };
                            var setLoader = function (value) { $scope.scopeModel.isSchemaDirectiveloading = value; };
                            var schemaDirectiveLoadDeferred;

                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, schemaDirectiveApi, schemaPayload, setLoader, schemaDirectiveLoadDeferred);

                        }
                    }
                }

            };

            $scope.scopeModel.onSchemaChanged = function (value) {
                if (schemaDirectiveApi != undefined) {
                    var data = schemaDirectiveApi.getSelectedIds();
                    if (data != undefined) {
                        if (schemaSelectedPromiseDeferred != undefined) {
                            schemaSelectedPromiseDeferred.resolve();
                        }
                        else {
                            
                            if (generatedScriptSettingsDirectiveApi != undefined)
                                generatedScriptSettingsDirectiveApi.clear();

                            var tablePayload = {
                                filter: {
                                    ConnectionId: connectionStringDirectiveApi.getSelectedIds(),
                                    SchemaName: schemaDirectiveApi.getSelectedIds()
                                }
                            };

                            var setLoader = function (value) { $scope.scopeModel.isTableDirectiveloading = value; };
                            var tableDirectiveLoadDeferred;
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, tableDirectiveApi, tablePayload, setLoader, tableDirectiveLoadDeferred);
                        }
                    }
                }

            };

            $scope.scopeModel.onTableChanged = function (value) {
                if (tableDirectiveApi != undefined) {
                    var data = tableDirectiveApi.getSelectedIds();
                    if (data != undefined) {
                        if (tableSelectedPromiseDeferred != undefined) {
                            tableSelectedPromiseDeferred.resolve();
                        }
                        else {

                            var settingsPayload = {
                                filter:{
                                    ConnectionId: connectionStringDirectiveApi.getSelectedIds(),
                                    SchemaName:schemaDirectiveApi.getSelectedIds(),
                                    TableName: tableDirectiveApi.getSelectedIds()
                                },
                                isEditMode: isEditMode,
                                Settings: {
                                    InsertColumns: [],
                                    UpdateColumns: [],
                                    IdentifierColumns: [],
                                    DataRows:[]
                                }
                            };
                            var setLoader = function (value) { $scope.scopeModel.isGeneratedScriptSettingsDirectiveloading = value; };
                            var generatedScriptSettingsDirectiveLoadDeferred;
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, generatedScriptSettingsDirectiveApi, settingsPayload, setLoader, generatedScriptSettingsDirectiveLoadDeferred);
                          
                        }
                    }
                }
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
            
            if (isEditMode) {
                connectionStringSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                schemaSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                tableSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
            }

            function loadConnectionStringDirective() {
                var connectionStringLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                connectionStringReadyPromiseDeferred.promise.then(function () {
                     var connectionStringPayload = { selectedIds: designEntity != undefined ? designEntity.ConnectionId : undefined };
                    VRUIUtilsService.callDirectiveLoad(connectionStringDirectiveApi, connectionStringPayload, connectionStringLoadPromiseDeferred);
                });
                return connectionStringLoadPromiseDeferred.promise;
            }

            function loadSchemaDirective() {
                var promises = [];
                var schemaDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                if (connectionStringSelectedPromiseDeferred != undefined)
                    promises.push(connectionStringSelectedPromiseDeferred.promise);
                promises.push(schemaReadyPromiseDeferred.promise);
                UtilsService.waitMultiplePromises(promises).then(function (response) {
                    var schemaPayload = {
                        selectedIds: designEntity.Schema,
                        filter: {
                            ConnectionId: designEntity.ConnectionId
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(schemaDirectiveApi, schemaPayload, schemaDirectiveLoadDeferred);
                    connectionStringSelectedPromiseDeferred = undefined;
                });

                return schemaDirectiveLoadDeferred.promise;
            }

            function loadTableDirective() {
                var promises = [];
                var tableDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                if (schemaSelectedPromiseDeferred != undefined)
                    promises.push(schemaSelectedPromiseDeferred.promise);
                promises.push(tableReadyPromiseDeferred.promise);
                UtilsService.waitMultiplePromises(promises).then(function (response) {
                    var tableDirectivePayload = {
                        filter: {
                            ConnectionId: designEntity.ConnectionId,
                            SchemaName: designEntity.Schema
                        },
                        selectedIds: designEntity.TableName
                    };
                    VRUIUtilsService.callDirectiveLoad(tableDirectiveApi, tableDirectivePayload, tableDirectiveLoadDeferred);
                    schemaSelectedPromiseDeferred = undefined;
                });

                return tableDirectiveLoadDeferred.promise;
            }


            function loadGeneratedScriptSettingsDirective() {
                var promises = [];
                var generatedScriptSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                if (tableSelectedPromiseDeferred != undefined)
                    promises.push(tableSelectedPromiseDeferred.promise);
                promises.push(generatedScriptSettingsPromiseDeferred.promise);
                UtilsService.waitMultiplePromises(promises).then(function (response) {

                    var settingsPayload = {
                        filter: {
                            ConnectionId: designEntity.ConnectionId,
                            SchemaName: designEntity.Schema,
                            TableName: designEntity.TableName
                        },
                        Settings: designEntity.Settings,
                        isEditMode: isEditMode,
                    };
                    VRUIUtilsService.callDirectiveLoad(generatedScriptSettingsDirectiveApi, settingsPayload, generatedScriptSettingsDirectiveLoadDeferred);
                    tableSelectedPromiseDeferred = undefined;

                });
                return generatedScriptSettingsDirectiveLoadDeferred.promise;
            }

            function loadDirectives() {
                if (isEditMode) {
                    var promises = [];
           
                    promises.push(loadSchemaDirective());
                    promises.push(loadTableDirective());
                    promises.push(loadGeneratedScriptSettingsDirective());

                    return UtilsService.waitMultiplePromises(promises);
                }
            }

            function setTitle() {
                if (isEditMode && designEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor("", "Generated Script Design");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Generated Script Design");
            }

            function loadStaticData() {

            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadConnectionStringDirective, loadDirectives]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
           .finally(function () {
               $scope.scopeModel.isLoading = false;
           });
        }

        function buildGeneratedScriptDesignObjectFromScope() {

            return {
                Entity: {
                    ConnectionId: connectionStringDirectiveApi.getSelectedIds(),
                    Schema: schemaDirectiveApi.getSelectedIds(),
                    TableName: tableDirectiveApi.getSelectedIds(),
                    Settings: generatedScriptSettingsDirectiveApi.getData().data,
                    Title: generatedScriptSettingsDirectiveApi.getData().Title
                }
            };
        }

        function insertGeneratedScriptDesign() {
            if ($scope.onGeneratedScriptDesignAdded != undefined) {
                $scope.onGeneratedScriptDesignAdded(buildGeneratedScriptDesignObjectFromScope());
            }
            $scope.modalContext.closeModal();
        }

        function updateGeneratedScriptDesign() {

            if ($scope.onGeneratedScriptDesignUpdated != undefined) {
                $scope.onGeneratedScriptDesignUpdated(buildGeneratedScriptDesignObjectFromScope());
            }
            $scope.modalContext.closeModal();
        }

    }
    appControllers.controller('Vanrise_Tools_GeneratedScriptDesignEditorController', generatedScriptDesignEditorController);
})(appControllers);