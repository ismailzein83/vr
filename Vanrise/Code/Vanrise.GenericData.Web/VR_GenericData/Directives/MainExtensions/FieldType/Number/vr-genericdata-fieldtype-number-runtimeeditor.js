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
            var ctor = new textCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
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

    function textCtor(ctrl, $scope, $attrs) {
        var fieldType;
        function initializeController() {

            $scope.scopeModel.values = [];

            $scope.scopeModel.showInMultipleMode = (ctrl.selectionmode == "dynamic" || ctrl.selectionmode == "multiple");

            $scope.scopeModel.disableAddButton = true;
            $scope.scopeModel.addValue = function () {
                $scope.scopeModel.values.push($scope.scopeModel.value);
                $scope.scopeModel.value = undefined;
                $scope.scopeModel.disableAddButton = true;
            }

            $scope.scopeModel.onValueChange = function (value) {
                $scope.scopeModel.disableAddButton = (value == undefined);
            }

            $scope.scopeModel.validateValue = function () {
                for (var i = 0; i < $scope.scopeModel.values.length; i++) {
                    if ($scope.scopeModel.value == $scope.scopeModel.values[i]) {
                        $scope.scopeModel.disableAddButton = true;
                        return 'Value already exists';
                    }
                }
                $scope.scopeModel.disableAddButton = false;
                return null;
            };

            defineAPI();
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
                    else {
                        $scope.scopeModel.value = fieldValue;
                    }
                }
            }

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
                else {
                    retVal = getValueAsNumber( $scope.scopeModel.value);
                }

                return retVal;
            }

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
            return '<vr-columns colnum="{{ctrl.normalColNum * 4}}">'
                    + '<vr-row>'
                        + getSingleSelectionModeTemplate()
                        + '<vr-columns withemptyline>'
                            + '<vr-button type="Add" data-onclick="scopeModel.addValue" standalone vr-disabled="scopeModel.disableAddButton"></vr-button>'
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
            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                    + '<vr-textbox type="number" label="{{scopeModel.label}}" value="scopeModel.value" onvaluechanged="scopeModel.onValueChange" customvalidate="scopeModel.validateValue()" isrequired="ctrl.isrequired"></vr-textbox>'
                + '</vr-columns>';
        }
    }

    return directiveDefinitionObject;
}]);

