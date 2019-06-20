'use strict';
app.directive('vrGenericdataFieldtypeDatarecordtypelistGridviewRuntime', ['VRUIUtilsService', 'UtilsService','VR_GenericData_DataRecordFieldAPIService',
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
            var availableFields;

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
                    isDisabled: field.isDisabled
                };
                entity.onRunTimeEditorDirectiveReady = function (api) {
                    entity.directiveAPI = api;
                    var payload = {
                        fieldTitle: '',
                        fieldType: field.fieldType,
                       
                    };
                    var setLoader = function (value) { $scope.scopeModel.isRuntimeEditorDirectiveloading = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, entity.directiveAPI , payload, setLoader);
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
                    return true;
                }
                return false;
            }
        
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    var dataRecordTypeId = payload.dataRecordTypeId;
                    var fieldsValues = payload.fieldValue;
                    $scope.scopeModel.fieldTitle = payload.fieldTitle;
                    var rootPromiseNode = {
                        promises: []
                    };
                    if (payload.definitionSettings != undefined) {
                        var definitionSettings = payload.definitionSettings
                        $scope.scopeModel.hideAddButton = definitionSettings.HideAddButton;
                        $scope.scopeModel.hideSection = definitionSettings.HideSection;
                        availableFields = definitionSettings.AvailableFields;
                    }
                    if (dataRecordTypeId != undefined) {
                        var dataRecordFields;
                        var dataRecordFieldsInfoPromise = VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId, undefined).then(function (response) {
                            dataRecordFields = response;
                        });
                        rootPromiseNode.promises.push(dataRecordFieldsInfoPromise);
                        rootPromiseNode.getChildNode = function () {
                            var childPromises = [];
                            if (dataRecordFields != undefined) {
                                for (var i = 0; i < dataRecordFields.length; i++) {
                                    var entity = dataRecordFields[i].Entity;
                                    if (checkIfFieldIsAvailable(entity)) {
                                        var name = entity.Name;
                                        $scope.scopeModel.columns.push({ name: name, title: entity.Title });
                                        recordTypeFields[name] = {
                                            fieldTitle: '',
                                            fieldType: entity.Type,
                                            fieldName: entity.Name,
                                            isRequired: entity.isRequired,
                                            isDisabled: entity.isDisabled
                                        };
                                    }
                                }
                            }
                   
                            if (fieldsValues != undefined) {
                                for (var i = 0; i < fieldsValues.length; i++) {
                                    var dataRow = [];
                                    var field = fieldsValues[i];
                                    for (var recordTypeField in recordTypeFields) {
                                        var fieldInfo = recordTypeFields[recordTypeField];
                                        var genericBEFieldObject = {
                                            fieldValue: field[recordTypeField],
                                            fieldInfo: fieldInfo,
                                            runtimeFieldReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                            runtimeFieldLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                        };
                                        childPromises.push(genericBEFieldObject.runtimeFieldLoadPromiseDeferred.promise);
                                        prepareEditedField(genericBEFieldObject, dataRow);
                                    }
                                    ctrl.datasource.push({ Entity: dataRow });
                                }

                                function prepareEditedField(genericBEFieldObject, dataRow) {
                                    var entity = {
                                        runtimeEditor: genericBEFieldObject.fieldInfo.fieldType.RuntimeEditor,
                                        fieldValue: genericBEFieldObject.fieldValue,
                                        fieldName: genericBEFieldObject.fieldInfo.fieldName,
                                        isRequired: genericBEFieldObject.fieldInfo.isRequired,
                                        isDisabled: genericBEFieldObject.fieldInfo.isDisabled
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

                            return { promises: childPromises };
                        }
                    }
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {

                    var returnedData = [];
                    for (var i = 0; i < ctrl.datasource.length; i++) {
                        var row = ctrl.datasource[i].Entity;
                        var dataRow = {};
                        for (var j = 0; j < row.length;j++ ) {
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