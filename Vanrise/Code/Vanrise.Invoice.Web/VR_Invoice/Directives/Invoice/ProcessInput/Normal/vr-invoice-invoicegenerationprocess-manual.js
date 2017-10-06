"use strict";
app.directive("vrInvoiceInvoicegenerationprocessManual", ['UtilsService', 'VRUIUtilsService', 'VRValidationService', 'VRDateTimeService', 'VR_Invoice_InvoiceGenerationPeriodEnum', 'VR_Invoice_InvoiceTypeAPIService',
    function (UtilsService, VRUIUtilsService, VRValidationService, VRDateTimeService, VR_Invoice_InvoiceGenerationPeriodEnum, VR_Invoice_InvoiceTypeAPIService) {
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
            templateUrl: "/Client/Modules/VR_Invoice/Directives/Invoice/ProcessInput/Normal/Templates/InvoiceGenerationProcessManualTemplate.html"
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            var invoicePartnerGroupAPI;
            var invoicePartnerGroupReadyDeferred = UtilsService.createPromiseDeferred();

            var invoiceGenerationPeriodAPI;

            var accountStatusSelectorAPI;
            var accountStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var invoicePartnerGridAPI;
            var invoicePartnerGridReadyDeferred = UtilsService.createPromiseDeferred();

            var invoiceType;
            var invoiceTypeId;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onInvoicePartnerGroupReady = function (api) {
                    invoicePartnerGroupAPI = api;
                    invoicePartnerGroupReadyDeferred.resolve();
                }

                $scope.scopeModel.onInvoiceGenerationPeriodReady = function (api) {
                    invoiceGenerationPeriodAPI = api;
                }

                $scope.scopeModel.validateDates = function () {
                    return VRValidationService.validateTimeRange($scope.scopeModel.fromDate, $scope.scopeModel.toDate);
                }

                $scope.scopeModel.areDatesRequired = function () {
                    if ($scope.scopeModel.selectedInvoiceGenerationPeriod != undefined && $scope.scopeModel.selectedInvoiceGenerationPeriod.datesRequired)
                        return true;
                    return false;
                }

                $scope.scopeModel.onAccountStatusSelectorReady = function (api) {
                    accountStatusSelectorAPI = api;
                    accountStatusSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onAccountStatusSelectionChanged = function (selectedItem) {
                    if (invoicePartnerGroupAPI != undefined) {
                        var accountStatusData = accountStatusSelectorAPI.getData();
                        invoicePartnerGroupAPI.accountStatusChanged(accountStatusData);
                    }
                }

                $scope.scopeModel.onInvoicePartnerReady = function (api) {
                    invoicePartnerGridAPI = api;
                    invoicePartnerGridReadyDeferred.resolve();
                }

                $scope.scopeModel.evaluate = function () {
                    var accountStatusData = accountStatusSelectorAPI.getData();
                    var query = {
                        InvoiceTypeId: invoiceTypeId,
                        EffectiveDate: accountStatusData.EffectiveDate,
                        IsEffectiveInFuture: accountStatusData.IsEffectiveInFuture,
                        Status: accountStatusData.Status,
                        Period: $scope.scopeModel.selectedInvoiceGenerationPeriod.value,
                        FromDate: $scope.scopeModel.fromDate,
                        ToDate: $scope.scopeModel.toDate,
                        IssueDate: $scope.scopeModel.issueDate,
                        PartnerGroup: invoicePartnerGroupAPI.getData()
                    };
                    var gridPayload = { query: query, customPayloadDirective: invoiceType.Settings.ExtendedSettings.GenerationCustomSection.GenerationCustomSectionDirective };
                    invoicePartnerGridAPI.loadGrid(gridPayload);
                }

                UtilsService.waitMultiplePromises([invoicePartnerGridReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {

                var api = {};
                api.getData = function () {
                    return {
                        InputArguments: {
                            $type: "Vanrise.Invoice.BP.Arguments.InvoiceGenerationProcessInput, Vanrise.Invoice.BP.Arguments"
                        }
                    };
                };

                api.load = function (payload) {
                    var promises = [];
                    invoiceTypeId = 'eadc10c8-ffd7-4ee3-9501-0b2ce09029ad';//payload.invoiceTypeId;

                    $scope.scopeModel.invoiceGenerationPeriods = UtilsService.getArrayEnum(VR_Invoice_InvoiceGenerationPeriodEnum);
                    $scope.scopeModel.selectedInvoiceGenerationPeriod = UtilsService.getEnum(VR_Invoice_InvoiceGenerationPeriodEnum, 'value', VR_Invoice_InvoiceGenerationPeriodEnum.FollowBillingCycle.value);
                    $scope.scopeModel.issueDate = VRDateTimeService.getNowDateTime();

                    var loadAccountStatusSelectorDirectivePromise = loadAccountStatusSelectorDirective();
                    promises.push(loadAccountStatusSelectorDirectivePromise);

                    var loadPartnerGroupSelectorPromise = loadPartnerGroupSelector();
                    promises.push(loadPartnerGroupSelectorPromise);

                    var loadInvoiceTypePromise = loadInvoiceType();
                    promises.push(loadInvoiceTypePromise);

                    function loadInvoiceType() {
                        return VR_Invoice_InvoiceTypeAPIService.GetInvoiceType(invoiceTypeId).then(function (response) {
                            invoiceType = response;
                        });
                    }

                    function loadPartnerGroupSelector() {
                        var partnerGroupSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        UtilsService.waitMultiplePromises([invoicePartnerGroupReadyDeferred.promise, loadAccountStatusSelectorDirectivePromise]).then(function () {
                            var partnerGroupSelectorPaylod = { invoiceTypeId: invoiceTypeId, accountStatus: accountStatusSelectorAPI.getData() };
                            VRUIUtilsService.callDirectiveLoad(invoicePartnerGroupAPI, partnerGroupSelectorPaylod, partnerGroupSelectorLoadDeferred);
                        });

                        return partnerGroupSelectorLoadDeferred.promise;
                    }

                    function loadAccountStatusSelectorDirective() {
                        var accountStatusSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
                        accountStatusSelectorReadyDeferred.promise.then(function () {
                            var accountStatusSelectorPayload = {
                                selectFirstItem: true,
                                dontShowInActive: true
                            };

                            VRUIUtilsService.callDirectiveLoad(accountStatusSelectorAPI, accountStatusSelectorPayload, accountStatusSelectorPayloadLoadDeferred);
                        });
                        return accountStatusSelectorPayloadLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
