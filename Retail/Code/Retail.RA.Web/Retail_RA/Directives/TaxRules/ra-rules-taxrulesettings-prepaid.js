
(function (app) {

    'use strict';

    PrepaidTaxRuleSettingsDirective.$inject = ['RA_RulesAPIService', 'UtilsService', 'VRUIUtilsService'];

    function PrepaidTaxRuleSettingsDirective(RA_RulesAPIService, UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var taxRuleSettings = new PrepaidTaxRuleSettings(ctrl, $scope, $attrs);
                taxRuleSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_RA/Directives/TaxRules/Templates/PrepaidTaxRuleSettings.html"
        };

        function PrepaidTaxRuleSettings(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var topUpDirectiveAPI;
            var topUpDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.transactionTaxRuleTemplates = [];
                $scope.scopeModel.selectedTransactionTaxRuleTemplate;

                $scope.scopeModel.onTopUpDirectiveReady = function (api) {
                    topUpDirectiveAPI = api;
                    var setLoader = function (value) { $scope.scopeModel.isTopUpDirectiveLoading = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, topUpDirectiveAPI, undefined, setLoader, topUpDirectiveReadyPromiseDeferred);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.transactionTaxRuleTemplates.length = 0;

                    var initialPromises = [];
                    var settings;

                    if (payload != undefined) {
                        settings = payload.settings;
                    }

                    function loadTemplates() {
                        return RA_RulesAPIService.GetTransactionTaxRuleTemplates().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.transactionTaxRuleTemplates.push(response[i]);
                                }
                            }
                        });
                    }

                    function loadTopUpDirective() {
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        topUpDirectiveReadyPromiseDeferred.promise.then(function () {
                            topUpDirectiveReadyPromiseDeferred = undefined;
                            var directivePayload = {};
                            directivePayload.settings = settings != undefined?  settings.TopUpSettings: undefined;
                            VRUIUtilsService.callDirectiveLoad(topUpDirectiveAPI, directivePayload, directiveLoadDeferred);
                        });
                        return directiveLoadDeferred.promise;
                    }

                    initialPromises.push(loadTemplates());
                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [loadTopUpDirective()];
                            if (settings != undefined && settings.TopUpSettings != undefined) {
                                $scope.scopeModel.selectedTransactionTaxRuleTemplate = UtilsService.getItemByVal($scope.scopeModel.transactionTaxRuleTemplates, settings.TopUpSettings.ConfigId, 'ExtensionConfigurationId');
                            }
                            else if ($scope.scopeModel.transactionTaxRuleTemplates.length > 0)
                                $scope.scopeModel.selectedTransactionTaxRuleTemplate = $scope.scopeModel.transactionTaxRuleTemplates[0];

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    var obj=  {
                        $type: "Retail.RA.Business.PrepaidTaxRuleSettings, Retail.RA.Business",
                        ResidualSettings: {},
                        TopUpSettings: topUpDirectiveAPI.getData()
                    };
                    obj.TopUpSettings.ConfigId = $scope.scopeModel.selectedTransactionTaxRuleTemplate.ExtensionConfigurationId;
                    return obj;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('raRulesTaxrulesettingsPrepaid', PrepaidTaxRuleSettingsDirective);

})(app);