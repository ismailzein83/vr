(function (app) {

    'use strict';

    Specificaccounts.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceTypeAPIService'];

    function Specificaccounts(UtilsService, VRUIUtilsService, VR_Invoice_InvoiceTypeAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var selector = new SpecificaccountsCtor($scope, ctrl, $attrs);
                selector.initializeController();
            },
            controllerAs: "specificAccountCtrl",
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Invoice/Directives/Extensions/PartnerGroup/Templates/SpecificAccountsTemplate.html'
        };

        function SpecificaccountsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var invoiceTypeId;
            var accountStatus;

            var selectorAPI;
            var partnerSelectorAPI;
            var partnerSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onPartnerSelectorReady = function (api) {
                    partnerSelectorAPI = api;
                    partnerSelectorReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    invoiceTypeId = payload.invoiceTypeId;
                    accountStatus = payload.accountStatus;

                    var invoiceTypePromise = getInvoiceType();
                    promises.push(invoiceTypePromise);

                    var loadPartnerSelectorDirectivePromise = loadPartnerSelectorDirective();
                    promises.push(loadPartnerSelectorDirectivePromise);


                    function getInvoiceType() {
                        var invoiceTypePromise = VR_Invoice_InvoiceTypeAPIService.GetGeneratorInvoiceTypeRuntime(invoiceTypeId).then(function (response) {
                            $scope.scopeModel.invoiceTypeEntity = response;
                        });
                        return invoiceTypePromise;
                    }


                    function loadPartnerSelectorDirective() {
                        var partnerSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([partnerSelectorReadyDeferred.promise, invoiceTypePromise]).then(function () {
                            var partnerSelectorPayload = {
                                //context: getContext(),
                                extendedSettings: $scope.scopeModel.invoiceTypeEntity.InvoiceType.Settings.ExtendedSettings,
                                invoiceTypeId: invoiceTypeId,
                                filter: accountStatus
                            };

                            VRUIUtilsService.callDirectiveLoad(partnerSelectorAPI, partnerSelectorPayload, partnerSelectorPayloadLoadDeferred);
                        });
                        return partnerSelectorPayloadLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.accountStatusChanged = function (newAccountStatus) {
                    accountStatus = newAccountStatus;

                    if (partnerSelectorAPI != undefined) {
                        var partnerSelectorPayload = { extendedSettings: $scope.scopeModel.invoiceTypeEntity.InvoiceType.Settings.ExtendedSettings, invoiceTypeId: invoiceTypeId, filter: accountStatus };
                        var setLoader = function (value) {
                            $scope.isLoadingPartnerSelector = value;
                        };

                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, partnerSelectorAPI, partnerSelectorPayload, setLoader);
                    }
                };

                api.getData = function () {

                    var data = {
                        $type: "Vanrise.Invoice.MainExtensions.PartnerGroup.SpecifcAccounts, Vanrise.Invoice.MainExtensions",
                        AccountIds: partnerSelectorAPI.getData().selectedIds
                    };
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrInvoicePartnergroupSpecificaccounts', Specificaccounts);

})(app);
