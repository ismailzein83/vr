'use strict';
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

    function booleanCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            $scope.scopeModel.value;
            
            if (ctrl.selectionmode != 'single') {
                defineScopeForMultiModes();
            }

            defineAPI();
        }

        function defineScopeForMultiModes() {
            $scope.scopeModel.values = [];

            $scope.scopeModel.addValue = function () {
                var dataItem = {
                    id: $scope.scopeModel.length + 1,
                    value: $scope.scopeModel.value
                };
                $scope.scopeModel.values.push(dataItem);
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
                            var dataItem = {
                                id: $scope.scopeModel.length + 1,
                                value: val
                            };
                            $scope.scopeModel.values.push(dataItem);
                        });
                    }
                    else if (ctrl.selectionmode == "multiple") {
                        for (var i = 0; i < fieldValue.length; i++) {
                            var dataItem = {
                                id: $scope.scopeModel.length + 1,
                                value: fieldValue[i]
                            };
                            $scope.scopeModel.values.push(dataItem);
                        }
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
            }

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
                            + '<vr-button type="Add" data-onclick="scopeModel.addValue" standalone></vr-button>'
                        + '</vr-columns>'
                    + '</vr-row>'
                    + '<vr-row>'
                        + '<vr-columns colnum="{{ctrl.normalColNum * 2}}">'
                            + '<vr-datalist maxitemsperrow="6" datasource="scopeModel.values" autoremoveitem="true">{{dataItem.value}}</vr-datalist>'
                        + '</vr-columns>'
                    + '</vr-row>'
                + '</vr-columns>';
        }

        function getSingleSelectionModeTemplate() {
            return '<vr-columns colnum="{{ctrl.normalColNum}}" ng-if="scopeModel.label != undefined ">'
                        + '<vr-label>{{scopeModel.label}}</vr-label>'
                        + '<vr-switch value="scopeModel.value"></vr-switch>'
                + '</vr-columns>';
        }
    }

    return directiveDefinitionObject;
}]);

