(function (app) {

    'use strict';

    VRRestAPIAnalyticQueryInterceptorSelectiveDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Analytic_AnalyticConfigurationAPIService'];

    function VRRestAPIAnalyticQueryInterceptorSelectiveDirective(UtilsService, VRUIUtilsService, VR_Analytic_AnalyticConfigurationAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VRRestAPIAnalyticQueryInterceptorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/VRRestAPIAnalyticQueryInterceptor/Templates/VRRestAPIAnalyticQueryInterceptorSelectiveTemplate.html"
        };

        function VRRestAPIAnalyticQueryInterceptorCtor($scope, ctrl, $attrs) {
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

                    var directivePayload = {};

                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var vrRestAPIAnalyticQueryInterceptor;

                    if (payload != undefined) {
                        vrRestAPIAnalyticQueryInterceptor = payload.vrRestAPIAnalyticQueryInterceptor;
                    }

                    var getVRRestAPIAnalyticQueryInterceptorTemplateConfigsPromise = getVRRestAPIAnalyticQueryInterceptorTemplateConfigs();
                    promises.push(getVRRestAPIAnalyticQueryInterceptorTemplateConfigsPromise);

                    if (vrRestAPIAnalyticQueryInterceptor != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function getVRRestAPIAnalyticQueryInterceptorTemplateConfigs() {
                        return VR_Analytic_AnalyticConfigurationAPIService.GetVRRestAPIAnalyticQueryInterceptorConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (vrRestAPIAnalyticQueryInterceptor != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, vrRestAPIAnalyticQueryInterceptor.ConfigId, 'ExtensionConfigurationId');
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
                                vrRestAPIAnalyticQueryInterceptor: vrRestAPIAnalyticQueryInterceptor
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
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticRestapianalyticqueryinterceptorSelective', VRRestAPIAnalyticQueryInterceptorSelectiveDirective);

})(app);