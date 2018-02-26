(function (app) {

    'use strict';

    ActionSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceTypeConfigsAPIService'];

    function ActionSettingsDirective(UtilsService, VRUIUtilsService, VR_Invoice_InvoiceTypeConfigsAPIService) {
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
                var ctor = new ActionSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function ActionSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;
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
                    if (context != undefined && context.showSecurityGridCallBack != undefined && typeof (context.showSecurityGridCallBack) == 'function')
                        context.showSecurityGridCallBack(true);
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var payload = { context: getContext() };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, payload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};
                var serviceSettings;

                api.load = function (payload) {
                   
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var invoiceActionEntity;

                    if (payload != undefined) {
                        invoiceActionEntity = payload.invoiceActionEntity;
                        context = payload.context;
                    }

                    if (invoiceActionEntity != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getInvoiceGeneratorConfigsPromise = getInvoiceGeneratorTemplateConfigs();
                    promises.push(getInvoiceGeneratorConfigsPromise);

                    function getInvoiceGeneratorTemplateConfigs() {
                        return VR_Invoice_InvoiceTypeConfigsAPIService.GetInvoiceActionSettingsConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (invoiceActionEntity != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, invoiceActionEntity.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { context: getContext(), invoiceActionEntity: invoiceActionEntity };
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
                            data.ActionId = UtilsService.guid();

                        }
                    }
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
            function getContext()
            {
                var currectContext = context;
                if (currectContext == undefined)
                    currectContext = {};
                return currectContext;
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
                            + 'label="Action Type"'

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

    app.directive('vrInvoicetypeActionsettings', ActionSettingsDirective);

})(app);