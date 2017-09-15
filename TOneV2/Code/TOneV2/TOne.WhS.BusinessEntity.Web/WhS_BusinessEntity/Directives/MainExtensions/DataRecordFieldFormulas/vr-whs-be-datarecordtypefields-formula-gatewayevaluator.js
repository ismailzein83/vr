'use strict';

app.directive('vrWhsBeDatarecordtypefieldsFormulaGatewayevaluator', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new gatewayEvaluatorFieldFormulaCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/DataRecordFieldFormulas/Templates/GatewayEvaluatorFieldFormulaTemplate.html';
            }
        };

        function gatewayEvaluatorFieldFormulaCtor(ctrl, $scope) {
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
                            $scope.selectedPortFieldName = UtilsService.getItemByVal($scope.fields, payload.formula.PortFieldName, "fieldName");
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.DataRecordFieldFormulas.GatewayEvaluatorFieldFormula, TOne.WhS.BusinessEntity.MainExtensions",
                        SwitchFieldName: $scope.selectedSwitchFieldName != undefined ? $scope.selectedSwitchFieldName.fieldName : undefined,
                        PortFieldName: $scope.selectedPortFieldName != undefined ? $scope.selectedPortFieldName.fieldName : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);