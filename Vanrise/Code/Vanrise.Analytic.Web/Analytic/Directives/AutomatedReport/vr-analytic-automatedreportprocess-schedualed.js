"use strict";

app.directive("vrAnalyticAutomatedreportprocessSchedualed", ['UtilsService', 'VRAnalytic_AutomatedReportProcessScheduledService',  'VRUIUtilsService','VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService',
    function (UtilsService,VRAnalytic_AutomatedReportProcessScheduledService, VRUIUtilsService, VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService) {
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
        templateUrl: "/Client/Modules/Analytic/Directives/AutomatedReport/Templates/AutomatedReportProcessScheduled.html"
    };

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;
        
        var handlerSettingsAPI;
        var handlerSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var context;
        var queries;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.columns = [];

            var gridAPI;

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.scopeModel.onHandlerSettingsAutomatedReportReady = function (api) {
                handlerSettingsAPI = api;
                handlerSettingsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.addQuery = function () {

                var onQueryAdded = function (obj) {
                    $scope.scopeModel.columns.push(obj);
                };
                VRAnalytic_AutomatedReportProcessScheduledService.addQuery(onQueryAdded, getContext());
            };

            $scope.scopeModel.removeColumn = function (dataItem) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.columns, dataItem.id, 'id');
                if (index > -1) {
                    $scope.scopeModel.columns.splice(index, 1);
                }
            };


            $scope.scopeModel.validateColumns = function () {
                if ($scope.scopeModel.columns.length == 0) {
                    return 'At least one column must be added.';
                }
                var columnNames = [];
                for (var i = 0; i < $scope.scopeModel.columns.length; i++) {
                    var column = $scope.scopeModel.columns[i];
                    if (column.QueryTitle != undefined) {
                        columnNames.push(column.QueryTitle.toUpperCase());
                    }
                }
                while (columnNames.length > 0) {
                    var nameToValidate = columnNames[0];
                    columnNames.splice(0, 1);
                    if (!validateName(nameToValidate, columnNames)) {
                        return 'Two or more columns have the same name.';
                    }
                }
                return null;
                function validateName(name, array) {
                    for (var j = 0; j < array.length; j++) {
                        if (array[j] == name)
                            return false;
                    }
                    return true;
                }
            };

            defineMenuActions();
            defineAPI();
        }
        function defineAPI() {
            var api = {};
            api.getData = function () {
                return {
                    $type: "Vanrise.Analytic.BP.Arguments.VRAutomatedReportProcessInput,Vanrise.Analytic.BP.Arguments",
                    Name: $scope.scopeModel.name,
                    Queries: $scope.scopeModel.columns.length > 0 ? getColumns() : null,
                    Handler: {
                        Settings: handlerSettingsAPI.getData(),
                    },
                };
              
            };
           
           
            api.load = function (payload) {
                if (payload != undefined)
                    context = payload.context;
                if (payload != undefined && payload.data != undefined) {

                    var queries = payload.data.Queries;
                    var handler = payload.data.Handler;

                    for (var i = 0; i < queries.length; i++) {
                        var query = queries[i];
                        var gridItem = {
                            DefinitionId: query.DefinitionId,
                            VRAutomatedReportQueryId: query.VRAutomatedReportQueryId,
                            QueryTitle: query.QueryTitle,
                            Settings: query.Settings,
                        };
                        $scope.scopeModel.columns.push(gridItem);
                    }
                }
                loadHandlerSelector();
                function loadHandlerSelector() {
                    var handlerSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    handlerSettingsReadyPromiseDeferred.promise.then(function () {
                        var handlerPayload = {
                            settings: handler != undefined && handler.Settings != undefined ? handler.Settings : undefined,
                            context: getContext()
                        };
                        VRUIUtilsService.callDirectiveLoad(handlerSettingsAPI, handlerPayload, handlerSettingsLoadPromiseDeferred);

                    });
                    return handlerSettingsLoadPromiseDeferred.promise;
                }
            };

            api.validate = function () {
                var input = {
                    Queries: getColumns(),
                    HandlerSettings: handlerSettingsAPI.getData()
                };
                var validationPromise = UtilsService.createPromiseDeferred();
                VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService.ValidateQueryAndHandlerSettings(input).then(function (response) {
                    validationPromise.resolve(response);
                });
                return validationPromise.promise;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getColumns() {
            var columns = [];
            for (var i = 0; i < $scope.scopeModel.columns.length; i++) {
                var column = $scope.scopeModel.columns[i];
                columns.push({
                    $type: "Vanrise.Analytic.Entities.VRAutomatedReportQuery,Vanrise.Analytic.Entities",
                    DefinitionId: column.DefinitionId,
                    VRAutomatedReportQueryId: column.VRAutomatedReportQueryId,
                    QueryTitle: column.QueryTitle,
                    Settings: column.Settings,
                });
            }
            return columns;
        }

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editQuery,
            }];
        }

        function editQuery(object) {
            var onQueryUpdated = function (obj) {
                var index = $scope.scopeModel.columns.indexOf(object);
                $scope.scopeModel.columns[index] = obj;
            };
            VRAnalytic_AutomatedReportProcessScheduledService.editQuery(object, onQueryUpdated, getContext());
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            currentContext.getQueryInfo = function () {
                return getColumns();
            };

            currentContext.getQueryListNames = function (vrAutomatedReportQueryId) {
                var automatedReportDataSchemaPromise = getAutomatedReportDataSchema();
                var automatedReportDataSchemaPromiseDeferred = UtilsService.createPromiseDeferred();
                var listNames = [];
                automatedReportDataSchemaPromise.then(function (response) {
                    if (response != undefined) {
                        var dataSchema = response;
                        for (var queryId in dataSchema) {
                            if (queryId == vrAutomatedReportQueryId) {
                                var listSchemas = dataSchema[queryId] != undefined ? dataSchema[queryId].ListSchemas : undefined;
                                for (var listSchemaName in listSchemas) {
                                    if (listSchemaName != "$type") {
                                        listNames.push(listSchemaName);
                                    }
                                }
                            }
                        }
                    }
                    automatedReportDataSchemaPromiseDeferred.resolve(listNames);
                });
                return automatedReportDataSchemaPromiseDeferred.promise;
            };
            currentContext.getQueryFields = function (vrAutomatedReportQueryId) {
                var automatedReportDataSchemaPromise = getAutomatedReportDataSchema();
                var automatedReportDataSchemaPromiseDeferred = UtilsService.createPromiseDeferred();

                var fields = {};
                automatedReportDataSchemaPromise.then(function (response) {
                    if (response != undefined) {
                        for (var queryId in response) {
                            if (queryId == vrAutomatedReportQueryId) {
                                var listSchemas = response[queryId] != undefined ? response[queryId].ListSchemas : undefined;
                                for (var listSchemaName in listSchemas) {
                                    if (listSchemaName != "$type") {
                                        if (listSchemas[listSchemaName] != undefined) {
                                            var fieldSchemas = listSchemas[listSchemaName].FieldSchemas;
                                            if (fieldSchemas != undefined) {
                                                for (var fieldSchemaName in fieldSchemas) {
                                                    if (fieldSchemaName != "$type") {
                                                        var fieldSchema = fieldSchemas[fieldSchemaName];
                                                        if (fieldSchema != undefined && fieldSchema != "$type") {
                                                            var field = fieldSchema.Field;
                                                            if (field != undefined) {
                                                                fields[field.Name] = field.Title;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    automatedReportDataSchemaPromiseDeferred.resolve(fields);
                });
                return automatedReportDataSchemaPromiseDeferred.promise;
            };
            return currentContext;
        };
        

        function getSchemaList() {
            var queries = getColumns();
            if (queries != undefined) {
                var schemaList = {};
                schemaList.Queries = [];
                for (var i = 0; i < queries.length; i++) {
                    schemaList.Queries.push(queries[i]);
                }
            }
            return schemaList;
        }

        function getAutomatedReportDataSchema() {
            var automatedReportDataSchemaPromiseDeferred = UtilsService.createPromiseDeferred();
            var input = getSchemaList();
            VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService.GetAutomatedReportDataSchema(input).then(function (response) {
                automatedReportDataSchemaPromiseDeferred.resolve(response);
            });
            return automatedReportDataSchemaPromiseDeferred.promise;
        }
    }

    return directiveDefinitionObject;
}]);
