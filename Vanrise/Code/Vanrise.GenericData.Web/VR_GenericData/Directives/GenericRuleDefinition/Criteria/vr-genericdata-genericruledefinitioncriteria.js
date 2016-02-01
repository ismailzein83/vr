(function (app) {

    'use strict';

    GenericRuleDefinitionCriteriaDirective.$inject = ['VR_GenericData_GenericRuleDefinitionCriteriaFieldService'];

    function GenericRuleDefinitionCriteriaDirective(VR_GenericData_GenericRuleDefinitionCriteriaFieldService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericRuleDefinitionCriteria = new GenericRuleDefinitionCriteria($scope, ctrl);
                genericRuleDefinitionCriteria.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: '/Client/Modules/VR_Rules/Directives/GenericRuleDefinition/Criteria/Templates/GenericRuleDefinitionCriteriaTemplate.html'
        };

        function GenericRuleDefinitionCriteria($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                ctrl.criteriaFields = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
                ctrl.addCriteriaField = function () {
                    var onGenericRuleDefinitionCriteriaFieldAdded = function (addedCriteriaField) {
                        gridAPI.itemAdded(addedCriteriaField);
                    };
                    VR_GenericData_GenericRuleDefinitionCriteriaFieldService.addGenericRuleDefinitionCriteriaField(ctrl.criteriaFields, onGenericRuleDefinitionCriteriaFieldAdded);
                };
                ctrl.validateCriteriaFields = function () {
                    if (ctrl.criteriaFields.length == 0) {
                        return 'No fields added';
                    }
                    return null;
                };

                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                        for (var i = 0; i < payload.CriteriaFields.length; i++) {
                            ctrl.criteriaFields.push(payload.CriteriaFields[i]);
                        }
                    }
                };
                api.getData = function () {
                    return (ctrl.criteriaFields.length > 0) ? ctrl.criteriaFields : null;
                };
                return api;
            }

            function defineMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    clicked: editCriteriaField
                }, {
                    name: 'Delete',
                    clicked: deleteCriteriaField
                }];
            }

            function editCriteriaField() {

            }

            function deleteCriteriaField() {

            }
        }
    }

    app.directive('vrGenericdataGenericruledefinitionCriteria', GenericRuleDefinitionCriteriaDirective);

})(app);