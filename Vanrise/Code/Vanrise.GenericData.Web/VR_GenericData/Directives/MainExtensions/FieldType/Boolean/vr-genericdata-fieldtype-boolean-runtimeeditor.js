﻿'use strict';

app.directive('vrGenericdataFieldtypeBooleanRuntimeeditor', ['UtilsService', function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            selectionmode: '@',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            $scope.scopeModel = {};

            var ctor = new booleanCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getDirectiveTemplate(attrs);
        }
    };

    function booleanCtor(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;

        var fieldName;
        var genericContext;

        function initializeController() {

            $scope.scopeModel.value =  false;
            
            if (ctrl.selectionmode != 'single') {
                defineScopeForMultiModes();
            }
            else {
                defineScopeForSingleMode();
            }

            defineAPI();
        }

        function defineScopeForMultiModes() {
            $scope.scopeModel.values = [];
            $scope.scopeModel.isAddButtonDisabled = false;

            $scope.scopeModel.addValue = function () {
                $scope.scopeModel.values.push($scope.scopeModel.value);
            };

            $scope.scopeModel.validateValue = function () {
                if ($scope.scopeModel.values.length == 2) {
                    $scope.scopeModel.isAddButtonDisabled = true;
                    return null;
                }
                if (UtilsService.contains($scope.scopeModel.values, $scope.scopeModel.value)) {
                    $scope.scopeModel.isAddButtonDisabled = true;
                    return 'Value already exists';
                }
                $scope.scopeModel.isAddButtonDisabled = false;
                return null;
            };
        }

        function defineScopeForSingleMode() {
            $scope.scopeModel.onSwitchChanged = function () {

                if (genericContext != undefined && genericContext.notifyFieldValueChanged != undefined && typeof (genericContext.notifyFieldValueChanged) == "function") {
                    var valueAsArray = [$scope.scopeModel.value];
                    var changedField = { fieldName: fieldName, fieldValues: valueAsArray };
                    genericContext.notifyFieldValueChanged(changedField);
                }
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                $scope.scopeModel.values = [];
                $scope.scopeModel.value = undefined;

                var fieldType;
                var fieldValue;

                if (payload != undefined) {
                    $scope.scopeModel.label = payload.fieldTitle;
                    fieldType = payload.fieldType;
                    fieldValue = payload.fieldValue;
                    fieldName = payload.fieldName;
                    genericContext = payload.genericContext;
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
                            Values: $scope.scopeModel.values
                        };
                    }
                }
                else if (ctrl.selectionmode == "multiple") {
                    if ($scope.scopeModel.values.length > 0) {
                        retVal = $scope.scopeModel.values;
                    }
                }
                else {
                    retVal = $scope.scopeModel.value;
                }

                return retVal != undefined ? retVal : false;
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

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function setFieldValue(fieldValue) {
            if (ctrl.selectionmode == "dynamic") {
                angular.forEach(fieldValue.Values, function (val) {
                    $scope.scopeModel.values.push(val);
                });
            }
            else if (ctrl.selectionmode == "multiple") {
                for (var i = 0; i < fieldValue.length; i++) {
                    $scope.scopeModel.values.push(fieldValue[i]);
                }
            }
            else {
                $scope.scopeModel.value = fieldValue;
            }
        }
    }

    function getDirectiveTemplate(attrs) {

        if (attrs.selectionmode == 'single') {
            return getSingleSelectionModeTemplate();
        }
        else {
            return '<vr-columns colnum="{{ctrl.normalColNum * 4}}">'
                    + '<vr-row>'
                        + getSingleSelectionModeTemplate()
                        + '<vr-columns withemptyline>'
                            + '<vr-button type="Add" data-onclick="scopeModel.addValue" standalone vr-disabled="scopeModel.isAddButtonDisabled"></vr-button>'
                        + '</vr-columns>'
                    + '</vr-row>'
                    + '<vr-row>'
                        + '<vr-columns colnum="{{ctrl.normalColNum * 2}}">'
                            + '<vr-datalist maxitemsperrow="6" datasource="scopeModel.values" autoremoveitem="true">{{dataItem}}</vr-datalist>'
                        + '</vr-columns>'
                    + '</vr-row>'
                + '</vr-columns>';
        }

        function getSingleSelectionModeTemplate() {

            var labelTemplate = '<vr-label ng-if="scopeModel.label != undefined ">{{scopeModel.label}}</vr-label>';
            if (attrs.hidelabel != undefined)
                labelTemplate = " ";

            return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
                + labelTemplate
                + '<vr-validator validate="scopeModel.validateValue()"><vr-switch value="scopeModel.value" onvaluechanged="scopeModel.onSwitchChanged"></vr-switch></vr-validator>'
                + '</vr-columns>';
        }
    }

    return directiveDefinitionObject;
}]);