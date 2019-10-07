'use strict';

app.directive('vrGenericdataRequiredparentfieldRuntimeeditor', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                onselectionchanged: 'onselectionchanged'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RequiredParentFieldCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/BusinessEntity/Templates/RequiredParentFieldTemplate.html';
            }
        };

        function RequiredParentFieldCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var fieldAPI;
            var fieldReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var defaultFieldValue;
            var genericContext;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onFieldDirectiveReady = function (api) {
                    fieldAPI = api;
                    if (field.ReadOnly && fieldAPI.setOnlyViewMode != undefined && typeof (fieldAPI.setOnlyViewMode) == "function")
                        fieldAPI.setOnlyViewMode();
                    fieldReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promiseNode = {
                        promises: []
                    };

                    if (payload != undefined) {
                        genericContext = payload.genericContext;
                        var dataRecordTypeId = payload.dataRecordTypeId;
                        var fieldName = payload.fieldName;
                        var allFieldValuesByName = payload.allFieldValuesByName;
                        defaultFieldValue = payload.defaultFieldValue;
                        var useDefaultValue = genericContext != undefined && genericContext.isAddMode() && defaultFieldValue != undefined && (allFieldValuesByName == undefined || allFieldValuesByName[fieldName] == undefined);

                        $scope.scopeModel.columnNum = payload.fieldWidth != undefined ? payload.fieldWidth : 4;
                        $scope.scopeModel.hideLabel = payload.hideLabel ? "true" : "false";

                        var loadRuntimeFieldDirectivePromise = loadRuntimeFieldDirective();
                        promiseNode.promises.push(loadRuntimeFieldDirectivePromise);
                    }

                    function loadRuntimeFieldDirective() {
                        //if (currentContext != undefined && field.FieldType != undefined)
                        //    field.runTimeEditor = currentContext.getRuntimeEditor(field.FieldType.ConfigId);
                        var loadPromiseDeferred = UtilsService.createPromiseDeferred();

                        fieldReadyPromiseDeferred.promise.then(function () {
                            var fieldValue;
                            if (currentContext != undefined) {
                                fieldValue = currentContext.getFieldPathValue(field.FieldPath);
                            }

                            var payload = {
                                fieldName: field.FieldPath,
                                fieldTitle: field.FieldTitle,
                                fieldType: field.FieldType,
                                fieldValue: fieldValue,
                                fieldWidth: field.FieldWidth,
                                fieldViewSettings: field.FieldViewSettings,
                                isDisabled: field.IsDisabled,
                                readOnly: field.ReadOnly,
                                TextResourceKey: field.TextResourceKey,
                                showAsLabel: field.ShowAsLabel,
                                genericContext: genericContext,
                                allFieldValuesByName: allFieldValuesByName,
                                parentFieldValues: parentFieldValues,
                                //dataRecordTypeId: dataRecordTypeId
                            };
                            VRUIUtilsService.callDirectiveLoad(fieldAPI, payload, loadPromiseDeferred);
                        });

                        return loadPromiseDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode(promiseNode);
                };

                api.getData = function () {
                    return fieldAPI.getData();
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames) {
                    var _promises = [];
                    if (fieldAPI != undefined && fieldAPI.onFieldValueChanged != undefined && typeof (fieldAPI.onFieldValueChanged) == "function") {
                        var onFieldValueChangedPromise = fieldAPI.onFieldValueChanged(allFieldValuesByFieldNames);
                        if (onFieldValueChangedPromise != undefined)
                            _promises.push(onFieldValueChangedPromise);
                    }

                    return UtilsService.waitMultiplePromises(_promises);
                };

                api.setFieldValues = function (fieldValuesByNames) {
                    var _promises = [];

                    if (fieldAPI != undefined && fieldAPI.setFieldValues != undefined && typeof (fieldAPI.setFieldValues) == "function") {
                        var onFieldValueSettedPromise = fieldAPI.setFieldValues(fieldValuesByNames);
                        if (onFieldValueSettedPromise != undefined)
                            _promises.push(onFieldValueSettedPromise);
                    }

                    return UtilsService.waitMultiplePromises(_promises).then(function () {
                        if (useDefaultValue) {
                            genericContext.notifyFieldValueChanged({
                                fieldName: fieldName,
                                fieldValues: [defaultFieldValue]
                            });
                        }
                    });
                };

                api.setOnlyViewMode = function () {
                    if (fieldAPI != undefined && fieldAPI.setOnlyViewMode != undefined && typeof (fieldAPI.setOnlyViewMode) == "function") {
                        fieldAPI.setOnlyViewMode();
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }



            function getFieldTypeDescription(field) {
                var fieldValue = currentContext != undefined ? currentContext.getFieldPathValue(field.FieldPath) : undefined;
                return VR_GenericData_DataRecordFieldAPIService.GetFieldTypeDescription(field.FieldType, fieldValue).then(function (response) {
                    field.valueAsString = response;
                    field.valueAsString = response.FieldDescription;
                    field.styleDefinitionId = response.StyleDefinitionId;
                });
            }
        }

        return directiveDefinitionObject;
    }
]);