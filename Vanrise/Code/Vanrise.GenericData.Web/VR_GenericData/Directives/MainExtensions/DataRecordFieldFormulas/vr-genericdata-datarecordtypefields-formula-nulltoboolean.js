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
                var ctor = new nullToBooleanCtor(ctrl, $scope);
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

        function nullToBooleanCtor(ctrl, $scope) {

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.fields = [];

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
                            $scope.scopeModel.selectedNullableFieldName = UtilsService.getItemByVal($scope.scopeModel.fields, payload.formula.NullableFieldName, "fieldName");
                            $scope.scopeModel.nullIsFalse = payload.formula.NullIsFalse;
                        }
                        else {
                            $scope.scopeModel.nullIsFalse = true;
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas.NullToBooleanFieldFormula, Vanrise.GenericData.MainExtensions",
                        NullableFieldName: $scope.scopeModel.selectedNullableFieldName.fieldName,
                        NullIsFalse: $scope.scopeModel.nullIsFalse
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