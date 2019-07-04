//'use strict';
//app.directive('vrGenericdataFieldtypeDatarecordtypelistGridgenericeditorviewRuntime', ['VRUIUtilsService', 'UtilsService', 'VR_GenericData_DataRecordFieldAPIService',
//    function (VRUIUtilsService, UtilsService, VR_GenericData_DataRecordFieldAPIService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//                normalColNum: '@'
//            },
//            controller: function ($scope, $element, $attrs) {

//                var ctrl = this;

//                var ctor = new gridViewTypeListTypeCtor(ctrl, $scope);
//                ctor.initializeController();

//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            compile: function (element, attrs) {
//                return {
//                    pre: function ($scope, iElem, iAttrs, ctrl) {

//                    }
//                };
//            },
//            templateUrl: function (element, attrs) {
//                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/RuntimeViewTypeRuntime/Templates/GridGenericEditorViewTypeRuntimeTemplate.html';
//            }
//        };

//        function gridViewTypeListTypeCtor(ctrl, $scope) {

//            $scope.scopeModel = {};
//            ctrl.datasource = [];
//            $scope.scopeModel.columns = [];
//            var recordTypeFields = {};
//            var fields=[];
//            var columnsInfo;
//            var dataRecordTypeId;
//            var gridAPI;

//            var runtimeEditorAPI;
//            var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();
//            function initializeController() {
//                $scope.scopeModel.onGridReady = function (api) {
//                    gridAPI = api;
//                    defineAPI();
//                };

//                $scope.scopeModel.addRecord = function () {
//                    var dataRow = []; 
//                    for (var k = 0; k < columnsInfo.length; k++) {
//                        var columnInfo = columnsInfo[k];
//                        var fieldObject = {
//                            payload: columnInfo,
//                        };
//                        prepareAddedField(fieldObject, dataRow);
//                    }
//                    ctrl.datasource.push({ Entity: dataRow });
//                };
//                $scope.scopeModel.onDeleteRow = function (dataItem) {
//                    var index = ctrl.datasource.indexOf(dataItem);
//                    ctrl.datasource.splice(index, 1);
//                };

//                $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
//                    runtimeEditorAPI = api;
//                    runtimeEditorReadyDeferred.resolve();
//                };
//            }
//            function prepareAddedField(fieldObject, dataRow) {
//                var editorSettings = fieldObject.payload.editorSettings;

//                var entity = {
//                    name: fieldObject.payload.name
//                };
//                entity.onRunTimeEditorDirectiveReady = function (api) {
//                    entity.directiveAPI = api;
//                    var payload = {
//                        dataRecordTypeId: dataRecordTypeId,
//                        definitionSettings: editorSettings,
//                        runtimeEditor: editorSettings != undefined ? editorSettings.RuntimeEditor : undefined,
//                        isEditMode:false
//                    };
//                    var setLoader = function (value) { $scope.scopeModel.isRuntimeEditorDirectiveloading = value; };
//                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, entity.directiveAPI, payload, setLoader);
//                };

//                dataRow.push(entity);
//            }
//            function loadEditorRuntimeDirective() {
//                var runtimeEditorLoadDeferred = UtilsService.createPromiseDeferred();
//                runtimeEditorReadyDeferred.promise.then(function () {

//                    var runtimeEditorPayload = {
//                        selectedValues: fieldValues,
//                        dataRecordTypeId: dataRecordTypeId,
//                        definitionSettings: definitionSettings,
//                        parentFieldValues: fieldValues,
//                        runtimeEditor: definitionSettings != undefined ? definitionSettings.RuntimeEditor : undefined,
//                        isEditMode: false
//                    };
//                    VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, runtimeEditorPayload, runtimeEditorLoadDeferred);
//                });

//                return runtimeEditorLoadDeferred.promise;
//            }

//            function defineAPI() {
//                var api = {};
//                api.load = function (payload) {
//                    $scope.scopeModel.columns.length = 0;
//                    ctrl.datasource.length = 0;

//                     dataRecordTypeId = payload.dataRecordTypeId;
//                    var fieldsValues = payload.fieldValue;
//                    var rootPromiseNode = {
//                        promises: []
//                    };
//                    if (payload.definitionSettings != undefined) {
//                        var definitionSettings = payload.definitionSettings;
//                        $scope.scopeModel.hideAddButton = definitionSettings.HideAddButton;
//                        $scope.scopeModel.hideSection = definitionSettings.HideSection;
//                        $scope.scopeModel.enableDraggableRow = definitionSettings.EnableDraggableRow;
//                        if (definitionSettings.DrillDownSettings) {
//                            $scope.scopeModel.enableDrillDown = definitionSettings.DrillDownSettings.EnableDrillDown; 

//                        }
//                        $scope.scopeModel.autoExpand = definitionSettings.AutoExpand;
//                        fields = definitionSettings.Fields;
//                    }
//                    var columnsPayload = [];
//                     columnsInfo = [];
//                    if (fields != undefined) {
//                        for (var i = 0; i < fields.length; i++) {
//                            var field = fields[i];
//                            columnsPayload.push({
//                                Name: field.HeaderText,
//                                Title: field.HeaderText,
//                                GridColumnSettings: field.GridColumnSettings
//                            });
//                            columnsInfo.push({
//                                name: field.HeaderText,
//                                editorSettings: field.Settings
//                            })
//                        }
//                    }
//                    var promise1 = VR_GenericData_DataRecordFieldAPIService.GetListDataRecordTypeGridViewColumnAtts({ ColumnsInfo: columnsPayload }).then(function (response) {
//                        $scope.scopeModel.columns = response;
//                    });
//                    rootPromiseNode.promises =  [promise1];
//                    rootPromiseNode.getChildNode = function () {
//                        var childPromises = [];
//                        if (fieldsValues != undefined) {
//                            for (var i = 0; i < fieldsValues.length; i++) {
//                                var rowValues = fieldsValues[i];
//                                var dataRow = [];

//                                for (var k = 0; k < columnsInfo.length; k++) {
//                                    var columnInfo = columnsInfo[k];
//                                    var fieldObject = {
//                                        payload: columnInfo,
//                                        fieldValue: rowValues,
//                                        runtimeFieldReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
//                                        runtimeFieldLoadPromiseDeferred: UtilsService.createPromiseDeferred()
//                                    };
//                                    childPromises.push(fieldObject.runtimeFieldLoadPromiseDeferred.promise);
//                                    prepareEditedField(fieldObject, dataRow);
//                                }
//                                ctrl.datasource.push({ Entity: dataRow });
//                            }
//                        }
//                        function prepareEditedField(fieldObject, dataRow) {
//                            var editorSettings = fieldObject.payload.editorSettings;

//                            var entity = {
//                                name: fieldObject.payload.name
//                            };
//                            entity.onRunTimeEditorDirectiveReady = function (api) {
//                                entity.directiveAPI = api;
//                                fieldObject.runtimeFieldReadyPromiseDeferred.resolve();
//                            };
//                            fieldObject.runtimeFieldReadyPromiseDeferred.promise.then(function () {
//                                var runtimeEditorPayload = {
//                                    selectedValues: fieldObject.fieldValue,
//                                    dataRecordTypeId: dataRecordTypeId,
//                                    definitionSettings: editorSettings,
//                                    parentFieldValues: fieldObject.fieldValue,
//                                    runtimeEditor: editorSettings != undefined ? editorSettings.RuntimeEditor : undefined,
//                                    isEditMode: true
//                                };
//                                VRUIUtilsService.callDirectiveLoad(entity.directiveAPI, runtimeEditorPayload, fieldObject.runtimeFieldLoadPromiseDeferred);
//                            });

//                            dataRow.push(entity);
//                        }

//                        return {
//                            promises: childPromises,
//                        };
//                    };
//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };

//                api.getData = function () {

//                    var returnedData = [];
//                    for (var i = 0; i < ctrl.datasource.length; i++) {
//                        var row = ctrl.datasource[i].Entity;
//                        var dataRow = {};
//                        for (var j = 0; j < row.length; j++) {
//                            var fieldInRow = row[j]; 
//                            if (fieldInRow.directiveAPI != undefined) {
//                                fieldInRow.directiveAPI.setData(dataRow);
//                            } 
//                        }
//                        returnedData.push(dataRow);
//                    } 
//                    return returnedData;
//                };

//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }

//            this.initializeController = initializeController;
//        }
//    }]);