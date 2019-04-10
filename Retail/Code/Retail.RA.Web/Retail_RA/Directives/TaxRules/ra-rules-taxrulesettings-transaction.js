
(function (app) {

    'use strict';

    TaxRuleSettingsDirective.$inject = ['RA_RulesAPIService', 'UtilsService', 'VRUIUtilsService'];

    function TaxRuleSettingsDirective(RA_RulesAPIService, UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var taxRuleSettings = new TaxRuleSettings(ctrl, $scope, $attrs);
                taxRuleSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_RA/Directives/TaxRules/Templates/TransactionTaxRuleSettings.html"
        };

        function TaxRuleSettings(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();



            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.transactionTaxRuleTemplates = [];
                $scope.scopeModel.selectedTransactionTaxRuleTemplate;

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) { $scope.scopeModel.isLoadingDirective = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.transactionTaxRuleTemplates.length = 0;

                    var promises = [];
                    var settings;

                    if (payload != undefined) {
                        settings = payload.settings;
                    }

                    var loadTemplatesPromise = loadTemplates();
                    promises.push(loadTemplatesPromise);

                    var loadDirectivePromise = loadDirective();
                    promises.push(loadDirectivePromise);

                    loadTemplatesPromise.then(function () {
                        if (settings != undefined) {
                            $scope.scopeModel.selectedTransactionTaxRuleTemplate = UtilsService.getItemByVal($scope.scopeModel.transactionTaxRuleTemplates, settings.ConfigId, 'ExtensionConfigurationId');
                        }
                        else if ($scope.scopeModel.transactionTaxRuleTemplates.length > 0)
                            $scope.scopeModel.selectedTransactionTaxRuleTemplate = $scope.scopeModel.transactionTaxRuleTemplates[0];
                    });

                    function loadTemplates() {
                        return RA_RulesAPIService.GetTransactionTaxRuleTemplates().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.transactionTaxRuleTemplates.push(response[i]);
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
                    if (obj == undefined)
                        obj = {};
                    obj.ConfigId = $scope.scopeModel.selectedTransactionTaxRuleTemplate.ExtensionConfigurationId;
                    return obj;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('raRulesTaxrulesettingsTransaction', TaxRuleSettingsDirective);

})(app);