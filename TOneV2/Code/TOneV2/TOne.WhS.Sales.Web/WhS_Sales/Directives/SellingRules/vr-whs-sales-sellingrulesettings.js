(function (app) {

    'use strict';

    SellingRuleSettingsDirective.$inject = ['WhS_Sales_SellingRuleAPIService', 'UtilsService', 'VRUIUtilsService'];

    function SellingRuleSettingsDirective(WhS_Sales_SellingRuleAPIService, UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var sellingRuleSettings = new SellingRuleSettings(ctrl, $scope, $attrs);
                sellingRuleSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Sales/Directives/SellingRules/Templates/SellingRulesSettingsTemplate.html"
        };

        function SellingRuleSettings(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var thresholdDirectiveAPI;
            var thresholdDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.thresholdTemplateDataSource = [];
                $scope.thresholdSelectedTemplate;

                $scope.onThresholdDirectiveReady = function (api) {
                    thresholdDirectiveAPI = api;
                    var setLoader = function (value) { $scope.isLoadingDirective = false };
                    if (thresholdDirectiveAPI != undefined)
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, thresholdDirectiveAPI, undefined, setLoader, thresholdDirectiveReadyDeferred);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.thresholdTemplateDataSource.length = 0;

                    var promises = [];
                    var settings;

                    if (payload != undefined && payload.settings != undefined && payload.settings.RateRuleGrouped != undefined && payload.settings.RateRuleGrouped.RateRules != undefined) {
                        settings = payload.settings.RateRuleGrouped.RateRules[0];
                    }

                    var loadThresholdTemplatesPromise = loadThresholdTemplates();
                    promises.push(loadThresholdTemplatesPromise);

                    var loadDirectivePromise = loadDirective();
                    promises.push(loadDirectivePromise);

                    loadThresholdTemplatesPromise.then(function () {
                        if (settings != undefined && settings.Threshold != undefined) {
                            $scope.thresholdSelectedTemplate = UtilsService.getItemByVal($scope.thresholdTemplateDataSource, settings.Threshold.ConfigId, 'ExtensionConfigurationId');
                        }
                        else if ($scope.thresholdTemplateDataSource != undefined && $scope.thresholdTemplateDataSource.length > 0)
                            $scope.thresholdSelectedTemplate = $scope.thresholdTemplateDataSource[0];
                    });
                    function loadThresholdTemplates() {
                        return WhS_Sales_SellingRuleAPIService.GetSellingRuleThresholdTemplates().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.thresholdTemplateDataSource.push(response[i]);
                                }
                            }
                        });
                    }

                    function loadDirective() {
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                        thresholdDirectiveReadyDeferred.promise.then(function () {
                            thresholdDirectiveReadyDeferred = undefined;
                            var directivePayload = undefined;
                            if (settings != undefined)
                                directivePayload =
                                    {
                                        threshold: settings.Threshold

                                    };
                            VRUIUtilsService.callDirectiveLoad(thresholdDirectiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var thresholdObj = thresholdDirectiveAPI.getData();
                    thresholdObj.ConfigId = $scope.thresholdSelectedTemplate.ExtensionConfigurationId;

                    var rateRules = [];
                    rateRules.push({
                        $type: "TOne.WhS.Sales.Entities.RateRule,TOne.WhS.Sales.Entities",
                        Threshold: thresholdObj
                    });

                    var rateRuleGroupped =
                    {
                        $type: "TOne.WhS.Sales.Entities.RateRuleGrouped,TOne.WhS.Sales.Entities",
                        RateRules: rateRules
                    }

                    var setting =
                    {
                        $type: "TOne.WhS.Sales.Entities.SellingRuleSettings,TOne.WhS.Sales.Entities",
                        RateRuleGrouped: rateRuleGroupped
                    }
                    return setting;

                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsSalesSellingrulesettings', SellingRuleSettingsDirective);

})(app);