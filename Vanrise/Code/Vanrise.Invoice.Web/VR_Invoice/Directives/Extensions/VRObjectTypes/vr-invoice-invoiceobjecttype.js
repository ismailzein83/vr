(function (app) {

    'use strict';

    InvoiceObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function InvoiceObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var invoiceObjectType = new InvoiceDierctiveObjectType($scope, ctrl, $attrs);
                invoiceObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Invoice/Directives/Extensions/VRObjectTypes/Templates/InvoiceObjectTypeTemplate.html'


        };
        function InvoiceDierctiveObjectType($scope, ctrl, $attrs) {
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
                    var invoiceObjectTypeEntity;
                    if (payload != undefined)
                    {
                        invoiceObjectTypeEntity = payload.objectType;
                    }
                    var invoiceTypeSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
                    var promises = [];
                    promises.push(invoiceTypeSelectorPayloadLoadDeferred.promise);

                    invoiceTypeSelectorReadyDeferred.promise.then(function () {
                        var invoiceTypeSelectorPayload;
                        if (invoiceObjectTypeEntity != undefined) {
                            invoiceTypeSelectorPayload = { selectedIds: invoiceObjectTypeEntity.InvoiceTypeId };
                        }
                        VRUIUtilsService.callDirectiveLoad(invoiceTypeSelectorAPI, invoiceTypeSelectorPayload, invoiceTypeSelectorPayloadLoadDeferred);
                    });
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Invoice.MainExtensions.InvoiceObjectType, Vanrise.Invoice.MainExtensions",
                        InvoiceTypeId: invoiceTypeSelectorAPI.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrInvoiceInvoiceobjecttype', InvoiceObjectType);

})(app);