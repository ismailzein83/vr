"use strict";

app.directive("vrInvoicetypeGrid", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceTypeAPIService", "VRUIUtilsService", "VR_Invoice_InvoiceTypeService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceTypeAPIService, VRUIUtilsService, VR_Invoice_InvoiceTypeService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var invoiceGrid = new InvoiceTypeGrid($scope, ctrl, $attrs);
                invoiceGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/Templates/InvoiceTypeGridTemplate.html"

        };

        function InvoiceTypeGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {

                $scope.datastore = [];
                $scope.gridMenuActions = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VR_Invoice_InvoiceTypeService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };
                        directiveAPI.onInvoiceTypeAdded = function (invoiceType) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(invoiceType);
                            gridAPI.itemAdded(invoiceType);
                        };
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Invoice_InvoiceTypeAPIService.GetFilteredInvoiceTypes(dataRetrievalInput)
                        .then(function (response) {
                            if (response.Data != undefined) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                                }
                            }
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };

                defineMenuActions();
            }

            function defineMenuActions() {
                $scope.gridMenuActions.push({
                    name: "Edit",
                    clicked: editInvoiceType,
                    haspermission: hasUpdateInvoiceTypePermission
                });
            }
            function hasUpdateInvoiceTypePermission() {
                return VR_Invoice_InvoiceTypeAPIService.HasUpdateInvoiceTypePermission();
            }
            function editInvoiceType(dataItem)
            {
                var onInvoiceTypeUpdated = function (invoiceType) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(invoiceType);
                    gridAPI.itemUpdated(invoiceType);
                };
                VR_Invoice_InvoiceTypeService.editInvoiceType(onInvoiceTypeUpdated, dataItem.Entity.InvoiceTypeId);
            }
        }

        return directiveDefinitionObject;

    }
]);