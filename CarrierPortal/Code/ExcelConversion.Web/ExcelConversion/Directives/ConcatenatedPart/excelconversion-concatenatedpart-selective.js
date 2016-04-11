(function (app) {

    'use strict';

    concatenatedpartSelective.$inject = ['ExcelConversion_ExcelAPIService', 'CDRComparison_CDRSourceAPIService', 'CDRComparison_CDRSourceConfigAPIService', 'UtilsService', 'VRUIUtilsService'];

    function concatenatedpartSelective(ExcelConversion_ExcelAPIService, CDRComparison_CDRSourceAPIService, CDRComparison_CDRSourceConfigAPIService, UtilsService, VRUIUtilsService) {
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
                var concatenatedpartSelective = new ConcatenatedpartSelective($scope, ctrl, $attrs);
                concatenatedpartSelective.initializeController();
            },
            controllerAs: "fieldmappingCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Field Mappings'";
            if (attrs.hidelabel != undefined) {
                label = "";
                withemptyline = '';
            }


            var template =
                '<vr-row>'
              + '<vr-columns colnum="{{fieldmappingCtrl.normalColNum}}">'
              + ' <vr-select on-ready="onSelectorReady"'
              + ' datasource="templateConfigs"'
              + ' selectedvalues="selectedTemplateConfig"'
               + 'datavaluefield="TemplateConfigID"'
              + ' datatextfield="Name"'
              + label
               + ' isrequired="fieldmappingCtrl.isrequired"'
              + 'hideremoveicon>'
          + '</vr-select>'
             + '</vr-columns>'
            + ' <vr-columns colnum="{{fieldmappingCtrl.normalColNum}}" ' + withemptyline + '>'
              + '<vr-directivewrapper directive="selectedTemplateConfig.Editor" on-ready="onDirectiveReady" normal-col-num="{{fieldmappingCtrl.normalColNum}}" isrequired="fieldmappingCtrl.isrequired" customvalidate="fieldmappingCtrl.customvalidate" type="fieldmappingCtrl.type"></vr-directivewrapper>'
            '</vr-columns >'
           + '</vr-row>';
            return template;

        }
        function ConcatenatedpartSelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
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
                    directivePayload = {
                        context: getContext()
                    }
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
                        context = payload.context;
                    }
                    var getFieldMappingTemplateConfigsPromise = getFieldMappingTemplateConfigs();
                    promises.push(getFieldMappingTemplateConfigsPromise);

                    return UtilsService.waitMultiplePromises(promises);

                    function getFieldMappingTemplateConfigs() {
                        return ExcelConversion_ExcelAPIService.GetFieldMappingTemplateConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.templateConfigs.push(response[i]);
                                }
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
                            data.ConfigId = $scope.selectedTemplateConfig.TemplateConfigID;
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
            function getFieldMappingConfig() {
                return CDRComparison_CDRSourceConfigAPIService.GetCDRSourceConfig(cdrSourceConfigId).then(function (cdrSourceConfig) {
                    cdrSource = cdrSourceConfig.CDRSource;
                });
            }
        }
    }

    app.directive('excelconversionConcatenatedpartSelective', concatenatedpartSelective);

})(app);