(function (app) {

    'use strict';

    MeasureStyleruleconditionDirective.$inject = ['VR_Analytic_AnalyticConfigurationAPIService', 'UtilsService', 'VRUIUtilsService'];

    function MeasureStyleruleconditionDirective(VR_Analytic_AnalyticConfigurationAPIService, UtilsService, VRUIUtilsService) {
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
                var measureStylerulecondition = new MeasureStylerulecondition($scope, ctrl, $attrs);
                measureStylerulecondition.initializeController();
            },
            controllerAs: "styleruleConditionCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Style Rule Condition'";
            if (attrs.hidelabel != undefined) {
                label = "";
                withemptyline = '';
            }


            var template =
                '<vr-row>'
              + '<vr-columns colnum="{{styleruleConditionCtrl.normalColNum * 2}}">'
              + ' <vr-select on-ready="onSelectorReady"'
              + ' datasource="templateConfigs"'
              + ' selectedvalues="selectedTemplateConfig"'
               + 'datavaluefield="ExtensionConfigurationId"'
              + ' datatextfield="Title"'
              + label
               + ' isrequired="styleruleConditionCtrl.isrequired"'
              + 'hideremoveicon>'
          + '</vr-select>'
           + '</vr-row>'
              + '<vr-directivewrapper directive="selectedTemplateConfig.Editor" on-ready="onDirectiveReady" normal-col-num="{{styleruleConditionCtrl.normalColNum}}" isrequired="styleruleConditionCtrl.isrequired" customvalidate="styleruleConditionCtrl.customvalidate"></vr-directivewrapper>';
            return template;

        }
        function MeasureStylerulecondition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;
            var tableIds;
            function initializeController() {
                $scope.templateConfigs = [];
                $scope.selectedTemplateConfig;

                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directivePayload = undefined;
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
                    var measureStyles;
                    if (payload != undefined) {
                        tableIds = payload.tableIds;
                        measureStyles = payload.measureStyles;
                        if ($scope.selectedTemplateConfig != undefined)
                            $scope.selectedTemplateConfig = undefined;

                        directiveReadyDeferred = UtilsService.createPromiseDeferred();
                        var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var payloadDirective = {
                                measureStyles: measureStyles,
                            };

                            VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
                        });
                        promises.push(loadDirectivePromiseDeferred.promise);

                    }

                    var getMeasureStyleRuleTemplateConfigsPromise = getMeasureStyleRuleTemplateConfigs();
                    promises.push(getMeasureStyleRuleTemplateConfigsPromise);

                    return UtilsService.waitMultiplePromises(promises);

                    function getMeasureStyleRuleTemplateConfigs() {
                        return VR_Analytic_AnalyticConfigurationAPIService.GetMeasureStyleRuleTemplateConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.templateConfigs.push(response[i]);
                                }
                                if (measureStyles != undefined)
                                    $scope.selectedTemplateConfig = UtilsService.getItemByVal($scope.templateConfigs, measureStyles.ConfigId, 'ExtensionConfigurationId');
                                else
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

    app.directive('vrAnalyticMeasurestyleStylerulecondition', MeasureStyleruleconditionDirective);

})(app);