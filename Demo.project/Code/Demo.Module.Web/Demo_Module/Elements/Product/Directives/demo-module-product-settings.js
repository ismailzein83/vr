(function (app) {

    'use strict';

    productSettingsDirective.$inject = ['UtilsService', 'Demo_Module_ProductAPIService', 'VRUIUtilsService'];

    function productSettingsDirective(UtilsService, Demo_Module_ProductAPIService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope) {
                var ctrl = this;
                var ctor = new productSettingsCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function productSettingsCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];

                $scope.scopeModel.onSettingsSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoading = value; //doing nothing
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var settings;

                    if (payload != undefined) {
                        settings = payload.Settings;
                    }

                    if (settings != undefined) {
                        $scope.scopeModel.price = settings.Price;
                        var loadDirectivePromise = loadDirective(); //load current template if it exists
                        promises.push(loadDirectivePromise);
                    }

                    var productSettingsConfigsPromise = getProductSettingsConfigs(); //load templates and set the selector at the current template if it exists
                    promises.push(productSettingsConfigsPromise);

                    function getProductSettingsConfigs() {
                        return Demo_Module_ProductAPIService.GetProductSettingsConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                            }

                            if (settings != undefined) {
                                $scope.scopeModel.selectedTemplateConfig =
                                    UtilsService.getItemByVal($scope.scopeModel.templateConfigs, settings.ConfigId, 'ExtensionConfigurationId');
                            }
                        });
                    }

                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = {
                                Settings: settings
                            };
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
                            data.Price = $scope.scopeModel.price;
                        }
                    }

                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }

        function getTemplate(attrs) {
            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined) {
                hideremoveicon = 'hideremoveicon';
            }

            return ' <vr-row> '
                + ' <vr-columns colnum={{ctrl.normalColNum}}> '

                + ' <vr-textbox value=scopeModel.price label="Price ($)" type="number" isrequired></vr-textbox> '

                + ' </vr-columns> '
                + ' <vr-columns colnum="{{ctrl.normalColNum}}"> '

                + ' <vr-select '
                + ' on-ready="scopeModel.onSettingsSelectorReady" '
                + ' datasource="scopeModel.templateConfigs" '
                + ' selectedvalues="scopeModel.selectedTemplateConfig" '
                + ' datavaluefield="ExtensionConfigurationId" '
                + ' datatextfield="Title" '
                + ' label="Product Settings" '
                + ' entityName="Product Settings" '
                + ' isrequired="ctrl.isrequired" '
                + ' ' + hideremoveicon + '> '
                + ' </vr-select> '

                + ' </vr-columns> '
                + ' </vr-row> '

                + ' <vr-directivewrapper '
                + ' ng-if="scopeModel.selectedTemplateConfig != undefined" '
                + ' directive="scopeModel.selectedTemplateConfig.Editor" '
                + ' on-ready=scopeModel.onDirectiveReady '
                + ' normal-col-num="{{ctrl.normalColNum}}" '
                + ' isrequired="ctrl.isrequired" '
                + ' customvalidate="ctrl.customvalidate" >'
                + ' </vr-directivewrapper> ';
        }
    }

    app.directive('demoModuleProductSettings', productSettingsDirective);

})(app);