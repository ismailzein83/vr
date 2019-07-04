'use strict';
app.directive('vrGenericdataFieldtypeDatarecordtypelistGridviewRuntime', ['VRUIUtilsService', 'UtilsService', 'VR_GenericData_DataRecordFieldAPIService',
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/RuntimeViewTypeRuntime/Templates/GridViewTypeRuntimeTemplate.html';
            }
        };

        function gridViewTypeListTypeCtor(ctrl, $scope) {

            $scope.scopeModel = {};
            ctrl.datasource = [];
            $scope.scopeModel.columns = [];
            var recordTypeFields = {};
            var availableFields=[];

            var gridAPI;
            function initializeController() {
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.addRecord = function () {
                    var dataRow = [];
                    for (var recordTypeField in recordTypeFields) {
                        var field = recordTypeFields[recordTypeField];
                        prepareAddedField(field, dataRow);
                    }
                    ctrl.datasource.push({ Entity: dataRow });
                };
                $scope.scopeModel.onDeleteRow = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
            }
            function prepareAddedField(field, dataRow) {

                var entity = {
                    runtimeEditor: field.fieldType.RuntimeEditor,
                    directiveAPI: undefined,
                    fieldName: field.fieldName,
                    isRequired: field.isRequired,
                    isDisabled: field.isDisabled,
                    showAsLabel: field.showAsLabel
                };
                entity.onRunTimeEditorDirectiveReady = function (api) {
                    entity.directiveAPI = api;
                    var payload = {
                        fieldTitle: '',
                        fieldType: field.fieldType,
                    };
                    var setLoader = function (value) { $scope.scopeModel.isRuntimeEditorDirectiveloading = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, entity.directiveAPI, payload, setLoader);
                };
                dataRow.push(entity);
            }
            function checkIfFieldIsAvailable(entity) {
                if (availableFields == undefined || availableFields.length == 0) {
                    entity.isRequired = true;
                    return true;
                }
                var item = UtilsService.getItemByVal(availableFields, entity.Name, "FieldName");
                if (item != undefined) {
                    entity.isDisabled = item.IsDisabled;
                    entity.isRequired = item.IsRequired;
                    entity.showAsLabel = item.ShowAsLabel;
                    entity.gridColumnSettings = item.GridColumnSettings;
                    return true;
                }
                return false;
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    $scope.scopeModel.columns.length = 0;
                    ctrl.datasource.length = 0;

                    var fieldsDescription;
                    var dataRecordTypeId = payload.dataRecordTypeId;
                    var fieldsValues = payload.fieldValue;
                    $scope.scopeModel.colNum = payload.fieldWidth != undefined ? payload.fieldWidth : 12;
                    $scope.scopeModel.fieldTitle = payload.fieldTitle;
                    var rootPromiseNode = {
                        promises: []
                    };
                    if (payload.definitionSettings != undefined) {
                        var definitionSettings = payload.definitionSettings;
                        $scope.scopeModel.hideAddButton = definitionSettings.HideAddButton;
                        $scope.scopeModel.hideSection = definitionSettings.HideSection;
                        $scope.scopeModel.enableDraggableRow = definitionSettings.EnableDraggableRow;
                        availableFields = definitionSettings.AvailableFields;
                        $scope.scopeModel.deleteFunction = definitionSettings.HideRemoveIcon ? undefined : $scope.scopeModel.onDeleteRow;
                    }
                    if (dataRecordTypeId != undefined) {
                        var dataRecordFields;
                        var dataRecordFieldsInfoPromise = VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId, undefined).then(function (response) {
                            dataRecordFields = response;
                        });
                        rootPromiseNode.promises.push(dataRecordFieldsInfoPromise);
                        rootPromiseNode.getChildNode = function () {
                            var childPromises = [];
                            var fieldTypes = {};
                            var columnsInfo = [];
                            if (dataRecordFields != undefined) {
                                for (var i = 0; i < dataRecordFields.length; i++) {
                                    var entity = dataRecordFields[i].Entity;
                                    if (checkIfFieldIsAvailable(entity)) {
                                        var name = entity.Name;

                                        columnsInfo.push({
                                            Name: entity.Name,
                                            Title: entity.Title,
                                            GridColumnSettings: entity.gridColumnSettings
                                        });

                                        fieldTypes[entity.Name] = entity.Type;

                                        recordTypeFields[name] = {
                                            fieldTitle: '',
                                            fieldType: entity.Type,
                                            fieldName: entity.Name,
                                            isRequired: entity.isRequired,
                                            isDisabled: entity.isDisabled,
                                            showAsLabel: entity.showAsLabel
                                        };

                                    }
                                }
                            }
                            var fieldsValuesList = [];

                            if (fieldsValues != undefined) {
                                for (var i = 0; i < fieldsValues.length; i++) {
                                    var fieldValues = fieldsValues[i];
                                    var rowFields = {};
                                    for (var recordTypeField in recordTypeFields) {
                                        rowFields[recordTypeField] = fieldValues[recordTypeField];
                                    }
                                    fieldsValuesList.push(rowFields);
                                }
                            }

                            var childPromise1 = VR_GenericData_DataRecordFieldAPIService.GetFieldTypeListDescription({ FieldTypes: fieldTypes, FieldsValues: fieldsValuesList }).then(function (response) {
                                fieldsDescription = response;
                            });
                            var childPromise2 = VR_GenericData_DataRecordFieldAPIService.GetListDataRecordTypeGridViewColumnAtts({ ColumnsInfo: columnsInfo }).then(function (response) {
                                $scope.scopeModel.columns = response;
                            });

                            childPromises.push(childPromise1);
                            childPromises.push(childPromise2);

                            return {
                                promises: childPromises,
                                getChildNode: function () {
                                    var childPromises2 = [];
                                    if (fieldsValues != undefined) {
                                        for (var i = 0; i < fieldsValues.length; i++) {
                                            var dataRow = [];
                                            var field = fieldsValues[i];
                                            for (var recordTypeField in recordTypeFields) {
                                                var fieldInfo = recordTypeFields[recordTypeField];
                                                var genericBEFieldObject = {
                                                    fieldValue: field[recordTypeField],
                                                    fieldInfo: fieldInfo,
                                                    fieldLabel: fieldsDescription != undefined && fieldsDescription[i] != undefined ? fieldsDescription[i][recordTypeField] : undefined,
                                                    runtimeFieldReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                                    runtimeFieldLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                                };
                                                if (!genericBEFieldObject.fieldInfo.showAsLabel) {
                                                    childPromises2.push(genericBEFieldObject.runtimeFieldLoadPromiseDeferred.promise);
                                                }
                                                prepareEditedField(genericBEFieldObject, dataRow);
                                            }
                                            ctrl.datasource.push({ Entity: dataRow });
                                        }
                                        function prepareEditedField(genericBEFieldObject, dataRow) {
                                            var entity = {
                                                runtimeEditor: genericBEFieldObject.fieldInfo.fieldType.RuntimeEditor,
                                                fieldValue: genericBEFieldObject.fieldValue,
                                                fieldName: genericBEFieldObject.fieldInfo.fieldName,
                                                fieldLabel: genericBEFieldObject.fieldLabel,
                                                isRequired: genericBEFieldObject.fieldInfo.isRequired,
                                                isDisabled: genericBEFieldObject.fieldInfo.isDisabled,
                                                showAsLabel: genericBEFieldObject.fieldInfo.showAsLabel
                                            };
                                            entity.onRunTimeEditorDirectiveReady = function (api) {
                                                entity.directiveAPI = api;
                                                genericBEFieldObject.runtimeFieldReadyPromiseDeferred.resolve();
                                            };

                                            genericBEFieldObject.runtimeFieldReadyPromiseDeferred.promise.then(function () {
                                                var payload = {
                                                    fieldName: genericBEFieldObject.fieldInfo.fieldName,
                                                    fieldTitle: '',
                                                    fieldType: genericBEFieldObject.fieldInfo.fieldType,
                                                    fieldValue: genericBEFieldObject.fieldValue
                                                };
                                                VRUIUtilsService.callDirectiveLoad(entity.directiveAPI, payload, genericBEFieldObject.runtimeFieldLoadPromiseDeferred);
                                            });

                                            dataRow.push(entity);
                                        }

                                    }
                                    return { promises: childPromises2 };
                                }
                            };
                        };
                    }
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {

                    var returnedData = [];
                    for (var i = 0; i < ctrl.datasource.length; i++) {
                        var row = ctrl.datasource[i].Entity;
                        var dataRow = {};
                        for (var j = 0; j < row.length; j++) {
                            var fieldInRow = row[j];
                            dataRow[fieldInRow.fieldName] = fieldInRow.directiveAPI != undefined ? fieldInRow.directiveAPI.getData() : undefined;
                        }
                        returnedData.push(dataRow);
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