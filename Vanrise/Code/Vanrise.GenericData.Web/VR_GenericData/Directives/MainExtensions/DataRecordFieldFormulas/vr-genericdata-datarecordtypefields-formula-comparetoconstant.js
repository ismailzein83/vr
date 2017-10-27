'use strict';

app.directive('vrGenericdataDatarecordtypefieldsFormulaComparetoconstant', ['UtilsService', 'VR_GenericData_CompareToConstantOperatorEnum',
    function (UtilsService, VR_GenericData_CompareToConstantOperatorEnum) {

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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataRecordFieldFormulas/Templates/CompareToConstantFieldFormulaTemplate.html';
            }
        };

        function singleMathOperationCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.fields = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    $scope.scopeModel.compareToConstantOperators = UtilsService.getArrayEnum(VR_GenericData_CompareToConstantOperatorEnum);

                    if (payload != undefined) {
                        var context = payload.context;
                        if (context != undefined && context.getFields != undefined) {
                            $scope.scopeModel.fields = context.getFields();
                        }

                        if (payload.formula != undefined) {
                            $scope.scopeModel.selectedFieldName = UtilsService.getItemByVal($scope.scopeModel.fields, payload.formula.FieldName, "fieldName");
                            $scope.scopeModel.selectedCompareToConstanOperator = UtilsService.getItemByVal($scope.scopeModel.compareToConstantOperators, payload.formula.Operator, "value");
                            $scope.scopeModel.value = payload.formula.Value;
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas.CompareToConstantFieldFormula, Vanrise.GenericData.MainExtensions",
                        FieldName: $scope.scopeModel.selectedFieldName.fieldName,
                        Operator: $scope.scopeModel.selectedCompareToConstanOperator.value,
                        Value: $scope.scopeModel.value
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
]);