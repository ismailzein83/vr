﻿(function (app) {

    'use strict';

    InvoiceQueryInterceptorDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'PartnerPortal_Invoice_InvoiceAPIService'];

    function InvoiceQueryInterceptorDirective(UtilsService, VRUIUtilsService, PartnerPortal_Invoice_InvoiceAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new InvoiceQueryInterceptor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function InvoiceQueryInterceptor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var payload;
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, payload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};
                var serviceSettings;

                api.load = function (payload) {

                    selectorAPI.clearDataSource();

                    var promises = [];
                    var invoiceQueryInterceptorEntity;

                    if (payload != undefined) {
                        invoiceQueryInterceptorEntity = payload.invoiceQueryInterceptorEntity;
                    }

                    if (invoiceQueryInterceptorEntity != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getInvoiceQueryInterceptorTemplatesConfigsPromise = getInvoiceQueryInterceptorTemplatesConfigs();
                    promises.push(getInvoiceQueryInterceptorTemplatesConfigsPromise);

                    function getInvoiceQueryInterceptorTemplatesConfigs() {
                        return PartnerPortal_Invoice_InvoiceAPIService.GetInvoiceQueryInterceptorTemplates().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (invoiceQueryInterceptorEntity != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, invoiceQueryInterceptorEntity.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = {invoiceQueryInterceptorEntity: invoiceQueryInterceptorEntity };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                    }
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
           
        }

        function getTamplate(attrs) {

            var template =
                '<vr-row>'
                    + '<vr-columns colnum="{{ctrl.normalColNum}}">'
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="ExtensionConfigurationId"'
                            + ' datatextfield="Title"'
                            + 'label="Invoice Query Interceptor"'

                            + ' isrequired="true"'
                            + 'hideremoveicon>'
                        + '</vr-select>'
                    + ' </vr-columns>'
                + '</vr-row>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                        + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate">'
                + '</vr-directivewrapper>';
            return template;
        }
    }

    app.directive('partnerportalInvoiceInvoicequeryinterceptor', InvoiceQueryInterceptorDirective);

})(app);