"use strict";
app.directive("partnerportalInvoiceVieweditor", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ViewEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/PartnerPortal_Invoice/Elements/Invoice/Directives/Templates/InvoiceViewEditor.html"
        };
        function ViewEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var invoiceViewerTypeApi;
            var invoiceViewerTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onInvoiceViewerTypeSelectorReady = function (api) {
                    invoiceViewerTypeApi = api;
                    invoiceViewerTypeSelectorPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var invoiceViewItems;
                    var invoiceViewerTypeIds
                    if (payload != undefined) {
                        invoiceViewItems = payload.InvoiceViewItems;
                        if(invoiceViewItems != undefined)
                        {
                            invoiceViewerTypeIds = [];
                            for (var i = 0 ; i < invoiceViewItems.length; i++) {
                                var item = invoiceViewItems[i];
                                invoiceViewerTypeIds.push(item.InvoiceViewerTypeId);
                            }
                        }
                    }

                    var invoiceViewerTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    invoiceViewerTypeSelectorPromiseDeferred.promise.then(function () {
                        var payloadSelector = {
                        };
                        if (invoiceViewerTypeIds != undefined) {
                            payloadSelector.selectedIds = invoiceViewerTypeIds;
                        };
                        VRUIUtilsService.callDirectiveLoad(invoiceViewerTypeApi, payloadSelector, invoiceViewerTypeSelectorLoadDeferred);
                    });
                    promises.push(invoiceViewerTypeSelectorLoadDeferred.promise);


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var invoiceViewerTypeIds = invoiceViewerTypeApi.getSelectedIds();
                    var invoiceViewItems;
                    if (invoiceViewerTypeIds != undefined)
                    {
                        invoiceViewItems = [];
                        for (var i = 0 ; i < invoiceViewerTypeIds.length; i++) {
                            invoiceViewItems.push({
                                InvoiceViewerTypeId: invoiceViewerTypeIds[i]
                            });
                        }
                    }
                    return {
                        $type: "PartnerPortal.Invoice.Entities.InvoiceViewSettings, PartnerPortal.Invoice.Entities",
                        InvoiceViewItems: invoiceViewItems
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };


        };

        return directiveDefinitionObject;
    }
]);