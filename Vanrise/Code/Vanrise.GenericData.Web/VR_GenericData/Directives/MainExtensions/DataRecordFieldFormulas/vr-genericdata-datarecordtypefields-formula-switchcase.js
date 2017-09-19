'use strict';

app.directive('vrGenericdataDatarecordtypefieldsFormulaSwitchcase', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new switchCaseCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataRecordFieldFormulas/Templates/SwitchCaseFieldFormulaTemplate.html';
            }
        };

        function switchCaseCtor(ctrl, $scope) {

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
                            $scope.scopeModel.selectedTargetFieldName = UtilsService.getItemByVal($scope.scopeModel.fields, payload.formula.TargetFieldName, "fieldName");
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas.SwitchCaseFieldFormula, Vanrise.GenericData.MainExtensions",
                        TargetFieldName: $scope.scopeModel.selectedTargetFieldName.fieldName
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