"use strict";

app.directive("partnerportalInvoiceGridactions", ["UtilsService", "VRNotificationService", "PartnerPortal_Invoice_InvoiceViewerTypeService",
    function (UtilsService, VRNotificationService, PartnerPortal_Invoice_InvoiceViewerTypeService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new GridActions($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/PartnerPortal_Invoice/Elements/InvoiceViewerType/Directives/Templates/GridActionsManagement.html"

        };

        function GridActions($scope, ctrl, $attrs) {

            var gridAPI;
            var context;
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.addGridAction = function () {
                    var onGridActionAdded = function (gridAction) {
                        ctrl.datasource.push({ Entity: gridAction });
                    };

                    PartnerPortal_Invoice_InvoiceViewerTypeService.addGridAction(onGridActionAdded, getContext());
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
                        if (payload.gridActions != undefined) {
                            for (var i = 0; i < payload.gridActions.length; i++) {
                                var gridAction = payload.gridActions[i];
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
                var onGridActionUpdated = function (action) {
                    var index = ctrl.datasource.indexOf(actionObj);
                    ctrl.datasource[index] = { Entity: action };
                };
                PartnerPortal_Invoice_InvoiceViewerTypeService.editGridAction(actionObj.Entity, onGridActionUpdated, getContext());
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);