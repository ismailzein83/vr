"use strict";

app.directive("vrInvoicetypeMenualbulkactions", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceBulkActionService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceBulkActionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new MenualBulkActions($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/MenualBulkAction/Templates/MenualBulkActionsManagement.html"

        };

        function MenualBulkActions($scope, ctrl, $attrs) {

            var gridAPI;
            var context;
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.addMenualInvoiceBulkAction = function () {
                    var onInvoiceBulkActionAdded = function (bulkAction) {
                        ctrl.datasource.push({ Entity: bulkAction });
                    };

                    VR_Invoice_InvoiceBulkActionService.addMenualBulkAction(onInvoiceBulkActionAdded, getContext());
                };

                ctrl.removeAction = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var actions;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        actions = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            actions.push(currentItem.Entity);
                        }
                    }
                    return actions;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.invoiceMenualBulkActions != undefined) {
                            for (var i = 0; i < payload.invoiceMenualBulkActions.length; i++) {
                                var gridAction = payload.invoiceMenualBulkActions[i];
                                ctrl.datasource.push({ Entity: gridAction });
                            }
                        }
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editAction,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editAction(actionObj) {
                var onInvoiceBulkActionUpdated = function (bulkAction) {
                    var index = ctrl.datasource.indexOf(actionObj);
                    ctrl.datasource[index] = { Entity: bulkAction };
                };
                VR_Invoice_InvoiceBulkActionService.editMenualBulkAction(actionObj.Entity, onInvoiceBulkActionUpdated, getContext());
            }
            function getContext()
            {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);