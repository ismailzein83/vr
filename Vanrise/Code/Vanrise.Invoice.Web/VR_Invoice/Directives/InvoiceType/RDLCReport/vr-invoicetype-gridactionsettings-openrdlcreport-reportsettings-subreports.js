"use strict";

app.directive("vrInvoicetypeGridactionsettingsOpenrdlcreportReportsettingsSubreports", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceTypeService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceTypeService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SubReports($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/RDLCReport/Templates/RDLCSubReportsManagement.html"

        };

        function SubReports($scope, ctrl, $attrs) {

            var gridAPI;
            var context;
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [];

                //ctrl.isValid = function () {
                //    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                //        return null;
                //    return "You Should add at least one dataSource.";
                //}

                ctrl.addSubReport = function () {
                    var onSubReportAdded = function (subReport) {
                        ctrl.datasource.push({ Entity: subReport });
                    }
                    VR_Invoice_InvoiceTypeService.addSubReport(onSubReportAdded, getContext());
                };

                ctrl.removeSubReport = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                }
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var subReports;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        subReports = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            subReports.push({
                                SubReportName: currentItem.Entity.SubReportName,
                                SubReportDataSources: currentItem.Entity.SubReportDataSources,
                                FilterGroup: currentItem.Entity.FilterGroup,
                                RepeatedSubReport: currentItem.Entity.RepeatedSubReport,
                                ParentSubreportDataSource: currentItem.Entity.ParentSubreportDataSource,
                            });
                        }
                    }
                    return subReports;
                }

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.subReports != undefined) {
                            for (var i = 0; i < payload.subReports.length; i++) {
                                var subReport = payload.subReports[i];
                                ctrl.datasource.push({ Entity: subReport });
                            }
                        }
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext()
            {
                
                return context;
            }
            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editSubReport,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function editSubReport(subReportObj) {
                var onSubReportUpdated = function (subReport) {
                    var index = ctrl.datasource.indexOf(subReportObj);
                    ctrl.datasource[index] = { Entity: subReport };
                }
                VR_Invoice_InvoiceTypeService.editSubReport(subReportObj.Entity, onSubReportUpdated, getContext());
            }
        }

        return directiveDefinitionObject;

    }
]);