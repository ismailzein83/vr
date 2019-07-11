(function (app) {

    'use strict';

    GenericFieldsDefinitionDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRLocalizationService', 'VR_GenericData_DataRecordFieldAPIService'];

    function GenericFieldsDefinitionDirective(UtilsService, VRUIUtilsService, VRLocalizationService, VR_GenericData_DataRecordFieldAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericFieldsEditorDefinitionCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/GenericFieldsDefinitionTemplate.html'
        };

        function GenericFieldsEditorDefinitionCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var context;
            var gridAPI;
            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var setFieldsNumber;
            var dataRecordFieldTypes = [];

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.isLocalizationEnabled = VRLocalizationService.isLocalizationEnabled();

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();

                };
                $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onFieldSelected = function (item) {
                    prepareAddedField(item);
                };

                $scope.scopeModel.onFieldDeselected = function (item) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, item.Name, "entity.FieldPath");
                    $scope.scopeModel.datasource.splice(index, 1);

                    if (setFieldsNumber != undefined)
                        setFieldsNumber($scope.scopeModel.datasource.length);
                };

                $scope.scopeModel.removeField = function (dataItem) {
                    var index = $scope.scopeModel.datasource.indexOf(dataItem);
                    var selectedFieldIndex = UtilsService.getItemIndexByVal($scope.scopeModel.selectedFields, dataItem.entity.FieldPath, "Name");
                    $scope.scopeModel.selectedFields.splice(selectedFieldIndex, 1);
                    $scope.scopeModel.datasource.splice(index, 1);

                    if (setFieldsNumber != undefined)
                        setFieldsNumber($scope.scopeModel.datasource.length);
                };

                $scope.scopeModel.deselectAllFields = function () {
                    $scope.scopeModel.datasource.length = 0;

                    if (setFieldsNumber != undefined)
                        setFieldsNumber(0);
                };

                $scope.scopeModel.isValid = function () {
                    if ($scope.scopeModel.datasource == undefined || $scope.scopeModel.datasource.length == 0)
                        return "You Should add at least one Field.";
                    return null;
                };


            }
            function prepareAddedField(item) {

                var fieldType = context.getFieldType(item.Name);
                var dataRecordFieldType = UtilsService.getItemByVal(dataRecordFieldTypes, fieldType.ConfigId, "ExtensionConfigurationId");


                var dataItem = {
                    entity: {
                        FieldPath: item.Name,
                        FieldTitle: item.Title,
                        runtimeViewSettingEditor: fieldType.RuntimeViewSettingEditor
                    },
                };

                if (dataRecordFieldType != undefined) {
                    dataItem.fieldTypeRuntimeDirective = dataRecordFieldType.RuntimeEditor;
                }
                dataItem.onTextResourceSelectorReady = function (api) {
                    dataItem.textResourceSeletorAPI = api;
                    var setLoader = function (value) { dataItem.isFieldTextResourceSelectorLoading = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.textResourceSeletorAPI, undefined, setLoader);
                };

                if (fieldType.RuntimeViewSettingEditor != undefined) {
                    dataItem.onRuntimeViewSettingsEditorDirectiveReady = function (api) {
                        dataItem.directiveAPI = api;
                        var setLoader = function (value) { dataItem.isRuntimeViewSettingsEditorDirectiveLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, { context: context, dataRecordTypeId: fieldType.DataRecordTypeId }, setLoader);
                    };
                }

                dataItem.onFieldTypeRumtimeDirectiveReady = function (api) {
                    dataItem.fieldTypeRuntimeDirectiveAPI = api;
                    var setLoader = function (value) { dataItem.isFieldTypeRumtimeDirectiveLoading = value; };
                    var directivePayload = {
                        fieldTitle: "Default value",
                        fieldType: fieldType,
                        fieldName: item.Name,
                        fieldValue: undefined
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.fieldTypeRuntimeDirectiveAPI, directivePayload, setLoader);
                };
                gridAPI.expandRow(dataItem);
                $scope.scopeModel.datasource.push(dataItem);

                if (setFieldsNumber != undefined)
                    setFieldsNumber($scope.scopeModel.datasource.length);
            }
            function getDataRecordFieldTypeConfigs() {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                    dataRecordFieldTypes = [];
                    for (var i = 0; i < response.length; i++) {
                        dataRecordFieldTypes.push(response[i]);
                    }
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.datasource = [];
                    var fields;
                    var promises = [];
                    var rootPromiseNode;
                    if (payload != undefined) {
                        context = payload.context;
                        fields = payload.fields;
                        setFieldsNumber = payload.setFieldsNumber;

                        var selectedIds = [];
                        if (fields != undefined && fields.length > 0) {
                            for (var i = 0; i < fields.length; i++) {
                                selectedIds.push(fields[i].FieldPath);
                            }
                        }
                        if (context != undefined && context.getDataRecordTypeId() != undefined) {
                            var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                            dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                                var typeFieldsPayload = {
                                    dataRecordTypeId: context.getDataRecordTypeId(),
                                    selectedIds: selectedIds
                                };

                                VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, typeFieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                            });
                            promises.push(loadDataRecordTypeFieldsSelectorPromiseDeferred.promise);

                        }
                        promises.push(getDataRecordFieldTypeConfigs());
                        rootPromiseNode = {
                            promises: promises,
                            getChildNode: function () {
                                var childPromises = [];
                                if (fields != undefined) {
                                    for (var i = 0; i < fields.length; i++) {
                                        var field = fields[i];

                                        var fieldObject = {
                                            payload: field,
                                            textResourceReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                            textResourceLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                            fieldTypeRuntimeReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                            fieldTypeRuntimeLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                        };
                                        if ($scope.scopeModel.isLocalizationEnabled)
                                            childPromises.push(fieldObject.textResourceLoadPromiseDeferred.promise);
                                        childPromises.push(fieldObject.fieldTypeRuntimeLoadPromiseDeferred.promise);
                                        prepareDataItem(fieldObject, childPromises);
                                    }
                                }
                                return { promises: childPromises };
                            }
                        }
                    }
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };


                api.getData = function () {
                    return getFields();
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function prepareDataItem(field, promises) {
                var payload = field.payload;

                var fieldType = context.getFieldType(payload.FieldPath);
                var dataRecordFieldType = UtilsService.getItemByVal(dataRecordFieldTypes, fieldType.ConfigId, "ExtensionConfigurationId");
                var dataItem = {
                    entity: {
                        FieldPath: payload.FieldPath,
                        FieldTitle: payload.FieldTitle,
                        IsRequired: payload.IsRequired,
                        IsDisabled: payload.IsDisabled,
                        ShowAsLabel: payload.ShowAsLabel,
                        HideLabel: payload.HideLabel,
                        ReadOnly: payload.ReadOnly,
                        FieldWidth: payload.FieldWidth,
                        runtimeViewSettingEditor: fieldType.RuntimeViewSettingEditor,
                    },
                    oldTextResourceKey: payload.TextResourceKey
                };
                if (dataRecordFieldType != undefined) {
                    dataItem.fieldTypeRuntimeDirective = dataRecordFieldType.RuntimeEditor;
                }
                dataItem.onTextResourceSelectorReady = function (api) {
                    dataItem.textResourceSeletorAPI = api;
                    field.textResourceReadyPromiseDeferred.resolve();
                };


                field.textResourceReadyPromiseDeferred.promise.then(function () {
                    var textResourcePayload = { selectedValue: field.payload.TextResourceKey };
                    VRUIUtilsService.callDirectiveLoad(dataItem.textResourceSeletorAPI, textResourcePayload, field.textResourceLoadPromiseDeferred);
                });

                if (dataRecordFieldType != undefined) {
                    dataItem.fieldTypeRuntimeDirective = dataRecordFieldType.RuntimeEditor;
                }

                if (fieldType.RuntimeViewSettingEditor)
                    dataItem.onRuntimeViewSettingsEditorDirectiveReady = function (api) {
                        dataItem.directiveAPI = api;
                        field.fieldSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, {
                            configId: payload.FieldViewSettings != undefined ? payload.FieldViewSettings.ConfigId : undefined,
                            context: context,
                            settings: payload.FieldViewSettings,
                            dataRecordTypeId: fieldType.DataRecordTypeId
                        }, field.fieldSettingsLoadPromiseDeferred);
                        promises.push(field.fieldSettingsLoadPromiseDeferred.promise);
                    };

                dataItem.onFieldTypeRumtimeDirectiveReady = function (api) {
                    dataItem.fieldTypeRuntimeDirectiveAPI = api;
                    field.fieldTypeRuntimeReadyPromiseDeferred.resolve();
                };

                field.fieldTypeRuntimeReadyPromiseDeferred.promise.then(function () {
                    var directivePayload = {
                        fieldTitle: "Default Value",
                        fieldType: fieldType,
                        fieldName: payload.FieldPath,
                        fieldValue: payload.DefaultFieldValue
                    };
                    VRUIUtilsService.callDirectiveLoad(dataItem.fieldTypeRuntimeDirectiveAPI, directivePayload, field.fieldTypeRuntimeLoadPromiseDeferred);
                });

                gridAPI.expandRow(dataItem);

                $scope.scopeModel.datasource.push(dataItem);
            }


            function getFields() {
                var fields = [];
                for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                    var currentField = $scope.scopeModel.datasource[i];
                    var fieldEntity = currentField.entity;
                    fields.push({
                        FieldPath: fieldEntity.FieldPath,
                        FieldTitle: fieldEntity.FieldTitle,
                        IsRequired: fieldEntity.IsRequired,
                        IsDisabled: fieldEntity.IsDisabled,
                        ShowAsLabel: fieldEntity.ShowAsLabel,
                        HideLabel: fieldEntity.HideLabel,
                        ReadOnly: fieldEntity.ReadOnly,
                        FieldWidth: fieldEntity.FieldWidth,
                        FieldViewSettings: currentField.directiveAPI != undefined ? currentField.directiveAPI.getData() : undefined,
                        TextResourceKey: currentField.textResourceSeletorAPI != undefined ? currentField.textResourceSeletorAPI.getSelectedValues() : currentField.oldTextResourceKey,
                        DefaultFieldValue: currentField.fieldTypeRuntimeDirectiveAPI != undefined ? currentField.fieldTypeRuntimeDirectiveAPI.getData() : undefined,
                    });
                }
                return fields;
            }
        }
    }

    app.directive('vrGenericdataGenericfieldsDefinition', GenericFieldsDefinitionDirective);

})(app);