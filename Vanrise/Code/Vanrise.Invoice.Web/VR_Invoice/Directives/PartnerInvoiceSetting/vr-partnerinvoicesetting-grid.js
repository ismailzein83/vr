"use strict";

app.directive("vrPartnerinvoicesettingGrid", ["UtilsService", "VRNotificationService", "VR_Invoice_PartnerInvoiceSettingAPIService", "VRUIUtilsService", "VR_Invoice_PartnerInvoiceSettingService",
    function (UtilsService, VRNotificationService, VR_Invoice_PartnerInvoiceSettingAPIService, VRUIUtilsService, VR_Invoice_PartnerInvoiceSettingService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var invoiceGrid = new PartnerInvoiceSettingGrid($scope, ctrl, $attrs);
                invoiceGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/PartnerInvoiceSetting/Templates/PartnerInvoiceSettingGridTemplate.html"

        };

        function PartnerInvoiceSettingGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var invoiceSettingId;
            var invoiceTypeId;
            var gridQuery;
            function initializeController() {

                $scope.datastore = [];
                $scope.gridMenuActions = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (payload) {
                            if (payload != undefined)
                            {
                                gridQuery = payload.query;
                                if (gridQuery != undefined)
                                {
                                  invoiceSettingId = gridQuery.InvoiceSettingId;
                                }
                                invoiceTypeId = payload.invoiceTypeId;
                                return gridAPI.retrieveData(gridQuery);
                            }
                           
                        };
                        directiveAPI.onPartnerInvoiceSettingAdded = function (item) {
                            return gridAPI.retrieveData(gridQuery);
                        };
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Invoice_PartnerInvoiceSettingAPIService.GetFilteredPartnerInvoiceSettings(dataRetrievalInput)
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
                var defaultMenuAction = [{
                    name: "Edit",
                    clicked: editPartnerInvoiceSetting,
                    haspermission: function () {
                        return VR_Invoice_PartnerInvoiceSettingAPIService.HasAssignPartnerAccess(invoiceSettingId);
                    }

                }, {
                    name: "Delete",
                    clicked: deletePartnerInvoiceSetting,
                    haspermission: function () {
                        return VR_Invoice_PartnerInvoiceSettingAPIService.HasAssignPartnerAccess(invoiceSettingId);
                    }
                }];

                $scope.gridMenuActions = function (dataItem) {
                        return defaultMenuAction;
                };
            }
            function editPartnerInvoiceSetting(dataItem) {
                var onPartnerInvoiceSettingUpdated = function (invoiceSetting) {
                    gridAPI.itemUpdated(invoiceSetting);
                };
                VR_Invoice_PartnerInvoiceSettingService.editPartnerInvoiceSetting(onPartnerInvoiceSettingUpdated, invoiceTypeId,dataItem.Entity.PartnerInvoiceSettingId)
            }
            function deletePartnerInvoiceSetting(dataItem) {
                var onPartnerInvoiceSettingDeleted = function () {
                    gridAPI.itemDeleted(dataItem);
                };
                VR_Invoice_PartnerInvoiceSettingService.deletePartnerInvoiceSetting($scope, dataItem.Entity.PartnerInvoiceSettingId, dataItem.Entity.InvoiceSettingID, onPartnerInvoiceSettingDeleted);
            }
        }

        return directiveDefinitionObject;

    }
]);