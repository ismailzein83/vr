'use strict';
app.directive('vrGenericdataFieldtypeDatarecordtypelistGrideditorviewRuntime', ['VRUIUtilsService', 'UtilsService', 'VR_GenericData_DataRecordFieldAPIService','VR_GenericData_DataRecordTypeService',
    function (VRUIUtilsService, UtilsService, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_DataRecordTypeService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new gridViewTypeListTypeCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/RuntimeViewTypeRuntime/Templates/GridEditorViewTypeRuntimeTemplate.html';
            }
        };

        function gridViewTypeListTypeCtor(ctrl, $scope) {

            $scope.scopeModel = {};
            ctrl.datasource = [];
            $scope.scopeModel.columns = [];
            var columns = {};
            var dataRecordTypeId;
            var definitionSettings;
            var gridAPI;
            function initializeController() {
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
                $scope.scopeModel.onDataRecordTypeRowAdded = function () {
                    var onRowAdded = function (row) {
                        ctrl.datasource.push(row);
                    };
                    VR_GenericData_DataRecordTypeService.addListDataRecordTypeRow(dataRecordTypeId, definitionSettings, onRowAdded, $scope.scopeModel.fieldTitle);
                };
                defineMenuActions();
            }

            function getDataRecordTypeGridColumnsLoadPromise() {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordAttributes(dataRecordTypeId).then(function (response) {
                    var gridColumnAttributes = response;
                    if (gridColumnAttributes != undefined) {
                        for (var index = 0; index < gridColumnAttributes.length; index++) {
                            var gridColumnAttribute = gridColumnAttributes[index];
                            columns[gridColumnAttribute.Name] = gridColumnAttribute.Attribute;
                        }
                    }
                });
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {

                    dataRecordTypeId = payload.dataRecordTypeId;
                    definitionSettings = payload.definitionSettings != undefined ? payload.definitionSettings.Settings : undefined;
                    var fieldsValues = payload.fieldValue;
                    $scope.scopeModel.fieldTitle = payload.fieldTitle;
                    var rootPromiseNode = {
                        promises: [getDataRecordTypeGridColumnsLoadPromise()],
                        getChildNode: function () {

                            if (definitionSettings != undefined && definitionSettings.Rows != undefined) {
                                var rows = definitionSettings.Rows;
                                for (var k = 0; k < rows.length; k++) {
                                    var row = rows[k];
                                    if (row.Fields != undefined) {
                                        var fields = row.Fields;
                                        for (var l = 0; l < fields.length; l++) {
                                            var field = fields[l];
                                            var column = columns[field.FieldPath];
                                            column.Field = "Entity." + field.FieldPath;
                                            column.HeaderText = field.FieldTitle;
                                            column.Name = field.FieldPath;
                                            $scope.scopeModel.columns.push(column);
                                        } 
                                    }
                                }
                            }

                            if (fieldsValues != undefined) {
                                for (var i = 0; i < fieldsValues.length; i++) {
                                    var values = fieldsValues[i];
                                    var dataRow = {};
                                    for (var j = 0; j < $scope.scopeModel.columns.length; j++) {
                                        var columnName = $scope.scopeModel.columns[j].Name;
                                        dataRow[columnName] = values[columnName];
                                    }
                                    ctrl.datasource.push({ Entity: dataRow });
                                }
                            }
                            return {promises:[]}
                        }
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {

                    var returnedData = [];
                    for (var i = 0; i < ctrl.datasource.length; i++) {
                        returnedData.push(ctrl.datasource[i].Entity);
                    }
                    return returnedData;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                    {
                        name: "Edit",
                        clicked: editRow,
                    }];

                $scope.gridMenuActions = function () {
                    return defaultMenuActions;
                };
            }

            function editRow(row) {
                var onRowUpdated = function (updatedRow) {
                    var index = ctrl.datasource.indexOf(row);
                    ctrl.datasource[index] = updatedRow;
                };
                VR_GenericData_DataRecordTypeService.editListDataRecordTypeRow(row, dataRecordTypeId, definitionSettings, onRowUpdated, $scope.scopeModel.fieldTitle);
            }
            this.initializeController = initializeController;
        }
    }]);