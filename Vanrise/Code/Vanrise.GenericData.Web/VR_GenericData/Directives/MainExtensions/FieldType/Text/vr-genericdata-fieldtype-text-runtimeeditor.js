'use strict';
app.directive('vrGenericdataFieldtypeTextRuntimeeditor', ['UtilsService', function (UtilsService) {

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

    function textCtor(ctrl, $scope, $attrs) {

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

            $scope.scopeModel.getImportedValues = function (importedValues) {
                for (var i = 0 ; i < importedValues.length ; i++) {
                    if ($scope.scopeModel.values.indexOf(importedValues[i]) == -1)
                        $scope.scopeModel.values.push(importedValues[i]);
                }
            };

            $scope.scopeModel.validateValue = function () {
                if ($scope.scopeModel.value == undefined || $scope.scopeModel.value == null || $scope.scopeModel.value == '') {
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

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var fieldType;
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

                return retVal;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
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
                        + '<vr-columns withemptyline haschildcolumns colnum="1">'
                            + '<vr-columns   colnum="6">'
                                + '<vr-button type="Add" data-onclick="scopeModel.addValue" standalone vr-disabled="scopeModel.isAddButtonDisabled" ></vr-button>'
                            + '</vr-columns>'
                            + '<vr-columns   colnum="6">'
                                    + '<vr-common-excelfileparser  fieldname="{{scopeModel.label}}" onokclicked="scopeModel.getImportedValues" standalone ></vr-common-excelfileparser>'
                            + '</vr-columns>'
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
                    + '<vr-textbox type="text" label="{{scopeModel.label}}" value="scopeModel.value" customvalidate="scopeModel.validateValue()" isrequired="runtimeEditorCtrl.isrequired"></vr-textbox>'
                + '</vr-columns>';
        }
    }

    return directiveDefinitionObject;
}]);

