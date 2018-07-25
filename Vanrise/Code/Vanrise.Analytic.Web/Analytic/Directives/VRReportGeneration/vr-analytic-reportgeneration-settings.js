"use strict";

app.directive("vrAnalyticReportgenerationSettings", ['UtilsService', 'VRAnalytic_AutomatedReportProcessScheduledService', 'VRUIUtilsService', 'VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService', "VR_Analytic_AutomatedReportQuerySourceEnum", "VR_Analytic_QueryHandlerValidatorResultEnum",
    function (UtilsService, VRAnalytic_AutomatedReportProcessScheduledService, VRUIUtilsService, VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService, VR_Analytic_AutomatedReportQuerySourceEnum, VR_Analytic_QueryHandlerValidatorResultEnum) {
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
            templateUrl: "/Client/Modules/Analytic/Directives/VRReportGeneration/Templates/VrreportgenerationSettings.html"
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            var reportActionAPI;
            var reportActionReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var automatedReportQueryAPI;
            var automatedReportQueryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var context;
            var queries;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.columns = [];

                var gridAPI;

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                $scope.scopeModel.onReportActionReady = function (api) {
                    reportActionAPI = api;
                    reportActionReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onAutomatedReportQueryReady = function (api) {
                    automatedReportQueryAPI = api;
                    automatedReportQueryReadyPromiseDeferred.resolve();
                };


                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.getData = function () {
                    return {
                        Queries: automatedReportQueryAPI.getData(),
                        ReportAction:   reportActionAPI.getData()
                }
                };


                api.load = function (payload) {
                    var queries;
                    var reportAction;
                    if (payload != undefined)
                        context = payload.context;

                    if (payload != undefined && payload.Settings != undefined) {

                        queries = payload.Settings.Queries;
                        reportAction = payload.Settings.ReportAction;
                    }
                    function loadAutomatedReportQuery() {
                        var automatedReportQueryLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        automatedReportQueryReadyPromiseDeferred.promise.then(function () {
                            var automatedReportQueryPayload = {
                                Queries: queries != undefined ? queries : undefined,
                                context: getContext()
                            };
                            VRUIUtilsService.callDirectiveLoad(automatedReportQueryAPI, automatedReportQueryPayload, automatedReportQueryLoadPromiseDeferred);

                        });
                        return automatedReportQueryLoadPromiseDeferred.promise;
                    }
                    function loadReportActionSelector() {
                        var reportActionLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        reportActionReadyPromiseDeferred.promise.then(function () {
                            var reportActionPayload = {
                                reportAction: reportAction,
                                context: getContext()
                            }
                            VRUIUtilsService.callDirectiveLoad(reportActionAPI, reportActionPayload, reportActionLoadPromiseDeferred);

                        });
                        return reportActionLoadPromiseDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises([loadAutomatedReportQuery(), loadReportActionSelector()]);




                };               

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }




            function getColumns() {
                return automatedReportQueryAPI.getData();
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

                    var fields = [];
                    automatedReportDataSchemaPromise.then(function (response) {
                        if (response != undefined) {
                            for (var queryId in response) {
                                if (queryId == vrAutomatedReportQueryId) {
                                    var listSchemas = response[queryId] != undefined ? response[queryId].ListSchemas : undefined;
                                    for (var listSchemaName in listSchemas) {
                                        if (listSchemaName != "$type") {
                                            if (listSchemas[listSchemaName] != undefined) {
                                                var fieldSchemas = listSchemas[listSchemaName].FieldSchemas;
                                                for (var fieldSchemaName in fieldSchemas) {
                                                    if (fieldSchemaName != "$type") {
                                                        var fieldSchema = fieldSchemas[fieldSchemaName];
                                                        if (fieldSchema != undefined && fieldSchema != "$type") {
                                                            var field = fieldSchema.Field;
                                                            if (field != undefined) {
                                                                fields.push({
                                                                    FieldName: field.Name,
                                                                    FieldTitle: field.Title,
                                                                    Source: VR_Analytic_AutomatedReportQuerySourceEnum.MainTable
                                                                });
                                                            }
                                                        }
                                                    }
                                                }
                                                var subTablesSchemas = listSchemas[listSchemaName].SubTablesSchemas;
                                                for (var subTableId in subTablesSchemas) {
                                                    if (subTableId != "$type") {
                                                        var subTableSchema = subTablesSchemas[subTableId];
                                                        if (subTableSchema != undefined) {
                                                            fields.push({
                                                                FieldName: subTableSchema.SubTableTitle,
                                                                SubTableId: subTableId,
                                                                SubTableFields: subTableSchema.FieldSchemas,
                                                                Source: VR_Analytic_AutomatedReportQuerySourceEnum.SubTable
                                                            });
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
                return currentContext
            }


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
