'use strict';
app.directive('vrGenericdataDatarecordtypefieldsFormulaParentbusinessentity', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '=',
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataRecordFieldFormulas/Templates/ParentBusinessEntityFieldFormulaTemplate.html';
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
                        $scope.fields.length = 0;
                        context = payload.context;
                        if (context != undefined && context.getFields != undefined)
                            $scope.fields = context.getFields();
                        if (payload.formula != undefined) {
                            $scope.selectedFieldName = UtilsService.getItemByVal($scope.fields, payload.formula.ChildFieldName, "fieldName")
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas.ParentBusinessEntityFieldFormula, Vanrise.GenericData.MainExtensions",
                        ChildFieldName: $scope.selectedFieldName != undefined ? $scope.selectedFieldName.fieldName : undefined
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