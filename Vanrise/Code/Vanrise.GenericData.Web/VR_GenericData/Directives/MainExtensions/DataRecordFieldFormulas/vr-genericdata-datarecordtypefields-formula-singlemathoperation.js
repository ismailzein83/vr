'use strict';

app.directive('vrGenericdataDatarecordtypefieldsFormulaSinglemathoperation', ['UtilsService', 'VR_GenericData_MathOperatorEnum',
    function (UtilsService, VR_GenericData_MathOperatorEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new singleMathOperationCtor(ctrl, $scope);
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
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataRecordFieldFormulas/Templates/SingleMathOperationFieldFormulaTemplate.html';
            }
        };

        function singleMathOperationCtor(ctrl, $scope) {

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.fields = [];

                $scope.scopeModel.valueCustomValidation = function () {

                    if ($scope.scopeModel.value != undefined && $scope.scopeModel.value == 0)
                        return "Value should be different than Zero!!";

                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    $scope.scopeModel.mathOperators = UtilsService.getArrayEnum(VR_GenericData_MathOperatorEnum);

                    if (payload != undefined) {
                        var context = payload.context;
                        if (context != undefined && context.getFields != undefined) {
                            $scope.scopeModel.fields = context.getFields();
                        }

                        if (payload.formula != undefined) {
                            $scope.scopeModel.selectedFieldName = UtilsService.getItemByVal($scope.scopeModel.fields, payload.formula.FieldName, "fieldName");
                            $scope.scopeModel.selectedMathOperator = UtilsService.getItemByVal($scope.scopeModel.mathOperators, payload.formula.MathOperator, "value");
                            $scope.scopeModel.value = payload.formula.Value;
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas.SingleMathOperationFieldFormula, Vanrise.GenericData.MainExtensions",
                        FieldName: $scope.scopeModel.selectedFieldName.fieldName,
                        MathOperator: $scope.scopeModel.selectedMathOperator.value,
                        Value: $scope.scopeModel.value
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);