//(function (app) {

//    'use strict';

//    DataRecordStorageRDBSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService'];

//    function DataRecordStorageRDBSettingsDirective(UtilsService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '='
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var dataRecordStorageRDBSettings = new DataRecordStorageRDBSettings($scope, ctrl, $attrs);
//                dataRecordStorageRDBSettings.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            compile: function (element, attrs) {

//            },
//            templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStorage/DataRecordStorage/Templates/DataRecordStorageRDBSettingsTemplate.html'
//        };

//        function DataRecordStorageRDBSettings($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var dataRecordTypeFields;

//            var dataRecordTypeFieldsSelectorAPI;
//            var dataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();
//            var rdbDataTypeSelectorAPI;
//            var rdbDataTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {
//                ctrl.columns = [];

//                ctrl.onGridReady = function (api) {
//                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                        ctrl.onReady(getDirectiveAPI());
//                    }
//                };
//                ctrl.onDataRecordTypeFieldsSelectorReady = function (api) {
//                    dataRecordTypeFieldsSelectorAPI = api;
//                    dataRecordTypeFieldsSelectorReadyDeferred.resolve();
//                };

//                ctrl.addColumn = function () {
//                    addColumn();
//                };

//                ctrl.removeColumn = function (dataItem) {
//                    var index = UtilsService.getItemIndexByVal(ctrl.columns, dataItem.id, 'id');
//                    if (index > -1) {
//                        ctrl.columns.splice(index, 1);
//                    }
//                };

//                ctrl.validateColumns = function () {
//                    if (ctrl.columns.length == 0) {
//                        return 'Please, one record must be added at least.';
//                    }

//                    var columnNames = [];
//                    for (var i = 0; i < ctrl.columns.length; i++) {
//                        if (ctrl.columns[i].columnName != undefined) {
//                            columnNames.push(ctrl.columns[i].columnName.toUpperCase());
//                        }
//                    }

//                    while (columnNames.length > 0) {
//                        var nameToValidate = columnNames[0];
//                        columnNames.splice(0, 1);
//                        if (!validateName(nameToValidate, columnNames)) {
//                            return 'Two or more columns have the same name';
//                        }
//                    }

//                    return null;

//                    function validateName(name, array) {
//                        for (var j = 0; j < array.length; j++) {
//                            if (array[j] == name)
//                                return false;
//                        }
//                        return true;
//                    }
//                };

//                ctrl.validateNullableFields = function () {
//                    if (ctrl.columns == undefined || ctrl.selectedDataRecordTypeField == undefined)
//                        return null;

//                    var duplicatedFields = [];
//                    for (var i = 0; i < ctrl.columns.length; i++) {
//                        var currentColumn = ctrl.columns[i];
//                        for (var j = 0; j < ctrl.selectedDataRecordTypeField.length; j++) {
//                            if (currentColumn.selectedDataRecordTypeField != undefined && currentColumn.selectedDataRecordTypeField.Name == ctrl.selectedDataRecordTypeField[j].Name)
//                                duplicatedFields.push(currentColumn.selectedDataRecordTypeField.Name);
//                        }
//                    }

//                    if (duplicatedFields.length == 1) {
//                        return duplicatedFields[0] + " is selected at Table Definition";
//                    }
//                    if (duplicatedFields.length > 1) {
//                        return duplicatedFields.join(", ") + " are selected at Table Definition";
//                    }

//                    return null;
//                };
//            }

//            function getDirectiveAPI() {
//                var api = {
//                };

//                api.load = function (payload) {
//                    if (payload == undefined)
//                        return;

//                    var promises = [];

//                    if (payload.Columns != undefined) {
//                        ctrl.tableName = payload.TableName;
//                        ctrl.tableSchema = payload.TableSchema;
//                        ctrl.includeQueueItemId = payload.IncludeQueueItemId;
//                    }

//                    if (payload.DataRecordTypeId != undefined) {  // Definition Tab Fields
//                        var getDataRecordTypeFieldsPromise = getDataRecordTypeFields();
//                        promises.push(getDataRecordTypeFieldsPromise);
//                    }

//                    //loading DataRecordType Selector
//                    var loadDataRecordTypeSelectorPromise = loadDataRecordTypeSelector();
//                    promises.push(loadDataRecordTypeSelectorPromise);

//                    //var loadRDBDataTypeSelectorPromise = loadRDBDataTypeSelector();
//                    //promises.push(loadRDBDataTypeSelectorPromise);

//                    function getDataRecordTypeFields() {
//                        return VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(payload.DataRecordTypeId).then(function (response) {
//                            if (response != null && response.Fields != null) {
//                                dataRecordTypeFields = [];
//                                for (var i = 0; i < response.Fields.length; i++) {
//                                    dataRecordTypeFields.push(response.Fields[i]);
//                                }
//                                loadColumns();
//                            }
//                        });
//                    }
//                    function loadColumns() {
//                        if (payload.Columns != undefined) {
//                            for (var i = 0; i < payload.Columns.length; i++) {
//                                addColumn(payload.Columns[i], promises);
//                            }
//                        }
//                        else {
//                            for (var i = 0; i < ctrl.columns.length; i++) {
//                                setDataRecordTypeFields(ctrl.columns[i]);
//                            }
//                        }
//                    }
//                    function loadDataRecordTypeSelector() {
//                        var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

//                        dataRecordTypeFieldsSelectorReadyDeferred.promise.then(function () {

//                            var dataRecordTypeselectorPayload = {
//                                dataRecordTypeId: payload.DataRecordTypeId
//                            };
//                            if (payload.NullableFields != undefined) {
//                                dataRecordTypeselectorPayload.selectedIds = UtilsService.getPropValuesFromArray(payload.NullableFields, "Name");
//                            }

//                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, dataRecordTypeselectorPayload, dataRecordTypeSelectorLoadDeferred);
//                        });

//                        return dataRecordTypeSelectorLoadDeferred.promise;
//                    }
//                    return UtilsService.waitMultiplePromises(promises);
//                };

//                api.getData = function () {
//                    return {
//                        $type: 'Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageSettings, Vanrise.GenericData.RDBDataStorage',
//                        TableName: ctrl.tableName,
//                        TableSchema: ctrl.tableSchema,
//                        NullableFields: getNullableFields(),
//                        IncludeQueueItemId: ctrl.includeQueueItemId,
//                        Columns: ctrl.columns.length > 0 ? getColumns() : null,
//                    };
//                };

//                function getColumns() {
//                    var columns = [];
//                    for (var i = 0; i < ctrl.columns.length; i++) {
//                        var column = ctrl.columns[i];
//                        columns.push({
//                            $type: 'Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageColumn, Vanrise.GenericData.RDBDataStorage',
//                            FieldName: column.fieldName,
//                            ColumnName: column.columnName,
//                            DataType: column.rdbDataTypeFieldSelectorAPI != undefined ? column.rdbDataTypeFieldSelectorAPI.getSelectedIds().Type : undefined,
//                            Size: column.rdbDataTypeFieldSelectorAPI != undefined ? column.rdbDataTypeFieldSelectorAPI.getSelectedIds().Size : undefined,
//                            Precision: column.rdbDataTypeFieldSelectorAPI != undefined ? column.rdbDataTypeFieldSelectorAPI.getSelectedIds().Precision : undefined,
//                            IsUnique: column.isUnique,
//                            IsIdentity: column.isIdentity
//                        });
//                    }
//                    return columns;
//                }
//                function getNullableFields() {
//                    var nullableFields = [];

//                    var nullableFieldNames = dataRecordTypeFieldsSelectorAPI.getSelectedIds();
//                    if (nullableFieldNames != undefined) {
//                        for (var index = 0; index < nullableFieldNames.length; index++) {
//                            nullableFields.push({ Name: nullableFieldNames[index] });
//                        }
//                    }
//                    return nullableFields;
//                }

//                return api;
//            };

//            function addColumn(data, promises) {
//                var gridItem = {
//                    id: ctrl.columns.length + 1,

//                };

//                if (data != undefined) { 
//                    gridItem.fieldName = data.FieldName;
//                    gridItem.columnName = data.ColumnName;
//                    gridItem.isUnique = data.IsUnique;
//                    gridItem.isIdentity = data.IsIdentity;
//                    gridItem.isDisabled = true;
//                    gridItem.readyPromiseDeferred = UtilsService.createPromiseDeferred();
//                    gridItem.loadPromiseDeferred = UtilsService.createPromiseDeferred();
//                    gridItem.onRDBDataTypeSelectorReady = function (api) {
//                        gridItem.rdbDataTypeFieldSelectorAPI = api;
//                        gridItem.readyPromiseDeferred.resolve();
//                    };
//                    promises.push(gridItem.loadPromiseDeferred.promise);
//                    gridItem.readyPromiseDeferred.promise.then(function () {
//                        var payload = {
//                            selectedIds: data.DataType,
//                            size: data.Size,
//                            precision: data.Precision
//                        };
//                        VRUIUtilsService.callDirectiveLoad(gridItem.rdbDataTypeFieldSelectorAPI, payload, gridItem.loadPromiseDeferred);
//                    });
//                }
//                else {  
//                    gridItem.onRDBDataTypeSelectorReady = function (api) {
//                        gridItem.rdbDataTypeFieldSelectorAPI = api;
//                        var setLoader = function (value) { gridItem.isLoadingRDBDataTypeDirective = value; };
//                        var payload = { };
//                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.rdbDataTypeFieldSelectorAPI, payload, setLoader);
//                    };
//                }

//                gridItem.onSelectorReady = function (api) {
//                    setDataRecordTypeFields(gridItem);
//                };

//                ctrl.columns.push(gridItem);
//            }

//            function setDataRecordTypeFields(gridItem) {
//                gridItem.dataRecordTypeFields = [];
//                for (var i = 0; i < dataRecordTypeFields.length; i++) {
//                    gridItem.dataRecordTypeFields.push(dataRecordTypeFields[i]);
//                }
//                if (gridItem.selectedDataRecordTypeFieldName != undefined) {
//                    gridItem.selectedDataRecordTypeField = UtilsService.getItemByVal(gridItem.dataRecordTypeFields, gridItem.selectedDataRecordTypeFieldName, 'Name');
//                    gridItem.selectedDataRecordTypeFieldName = undefined;
//                }
//                else {
//                    gridItem.selectedDataRecordTypeField = undefined;
//                }
//            }
//        }
//    }

//    app.directive('vrGenericdataDatarecordstoragesettingsRdb', DataRecordStorageRDBSettingsDirective);

//})(app);