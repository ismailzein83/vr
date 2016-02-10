'use strict';
app.directive('vrGenericdataNumericRuntimeeditor', ['UtilsService', function (UtilsService) {

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
        templateUrl: "/Client/Modules/VR_GenericData/Directives/RuntimeEditors/Templates/NumericEditorTemplate.html"
    };

    function textCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            $scope.scopeModel.values = [];

            $scope.scopeModel.showInMultipleMode = (ctrl.selectionmode == "dynamic" || ctrl.selectionmode == "multiple");

            $scope.scopeModel.isValid = function () {
                if ($scope.scopeModel.values != undefined && $scope.scopeModel.values.length > 0)
                    return null;
                return "You should add at least one choice."
            }

            $scope.scopeModel.disableAddButton = true;
            $scope.scopeModel.addValue = function () {
                $scope.scopeModel.values.push($scope.scopeModel.value);
                $scope.scopeModel.value = undefined;
                $scope.scopeModel.disableAddButton = true;
            }

            $scope.scopeModel.onValueChange = function (value) {
                $scope.scopeModel.disableAddButton = (value == undefined);
            }

            defineAPI();
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
                    angular.forEach(fieldValue.Values, function (val) {
                        $scope.scopeModel.values.push(val);
                    });
                }
            }

            api.getData = function () {
                var retVal;

                if (ctrl.selectionmode == "dynamic") {
                    retVal = {
                        $type: "Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions",
                        Values: $scope.scopeModel.values
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
    return directiveDefinitionObject;
}]);

