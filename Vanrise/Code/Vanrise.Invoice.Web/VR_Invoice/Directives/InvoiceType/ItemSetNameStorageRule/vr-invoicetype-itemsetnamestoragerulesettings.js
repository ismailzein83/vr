﻿(function (app) {

    'use strict';

    ItemSetNameStorageRuleSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceTypeConfigsAPIService'];

    function ItemSetNameStorageRuleSettingsDirective(UtilsService, VRUIUtilsService, VR_Invoice_InvoiceTypeConfigsAPIService) {
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
                var ctor = new ItemSetNameStorageRuleSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function ItemSetNameStorageRuleSettings($scope, ctrl, $attrs) {
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
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};
                var extendedSettings;

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var itemSetNameStorageRuleSettings;

                    if (payload != undefined) {
                        itemSetNameStorageRuleSettings = payload.itemSetNameStorageRuleSettings;
                    }

                    if (itemSetNameStorageRuleSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getItemSetNameStorageRuleConfigsPromise = getItemSetNameStorageRuleConfigs();
                    promises.push(getItemSetNameStorageRuleConfigsPromise);

                    function getItemSetNameStorageRuleConfigs() {
                        return VR_Invoice_InvoiceTypeConfigsAPIService.GetItemSetNameStorageRuleConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (itemSetNameStorageRuleSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, itemSetNameStorageRuleSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { itemSetNameStorageRuleSettings: itemSetNameStorageRuleSettings };
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
                            + 'label="Type"'

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

    app.directive('vrInvoicetypeItemsetnamestoragerulesettings', ItemSetNameStorageRuleSettingsDirective);

})(app);