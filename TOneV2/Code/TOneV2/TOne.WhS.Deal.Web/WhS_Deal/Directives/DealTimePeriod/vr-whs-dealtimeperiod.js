(function (app) {

    'use strict';

    DealTimePeriodDirective.$inject = ['UtilsService', 'VRUIUtilsService','WhS_Deal_DealTimePeriodAPIService'];

    function DealTimePeriodDirective(UtilsService, VRUIUtilsService, WhS_Deal_DealTimePeriodAPIService) {

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
                var ctor = new DealTimePeriodCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Deal/Directives/DealTimePeriod/Templates/DealTimePeriodTemplate.html"
        };

        function DealTimePeriodCtor($scope, ctrl, $attrs) {
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

                    var directivePayload = undefined;

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

                    var dealTimePeriod;

                    if (payload != undefined) {
                        dealTimePeriod = payload.timePeriod;
                    }

                    var promises = [];

                    var getDealTimePeriodTemplateConfigsPromise = getDealTimePeriodTemplateConfigs();
                    promises.push(getDealTimePeriodTemplateConfigsPromise);

                    if (dealTimePeriod != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }


                    function getDealTimePeriodTemplateConfigs() {
                        return WhS_Deal_DealTimePeriodAPIService.GetDealTimePeriodTemplateConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }

                                if (dealTimePeriod != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig = 
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, dealTimePeriod.ConfigId, 'ExtensionConfigurationId');
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
                                dealTimePeriod: dealTimePeriod
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

    app.directive('vrWhsDealtimeperiod', DealTimePeriodDirective);
})(app);