'use strict';
app.directive('vrGenericdataFieldtypeDatarecordtypelistGridgenericeditorviewRuntime', ['VRUIUtilsService', 'UtilsService', 'VR_GenericData_DataRecordFieldAPIService',
    function (VRUIUtilsService, UtilsService, VR_GenericData_DataRecordFieldAPIService) {
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/RuntimeViewTypeRuntime/Templates/GridGenericEditorViewTypeRuntimeTemplate.html';
            }
        };

        function gridViewTypeListTypeCtor(ctrl, $scope) {

            $scope.scopeModel = {};
            ctrl.datasource = [];
            $scope.scopeModel.columns = [];
            $scope.scopeModel.autoExpand = false;
            var drillDownEditorSettings;

            var fields=[];
            var columnsInfo;
            var dataRecordTypeId;
            var gridAPI;

           
            function initializeController() {

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.addRecord = function () {
                    var dataRow = []; 
                    for (var k = 0; k < columnsInfo.length; k++) {
                        var columnInfo = columnsInfo[k];
                        var fieldObject = {
                            payload: columnInfo,
                        };

                        prepareAddedField(fieldObject, dataRow);
                    }

                    var row = { Entity: dataRow};

                    if ($scope.scopeModel.enableDrillDown) {

                        row.onRunTimeEditorDirectiveReady = function (api) {
                            row.drillDownEditorDirectiveAPI = api;

                            var runtimeEditorPayload = {
                                dataRecordTypeId: dataRecordTypeId,
                                definitionSettings: drillDownEditorSettings,
                                runtimeEditor: drillDownEditorSettings != undefined ? drillDownEditorSettings.RuntimeEditor : undefined,
                                isEditMode: false
                            };
                            var setLoader = function (value) { row.isRuntimeEditorDirectiveloading = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, row.drillDownEditorDirectiveAPI, runtimeEditorPayload, setLoader);
                        };
                    }

                    if ($scope.scopeModel.autoExpand)
                        gridAPI.expandRow(row);

                    ctrl.datasource.push(row);
                };
                $scope.scopeModel.onDeleteRow = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                $scope.scopeModel.showExpandIcon = function () {
                    return $scope.scopeModel.enableDrillDown;
                };
            }
            function prepareAddedField(fieldObject, dataRow) {
                var editorSettings = fieldObject.payload.editorSettings;

                var entity = {
                    name: fieldObject.payload.name
                };
                entity.onRunTimeEditorDirectiveReady = function (api) {
                    entity.directiveAPI = api;
                    var payload = {
                        dataRecordTypeId: dataRecordTypeId,
                        definitionSettings: editorSettings,
                        runtimeEditor: editorSettings != undefined ? editorSettings.RuntimeEditor : undefined,
                        isEditMode:false
                    };
                    var setLoader = function (value) { entity.isRuntimeEditorDirectiveloading = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, entity.directiveAPI, payload, setLoader);
                };

                dataRow.push(entity);
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    $scope.scopeModel.columns.length = 0;
                    ctrl.datasource.length = 0;
                    $scope.scopeModel.fieldTitle = payload.fieldTitle;
                     dataRecordTypeId = payload.dataRecordTypeId;
                    var fieldsValues = payload.fieldValue;
                    var rootPromiseNode = {
                        promises: []
                    };
                    $scope.scopeModel.colNum = payload.fieldWidth != undefined ? payload.fieldWidth : 12;

                    if (payload.definitionSettings != undefined) {
                        var definitionSettings = payload.definitionSettings;
                        $scope.scopeModel.hideAddButton = definitionSettings.HideAddButton;
                        $scope.scopeModel.hideSection = definitionSettings.HideSection;
                        $scope.scopeModel.enableDraggableRow = definitionSettings.EnableDraggableRow;

                        $scope.scopeModel.deleteFunction = definitionSettings.HideRemoveIcon ? undefined : $scope.scopeModel.onDeleteRow;

                        if (definitionSettings.DrillDownSettings != undefined) {
                            $scope.scopeModel.enableDrillDown = definitionSettings.DrillDownSettings.EnableDrillDown;
                            drillDownEditorSettings = definitionSettings.DrillDownSettings.DrillDownSettings;
                            $scope.scopeModel.autoExpand = definitionSettings.DrillDownSettings.AutoExpand; 
                        }
                        fields = definitionSettings.Fields;
                        gridAPI.refreshGridAttributes();
                    }
                    var columnsPayload = [];
                    columnsInfo = [];

                    if (fields != undefined) {
                        for (var i = 0; i < fields.length; i++) {
                            var field = fields[i];
                            columnsPayload.push({
                                Name: field.HeaderText,
                                Title: field.HeaderText,
                                GridColumnSettings: field.GridColumnSettings
                            });
                            columnsInfo.push({
                                name: field.HeaderText,
                                editorSettings: field.Settings
                            });
                        }
                    }
                    var promise1 = VR_GenericData_DataRecordFieldAPIService.GetListDataRecordTypeGridViewColumnAtts({ ColumnsInfo: columnsPayload }).then(function (response) {
                        $scope.scopeModel.columns = response;
                    });

                    rootPromiseNode.promises = [promise1];

                    rootPromiseNode.getChildNode = function () {
                        var childPromises = [];
                        if (fieldsValues != undefined) {
                            for (var i = 0; i < fieldsValues.length; i++) {
                                var rowValues = fieldsValues[i];
                                var dataRow = [];

                                for (var k = 0; k < columnsInfo.length; k++) {
                                    var columnInfo = columnsInfo[k];
                                    var fieldObject = {
                                        payload: columnInfo,
                                        fieldValue: rowValues,
                                        runtimeFieldReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        runtimeFieldLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    childPromises.push(fieldObject.runtimeFieldLoadPromiseDeferred.promise);
                                    prepareEditedField(fieldObject, dataRow);
                                }
                                var row = { Entity: dataRow, oldValues: rowValues };
                                prepareDrillDownEditor(row);

                                ctrl.datasource.push(row);
                                if ($scope.scopeModel.autoExpand)
                                    gridAPI.expandRow(row);

                            }
                        }
                        function prepareDrillDownEditor(row) {
                            if ($scope.scopeModel.enableDrillDown) {

                                if (!drillDownEditorSettings.AutoExpand) {
                                    row.onRunTimeEditorDirectiveReady = function (api) {
                                        row.drillDownEditorDirectiveAPI = api;

                                        var runtimeEditorPayload = {
                                            selectedValues: row.oldValues,
                                            dataRecordTypeId: dataRecordTypeId,
                                            definitionSettings: drillDownEditorSettings,
                                            parentFieldValues: row.oldValues,
                                            runtimeEditor: drillDownEditorSettings != undefined ? drillDownEditorSettings.RuntimeEditor : undefined,
                                            isEditMode: true
                                        };
                                        var setLoader = function (value) { row.isRuntimeEditorDirectiveloading = value;};
                                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, row.drillDownEditorDirectiveAPI, runtimeEditorPayload, setLoader);

                                    };
                                }
                                else {
                                    row.drillDownEditorSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                                    row.onRunTimeEditorDirectiveReady = function (api) {
                                        row.drillDownEditorDirectiveAPI = api;
                                        row.drillDownEditorSettingsReadyPromiseDeferred.resolve();
                                    };

                                    row.drillDownEditorSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                                    row.drillDownEditorSettingsReadyPromiseDeferred.promise.then(function () {

                                        var runtimeEditorPayload = {
                                            selectedValues: row.oldValues,
                                            dataRecordTypeId: dataRecordTypeId,
                                            definitionSettings: drillDownEditorSettings,
                                            parentFieldValues: row.oldValues,
                                            runtimeEditor: drillDownEditorSettings != undefined ? drillDownEditorSettings.RuntimeEditor : undefined,
                                            isEditMode: true
                                        };
                                        VRUIUtilsService.callDirectiveLoad(row.drillDownEditorDirectiveAPI, runtimeEditorPayload, row.drillDownEditorSettingsReadyPromiseDeferred);
                                    });
                                    childPromises.push(row.drillDownEditorSettingsReadyPromiseDeferred);
                                }

                            }



                        }
                        function prepareEditedField(fieldObject, dataRow) {
                            var editorSettings = fieldObject.payload.editorSettings;

                            var entity = {
                                name: fieldObject.payload.name
                            };
                            entity.onRunTimeEditorDirectiveReady = function (api) {
                                entity.directiveAPI = api;
                                fieldObject.runtimeFieldReadyPromiseDeferred.resolve();
                            };
                            fieldObject.runtimeFieldReadyPromiseDeferred.promise.then(function () {
                                var runtimeEditorPayload = {
                                    selectedValues: fieldObject.fieldValue,
                                    dataRecordTypeId: dataRecordTypeId,
                                    definitionSettings: editorSettings,
                                    parentFieldValues: fieldObject.fieldValue,
                                    runtimeEditor: editorSettings != undefined ? editorSettings.RuntimeEditor : undefined,
                                    isEditMode: true
                                };
                                VRUIUtilsService.callDirectiveLoad(entity.directiveAPI, runtimeEditorPayload, fieldObject.runtimeFieldLoadPromiseDeferred);
                            });

                            dataRow.push(entity);
                        }

                     
                        return {
                            promises: childPromises,
                        };
                    };
                  
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {

                    var returnedData = [];
                    for (var i = 0; i < ctrl.datasource.length; i++) {

                        var row = ctrl.datasource[i];
                        var entity = row.Entity;
                        var rowValues = row.oldValues != undefined ? row.oldValues : {};

                        for (var j = 0; j < entity.length; j++) {
                            var fieldInRow = entity[j]; 
                            if (fieldInRow.directiveAPI != undefined) {
                                fieldInRow.directiveAPI.setData(rowValues);
                            } 
                        }
                        if (row.drillDownEditorDirectiveAPI != undefined)
                            row.drillDownEditorDirectiveAPI.setData(rowValues);
                        returnedData.push(rowValues);
                    } 
                    return returnedData;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
    }]);