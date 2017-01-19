﻿"use strict";

app.directive("vrInvoicetypeAutomaticinvoiceactions", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceActionService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceActionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new AutomaticInvoiceActions($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/AutomaticInvoiceAction/Templates/AutomaticInvoiceActionsGridTemplate.html"

        };

        function AutomaticInvoiceActions($scope, ctrl, $attrs) {

            var gridAPI;
            var context;
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.addAutomaticInvoiceAction = function () {
                    var onAutomaticInvoiceActionAdded = function (gridAction) {
                        ctrl.datasource.push({ Entity: gridAction });
                    };

                    VR_Invoice_InvoiceActionService.addAutomaticInvoiceAction(onAutomaticInvoiceActionAdded, getContext());
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
                        if (payload.invoiceActions != undefined) {
                            for (var i = 0; i < payload.invoiceActions.length; i++) {
                                var gridAction = payload.invoiceActions[i];
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
                var onAutomaticInvoiceActionUpdated = function (action) {
                    var index = ctrl.datasource.indexOf(actionObj);
                    ctrl.datasource[index] = { Entity: action };
                };
                VR_Invoice_InvoiceActionService.editAutomaticInvoiceAction(actionObj.Entity, onAutomaticInvoiceActionUpdated, getContext());
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                currentContext.getRDLCActionsInfo = function () {
                    var actionsInfo = [];
                    if (ctrl.datasource.length > 0) {


                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var action = ctrl.datasource[i];
                            actionsInfo.push({
                                Title: action.Entity.Title,
                                AutomaticInvoiceActionId: action.Entity.AutomaticInvoiceActionId
                            });
                        }
                    }
                    return actionsInfo;
                };
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);