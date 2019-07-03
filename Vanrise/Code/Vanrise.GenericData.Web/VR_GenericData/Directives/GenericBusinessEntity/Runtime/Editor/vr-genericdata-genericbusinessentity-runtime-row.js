'use strict';

app.directive('vrGenericdataGenericbusinessentityRuntimeRow', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RowCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/Editor/Templates/RowTemplate.html';
            }
        };

        function RowCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var currentContext;
            var genericContext;
            var allFieldValuesByName;
            var parentFieldValues;
            //var dataRecordTypeId;

            function initializeController() {
                ctrl.fields = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload.fields != undefined) {
                        currentContext = payload.context;
                        genericContext = payload.genericContext;
                        allFieldValuesByName = payload.allFieldValuesByName;
                        parentFieldValues = payload.parentFieldValues;
                        //dataRecordTypeId = payload.dataRecordTypeId;

                        var promises = [];

                        for (var i = 0; i < payload.fields.length; i++) {
                            var field = payload.fields[i];
                            field.columnNum = field.FieldWidth != undefined ? field.FieldWidth : 4;
                            field.readyPromiseDeferred = UtilsService.createPromiseDeferred();
                            field.loadPromiseDeferred = UtilsService.createPromiseDeferred();
                            if (field.ShowAsLabel) {
                                promises.push(getFieldTypeDescription(field));
                            }
                            else {
                                promises.push(field.loadPromiseDeferred.promise);
                            }
                            prepareFieldObject(field);
                        }

                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    var fields = {};
                    for (var i = 0; i < ctrl.fields.length; i++) {
                        var field = ctrl.fields[i];
                        if (field.fieldAPI != undefined)
                            fields[field.FieldPath] = field.fieldAPI.getData();
                    }
                    return fields;
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames) {
                    var _promises = [];

                    for (var i = 0; i < ctrl.fields.length; i++) {
                        var field = ctrl.fields[i];
                        if (field.fieldAPI != undefined && field.fieldAPI.onFieldValueChanged != undefined && typeof (field.fieldAPI.onFieldValueChanged) == "function") {
                            var onFieldValueChangedPromise = field.fieldAPI.onFieldValueChanged(allFieldValuesByFieldNames);
                            if (onFieldValueChangedPromise != undefined)
                                _promises.push(onFieldValueChangedPromise);
                        }
                    }

                    return UtilsService.waitMultiplePromises(_promises);
                };

                api.setFieldValues = function (fieldValuesByNames) {
                    var _promises = [];

                    for (var i = 0; i < ctrl.fields.length; i++) {
                        var field = ctrl.fields[i];
                        if (field.fieldAPI != undefined && field.fieldAPI.setFieldValues != undefined && typeof (field.fieldAPI.setFieldValues) == "function") {
                            var onFieldValueSettedPromise = field.fieldAPI.setFieldValues(fieldValuesByNames);
                            if (onFieldValueSettedPromise != undefined)
                                _promises.push(onFieldValueSettedPromise);
                        }
                    }

                    return UtilsService.waitMultiplePromises(_promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api); 
            }
               
            function prepareFieldObject(field) {
                if (currentContext != undefined && field.FieldType != undefined)
                    field.runTimeEditor = currentContext.getRuntimeEditor(field.FieldType.ConfigId);

                field.onFieldDirectiveReady = function (api) {
                    field.fieldAPI = api;
                    field.readyPromiseDeferred.resolve();
                };


                if (field.readyPromiseDeferred != undefined) {

                    var useDefaultValue = genericContext.isAddMode() && field.DefaultFieldValue != undefined && allFieldValuesByName[field.FieldPath] == undefined;

                    field.readyPromiseDeferred.promise.then(function () {
                        field.readyPromiseDeferred = undefined;
                        var fieldValue;
                        if (currentContext != undefined)
                            fieldValue = currentContext.getFieldPathValue(field.FieldPath);
                        if (useDefaultValue)
                            fieldValue = field.DefaultFieldValue;

                        var payload = {
                            fieldName: field.FieldPath,
                            fieldTitle: field.FieldTitle,
                            fieldType: field.FieldType,
                            fieldValue: fieldValue,
                            fieldViewSettings: field.FieldViewSettings,
                            TextResourceKey: field.TextResourceKey,
                            showAsLabel: field.ShowAsLabel,
                            genericContext: genericContext,
                            allFieldValuesByName: allFieldValuesByName,
                            parentFieldValues: parentFieldValues,
                            //dataRecordTypeId: dataRecordTypeId
                        };
                        VRUIUtilsService.callDirectiveLoad(field.fieldAPI, payload, field.loadPromiseDeferred);
                    });

                    field.loadPromiseDeferred.promise.then(function () {
                        if (useDefaultValue) {
                            genericContext.notifyFieldValueChanged({
                                fieldName: field.FieldPath,
                                fieldValues: [field.DefaultFieldValue]
                            });
                        }
                    });
                }


                

                ctrl.fields.push(field);
            }

            function getFieldTypeDescription(field) {
                var fieldValue = currentContext != undefined ? currentContext.getFieldPathValue(field.FieldPath) : undefined;
                return VR_GenericData_DataRecordFieldAPIService.GetFieldTypeDescription(field.FieldType, fieldValue).then(function (response) {
                    field.valueAsString = response;
                });
            }
        }  

        return directiveDefinitionObject;
    }
]);