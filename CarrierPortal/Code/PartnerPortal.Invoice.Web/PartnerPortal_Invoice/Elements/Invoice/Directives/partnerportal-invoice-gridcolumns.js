"use strict";

app.directive("partnerportalInvoiceGridcolumns", ["UtilsService", "VRNotificationService", "PartnerPortal_Invoice_InvoiceService", "PartnerPortal_Invoice_InvoiceTypeAPIService",
    function (UtilsService, VRNotificationService, PartnerPortal_Invoice_InvoiceService, PartnerPortal_Invoice_InvoiceTypeAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new MainGridColumns($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/PartnerPortal_Invoice/Elements/Invoice/Directives/Templates/GridColumnsManagement.html"

        };

        function MainGridColumns($scope, ctrl, $attrs) {

            var gridAPI;
            var context;

            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];

                ctrl.invoiceFields = [];

                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one column.";
                };

                ctrl.addGridColumn = function () {
                    var onGridColumnAdded = function (gridColumn) {
                        ctrl.datasource.push(addNeededFields({ Entity: gridColumn }));
                    };

                    PartnerPortal_Invoice_InvoiceService.addGridColumn(onGridColumnAdded, getContext());
                };

                ctrl.removeColumn = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                ctrl.disableAddGridColumn = function()
                {
                    if (context != undefined && context.getConnectionId != undefined)
                        return false;
                    return true;
                }
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var columns;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        columns = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            columns.push({
                                Header: currentItem.Entity.Header,
                                Field: currentItem.Entity.Field,
                                CustomFieldName: currentItem.Entity.CustomFieldName,
                            });
                        }
                    }
                    return columns;
                };

                api.load = function (payload) {
                    ctrl.datasource.length = 0;
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                    }
                    promises.push(loadInvoiceFields());
                    function loadInvoiceFields() {
                        if(context != undefined && context.getConnectionId != undefined)
                        {
                            var connectionId = context.getConnectionId();
                            return PartnerPortal_Invoice_InvoiceTypeAPIService.GetRemoteInvoiceFieldsInfo(connectionId).then(function (response) {
                                ctrl.invoiceFields = response;
                                if (payload.gridColumns != undefined) {
                                    for (var i = 0; i < payload.gridColumns.length; i++) {
                                        var gridColumn = payload.gridColumns[i];
                                        ctrl.datasource.push(addNeededFields({ Entity: gridColumn }));
                                    }

                                }
                            });
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function addNeededFields(dataItem) {
                dataItem.FieldDescription = UtilsService.getItemByVal(ctrl.invoiceFields, dataItem.Entity.Field, "InvoiceFieldId").Name;
                return dataItem;
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editColumn,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editColumn(columnObj) {
                var onGridColumnUpdated = function (column) {
                    var index = ctrl.datasource.indexOf(columnObj);
                    ctrl.datasource[index] = addNeededFields({ Entity: column });
                };

                PartnerPortal_Invoice_InvoiceService.editGridColumn(columnObj.Entity, onGridColumnUpdated, getContext());
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