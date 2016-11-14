(function (app) {

    'use strict';

    DAProfCalcAggregationFieldManagementDirective.$inject = ['VR_Analytic_DAProfCalcAggregationFieldService', 'UtilsService', 'VRNotificationService'];

    function DAProfCalcAggregationFieldManagementDirective(VR_Analytic_DAProfCalcAggregationFieldService, UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var daProfCalcAggregationField = new DAProfCalcAggregationField($scope, ctrl);
                daProfCalcAggregationField.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/Common/Templates/DAProfCalcAggregationFieldManagementTemplate.html'
        };

        function DAProfCalcAggregationField($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var context;

            function initializeController() {
                ctrl.daProfCalcAggregationFields = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                ctrl.onAddDAProfCalcAggregationField = function () {
                    var onDAProfCalcAggregationFieldAdded = function (addedDAProfCalcAggregationField) {
                        ctrl.daProfCalcAggregationFields.push(addedDAProfCalcAggregationField);
                    };

                    VR_Analytic_DAProfCalcAggregationFieldService.addDAProfCalcAggregationField(buildContext(), onDAProfCalcAggregationFieldAdded);
                };
                ctrl.onDeleteDAProfCalcAggregationField = function (daProfCalcAggregationField) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal(ctrl.daProfCalcAggregationFields, daProfCalcAggregationField.FieldName, 'FieldName');
                            ctrl.daProfCalcAggregationFields.splice(index, 1);
                        }
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        context = payload.context;
                    }

                    if (payload != undefined && payload.aggregationFields) {
                        ctrl.daProfCalcAggregationFields = payload.aggregationFields;
                    }
                };

                api.getData = function () {

                    return ctrl.daProfCalcAggregationFields;
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
                    clicked: editDAProfCalcAggregationField
                }];
            }
            function editDAProfCalcAggregationField(daProfCalcAggregationField) {
                var onDAProfCalcAggregationFieldUpdated = function (updatedDAProfCalcAggregationField) {
                    var index = UtilsService.getItemIndexByVal(ctrl.daProfCalcAggregationFields, daProfCalcAggregationField.FieldName, 'FieldName');
                    ctrl.daProfCalcAggregationFields[index] = updatedDAProfCalcAggregationField;
                };

                VR_Analytic_DAProfCalcAggregationFieldService.editDAProfCalcAggregationField(daProfCalcAggregationField, buildContext(), onDAProfCalcAggregationFieldUpdated);
            }
        }
    }

    app.directive('vrAnalyticDaprofcalcAggregationfieldManagement', DAProfCalcAggregationFieldManagementDirective);

})(app);