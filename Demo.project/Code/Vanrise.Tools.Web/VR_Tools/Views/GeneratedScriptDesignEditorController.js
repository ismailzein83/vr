(appControllers); (function (appControllers) {
    "use strict";
    generatedScriptDesignEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'Vanrise_Tools_GeneratedScriptService'];

    function generatedScriptDesignEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, Vanrise_Tools_GeneratedScriptService) {

        var isEditMode;
        var generatedScriptDesignEntity = {};

        var connectionStringDirectiveApi;
        var tableDirectiveApi;
        var schemaDirectiveApi;
        var insertColumnsDirectiveApi;
        var updateColumnsDirectiveApi;
        var identifierColumnsDirectiveApi;

        var connectionStringReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var tableReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var schemaReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var insertColumnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var updateColumnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var identifierColumnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        var connectionStringSelectedPromiseDeferred;
        var schemaSelectedPromiseDeferred;
        var tableSelectedPromiseDeferred;



        $scope.scopeModel = {};

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                generatedScriptDesignEntity = parameters.generatedScriptDesignEntity;
            }
            isEditMode = (generatedScriptDesignEntity != undefined);
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
            $scope.scopeModel.onInsertColumnsDirectiveReady = function (api) {

                insertColumnsDirectiveApi = api;
                insertColumnsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onInsertColumnsDirectiveReady = function (api) {

                updateColumnsDirectiveApi = api;
                updateColumnsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onInsertColumnsDirectiveReady = function (api) {

                identifierColumnsDirectiveApi = api;
                identifierColumnsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onConnectionStringChanged = function () {
                if (connectionStringDirectiveApi != undefined) {
                    var data = connectionStringDirectiveApi.getSelectedIds()
                    if (data != undefined) {
                        if (connectionStringSelectedPromiseDeferred != undefined) {
                            connectionStringSelectedPromiseDeferred.resolve();
                        }
                        else {
                            if (schemaDirectiveApi != undefined)
                                schemaDirectiveApi.clear();
                            if (insertColumnsDirectiveApi != undefined)
                                insertColumnsDirectiveApi.clear();
                            if (updateColumnsDirectiveApi != undefined)
                                updateColumnsDirectiveApi.clear();
                            if (identifierColumnsDirectiveApi != undefined)
                                identifierColumnsDirectiveApi.clear();

                            var schemaPayload = {};
                            var Filter = {
                                ConnectionStringId: connectionStringDirectiveApi.getSelectedIds()
                            };
                            schemaPayload.filter = Filter;

                            loadSchemaDirective(schemaPayload);

                        }
                    }
                }

            }

            $scope.scopeModel.onSchemaChanged = function () {
                if (schemaDirectiveApi != undefined) {
                    var data = schemaDirectiveApi.getSelectedIds()
                    if (data != undefined) {
                        if (schemaSelectedPromiseDeferred != undefined) {
                            schemaSelectedPromiseDeferred.resolve();
                        }
                        else {
                            if (insertColumnsDirectiveApi != undefined)
                                insertColumnsDirectiveApi.clear();
                            if (updateColumnsDirectiveApi != undefined)
                                updateColumnsDirectiveApi.clear();
                            if (identifierColumnsDirectiveApi != undefined)
                                identifierColumnsDirectiveApi.clear();

                            var tablePayload = {};
                            var Filter = {
                                SchemaId: schemaDirectiveApi.getSelectedIds()
                            };
                            tablePayload.filter = Filter;

                            loadTableDirective(tablePayload);

                        }
                    }
                }

            }

            $scope.scopeModel.onTableChanged = function () {
                if (tableDirectiveApi != undefined) {
                    var data = tableDirectiveApi.getSelectedIds()
                    if (data != undefined) {
                        if (tableSelectedPromiseDeferred != undefined) {
                            tableSelectedPromiseDeferred.resolve();
                        }
                        else {
                            var insertColumnsPayload = {};
                            var Filter = {
                                ConnectionStringId: connectionStringDirectiveApi.getSelectedIds(),
                                SchemaId: schemaDirectiveApi.getSelectedIds(),
                                TableId: tableDirectiveApi.getSelectedIds()
                            };
                            insertColumnsPayload.filter = Filter;

                            var updateColumnsPayload = {};
                            var Filter = {
                                ConnectionStringId: connectionStringDirectiveApi.getSelectedIds(),
                                SchemaId: schemaDirectiveApi.getSelectedIds(),
                                TableId: tableDirectiveApi.getSelectedIds()
                            };
                            updateColumnsPayload.filter = Filter;

                            var identifierColumnsPayload = {};
                            var Filter = {
                                ConnectionStringId: connectionStringDirectiveApi.getSelectedIds(),
                                SchemaId: schemaDirectiveApi.getSelectedIds(),
                                TableId: tableDirectiveApi.getSelectedIds()
                            };
                            identifierColumnsPayload.filter = Filter;

                            loadInsertColumnsDirective(insertColumnsPayload);
                            loadUpdateColumnsDirective(updateColumnsPayload);
                            loadIdentifierColumnsDirective(identifierColumnsPayload);

                        }
                    }
                }

            }



        }


        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                loadAllControls().finally(function () {
                    designEntity = undefined;
                });
            }
            else
                loadAllControls();
        }

        function loadConnectionStringDirective(connectionStringPayload) {
            var connectionStringLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            connectionStringReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(connectionStringDirectiveApi, connectionStringPayload, connectionStringLoadPromiseDeferred);
            });
            return connectionStringLoadPromiseDeferred.promise;
        }

        function loadSchemaDirective(schemaDirectivePayload) {
            var promises = [];
            var schemaDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            if (connectionStringSelectedPromiseDeferred != undefined)
                promises.push(connectionStringSelectedPromiseDeferred.promise);
            promises.push(schemaReadyPromiseDeferred.promise)
            UtilsService.waitMultiplePromises(promises).then(function (response) {

                VRUIUtilsService.callDirectiveLoad(schemaDirectiveApi, schemaDirectivePayload, schemaDirectiveLoadDeferred);
                connectionStringSelectedPromiseDeferred = undefined;
            });

            return schemaDirectiveLoadDeferred.promise;
        }

        function loadTableDirective(tableDirectivePayload) {
            var promises = [];
            var tableDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            if (schemaSelectedPromiseDeferred != undefined)
                promises.push(schemaSelectedPromiseDeferred.promise);
            promises.push(tableReadyPromiseDeferred.promise)
            UtilsService.waitMultiplePromises(promises).then(function (response) {

                VRUIUtilsService.callDirectiveLoad(tableDirectiveApi, tableDirectivePayload, tableDirectiveLoadDeferred);
                schemaSelectedPromiseDeferred = undefined;
            });

            return tableDirectiveLoadDeferred.promise;
        }

        function loadInsertColumnsDirective(insertColumnsDirectivePayload) {
            var promises = [];
            var insertColumnsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            if (tableSelectedPromiseDeferred != undefined)
                promises.push(tableSelectedPromiseDeferred.promise);
            promises.push(insertColumnsReadyPromiseDeferred.promise)
            UtilsService.waitMultiplePromises(promises).then(function (response) {

                VRUIUtilsService.callDirectiveLoad(insertColumnsDirectiveApi, insertColumnsDirectivePayload, insertColumnsDirectiveLoadDeferred);
                tableSelectedPromiseDeferred = undefined;
            });

            return insertColumnsDirectiveLoadDeferred.promise;
        }

        function loadUpdateColumnsDirective(updateColumnsDirectivePayload) {
            var promises = [];
            var updateColumnsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            if (tableSelectedPromiseDeferred != undefined)
                promises.push(tableSelectedPromiseDeferred.promise);
            promises.push(updateColumnsReadyPromiseDeferred.promise)
            UtilsService.waitMultiplePromises(promises).then(function (response) {

                VRUIUtilsService.callDirectiveLoad(updateColumnsDirectiveApi, updateColumnsDirectivePayload, updateColumnsDirectiveLoadDeferred);
                tableSelectedPromiseDeferred = undefined;
            });

            return updateColumnsDirectiveLoadDeferred.promise;
        }

        function loadIdentifierColumnsDirective(identifierColumnsDirectivePayload) {
            var promises = [];
            var identifierColumnsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            if (tableSelectedPromiseDeferred != undefined)
                promises.push(tableSelectedPromiseDeferred.promise);
            promises.push(identifierColumnsReadyPromiseDeferred.promise)
            UtilsService.waitMultiplePromises(promises).then(function (response) {

                VRUIUtilsService.callDirectiveLoad(identifierColumnsDirectiveApi, identifierColumnsDirectivePayload, identifierColumnsDirectiveLoadDeferred);
                tableSelectedPromiseDeferred = undefined;
            });

            return identifierColumnsDirectiveLoadDeferred.promise;
        }


        function loadAllControls() {
            
            function loadDirectives() {

                if (isEditMode) {
                    var promises = [];
                    connectionStringSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                    var connectionStringPayload = {};
                    if (designEntity != undefined)
                        connectionStringPayload.selectedIds = designEntity.ConnectionString;

                    promises.push(loadConnectionStringDirective(connectionStringPayload));

                    schemaSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                    var schemaPayload = {};
                    var Filter = {
                        ConnectionStringId: connectionStringPayload.selectedIds
                    };
                    schemaPayload.filter = schemaFilter;
                    if (designEntity != undefined)
                        schemaPayload.selectedIds = designEntity.Schema;

                    promises.push(loadSchemaDirective(schemaPayload));

                    tableSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                    var tablePayload = {};
                    var Filter = {
                        SchemaId: schemaPayload.selectedIds
                    };
                    tablePayload.filter = Filter;
                    if (designEntity != undefined)
                        tablePayload.selectedIds = designEntity.Table;

                    promises.push(loadTableDirective(tablePayload));

                    var insertColumnsPayload = {};
                    var insertColumnsFilter = {
                        ConnectionStringId: connectionStringPayload.selectedIds,
                        SchemaId: schemaPayload.selectedIds,
                        TableId: tablePayload.selectedIds
                    };
                    insertColumnsPayload.filter = insertColumnsFilter;
                    if (designEntity != undefined)
                        insertColumnsPayload.selectedIds = designEntity.InsertColumns;

                    promises.push(loadInsertColumnsDirective(insertColumnsPayload));

                    var updateColumnsPayload = {};
                    var updateColumnsFilter = {
                        ConnectionStringId: connectionStringPayload.selectedIds,
                        SchemaId: schemaPayload.selectedIds,
                        TableId: tablePayload.selectedIds
                    };
                    updateColumnsPayload.filter = updateColumnsFilter;
                    if (designEntity != undefined)
                        updateColumnsPayload.selectedIds = designEntity.UpdateColumns;

                    promises.push(loadUpdateColumnsDirective(updateColumnsPayload));

                    var identifierColumnsPayload = {};
                    var identifierColumnsFilter = {
                        ConnectionStringId: connectionStringPayload.selectedIds,
                        SchemaId: schemaPayload.selectedIds,
                        TableId: tablePayload.selectedIds
                    };
                    identifierColumnsPayload.filter = identifierColumnsFilter;
                    if (designEntity != undefined)
                        identifierColumnsPayload.selectedIds = designEntity.IdentifierColumns;

                    promises.push(loadIdentifierColumnsDirective(identifierColumnsPayload));



                    return UtilsService.waitMultiplePromises(promises);


                }

                else return loadDemoCountrySelector();
            }

            function setTitle() {
                if (isEditMode && generatedScriptDesignEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(generatedScriptDesignEntity.Name, "Design");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Generated Script Design");
            }

            function loadStaticData() {

                if (generatedScriptDesignEntity != undefined) {

                }
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSubviewSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
           .finally(function () {
               $scope.scopeModel.isLoading = false;
           });
        }

        function buildGeneratedScriptDesignObjectFromScope() {
            return {
                ConnectionStrings: connectionStringDirectiveApi.getSelectedIds(),
                Schema: schemaDirectiveApi.getSelectedIds(),
                Table: tableDirectiveApi.getSelectedIds(),
                InsertColumns: insertColumnsDirectiveApi.getSelectedIds(),
                UpdateColumns: updateColumnsDirectiveApi.getSelectedIds(),
                IdentifierColumns:identifierColumnsDirectiveApi.getSelectedIds()

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

    };
    appControllers.controller('Vanrise_Tools_GeneratedScriptDesignEditorController', generatedScriptDesignEditorController);
})(appControllers);