﻿(function (app) {

    'use strict';

    MarginRateCalculationDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'WhS_Sales_PricingTemplateAPIService'];

    function MarginRateCalculationDirective(UtilsService, VRUIUtilsService, WhS_Sales_PricingTemplateAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: '=',
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountConditionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "marginRateCalculationctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Sales/Directives/PricingTemplate/Templates/MarginRateCalculationTemplate.html"
        };

        function AccountConditionCtor($scope, ctrl, $attrs) {
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

                    var directivePayload;

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

                    var marginRateCalculation;

                    if (payload != undefined) {
                        marginRateCalculation = payload.marginRateCalculation;
                    }

                    var getMarginRateCalculationTemplateConfigsPromise = getMarginRateCalculationTemplateConfigs();
                    promises.push(getMarginRateCalculationTemplateConfigsPromise);

                    if (marginRateCalculation != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }


                    function getMarginRateCalculationTemplateConfigs() {
                        return WhS_Sales_PricingTemplateAPIService.GetMarginRateCalculationExtensionConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (marginRateCalculation != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, marginRateCalculation.ConfigId, 'ExtensionConfigurationId');
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
                                marginRateCalculation: marginRateCalculation
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

    app.directive('vrWhsSalesPricingtemplateMarginratecalculation', MarginRateCalculationDirective);

})(app);