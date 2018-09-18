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
            };

            $scope.scopeModel.validateGrid = function () {
                if ($scope.scopeModel.fields.length == 0)
                    return "At least one field must be added.";
            };
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.scopeModel.addField = function () {
                var flatFileField = getFlatFileField();
                $scope.scopeModel.fields.push(flatFileField);
            };
            $scope.scopeModel.removeField = function (dataItem)
            {
                var index = $scope.scopeModel.fields.indexOf(dataItem);
                $scope.scopeModel.fields.splice(index, 1);
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
                                });
                            }
                        });
                    }
                }
                return UtilsService.waitMultiplePromises(promises).then(function () {
                    $scope.scopeModel.isLoading = false;
                });
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

        function getFlatFileField() {
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
    }

    return directiveDefinitionObject;
}
]);