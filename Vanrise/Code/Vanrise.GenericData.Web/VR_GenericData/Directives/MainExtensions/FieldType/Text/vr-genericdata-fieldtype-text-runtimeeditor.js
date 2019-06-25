'use strict';
app.directive('vrGenericdataFieldtypeTextRuntimeeditor', ['UtilsService', 'VR_GenericData_FieldTextTextTypeEnum', function (UtilsService, VR_GenericData_FieldTextTextTypeEnum) {

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
            };
        },
        template: function (element, attrs) {
            return getDirectiveTemplate(attrs);
        }
    };

    function textCtor(ctrl, $scope, $attrs) {

        this.initializeController = initializeController;

        var fieldName;
        var oldValue;
        var genericContext;

        function initializeController() {
            $scope.scopeModel.value;

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
            $scope.scopeModel.isAddButtonDisabled = true;

            $scope.scopeModel.addValue = function () {
                $scope.scopeModel.values.push($scope.scopeModel.value);
                $scope.scopeModel.value = undefined;
            };

            $scope.scopeModel.getImportedValues = function (importedValues) {
                for (var i = 0; i < importedValues.length; i++) {
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

        function defineScopeForSingleMode() {
            $scope.scopeModel.onFieldBlur = function () {
                if (oldValue == $scope.scopeModel.value)
                    return;

                oldValue = $scope.scopeModel.value;

                var valueAsArray = [$scope.scopeModel.value];

                if (genericContext != undefined && genericContext.notifyFieldValueChanged != undefined && typeof (genericContext.notifyFieldValueChanged) == "function") {
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
                    fieldName = payload.fieldName;
                    $scope.scopeModel.label = payload.fieldTitle;
                    fieldType = payload.fieldType;
                    if (fieldType != undefined) {
                        $scope.scopeModel.hint = fieldType.Hint;
                        switch (fieldType.TextType) {
                            case VR_GenericData_FieldTextTextTypeEnum.RichText.value: $scope.scopeModel.isRichText = true; break;
                            case VR_GenericData_FieldTextTextTypeEnum.MultipleText.value: $scope.scopeModel.isMultipleText = true; break;
                            default: $scope.scopeModel.isSingleText = true; break;
                        }
                    }
                    fieldValue = payload.fieldValue;
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

                return retVal;
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
            var hidelabel = "";

            if (attrs.hidelabel != undefined)
                hidelabel = " hidelabel ";

            return '<vr-columns ng-if="scopeModel.isRichText" colnum="{{runtimeEditorCtrl.normalColNum}}">'
                + ' <vr-editor ' + hidelabel+' label="{{scopeModel.label}}" value="scopeModel.value" isrequired="runtimeEditorCtrl.isrequired"></vr-editor>'
                + '</vr-columns>'
                + '<vr-columns  ng-if="scopeModel.isMultipleText"   colnum="{{runtimeEditorCtrl.normalColNum*2}}">'
                + '<vr-textarea ' + hidelabel+'  value="scopeModel.value" label="{{scopeModel.label}}" rows="4" isrequired ="runtimeEditorCtrl.isrequired" ></vr-textarea>'
                + '</vr-columns>'
                + '<vr-columns  ng-if="scopeModel.isSingleText"   colnum="{{runtimeEditorCtrl.normalColNum}}">'
                + '<vr-textbox ' + hidelabel+' type="text" label="{{scopeModel.label}}" hint="{{scopeModel.hint}}"  value="scopeModel.value" onblurtextbox="scopeModel.onFieldBlur" customvalidate="scopeModel.validateValue()" isrequired = "runtimeEditorCtrl.isrequired" ></vr - textbox > '
                + '</vr-columns>';

        }
    }

    return directiveDefinitionObject;
}]);

