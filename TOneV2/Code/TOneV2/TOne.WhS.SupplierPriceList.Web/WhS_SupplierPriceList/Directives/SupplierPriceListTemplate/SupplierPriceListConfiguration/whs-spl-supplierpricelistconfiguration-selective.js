(function (app) {

    'use strict';

    SupplierpricelistconfigurationSelective.$inject = ['WhS_SupPL_SupplierPriceListTemplateAPIService', 'UtilsService', 'VRUIUtilsService'];

    function SupplierpricelistconfigurationSelective(WhS_SupPL_SupplierPriceListTemplateAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                label: '@',
                customvalidate: '=',
                type: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var supplierpricelistconfiguration = new Supplierpricelistconfiguration($scope, ctrl, $attrs);
                supplierpricelistconfiguration.initializeController();
            },
            controllerAs: "fieldmappingCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Supplier Configuration'";
            if (attrs.hidelabel != undefined) {
                label = "";
                withemptyline = '';
            }


            var template =
                '<vr-row ng-show="showConfigurationSelector">'
              + '<vr-columns colnum="{{fieldmappingCtrl.normalColNum * 2}}" >'
              + ' <vr-select on-ready="onSelectorReady"'
              + ' datasource="templateConfigs"'
              + ' selectedvalues="selectedTemplateConfig"'
               + 'datavaluefield="ExtensionConfigurationId"'
              + ' datatextfield="Title"'
              + label
               + ' isrequired="fieldmappingCtrl.isrequired"'
              + 'hideremoveicon>'
          + '</vr-select>'
           + '</vr-row>'
              + '<vr-directivewrapper directive="selectedTemplateConfig.Editor" vr-loader="isLoadingDirective" on-ready="onDirectiveReady" normal-col-num="{{fieldmappingCtrl.normalColNum}}" isrequired="fieldmappingCtrl.isrequired" customvalidate="fieldmappingCtrl.customvalidate" type="fieldmappingCtrl.type"></vr-directivewrapper>';
            return template;

        }
        function Supplierpricelistconfiguration($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var selectorAPI;
            var context;
            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();
            var directivePayload;
            var configDetails;
            function initializeController() {
                $scope.templateConfigs = [];
                $scope.showConfigurationSelector = false;
                $scope.selectedTemplateConfig;

                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directivePayload = {
                        context: getContext(),
                        configDetails: configDetails
                    };
                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var getSupplierPriceListConfigurationPromise = getSupplierPriceListConfiguration();
                    promises.push(getSupplierPriceListConfigurationPromise);
                    if (payload != undefined) {
                        context = payload.context;
                        configDetails = payload.configDetails;
                        var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        directiveReadyDeferred.promise.then(function () {
                            var payload = {
                                context: getContext()
                            };
                            if (configDetails != undefined)
                                payload.configDetails = configDetails;
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, payload, loadDirectivePromiseDeferred);
                        });
                        promises.push(loadDirectivePromiseDeferred.promise);
                    }

                    function getSupplierPriceListConfiguration() {
                        return WhS_SupPL_SupplierPriceListTemplateAPIService.GetSupplierPriceListConfigurationTemplateConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.templateConfigs.push(response[i]);
                                }

                                if (configDetails != undefined) {
                                    $scope.selectedTemplateConfig = UtilsService.getItemByVal($scope.templateConfigs, configDetails.ConfigId, 'ExtensionConfigurationId');
                                } else if ($scope.templateConfigs.length == 1)
                                    $scope.selectedTemplateConfig = $scope.templateConfigs[0];
                                if ($scope.templateConfigs.length > 1) {
                                    $scope.showConfigurationSelector = true;
                                }
                            }
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data;
                    if ($scope.selectedTemplateConfig != undefined && directiveAPI != undefined) {
                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                    }
                    return data;
                }
            }

            function getContext() {

                if (context != undefined) {
                    var currentContext = UtilsService.cloneObject(context);
                    return currentContext;
                }
            }
        }
    }

    app.directive('whsSplSupplierpricelistconfigurationSelective', SupplierpricelistconfigurationSelective);

})(app);