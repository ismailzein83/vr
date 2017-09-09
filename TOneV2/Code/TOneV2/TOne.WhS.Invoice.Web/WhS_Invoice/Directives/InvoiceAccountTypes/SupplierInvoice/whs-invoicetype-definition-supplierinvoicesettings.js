"use strict";

app.directive("whsInvoicetypeDefinitionSupplierinvoicesettings", ["UtilsService", "VRNotificationService",
    function (UtilsService, VRNotificationService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SupplierInvoiceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Invoice/Directives/InvoiceAccountTypes/SupplierInvoice/Templates/SupplierInvoiceSettingsDefinitionTemplate.html"

        };

        function SupplierInvoiceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var usageTransactionTypeSelectorAPI;
            var usageTransactionTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var invoiceTransactionTypeSelectorAPI;
            var invoiceTransactionTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onUsageTransactionTypeSelectorReady = function (api) {
                    usageTransactionTypeSelectorAPI = api;
                    usageTransactionTypeSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onInvoiceTransactionTypeSelectorReady = function (api) {
                    invoiceTransactionTypeSelectorAPI = api;
                    invoiceTransactionTypeSelectorReadyPromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([invoiceTransactionTypeSelectorReadyPromiseDeferred.promise, usageTransactionTypeSelectorReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var extendedSettingsEntity;
                    if (payload != undefined) {
                        extendedSettingsEntity = payload.extendedSettingsEntity;
                    }
                    var promises = [];
                    promises.push(loadInvoiceTransactionTypeSelector());
                    promises.push(loadUsageTransactionTypeSelector());

                    function loadInvoiceTransactionTypeSelector() {
                        var invoiceTransactionTypePayload;
                        if (extendedSettingsEntity != undefined) {
                            invoiceTransactionTypePayload = {
                                selectedIds: extendedSettingsEntity.InvoiceTransactionTypeId
                            };
                        }
                        return invoiceTransactionTypeSelectorAPI.load(invoiceTransactionTypePayload);
                    }

                    function loadUsageTransactionTypeSelector() {
                        var usageTransactionTypePayload;
                        if (extendedSettingsEntity != undefined) {
                            usageTransactionTypePayload = {
                                selectedIds: extendedSettingsEntity.UsageTransactionTypeIds
                            };
                        }
                        return usageTransactionTypeSelectorAPI.load(usageTransactionTypePayload);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Invoice.Business.Extensions.SupplierInvoiceSettings ,TOne.WhS.Invoice.Business",
                        InvoiceTransactionTypeId: invoiceTransactionTypeSelectorAPI.getSelectedIds(),
                        UsageTransactionTypeIds: usageTransactionTypeSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);