(function (app) {

    'use strict';

    PackageitemDirective.$inject = ['Retail_BE_PackageAPIService', 'UtilsService', 'VRUIUtilsService'];

    function PackageitemDirective(Retail_BE_PackageAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var packageItem = new PackageItem($scope, ctrl, $attrs);
                packageItem.initializeController();
            },
            controllerAs: "packageItemCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Package Item'";
            if (attrs.hidelabel != undefined) {
                label = "";
                withemptyline = '';
            }
            var template =
                '<vr-row>'
                 + '<vr-columns colnum="{{packageItemCtrl.normalColNum}}">'
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
                 + ' </vr-columns>'
                 + '</vr-row>'
            + '<vr-row>'
              + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig !=undefined" directive="scopeModel.selectedTemplateConfig.Editor" on-ready="scopeModel.onDirectiveReady" normal-col-num="{{packageItemCtrl.normalColNum}}" isrequired="packageItemCtrl.isrequired" customvalidate="packageItemCtrl.customvalidate"></vr-directivewrapper>'
            + '</vr-row>';
            return template;

        }
        function PackageItem($scope, ctrl, $attrs) {
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
                    var getPackageItemsTemplateConfigsPromise = getPackageItemsTemplateConfigs();
                    promises.push(getPackageItemsTemplateConfigsPromise);

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
                function getPackageItemsTemplateConfigs() {
                    return Retail_BE_PackageAPIService.GetServicePackageItemConfigs().then(function (response) {
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

    app.directive('retailBePackagePackageitem', PackageitemDirective);

})(app);