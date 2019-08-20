(function (app) {

    'use strict';

    GenericFieldsEditorRuntimeDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService', 'VR_GenericData_GenericUIRuntimeAPIService'];

    function GenericFieldsEditorRuntimeDirective(UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_GenericUIRuntimeAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericFieldsEditorRuntimeCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/GenericFields/Templates/GenericFieldsEditorRuntimeSettingTemplate.html'
        };

        function GenericFieldsEditorRuntimeCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var selectedValues;
            var runtimeFields;

            var rowDirectiveAPI;
            var rowDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRowDirectiveReady = function (api) {
                    rowDirectiveAPI = api;
                    rowDirectiveReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var fields;
                    var allFieldValuesByName;
                    var definitionSettings;
                    var historyId;
                    var parentFieldValues;
                    var genericContext;
                    var dataRecordTypeId;

                    if (payload != undefined) {
                        allFieldValuesByName = payload.allFieldValuesByName;
                        definitionSettings = payload.definitionSettings;
                        historyId = payload.historyId;
                        selectedValues = payload.selectedValues;
                        parentFieldValues = payload.parentFieldValues;
                        genericContext = payload.genericContext;
                        dataRecordTypeId = payload.dataRecordTypeId;
                    }

                    var loadRunTimeFieldTypeTemplatesPromise = loadRunTimeFieldTypeTemplates();
                    promises.push(loadRunTimeFieldTypeTemplatesPromise);

                    var getGenericFieldsRuntimeRowPromise = getGenericFieldsRuntimeRow();
                    promises.push(getGenericFieldsRuntimeRowPromise);

                    if (definitionSettings != undefined) {
                        if (definitionSettings.Fields != undefined) {
                            fields = definitionSettings.Fields;
                            var rowDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(rowDirectiveLoadPromiseDeferred.promise);

                            UtilsService.waitMultiplePromises([loadRunTimeFieldTypeTemplatesPromise, getGenericFieldsRuntimeRowPromise]).then(function () {
                                loadRowDirective().then(function () {
                                    rowDirectiveLoadPromiseDeferred.resolve();
                                });
                            });
                        }
                    }

                    function loadRunTimeFieldTypeTemplates() {
                        return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                            if (response) {
                                $scope.fieldTypeConfigs = response;
                            }
                        });
                    }

                    function getGenericFieldsRuntimeRow() {
                        var input = {
                            Fields: definitionSettings != undefined ? definitionSettings.Fields : definitionSettings,
                            DataRecordTypeId: dataRecordTypeId,
                            RecordFields: genericContext != undefined && dataRecordTypeId == undefined ? genericContext.getRecordFields() : undefined
                        };

                        return VR_GenericData_GenericUIRuntimeAPIService.GetGenericFieldsRuntimeRow(input).then(function (response) {
                            if (parentFieldValues != undefined && response != undefined && response.Fields != undefined) {
                                var extendedFields = [];
                                for (var i = 0; i < response.Fields.length; i++) {
                                    var currentField = response.Fields[i];

                                    var parentFieldValue = parentFieldValues[currentField.FieldPath];
                                    if (parentFieldValue == undefined || !parentFieldValue.isHidden) // ***** Here We hide ParentField *****
                                        extendedFields.push(currentField);
                                }
                                response.Fields = extendedFields;
                            }

                            runtimeFields = response ? response.Fields : undefined;
                        });
                    }


                    function loadRowDirective() {
                        var rowDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        rowDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payload = {
                                fields: runtimeFields,
                                context: getContext(),
                                genericContext: genericContext,
                                historyId: historyId,
                                allFieldValuesByName: allFieldValuesByName,
                                parentFieldValues: parentFieldValues,
                                //dataRecordTypeId: dataRecordTypeId
                            };

                            VRUIUtilsService.callDirectiveLoad(rowDirectiveAPI, payload, rowDirectiveLoadPromiseDeferred);
                        });

                        return rowDirectiveLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return rowDirectiveAPI != undefined ? rowDirectiveAPI.getData() : undefined;
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames) {
                    var _promises = [];

                    if (rowDirectiveAPI != undefined && rowDirectiveAPI.onFieldValueChanged != undefined && typeof (rowDirectiveAPI.onFieldValueChanged) == "function") {
                        var onFieldValueChangedPromise = rowDirectiveAPI.onFieldValueChanged(allFieldValuesByFieldNames);
                        if (onFieldValueChangedPromise != undefined)
                            _promises.push(onFieldValueChangedPromise);
                    }

                    return UtilsService.waitMultiplePromises(_promises);
                };

                api.setData = function (dicData) {
                    if (rowDirectiveAPI != undefined) {
                        dicData = UtilsService.mergeObject(dicData, rowDirectiveAPI.getData(), true);
                    }
                };

                api.setFieldValues = function (fieldValuesByNames) {
                    var _promises = [];

                    if (rowDirectiveAPI.setFieldValues != undefined && typeof (rowDirectiveAPI.setFieldValues) == "function") {
                        var onFieldValueSettedPromise = rowDirectiveAPI.setFieldValues(fieldValuesByNames);
                        if (onFieldValueSettedPromise != undefined)
                            _promises.push(onFieldValueSettedPromise);
                    }

                    return UtilsService.waitMultiplePromises(_promises);
                };



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


        }
    }

    app.directive('vrGenericdataGenericfieldseditorsettingRuntime', GenericFieldsEditorRuntimeDirective);

})(app);