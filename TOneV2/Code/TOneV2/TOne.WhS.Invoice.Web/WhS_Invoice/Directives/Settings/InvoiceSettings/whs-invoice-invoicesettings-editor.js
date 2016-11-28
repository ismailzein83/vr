'use strict';

app.directive('whsInvoiceInvoicesettingsEditor', ['UtilsService', 'VRUIUtilsService','WhS_Invoice_InvoiceSettingService',
    function (UtilsService, VRUIUtilsService, WhS_Invoice_InvoiceSettingService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new InvoiceSettingsEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Invoice/Directives/Settings/InvoiceSettings/Templates/InvoiceSettingsTemplate.html"
        };

        function InvoiceSettingsEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;
            var customerInvoiceMailTemplateReadyAPI;
            var customerInvoiceMailTemplateReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.onCustomerInvoiceMailTemplateSelectorReady = function (api) {
                    customerInvoiceMailTemplateReadyAPI = api;
                    customerInvoiceMailTemplateReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var invoiceSettingsPayload;
                    if (payload != undefined && payload.data != undefined) {
                        invoiceSettingsPayload = payload.data;
                    }

                    var promises = [];

                    var customerInvoiceMailTemplateLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(customerInvoiceMailTemplateLoadPromiseDeferred.promise);
                    WhS_Invoice_InvoiceSettingService.getCustomerInvoiceMailType().then(function(response)
                    {
                        customerInvoiceMailTemplateReadyPromiseDeferred.promise.then(function () {
                            var selectorPayload = { filter: { VRMailMessageTypeId: response } };
                            if (invoiceSettingsPayload != undefined && invoiceSettingsPayload.CustomerInvoiceSettings != undefined) {
                                selectorPayload.selectedIds = invoiceSettingsPayload.CustomerInvoiceSettings.DefaultEmailId;
                            }
                            VRUIUtilsService.callDirectiveLoad(customerInvoiceMailTemplateReadyAPI, selectorPayload, customerInvoiceMailTemplateLoadPromiseDeferred);
                        });
                    })

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Invoice.Entities.InvoiceSettings, TOne.WhS.Invoice.Entities",
                        CustomerInvoiceSettings: {
                            DefaultEmailId: customerInvoiceMailTemplateReadyAPI.getSelectedIds()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);


            }
        }
    }]);