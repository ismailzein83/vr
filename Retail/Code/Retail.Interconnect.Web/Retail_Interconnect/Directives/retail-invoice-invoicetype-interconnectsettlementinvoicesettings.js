"use strict";

app.directive("retailInvoiceInvoicetypeInterconnectsettlementinvoicesettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "Retail_Interconnect_InvoiceType",
    function (UtilsService, VRNotificationService, VRUIUtilsService, Retail_Interconnect_InvoiceType) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new InterconnectInvoiceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Interconnect/Directives/Templates/InterconnectSettlementInvoiceSettingsTemplate.html"

        };

        function InterconnectInvoiceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionSelectorAPI;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var customerInvoiceTypeSelectorAPI;
            var customerInvoiceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var supplierInvoiceTypeSelectorAPI;
            var supplierInvoiceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorAPI = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCustomerInvoiceTypeSelectorReady = function (api) {
                    customerInvoiceTypeSelectorAPI = api;
                    customerInvoiceTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onSupplierInvoiceTypeSelectorReady = function (api) {
                    supplierInvoiceTypeSelectorAPI = api;
                    supplierInvoiceTypeSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                   
                    promises.push(getBusinessEntityDefinitionSelectorLoadPromise());
                    promises.push(loadCustomerInvoiceTypeSelector());
                    promises.push(loadSupplierInvoiceTypeSelector());

                    function getBusinessEntityDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };
                            if (payload != undefined && payload.extendedSettingsEntity != undefined) {
                                selectorPayload.selectedIds = payload.extendedSettingsEntity.AccountBEDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });
                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }

                    function loadCustomerInvoiceTypeSelector() {
                        var customerInvoiceTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        customerInvoiceTypeSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload;
                            if (payload != undefined && payload.extendedSettingsEntity != undefined) {
                                selectorPayload = {
                                    selectedIds: payload.extendedSettingsEntity.CustomerInvoiceTypeId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(customerInvoiceTypeSelectorAPI, selectorPayload, customerInvoiceTypeSelectorLoadDeferred);
                        });
                        return customerInvoiceTypeSelectorLoadDeferred.promise;
                    }

                    function loadSupplierInvoiceTypeSelector() {
                        var supplierInvoiceTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        supplierInvoiceTypeSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload;
                            if (payload != undefined && payload.extendedSettingsEntity != undefined) {
                                selectorPayload = {
                                    selectedIds: payload.extendedSettingsEntity.SupplierInvoiceTypeId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(supplierInvoiceTypeSelectorAPI, selectorPayload, supplierInvoiceTypeSelectorLoadDeferred);
                        });
                        return supplierInvoiceTypeSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = function () {
                    return {
                        $type: "Retail.Interconnect.Business.InterconnectSettlementInvoiceSettings, Retail.Interconnect.Business",
                        AccountBEDefinitionId: beDefinitionSelectorAPI.getSelectedIds(),
                        CustomerInvoiceTypeId: customerInvoiceTypeSelectorAPI.getSelectedIds(),
                        SupplierInvoiceTypeId: supplierInvoiceTypeSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);