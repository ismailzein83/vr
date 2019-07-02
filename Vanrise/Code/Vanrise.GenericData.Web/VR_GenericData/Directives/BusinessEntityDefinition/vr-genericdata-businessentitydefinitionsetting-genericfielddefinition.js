(function (app) {

    'use strict';

    GenericFieldDefinition.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService'];

    function GenericFieldDefinition(UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                $scope.scopeModel = {};
                $scope.scopeModel.selectedFields = [];
                $scope.scopeModel.extendedSelectedFields = [];

                if ($attrs.ismultipleselection != undefined) {
                    $scope.scopeModel.selectedFields = [];
                }

                var genericFieldDefinitionDirective = new GenericFieldDefinitionDirective(ctrl, $scope, $attrs);
                genericFieldDefinitionDirective.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function getDirectiveTemplate(attrs) {

            var ismultipleselection = '';
            var hideselectedvaluessection = '';

            if (attrs.ismultipleselection != undefined) {
                ismultipleselection = 'ismultipleselection';
                hideselectedvaluessection = 'hideselectedvaluessection';
            }

            return '<vr-columns width="1/3row">'
                + ' <vr-select datatextfield="fieldTitle" datavaluefield="fieldPath" label="Fields" isrequired="true"'
                + '    entityname="Field Path" selectedvalues="scopeModel.selectedFields" datasource="scopeModel.fields"'
                + ' onselectitem="scopeModel.onSelectField" ondeselectitem="scopeModel.onDeSelectField" ' + ismultipleselection + ' ' + hideselectedvaluessection + ' hideremoveicon>'
                + '</vr-select>'
                + '</vr-columns>'
                + '<vr-columns width="2/3row">'
                + '  <vr-datalist maxitemsperrow="1" datasource="scopeModel.extendedSelectedFields" isitemdraggable onremoveitem="scopeModel.onRemoveField">'
                + '      <vr-row removeline>'
                + '          <vr-columns width="1/3row">'
                + '              <vr-label>Field Name: {{ dataItem.fieldPath }}</vr-label>'
                + '          </vr-columns>'
                + '      </vr-row>'
                + '      <vr-row removeline>'
                + '          <vr-columns width="1/3row">'
                + '              <vr-textbox value="dataItem.fieldTitle" label="Field Title" isrequired="true"></vr-textbox>'
                + '          </vr-columns>'
                + '         <span vr-loader="dataItem.isFieldTypeRumtimeDirectiveLoading" ng-if="dataItem.fieldTypeRuntimeDirective != undefined">'
                + '              <vr-directivewrapper directive="dataItem.fieldTypeRuntimeDirective"'
                + '                                   on-ready="dataItem.onFieldTypeRumtimeDirectiveReady"'
                + '                                   selectionmode = "single"'
                + '                                   normal-col-num="4">'
                + '             </vr-directivewrapper>'
                + '	        </span>'
                + '          <vr-columns colnum = "4">'
                + '              <vr-textbox value="dataItem.fieldWidth" label="Field Width" type="number" decimalprecision="0" minvalue="1" maxvalue="12"></vr-textbox>'
                + '          </vr-columns>'
                + '      </vr-row>'
                + '      <vr-row removeline>'
                + '         <span vr-loader="dataItem.isLocalizationTextResourceSelectorLoading">'
                + '              <vr-common-vrlocalizationtextresource-selector on-ready="dataItem.onLocalizationTextResourceDirectiveReady" normal-col-num="4"></vr-common - vrlocalizationtextresource - selector > '
                + '         </span>'
                +'           <vr-columns colnum="8" haschildcolumns>'
                + '              <vr-columns colnum="4">'
                + '                  <vr-switch value="dataItem.isRequired" label="Required"></vr-switch>'
                + '              </vr-columns>'
                + '              <vr-columns colnum="4">'
                + '                  <vr-switch value="dataItem.isDisabled" label="Disabled"></vr-switch>'
                + '              </vr-columns>'
                + '              <vr-columns colnum="4">'
                + '                 <vr-switch value="dataItem.showAsLabel" label="Show as Label"></vr-switch>'
                + '             </vr-columns>'
                + '          </vr-columns>'
                + '      </vr-row>'
                + '          <span vr-loader="scopeModel.isRuntimeViewSettingsEditorDirectiveLoading" ng-if="dataItem.runtimeViewSettingEditor !=undefined">'
                + '              <vr-directivewrapper  directive="dataItem.runtimeViewSettingEditor" '
                + 'selectionmode=single isrequired="true" normal-col-num="6" on-ready="dataItem.onRuntimeViewSettingsEditorDirectiveReady"></vr-directivewrapper>'
                + '        </span>'
             
                + '  </vr-datalist>'
                + '</vr-columns>';
        }

        function GenericFieldDefinitionDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel.fields = [];

                $scope.scopeModel.onSelectField = function (dataItem) {
                    if (attrs.ismultipleselection != undefined) {
                        $scope.scopeModel.extendedSelectedFields.push(dataItem);
                    }
                    else {
                        $scope.scopeModel.extendedSelectedFields = [dataItem];
                    }
                };

                $scope.scopeModel.onDeSelectField = function (dataItem) {
                    if (attrs.ismultipleselection != undefined) {
                        var index = $scope.scopeModel.extendedSelectedFields.indexOf(dataItem);
                        if (index != -1) {
                            $scope.scopeModel.extendedSelectedFields.splice(index, 1);
                        }
                    }
                };

                $scope.scopeModel.onRemoveField = function (dataItem) {
                    var index;
                    if (attrs.ismultipleselection != undefined) {
                        index = $scope.scopeModel.selectedFields.indexOf(dataItem);
                        if (index != -1) {
                            $scope.scopeModel.selectedFields.splice(index, 1);
                        }
                    }
                    else {
                        $scope.scopeModel.selectedFields = undefined;
                    }

                    index = $scope.scopeModel.extendedSelectedFields.indexOf(dataItem);
                    if (index != -1) {
                        $scope.scopeModel.extendedSelectedFields.splice(index, 1);
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.fields.length = 0;
                    var initialPromises = [];

                    var recordTypeFields;
                    var context;
                    var entity;
                    var dataRecordFieldTypes= [];

                    if (payload != undefined) {
                        recordTypeFields = payload.recordTypeFields;
                        context = payload.context;
                        entity = payload.entity;
                    }

                    function getDataRecordFieldTypeConfigs() {
                        return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                            for (var i = 0; i < response.length; i++) {
                                var dataRecordFieldType = response[i];
                                dataRecordFieldTypes.push(dataRecordFieldType);
                            }
                        });
                    }

                    function prepareFieldData(field) {
                        var fieldType = context.getFieldType(field.FieldPath);
                        var dataRecordFieldType = UtilsService.getItemByVal(dataRecordFieldTypes, fieldType.ConfigId, "ExtensionConfigurationId");
                        var fieldData = {
                            fieldPath: field.FieldPath,
                            fieldTitle: field.FieldTitle,
                            defaultFieldValue: undefined,
                            runtimeViewSettingEditor: fieldType.RuntimeViewSettingEditor,
                            fieldTypeRuntimeDirective: undefined,
                            directiveAPI: undefined,
                            localizationResourceSelectorDirectiveReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                            fieldTypeRuntimeDirectiveAPI: undefined,
                            fieldTypeRuntimeReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                        };

                        if (dataRecordFieldType != undefined) {
                            fieldData.fieldTypeRuntimeDirective = dataRecordFieldType.RuntimeEditor;
                        }

                        fieldData.onLocalizationTextResourceDirectiveReady = function (api) {
                            fieldData.localizationTextResourceSelectorAPI = api;
                            fieldData.localizationResourceSelectorDirectiveReadyPromiseDeferred.resolve();
                        };

                        fieldData.localizationResourceSelectorDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payload = {
                                selectedValue: field != undefined ? field.TextResourceKey : undefined
                            };
                            var setLoader = function (value) { fieldData.isLocalizationTextResourceSelectorLoading = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, fieldData.localizationTextResourceSelectorAPI, payload, setLoader);
                        });

                        if (fieldType.RuntimeViewSettingEditor) {
                            fieldData.onRuntimeViewSettingsEditorDirectiveReady = function (api) {
                                fieldData.directiveAPI = api;
                                var setLoader = function (value) { $scope.scopeModel.isRuntimeViewSettingsEditorDirectiveLoading = value; };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, fieldData.directiveAPI, { context: context, dataRecordTypeId: fieldType.DataRecordTypeId }, setLoader);
                            };
                        }

                        fieldData.onFieldTypeRumtimeDirectiveReady = function (api) {
                            fieldData.fieldTypeRuntimeDirectiveAPI = api;
                            fieldData.fieldTypeRuntimeReadyPromiseDeferred.resolve();
                        };

                        fieldData.fieldTypeRuntimeReadyPromiseDeferred.promise.then(function () {
                            var setLoader = function (value) { fieldData.isFieldTypeRumtimeDirectiveLoading = value; };
                            var directivePayload = {
                                fieldTitle: "Default value",
                                fieldType: fieldType,
                                fieldName: fieldData.fieldPath,
                                fieldValue: undefined
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, fieldData.fieldTypeRuntimeDirectiveAPI, directivePayload, setLoader);
                        });

                        return fieldData;
                    }

                    function loadGenericFields() {
                        var _promises = [];
                        var rootPromiseNode = {
                            promises: _promises
                        };
                        if (entity != undefined && recordTypeFields != undefined) {
                            if (attrs.ismultipleselection != undefined) {
                                for (var i = 0; i < entity.length; i++) {
                                    var selectedField = entity[i];
                                    setSelectedFields(selectedField);
                                }
                            }
                            else {
                                setSelectedFields(entity);
                            }
                        }

                        function setSelectedFields(selectedField) {
                            for (var j = 0; j < recordTypeFields.length; j++) {
                                var field = recordTypeFields[j];
                                if (field.FieldPath == selectedField.FieldPath) {
                                    var fieldInfo = prepareFieldInfo(field, selectedField);
                                    _promises.push(fieldInfo.localizationResourceSelectorLoadPromiseDeferred.promise);
                                    if (fieldInfo.runtimeViewSettingEditor != undefined)
                                        _promises.push(fieldInfo.fieldSettingsLoadPromiseDeferred.promise);
                                    if (attrs.ismultipleselection != undefined)
                                        $scope.scopeModel.selectedFields.push(fieldInfo);
                                    else
                                        $scope.scopeModel.selectedFields = fieldInfo;
                                    $scope.scopeModel.extendedSelectedFields.push(fieldInfo);
                                }
                            }
                        }

                        function prepareFieldInfo(field, selectedField) {
                            var fieldType = context.getFieldType(field.FieldPath);
                            var dataRecordFieldType = UtilsService.getItemByVal(dataRecordFieldTypes, fieldType.ConfigId, "ExtensionConfigurationId");
                            var fieldInfo = {
                                fieldPath: field.FieldPath,
                                fieldTitle: selectedField.FieldTitle,
                                defaultFieldValue: selectedField.DefaultFieldValue,
                                runtimeViewSettingEditor: fieldType.RuntimeViewSettingEditor,
                                fieldSettingsLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                isRequired: selectedField.IsRequired,
                                isDisabled: selectedField.IsDisabled,
                                showAsLabel: selectedField.ShowAsLabel,
                                fieldWidth: selectedField.FieldWidth,
                                directiveAPI: undefined,
                                localizationResourceSelectorDirectiveReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                localizationResourceSelectorLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                fieldTypeRuntimeDirectiveAPI: undefined,
                                fieldTypeRuntimeReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                fieldTypeRuntimeLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                            };

                            if (dataRecordFieldType != undefined) {
                                fieldInfo.fieldTypeRuntimeDirective = dataRecordFieldType.RuntimeEditor;
                            }

                            fieldInfo.onLocalizationTextResourceDirectiveReady = function (api) {
                                fieldInfo.localizationTextResourceSelectorAPI = api;
                                fieldInfo.localizationResourceSelectorDirectiveReadyPromiseDeferred.resolve();
                            };


                            fieldInfo.localizationResourceSelectorDirectiveReadyPromiseDeferred.promise.then(function () {
                                var payload = {
                                    selectedValue: selectedField != undefined ? selectedField.TextResourceKey : undefined
                                };
                                VRUIUtilsService.callDirectiveLoad(fieldInfo.localizationTextResourceSelectorAPI, payload, fieldInfo.localizationResourceSelectorLoadPromiseDeferred);
                            });

                            if (fieldType.RuntimeViewSettingEditor)
                                fieldInfo.onRuntimeViewSettingsEditorDirectiveReady = function (api) {
                                    fieldInfo.directiveAPI = api;
                                    VRUIUtilsService.callDirectiveLoad(fieldInfo.directiveAPI, {
                                        configId: selectedField.FieldViewSettings != undefined ? selectedField.FieldViewSettings.ConfigId : undefined,
                                        context: context,
                                        settings: selectedField.FieldViewSettings,
                                        dataRecordTypeId: fieldType.DataRecordTypeId
                                    }, fieldInfo.fieldSettingsLoadPromiseDeferred);
                                };

                            fieldInfo.onFieldTypeRumtimeDirectiveReady = function (api) {
                                fieldInfo.fieldTypeRuntimeDirectiveAPI = api;
                                fieldInfo.fieldTypeRuntimeReadyPromiseDeferred.resolve();
                            };

                            fieldInfo.fieldTypeRuntimeReadyPromiseDeferred.promise.then(function () {
                                var directivePayload = {
                                    fieldTitle: "Default Value",
                                    fieldType: fieldType,
                                    fieldName: fieldInfo.fieldPath,
                                    fieldValue: fieldInfo.defaultFieldValue
                                };
                                VRUIUtilsService.callDirectiveLoad(fieldInfo.fieldTypeRuntimeDirectiveAPI, directivePayload, fieldInfo.fieldTypeRuntimeLoadPromiseDeferred);
                            });

                            return fieldInfo;
                        }

                        return UtilsService.waitPromiseNode(rootPromiseNode);
                    }

                    initialPromises.push(getDataRecordFieldTypeConfigs());

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];

                            if (recordTypeFields != undefined) {
                                for (var i = 0; i < recordTypeFields.length; i++) {
                                    var field = recordTypeFields[i];
                                    $scope.scopeModel.fields.push(prepareFieldData(field));
                                }
                            }

                            directivePromises.push(loadGenericFields());

                            return {
                                promises: directivePromises,
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    if ($scope.scopeModel.extendedSelectedFields == undefined || $scope.scopeModel.extendedSelectedFields.length == 0)
                        return null;

                    var rowObject = [];
                    for (var i = 0; i < $scope.scopeModel.extendedSelectedFields.length; i++) {
                        var selectedField = $scope.scopeModel.extendedSelectedFields[i];
                        rowObject.push({
                            FieldPath: selectedField.fieldPath,
                            FieldTitle: selectedField.fieldTitle,
                            IsRequired: selectedField.isRequired,
                            IsDisabled: selectedField.isDisabled,
                            ShowAsLabel: selectedField.showAsLabel,
                            FieldWidth: selectedField.fieldWidth,
                            FieldViewSettings: selectedField.directiveAPI != undefined ? selectedField.directiveAPI.getData() : undefined,
                            TextResourceKey: selectedField.localizationTextResourceSelectorAPI.getSelectedValues(),
                            DefaultFieldValue: selectedField.fieldTypeRuntimeDirectiveAPI != undefined ? selectedField.fieldTypeRuntimeDirectiveAPI.getData() : undefined
                        });
                    }

                    if (attrs.ismultipleselection != undefined)
                        return rowObject;

                    return rowObject[0];//case single select
                };

                if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                    ctrl.onReady(api);
                }
            }
        }

        return directiveDefinitionObject;
    }

    app.directive('vrGenericdataBusinessentitydefinitionsettingGenericfielddefinition', GenericFieldDefinition);

})(app);
