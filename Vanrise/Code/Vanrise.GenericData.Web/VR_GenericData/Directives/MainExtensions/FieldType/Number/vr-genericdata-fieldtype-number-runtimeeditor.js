'use strict';

app.directive('vrGenericdataFieldtypeNumberRuntimeeditor', ['UtilsService',
    function (UtilsService) {

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
                var ctor = new numberCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'runtimeEditorCtrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function numberCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var fieldType;
            var fieldName;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.value;

                if (ctrl.selectionmode != 'single') {
                    defineScopeForMultiModes();
                }

                $scope.scopeModel.validateValue = function () {

                    var error = customValidate();
                    if (error != undefined) {
                        return error;
                    }

                    if ($scope.scopeModel.value == undefined || $scope.scopeModel.value == null || $scope.scopeModel.value == '') {
                        $scope.scopeModel.isAddButtonDisabled = true;
                        return null;
                    }

                    var valuesAsNumber = getValuesAsNumber($scope.scopeModel.values);
                    if (valuesAsNumber != undefined && UtilsService.contains(valuesAsNumber, getValueAsNumber($scope.scopeModel.value))) {
                        $scope.scopeModel.isAddButtonDisabled = true;
                        return 'Value already exists';
                    }

                    $scope.scopeModel.isAddButtonDisabled = false;
                    return null;
                };

                defineAPI();
            }

            function defineScopeForMultiModes() {
                $scope.scopeModel.values = [];
                $scope.scopeModel.isAddButtonDisabled = true;

                $scope.scopeModel.addValue = function () {
                    $scope.scopeModel.values.push($scope.scopeModel.value);
                    $scope.scopeModel.value = undefined;
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.values = [];
                    $scope.scopeModel.value = undefined;

                    var fieldValue;

                    if (payload != undefined) {
                        $scope.scopeModel.label = payload.fieldTitle;
                        fieldName = payload.fieldName;
                        fieldType = payload.fieldType;
                        fieldValue = payload.fieldValue;
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

                api.setLabel = function (value) {
                    $scope.scopeModel.label = value;
                };

                api.setFieldValues = function (fieldValuesByNames) {
                    if (!(fieldName in fieldValuesByNames))
                        return;

                    var fieldValue = fieldValuesByNames[fieldName];
                    if (fieldValue == undefined) {
                        $scope.scopeModel.values.length = 0;
                        $scope.scopeModel.value = undefined;
                        return;
                    }

                    setFieldValue(fieldValue);
                };

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

            function customValidate() {
                if (ctrl.customvalidate != undefined && typeof (ctrl.customvalidate) == 'function')
                    return ctrl.customvalidate();
                return undefined;
            }
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