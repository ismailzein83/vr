"use strict";

appControllers.directive("vrDevtoolsGeneratedScriptSelectVariable", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "="
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new Delete($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_DevTools/Elements/GeneratedScript/Directives/VRGeneratedScriptMainExtensions/VariableSettings/Templates/VRGeneratedScriptSelectVariable.html"
        };

        function Delete($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            $scope.scopeModel = {};
            var connectionStringId;
            var isEditMode;
            var tableDirectiveApi;
            var tableReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var tableSelectedPromiseDeferred;

            var schemaDirectiveApi;
            var schemaReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var schemaSelectedPromiseDeferred;

            var columnsDirectiveApi;
            var columnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var filterColumnsDirectiveApi;
            var filterColumnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var filtrColumnSelectedPromiseDeferred;

            $scope.scopeModel.onSchemaDirectiveReady = function (api) {
                schemaDirectiveApi = api;
                schemaReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onTableDirectiveReady = function (api) {
                tableDirectiveApi = api;
                tableReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onColumnDirectiveReady = function (api) {
                columnsDirectiveApi = api;
                columnsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onFilterColumnDirectiveReady = function (api) {
                filterColumnsDirectiveApi = api;
                filterColumnsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onSchemaChanged = function (value) {
                if (schemaDirectiveApi != undefined) {
                    var data = schemaDirectiveApi.getSelectedIds();
                    if (data != undefined) {
                        if (schemaSelectedPromiseDeferred != undefined) {
                            schemaSelectedPromiseDeferred.resolve();
                        }
                        else {

                            if (columnsDirectiveApi != undefined)
                                columnsDirectiveApi.clear();
                            if (filterColumnsDirectiveApi != undefined)
                                filterColumnsDirectiveApi.clear();
                            var tablePayload = {
                                filter: {
                                    ConnectionId: connectionStringId,
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

                            var columnsPayload = {
                                filter: {
                                    ConnectionId: connectionStringId,
                                    SchemaName: schemaDirectiveApi.getSelectedIds(),
                                    TableName: tableDirectiveApi.getSelectedIds()
                                }
                                
                            };
                            var setLoader = function (value) { $scope.scopeModel.isColumnsDirectiveloading = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, columnsDirectiveApi, columnsPayload, setLoader);

                            var setFilterColumnsLoader = function (value) { $scope.scopeModel.isFilterColumnsDirectiveloading = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, filterColumnsDirectiveApi, columnsPayload, setFilterColumnsLoader);

                        }
                    }
                }
            };

       
            function initializeController() {
                defineAPI();

                function loadSchemaDirective(payload) {
                    var schemaDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    schemaReadyPromiseDeferred.promise.then(function (response) {
                        var schemaPayload = {
                            selectedIds: payload.settings!=undefined?payload.settings.SchemaName:undefined,
                            filter: {
                                ConnectionId: connectionStringId
                            }
                        };
                        VRUIUtilsService.callDirectiveLoad(schemaDirectiveApi, schemaPayload, schemaDirectiveLoadDeferred);
                    });
                    return schemaDirectiveLoadDeferred.promise;
                }

                function loadTableDirective(payload) {
                    var promises = [];
                    var tableDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    if (schemaSelectedPromiseDeferred != undefined)
                        promises.push(schemaSelectedPromiseDeferred.promise);
                    promises.push(tableReadyPromiseDeferred.promise);
                    UtilsService.waitMultiplePromises(promises).then(function (response) {
                        var tableDirectivePayload = {
                            filter: {
                                ConnectionId: connectionStringId,
                                SchemaName: payload.settings.SchemaName
                            },
                            selectedIds: payload.settings.TableName
                        };
                        VRUIUtilsService.callDirectiveLoad(tableDirectiveApi, tableDirectivePayload, tableDirectiveLoadDeferred);
                        schemaSelectedPromiseDeferred = undefined;
                    });

                    return tableDirectiveLoadDeferred.promise;
                }

                function loadColumnsDirective(payload) {
                    var promises = [];
                    var columnsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    if (tableSelectedPromiseDeferred != undefined)
                        promises.push(tableSelectedPromiseDeferred.promise);
                    promises.push(columnsReadyPromiseDeferred.promise);
                    UtilsService.waitMultiplePromises(promises).then(function (response) {
                          var columnsPayload = {
                                filter: {
                                    ConnectionId: connectionStringId,
                                    SchemaName: payload.settings.SchemaName,
                                    TableName: payload.settings.TableName
                                },
                              selectedIds: payload.settings.ColumnName
                            };
                        
                        VRUIUtilsService.callDirectiveLoad(columnsDirectiveApi, columnsPayload, columnsDirectiveLoadDeferred);
                        tableSelectedPromiseDeferred = undefined;
                    });

                    return columnsDirectiveLoadDeferred.promise;
                }
                function loadFilterColumnsDirective(payload) {
                    var promises = [];
                    var filterColumnsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    if (tableSelectedPromiseDeferred != undefined)
                        promises.push(tableSelectedPromiseDeferred.promise);
                    promises.push(filterColumnsReadyPromiseDeferred.promise);
                    UtilsService.waitMultiplePromises(promises).then(function (response) {
                        var  filterColumnsPayload = {
                                filter: {
                                    ConnectionId: connectionStringId,
                                    SchemaName: payload.settings.SchemaName,
                                    TableName: payload.settings.TableName
                                },
                                selectedIds: payload.settings.FilterColumnName
                            };
                        VRUIUtilsService.callDirectiveLoad(filterColumnsDirectiveApi, filterColumnsPayload, filterColumnsDirectiveLoadDeferred);
                    });

                    return filterColumnsDirectiveLoadDeferred.promise;
                }
                function defineAPI() {
                    var api = {};

                    api.load = function (payload) {
                        var promises = [];
                        if (payload != undefined) {
                            
                            connectionStringId = payload.connectionStringId;
                            promises.push(loadSchemaDirective(payload));
                            if (payload.isEditMode) {
                                schemaSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                                tableSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                                promises.push(loadTableDirective(payload));
                                promises.push(loadColumnsDirective(payload));
                                promises.push(loadFilterColumnsDirective(payload));
                                $scope.scopeModel.filterColumnValue = payload.settings.FilterColumnValue;
                            }
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    };

                    api.getData = function () {
                        return {
                            $type: "Vanrise.DevTools.MainExtensions.SelectGeneratedScriptVariable,Vanrise.DevTools.MainExtensions",
                            TableName: tableDirectiveApi.getSelectedIds(),
                            SchemaName: schemaDirectiveApi.getSelectedIds(),
                            ColumnName: columnsDirectiveApi.getSelectedIds(),
                            FilterColumnName: filterColumnsDirectiveApi.getSelectedIds(),
                            FilterValue: $scope.scopeModel.filterColumnValue
                        };
                    };

                    if (ctrl.onReady != null)
                        ctrl.onReady(api);
                }
            }
        }

        return directiveDefinitionObject;

    }
]);