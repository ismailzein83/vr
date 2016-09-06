(function (app) {

    'use strict';

    DARecordAggregateSelectiveDirective.$inject = ['VR_Analytic_DataAnalysisItemDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function DARecordAggregateSelectiveDirective(VR_Analytic_DataAnalysisItemDefinitionAPIService, UtilsService, VRUIUtilsService) {
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
            templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/Common/Templates/DARecordAggregateSelectiveTemplate.html'
        };

        function DARecordAggregateSelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var context;

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

                    var directivePayload = { context: context };
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
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
                        context = payload.context;
                    }

                    var getDARecordAggregateTemplateConfigsPromise = getDARecordAggregateTemplateConfigs();
                    promises.push(getDARecordAggregateTemplateConfigsPromise);

                    if (recordAggregate != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }


                    function getDARecordAggregateTemplateConfigs() {
                        return VR_Analytic_DataAnalysisItemDefinitionAPIService.GetDARecordAggregateExtensionConfigs().then(function (response) {

                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (recordAggregate != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, recordAggregate.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;

                            var directivePayload = {
                                recordAggregate: recordAggregate,
                                context: context
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
                        }
                    }
                    return data
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticDarecordaggregateSelective', DARecordAggregateSelectiveDirective);

})(app);