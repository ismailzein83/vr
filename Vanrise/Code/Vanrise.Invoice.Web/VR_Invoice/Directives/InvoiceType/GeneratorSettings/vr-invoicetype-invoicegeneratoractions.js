"use strict";

app.directive("vrInvoicetypeInvoicegeneratoractions", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceGeneratorActionService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceGeneratorActionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new GeneratorActionGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/GeneratorSettings/Templates/GeneratorActionGridTemplate.html"

        };

        function GeneratorActionGrid($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                ctrl.datasource = [];

                ctrl.addInvoiceGeneratorAction = function () {
                    var onInvoiceGeneratorActionAdded = function (invoiceGeneratorAction) {
                        ctrl.datasource.push({ Entity: invoiceGeneratorAction });
                    }

                    VR_Invoice_InvoiceGeneratorActionService.addInvoiceGeneratorAction(onInvoiceGeneratorActionAdded, getContext());
                };

                ctrl.removeGeneratorAction = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                }
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var invoiceGeneratorActions;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        invoiceGeneratorActions = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            invoiceGeneratorActions.push(currentItem.Entity);
                        }
                    }
                    return invoiceGeneratorActions;
                }

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.invoiceGeneratorActions != undefined) {
                            for (var i = 0; i < payload.invoiceGeneratorActions.length; i++) {
                                var invoiceGeneratorAction = payload.invoiceGeneratorActions[i];
                                ctrl.datasource.push({ Entity: invoiceGeneratorAction });
                            }
                        }
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editInvoiceGeneratorAction,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function editInvoiceGeneratorAction(invoiceGeneratorActionObj) {
                var onInvoiceGeneratorActionUpdated = function (invoiceGeneratorAction) {
                    var index = ctrl.datasource.indexOf(invoiceGeneratorActionObj);
                    ctrl.datasource[index] = { Entity: invoiceGeneratorAction };
                }

                VR_Invoice_InvoiceGeneratorActionService.editInvoiceGeneratorAction(invoiceGeneratorActionObj.Entity, onInvoiceGeneratorActionUpdated, getContext());
            }
            function getContext() {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);