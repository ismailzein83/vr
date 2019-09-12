﻿'use strict';

app.directive('vrGenericdataFieldtypeDatetimeRuntimeeditor', ['UtilsService', 'VR_GenericData_DateTimeDataTypeEnum', 'VR_GenericData_DateTimeDefaultValueEnum', 'VR_GenericData_DateTimeValidationOperatorEnum', 'VR_GenericData_DateTimeValidationTargetEnum', 'VRDateTimeService',
    function (UtilsService, VR_GenericData_DateTimeDataTypeEnum, VR_GenericData_DateTimeDefaultValueEnum, VR_GenericData_DateTimeValidationOperatorEnum, VR_GenericData_DateTimeValidationTargetEnum, VRDateTimeService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectionmode: '@',
                normalColNum: '@',
                isrequired: '=',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new dateTimeCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'runtimeEditorCtrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function dateTimeCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var fieldName;
            var genericContext;
            var validations;

            function initializeController() {
                $scope.scopeModel = {};

                if (ctrl.selectionmode != 'single') {
                    defineScopeForMultiModes();
                } else {
                    defineScopeForSingleMode();
                }

                if (ctrl.onReady != undefined) {
                    ctrl.onReady(getDirectiveAPI());
                }
            }

            function defineScopeForMultiModes() {
                $scope.scopeModel.values = [];
                $scope.scopeModel.isAddButtonDisabled = true;

                $scope.scopeModel.addValue = function () {
                    $scope.scopeModel.values.push(getDataItem($scope.scopeModel.value));
                    $scope.scopeModel.value = undefined;
                };

                $scope.scopeModel.validateValue = function () {

                    var error = customValidate();
                    if (error != undefined)
                        return error;

                    if ($scope.scopeModel.value == undefined || $scope.scopeModel.value == null || $scope.scopeModel.value == '') {
                        $scope.scopeModel.isAddButtonDisabled = true;
                        return null;
                    }
                    for (var i = 0; i < $scope.scopeModel.values.length; i++) {
                        if (UtilsService.areDateTimesEqual($scope.scopeModel.values[i].value, $scope.scopeModel.value)) {
                            $scope.scopeModel.isAddButtonDisabled = true;
                            return 'Value already exists';
                        }
                    }
                    $scope.scopeModel.isAddButtonDisabled = false;
                    return null;
                };
            }

            function defineScopeForSingleMode() {

                $scope.scopeModel.validateValue = function () {
                    var customValdiateResult = customValidate();
                    if (customValdiateResult != undefined)
                        return customValdiateResult;

                    return evaluateValidationsSetting();
                };

                $scope.scopeModel.onDateChanged = function () {

                    if (genericContext != undefined && genericContext.notifyFieldValueChanged != undefined && typeof (genericContext.notifyFieldValueChanged) == "function") {
                        var changedField = {
                            fieldName: fieldName, fieldValues: [dateFormat($scope.scopeModel.value, "yyyy-mm-dd'T'hh:MM:ss")]
                        };
                        genericContext.notifyFieldValueChanged(changedField);
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.value = undefined;
                    $scope.scopeModel.values = [];

                    var fieldType;
                    var fieldValue;
                    var fieldViewSettings;

                    if (payload != undefined) {
                        $scope.scopeModel.label = payload.fieldTitle;
                        fieldType = payload.fieldType;
                        fieldValue = payload.fieldValue;
                        fieldName = payload.fieldName;
                        var dataTypes = UtilsService.getArrayEnum(VR_GenericData_DateTimeDataTypeEnum);
                        $scope.scopeModel.fieldType = UtilsService.getItemByVal(dataTypes, fieldType.DataType, 'value');
                        genericContext = payload.genericContext;
                        fieldViewSettings = payload.fieldViewSettings;
                    }

                    if (fieldViewSettings != undefined) {
                        if (genericContext != undefined && genericContext.isAddMode()) {
                            $scope.scopeModel.value = getValueFromDefaultValueEnum(fieldViewSettings.DefaultValue);
                        }

                        validations = fieldViewSettings.Validations;
                    }

                    if (fieldValue != undefined) {
                        setFieldValue(fieldValue);
                    }
                };

                api.getData = function () {
                    var retVal;

                    if (ctrl.selectionmode == "dynamic") {
                        if ($scope.scopeModel.values.length > 0) {
                            retVal = {
                                $type: "Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions",
                                Values: UtilsService.getPropValuesFromArray($scope.scopeModel.values, 'value')
                            };
                        }
                    }
                    else if (ctrl.selectionmode == "multiple") {
                        if ($scope.scopeModel.values.length > 0) {
                            retVal = UtilsService.getPropValuesFromArray($scope.scopeModel.values, 'value');
                        }
                    }
                    else {
                        retVal = $scope.scopeModel.value;
                    }

                    return retVal;
                };

                api.setLabel = function (value) {
                    $scope.scopeModel.label = value;
                };
                api.setOnlyViewMode = function () {
                    UtilsService.setContextReadOnly($scope);
                };
                api.setFieldValues = function (fieldValuesByNames) {
                    if (fieldValuesByNames == undefined || !(fieldName in fieldValuesByNames))
                        return;

                    var fieldValue = fieldValuesByNames[fieldName];
                    if (fieldValue != undefined) {
                        setFieldValue(fieldValue);
                    }
                    else {
                        $scope.scopeModel.values.length = 0;
                        $scope.scopeModel.value = undefined;
                    }
                };

                return api;
            }

            function setFieldValue(fieldValue) {
                if (ctrl.selectionmode == "dynamic") {
                    angular.forEach(fieldValue.Values, function (val) {
                        $scope.scopeModel.values.push(getDataItem(val));
                    });
                }
                else if (ctrl.selectionmode == "multiple") {
                    for (var i = 0; i < fieldValue.length; i++) {
                        $scope.scopeModel.values.push(getDataItem(fieldValue[i]));
                    }
                }
                else {
                    if ($scope.scopeModel.fieldType.type != "time") {
                        $scope.scopeModel.value = fieldValue;


                    }
                    else {
                        $scope.scopeModel.value = fieldValue;
                    }
                }
            }

            function getDataItem(value) {
                return {
                    id: $scope.scopeModel.values.length + 1,
                    value: value,
                    displayValue: getDateTimeString(value)
                };

                function getDateTimeString(dateTime) {
                    var type = $scope.scopeModel.fieldType.type;
                    var dateTimeObject = new Date(dateTime);

                    switch (type) {
                        case 'dateTime':
                            return dateTimeObject.toString();
                        case 'date':
                            return dateTimeObject.toDateString();
                        case 'time':
                            return dateTime.Hour + ':' + dateTime.Minute;
                    }
                }
            }

            function customValidate() {
                if (ctrl.customvalidate != undefined && typeof (ctrl.customvalidate) == 'function')
                    return ctrl.customvalidate();

                return undefined;
            }

            function evaluateValidationsSetting() {
                if ($scope.scopeModel.value == undefined || validations == undefined || validations.length == 0)
                    return null;

                for (var i = 0; i < validations.length; i++) {
                    var currentValidation = validations[i];
                    var validationOperatorObject = UtilsService.getEnum(VR_GenericData_DateTimeValidationOperatorEnum, 'value', currentValidation.ValidationOperator);
                    var validationOperatorSyntax = validationOperatorObject.ifSyntax;
                    var validationOperatorDescription = validationOperatorObject.description;
                    var validationTarget = currentValidation.ValidationTarget;

                    var valueToCompare;
                    var fieldNameToCompare;

                    switch (validationTarget) {
                        case VR_GenericData_DateTimeValidationTargetEnum.Today.value: {
                            valueToCompare = VRDateTimeService.getTodayDate();
                            fieldNameToCompare = "Today";
                            break;
                        }

                        case VR_GenericData_DateTimeValidationTargetEnum.Now.value: {
                            valueToCompare = VRDateTimeService.getNowDateTime();
                            fieldNameToCompare = "Now";
                            break;
                        }

                        case VR_GenericData_DateTimeValidationTargetEnum.SpecificField.value: {
                            var fields = genericContext.getFieldValues();
                            var validationFieldName = currentValidation.FieldName;

                            if (fields == undefined || fields[validationFieldName] == undefined)
                                return null;

                            valueToCompare = fields[validationFieldName];
                            fieldNameToCompare = validationFieldName;
                            break;
                        }

                        default: return null;
                    }

                    var firstValue = $scope.scopeModel.value;
                    if (!(firstValue instanceof Date))
                        firstValue = UtilsService.createDateFromString(firstValue);

                    var secondValue = valueToCompare;
                    if (!(secondValue instanceof Date))
                        secondValue = UtilsService.createDateFromString(secondValue);

                    if (!eval(firstValue.getTime() + validationOperatorSyntax + secondValue.getTime())) {
                        return fieldName + " must be " + validationOperatorDescription + " " + fieldNameToCompare;
                    }

                    return null;
                }

                return null;
            }

            function getValueFromDefaultValueEnum(defaultValueEnum) {
                if (ctrl.selectionmode != 'single' || defaultValueEnum == undefined)
                    return undefined;

                switch (defaultValueEnum) {
                    case VR_GenericData_DateTimeDefaultValueEnum.Today.value: return VRDateTimeService.getTodayDate();
                    case VR_GenericData_DateTimeDefaultValueEnum.Now.value: return VRDateTimeService.getNowDateTime();
                    default: return undefined;
                }
            }
        }

        function getDirectiveTemplate(attrs) {
            if (attrs.selectionmode == 'single') {
                return getSingleSelectionModeTemplate();
            }
            else {
                return '<vr-columns colnum="{{runtimeEditorCtrl.normalColNum * 4}}">'
                    + '<vr-row>'
                    + getSingleSelectionModeTemplate()
                    + '<vr-columns withemptyline>'
                    + '<vr-button type="Add" data-onclick="scopeModel.addValue" standalone vr-disabled="scopeModel.isAddButtonDisabled"></vr-button>'
                    + '</vr-columns>'
                    + '</vr-row>'
                    + '<vr-row>'
                    + '<vr-columns colnum="{{runtimeEditorCtrl.normalColNum * 2}}">'
                    + '<vr-datalist maxitemsperrow="6" datasource="scopeModel.values" autoremoveitem="true">{{dataItem.displayValue}}</vr-datalist>'
                    + '</vr-columns>'
                    + '</vr-row>'
                    + '</vr-columns>';
            }

            function getSingleSelectionModeTemplate() {

                var hidelabel = "";
                var labelTemplate = "<vr-label>{{scopeModel.label}}</vr-label>";
                if (attrs.hidelabel != undefined) {
                    var hideLabelValue = attrs.hidelabel;
                    if (hideLabelValue != "false") {
                        hidelabel = " hidelabel ";
                        labelTemplate = "";
                    }
                }
                return '<vr-columns colnum="{{runtimeEditorCtrl.normalColNum}}" ng-if="scopeModel.fieldType != undefined && scopeModel.label != undefined ">'
                    + ' ' + labelTemplate + ' '
                    + '<vr-directivewrapper ' + hidelabel + ' directive="\'vr-datetimepicker\'" type="{{scopeModel.fieldType.type}}" value="scopeModel.value" customvalidate="scopeModel.validateValue()" onvaluechanged="scopeModel.onDateChanged()" isrequired="runtimeEditorCtrl.isrequired"></vr-directivewrapper>'
                    + '</vr-columns>';
            }
        }

        return directiveDefinitionObject;
    }]);