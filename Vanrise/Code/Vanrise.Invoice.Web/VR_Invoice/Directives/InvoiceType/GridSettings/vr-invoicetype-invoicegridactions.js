"use strict";

app.directive("vrInvoicetypeInvoicegridactions", ["UtilsService", "VRNotificationService","VR_Invoice_InvoiceTypeService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceTypeService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new InvoiceGridActions($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/GridSettings/Templates/InvoiceGridActionsManagement.html"

        };

        function InvoiceGridActions($scope, ctrl, $attrs) {

            var gridAPI;
            var context;
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [];

                //ctrl.isValid = function () {
                //    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                //        return null;
                //    return "You Should add at least one action.";
                //}

                ctrl.addGridAction = function () {
                    var onGridActionAdded = function (gridAction) {
                        ctrl.datasource.push({ Entity: gridAction });
                    }

                    VR_Invoice_InvoiceTypeService.addGridAction(onGridActionAdded, getContext());
                };

                ctrl.removeAction = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                }
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
                            actions.push({
                                Title: currentItem.Entity.Title,
                                ReloadGridItem: currentItem.Entity.ReloadGridItem,
                                InvoiceFilterCondition: currentItem.Entity.InvoiceFilterCondition,
                                Settings: currentItem.Entity.Settings,
                            });
                        }
                    }
                    return actions;
                }

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.invoiceGridActions != undefined) {
                            for (var i = 0; i < payload.invoiceGridActions.length; i++) {
                                var gridAction = payload.invoiceGridActions[i];
                                ctrl.datasource.push({ Entity: gridAction });
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
                    clicked: editAction,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function editAction(actionObj) {
                var onGridActionUpdated = function (action) {
                    var index = ctrl.datasource.indexOf(actionObj);
                    ctrl.datasource[index] = { Entity: action };
                }
                VR_Invoice_InvoiceTypeService.editGridAction(actionObj.Entity, onGridActionUpdated, getContext());
            }
            function getContext()
            {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);