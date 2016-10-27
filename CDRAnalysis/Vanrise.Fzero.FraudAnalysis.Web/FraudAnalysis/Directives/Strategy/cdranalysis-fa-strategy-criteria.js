(function (app) {

    'use strict';

    StrategyCriteriaDirective.$inject = ['StrategyAPIService', 'UtilsService', 'VRUIUtilsService'];

    function StrategyCriteriaDirective(StrategyAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                label: '@',
                customvalidate: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var strategyCriteria = new StrategyCriteria($scope, ctrl, $attrs);
                strategyCriteria.initializeController();
            },
            controllerAs: "strategyCriteriaCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Strategy Criteria'";
            if (attrs.hidelabel != undefined) {
                label = "strategy Criteria";
                withemptyline = '';
            }


            var template =
                '<vr-row>'
              + '<vr-columns colnum="{{strategyCriteriaCtrl.normalColNum}}">'
                     + ' <vr-select on-ready="onSelectorReady" datasource="templateConfigs" selectedvalues="selectedTemplateConfig" datavaluefield="ExtensionConfigurationId" datatextfield="Title"'
                     + label + ' isrequired="strategyCriteriaCtrl.isrequired" hideremoveicon></vr-select>'
               + ' </vr-columns>'
              + '</vr-row>'
            + '<vr-row vr-loader = "isLoadingDirective" >'
                + '<vr-directivewrapper directive="selectedTemplateConfig.Editor" on-ready="onDirectiveReady" normal-col-num="{{strategyCriteriaCtrl.normalColNum}}" isrequired="strategyCriteriaCtrl.isrequired" customvalidate="strategyCriteriaCtrl.customvalidate"></vr-directivewrapper>'
             + '</vr-row>';
            return template;

        }
        function StrategyCriteria($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;
            var strategyCriteria;
            var context;
            var filter;
            function initializeController() {
                $scope.templateConfigs = [];
                $scope.selectedTemplateConfig;

                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directivePayload = { filter: filter, context: context };
                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.selectedTemplateConfig = undefined;
                    var promises = [];
                    if (payload != undefined)
                    {
                        filter = payload.filter;
                        context = payload.context;
                        if (payload.strategyCriteria != undefined) {
                            strategyCriteria = payload.strategyCriteria;
                            if (strategyCriteria != undefined)
                                directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                            directiveReadyDeferred.promise.then(function () {
                                directiveReadyDeferred = undefined;
                                var payloadDirective = { strategyCriteria: strategyCriteria, context: context, filter: filter };

                                VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
                            });
                            promises.push(loadDirectivePromiseDeferred.promise);

                        }
                    }
                   

                    var getStrategyCriteriaTemplateConfigsPromise = getStrategyCriteriaTemplateConfigs();
                    promises.push(getStrategyCriteriaTemplateConfigsPromise);

                    return UtilsService.waitMultiplePromises(promises);

                    function getStrategyCriteriaTemplateConfigs() {
                        return StrategyAPIService.GetStrategyCriteriaTemplateConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.templateConfigs.push(response[i]);
                                }
                                if (strategyCriteria != undefined)
                                    $scope.selectedTemplateConfig = UtilsService.getItemByVal($scope.templateConfigs, strategyCriteria.ConfigId, 'ExtensionConfigurationId');
                                else
                                    $scope.selectedTemplateConfig = $scope.templateConfigs[0];
                            }
                        });
                    }


                };
                api.getFilterHint = function (parameter) {
                    if (directiveAPI != undefined && directiveAPI.getFilterHint != undefined)
                         return  directiveAPI.getFilterHint(parameter);
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

    app.directive('cdranalysisFaStrategyCriteria', StrategyCriteriaDirective);

})(app);