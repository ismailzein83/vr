'use strict';
app.directive('vrGenericdataFieldtypeDatarecordtypelistGrideditorviewRuntime', ['VRUIUtilsService', 'UtilsService', 'VR_GenericData_DataRecordFieldAPIService', 'VR_GenericData_DataRecordTypeService', 'VR_GenericData_GenericBusinessEntityAPIService',
    function (VRUIUtilsService, UtilsService, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_DataRecordTypeService, VR_GenericData_GenericBusinessEntityAPIService) {
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
            var dataRecordTypeId;
            var editorSettings;
            var gridAPI;
            function initializeController() {
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
                $scope.scopeModel.onDataRecordTypeRowAdded = function () {
                    var onRowAdded = function (row) {
                        VR_GenericData_DataRecordFieldAPIService.GetFieldsDescription({ DataRecordTypeId: dataRecordTypeId, FieldsValues: [row.Entity] }).then(function (response) {
                            ctrl.datasource.push({ Values: row.Entity, Entity: response != undefined && response.length > 0 ? response[0] : undefined });
                        });
                    };
                    VR_GenericData_DataRecordTypeService.addListDataRecordTypeRow(dataRecordTypeId, editorSettings, onRowAdded, $scope.scopeModel.fieldTitle);
                };
                $scope.scopeModel.onDeleteRow = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
            }

            function defineAPI() {
                var api = {};
                var fieldsDescription;
                api.load = function (payload) {
                    dataRecordTypeId = payload.dataRecordTypeId;

                    var definitionSettings = payload.definitionSettings;
                    editorSettings = definitionSettings != undefined ? definitionSettings.Settings : undefined;
                    $scope.scopeModel.hideSection = definitionSettings.HideSection;
                    var fieldsValues = [];

                    var fields = payload.fieldValue;

                    $scope.scopeModel.fieldTitle = payload.fieldTitle;
                    var input = {
                        ListRecordViewType: definitionSettings,
                        DataRecordTypeId: dataRecordTypeId,
                    };
                    if (fields != undefined && fields.length > 0) {
                        for (var i = 0; i < fields.length > 0; i++) {
                            var field = fields[i];
                            var rowFieldsValues = {};

                            for (var prop in field) {
                                if (prop != '$type' && prop != 'QueueItemId')
                                    rowFieldsValues[prop] = field[prop];
                            }
                            fieldsValues.push(rowFieldsValues);
                        } 
                    }
                    var getColumnsInfoPromise = VR_GenericData_GenericBusinessEntityAPIService.GetGenericEditorColumnsInfo(input).then(function (response) {
                        if (response != undefined) {
                            for (var col in response) {
                                if (col != '$type') {
                                    var column = response[col];
                                    column.Field = "Entity." + column.Field;
                                    $scope.scopeModel.columns.push(column);
                                }
                            }
                        }
                    });
                    var getFieldsDescriptionPromise = VR_GenericData_DataRecordFieldAPIService.GetFieldsDescription({ DataRecordTypeId: dataRecordTypeId, FieldsValues: fieldsValues }).then(function (response) {
                        fieldsDescription = response;
                        if (fieldsDescription != undefined && fieldsDescription.length > 0) {

                            for (var i = 0; i < fieldsDescription.length; i++) {
                                ctrl.datasource.push({ Values: fieldsValues[i], Entity: fieldsDescription[i] });
                            }
                        }
                    });


                    var rootPromiseNode = {
                        promises: [getColumnsInfoPromise, getFieldsDescriptionPromise]
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {

                    var returnedData = [];
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            returnedData.push(ctrl.datasource[i].Values);
                        }
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
                VR_GenericData_DataRecordTypeService.editListDataRecordTypeRow(row, dataRecordTypeId, editorSettings, onRowUpdated, $scope.scopeModel.fieldTitle);
            }
            this.initializeController = initializeController;
        }
    }]);