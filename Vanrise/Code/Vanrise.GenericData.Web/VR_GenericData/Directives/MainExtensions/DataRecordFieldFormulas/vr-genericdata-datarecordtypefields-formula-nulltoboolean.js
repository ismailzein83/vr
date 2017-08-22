'use strict';

app.directive('vrGenericdataDatarecordtypefieldsFormulaNulltoboolean', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new textTypeCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataRecordFieldFormulas/Templates/NullToBooleanFieldFormulaTemplate.html';
            }

        };

        function textTypeCtor(ctrl, $scope) {

            var context;

            function initializeController() {
                $scope.fields = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        var context = payload.context;
                        if (context != undefined && context.getFields != undefined) {
                            $scope.scopeModel.fields = context.getFields();
                        }

                        if (payload.formula != undefined) {
                            $scope.scopeModel.selectedNullableFieldFieldName = UtilsService.getItemByVal($scope.scopeModel.fields, payload.formula.NullableFieldFieldName, "fieldName");
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas.NullToBooleanFieldFormula, Vanrise.GenericData.MainExtensions",
                        NullableFieldFieldName: $scope.selectedFieldName != undefined ? $scope.selectedFieldName.fieldName : undefined
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