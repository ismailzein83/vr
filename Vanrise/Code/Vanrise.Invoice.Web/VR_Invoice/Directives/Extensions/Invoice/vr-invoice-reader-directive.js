'use strict';

app.directive('vrInvoiceReaderDirective', ['VRUIUtilsService', 'UtilsService',
    function (VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var sqlSourceReader = new invoiceSourceReader($scope, ctrl, $attrs);
                sqlSourceReader.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Invoice/Directives/Extensions/Invoice/Templates/InvoiceSourceReaderTemplate.html'
        };

        function invoiceSourceReader($scope, ctrl, $attrs) {
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
                    var invoiceTypeId

                    if (payload != undefined && payload.Setting != undefined) {
                        invoiceTypeId = payload.Setting.InvoiceTypeId;
                        $scope.scopeModel.batchSize = payload.Setting.BatchSize;
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
                    var setting =
                    {
                        InvoiceTypeId: invoiceTypeSelectorAPI.getSelectedIds(),
                        BatchSize: $scope.scopeModel.batchSize
                    };
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.InvoiceReader.InvoiceSourceReader, Vanrise.Invoice.MainExtensions",
                        Setting: setting
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
