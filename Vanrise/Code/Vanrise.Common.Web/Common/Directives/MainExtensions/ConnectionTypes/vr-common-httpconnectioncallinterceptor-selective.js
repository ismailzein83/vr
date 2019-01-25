(function (app) {
    'use strict';

    HttpConnectionCallInterceptor.$inject = ['VRCommon_HttpConnectionCallInterceptorAPIService', 'UtilsService', 'VRUIUtilsService'];
    function HttpConnectionCallInterceptor(VRCommon_HttpConnectionCallInterceptorAPIService, UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var interceptorTemplates = new InterceptorTemplates(ctrl, $scope, $attrs);
                interceptorTemplates.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/Common/Directives/MainExtensions/ConnectionTypes/Templates/HttpConnectionCallInterceptorSelectiveTemplate.html'
        };

        function InterceptorTemplates(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();



            function initializeController() {
                $scope.interceptorTemplates = [];
                $scope.selectedInterceptorTemplate;

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) { $scope.isLoadingDirective = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    console.log("Selective load");

                    $scope.interceptorTemplates.length = 0;

                    var promises = [];
                    var settings;

                    if (payload != undefined) {
                        settings = payload.settings;
                    }
                    console.log("RA");
                    var loadTemplatesPromise = loadTemplates();
                    promises.push(loadTemplatesPromise);

                    var loadDirectivePromise = loadDirective();
                    promises.push(loadDirectivePromise);

                    loadTemplatesPromise.then(function () {
                        if (settings != undefined) {
                            $scope.selectedInterceptorTemplate = UtilsService.getItemByVal($scope.interceptorTemplates, settings.ConfigId, 'ExtensionConfigurationId');
                        }
                        else if ($scope.interceptorTemplates.length > 0)
                            $scope.selectedInterceptorTemplate = $scope.interceptorTemplates[0];
                    });

                    function loadTemplates() {
                        return VRCommon_HttpConnectionCallInterceptorAPIService.GetHttpConnectionCallInterceptorTemplateConfigs().then(function (response) {
                            console.log(response);
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.interceptorTemplates.push(response[i]);
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { settings: settings };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = directiveAPI.getData();
                    obj.ConfigId = $scope.selectedInterceptorTemplate.ExtensionConfigurationId;
                    return obj;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrCommonHttpconnectioncallinterceptorSelective', HttpConnectionCallInterceptor);

})(app);