(function (app) {

    'use strict';

    Invoicesetting.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceGenerationSettingEnum'];

    function Invoicesetting(UtilsService, VRUIUtilsService, VR_Invoice_InvoiceGenerationSettingEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var selector = new InvoicesettingCtor($scope, ctrl, $attrs);
                selector.initializeController();
            },
            controllerAs: "invoiceSettingCtrl",
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Invoice/Directives/Extensions/PartnerGroup/Templates/InvoiceSettingTemplate.html'
        };

        function InvoicesettingCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var invoiceSettingSelectorAPI;
            var invoiceSettingSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var invoiceGenerationSettingAPI;
            var isAutomatic;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onInvoiceSettingSelectorReady = function (api) {
                    invoiceSettingSelectorAPI = api;
                    invoiceSettingSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onInvoiceGenerationSettingReady = function (api) {
                    invoiceGenerationSettingAPI = api;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var invoiceTypeId = payload.invoiceTypeId;
                    var partnerGroup = payload.partnerGroup;
                    isAutomatic = payload.isAutomatic;
                    $scope.scopeModel.showInvoiceGenerationSettings = isAutomatic != undefined ? !isAutomatic : true;
                    $scope.scopeModel.invoiceGenerationSettings = UtilsService.getArrayEnum(VR_Invoice_InvoiceGenerationSettingEnum);

                    if (partnerGroup != undefined) {
                        $scope.scopeModel.selectedInvoiceGenerationSetting = UtilsService.getEnum(VR_Invoice_InvoiceGenerationSettingEnum, 'value', partnerGroup.Setting);
                    }
                    else {
                        if (isAutomatic) {
                            $scope.scopeModel.selectedInvoiceGenerationSetting = UtilsService.getEnum(VR_Invoice_InvoiceGenerationSettingEnum, 'value', VR_Invoice_InvoiceGenerationSettingEnum.OnlyEnabledAutomaticInvoice.value);
                        }
                        else {
                            $scope.scopeModel.selectedInvoiceGenerationSetting = UtilsService.getEnum(VR_Invoice_InvoiceGenerationSettingEnum, 'value', VR_Invoice_InvoiceGenerationSettingEnum.All.value);
                        }
                    }

                    var loadInvoiceSettingPromise = loadInvoiceSetting();
                    promises.push(loadInvoiceSettingPromise);

                    function loadInvoiceSetting() {
                        var invoiceSettingDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        invoiceSettingSelectorReadyDeferred.promise.then(function () {
                            var filter = { InvoiceTypeId: invoiceTypeId };
                            var invoiceSettingPayload = { filter: filter };
                            if (partnerGroup != undefined) {
                                invoiceSettingPayload.selectedIds = partnerGroup.InvoiceSettingIds;
                            }
                            VRUIUtilsService.callDirectiveLoad(invoiceSettingSelectorAPI, invoiceSettingPayload, invoiceSettingDirectiveLoadPromiseDeferred);
                        });
                        return invoiceSettingDirectiveLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.accountStatusChanged = function (accountStatus) { };

                api.getData = function () {

                    var data = {
                        $type: "Vanrise.Invoice.MainExtensions.PartnerGroup.InvoiceSetting, Vanrise.Invoice.MainExtensions",
                        InvoiceSettingIds: invoiceSettingSelectorAPI.getSelectedIds(),
                        Setting: $scope.scopeModel.selectedInvoiceGenerationSetting.value
                    };
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrInvoicePartnergroupInvoicesetting', Invoicesetting);

})(app);
