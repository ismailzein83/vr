(function (app) {

    'use strict';

    RetailBeSwitchSettingsDirective.$inject = ['Retail_BE_SwitchAPIService', 'UtilsService', 'VRUIUtilsService'];

    function RetailBeSwitchSettingsDirective(Retail_BE_SwitchAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                type: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var retailBePackageService = new RetailBePackageService($scope, ctrl, $attrs);
                retailBePackageService.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Settings'";
            if (attrs.hidelabel != undefined) {
                label = "";
                withemptyline = '';
            }
            var template =
                '<vr-row>'
                 + '<vr-columns colnum="{{ctrl.normalColNum}}">'
                 + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                 + ' datasource="scopeModel.templateConfigs"'
                 + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                 + 'datavaluefield="ExtensionConfigurationId"'
                 + ' datatextfield="Title"'
                 + label
                 + ' isrequired="true"'
                 + 'hideremoveicon>'
                 + '</vr-select>'
                 + ' </vr-columns>'
                 + '</vr-row>'
            + '<vr-row>'
              + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig !=undefined" directive="scopeModel.selectedTemplateConfig.Editor" on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate" type="ctrl.type"></vr-directivewrapper>'
            + '</vr-row>';
            return template;

        }
        function RetailBePackageService($scope, ctrl, $attrs) {
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
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };

                defineAPI();

            }

            function defineAPI() {
                var api = {};
                var serviceSettings;
                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        if (payload.serviceSettings != undefined) {
                            serviceSettings = payload.serviceSettings;
                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                            directiveReadyDeferred.promise.then(function () {
                                directiveReadyDeferred = undefined;
                                var payloadDirective = {
                                    serviceSettings: payload.serviceSettings
                                };
                                VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
                            });
                            promises.push(loadDirectivePromiseDeferred.promise);
                        }
                    }
                    var getSwitchSettingsTemplateConfigsPromise = getSwitchSettingsTemplateConfigs();
                    promises.push(getServicesTemplateConfigsPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                    }
                    return data;
                }

                function getSwitchSettingsTemplateConfigs() {
                    return Retail_BE_SwitchAPIService.GetSwitchSettingsTemplateConfigs().then(function (response) {
                        if (selectorAPI != undefined)
                            selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModel.templateConfigs.push(response[i]);
                            }
                            if (serviceSettings != undefined)
                                $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, serviceSettings.ConfigId, 'ExtensionConfigurationId');
                            //else
                            //$scope.selectedTemplateConfig = $scope.templateConfigs[0];
                        }
                    });
                }

            }
        }
    }

    app.directive('retailBeSwitchSettings', RetailBeSwitchSettingsDirective);

})(app);