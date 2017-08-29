'use strict';

app.directive('vrGenericdataDatarecordtypefieldsFormulaDatetimeround', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new dateTimeRoundCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataRecordFieldFormulas/Templates/DateTimeRoundFieldFormulaTemplate.html';
            }
        };

        function dateTimeRoundCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var context;

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
                            $scope.scopeModel.selectedDateTimeFieldName = UtilsService.getItemByVal($scope.scopeModel.fields, payload.formula.DateTimeFieldName, "fieldName");
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas.DateTimeRoundFieldFormula, Vanrise.GenericData.MainExtensions",
                        DateTimeFieldName: $scope.scopeModel.selectedDateTimeFieldName.fieldName
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }
        return directiveDefinitionObject;
    }
]);