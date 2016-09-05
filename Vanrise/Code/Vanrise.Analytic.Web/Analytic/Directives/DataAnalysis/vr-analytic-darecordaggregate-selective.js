(function (app) {

    'use strict';

    DARecordAggregateSelectiveDirective.$inject = ['VR_Analytic_DataAnalysisItemDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function DARecordAggregateSelectiveDirective(VRCommon_StyleDefinitionAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var daRecordAggregateSelective = new DARecordAggregateSelective($scope, ctrl, $attrs);
                daRecordAggregateSelective.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/Templates/DARecordAggregateSelectiveTemplate.html'
        };

        function DARecordAggregateSelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;

                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var recordAggregate;

                    if (payload != undefined) {
                        recordAggregate = payload.recordAggregate;
                    }

                    var getDARecordAggregateTemplateConfigsPromise = getDARecordAggregateTemplateConfigs();
                    promises.push(getDARecordAggregateTemplateConfigsPromise);

                    if (daRecordAggregateSelective != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function getDARecordAggregateTemplateConfigs() {
                        return VR_Analytic_DataAnalysisItemDefinitionAPIService.GetDARecordAggregateExtensionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (daRecordAggregateSelective != undefined && daRecordAggregateSelective.StyleFormatingSettings != null) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, daRecordAggregateSelective.StyleFormatingSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { recordAggregate: recordAggregate };
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
                        }
                    }
                    return {
                        StyleFormatingSettings: data
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }

        function getTamplate(attrs) {

            var template =
                '<vr-row>'
                    + ' <vr-columns width="1/2row">'
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="ExtensionConfigurationId"'
                            + ' datatextfield="Title"'
                            + ' isrequired="true"'
                            + ' label="Style Formating"'
                            + ' hideremoveicon>'
                        + '</vr-select>'
                    + ' </vr-columns>'
                + ' </vr-row>'
                    + ' <vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                            + ' on-ready="scopeModel.onDirectiveReady" isrequired="ctrl.isrequired" normal-col-num="{{ctrl.normalColNum}}" customvalidate="ctrl.customvalidate">'
                    + ' </vr-directivewrapper>'

            return template;
        }
    }

    app.directive('vrAnalyticDarecordaggregateSelective', DARecordAggregateSelectiveDirective);

})(app);