"use strict";

app.directive("vrGenericdataGenericeditorsettingRuntime", ["UtilsService", "VRUIUtilsService", "VR_GenericData_DataRecordFieldAPIService", "VR_GenericData_GenericUIRuntimeAPIService",
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_GenericUIRuntimeAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericEditorDefinitionSetting($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/GenericEditorRuntimeSettingTemplate.html"
        };

        function GenericEditorDefinitionSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectedValues;
            var allFieldValuesByName;
            var runtimeRows;
            var definitionSettings;
            var dataRecordTypeId;
            var historyId;
            var parentFieldValues;
            var genericContext;

            var sectionDirectiveApi;
            var sectionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSectionDirectiveReady = function (api) {
                    sectionDirectiveApi = api;
                    sectionDirectivePromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        allFieldValuesByName = payload.allFieldValuesByName;
                        definitionSettings = payload.definitionSettings;
                        dataRecordTypeId = payload.dataRecordTypeId;
                        historyId = payload.historyId;
                        parentFieldValues = payload.parentFieldValues;
                        genericContext = payload.genericContext;
                    }

                    var editorpromise = UtilsService.createPromiseDeferred();
                    promises.push(editorpromise.promise);

                    UtilsService.waitMultipleAsyncOperations([getGenericEditorRuntimeRows, loadRunTimeFieldTypeTemplates]).then(function () {
                        loadBusinessEntityDefinitionSelector().then(function () {
                            editorpromise.resolve();
                        }).catch(function (error) {
                            editorpromise.reject(error);
                        });
                    }).catch(function (error) {
                        editorpromise.reject(error);
                    });

                    function loadBusinessEntityDefinitionSelector() {
                        var sectionDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        sectionDirectivePromiseDeferred.promise.then(function () {
                            var payloadSelector = {
                                rows: runtimeRows,
                                context: getContext(),
                                allFieldValuesByName: allFieldValuesByName,
                                genericContext: genericContext,
                                parentFieldValues: parentFieldValues
                            };
                            VRUIUtilsService.callDirectiveLoad(sectionDirectiveApi, payloadSelector, sectionDirectiveLoadDeferred);
                        });

                        return sectionDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function (dicData) {

                    var sectionData = sectionDirectiveApi.getData();
                    if (sectionData != undefined) {
                        for (var prop in sectionData) {
                            dicData[prop] = sectionData[prop];
                        }
                    }
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames) {
                    var _promises = [];

                    if (sectionDirectiveApi.onFieldValueChanged != undefined && typeof (sectionDirectiveApi.onFieldValueChanged) == "function") {
                        var onFieldValueChangedPromise = sectionDirectiveApi.onFieldValueChanged(allFieldValuesByFieldNames);
                        if (onFieldValueChangedPromise != undefined)
                            _promises.push(onFieldValueChangedPromise);
                    }

                    return UtilsService.waitMultiplePromises(_promises);
                };

                //api.setFieldValues = function (fieldValuesByNames) {
                //    var _promises = [];
                   
                //    if (sectionDirectiveApi.setFieldValues != undefined && typeof (sectionDirectiveApi.setFieldValues) == "function") {
                //        var setFieldValuesPromise = sectionDirectiveApi.setFieldValues(fieldValuesByNames);
                //        if (setFieldValuesPromise != undefined)
                //            _promises.push(setFieldValuesPromise);
                //    }

                //    return UtilsService.waitMultiplePromises(_promises);
                //};

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function getContext() {
                var context = {
                    getRuntimeEditor: getRuntimeEditor,
                    getFieldPathValue: getFieldPathValue
                };
                return context;
            }

            function getRuntimeEditor(configId) {
                if ($scope.fieldTypeConfigs != undefined) {
                    var dataRecordFieldTypeConfig = UtilsService.getItemByVal($scope.fieldTypeConfigs, configId, 'ExtensionConfigurationId');
                    if (dataRecordFieldTypeConfig != undefined)
                        return dataRecordFieldTypeConfig.RuntimeEditor;
                }
            }

            function getFieldPathValue(fieldPath) {
                if (selectedValues != undefined && fieldPath != undefined)
                    return selectedValues[fieldPath];
            }

            function loadRunTimeFieldTypeTemplates() {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                    if (response) {
                        $scope.fieldTypeConfigs = response;
                    }
                });
            }

            function extendGenericEditorRuntimeRowFields(fields) {
                var extendedFields = [];

                for (var j = 0; j < fields.length; j++) {
                    var currentField = fields[j];
                    if (parentFieldValues == undefined) {
                        extendedFields.push(currentField);
                        continue;
                    }

                    var parentFieldValue = parentFieldValues[currentField.FieldPath];
                    //if (parentFieldValue == undefined || !parentFieldValue.isHidden) // ***** Here We hide ParentField *****
                    extendedFields.push(currentField);
                }

                return extendedFields;
            }

            function getGenericEditorRuntimeRows() {
                var input = {
                    Rows: definitionSettings != undefined ? definitionSettings.Rows : definitionSettings,
                    DataRecordTypeId: dataRecordTypeId
                };
                return VR_GenericData_GenericUIRuntimeAPIService.GetGenericEditorRuntimeRows(input).then(function (response) {
                    if (parentFieldValues != undefined && response != undefined && response.length > 0) {
                        for (var i = 0; i < response.length; i++) {
                            var currentRow = response[i];
                            if (currentRow.Fields != undefined && currentRow.Fields.length > 0)
                                currentRow.Fields = extendGenericEditorRuntimeRowFields(currentRow.Fields);
                        }
                    }
                    runtimeRows = response;
                });
            }
        }

        return directiveDefinitionObject;
    }
]);