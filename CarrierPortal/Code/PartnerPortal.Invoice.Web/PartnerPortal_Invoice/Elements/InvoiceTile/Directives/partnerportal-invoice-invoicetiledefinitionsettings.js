"use strict";
app.directive("partnerportalInvoiceInvoicetiledefinitionsettings", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new InvoiceTileDefinitionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/PartnerPortal_Invoice/Elements/InvoiceTile/Directives/Templates/InvoiceTileDefinitionSettings.html"
        };
        function InvoiceTileDefinitionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var connectionSelectorApi;
            var connectionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var selectedConnectionPromiseDeffered;
           
            var invoiceTypeApi;
            var invoiceTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onInvoiceTypeSelectorReady = function (api) {
                    invoiceTypeApi = api;
                    invoiceTypeSelectorPromiseDeferred.resolve();
                };
                $scope.scopeModel.onConnectionSelectorReady = function (api) {
                    connectionSelectorApi = api;
                    connectionSelectorPromiseDeferred.resolve();
                };
                $scope.scopeModel.onConnectionSelectionChanged = function (value) {
                    if(value != undefined)
                    {
                        var setLoader = function (value) {
                            $scope.scopeModel.isDirectiveLoading = value;
                        };
                        var invoiceTypeSelectorPayload = { connectionId: connectionSelectorApi.getSelectedIds() };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, invoiceTypeApi, invoiceTypeSelectorPayload, setLoader, selectedConnectionPromiseDeffered);
                    }
                };
                UtilsService.waitMultiplePromises([invoiceTypeSelectorPromiseDeferred.promise, connectionSelectorPromiseDeferred.promise]).then(function(){
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var tileExtendedSettings;
                    var invoiceViewerTypeIds;
                    if (payload != undefined) {
                        tileExtendedSettings = payload.tileExtendedSettings;
                    }

                    function loadInvoiceSelector()
                    {
                        var payloadSelector = {
                            selectedIds :tileExtendedSettings.InvoiceTypeId,
                            connectionId : tileExtendedSettings.VRConnectionId
                        };
                        return invoiceTypeApi.load(payloadSelector);
                    }

                    if (tileExtendedSettings != undefined)
                    {
                        selectedConnectionPromiseDeffered = UtilsService.createPromiseDeferred();
                        promises.push(loadInvoiceSelector());
                    }
                      
                    function loadConnectionSelector()
                    {
                        var payloadConnectionSelector = {
                            filter: { Filters: [] }
                        };
                        payloadConnectionSelector.filter.Filters.push({
                            $type: "Vanrise.Common.Business.VRInterAppRestConnectionFilter, Vanrise.Common.Business"
                        });
                        if (tileExtendedSettings != undefined) {
                            payloadConnectionSelector.selectedIds = tileExtendedSettings.VRConnectionId;
                        };
                        return connectionSelectorApi.load(payloadConnectionSelector);

                    }
                 
                    promises.push(loadConnectionSelector());

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        selectedConnectionPromiseDeffered = undefined;
                    });
                };

                api.getData = function () {
                    return {
                        $type: "PartnerPortal.Invoice.Business.InvoiceTileDefinitionSettings, PartnerPortal.Invoice.Business",
                        VRConnectionId: connectionSelectorApi.getSelectedIds(),
                        InvoiceTypeId: invoiceTypeApi.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };


        };

        return directiveDefinitionObject;
    }
]);