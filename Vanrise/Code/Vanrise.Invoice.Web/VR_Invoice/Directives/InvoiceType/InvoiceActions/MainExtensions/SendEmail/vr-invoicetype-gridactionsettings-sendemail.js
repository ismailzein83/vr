"use strict";

app.directive("vrInvoicetypeGridactionsettingsSendemail", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SendEmailAction($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceActions/MainExtensions/SendEmail/Templates/SendEmailActionTemplate.html"

        };

        function SendEmailAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainTypeSelectorAPI;
            var mainTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var invoiceAttachmentSelectorAPI;
            var invoiceAttachmentSelectorReadyDeferred = UtilsService.createPromiseDeferred();


            var gridAPI;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.infoType = "MainTemplate";
                $scope.onMainTypeSelectorReady = function (api) {
                    mainTypeSelectorAPI = api;
                    mainTypeSelectorReadyDeferred.resolve();
                };
                $scope.onInvoiceAttachmentSelectorReady = function (api) {
                    invoiceAttachmentSelectorAPI = api;
                    invoiceAttachmentSelectorReadyDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([mainTypeSelectorReadyDeferred.promise, invoiceAttachmentSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var invoiceActionEntity;
                    if (payload != undefined) {
                        invoiceActionEntity = payload.invoiceActionEntity;
                        context = payload.context;
                        if (invoiceActionEntity != undefined)
                         $scope.infoType = invoiceActionEntity.InfoType;
                    }
                   
                    var promises = [];
                    function loadMainTypeSelectorDirective() {
                        var mainTypeSelectorPayload;
                        if (invoiceActionEntity != undefined) {
                            mainTypeSelectorPayload = {
                                selectedIds: invoiceActionEntity.InvoiceMailTypeId
                            };
                        }
                        return mainTypeSelectorAPI.load(mainTypeSelectorPayload);
                    }
                    promises.push(loadMainTypeSelectorDirective());

                    function loadInvoiceAttachmentSelectorDirective() {
                        var invoiceAttachmentSelectorPayload = { context: getContext() };
                        if (invoiceActionEntity != undefined) {
                            invoiceAttachmentSelectorPayload.selectedIds  = invoiceActionEntity.AttachmentsIds;
                        }
                        return invoiceAttachmentSelectorAPI.load(invoiceAttachmentSelectorPayload);
                    }
                    promises.push(loadInvoiceAttachmentSelectorDirective());
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.SendEmailAction ,Vanrise.Invoice.MainExtensions",
                        InfoType: $scope.infoType,
                        InvoiceMailTypeId: mainTypeSelectorAPI.getSelectedIds(),
                        AttachmentsIds: invoiceAttachmentSelectorAPI.getSelectedIds(),
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