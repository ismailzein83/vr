(function (app) {

    'use strict';

    DAProfCalcGroupingFieldManagementDirective.$inject = ['VR_Analytic_DAProfCalcGroupingFieldService', 'UtilsService', 'VRNotificationService'];

    function DAProfCalcGroupingFieldManagementDirective(VR_Analytic_DAProfCalcGroupingFieldService, UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var daProfCalcGroupingField = new DAProfCalcGroupingField($scope, ctrl);
                daProfCalcGroupingField.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/Common/Templates/DAProfCalcGroupingFieldManagementTemplate.html'
        };

        function DAProfCalcGroupingField($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var context;

            function initializeController() {
                ctrl.daProfCalcGroupingFields = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                ctrl.onAddDAProfCalcGroupingField = function () {
                    var onDAProfCalcGroupingFieldAdded = function (addedDAProfCalcGroupingField) {
                        ctrl.daProfCalcGroupingFields.push(addedDAProfCalcGroupingField);
                    };

                    VR_Analytic_DAProfCalcGroupingFieldService.addDAProfCalcGroupingField(buildContext(), onDAProfCalcGroupingFieldAdded);
                };
                ctrl.onDeleteDAProfCalcGroupingField = function (daProfCalcGroupingField) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal(ctrl.daProfCalcGroupingFields, daProfCalcGroupingField.FieldName, 'FieldName');
                            ctrl.daProfCalcGroupingFields.splice(index, 1);
                        }
                    });
                }

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    ctrl.daProfCalcGroupingFields = [];

                    if (payload != undefined) {
                        context = payload.context;
                    }

                    if (payload != undefined && payload.groupingFields != undefined) {
                        ctrl.daProfCalcGroupingFields = payload.groupingFields;
                    }
                };

                api.getData = function () {

                    return ctrl.daProfCalcGroupingFields;
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
                    clicked: editDAProfCalcGroupingField
                }];
            }
            function editDAProfCalcGroupingField(daProfCalcGroupingField) {
                var onDAProfCalcGroupingFieldUpdated = function (updatedDAProfCalcGroupingField) {
                    var index = UtilsService.getItemIndexByVal(ctrl.daProfCalcGroupingFields, daProfCalcGroupingField.RecordFilter, 'RecordFilter');
                    ctrl.daProfCalcGroupingFields[index] = updatedDAProfCalcGroupingField;
                };

                VR_Analytic_DAProfCalcGroupingFieldService.editDAProfCalcGroupingField(daProfCalcGroupingField, buildContext(), onDAProfCalcGroupingFieldUpdated);
            }
        }
    }

    app.directive('vrAnalyticDaprofcalcGroupingfieldManagement', DAProfCalcGroupingFieldManagementDirective);

})(app);