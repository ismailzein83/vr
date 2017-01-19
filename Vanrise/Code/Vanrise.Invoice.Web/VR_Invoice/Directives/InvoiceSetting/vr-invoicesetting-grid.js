"use strict";

app.directive("vrInvoicesettingGrid", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceSettingAPIService", "VRUIUtilsService", "VR_Invoice_InvoiceSettingService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceSettingAPIService, VRUIUtilsService, VR_Invoice_InvoiceSettingService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var invoiceGrid = new InvoiceSettingGrid($scope, ctrl, $attrs);
                invoiceGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceSetting/Templates/InvoiceSettingGridTemplate.html"

        };

        function InvoiceSettingGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            function initializeController() {

                $scope.datastore = [];
                $scope.gridMenuActions = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Invoice_InvoiceSettingAPIService.GetFilteredInvoiceSettings(dataRetrievalInput)
                        .then(function (response) {
                            if (response.Data != undefined) {
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
                    clicked: editInvoiceSetting,
                    haspermission: hasUpdateInvoiceSettingPermission
                });
            }
            function hasUpdateInvoiceSettingPermission() {
                return VR_Invoice_InvoiceSettingAPIService.HasUpdateInvoiceSettingPermission();
            }
            function editInvoiceSetting(dataItem) {
                var onInvoiceSettingUpdated = function (invoiceSetting) {
                    gridAPI.itemUpdated(invoiceSetting);
                };
                VR_Invoice_InvoiceSettingService.editInvoiceSetting(onInvoiceSettingUpdated, dataItem.Entity.InvoiceSettingId, dataItem.Entity.InvoiceTypeId)
            }
        }

        return directiveDefinitionObject;

    }
]);