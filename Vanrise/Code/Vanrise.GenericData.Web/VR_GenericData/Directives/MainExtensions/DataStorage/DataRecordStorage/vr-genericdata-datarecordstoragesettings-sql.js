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

            function initializeController() {
                ctrl.columns = [];
                ctrl.dataRecordTypeFields = [];

                ctrl.onGridReady = function (api) {
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
                ctrl.addColumn = function () {
                    addColumn();
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        ctrl.tableName = payload.TableName;
                    }

                    return loadDataRecordTypeFields();

                    function loadDataRecordTypeFields() {
                        var dataRecordTypeFieldsLoadDeferred = UtilsService.createPromiseDeferred();

                        if (payload == undefined || payload.DataRecordTypeId == undefined) {
                            dataRecordTypeFieldsLoadDeferred.reject();
                        }
                        else {
                            VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(payload.DataRecordTypeId).then(function (response) {
                                if (response != null && response.Fields != null) {
                                    for (var i = 0; i < response.Fields.length; i++) {
                                        ctrl.dataRecordTypeFields.push(response.Fields[i]);
                                    }
                                    if (payload.Columns != undefined) {
                                        loadColumns();
                                    }
                                }
                            }).catch(function (error) {
                                dataRecordTypeFieldsLoadDeferred.reject(error);
                            });
                        }

                        return dataRecordTypeFieldsLoadDeferred.promise;

                        function loadColumns() {
                            for (var i = 0; i < payload.Columns.length; i++) {
                                addColumn(payload.Columns[i]);
                            }
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.MainExtensions.DataStorages.SQLDataRecordStorageSettings, Vanrise.GenericData.MainExtensions',
                        TableName: ctrl.tableName,
                        Columns: ctrl.columns.length > 0 ? getColumns() : null
                    };

                    function getColumns() {
                        var columns = [];
                        for (var i = 0; i < ctrl.columns.length; i++) {
                            columns.push({
                                $type: 'Vanrise.GenericData.MainExtensions.DataStorages.SQLDataRecordStorageColumn, Vanrise.GenericData.MainExtensions',
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
                var gridItem = {};
                setDataRecordTypeFields(gridItem);

                if (data != undefined) {
                    gridItem = {
                        columnName: data.ColumnName,
                        sqlDataType: data.SQLDataType,
                        selectedDataRecordTypeFieldName: data.ValueExpression
                    };
                }

                gridItem.onSelectorReady = function (api) {
                    gridItem.selectedDataRecordTypeField = UtilsService.getItemByVal(ctrl.dataRecordTypeFields, gridItem.selectedDataRecordTypeFieldName, 'Name');
                    gridItem.selectedDataRecordTypeFieldName = undefined;
                };

                ctrl.columns.push(gridItem);

                function setDataRecordTypeFields(gridItem) {
                    gridItem.dataRecordTypeFields = [];
                    for (var i = 0; i < ctrl.dataRecordTypeFields.length; i++) {
                        gridItem.dataRecordTypeFields.push(ctrl.dataRecordTypeFields[i]);
                    }
                }
            }
        }
    }

    app.directive('vrGenericdataDatarecordstoragesettingsSql', DataRecordStorageSQLSettingsDirective);

})(app);