"use strict";
app.directive("vrAnalyticRecordsearchquerydefinitionsettingsRuntimeeditor", ["VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService", "UtilsService", "VRUIUtilsService", "VR_Analytic_OrderDirectionEnum", 'VR_GenericData_DataRecordFieldAPIService', 'VR_GenericData_GenericBEDefinitionAPIService', "VRNotificationService",
function (VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService, UtilsService, VRUIUtilsService, VR_Analytic_OrderDirectionEnum, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_GenericBEDefinitionAPIService, VRNotificationService) {
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
        var dataRecordTypeIdDeferred;
        var dataStorageIsSelectedArrayDeffered;

        var dataRecordTypeFieldsDeferred = UtilsService.createPromiseDeferred();

        var recordFilterDirectiveAPI;
        var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        
        var context;
        var dataRecordTypeFields;
        var gridColumns =[];

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

            $scope.scopeModel.onFieldsToSortSelectionChanged = function (fieldsToSort) {
                if (fieldsToSort != undefined && fieldsToSort.length > 0) {
                    for (var i = 0; i < fieldsToSort.length; i++) {
                        var fieldToSort = fieldsToSort[i];
                        if (!gridColumns.includes(fieldToSort.value)) {
                            $scope.scopeModel.sortColumns.push({
                                entity:
                                        {
                                            FieldName: fieldToSort.value,
                                            IsDescending: false
                                        }
                            });
                            gridColumns.push(fieldToSort.value);
                        }
                    }
                }
                else {
                    gridColumns.length = 0;
                    $scope.scopeModel.sortColumns.length = 0;
                }
            };
            $scope.scopeModel.onDeselectSortColumn = function (dataItem) {
                var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.sortColumns, dataItem.value, 'entity.FieldName');
                $scope.scopeModel.sortColumns.splice(datasourceIndex, 1);
            };

            $scope.scopeModel.removeColumn = function (dataItem) {
                if (dataItem != undefined && dataItem.entity !=undefined) {
                var columnsDatasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.sortColumns, dataItem.entity.FieldName, 'entity.FieldName');
                $scope.scopeModel.sortColumns.splice(columnsDatasourceIndex, 1);
                var sortColumnsDataSourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.selectedSortColumns, dataItem.entity.FieldName, 'value');
                $scope.scopeModel.selectedSortColumns.splice(sortColumnsDataSourceIndex, 1);
                }
            };

            $scope.scopeModel.validateSortColumns = function () {
                if ($scope.scopeModel.sortColumns != undefined && $scope.scopeModel.sortColumns.length == 0)
                    return "At least one sort column must be added.";
            };

            UtilsService.waitMultiplePromises([dataRecordStorageSelectorReadyDeferred.promise, timePeriodSelectorReadyDeferred.promise, dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise, recordFilterDirectiveReadyDeferred.promise, gridReadyDeferred.promise, fieldsToSortReadyDeferred.promise]).then(function () {
                defineAPI();

            });
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                dataStorageIsSelectedArrayDeffered = UtilsService.createPromiseDeferred();
                dataRecordTypeIdDeferred = UtilsService.createPromiseDeferred();

                $scope.scopeModel.fieldsToSort.length = 0;
                $scope.scopeModel.selectedSortColumns.length = 0;
                $scope.scopeModel.sortColumns.length = 0;
                $scope.scopeModel.selectedOrderDirection = undefined;

                var entity;
                var definitionId;
                var dataStorageArray = [];
                var dataStorageIsSelectedArray = [];
                var dataRecordStoragePayload;
                var columns = [];
                var columnNames = [];
                var context;
                var sortColumns;
                var filterGroup;

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
                        if(entity.Direction!=undefined)
                            $scope.scopeModel.selectedOrderDirection = UtilsService.getItemByVal($scope.scopeModel.orderDirections, entity.Direction, "value");
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
                                }
                            }
                        }
                        if (sortColumns != undefined) {
                            loadDataRecordTypeFields().then(function () {
                                for (var i = 0; i < sortColumns.length; i++) {
                                    var sortColumn = sortColumns[i];
                                    $scope.scopeModel.sortColumns.push({
                                        entity: sortColumn
                                    });
                                    gridColumns.push(sortColumn.FieldName);
                                    if (dataRecordTypeFields != undefined) {
                                  for (var fieldName in dataRecordTypeFields) {
                                        if (fieldName != "$type") {
                                            if (sortColumn.FieldName == fieldName) {
                                                $scope.scopeModel.selectedSortColumns.push({
                                                            description: dataRecordTypeFields[fieldName].Title,
                                                            value: fieldName
                                                });
                                                }
                                            }
                                        }
                                }
                            }
                            });
                        }
                    }
                };

             

                function getInitialData() {
             
                    VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService.GetVRAutomatedReportQueryDefinitionSettings(definitionId).then(function (response) {
                        if (response != undefined && response.ExtendedSettings != undefined) {
                            if (entity == undefined) {
                                dataRecordStorageSelectorAPI.clearDataSource();
                                $scope.scopeModel.limitResult = '';
                                var arr = response.ExtendedSettings.DataRecordStorages;
                                if (arr != undefined) {
                                    for (var i = 0; i < arr.length; i++) {
                                        dataStorageArray.push(arr[i]);
                                    }
                                    if (dataStorageArray != undefined) {
                                        for (var i = 0; i < dataStorageArray.length; i++) {
                                            var dataStorageItem = dataStorageArray[i];
                                            if (dataStorageItem.IsSelected == true) {
                                                dataStorageIsSelectedArray.push(dataStorageItem.DataRecordStorageId);
                                            }
                                        }
                                        dataStorageIsSelectedArrayDeffered.resolve();
                                    }
                                }
                            }
                            dataRecordTypeId = response.ExtendedSettings.DataRecordTypeId;
                            dataRecordTypeIdDeferred.resolve();
                        }
                    });
                    return UtilsService.waitMultiplePromises([dataRecordTypeIdDeferred.promise, dataStorageIsSelectedArrayDeffered.promise]);

                }

                function loadDataRecordStorageSelector() {
                    var dataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    dataRecordStorageSelectorReadyDeferred.promise.then(function () {
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
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
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

                    recordFilterDirectiveReadyDeferred.promise.then(function () {
                        var recordFilterDirectivePayload = {
                            context: buildContext(),
                            FilterGroup: filterGroup
                        };
                        VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                    });

                    return recordFilterDirectiveLoadDeferred.promise;
                };

                function loadFieldsToSortSelector() {
                    var fieldsToSortSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    if (dataRecordTypeFields != undefined) {
                        for (var fieldName in dataRecordTypeFields) {
                            if (fieldName != "$type") {
                                $scope.scopeModel.fieldsToSort.push({
                                    description: dataRecordTypeFields[fieldName].Title,
                                    value: fieldName
                                });
                            }
                        }
                        fieldsToSortSelectorLoadDeferred.resolve();
                    }
                    return fieldsToSortSelectorLoadDeferred.promise;
                }

                var rootPromiseNode = {
                    promises: [getInitialData(), loadDataRecordFieldTypeConfig()],
                    getChildNode: function () {
                        return {
                            promises: [loadDataRecordTypeFields()],
                            getChildNode: function () {
                                return {
                                    promises: [UtilsService.waitMultiplePromises([loadDataRecordStorageSelector(), loadTimePeriodSelector(), loadDataRecordTypeFieldsSelector(), loadRecordFilterDirective(), loadFieldsToSortSelector()])]
                                };
                            },
                        };
                    }
                };
                return UtilsService.waitPromiseNode(rootPromiseNode);
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
                    Direction:$scope.scopeModel.selectedOrderDirection.value,
                    SortColumns: getSortColumns()
                };
                function getSortColumns() {
                    var sortColumns = [];
                    for (var i = 0; i < $scope.scopeModel.sortColumns.length; i++) {
                        var sortColumn = $scope.scopeModel.sortColumns[i];
                        if (sortColumn != undefined && sortColumn.entity != undefined) {
                            sortColumns.push(sortColumn.entity);
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
                            var dataRecordTypeFieldValue = dataRecordTypeFields[fieldName];
                            fields.push({
                                FieldTitle: dataRecordTypeFieldValue.Title,
                                FieldName: fieldName,
                                Type: dataRecordTypeFieldValue.Type
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