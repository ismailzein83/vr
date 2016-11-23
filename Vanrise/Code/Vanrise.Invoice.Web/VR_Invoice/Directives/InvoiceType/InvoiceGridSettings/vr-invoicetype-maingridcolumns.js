"use strict";

app.directive("vrInvoicetypeMaingridcolumns", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceGridSettingService", "VR_Invoice_InvoiceFieldEnum",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceGridSettingService, VR_Invoice_InvoiceFieldEnum) {

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
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceGridSettings/Templates/MainGridColumnsManagement.html"

        };

        function MainGridColumns($scope, ctrl, $attrs) {

            var gridAPI;
            var context;

            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];

                ctrl.invoiceFields = UtilsService.getArrayEnum(VR_Invoice_InvoiceFieldEnum);

                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one column.";
                };

                ctrl.addGridColumn = function () {
                    var onGridColumnAdded = function (gridColumn) {
                        ctrl.datasource.push(addNeededFields({ Entity: gridColumn }));
                    };

                    VR_Invoice_InvoiceGridSettingService.addGridColumn(onGridColumnAdded, getContext());
                };

                ctrl.removeColumn = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
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
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.mainGridColumns != undefined) {
                            for (var i = 0; i < payload.mainGridColumns.length; i++) {
                                var gridColumn = payload.mainGridColumns[i];
                                ctrl.datasource.push(addNeededFields({ Entity: gridColumn }));
                            }
                        }
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function addNeededFields(dataItem) {
                dataItem.FieldDescription = UtilsService.getItemByVal(ctrl.invoiceFields, dataItem.Entity.Field, "value").description;
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

                VR_Invoice_InvoiceGridSettingService.editGridColumn(columnObj.Entity, onGridColumnUpdated, getContext());
            }
            function getContext()
            {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);