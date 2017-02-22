"use strict";

app.directive("partnerportalInvoiceInvoiceViewertypesettings", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new InvoiceViewerTypeSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/PartnerPortal_Invoice/Elements/Invoice/Directives/Templates/InvoiceViewerTypeSettings.html"
        };
        function InvoiceViewerTypeSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var connectionSelectorApi;
            var connectionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var selectedConnectionSelectorPromiseDeferred;

            var invoiceTypeSelectorApi;
            var invoiceTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();


            var invoiceQueryInterceptorApi;
            var invoiceQueryInterceptorPromiseDeferred = UtilsService.createPromiseDeferred();

            var gridColumnsAPI;
            var gridColumnsSectionReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onConnectionSelectorReady = function (api) {
                    connectionSelectorApi = api;
                    connectionSelectorPromiseDeferred.resolve();
                };
                $scope.scopeModel.onGridColumnsReady = function (api) {
                    gridColumnsAPI = api;
                    gridColumnsSectionReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onInvoiceTypeSelectorReady = function (api) {
                    invoiceTypeSelectorApi = api;
                    invoiceTypeSelectorPromiseDeferred.resolve();
                };

                $scope.scopeModel.onInvoiceQueryInterceptorReady = function (api) {
                    invoiceQueryInterceptorApi = api;
                    invoiceQueryInterceptorPromiseDeferred.resolve();
                };
                $scope.scopeModel.onConnectionSelectionChanged = function (value) {
                    if(value != undefined)
                    {
                        if (selectedConnectionSelectorPromiseDeferred != undefined)
                            selectedConnectionSelectorPromiseDeferred.resolve();
                        else
                        {
                            var setLoader = function (value) {
                                $scope.scopeModel.isLoadingDirective = value;
                            };
                            var payload = { connectionId: connectionSelectorApi.getSelectedIds() };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, invoiceTypeSelectorApi, payload, setLoader);

                            var setLoader = function (value) {
                                $scope.scopeModel.isLoadingDirective = value;
                            };
                            var payload = { context: getContext() };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridColumnsAPI, payload, setLoader);
                        }
                        
                    }
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var invoiceViewerTypeSettings
                    if (payload != undefined) {
                        invoiceViewerTypeSettings = payload.componentType;
                        if (invoiceViewerTypeSettings != undefined)
                        {
                            $scope.scopeModel.name = invoiceViewerTypeSettings.Name;
                            selectedConnectionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        }
                    }

                    promises.push(loadConnectionSelector());
                    if (invoiceViewerTypeSettings != undefined && invoiceViewerTypeSettings.Settings != undefined) {
                        promises.push(loadInvoiceTypeSelector());
                        promises.push(loadGridSettingsSection());
                    }
                    promises.push(loadInvoiceQueryInterceptor());
                 

                    function loadConnectionSelector()
                    {
                        var connectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        connectionSelectorPromiseDeferred.promise.then(function () {
                            var payloadSelector = {
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.Common.Business.VRInterAppRestConnectionFilter, Vanrise.Common.Business"
                                    }]
                                }
                            };
                            if (invoiceViewerTypeSettings != undefined && invoiceViewerTypeSettings.Settings != undefined) {
                                payloadSelector.selectedIds = invoiceViewerTypeSettings.Settings.VRConnectionId;
                            };
                            VRUIUtilsService.callDirectiveLoad(connectionSelectorApi, payloadSelector, connectionSelectorLoadDeferred);
                        });
                        return connectionSelectorLoadDeferred.promise;
                    }
                    function loadInvoiceTypeSelector() {
                            var invoiceTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                            UtilsService.waitMultiplePromises([selectedConnectionSelectorPromiseDeferred.promise, invoiceTypeSelectorPromiseDeferred.promise]).then(function () {
                                var payloadSelector = {
                                    connectionId: invoiceViewerTypeSettings.Settings.VRConnectionId,
                                    selectedIds:  invoiceViewerTypeSettings.Settings.InvoiceTypeId
                                };
                                VRUIUtilsService.callDirectiveLoad(invoiceTypeSelectorApi, payloadSelector, invoiceTypeSelectorLoadDeferred);
                            });
                            return invoiceTypeSelectorLoadDeferred.promise;
                    }
                    function loadInvoiceQueryInterceptor() {
                        var invoiceQueryInterceptorLoadDeferred = UtilsService.createPromiseDeferred();
                        invoiceQueryInterceptorPromiseDeferred.promise.then(function () {
                            var invoiceQueryInterceptorPayload;
                            if (invoiceViewerTypeSettings != undefined && invoiceViewerTypeSettings.Settings != undefined) {
                                invoiceQueryInterceptorPayload = { invoiceQueryInterceptorEntity: invoiceViewerTypeSettings.Settings.InvoiceQueryInterceptor };
                            };
                            VRUIUtilsService.callDirectiveLoad(invoiceQueryInterceptorApi, invoiceQueryInterceptorPayload, invoiceQueryInterceptorLoadDeferred);
                        });
                        return invoiceQueryInterceptorLoadDeferred.promise;
                    }
                    function loadGridSettingsSection() {
                        var gridColumnsSectionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([selectedConnectionSelectorPromiseDeferred.promise, gridColumnsSectionReadyPromiseDeferred.promise]).then(function () {
                            var gridColumnsPayload = { context: getContext() };
                            if (invoiceViewerTypeSettings != undefined && invoiceViewerTypeSettings.Settings && invoiceViewerTypeSettings.Settings.GridSettings != undefined) {
                                gridColumnsPayload.gridColumns = invoiceViewerTypeSettings.Settings.GridSettings.InvoiceGridFields;
                            }
                            VRUIUtilsService.callDirectiveLoad(gridColumnsAPI, gridColumnsPayload, gridColumnsSectionLoadPromiseDeferred);
                        });
                        return gridColumnsSectionLoadPromiseDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        selectedConnectionSelectorPromiseDeferred = undefined;
                    });
                };

                api.getData = function () {
                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "PartnerPortal.Invoice.Business.Extensions.InvoiceViewerTypeSettings, PartnerPortal.Invoice.Business",
                            VRConnectionId: connectionSelectorApi.getSelectedIds(),
                            InvoiceTypeId: invoiceTypeSelectorApi.getSelectedIds(),
                            InvoiceQueryInterceptor: invoiceQueryInterceptorApi.getData(),
                            GridSettings:{
                                InvoiceGridFields: gridColumnsAPI.getData(),
                            }
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };
            function getContext()
            {
                var context = {
                    getConnectionId: function () {
                        return connectionSelectorApi.getSelectedIds();
                    },
                    getInvoiceTypeId:function()
                    {
                        return invoiceTypeSelectorApi.getSelectedIds();
                    }
                };
                return context;
            }

        };

        return directiveDefinitionObject;
    }
]);