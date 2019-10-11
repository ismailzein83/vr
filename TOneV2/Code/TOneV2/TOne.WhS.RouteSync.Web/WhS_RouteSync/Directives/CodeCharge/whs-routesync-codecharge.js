(function (app) {

    'use strict';

    codeChargeDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'WhS_RouteSync_CodeChargeAPIService'];

    function codeChargeDirective(UtilsService, VRUIUtilsService, WhS_RouteSync_CodeChargeAPIService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new codeChargeDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'codeChargeCtrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Whs_RouteSync/Directives/CodeCharge/Templates/CodeChargeTemplate.html'
        };

        function codeChargeDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var codeChargeSelectorAPI;
            var codeChargeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var directiveAPI;
            var directiveReadyPromiseDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];

                $scope.scopeModel.onCodeChargeSelectorReady = function (api) {
                    codeChargeSelectorAPI = api;
                    codeChargeSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCodeChargeSelectionChanged = function (value) {
                    if (value == undefined) {
                        directiveAPI = undefined;
                    }
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyPromiseDeferred);
                };

                UtilsService.waitMultiplePromises([codeChargeSelectorReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var codeChargeEvaluator;

                    if (payload != undefined) {
                        codeChargeEvaluator = payload.codeChargeEvaluator;
                    }

                    var promises = [];

                    var codeChargeTemplateConfigsLoadPromise = getCodeChargeTemplateConfigsLoadPromise();
                    promises.push(codeChargeTemplateConfigsLoadPromise);

                    var directiveLoadPromise;
                    if (codeChargeEvaluator != undefined) {
                        directiveLoadPromise = getdirectiveLoadPromise();
                        promises.push(directiveLoadPromise);
                    }

                    function getCodeChargeTemplateConfigsLoadPromise() {
                        return WhS_RouteSync_CodeChargeAPIService.GetCodeChargeEvaluatorExtensionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (codeChargeEvaluator != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, codeChargeEvaluator.ConfigID, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function getdirectiveLoadPromise() {
                        directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyPromiseDeferred.promise.then(function () {
                            directiveReadyPromiseDeferred = undefined;
                            var directivePayload = { codeChargeEvaluator: codeChargeEvaluator };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadPromiseDeferred);
                        });

                        return directiveLoadPromiseDeferred.promise;
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

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncCodecharge', codeChargeDirective);
})(app);