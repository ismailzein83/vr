'use strict';
app.directive('vrGenericdataFieldtypeNumberRuntimeeditor', ['UtilsService', 'VR_GenericData_FieldNumberDataTypeEnum', function (UtilsService, VR_GenericData_FieldNumberDataTypeEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            selectionmode: '@',
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            $scope.scopeModel = {};
            var ctor = new numberCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'runtimeEditorCtrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        template: function (element, attrs) {
            return getDirectiveTemplate(attrs);
        }
    };

    function numberCtor(ctrl, $scope, $attrs) {
        var fieldType;
        function initializeController() {
            $scope.scopeModel.value;

            if (ctrl.selectionmode != 'single') {
                defineScopeForMultiModes();
            }

            defineAPI();
        }

        function defineScopeForMultiModes() {
            $scope.scopeModel.values = [];
            $scope.scopeModel.isAddButtonDisabled = true;

            $scope.scopeModel.addValue = function () {
                $scope.scopeModel.values.push($scope.scopeModel.value);
                $scope.scopeModel.value = undefined;
            };

            $scope.scopeModel.validateValue = function () {
                if ($scope.scopeModel.value == undefined || $scope.scopeModel.value == null || $scope.scopeModel.value == '') {
                    $scope.scopeModel.isAddButtonDisabled = true;
                    return null;
                }
                if (UtilsService.contains(getValuesAsNumber($scope.scopeModel.values), getValueAsNumber($scope.scopeModel.value))) {                   
                    $scope.scopeModel.isAddButtonDisabled = true;
                    return 'Value already exists';
                }
                $scope.scopeModel.isAddButtonDisabled = false;
                return null;
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var fieldValue;

                if (payload != undefined) {
                    $scope.scopeModel.label = payload.fieldTitle;
                    fieldType = payload.fieldType;
                    fieldValue = payload.fieldValue;
                }

                if (fieldValue != undefined) {
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
            };

            api.getData = function () {
                var retVal;

                if (ctrl.selectionmode == "dynamic") {
                    if ($scope.scopeModel.values.length > 0) {
                        retVal = {
                            $type: "Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions",
                            Values: getValuesAsNumber($scope.scopeModel.values)
                        };
                    }
                }
                else if (ctrl.selectionmode == 'multiple') {
                    if ($scope.scopeModel.values.length > 0) {
                        retVal = getValuesAsNumber($scope.scopeModel.values);
                    }
                }
                else {
                    retVal = getValueAsNumber($scope.scopeModel.value);
                }

                return retVal;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getValuesAsNumber(valuesAsString) {
            if (valuesAsString != undefined) {
                var values = [];
                for (var i = 0; i < valuesAsString.length; i++) {
                    values.push(getValueAsNumber(valuesAsString[i]));
                }
                return values;
            }
            return null;
        }

        function getValueAsNumber(valueAsString) {
            if (valueAsString != undefined)
                return parseFloat(valueAsString);
            else
                return null;
        }

        this.initializeController = initializeController;

    }

    function getDirectiveTemplate(attrs) {

        if (attrs.selectionmode == 'single') {
            return getSingleSelectionModeTemplate();
        }
        else {
            return '<vr-columns colnum="12" haschildcolumns>'
                    //+ '<vr-row>'
                        + getSingleSelectionModeTemplate()
                        + '<vr-columns withemptyline>'
                            + '<vr-button type="Add" data-onclick="scopeModel.addValue" standalone vr-disabled="scopeModel.isAddButtonDisabled"></vr-button>'
                        + '</vr-columns>'
                    //+ '</vr-row>'
                    //+ '<vr-row>'
                        + '<vr-columns colnum="12">'
                            + '<vr-datalist maxitemsperrow="4" datasource="scopeModel.values" autoremoveitem="true">{{dataItem}}</vr-datalist>'
                        + '</vr-columns>'
                    //+ '</vr-row>'
                + '</vr-columns>';
        }

        function getSingleSelectionModeTemplate() {
            return '<vr-columns colnum="{{runtimeEditorCtrl.normalColNum}}">'
                    + '<vr-textbox type="number" label="{{scopeModel.label}}" value="scopeModel.value" customvalidate="scopeModel.validateValue()" isrequired="runtimeEditorCtrl.isrequired"></vr-textbox>'
                + '</vr-columns>';
        }
    }

    return directiveDefinitionObject;
}]);

