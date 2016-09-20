(function (app) {

    'use strict';

    OutputpricelistconfigurationSelective.$inject = ['XBooster_PriceListConversion_PriceListTemplateAPIService', 'UtilsService', 'VRUIUtilsService'];

    function OutputpricelistconfigurationSelective(XBooster_PriceListConversion_PriceListTemplateAPIService, UtilsService, VRUIUtilsService) {
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
                var outputpricelistconfiguration = new Outputpricelistconfiguration($scope, ctrl, $attrs);
                outputpricelistconfiguration.initializeController();
            },
            controllerAs: "fieldmappingCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Type'";
            if (attrs.hidelabel != undefined) {
                label = "";
                withemptyline = '';
            }


            var template =
                '<vr-row  ng-show="templateConfigs.length>1">'
              + '<vr-columns colnum="{{fieldmappingCtrl.normalColNum * 2}}">'
              + ' <vr-select on-ready="onSelectorReady"'
              + ' datasource="templateConfigs"'
              + ' selectedvalues="selectedTemplateConfig"'
               + 'datavaluefield="ExtensionConfigurationId"'
              + ' datatextfield="Title"'
              + label
               + ' isrequired="fieldmappingCtrl.isrequired"'
              + 'hideremoveicon>'
          + '</vr-select></vr-columns>'
           + '</vr-row>'
          + '<vr-row >'
                        + '<vr-directivewrapper directive="selectedTemplateConfig.Editor" on-ready="onDirectiveReady" normal-col-num="{{fieldmappingCtrl.normalColNum}}" isrequired="fieldmappingCtrl.isrequired" customvalidate="fieldmappingCtrl.customvalidate" type="fieldmappingCtrl.type"></vr-directivewrapper>';
          +'</vr-row>'
            return template;

        }

        function Outputpricelistconfiguration($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;

            function initializeController() {
                $scope.templateConfigs = [];

                $scope.selectedTemplateConfig;

                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directivePayload;
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
                    var configDetails;
                    if (payload != undefined) {
                        configDetails = payload.configDetails;
                        if(configDetails !=undefined)
                        {
                            directiveReadyDeferred  = UtilsService.createPromiseDeferred();
                            var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                            directiveReadyDeferred.promise.then(function () {
                                var payload = {
                                    configDetails: configDetails,
                                };
                                VRUIUtilsService.callDirectiveLoad(directiveAPI, payload, loadDirectivePromiseDeferred);
                            });
                            promises.push(loadDirectivePromiseDeferred.promise);
                          
                        }
                    }
                    var getOutputPriceListConfigurationPromise = getOutputPriceListConfiguration();
                    promises.push(getOutputPriceListConfigurationPromise);

                    return UtilsService.waitMultiplePromises(promises);

                    function getOutputPriceListConfiguration() {
                        return XBooster_PriceListConversion_PriceListTemplateAPIService.GetOutputPriceListConfigurationTemplateConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.templateConfigs.push(response[i]);
                                }
                                if (configDetails != undefined)
                                {
                                    $scope.selectedTemplateConfig = UtilsService.getItemByVal($scope.templateConfigs, configDetails.ConfigId, 'ExtensionConfigurationId');
                                } else if ($scope.templateConfigs.length == 1)
                                    $scope.selectedTemplateConfig = $scope.templateConfigs[0];
                            }
                        });
                    }


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
        }
    }

    app.directive('xboosterPricelistconversionOutputpricelistconfigurationSelective', OutputpricelistconfigurationSelective);

})(app);