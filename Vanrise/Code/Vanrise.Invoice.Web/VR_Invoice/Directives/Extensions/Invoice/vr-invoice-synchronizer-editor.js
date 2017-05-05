'use strict';

app.directive('vrInvoiceSynchronizerEditor', ['VRUIUtilsService', 'UtilsService',
    function (VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var invoiceSynchronizer = new InvoiceSynchronizer($scope, ctrl, $attrs);
                invoiceSynchronizer.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Invoice/Directives/Extensions/Invoice/Templates/InvoiceSynchronizerTemplate.html'
        };

        function InvoiceSynchronizer($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var invoiceTypeSelectorAPI;
            var invoiceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onInvoiceTypeSelectorReady = function (api) {
                    invoiceTypeSelectorAPI = api;
                    invoiceTypeSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    var invoiceTypeId;

                    if (payload != undefined) {
                        invoiceTypeId = payload.InvoiceTypeId;
                        $scope.scopeModel.isUpdateOnly = payload.IsUpdateOnly;
                    }

                    var invoiceTypeSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
                    var promises = [];
                    promises.push(invoiceTypeSelectorPayloadLoadDeferred.promise);

                    invoiceTypeSelectorReadyDeferred.promise.then(function () {
                        var invoiceTypeSelectorPayload;
                        if (invoiceTypeId != undefined) {
                            invoiceTypeSelectorPayload = { selectedIds: invoiceTypeId };
                        }
                        VRUIUtilsService.callDirectiveLoad(invoiceTypeSelectorAPI, invoiceTypeSelectorPayload, invoiceTypeSelectorPayloadLoadDeferred);
                    });
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.Business.InvoiceSynchronizer, Vanrise.Invoice.Business",
                        InvoiceTypeId: invoiceTypeSelectorAPI.getSelectedIds(),
                        IsUpdateOnly: $scope.scopeModel.isUpdateOnly
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
