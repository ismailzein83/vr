(function (app) {

    'use strict';

    DAProfCalcCalculationFieldManagementDirective.$inject = ['VR_Analytic_DAProfCalcCalculationFieldService', 'UtilsService', 'VRNotificationService'];

    function DAProfCalcCalculationFieldManagementDirective(VR_Analytic_DAProfCalcCalculationFieldService, UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var daProfCalcCalculationField = new DAProfCalcCalculationField($scope, ctrl);
                daProfCalcCalculationField.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/Common/Templates/DAProfCalcCalculationFieldManagementTemplate.html'
        };

        function DAProfCalcCalculationField($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var context;

            function initializeController() {
                ctrl.daProfCalcCalculationFields = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                ctrl.onAddDAProfCalcCalculationField = function () {
                    var onDAProfCalcCalculationFieldAdded = function (addedDAProfCalcCalculationField) {
                        ctrl.daProfCalcCalculationFields.push(addedDAProfCalcCalculationField);
                    };

                    VR_Analytic_DAProfCalcCalculationFieldService.addDAProfCalcCalculationField(buildContext(), onDAProfCalcCalculationFieldAdded);
                };
                ctrl.onDeleteDAProfCalcCalculationField = function (daProfCalcCalculationField) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal(ctrl.daProfCalcCalculationFields, daProfCalcCalculationField.FieldName, 'FieldName');
                            ctrl.daProfCalcCalculationFields.splice(index, 1);
                        }
                    });
                }

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    
                    ctrl.daProfCalcCalculationFields = [];

                    if (payload != undefined) {
                        context = payload.context;
                    }

                    if (payload != undefined && payload.calculationFields != undefined) {
                        ctrl.daProfCalcCalculationFields = payload.calculationFields;
                    }
                };

                api.getData = function () {

                    return ctrl.daProfCalcCalculationFields;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildContext() {
                return context;
            }

            function defineMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    clicked: editDAProfCalcCalculationField
                }];
            }
            function editDAProfCalcCalculationField(daProfCalcCalculationField) {
                var onDAProfCalcCalculationFieldUpdated = function (updatedDAProfCalcCalculationField) {
                    var index = UtilsService.getItemIndexByVal(ctrl.daProfCalcCalculationFields, daProfCalcCalculationField.RecordFilter, 'RecordFilter');
                    ctrl.daProfCalcCalculationFields[index] = updatedDAProfCalcCalculationField;
                };

                VR_Analytic_DAProfCalcCalculationFieldService.editDAProfCalcCalculationField(daProfCalcCalculationField, buildContext(), onDAProfCalcCalculationFieldUpdated);
            }
        }
    }

    app.directive('vrAnalyticDaprofcalcCalculationfieldManagement', DAProfCalcCalculationFieldManagementDirective);

})(app);