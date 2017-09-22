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
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.fields = [];
                $scope.scopeModel.switchCaseMappings = [];

                $scope.scopeModel.onAddSwitchCaseMapping = function (selectedItem) {

                    $scope.scopeModel.switchCaseMappings.push({
                        fields: $scope.scopeModel.fields,
                        targetFieldValue: undefined,
                        selectedMappingField: undefined
                    });
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.switchCaseMappings, deletedItem.targetFieldValue, 'targetFieldValue');
                    $scope.scopeModel.switchCaseMappings.splice(index, 1);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var switchCaseMappings;

                    if (payload != undefined) {
                        var context = payload.context;
                        if (context != undefined && context.getFields != undefined) {
                            $scope.scopeModel.fields = context.getFields();
                        }

                        if (payload.formula != undefined) {
                            $scope.scopeModel.selectedTargetFieldName = UtilsService.getItemByVal($scope.scopeModel.fields, payload.formula.TargetFieldName, "fieldName");
                            switchCaseMappings = payload.formula.SwitchCaseMappings;
                        }
                    }

                    for (var key in switchCaseMappings) {
                        if (key == '$type')
                            continue;

                        var switchCaseMapping = switchCaseMappings[key];

                        $scope.scopeModel.switchCaseMappings.push({
                            fields: $scope.scopeModel.fields,
                            targetFieldValue: key,
                            selectedMappingField: UtilsService.getItemByVal($scope.scopeModel.fields, switchCaseMapping.MappingFieldName, "fieldName")
                        });
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas.SwitchCaseFieldFormula, Vanrise.GenericData.MainExtensions",
                        TargetFieldName: $scope.scopeModel.selectedTargetFieldName.fieldName,
                        SwitchCaseMappings: getSwitchCaseMappings()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getSwitchCaseMappings() {
                var switchCaseMappings = {};

                for (var index = 0 ; index < $scope.scopeModel.switchCaseMappings.length; index++) {
                    var switchCaseMapping = $scope.scopeModel.switchCaseMappings[index];
                    switchCaseMappings[switchCaseMapping.targetFieldValue] = {
                        MappingFieldName: switchCaseMapping.selectedMappingField.fieldName
                    };
                }

                return switchCaseMappings;
            }

        }
        return directiveDefinitionObject;
    }
]);