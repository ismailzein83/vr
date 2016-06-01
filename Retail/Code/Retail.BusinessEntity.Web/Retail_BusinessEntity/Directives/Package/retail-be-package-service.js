(function (app) {

    'use strict';

    RetailBePackageServiceDirective.$inject = ['Retail_BE_PackageAPIService', 'UtilsService', 'VRUIUtilsService'];

    function RetailBePackageServiceDirective(Retail_BE_PackageAPIService, UtilsService, VRUIUtilsService) {
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
            controllerAs: "serviceCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Service'";
            if (attrs.hidelabel != undefined) {
                label = "";
                withemptyline = '';
            }
            var template =
                '<vr-row>'
                 + '<vr-columns colnum="{{serviceCtrl.normalColNum}}">'
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

                + '<vr-columns colnum="{{serviceCtrl.normalColNum}}">'
                 + ' <vr-switch  label="Enabled" value="scopeModel.enabled">'
                     + '</vr-switch>'
                + ' </vr-columns>'
                + '</vr-row>'
            + '<vr-row>'
              + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig !=undefined" directive="scopeModel.selectedTemplateConfig.Editor" on-ready="scopeModel.onDirectiveReady" normal-col-num="{{serviceCtrl.normalColNum}}" isrequired="serviceCtrl.isrequired" customvalidate="serviceCtrl.customvalidate" type="serviceCtrl.type"></vr-directivewrapper>'
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
                var serviceEntity;
                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        if (payload.serviceEntity != undefined) {
                            serviceEntity = payload.serviceEntity;
                            $scope.scopeModel.enabled = serviceEntity.Enabled;
                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                            directiveReadyDeferred.promise.then(function () {
                                directiveReadyDeferred = undefined;
                                var payloadDirective = {
                                    serviceEntity: payload.serviceEntity
                                };
                                VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
                            });
                            promises.push(loadDirectivePromiseDeferred.promise);
                        }
                    }
                    var getServicesTemplateConfigsPromise = getServicesTemplateConfigs();
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
                            data.Enabled = $scope.scopeModel.enabled;
                        }
                    }
                    return data;
                }
                function getServicesTemplateConfigs() {
                    return Retail_BE_PackageAPIService.GetServicesTemplateConfigs().then(function (response) {
                        if (selectorAPI != undefined)
                            selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModel.templateConfigs.push(response[i]);
                            }
                            if (serviceEntity != undefined)
                                $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, serviceEntity.ConfigId, 'ExtensionConfigurationId');
                            //else
                            //$scope.selectedTemplateConfig = $scope.templateConfigs[0];
                        }
                    });
                }

            }
        }
    }

    app.directive('retailBePackageService', RetailBePackageServiceDirective);

})(app);