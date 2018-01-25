(function (app) {

    'use strict';

    ReprocessfilterdefinitionDefinitionGenericfilter.$inject = ['Reprocess_ReprocessFilterFieldDefinitionService', 'UtilsService', 'VRNotificationService'];

    function ReprocessfilterdefinitionDefinitionGenericfilter(Reprocess_ReprocessFilterFieldDefinitionService, UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var reprocessFilterFieldDefinition = new ReprocessFilterFieldDefinition($scope, ctrl);
                reprocessFilterFieldDefinition.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Reprocess/Directives/MainExtensions/Definition/ReprocessFilterDefinition/Templates/ReprocessFilterDefinitionGenericFilter.html'
        };

        function ReprocessFilterFieldDefinition($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var context;

            function initializeController() {
                ctrl.reprocessFilterFieldDefinitionFields = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                ctrl.onAddReprocessFilterFieldDefinition = function () {
                    var onReprocessFilterFieldDefinitionAdded = function (addedReprocessFilterFieldDefinition) {
                        if (context.onFieldAdded != undefined && typeof (context.onFieldAdded) == 'function') {
                            context.onFieldAdded(addedReprocessFilterFieldDefinition.FieldName);
                        }
                        ctrl.reprocessFilterFieldDefinitionFields.push(addedReprocessFilterFieldDefinition);
                    };

                    Reprocess_ReprocessFilterFieldDefinitionService.addReprocessFilterFieldDefinition(onReprocessFilterFieldDefinitionAdded);
                };

                ctrl.onDeleteReprocessFilterFieldDefinition = function (reprocessFilterFieldDefinitionField) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            if (context.onFieldDeleted != undefined && typeof (context.onFieldDeleted) == 'function') {
                                context.onFieldDeleted(reprocessFilterFieldDefinitionField.FieldName);
                            }
                            var index = UtilsService.getItemIndexByVal(ctrl.reprocessFilterFieldDefinitionFields, reprocessFilterFieldDefinitionField.FieldName, 'FieldName');
                            ctrl.reprocessFilterFieldDefinitionFields.splice(index, 1);
                        }
                    });
                };

                ctrl.isValid = function () {
                    if (ctrl.reprocessFilterFieldDefinitionFields == undefined || ctrl.reprocessFilterFieldDefinitionFields.length == 0) {
                        return "You should add at least one field";
                    }
                    return null;
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    ctrl.reprocessFilterFieldDefinitionFields = [];

                    if (payload != undefined) {
                        context = payload.context;
                    }

                    if (payload != undefined && payload.filterDefinition != undefined && payload.filterDefinition.Fields != undefined) {
                        ctrl.reprocessFilterFieldDefinitionFields = payload.filterDefinition.Fields;
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.Reprocess.Business.GenericReprocessFilterDefinition,Vanrise.Reprocess.Business',
                        Fields: ctrl.reprocessFilterFieldDefinitionFields
                    };
                };

                

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    clicked: editReprocessFilterFieldDefinition
                }];
            }
            function editReprocessFilterFieldDefinition(reprocessFilterFieldDefinition) {
                var onReprocessFilterFieldDefinitionUpdated = function (updatedReprocessFilterFieldDefinition) {
                    var index = UtilsService.getItemIndexByVal(ctrl.reprocessFilterFieldDefinitionFields, reprocessFilterFieldDefinition.FieldName, 'FieldName');
                    ctrl.reprocessFilterFieldDefinitionFields[index] = updatedReprocessFilterFieldDefinition;
                };

                Reprocess_ReprocessFilterFieldDefinitionService.editReprocessFilterFieldDefinition(reprocessFilterFieldDefinition, onReprocessFilterFieldDefinitionUpdated);
            }
        }
    }
    
    app.directive('reprocessReprocessfilterdefinitionDefinitionGenericfilter', ReprocessfilterdefinitionDefinitionGenericfilter);

})(app);