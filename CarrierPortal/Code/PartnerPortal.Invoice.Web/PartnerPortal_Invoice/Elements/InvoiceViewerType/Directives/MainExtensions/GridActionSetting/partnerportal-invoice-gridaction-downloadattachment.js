"use strict";

app.directive("partnerportalInvoiceGridactionDownloadattachment", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new DownloadAttachmentAction($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/PartnerPortal_Invoice/Elements/InvoiceViewerType/Directives/MainExtensions/GridActionSetting/Templates/DownloadAttachmentAction.html"

        };

        function DownloadAttachmentAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var invoiceAttachmentSelectorApi;
            var invoiceAttachmentSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onInvoiceAttachmentSelectorReady = function (api) {
                    invoiceAttachmentSelectorApi = api;
                    invoiceAttachmentSelectorPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var gridActionEntity;
                    if (payload != undefined) {
                        gridActionEntity = payload.gridActionEntity;
                        context = payload.context;
                    }
                    var promises = [];
                    promises.push(loadInvoiceAttachmentSelector());

                    function loadInvoiceAttachmentSelector() {
                        var invoiceAttachmentSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        invoiceAttachmentSelectorPromiseDeferred.promise.then(function () {
                            var payloadSelector = {
                                connectionId: context != undefined ? context.getConnectionId() : undefined,
                                invoiceTypeId: context != undefined ? context.getInvoiceTypeId() : undefined,
                                selectedIds: gridActionEntity != undefined ? gridActionEntity.InvoiceAttachmentId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(invoiceAttachmentSelectorApi, payloadSelector, invoiceAttachmentSelectorLoadDeferred);
                        });
                        return invoiceAttachmentSelectorLoadDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "PartnerPortal.Invoice.MainExtensions.InvoiceViewerTypeGridAction.DownloadAttachmentAction ,PartnerPortal.Invoice.MainExtensions",
                        InvoiceAttachmentId: invoiceAttachmentSelectorApi.getSelectedIds()
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