(function (app) {

    'use strict';

    DataRecordStorageSQLSettingsDirective.$inject = ['VR_GenericData_DataRecordTypeAPIService', 'UtilsService'];

    function DataRecordStorageSQLSettingsDirective(VR_GenericData_DataRecordTypeAPIService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataRecordStorageSQLSettings = new DataRecordStorageSQLSettings($scope, ctrl, $attrs);
                dataRecordStorageSQLSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStorage/DataRecordStorage/Templates/DataRecordStorageSQLSettingsTemplate.html'
        };

        function DataRecordStorageSQLSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeFields;

            function initializeController() {
                ctrl.columns = [];

                ctrl.onGridReady = function (api) {
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
                ctrl.addColumn = function () {
                    addColumn();
                };
                ctrl.removeColumn = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal(ctrl.columns, dataItem.id, 'id');
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
                        if (ctrl.columns[i].columnName != undefined) {
                            columnNames.push(ctrl.columns[i].columnName.toUpperCase());
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
                            if (array[j] == name)
                                return false;
                        }
                        return true;
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload == undefined) {
                        return;
                    }
                    if (payload.Columns != undefined) {
                        ctrl.tableName = payload.TableName;
                        ctrl.tableSchema = payload.TableSchema;
                    }
                    if (payload.DataRecordTypeId != undefined) {
                        return VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(payload.DataRecordTypeId).then(function (response) {
                            if (response != null && response.Fields != null) {
                                dataRecordTypeFields = [];
                                for (var i = 0; i < response.Fields.length; i++) {
                                    dataRecordTypeFields.push(response.Fields[i]);
                                }
                                loadColumns();
                            }
                        });
                    }

                    function loadColumns() {
                        if (payload.Columns != undefined) {
                            for (var i = 0; i < payload.Columns.length; i++) {
                                addColumn(payload.Columns[i]);
                            }
                        }
                        else {
                            for (var i = 0; i < ctrl.columns.length; i++) {
                                setDataRecordTypeFields(ctrl.columns[i]);
                            }
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageSettings, Vanrise.GenericData.SQLDataStorage',
                        TableName: ctrl.tableName,
                        TableSchema: ctrl.tableSchema,
                        Columns: ctrl.columns.length > 0 ? getColumns() : null
                    };

                    function getColumns() {
                        var columns = [];
                        for (var i = 0; i < ctrl.columns.length; i++) {
                            columns.push({
                                $type: 'Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage',
                                ColumnName: ctrl.columns[i].columnName,
                                SQLDataType: ctrl.columns[i].sqlDataType,
                                ValueExpression: ctrl.columns[i].selectedDataRecordTypeField.Name
                            });
                        }
                        return columns;
                    }
                };

                return api;
            }

            function addColumn(data) {
                var gridItem = {
                    id: ctrl.columns.length + 1
                };

                if (data != undefined) {
                    gridItem.columnName = data.ColumnName,
                    gridItem.sqlDataType = data.SQLDataType,
                    gridItem.selectedDataRecordTypeFieldName = data.ValueExpression,
                    gridItem.isDisabled = true
                }

                gridItem.onSelectorReady = function (api) {
                    setDataRecordTypeFields(gridItem);
                };

                ctrl.columns.push(gridItem);
            }

            function setDataRecordTypeFields(gridItem) {
                gridItem.dataRecordTypeFields = [];
                for (var i = 0; i < dataRecordTypeFields.length; i++) {
                    gridItem.dataRecordTypeFields.push(dataRecordTypeFields[i]);
                }
                if (gridItem.selectedDataRecordTypeFieldName != undefined) {
                    gridItem.selectedDataRecordTypeField = UtilsService.getItemByVal(gridItem.dataRecordTypeFields, gridItem.selectedDataRecordTypeFieldName, 'Name');
                    gridItem.selectedDataRecordTypeFieldName = undefined;
                }
                else {
                    gridItem.selectedDataRecordTypeField = undefined;
                }
            }
        }
    }

    app.directive('vrGenericdataDatarecordstoragesettingsSql', DataRecordStorageSQLSettingsDirective);

})(app);