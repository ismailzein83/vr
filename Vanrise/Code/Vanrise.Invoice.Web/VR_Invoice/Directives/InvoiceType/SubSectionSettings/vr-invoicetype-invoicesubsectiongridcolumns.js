"use strict";

app.directive("vrInvoicetypeInvoicesubsectiongridcolumns", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceTypeService", "VR_Invoice_InvoiceFieldEnum",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceTypeService, VR_Invoice_InvoiceFieldEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new InvoiceSubSectionGridColumns($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/SubSectionSettings/Templates/InvoiceSubSectionGridColumnsManagement.html"

        };

        function InvoiceSubSectionGridColumns($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one column.";
                }

                ctrl.addGridColumn = function () {
                    var onSubSectionGridColumnAdded = function (gridColumn) {
                        ctrl.datasource.push({ Entity: gridColumn });
                    }
                    VR_Invoice_InvoiceTypeService.addSubSectionGridColumn(onSubSectionGridColumnAdded, ctrl.datasource);
                };

                ctrl.removeColumn = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
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
                                FieldName: currentItem.Entity.FieldName,
                                FieldType: currentItem.Entity.FieldType,
                                WidthFactor: currentItem.Entity.WidthFactor,
                            });
                        }
                    }
                    return columns;
                }

                api.load = function (payload) {
                    if (payload != undefined) {
                        if (payload.gridColumns != undefined) {
                            for (var i = 0; i < payload.gridColumns.length; i++) {
                                var gridColumn = payload.gridColumns[i];
                                ctrl.datasource.push({ Entity: gridColumn });
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
                    clicked: editColumn,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function editColumn(columnObj) {
                var onSubSectionGridColumnUpdated = function (column) {
                    var index = ctrl.datasource.indexOf(columnObj);
                    ctrl.datasource[index] = { Entity: column };
                }
                VR_Invoice_InvoiceTypeService.editSubSectionGridColumn(columnObj.Entity, onSubSectionGridColumnUpdated, ctrl.datasource);
            }
        }

        return directiveDefinitionObject;

    }
]);