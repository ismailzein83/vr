(appControllers); (function (appControllers) {
    "use strict";
    generatedScriptDesignEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Tools_GeneratedScriptService'];

    function generatedScriptDesignEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Tools_GeneratedScriptService) {

        var isEditMode;
        var tableDataGridApi;
        var tableDataGridPromiseDeferred = UtilsService.createPromiseDeferred();
        var selectedTableDataGridApi;
        var selectTableDataGridPromiseDeferred=UtilsService.createPromiseDeferred();
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

        var insertColumnsDirectiveApi;
        var insertColumnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var updateColumnsDirectiveApi;
        var updateColumnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var identifierColumnsDirectiveApi;
        var identifierColumnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        $scope.scopeModel = {};
        $scope.scopeModel.showGrid = false;
        $scope.scopeModel.showSelectedRowsGrid = false;
        $scope.scopeModel.setActionsEnablity = true;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                designEntity = parameters.design;
            }
            isEditMode = (designEntity != undefined);
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

            $scope.scopeModel.selectAll = function () {

                tableDataGridApi.selectAllAcounts();
            }

            $scope.scopeModel.deSelectAll = function () {

                tableDataGridApi.deselectAllAcounts();
            }

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

            $scope.scopeModel.onUpdateColumnsDirectiveReady = function (api) {
                updateColumnsDirectiveApi = api;
                updateColumnsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onIdentifierColumnsDirectiveReady = function (api) {
                identifierColumnsDirectiveApi = api;
                identifierColumnsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onConnectionStringChanged = function (value) {
                if (connectionStringDirectiveApi != undefined) {
                    if (value != undefined) {
                        if (connectionStringSelectedPromiseDeferred != undefined) {
                            connectionStringSelectedPromiseDeferred.resolve();
                        }
                        else {
                            if (tableDirectiveApi != undefined)
                                tableDirectiveApi.clear();
                            if (insertColumnsDirectiveApi != undefined)
                                insertColumnsDirectiveApi.clear();
                            if (updateColumnsDirectiveApi != undefined)
                                updateColumnsDirectiveApi.clear();
                            if (identifierColumnsDirectiveApi != undefined)
                                identifierColumnsDirectiveApi.clear();

                            var schemaPayload = {
                                filter: {
                                    ConnectionId: connectionStringDirectiveApi.getSelectedIds()
                                }
                            };
                            var setLoader = function (value) { $scope.scopeModel.isSchemaDirectiveloading = value; }
                            var schemaDirectiveLoadDeferred;

                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, schemaDirectiveApi, schemaPayload, setLoader, schemaDirectiveLoadDeferred);

                        }
                    }
                }

            };

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

                            var tablePayload = {
                                filter: {
                                    ConnectionId: connectionStringDirectiveApi.getSelectedIds(),
                                    SchemaName: schemaDirectiveApi.getSelectedIds()
                                }
                            };

                            var setLoader = function (value) { $scope.scopeModel.isTableDirectiveloading = value; }
                            var tableDirectiveLoadDeferred;
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, tableDirectiveApi, tablePayload, setLoader, tableDirectiveLoadDeferred);
                        }
                    }
                }

            };

            $scope.scopeModel.onTableChanged = function () {
                if (tableDirectiveApi != undefined) {
                    var data = tableDirectiveApi.getSelectedIds()
                    if (data != undefined) {
                        if (tableSelectedPromiseDeferred != undefined) {
                            tableSelectedPromiseDeferred.resolve();
                        }
                        else {

                            var columnsPayload = {
                                filter:{
                                    ConnectionId: connectionStringDirectiveApi.getSelectedIds(),
                                    SchemaName: schemaDirectiveApi.getSelectedIds(),
                                    TableName: tableDirectiveApi.getSelectedIds()
                                }
                            };
                           
                            var setLoader = function (value) { $scope.scopeModel.isInsertColumnsDirectiveloading = value; };
                            var insertColumnsDirectiveLoadDeferred;
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, insertColumnsDirectiveApi, columnsPayload, setLoader, insertColumnsDirectiveLoadDeferred);

                            var setLoader = function (value) { $scope.scopeModel.isUpdateColumnsDirectiveloading = value; };
                            var updateColumnsDirectiveLoadDeferred;
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, updateColumnsDirectiveApi, columnsPayload, setLoader, updateColumnsDirectiveLoadDeferred);

                            var setLoader = function (value) { $scope.scopeModel.isIdentifierColumnsDirectiveloading = value; };
                            var identifierColumnsDirectiveLoadDeferred;
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, identifierColumnsDirectiveApi, columnsPayload, setLoader, identifierColumnsDirectiveLoadDeferred);
                           
                        }
                    }
                }
            };

            //$scope.scopeModel.onIdentifierColumnsChanged = function () {
            //    if ($scope.scopeModel.showGrid) {
            //        tableDataGridApi.load(getFilter()).then(function(){

            //            if($scope.scopeModel.showSelectedRowsGrid){

            //                ar payload = { Query: tableDataGridApi.getData().gridQuery, ColumnNames: tableDataGridApi.getData().columnNames }; 
            //    $scope.scopeModel.showSelectedRowsGrid = true;

            //    selectTableDataGridPromiseDeferred.promise.then(function (response) {
            //        selectedTableDataGridApi.load(payload)});

            //            }

            //        }
            //    }
            //}

            $scope.scopeModel.generateTableDataGrid = function () {

             

            };

            $scope.scopeModel.generateSelectedTableDataGrid = function () {
                $scope.scopeModel.showSelectedRowsGrid = true;

                $scope.scopeModel.onSelectedTableDataGridReady = function (api) {
                    selectedTableDataGridApi = api;
                    selectTableDataGridPromiseDeferred.resolve();
                };

                var payload = { Query: tableDataGridApi.getData().gridQuery, ColumnNames: tableDataGridApi.getData().columnNames }; 

                selectTableDataGridPromiseDeferred.promise.then(function (response) {
                    selectedTableDataGridApi.load(payload)});

            };


            $scope.scopeModel.executeQuery = function () {
                $scope.scopeModel.showGrid = true;

                $scope.scopeModel.onTableDataGridReady = function (api) {
                    tableDataGridApi = api;
                    tableDataGridPromiseDeferred.resolve();
                };
                tableDataGridPromiseDeferred.promise.then(function (response) {
                    tableDataGridApi.load(getFilter());
                    $scope.scopeModel.showSelectedRowsGrid = false;
                    $scope.scopeModel.showSelectedRowsGrid = true;

                    //if (selectedTableDataGridApi) {
                    //    var payload = { Query: tableDataGridApi.getData().gridQuery, ColumnNames: tableDataGridApi.getData().columnNames };

                    //    selectedTableDataGridApi.load(payload);
                    //}
                });

            }

            $scope.scopeModel.disableSelectAll = true;
            $scope.scopeModel.disableDeSelectAll = true;

             
            function getFilter() {
         
                    return {
                        
                        context :{
                            triggerRetrieveData: function () {
                            gridQuery.BulkActionState = bulkActionDraftInstance.getBulkActionState();
                            gridApi.retrieveData(gridQuery)
                        },
                        hasItems: function () {
                            return $scope.scopeModel.tableData.length > 0;
                        },

                        setSelectAllEnablity: function (enablity) {

                            $scope.scopeModel.disableSelectAll = !enablity;
                        },
                        setDeselectAllEnablity:function (enablity) {
                            $scope.scopeModel.disableDeSelectAll = !enablity;
                        },
            
                        setActionsEnablity : function (enablity) {
                            $scope.scopeModel.setActionsEnablity = !enablity;
                        }
                    },
                        query: {
                            ConnectionId: connectionStringDirectiveApi.getSelectedIds(),
                            SchemaName:schemaDirectiveApi.getSelectedIds(),
                             TableName:tableDirectiveApi.getSelectedIds(),
                             WhereCondition: $scope.scopeModel.sqlFilter,
                            IdentifierColumns:identifierColumnsDirectiveApi.getSelectedIds()
                        }
                    };
                };
           

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





        function loadAllControls() {
            
            if (isEditMode) {
                connectionStringSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                schemaSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                tableSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
            }

            function loadConnectionStringDirective() {
                var connectionStringLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                connectionStringReadyPromiseDeferred.promise.then(function () {
                     var connectionStringPayload = { selectedIds: designEntity != undefined ? designEntity.ConnectionStringId : undefined };
                    VRUIUtilsService.callDirectiveLoad(connectionStringDirectiveApi, connectionStringPayload, connectionStringLoadPromiseDeferred);
                });
                return connectionStringLoadPromiseDeferred.promise;
            }

            function loadDirectives() {
                if (isEditMode) {
                    var promises = [];

                    function loadSchemaDirective() {
                        var promises = [];
                        var schemaDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        if (connectionStringSelectedPromiseDeferred != undefined)
                            promises.push(connectionStringSelectedPromiseDeferred.promise);
                        promises.push(schemaReadyPromiseDeferred.promise)
                        UtilsService.waitMultiplePromises(promises).then(function (response) {
                            var schemaPayload = {
                                selectedIds: designEntity.Schema,
                                filter: {
                                    ConnectionId: designEntity.ConnectionStringId
                                }
                            };
                            VRUIUtilsService.callDirectiveLoad(schemaDirectiveApi, schemaPayload, schemaDirectiveLoadDeferred);
                            connectionStringSelectedPromiseDeferred = undefined;
                        });

                        return schemaDirectiveLoadDeferred.promise;
                    }
                    promises.push(loadSchemaDirective());

                    function loadTableDirective() {
                        var promises = [];
                        var tableDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        if (schemaSelectedPromiseDeferred != undefined)
                            promises.push(schemaSelectedPromiseDeferred.promise);
                        promises.push(tableReadyPromiseDeferred.promise)
                        UtilsService.waitMultiplePromises(promises).then(function (response) {
                            var tableDirectivePayload = {
                                filter: {
                                    ConnectionId: designEntity.ConnectionStringId,
                                    SchemaName: designEntity.Schema
                                },
                                selectedIds: designEntity.Table
                            };
                            VRUIUtilsService.callDirectiveLoad(tableDirectiveApi, tableDirectivePayload, tableDirectiveLoadDeferred);
                            schemaSelectedPromiseDeferred = undefined;
                        });

                        return tableDirectiveLoadDeferred.promise;
                    }
                    promises.push(loadTableDirective());

                    function loadInsertColumnsDirective() {
                        var promises = [];
                        var insertColumnsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        if (tableSelectedPromiseDeferred != undefined)
                            promises.push(tableSelectedPromiseDeferred.promise);
                        promises.push(insertColumnsReadyPromiseDeferred.promise)
                        UtilsService.waitMultiplePromises(promises).then(function (response) {

                            var InsertColumnsNames = [];
                            for (var i = 0; i < designEntity.InsertColumns.length; i++) {
                                InsertColumnsNames.push(designEntity.InsertColumns[i].ColumnName)

                            }
                            var insertColumnsPayload = {
                                filter: {
                                    ConnectionId: designEntity.ConnectionStringId,
                                    TableName: designEntity.Table
                                },
                                selectedIds: InsertColumnsNames
                            }
                            VRUIUtilsService.callDirectiveLoad(insertColumnsDirectiveApi, insertColumnsPayload, insertColumnsDirectiveLoadDeferred);
                            tableSelectedPromiseDeferred = undefined;
                        });

                        return insertColumnsDirectiveLoadDeferred.promise;
                    }
                    promises.push(loadInsertColumnsDirective());

                    function loadUpdateColumnsDirective() {
                        var promises = [];
                        var updateColumnsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        if (tableSelectedPromiseDeferred != undefined)
                            promises.push(tableSelectedPromiseDeferred.promise);
                        promises.push(updateColumnsReadyPromiseDeferred.promise)
                        UtilsService.waitMultiplePromises(promises).then(function (response) {

                            var UpdateColumnsNames = [];
                            for (var i = 0; i < designEntity.UpdateColumns.length; i++) {
                                UpdateColumnsNames.push(designEntity.UpdateColumns[i].ColumnName)

                            }
                            var updateColumnsPayload = {
                                filter: {
                                    ConnectionId: designEntity.ConnectionStringId,
                                    TableName: designEntity.Table
                                },
                                selectedIds: UpdateColumnsNames
                            }

                            VRUIUtilsService.callDirectiveLoad(updateColumnsDirectiveApi, updateColumnsPayload, updateColumnsDirectiveLoadDeferred);
                            tableSelectedPromiseDeferred = undefined;
                        });

                        return updateColumnsDirectiveLoadDeferred.promise;
                    }
                    promises.push(loadUpdateColumnsDirective());

                    function loadIdentifierColumnsDirective() {
                        var promises = [];
                        var identifierColumnsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        if (tableSelectedPromiseDeferred != undefined)
                            promises.push(tableSelectedPromiseDeferred.promise);
                        promises.push(identifierColumnsReadyPromiseDeferred.promise)
                        UtilsService.waitMultiplePromises(promises).then(function (response) {

                            var IdentifierColumnsNames = [];
                            for (var i = 0; i < designEntity.IdentifierColumns.length; i++) {
                                IdentifierColumnsNames.push(designEntity.IdentifierColumns[i].ColumnName)

                            }
                            var identifierColumnsPayload = {
                                filter: {
                                    ConnectionId: designEntity.ConnectionStringId,
                                    TableName: designEntity.Table
                                },
                                selectedIds: IdentifierColumnsNames
                            }
                            VRUIUtilsService.callDirectiveLoad(identifierColumnsDirectiveApi, identifierColumnsPayload, identifierColumnsDirectiveLoadDeferred);
                            tableSelectedPromiseDeferred = undefined;
                        });

                        return identifierColumnsDirectiveLoadDeferred.promise;
                    }
                    promises.push(loadIdentifierColumnsDirective());

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
            var insertColumnsNames = [];
            var insertColumnsData=insertColumnsDirectiveApi.getSelectedIds();
            var updateColumnsData=updateColumnsDirectiveApi.getSelectedIds();
            var identifierColumnsData=identifierColumnsDirectiveApi.getSelectedIds();
            for (var j = 0; j < insertColumnsData.length; j++) {
                insertColumnsNames.push(insertColumnsData[j].ColumnName)
            }
            var updateColumnsNames = [];
            for (var j = 0; j < updateColumnsData.length; j++) {
                updateColumnsNames.push(updateColumnsData[j].ColumnName)
            }
            var identifierColumnsNames = [];
            for (var j = 0; j < identifierColumnsData.length; j++) {
                identifierColumnsNames.push(identifierColumnsData[j].ColumnName)
            }

            $scope.addText(angular.toJson({
                ConnectionStringId: connectionStringDirectiveApi.getSelectedIds(),
                Schema: schemaDirectiveApi.getSelectedIds(),
                Table: tableDirectiveApi.getSelectedIds(),
                InsertColumns: insertColumnsDirectiveApi.getSelectedIds(),
                UpdateColumns: updateColumnsDirectiveApi.getSelectedIds(),
                IdentifierColumns: identifierColumnsDirectiveApi.getSelectedIds(),
                DataRows: selectedTableDataGridApi.getData().tableRows
            }));
            
            return {
                
                ConnectionStringId: connectionStringDirectiveApi.getSelectedIds(),
                Schema: schemaDirectiveApi.getSelectedIds(),
                Table: tableDirectiveApi.getSelectedIds(),
                InsertColumns: insertColumnsDirectiveApi.getSelectedIds(),
                UpdateColumns: updateColumnsDirectiveApi.getSelectedIds(),
                IdentifierColumns: identifierColumnsDirectiveApi.getSelectedIds(),
                InsertColumnNames: insertColumnsNames.join(),
                UpdateColumnNames:updateColumnsNames.join(),
                IdentifierColumnNames:identifierColumnsNames.join(),
                DataRows: selectedTableDataGridApi.getData().tableRows
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