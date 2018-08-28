'use strict';
app.directive('vrGenericdataDatarecordtypefieldsFormulaEncryptedcalculated', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new EncryptedCalculatedCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataRecordFieldFormulas/Templates/EncryptedCalculatedFieldFormulaTemplate.html';
            }
        };

        function EncryptedCalculatedCtor(ctrl, $scope) {

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
                            $scope.scopeModel.selectedFieldName = UtilsService.getItemByVal($scope.scopeModel.fields, payload.formula.FieldName, "fieldName");
                            $scope.scopeModel.decrypt = payload.formula.Decrypt;
                        }
                        else {
                            $scope.scopeModel.decrypt = false;
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas.EncryptedCalculatedFieldFormula, Vanrise.GenericData.MainExtensions",
                        Decrypt: $scope.scopeModel.decrypt,
                        FieldName: $scope.scopeModel.selectedFieldName.fieldName
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