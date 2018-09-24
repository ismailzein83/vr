"use strict";
app.directive("vrAnalyticFlatfileFilegenerator", ["UtilsService", "VRAnalytic_AdvancedExcelFileGeneratorService", "VRNotificationService", "VRUIUtilsService",  "VR_Analytic_AutomatedReportQuerySourceEnum",
function (UtilsService, VRAnalytic_AdvancedExcelFileGeneratorService, VRNotificationService, VRUIUtilsService,  VR_Analytic_AutomatedReportQuerySourceEnum) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var flatFile = new FlatFile($scope, ctrl, $attrs);
            flatFile.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/FileGenerator/FlatFile/Templates/FlatFileGeneratorTemplate.html"
    };


    function FlatFile($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var context;

        var queryNameSelectorAPI;
        var queryNameSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var listNameSelectorAPI;
        var listNameSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;

        var queries;
        var isEditMode;

        var allFields;

        function initializeController() {
            var promises = [queryNameSelectorReadyDeferred.promise, listNameSelectorReadyDeferred.promise];

            $scope.scopeModel = {};

            $scope.scopeModel.queries = [];
            $scope.scopeModel.listNames = [];
            $scope.scopeModel.fields = [];

            $scope.scopeModel.fileExtension = "csv";
            $scope.scopeModel.delimiter = ",";

            $scope.scopeModel.onQueryNameSelectorReady = function (api) {
                queryNameSelectorAPI = api;
                queryNameSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onListNameSelectorReady = function (api) {
                listNameSelectorAPI = api;
                listNameSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.validateFileExtension = function () {
                if ($scope.scopeModel.fileExtension != undefined && $scope.scopeModel.fileExtension.length > 0) {
                    if (UtilsService.contains($scope.scopeModel.fileExtension, "."))
                        return "Please enter the extension name only.";
                }
                return null;
            };

            $scope.scopeModel.onQuerySelectionChanged = function (query) {
                if (query != undefined) {
                    $scope.scopeModel.fields.length = 0;
                    $scope.scopeModel.listNames.length = 0;
                    $scope.scopeModel.isLoadingListNames = true;
                    var listNameSelectorDataPromise = context.getQueryListNames(query.value);
                    promises.push(listNameSelectorDataPromise);
                    listNameSelectorDataPromise.then(function (listNames) {
                        if (listNames != undefined) {
                            for (var i = 0; i < listNames.length; i++) {
                                var listName = listNames[i];
                                $scope.scopeModel.listNames.push({
                                    description: listName,
                                    value: listName
                                });
                            }
                        }
                        listNameSelectorAPI.selectIfSingleItem();
                    }).then(function () {
                        $scope.scopeModel.isLoadingListNames = false;
                    });
                }
                else {
                    $scope.scopeModel.fields.length = 0;
                    $scope.scopeModel.listNames.length = 0;
                }
            };

            $scope.scopeModel.validateGrid = function () {
                if ($scope.scopeModel.fields.length == 0)
                    return "At least one field must be added.";
            };
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.scopeModel.addAllFields = function () {
                if (context != undefined && context.getQueryFields != undefined && typeof (context.getQueryFields) == "function") {
                    var fieldsPromise = getAllFields($scope.scopeModel.querySelected.value);
                    fieldsPromise.then(function (fields) {
                        if (fields != undefined) {
                            for (var i = 0; i < fields.length; i++) {
                                var flatFileField = getFlatFileField(fields[i]);
                                $scope.scopeModel.fields.push(flatFileField);
                            }
                            disableTestGenerate();
                        }
                    });
                }
            };

            $scope.scopeModel.addField = function () {
                var flatFileField = getFlatFileField();
                $scope.scopeModel.fields.push(flatFileField);
                disableTestGenerate();
            };
            $scope.scopeModel.removeField = function (dataItem)
            {
                var index = $scope.scopeModel.fields.indexOf(dataItem);
                $scope.scopeModel.fields.splice(index, 1);
                disableTestGenerate();
            };
                UtilsService.waitMultiplePromises(promises).then(function () {
                defineAPI();
            });
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                $scope.scopeModel.isLoading = true;
                var promises = [];
                var queryArrayPromise = UtilsService.createPromiseDeferred();
                if (payload != undefined) {
                    context = payload.context;
                    if (context != undefined) {
                        promises.push(queryArrayPromise.promise);
                        queries = context.getQueryInfo();
                        queryNameSelectorReadyDeferred.promise.then(function () {
                            if (queries != undefined) {
                                for (var i = 0; i < queries.length; i++) {
                                    var query = queries[i];
                                    $scope.scopeModel.queries.push({
                                        description: query.QueryTitle,
                                        value: query.VRAutomatedReportQueryId
                                    });
                                }
                                queryNameSelectorAPI.selectIfSingleItem();
                                queryArrayPromise.resolve();
                            }
                            else {
                                queryArrayPromise.resolve();
                            }
                        });
                    }
                    if (payload.fileGenerator != undefined) {
                        $scope.scopeModel.fileExtension = payload.fileGenerator.FileExtension;
                        $scope.scopeModel.delimiter = payload.fileGenerator.Delimiter;
                        $scope.scopeModel.withoutHeaders = payload.fileGenerator.WithoutHeaders;
                        queryArrayPromise.promise.then(function () {
                            $scope.scopeModel.listNameSelected = UtilsService.getItemByVal($scope.scopeModel.listNames, payload.fileGenerator.ListName, 'value');
                            $scope.scopeModel.querySelected = UtilsService.getItemByVal($scope.scopeModel.queries, payload.fileGenerator.VRAutomatedReportQueryId, 'value');
                            if (payload.fileGenerator.Fields != undefined && payload.fileGenerator.Fields.length > 0) {
                                var allFieldsPromise = getAllFields(payload.fileGenerator.VRAutomatedReportQueryId);
                                allFieldsPromise.then(function () {
                                    for (var i = 0; i < payload.fileGenerator.Fields.length; i++) {
                                        var field = payload.fileGenerator.Fields[i];
                                        var selectedField = UtilsService.getItemByVal(allFields, field.FieldName, 'FieldName');
                                        $scope.scopeModel.fields.push({
                                            editedTitle: field.FieldTitle,
                                            selectedField: {
                                                description: selectedField.FieldTitle,
                                                value: selectedField.FieldName,
                                                source: VR_Analytic_AutomatedReportQuerySourceEnum.MainTable
                                            }
                                        });
                                    }
                                    disableTestGenerate();
                                });
                            }
                        });
                    }
                }
                return UtilsService.waitMultiplePromises(promises).then(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };
            api.reload = function (newQueries) {
                if (queries != undefined && newQueries != undefined) {
                    if (queries.length != newQueries.length) {
                        if (queries.length < newQueries.length) {
                            var queryAdded = newQueries[newQueries.length - 1];
                            queries.push(queryAdded);
                            $scope.scopeModel.queries.push({
                                description: queryAdded.QueryTitle,
                                value: queryAdded.VRAutomatedReportQueryId
                            });
                        }
                        else {
                            var queryDeleted;
                            for (var i = 0; i < queries.length; i++) {
                                var oldQuery = queries[i];
                                var oldQueryIndex = UtilsService.getItemIndexByVal(newQueries, oldQuery.VRAutomatedReportQueryId, 'VRAutomatedReportQueryId');
                                if (oldQueryIndex == -1) {
                                    queryDeleted = oldQuery;
                                }
                            }
                            if (queryDeleted != undefined) {
                                var matchingDeletedQueryIndex = UtilsService.getItemIndexByVal($scope.scopeModel.queries, queryDeleted.VRAutomatedReportQueryId, 'value');
                                $scope.scopeModel.queries.splice(matchingDeletedQueryIndex, 1);
                                var matchingDeletedQueryArrayIndex = UtilsService.getItemIndexByVal(queries, queryDeleted.VRAutomatedReportQueryId, 'VRAutomatedReportQueryId');
                                queries.splice(matchingDeletedQueryArrayIndex, 1);
                                if ($scope.scopeModel.queries.length == 0 || ($scope.scopeModel.querySelected != undefined && $scope.scopeModel.querySelected.value == queryDeleted.VRAutomatedReportQueryId)) {
                                    $scope.scopeModel.querySelected = undefined;
                                    $scope.scopeModel.listNameSelected = undefined;
                                    $scope.scopeModel.listNames.length = 0;
                                }
                                disableTestGenerate();
                                }
                            }
                            }
                    else {
                        var queryUpdated;
                        for (var i = 0; i < queries.length; i++) {
                            var oldQuery = queries[i];
                            var newQuery = newQueries[i];
                            if (oldQuery.Settings != newQuery.Settings) {
                                queryUpdated = newQuery;
                                break;
                            }
                        }
                        if (queryUpdated != undefined) {
                            var matchingQuery = UtilsService.getItemByVal($scope.scopeModel.queries, queryUpdated.VRAutomatedReportQueryId, 'value');
                            if (matchingQuery != undefined) {
                                if (matchingQuery.description != queryUpdated.QueryTitle) {
                                    var matchingUpdatedQueryIndex = $scope.scopeModel.queries.indexOf(matchingQuery);
                                    var matchingUpdatedQueryArrayIndex = UtilsService.getItemIndexByVal(queries, matchingQuery.value, 'VRAutomatedReportQueryId');
                                    queries[matchingUpdatedQueryArrayIndex].QueryTitle = queryUpdated.QueryTitle;
                                    $scope.scopeModel.queries[matchingUpdatedQueryIndex].description = queryUpdated.QueryTitle;
                                    if ($scope.scopeModel.queries.length == 1 && $scope.scopeModel.queries[0].value == queryUpdated.VRAutomatedReportQueryId) {
                                        $scope.scopeModel.querySelected.description = queryUpdated.QueryTitle;
                                    }
                                }
                                var newFieldsPromise = getAllFields(matchingQuery.value);
                                newFieldsPromise.then(function (newFields) {
                                for (var i = 0; i < $scope.scopeModel.fields.length; i++) {
                                var field = $scope.scopeModel.fields[i];
                                    $scope.scopeModel.fields[i].fields = newFields;
                                    allFields = newFields;
                                    if (field.selectedField != undefined) {
                                        var selectedFieldIndex = UtilsService.getItemIndexByVal(newFields, field.selectedField.value, 'value');
                                        if (selectedFieldIndex == -1) {
                                            $scope.scopeModel.fields[i].selectedField = undefined;
                                            $scope.scopeModel.fields[i].editedTitle = undefined;
                                            }
                                        }
                                    }
                                });
                            }
                        }
                    }
                }
                else if (queries == null && newQueries != undefined) {
                    queries = [];
                    var queryAdded = newQueries[newQueries.length - 1];
                    queries.push(queryAdded);
                    $scope.scopeModel.queries.push({
                        description: queryAdded.QueryTitle,
                        value: queryAdded.VRAutomatedReportQueryId
                    });
                    queryNameSelectorReadyDeferred.promise.then(function() {
                        queryNameSelectorAPI.selectIfSingleItem();
                    });
                }
                queries = newQueries;
            };

            api.getData = function () {
                function getFields() {
                    var flatFileFields = [];
                    if ($scope.scopeModel.fields.length > 0) {
                        for (var i = 0; i < $scope.scopeModel.fields.length; i++) {
                            var field = $scope.scopeModel.fields[i];
                            if (field.selectedField != undefined) {
                                flatFileFields.push({
                                    FieldName: field.selectedField.value,
                                    FieldTitle: field.editedTitle
                                });
                            }
                        }
                    }
                    return flatFileFields;
                }
                var obj = {
                    $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators.FlatFileGenerator,Vanrise.Analytic.MainExtensions",
                    FileExtension: $scope.scopeModel.fileExtension,
                    Delimiter: $scope.scopeModel.delimiter,
                    Fields: getFields(),
                    WithoutHeaders: $scope.scopeModel.withoutHeaders,
                    ListName: $scope.scopeModel.listNameSelected.value,
                    VRAutomatedReportQueryId: $scope.scopeModel.querySelected.value
                };
                return obj;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getFlatFileField(selectedField) {
            var flatFileField = {};

            flatFileField.onFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            flatFileField.fieldSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            flatFileField.fields = [];
            flatFileField.onFieldSelectorReady = function (api) {
                flatFileField.fieldSelectorAPI = api;
                flatFileField.onFieldSelectorReadyDeferred.resolve();

                flatFileField.onFieldSelectorReadyDeferred.promise.then(function () {
                    var fieldSelectorReadyPromiseDeferred = getAllFields($scope.scopeModel.querySelected.value);
                    fieldSelectorReadyPromiseDeferred.then(function (fields) {
                        if (fields != undefined) {
                            flatFileField.fields = fields;
                            flatFileField.fieldSelectorLoadPromiseDeferred.resolve();
                        }
                        flatFileField.fieldSelectorAPI.selectIfSingleItem();
                        if (selectedField != undefined) {
                            flatFileField.selectedField = selectedField;
                        }
                    });
                });
            };
            flatFileField.onFieldSelectionChanged = function (value) {
                if (value != undefined) {
                    flatFileField.editedTitle = value.description;
                }
            };


            return flatFileField;
        }

        function getAllFields(selectedQueryId) {
            var fieldsPromise = context.getQueryFields(selectedQueryId);
            var fieldsArrayPromise = UtilsService.createPromiseDeferred();
            var fieldsArray = [];
            fieldsPromise.then(function (fields) {
                if (fields != undefined) {
                    allFields = fields;
                    for (var i = 0; i < fields.length; i++) {
                        var field = fields[i];
                        if (field.Source == VR_Analytic_AutomatedReportQuerySourceEnum.MainTable) {
                            fieldsArray.push({
                                description: field.FieldTitle,
                                value: field.FieldName,
                                source: field.Source
                            });
                        }
                    }
                    fieldsArrayPromise.resolve(fieldsArray);
                }
            });
            return fieldsArrayPromise.promise;
        }

        function getContext() {
            var currentContext = UtilsService.cloneObject(context);
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }
        
        function disableTestGenerate() {
            if (context !=undefined && context.disableTestGenerateButton != undefined && typeof (context.disableTestGenerateButton) == 'function') {
                if ($scope.scopeModel.fields.length == 0) {
                    context.disableTestGenerateButton(true);
                }
                else {
                    context.disableTestGenerateButton(false);
                }
            }
        }
    }

    return directiveDefinitionObject;
}
]);