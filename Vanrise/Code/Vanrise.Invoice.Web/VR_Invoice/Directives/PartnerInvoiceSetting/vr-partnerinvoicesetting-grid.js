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
                        directiveAPI.onPartnerInvoiceSettingAdded = function (item) {
                            gridAPI.itemAdded(item);
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
                }];

                $scope.gridMenuActions = function (dataItem) {
                        return defaultMenuAction;
                };
            }
            function editPartnerInvoiceSetting(dataItem) {
                var onPartnerInvoiceSettingUpdated = function (invoiceSetting) {
                    gridAPI.itemUpdated(invoiceSetting);
                };
                VR_Invoice_PartnerInvoiceSettingService.editPartnerInvoiceSetting(onPartnerInvoiceSettingUpdated, dataItem.Entity.PartnerInvoiceSettingId)
            }
        }

        return directiveDefinitionObject;

    }
]);