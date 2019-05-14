(function (app) {

    'use strict';

    DataRecordStorageRDBSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VR_GenericData_GenericBEDefinitionAPIService', 'VR_GenericData_DataRecordFieldAPIService'];

    function DataRecordStorageRDBSettingsDirective(UtilsService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService, VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_DataRecordFieldAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataRecordStorageRDBSettings = new DataRecordStorageRDBSettings($scope, ctrl, $attrs);
                dataRecordStorageRDBSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStorage/DataRecordStorage/Templates/DataRecordStorageRDBSettingsTemplate.html'
        };

        function DataRecordStorageRDBSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeFields;
            var dataRecordTypeId;
            var joins = [];
            var expressionFields = [];
            var filter;

            var parentRecordStorageSelectorAPI;
            var parentRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var rdbJoinsDataRecordStorageAPI;
            var rdbJoinsDataRecordStorageReadyDeferred = UtilsService.createPromiseDeferred();

            var expressionFieldsAPI;
            var expressionFieldsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var filterDirectiveAPI;
            var filterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.isLoading = true;

                $scope.scopeModel.showAddAllFields = function () {
                    if (ctrl.columns.length == 0)
                        return true;
                    else if (dataRecordTypeFields != undefined && ctrl.columns.length < dataRecordTypeFields.length)
                        return true;
                    else
                        return false;
                };

                $scope.scopeModel.onParentRecordStorageSelectorReady = function (api) {
                    parentRecordStorageSelectorAPI = api;
                    parentRecordStorageSelectorReadyDeferred.resolve();
                };

                ctrl.columns = [];

                ctrl.onGridReady = function (api) {
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.onDataRecordTypeFieldsSelectorReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyDeferred.resolve();
                };

                ctrl.addColumn = function () {
                    addColumn();
                };

                ctrl.removeColumn = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal(ctrl.columns, dataItem.fieldName, 'fieldName');
                    if (index > -1) {
                        ctrl.columns.splice(index, 1);
                    }
                };

                ctrl.validateColumns = function () {
                    if (ctrl.columns.length == 0) {
                        return 'Please, one record must be added at least.';
                    }

                    var columnNames = [];
                    for (var i = 0; i < ctrl.columns.length; i++) {
                        var column = ctrl.columns[i];
                        if (column.columnName != undefined) {
                            columnNames.push(column.columnName.toUpperCase());
                        }
                    }

                    while (columnNames.length > 0) {
                        var nameToValidate = columnNames[0];
                        columnNames.splice(0, 1);
                        if (!validateName(nameToValidate, columnNames)) {
                            return 'Two or more columns have the same name';
                        }
                    }

                    return null;

                    function validateName(name, array) {
                        for (var j = 0; j < array.length; j++) {
                            var arrayElement = array[j];
                            if (arrayElement == name)
                                return false;
                        }
                        return true;
                    }
                };

                ctrl.validateNullableFields = function () {
                    if (ctrl.columns == undefined || ctrl.selectedDataRecordTypeField == undefined)
                        return null;

                    var duplicatedFields = [];
                    for (var i = 0; i < ctrl.columns.length; i++) {
                        var currentColumn = ctrl.columns[i];
                        for (var j = 0; j < ctrl.selectedDataRecordTypeField.length; j++) {
                            if (currentColumn.selectedDataRecordTypeField != undefined && currentColumn.selectedDataRecordTypeField.Name == ctrl.selectedDataRecordTypeField[j].Name)
                                duplicatedFields.push(currentColumn.selectedDataRecordTypeField.Name);
                        }
                    }

                    if (duplicatedFields.length == 1) {
                        return duplicatedFields[0] + " is selected at Table Definition";
                    }

                    if (duplicatedFields.length > 1) {
                        return duplicatedFields.join(", ") + " are selected at Table Definition";
                    }

                    return null;
                };

                ctrl.loadDefaultRDBTypeTableSettings = function () {
                    $scope.scopeModel.isLoading = true;
                    var columns = getColumns();
                    for (var i = 0; i < dataRecordTypeFields.length; i++) {
                        var dataRecordField = dataRecordTypeFields[i];

                        if (ctrl.columns.length == 0) {
                            addDataRecordFieldToColumns(dataRecordField);
                        }
                        else if (columns.findIndex(x => x.FieldName === dataRecordField.FieldName) == -1) {
                            addDataRecordFieldToColumns(dataRecordField);
                        }
                    }
                    $scope.scopeModel.isLoading = false;
                };

                $scope.scopeModel.onRDBJoinsDataRecordStorageReady = function (api) {
                    rdbJoinsDataRecordStorageAPI = api;
                    rdbJoinsDataRecordStorageReadyDeferred.resolve();
                };

                $scope.scopeModel.onRDBExpressionFieldsReady = function (api) {
                    expressionFieldsAPI = api;
                    expressionFieldsReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onFilterDirectiveReady = function (api) {
                    filterDirectiveAPI = api;
                    filterDirectiveReadyDeferred.resolve();
                };

            }

            function addDataRecordFieldToColumns(dataRecordField) {
                var gridItem = {
                    columnName: dataRecordField.FieldName,
                    isUnique: dataRecordField.IsUnique,
                    fieldName: dataRecordField.FieldName,
                    rdbDataType: dataRecordField.RDBDataType,
                    size: dataRecordField.Size,
                    precision: dataRecordField.Precision,
                    readyDataRecordTypeFieldPromiseDeferred: UtilsService.createPromiseDeferred(),
                    readyRDBDataTypeFieldItemPromiseDeferred: UtilsService.createPromiseDeferred()
                };

                loadFieldNameItem(gridItem);
                loadRDBDataTypeFieldItem(gridItem);

                ctrl.columns.push(gridItem);
            }

            function getColumns() {
                var columns = [];
                for (var i = 0; i < ctrl.columns.length; i++) {
                    var column = ctrl.columns[i];
                    columns.push({
                        $type: 'Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageColumn, Vanrise.GenericData.RDBDataStorage',
                        FieldName: column.dataRecordTypeFieldSelectorAPI != undefined ? column.dataRecordTypeFieldSelectorAPI.getSelectedIds() : undefined,
                        ColumnName: column.columnName,
                        DataType: column.rdbDataTypeFieldSelectorAPI != undefined ? column.rdbDataTypeFieldSelectorAPI.getSelectedIds().Type : undefined,
                        Size: column.rdbDataTypeFieldSelectorAPI != undefined ? column.rdbDataTypeFieldSelectorAPI.getSelectedIds().Size : undefined,
                        Precision: column.rdbDataTypeFieldSelectorAPI != undefined ? column.rdbDataTypeFieldSelectorAPI.getSelectedIds().Precision : undefined,
                        IsUnique: column.isUnique,
                        IsIdentity: column.isIdentity
                    });
                }
                return columns;
            }

            function getDirectiveAPI() {
                var api = {
                };

                api.load = function (payload) {
                    if (payload == undefined)
                        return;

                    var promises = [];

                    if (payload.Columns != undefined) {
                        ctrl.tableName = payload.TableName;
                        ctrl.tableSchema = payload.TableSchema;
                        ctrl.includeQueueItemId = payload.IncludeQueueItemId;
                    }
                    
                    joins = payload.Joins;
                    expressionFields = payload.ExpressionFields;
                    filter = payload.Filter;

                    if (payload.DataRecordTypeId != undefined) {
                        dataRecordTypeId = payload.DataRecordTypeId;
                        loadDefaultRDBTypeList();
                        loadColumns();
                    }

                    //loading ParentDataRecordStorage Selector
                    var loadParentDataRecordStorageSelectorPromise = loadParentDataRecordStorageSelector();
                    promises.push(loadParentDataRecordStorageSelectorPromise);

                    //loading DataRecordType Selector
                    var loadDataRecordTypeSelectorPromise = loadDataRecordTypeSelector();
                    promises.push(loadDataRecordTypeSelectorPromise);

                    //loading Joins Tab
                    var loadRDBJoinsDataRecordStoragePromise = loadRDBJoinsDataRecordStorage();
                    promises.push(loadRDBJoinsDataRecordStoragePromise);
                    
                    //loading Expression Fields Tab
                    var loadRDBExpressionFieldsPromise = loadRDBExpressionFields();
                    promises.push(loadRDBExpressionFieldsPromise);

                    //loading Filter Tab
                    var loadFilterDirectivePromise = loadFilterDirective();
                    promises.push(loadFilterDirectivePromise);

                    function loadColumns() {
                        if (payload.Columns != undefined) {
                            for (var i = 0; i < payload.Columns.length; i++) {
                                var column = payload.Columns[i];
                                addColumn(column, promises);
                            }
                        }
                        else {
                            for (var i = 0; i < ctrl.columns.length; i++) {
                                var column = ctrl.columns[i];
                                setDataRecordTypeFields(column);
                            }
                        }
                    }

                    function loadParentDataRecordStorageSelector() {
                        var parentDataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        parentRecordStorageSelectorReadyDeferred.promise.then(function () {

                            var parentDataRecordStorageSelectorPayload = {
                                filters: [{
                                    $type: "Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageFilter, Vanrise.GenericData.RDBDataStorage"
                                }]
                            };

                            if (payload.ParentRecordStorageId != undefined)
                                parentDataRecordStorageSelectorPayload.selectedIds = payload.ParentRecordStorageId;

                            VRUIUtilsService.callDirectiveLoad(parentRecordStorageSelectorAPI, parentDataRecordStorageSelectorPayload, parentDataRecordStorageSelectorLoadDeferred);
                        });

                        return parentDataRecordStorageSelectorLoadDeferred.promise;
                    }


                    function loadDataRecordTypeSelector() {
                        var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        dataRecordTypeFieldsSelectorReadyDeferred.promise.then(function () {

                            var dataRecordTypeselectorPayload = {
                                dataRecordTypeId: payload.DataRecordTypeId
                            };
                            if (payload.NullableFields != undefined) {
                                dataRecordTypeselectorPayload.selectedIds = UtilsService.getPropValuesFromArray(payload.NullableFields, "Name");
                            }

                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, dataRecordTypeselectorPayload, dataRecordTypeSelectorLoadDeferred);
                        });

                        return dataRecordTypeSelectorLoadDeferred.promise;
                    }


                    function loadRDBJoinsDataRecordStorage() {
                        var rdbJoinsDataRecordStorageLoadDeferred = UtilsService.createPromiseDeferred();

                        rdbJoinsDataRecordStorageReadyDeferred.promise.then(function () {
                            var rdbJoinsDataRecordStoragePayload = {
                                context: buildContext(),
                                joins: joins != undefined && joins.length > 0 ? joins : undefined 
                            };
                            VRUIUtilsService.callDirectiveLoad(rdbJoinsDataRecordStorageAPI, rdbJoinsDataRecordStoragePayload, rdbJoinsDataRecordStorageLoadDeferred);
                        });

                        return rdbJoinsDataRecordStorageLoadDeferred.promise;
                    }

                    function loadRDBExpressionFields() {
                        var rdbExpressionFieldsLoadDeferred = UtilsService.createPromiseDeferred();

                        expressionFieldsReadyPromiseDeferred.promise.then(function () {
                            var rdbExpressionFieldsPayload = {
                                context: buildContext(),
                                expressionFields: expressionFields != undefined && expressionFields.length > 0 ? expressionFields : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(expressionFieldsAPI, rdbExpressionFieldsPayload, rdbExpressionFieldsLoadDeferred);
                        });

                        return rdbExpressionFieldsLoadDeferred.promise;
                    }

                    function loadFilterDirective() {
                        var filterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        filterDirectiveReadyDeferred.promise.then(function () {
                            VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId).then(function (response) {
                                var dataRecordFieldsInfo = response;
                                var recordFilterGroupDirectivePayload = {
                                    context: buildContext(dataRecordFieldsInfo),
                                    filter: filter
                                };
                                VRUIUtilsService.callDirectiveLoad(filterDirectiveAPI, recordFilterGroupDirectivePayload, filterDirectiveLoadDeferred);
                            });
                        });
                        return filterDirectiveLoadDeferred.promise;
                    }

                    $scope.scopeModel.isLoading = false;

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var joinsList = rdbJoinsDataRecordStorageAPI.getData();
                    var expressionFieldsList = expressionFieldsAPI.getData();
                    return {
                        $type: 'Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageSettings, Vanrise.GenericData.RDBDataStorage',
                        ParentRecordStorageId: parentRecordStorageSelectorAPI.getSelectedIds(),
                        TableName: ctrl.tableName,
                        TableSchema: ctrl.tableSchema,
                        NullableFields: getNullableFields(),
                        IncludeQueueItemId: ctrl.includeQueueItemId,
                        Columns: ctrl.columns.length > 0 ? getColumns() : null,
                        Joins: (joinsList != undefined && joinsList.length > 0) ? joinsList : null,
                        ExpressionFields: (expressionFieldsList != undefined && expressionFieldsList.length > 0) ? expressionFieldsList : null,
                        Filter: filterDirectiveAPI.getData()
                    };
                };

                function getNullableFields() {
                    var nullableFields = [];

                    var nullableFieldNames = dataRecordTypeFieldsSelectorAPI.getSelectedIds();
                    if (nullableFieldNames != undefined) {
                        for (var index = 0; index < nullableFieldNames.length; index++) {
                            nullableFields.push({ Name: nullableFieldNames[index] });
                        }
                    }
                    return nullableFields;
                }

                return api;
            };

            function loadFieldNameItem(gridItem, promises) {
                gridItem.loadDataRecordTypeFieldPromiseDeferred = UtilsService.createPromiseDeferred();

                gridItem.onDataRecordTypeFieldsSelectorReady = function (api) {
                    gridItem.dataRecordTypeFieldSelectorAPI = api;
                    gridItem.readyDataRecordTypeFieldPromiseDeferred.resolve();
                };

                gridItem.readyDataRecordTypeFieldPromiseDeferred.promise.then(function () {
                    var payload = {
                        selectedIds: gridItem.fieldName,
                        dataRecordTypeId: dataRecordTypeId
                    };
                    var setLoader = function (value) { gridItem.isLoadingDataRecordTypeFieldsDirective = value; };
                    VRUIUtilsService.callDirectiveLoad(gridItem.dataRecordTypeFieldSelectorAPI, payload, gridItem.loadDataRecordTypeFieldPromiseDeferred, setLoader);
                });
                if (promises != undefined)
                    promises.push(gridItem.loadDataRecordTypeFieldPromiseDeferred.promise);
            }

            function loadRDBDataTypeFieldItem(gridItem, promises) {
                gridItem.loadRDBDataTypeFieldItemPromiseDeferred = UtilsService.createPromiseDeferred();

                gridItem.onRDBDataTypeSelectorReady = function (api) {
                    gridItem.rdbDataTypeFieldSelectorAPI = api;
                    gridItem.readyRDBDataTypeFieldItemPromiseDeferred.resolve();
                };

                gridItem.readyRDBDataTypeFieldItemPromiseDeferred.promise.then(function () {
                    var payload = {
                        selectedIds: gridItem.rdbDataType,
                        size: gridItem.size,
                        precision: gridItem.precision
                    };
                    var setLoader = function (value) { gridItem.isLoadingRDBDataTypeDirective = value; };
                    VRUIUtilsService.callDirectiveLoad(gridItem.rdbDataTypeFieldSelectorAPI, payload, gridItem.loadRDBDataTypeFieldItemPromiseDeferred, setLoader);
                });
                if (promises != undefined)
                    promises.push(gridItem.loadRDBDataTypeFieldItemPromiseDeferred.promise);
            }

            function changedDataRecordTypeFields(gridItem, promises) {
                gridItem.onDataRecordTypeFieldsChanged = function (selectedDataRecordTypeField) {
                    if (selectedDataRecordTypeField != undefined) {
                        var columns = getColumns();
                        var index = columns.findIndex(x => x.FieldName === selectedDataRecordTypeField.Name);
                        var col = columns[index];
                        if (checkIfValuesAreDefault(col)) {
                            var dataRecordField = dataRecordTypeFields.find(function (element) {
                                return element.FieldName == col.FieldName;
                            });
                            gridItem.columnName = dataRecordField.FieldName;
                            gridItem.isUnique = dataRecordField.IsUnique;
                            gridItem.rdbDataType = dataRecordField.RDBDataType;
                            gridItem.size = dataRecordField.Size;
                            gridItem.precision = dataRecordField.Precision;
                            loadRDBDataTypeFieldItem(gridItem, promises);
                            ctrl.columns[index] = gridItem;
                        }
                    }
                };

                function checkIfValuesAreDefault(column) {
                    if (column.ColumnName != undefined)
                        return false;
                    if (column.DataType != undefined)
                        return false;
                    if (column.IsIdentity == true)
                        return false;
                    if (column.IsUnique == true)
                        return false;
                    return true;
                }
            }

            function loadDefaultRDBTypeList() {
                return VR_GenericData_DataRecordTypeAPIService.GetDataRecordFieldsTranslatedToRDB(dataRecordTypeId).then(function (response) {
                    dataRecordTypeFields = response;
                });
            }

            function addColumn(data, promises) {

                var gridItem = {
                    id: ctrl.columns.length + 1,
                    readyDataRecordTypeFieldPromiseDeferred: UtilsService.createPromiseDeferred(),
                    readyRDBDataTypeFieldItemPromiseDeferred: UtilsService.createPromiseDeferred()
                };

                if (data != undefined) {
                    gridItem.columnName = data.ColumnName;
                    gridItem.isUnique = data.IsUnique;
                    gridItem.isIdentity = data.IsIdentity;
                    gridItem.isDisabled = true;
                    gridItem.rdbDataType = data.DataType;
                    gridItem.size = data.Size;
                    gridItem.precision = data.Precision;
                    gridItem.fieldName = data.FieldName;
                }

                loadFieldNameItem(gridItem, promises);
                loadRDBDataTypeFieldItem(gridItem, promises);
                changedDataRecordTypeFields(gridItem, promises);
                ctrl.columns.push(gridItem);
            }

            function setDataRecordTypeFields(gridItem) {
                ctrl.columns = [];
                gridItem.dataRecordTypeFields = [];
                for (var i = 0; i < dataRecordTypeFields.length; i++) {
                    gridItem.dataRecordTypeFields.push(dataRecordTypeFields[i]);
                }
            }

            function buildContext(dataRecordFieldsInfo) {
                var context = {
                    getFields: function () {
                        var fields = [];
                        if (dataRecordFieldsInfo != undefined) {
                            for (var i = 0; i < dataRecordFieldsInfo.length; i++) {
                                var dataRecordField = dataRecordFieldsInfo[i].Entity;

                                fields.push({
                                    FieldName: dataRecordField.Name,
                                    FieldTitle: dataRecordField.Title,
                                    Type: dataRecordField.Type
                                });
                            }
                        }
                        return fields;
                    },
                    getJoinsList: function () {
                        return rdbJoinsDataRecordStorageAPI.getData();
                    },
                    getMainDataRecordTypeId: function () {
                        return dataRecordTypeId;
                    }
                };
                return context;
            }
        }
    }
    app.directive('vrGenericdataDatarecordstoragesettingsRdb', DataRecordStorageRDBSettingsDirective);
})(app);