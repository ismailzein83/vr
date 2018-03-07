"use strict";

app.directive("whsInvoicetypeDefinitionSettlementinvoicesettings", ["UtilsService", "VRNotificationService",
    function (UtilsService, VRNotificationService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SattlementInvoiceSettingsDefinition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Invoice/Directives/InvoiceAccountTypes/SettlementInvoice/Templates/SettlementInvoiceSettingsDefinitionTemplate.html"

        };

        function SattlementInvoiceSettingsDefinition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            var supplierInvoiceTypeSelectorAPI;
            var supplierInvoiceTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var customerInvoiceTypeSelectorAPI;
            var customerInvoiceTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSupplierInvoiceTypeSelectorReady = function (api) {
                    supplierInvoiceTypeSelectorAPI = api;
                    supplierInvoiceTypeSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCustomerInvoiceTypeSelectorReady = function (api) {
                    customerInvoiceTypeSelectorAPI = api;
                    customerInvoiceTypeSelectorReadyPromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([supplierInvoiceTypeSelectorReadyPromiseDeferred.promise, customerInvoiceTypeSelectorReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var extendedSettingsEntity;
                    if (payload != undefined) {
                        extendedSettingsEntity = payload.extendedSettingsEntity;
                    }

                    promises.push(loadSupplierInvoiceTypeSelector());
                    promises.push(loadCustomerInvoiceTypeSelector());

                    function loadCustomerInvoiceTypeSelector() {
                        var customerInvoiceTypePayload;
                        if (extendedSettingsEntity != undefined) {
                            customerInvoiceTypePayload = {
                                selectedIds: extendedSettingsEntity.CustomerInvoiceTypeId
                            };
                        }
                        return customerInvoiceTypeSelectorAPI.load(customerInvoiceTypePayload);
                    }


                    function loadSupplierInvoiceTypeSelector() {
                        var supplierInvoiceTypePayload;
                        if (extendedSettingsEntity != undefined) {
                            supplierInvoiceTypePayload = {
                                selectedIds: extendedSettingsEntity.SupplierInvoiceTypeId
                            };
                        }
                        return supplierInvoiceTypeSelectorAPI.load(supplierInvoiceTypePayload);
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Invoice.Business.Extensions.SettlementInvoiceSettings ,TOne.WhS.Invoice.Business",
                        CustomerInvoiceTypeId: customerInvoiceTypeSelectorAPI.getSelectedIds(),
                        SupplierInvoiceTypeId: supplierInvoiceTypeSelectorAPI.getSelectedIds(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);