"use strict";
app.directive("vrAnalyticRecordsearchquerydefinitionsettingsRuntimeeditor", ["VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService", "UtilsService", "VRUIUtilsService", "VR_Analytic_OrderDirectionEnum", 'VR_GenericData_DataRecordFieldAPIService', 'VR_GenericData_GenericBEDefinitionAPIService',
function (VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService, UtilsService, VRUIUtilsService, VR_Analytic_OrderDirectionEnum, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_GenericBEDefinitionAPIService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        }, 
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var runtimeEditor = new RuntimeEditor($scope, ctrl, $attrs);
            runtimeEditor.initializeController();
        },
        controllerAs: "ctrlrutnime",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Queries/Templates/RecordSearchQueryDefinitionSettingsRuntimeEditorTemplate.html"
    };

    function RuntimeEditor($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
       
        var dataRecordStorageSelectorAPI;
        var dataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var timePeriodSelectorAPI;
        var timePeriodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataRecordTypeFieldsSelectorAPI;
        var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        var groupFilterAPI;
        var groupFilterReadyDeferred = UtilsService.createPromiseDeferred();

        var fieldsToSortAPI;
        var fieldsToSortReadyDeferred = UtilsService.createPromiseDeferred();

        var dataRecordTypeId;
        var dataRecordTypeIdDeferred = UtilsService.createPromiseDeferred();

        var dataRecordTypeFieldsDeferred = UtilsService.createPromiseDeferred();

        var recordFilterDirectiveAPI;
        var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        
        var filterGroup;
        var context;
        var dataRecordTypeFields = [];

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.sortColumns =[];

            $scope.scopeModel.fieldsToSort = [];

            $scope.scopeModel.selectedFields = [];

            $scope.scopeModel.selectedSortColumns = [];

            $scope.scopeModel.orderDirections = UtilsService.getArrayEnum(VR_Analytic_OrderDirectionEnum);

            $scope.scopeModel.onDataRecordStorageSelectorReady = function (api) {
                dataRecordStorageSelectorAPI = api;
                dataRecordStorageSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onTimePeriodSelectorReady = function (api) {
                timePeriodSelectorAPI = api;
                timePeriodSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                dataRecordTypeFieldsSelectorAPI = api;
                dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
            };


            $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                recordFilterDirectiveAPI = api;
                recordFilterDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };

            $scope.scopeModel.onFieldsToSortReady = function (api) {
                fieldsToSortAPI = api;
                fieldsToSortReadyDeferred.resolve();
            };

            $scope.scopeModel.onSelectField = function (dataItem) {
                if (dataItem != undefined) {
                    $scope.scopeModel.fieldsToSort.push({
                        description: dataItem.Title,
                        value: dataItem.Name
                    });
                }
            };

            $scope.scopeModel.onDeselectField = function (dataItem) {
                if (dataItem != undefined) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.fieldsToSort, dataItem.Name, 'value');
                    $scope.scopeModel.fieldsToSort.splice(datasourceIndex, 1);
                }
            };

            $scope.scopeModel.onSelectSortColumn = function (dataItem) {
                $scope.scopeModel.sortColumns.push({
                    entity:
                            {
                                FieldName: dataItem.value,
                                IsDescending: false
                            }
                        
                    });
            };

            $scope.scopeModel.onDeselectSortColumn = function (dataItem) {
                var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.sortColumns, dataItem.FieldName, 'entity.FieldName');
                $scope.scopeModel.sortColumns.splice(datasourceIndex, 1);
            };

            $scope.scopeModel.removeColumn = function (dataItem) {
                var columnsDatasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.sortColumns, dataItem.FieldName, 'entity.FieldName');
                $scope.scopeModel.sortColumns.splice(columnsDatasourceIndex, 1);
                var sortColumnsDataSourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.selectedSortColumns, dataItem.FieldName, 'value');
                $scope.scopeModel.selectedSortColumns.splice(sortColumnsDataSourceIndex, 1);
            };

                defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var dataStorageIsSelectedArrayDeffered = UtilsService.createPromiseDeferred();
                var promises = [];
                var entity;
                var definitionId;
                var dataStorageArray = [];
                var dataStorageIsSelectedArray = [];
                var dataRecordStoragePayload = [];
                var columns = [];
                var columnNames = [];
                var context;
                var sortColumns =[];

                dataRecordTypeId = undefined;
                if (payload != undefined) {
                    entity = payload.runtimeDirectiveEntity;
                    definitionId = payload.definitionId;
                    context = payload.context;
                    if (entity != undefined) {
                        $scope.scopeModel.limitResult = entity.LimitResult;
                        dataRecordStoragePayload = entity.DataRecordStorages;
                        columns = entity.Columns;
                        filterGroup = entity.FilterGroup;
                        $scope.scopeModel.selectedOrderDirection = UtilsService.getEnum(VR_Analytic_OrderDirectionEnum, "value", entity.Direction);
                        sortColumns = entity.SortColumns;
                        if (dataRecordStoragePayload != undefined) {
                            for (var i = 0; i < dataRecordStoragePayload.length; i++) {
                                dataStorageIsSelectedArray.push(dataRecordStoragePayload[i].DataRecordStorageId);
                            }
                            dataStorageIsSelectedArrayDeffered.resolve();
                        }
                        if (columns != undefined) {
                            for (var i = 0; i < columns.length; i++) {
                                var column = columns[i];
                                if (column != undefined) {
                                    columnNames.push(column.ColumnName);
                                    $scope.scopeModel.fieldsToSort.push({
                                        description: column.ColumnTitle,
                                        value: column.ColumnName
                                    });
                                    $scope.scopeModel.selectedFields.push({
                                        Name: column.ColumnName,
                                        Title: column.ColumnTitle
                                    });
                                }
                            }
                        }
                        if (sortColumns != undefined) {
                            for (var i = 0; i < sortColumns.length; i++) {
                                var sortColumn = sortColumns[i];
                                $scope.scopeModel.sortColumns.push({
                                        entity: sortColumn
                                });
                                for (var j = 0; j < $scope.scopeModel.fieldsToSort.length; j++) {
                                    if (sortColumn.FieldName == $scope.scopeModel.fieldsToSort[j].value) {
                                        $scope.scopeModel.selectedSortColumns.push({
                                            description: $scope.scopeModel.fieldsToSort[j].description,
                                            value: $scope.scopeModel.fieldsToSort[j].value
                                        });
                                    }
                                }
                            }
                        }
                    }

                    VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService.GetVRAutomatedReportQueryDefinitionSettings(definitionId).then(function (response) {
                        if (response != undefined && response.ExtendedSettings != undefined) {
                            if (entity == undefined) {
                                dataRecordStorageSelectorAPI.clearDataSource();
                                $scope.scopeModel.limitResult = '';
                                var arr = response.ExtendedSettings.DataRecordStorages;
                                for (var i = 0; i < arr.length; i++)
                                {
                                    dataStorageArray.push(arr[i]);
                                }
                                for (var i = 0; i < dataStorageArray.length; i++)
                                {
                                    if (dataStorageArray[i].IsSelected == true) {
                                        dataStorageIsSelectedArray.push(dataStorageArray[i].DataRecordStorageId);
                                    }
                                }
                                dataStorageIsSelectedArrayDeffered.resolve();
                            }
                            dataRecordTypeId = response.ExtendedSettings.DataRecordTypeId;
                            dataRecordTypeIdDeferred.resolve();
                        }
                    });
                    
                };

                var loadDataRecordStorageSelectorPromise = loadDataRecordStorageSelector();
                var loadTimePeriodSelectorPromise = loadTimePeriodSelector();
                var loadDataRecordTypeFieldsSelectorPromise = loadDataRecordTypeFieldsSelector();
                var loadDataRecordTypeFieldsPromise = loadDataRecordTypeFields();
                var loadRecordFilterDirectivePromise = loadRecordFilterDirective();
                var loadDataRecordFieldTypeConfigPromise = loadDataRecordFieldTypeConfig();
        
                promises.push(loadDataRecordStorageSelectorPromise);
                promises.push(loadTimePeriodSelectorPromise);
                promises.push(loadDataRecordTypeFieldsSelectorPromise);
                promises.push(loadDataRecordTypeFieldsPromise);
                promises.push(loadRecordFilterDirectivePromise);
                promises.push(loadDataRecordFieldTypeConfigPromise);

                function loadDataRecordStorageSelector() {
                    var dataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var promises = [dataRecordStorageSelectorReadyDeferred.promise, dataStorageIsSelectedArrayDeffered.promise, dataRecordTypeIdDeferred.promise];
                    UtilsService.waitMultiplePromises(promises).then(function () {
                        var dataStorageSelectorPayload = {
                            DataRecordTypeId: definitionId != undefined ? dataRecordTypeId : undefined,
                            selectedIds: definitionId != undefined ? dataStorageIsSelectedArray : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(dataRecordStorageSelectorAPI, dataStorageSelectorPayload, dataRecordStorageSelectorLoadDeferred);
                    });
                    return dataRecordStorageSelectorLoadDeferred.promise;
                }

                function loadTimePeriodSelector() {
                    var timePeriodSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    timePeriodSelectorReadyDeferred.promise.then(function () {
                        var timePeriodSelectorPayload = {
                            timePeriod: entity != undefined ? entity.TimePeriod : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(timePeriodSelectorAPI, timePeriodSelectorPayload, timePeriodSelectorLoadDeferred);
                    });
                    return timePeriodSelectorLoadDeferred.promise;
                }

                function loadDataRecordTypeFieldsSelector() {
                    var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                    var promises = [dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise, dataRecordTypeIdDeferred.promise];
                    UtilsService.waitMultiplePromises(promises).then(function () {
                        var typeFieldsPayload = {
                            dataRecordTypeId: dataRecordTypeId,
                            selectedIds: columnNames
                        };
                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, typeFieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                    });
                    return loadDataRecordTypeFieldsSelectorPromiseDeferred.promise;
                }

                function loadRecordFilterDirective() {
                    var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    UtilsService.waitMultiplePromises([recordFilterDirectiveReadyDeferred.promise, dataRecordTypeFieldsDeferred.promise]).then(function () {
                        var recordFilterDirectivePayload = {
                            context: buildContext(),
                            FilterGroup: filterGroup
                        };
                        VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                    });

                    return recordFilterDirectiveLoadDeferred.promise;
                };
                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {

                var array = dataRecordStorageSelectorAPI.getSelectedIds();
                var dataRecordStoragesArray = [];
                for (var i = 0; i < array.length; i++) {
                    dataRecordStoragesArray.push({ DataRecordStorageId: array [i]});
                }

                var columnNames =[];
                var columnsArray = [];

                for (var i = 0; i < $scope.scopeModel.selectedFields.length; i++) {
                    var field = $scope.scopeModel.selectedFields[i];
                    if (field != undefined) {
                        columnsArray.push({
                            ColumnName: field.Name,
                            ColumnTitle: field.Title
                        });
                        columnNames.push(field.Name);
                    }
                }
                var obj = {
                    $type: 'Vanrise.Analytic.MainExtensions.AutomatedReport.Queries.RecordSearchQuerySettings,Vanrise.Analytic.MainExtensions',
                    DataRecordStorages: dataRecordStoragesArray,
                    TimePeriod: timePeriodSelectorAPI.getData(),
                    Columns: columnsArray,
                    FilterGroup: recordFilterDirectiveAPI.getData().filterObj,
                    LimitResult: $scope.scopeModel.limitResult,
                    Direction:$scope.scopeModel.selectedOrderDirection,
                    SortColumns: getSortColumns()
            };
                function getSortColumns() {
                    var sortColumns = [];
                    for (var i = 0; i < $scope.scopeModel.sortColumns.length; i++) {
                        if ($scope.scopeModel.sortColumns[i] != undefined && $scope.scopeModel.sortColumns[i].entity !=undefined) {
                            sortColumns.push($scope.scopeModel.sortColumns[i].entity);
                        }
                    }
                    return sortColumns;
                }
                return obj;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function buildContext() {
            var context = {
                getFields: function () {
                    var fields = [];
                    for (var fieldName in dataRecordTypeFields) {
                        if (fieldName != "$type") {
                            fields.push({
                                FieldTitle: dataRecordTypeFields[fieldName].Title,
                                FieldName: fieldName,
                                Type: dataRecordTypeFields[fieldName].Type
                            });
                        }
                    }
                    return fields;
                },
                getRuleEditor: getRuleFilterEditorByFieldType
            };
            return context;
        }

        function getRuleFilterEditorByFieldType(configId) {
            var dataRecordFieldTypeConfig = UtilsService.getItemByVal($scope.dataRecordFieldTypesConfig, configId, 'ExtensionConfigurationId');
            if (dataRecordFieldTypeConfig != undefined) {
                return dataRecordFieldTypeConfig.RuleFilterEditor;
            }
        }
        function loadDataRecordFieldTypeConfig() {
            $scope.dataRecordFieldTypesConfig = [];
            return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                if (response) {
                    for (var i = 0; i < response.length; i++) {
                        $scope.dataRecordFieldTypesConfig.push(response[i]);
                    }
                }
            });
        }
        function loadDataRecordTypeFields() {
            var loadDataRecordTypeFieldsLoadDeferred = UtilsService.createPromiseDeferred();
            dataRecordTypeIdDeferred.promise.then(function () {
                VR_GenericData_GenericBEDefinitionAPIService.GetDataRecordTypeFields(dataRecordTypeId).then(function (response) {
                    dataRecordTypeFields = response;
                    dataRecordTypeFieldsDeferred.resolve();
                    loadDataRecordTypeFieldsLoadDeferred.resolve();
                 });
            });
           
            return loadDataRecordTypeFieldsLoadDeferred.promise;
        }
    }

    return directiveDefinitionObject;
}
]);