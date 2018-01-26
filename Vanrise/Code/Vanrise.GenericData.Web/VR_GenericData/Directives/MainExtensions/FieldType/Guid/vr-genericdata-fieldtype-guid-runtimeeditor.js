'use strict';
app.directive('vrGenericdataFieldtypeGuidRuntimeeditor', ['UtilsService', function (UtilsService) {

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

            var ctor = new guidCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
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

    function guidCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            $scope.scopeModel.value =  false;
            
            if (ctrl.selectionmode != 'single') {
                defineScopeForMultiModes();
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
                            $scope.scopeModel.values.push(value);
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
            return '<vr-columns colnum="{{ctrl.normalColNum}}" ng-if="scopeModel.label != undefined ">'
                        + '<vr-label>{{scopeModel.label}}</vr-label>'
                        + '<vr-validator validate="scopeModel.validateValue()"><vr-textbox value="scopeModel.value"></vr-textbox></vr-validator>'
                + '</vr-columns>';
        }
    }

    return directiveDefinitionObject;
}]);

