'use strict';

app.directive('vrWhsBeDatarecordtypefieldsFormulaPointofinterconnectevaluator', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new pointofinterconnectEvaluatorFieldFormulaCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/DataRecordFieldFormulas/Templates/PointofinterconnectEvaluatorFieldFormulaTemplate.html';
            }
        };

        function pointofinterconnectEvaluatorFieldFormulaCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var context;

            function initializeController() {
                $scope.fields = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        $scope.fields.length = 0;

                        context = payload.context;
                        if (context != undefined && context.getFields != undefined)
                            $scope.fields = context.getFields();

                        if (payload.formula != undefined) {
                            $scope.selectedSwitchFieldName = UtilsService.getItemByVal($scope.fields, payload.formula.SwitchFieldName, "fieldName");
                            $scope.selectedTrunkFieldName = UtilsService.getItemByVal($scope.fields, payload.formula.TrunkFieldName, "fieldName");
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.DataRecordFieldFormulas.PointOfInterconnectEvaluatorFieldFormula, TOne.WhS.BusinessEntity.MainExtensions",
                        SwitchFieldName: $scope.selectedSwitchFieldName != undefined ? $scope.selectedSwitchFieldName.fieldName : undefined,
                        TrunkFieldName: $scope.selectedTrunkFieldName != undefined ? $scope.selectedTrunkFieldName.fieldName : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);