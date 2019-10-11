(function (app) {

    'use strict';

    numberLengthDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'WhS_RouteSync_NumberLengthAPIService'];

    function numberLengthDirective(UtilsService, VRUIUtilsService, WhS_RouteSync_NumberLengthAPIService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new numberLengthDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'numberLengthCtrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Whs_RouteSync/Directives/NumberLength/Templates/NumberLengthTemplate.html'
        };

        function numberLengthDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var numberLengthSelectorAPI;
            var numberLengthSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var directiveAPI;
            var directiveReadyPromiseDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];

                $scope.scopeModel.onNumberLengthSelectorReady = function (api) {
                    numberLengthSelectorAPI = api;
                    numberLengthSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyPromiseDeferred);
                };

                UtilsService.waitMultiplePromises([numberLengthSelectorReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var numberLengthEvaluator;

                    if (payload != undefined) {
                        numberLengthEvaluator = payload.numberLengthEvaluator;
                    }

                    var numberLengthTemplateConfigsLoadPromise = getNumberLengthTemplateConfigsLoadPromise();
                    promises.push(numberLengthTemplateConfigsLoadPromise);

                    var directiveLoadPromise;
                    if (numberLengthEvaluator != undefined) {
                        directiveLoadPromise = getdirectiveLoadPromise();
                        promises.push(directiveLoadPromise);
                    }

                    function getNumberLengthTemplateConfigsLoadPromise() {
                        return WhS_RouteSync_NumberLengthAPIService.GetNumberLengthEvaluatorExtensionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (numberLengthEvaluator != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, numberLengthEvaluator.ConfigID, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function getdirectiveLoadPromise() {
                        directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyPromiseDeferred.promise.then(function () {
                            directiveReadyPromiseDeferred = undefined;
                            var directivePayload = { numberLengthEvaluator: numberLengthEvaluator };
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

    app.directive('whsRoutesyncNumberlength', numberLengthDirective);
})(app);