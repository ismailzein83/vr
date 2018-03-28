"use strict";

app.directive("vrInvoiceAutomaticinvoiceprocessManual", ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: "/Client/Modules/VR_Invoice/Directives/AutomaticInvoice/ProcessInput/Normal/Templates/AutomaticInvoiceProcessManualTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        var invoiceTypeSelectorAPI;
        var invoiceTypeSelectorAPIReadyDeferred = UtilsService.createPromiseDeferred();
        var invoiceTypeSelectorAPISelectionChangedDeferred;

        var invoicePeriodGapActionReadyDeferred = UtilsService.createPromiseDeferred();
        var invoicePeriodGapActionAPI;


        var invoicePartnerGroupAPI;
        var invoicePartnerGroupReadyDeferred = UtilsService.createPromiseDeferred();

        var invoiceGenerationPeriodAPI;

        var accountStatusSelectorAPI;
        var accountStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.onInvoicePeriodGapActionReady = function (api) {
                invoicePeriodGapActionAPI = api;
                invoicePeriodGapActionReadyDeferred.resolve();
            };

            $scope.invoiceTypeSelectorReady = function (api) {
                invoiceTypeSelectorAPI = api;
                invoiceTypeSelectorAPIReadyDeferred.resolve();
            };

            $scope.onInvoiceTypeSelectionChanged = function (selectedInvoiceType) {
                if (selectedInvoiceType != undefined) {
                    if (invoiceTypeSelectorAPISelectionChangedDeferred != undefined) {
                        invoiceTypeSelectorAPISelectionChangedDeferred.resolve();
                    }
                    else {
                        var partnerGroupSelectorPaylod = { invoiceTypeId: selectedInvoiceType.InvoiceTypeId, accountStatus: accountStatusSelectorAPI.getData(), isAutomatic: true };
                        var setLoader = function (value) {
                            $scope.isLoadingPartnerGroup = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, invoicePartnerGroupAPI, partnerGroupSelectorPaylod, setLoader);
                    }
                }
            };

            $scope.onInvoicePartnerGroupReady = function (api) {
                invoicePartnerGroupAPI = api;
                invoicePartnerGroupReadyDeferred.resolve();
            };

            $scope.onAccountStatusSelectorReady = function (api) {
                accountStatusSelectorAPI = api;
                accountStatusSelectorReadyDeferred.resolve();
            };

            $scope.onAccountStatusSelectionChanged = function (selectedItem) {
                if (invoicePartnerGroupAPI != undefined) {
                    var accountStatusData = accountStatusSelectorAPI.getData();
                    invoicePartnerGroupAPI.accountStatusChanged(accountStatusData);
                }
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                var isEditMode;

                if (payload != undefined && payload.data != undefined) {
                    isEditMode = true;
                  //  $scope.endDateOffsetFromToday = payload.data.EndDateOffsetFromToday;
                    $scope.issueDateOffsetFromToday = payload.data.IssueDateOffsetFromToday;
                }
                else {
                  //  $scope.endDateOffsetFromToday = 0;
                    $scope.issueDateOffsetFromToday = 0;
                }

                var loadInvoiceTypeSelectorPromise = loadInvoiceTypeSelector();
                promises.push(loadInvoiceTypeSelectorPromise);

                var loadInvoicePeriodGapActionPromise = loadInvoicePeriodGapActionSelector();
                promises.push(loadInvoicePeriodGapActionPromise);

                var loadAccountStatusSelectorDirectivePromise = loadAccountStatusSelectorDirective();
                promises.push(loadAccountStatusSelectorDirectivePromise);

                if (isEditMode) {
                    invoiceTypeSelectorAPISelectionChangedDeferred = UtilsService.createPromiseDeferred();

                    var loadPartnerGroupSelectorPromise = loadPartnerGroupSelector();
                    promises.push(loadPartnerGroupSelectorPromise);
                }

                function loadInvoicePeriodGapActionSelector() {
                    var invoicePeriodGapActionLoadDeferred = UtilsService.createPromiseDeferred();

                    invoicePeriodGapActionReadyDeferred.promise.then(function () {

                        var payloadInvoicePeriodGapAction = {selectFirstItem:true};
                        if (payload != undefined && payload.data != undefined) {
                            payloadInvoicePeriodGapAction.selectedIds = payload.data.InvoiceGapAction;
                        }
                        VRUIUtilsService.callDirectiveLoad(invoicePeriodGapActionAPI, payloadInvoicePeriodGapAction, invoicePeriodGapActionLoadDeferred);
                    });

                    return invoicePeriodGapActionLoadDeferred.promise;
                }

                function loadInvoiceTypeSelector() {
                    var invoiceTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    invoiceTypeSelectorAPIReadyDeferred.promise.then(function () {

                        var payloadSelector;
                        if (payload != undefined && payload.data != undefined) {
                            payloadSelector = { selectedIds: payload.data.InvoiceTypeId };
                        }
                        VRUIUtilsService.callDirectiveLoad(invoiceTypeSelectorAPI, payloadSelector, invoiceTypeSelectorLoadDeferred);
                    });

                    return invoiceTypeSelectorLoadDeferred.promise;
                }

                function loadAccountStatusSelectorDirective() {
                    var accountStatusSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();

                    accountStatusSelectorReadyDeferred.promise.then(function () {
                        var accountStatusSelectorPayload = {
                            selectFirstItem: true,
                            dontShowInActive: true
                        };
                        if (payload != undefined && payload.data != undefined)
                            accountStatusSelectorPayload.selectedIds = payload.data.AccountStatus;

                        VRUIUtilsService.callDirectiveLoad(accountStatusSelectorAPI, accountStatusSelectorPayload, accountStatusSelectorPayloadLoadDeferred);
                    });

                    return accountStatusSelectorPayloadLoadDeferred.promise;
                }

                function loadPartnerGroupSelector() {
                    var partnerGroupSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    var partnerGroupPromises = [];
                    partnerGroupPromises.push(invoicePartnerGroupReadyDeferred.promise);
                    partnerGroupPromises.push(loadAccountStatusSelectorDirectivePromise);
                    partnerGroupPromises.push(invoiceTypeSelectorAPISelectionChangedDeferred.promise);

                    UtilsService.waitMultiplePromises(partnerGroupPromises).then(function () {
                        invoiceTypeSelectorAPISelectionChangedDeferred = undefined;

                        var partnerGroupSelectorPaylod = { invoiceTypeId: payload.data.InvoiceTypeId, accountStatus: accountStatusSelectorAPI.getData(), partnerGroup: payload.data.PartnerGroup, isAutomatic: true };
                        VRUIUtilsService.callDirectiveLoad(invoicePartnerGroupAPI, partnerGroupSelectorPaylod, partnerGroupSelectorLoadDeferred);
                    });

                    return partnerGroupSelectorLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                var accountStatusData = accountStatusSelectorAPI.getData();
                return {
                    InputArguments: {
                        $type: "Vanrise.Invoice.BP.Arguments.AutomaticInvoiceProcessInput, Vanrise.Invoice.BP.Arguments",
                        InvoiceTypeId: invoiceTypeSelectorAPI.getSelectedIds(),
                       // EndDateOffsetFromToday: $scope.endDateOffsetFromToday,
                        IssueDateOffsetFromToday: $scope.issueDateOffsetFromToday,
                        EffectiveDate: accountStatusData.EffectiveDate,
                        IsEffectiveInFuture: accountStatusData.IsEffectiveInFuture,
                        Status: accountStatusData.Status,
                        AccountStatus: accountStatusData.selectedId,
                        PartnerGroup: invoicePartnerGroupAPI.getData(),
                        InvoiceGapAction: invoicePeriodGapActionAPI.getSelectedIds()
                    }
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
