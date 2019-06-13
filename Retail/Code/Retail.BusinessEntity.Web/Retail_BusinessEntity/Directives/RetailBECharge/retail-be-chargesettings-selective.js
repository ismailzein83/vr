(function (app) {

    'use strict';

    RetailBEChargeSettingsSelectiveDirective.$inject = ['Retail_BE_RetailBEChargeAPIService', 'UtilsService', 'VRUIUtilsService'];

    function RetailBEChargeSettingsSelectiveDirective(Retail_BE_RetailBEChargeAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '=',
                isrequired:'='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RetailBEChargeSettingsSelectiveDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (attrs) {
                return getTemplate(attrs);
            }
        };

        function RetailBEChargeSettingsSelectiveDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

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

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var retailBEChargeSettings;
                    if (payload != undefined) {
                        retailBEChargeSettings = payload.settings;
                        $scope.scopeModel.label = payload.title;
                    }

                    function getRetailBEChargeSettingsConfigs() {
                        return Retail_BE_RetailBEChargeAPIService.GetRetailBEChargeSettingsConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (retailBEChargeSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, retailBEChargeSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { retailBEChargeSettings: retailBEChargeSettings };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }
                    if (retailBEChargeSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    promises.push(getRetailBEChargeSettingsConfigs());


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

        function getTemplate(attrs) {
            var template = ' <vr-columns colnum="{{ctrl.normalColNum}}">'
                +'<vr-label>{{scopeModel.label}}</vr-label>'
                + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                + ' datasource="scopeModel.templateConfigs"'
                + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                + ' datavaluefield="ExtensionConfigurationId"'
                + ' datatextfield="Title"'
                + ' isrequired="ctrl.isrequired">'
                + '</vr-select>'
                + ' </vr-columns>'
                + ' <vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor" vr-loader="scopeModel.isLoadingDirective"'
                + ' on-ready="scopeModel.onDirectiveReady" isrequired="ctrl.isrequired" normal-col-num="{{ctrl.normalColNum}}" customvalidate="ctrl.customvalidate">'
                + ' </vr-directivewrapper>';
            return template;
        }
    }

    app.directive('retailBeChargesettingsSelective', RetailBEChargeSettingsSelectiveDirective);

})(app);