﻿(function (app) {

    'use strict';

    RateValueSettingsDirective.$inject = ['VR_Rules_PricingRuleAPIService', 'UtilsService', 'VRUIUtilsService'];

    function RateValueSettingsDirective(VR_Rules_PricingRuleAPIService, UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var rateValueSettings = new RateValueSettings(ctrl, $scope, $attrs);
                rateValueSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Rules/Directives/PricingRule/Templates/PricingRuleRateValueSettings.html"
        };

        function RateValueSettings(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.rateValueTemplates = [];
                $scope.selectedRateValueTemplate;

                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) { $scope.isLoadingDirective = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.getData = function () {
                    var data = directiveAPI.getData();
                    data.ConfigId = $scope.selectedRateValueTemplate.TemplateConfigID;
                    return data;
                };

                api.load = function (payload) {
                    var promises = [];
                    var configId;

                    if (payload != undefined) {
                        configId = payload.ConfigId;
                    }
                    
                    var templatesPromise = VR_Rules_PricingRuleAPIService.GetPricingRuleRateValueTemplates();
                    promises.push(templatesPromise);

                    templatesPromise.then(function (response) {
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.rateValueTemplates.push(response[i]);
                            }

                            if (configId != undefined) {
                                $scope.selectedRateValueTemplate = UtilsService.getItemByVal($scope.rateValueSettings, configId, 'TemplateConfigID');
                            }
                        }
                    });
                    
                    if (payload != undefined) {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                        promises.push(directiveLoadDeferred.promise);

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, payload, directiveLoadDeferred);
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                return api;
            }
        }
    }

    app.directive('vrRulesPricingrulesettingsRatevalue', RateValueSettingsDirective);

})(app);