"use strict";

app.directive("vrInvoicetypeGridactionsettingsOpenrdlcreportDatasources", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceTypeService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceTypeService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new DataSources($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/RDLCReport/Templates/RDLCReportDataSourceManagement.html"

        };

        function DataSources($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one dataSource.";
                }

                ctrl.addDataSource = function () {
                    var onDataSourceAdded = function (dataSource) {
                        ctrl.datasource.push({ Entity: dataSource });
                    }
                    VR_Invoice_InvoiceTypeService.addDataSource(onDataSourceAdded, ctrl.datasource);
                };

                ctrl.removeDataSource = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                }
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var dataSources;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        dataSources = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            dataSources.push({
                                DataSourceName: currentItem.Entity.DataSourceName,
                                Settings: currentItem.Entity.Settings,
                            });
                        }
                    }
                    return dataSources;
                }

                api.load = function (payload) {
                    if (payload != undefined) {
                        if (payload.dataSources != undefined) {
                            for (var i = 0; i < payload.dataSources.length; i++) {
                                var dataSource = payload.dataSources[i];
                                ctrl.datasource.push({ Entity: dataSource });
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
                    clicked: editDataSource,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function editDataSource(dataSourceObj) {
                var onDataSourceUpdated = function (dataSource) {
                    var index = ctrl.datasource.indexOf(dataSourceObj);
                    ctrl.datasource[index] = { Entity: dataSource };
                }
                VR_Invoice_InvoiceTypeService.editDataSource(dataSourceObj.Entity, onDataSourceUpdated, ctrl.datasource);
            }
        }

        return directiveDefinitionObject;

    }
]);