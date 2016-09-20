(function (app) {

    'use strict';

    OutputfieldmappingSelective.$inject = ['XBooster_PriceListConversion_PriceListTemplateAPIService', 'UtilsService', 'VRUIUtilsService'];

    function OutputfieldmappingSelective(XBooster_PriceListConversion_PriceListTemplateAPIService, UtilsService, VRUIUtilsService) {
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
                var outputfieldmapping = new Outputfieldmapping($scope, ctrl, $attrs);
                outputfieldmapping.initializeController();
            },
            controllerAs: "fieldmappingCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Output Field Mapping'";
            if (attrs.hidelabel != undefined) {
                label = "";
            }


            var template =
                '<vr-row removeline>'
              + '<vr-columns colnum="{{fieldmappingCtrl.normalColNum}}">'
              + ' <vr-select on-ready="onSelectorReady"'
              + ' datasource="templateConfigs"'
              + ' selectedvalues="selectedTemplateConfig"'
               + 'datavaluefield="ExtensionConfigurationId"'
              + ' datatextfield="Title"'
              + label
               + ' isrequired="fieldmappingCtrl.isrequired"'
              + 'hideremoveicon>'
          + '</vr-select></vr-columns> '
          + '<vr-directivewrapper directive="selectedTemplateConfig.Editor" vr-loader="isLoadingDirective" on-ready="onDirectiveReady" normal-col-num="{{fieldmappingCtrl.normalColNum}}" isrequired="fieldmappingCtrl.isrequired" customvalidate="fieldmappingCtrl.customvalidate" type="fieldmappingCtrl.type"></vr-directivewrapper>';
           + '</vr-row>'
            return template;

        }
        function Outputfieldmapping($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var selectorAPI;
            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;
            var outputFieldValue;

            function initializeController() {
                $scope.templateConfigs = [];
                $scope.selectedTemplateConfig;

                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directivePayload = {
                        outputFieldValue: outputFieldValue
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
                    if (payload != undefined) {
                        outputFieldValue = payload.outputFieldValue;
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();
                        var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var payload = {
                            };
                            if (outputFieldValue != undefined)
                                payload.outputFieldValue = outputFieldValue;
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, payload, loadDirectivePromiseDeferred);
                        });
                        promises.push(loadDirectivePromiseDeferred.promise);
                    }
                    var getOutputFieldMappingPromise = getOutputFieldMapping();
                    promises.push(getOutputFieldMappingPromise);

                    function getOutputFieldMapping() {
                        return XBooster_PriceListConversion_PriceListTemplateAPIService.GetOutputFieldMappingTemplateConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.templateConfigs.push(response[i]);
                                }

                                if (outputFieldValue != undefined && !outputFieldValue.isDefaultData) {
                                    $scope.selectedTemplateConfig = UtilsService.getItemByVal($scope.templateConfigs, outputFieldValue.ConfigId, 'ExtensionConfigurationId');
                                } else
                                    $scope.selectedTemplateConfig = $scope.templateConfigs[0];
                            }
                        });
                    }
                    return UtilsService.waitMultiplePromises(promises);

                };
                api.isDateTime = function()
                {
                    if(directiveAPI != undefined && directiveAPI.isDateTime !=undefined)
                    {
                        return directiveAPI.isDateTime();
                    }
                }
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

    app.directive('xboosterPricelistconversionOutputfieldmappingSelective', OutputfieldmappingSelective);

})(app);