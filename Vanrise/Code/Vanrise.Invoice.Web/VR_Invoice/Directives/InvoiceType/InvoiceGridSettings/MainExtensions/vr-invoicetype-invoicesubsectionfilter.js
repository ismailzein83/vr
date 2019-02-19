(function (app) {

    'use strict';

    InvoiceSubsectionFilterDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceTypeConfigsAPIService'];

    function InvoiceSubsectionFilterDirective(UtilsService, VRUIUtilsService, VR_Invoice_InvoiceTypeConfigsAPIService) {
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
                var ctor = new InvoiceUIGridColumnFilter($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function InvoiceUIGridColumnFilter($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var context;
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
                    var directivePayload = { context: getContext() };

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};
                var serviceSettings;

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var invoiceSubsectionFilterEntity;

                    if (payload != undefined) {
                        invoiceSubsectionFilterEntity = payload.invoiceSubsectionFilterEntity;
                    }

                    if (invoiceSubsectionFilterEntity != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getInvoiceGeneratorConfigsPromise = getInvoiceGeneratorTemplateConfigs();
                    promises.push(getInvoiceGeneratorConfigsPromise);

                    function getInvoiceGeneratorTemplateConfigs() {
                        return VR_Invoice_InvoiceTypeConfigsAPIService.GetInvoiceSubSectionFilterConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (invoiceSubsectionFilterEntity != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, invoiceSubsectionFilterEntity.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { context: getContext(), invoiceSubsectionFilterEntity: invoiceSubsectionFilterEntity };
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
            function getContext() {
                return context;
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
                + 'label="Condition"'

                + ' isrequired="ctrl.isrequired">'
                + '</vr-select>'
                + ' </vr-columns>'
                + '</vr-row>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate">'
                + '</vr-directivewrapper>';
            return template;
        }
    }

    app.directive('vrInvoicetypeInvoicesubsectionfilter', InvoiceSubsectionFilterDirective);

})(app);