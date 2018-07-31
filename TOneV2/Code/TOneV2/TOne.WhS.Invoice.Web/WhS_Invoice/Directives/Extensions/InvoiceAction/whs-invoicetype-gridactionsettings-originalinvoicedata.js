"use strict";

app.directive("whsInvoicetypeGridactionsettingsOriginalinvoicedata", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "WhS_Invoice_InvoiceTypeEnum",
    function (UtilsService, VRNotificationService, VRUIUtilsService, WhS_Invoice_InvoiceTypeEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new OriginalInvoiceDataAction($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Invoice/Directives/Extensions/InvoiceAction/Templates/OriginalInvoiceDataActionTemplate.html"

        };

        function OriginalInvoiceDataAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var invoiceCarrierTypeDirectiveAPI;
            var invoiceCarrierSelectorReadyPromiseDeffered= UtilsService.createPromiseDeferred();
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.invoiceCarrierType = UtilsService.getArrayEnum(WhS_Invoice_InvoiceTypeEnum);
                $scope.scopeModel.onInvoiceCarrierTypeDirectiveReady = function (api) {
                    invoiceCarrierTypeDirectiveAPI = api;
                    invoiceCarrierSelectorReadyPromiseDeffered.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var invoiceActionEntity;
                    if (payload != undefined) {
                        invoiceActionEntity = payload.invoiceActionEntity;
                        if (payload.invoiceActionEntity != undefined && payload.invoiceActionEntity.InvoiceCarrierType!=undefined)
                        $scope.scopeModel.selectedValue = UtilsService.getItemByVal($scope.scopeModel.invoiceCarrierType, payload.invoiceActionEntity.InvoiceCarrierType, "value");
                        context = payload.context;
                    }
                    var promises = [];

                    if (invoiceActionEntity != undefined) {
                       
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Invoice.Business.Extensions.OriginalInvoiceDataAction ,TOne.WhS.Invoice.Business",
                        InvoiceCarrierType: $scope.scopeModel.selectedValue != undefined ? $scope.scopeModel.selectedValue.value : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);