(function (app) {

    'use strict';

    SMSTaxRuleSettingsDirective.$inject = ['RA_RulesAPIService', 'UtilsService', 'VRUIUtilsService'];

    function SMSTaxRuleSettingsDirective(RA_RulesAPIService, UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var smsTaxRuleSettings = new SmsTaxRuleSettings(ctrl, $scope, $attrs);
                smsTaxRuleSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_RA/Directives/TaxRules/Templates/SMSTaxRuleSettings.html"
        };

        function SmsTaxRuleSettings(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();



            function initializeController() {
                $scope.smsTaxRuleTemplates = [];
                $scope.selectedSMSTaxRuleTemplate;

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
                    $scope.smsTaxRuleTemplates.length = 0;

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
                            $scope.selectedSMSTaxRuleTemplate = UtilsService.getItemByVal($scope.smsTaxRuleTemplates, settings.ConfigId, 'ExtensionConfigurationId');
                        }
                        else if ($scope.smsTaxRuleTemplates.length > 0)
                            $scope.selectedSMSTaxRuleTemplate = $scope.smsTaxRuleTemplates[0];
                    });

                    function loadTemplates() {
                        return RA_RulesAPIService.GetSMSTaxRuleTemplates().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.smsTaxRuleTemplates.push(response[i]);
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
                    obj.ConfigId = $scope.selectedSMSTaxRuleTemplate.ExtensionConfigurationId;
                    return obj;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('raRulesTaxrulesettingsSms', SMSTaxRuleSettingsDirective);

})(app);