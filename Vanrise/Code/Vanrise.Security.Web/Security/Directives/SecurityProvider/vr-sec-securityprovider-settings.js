'use strict';

app.directive('vrSecSecurityproviderSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var securityProviderSettings = new SecurityProviderSettings(ctrl, $scope, $attrs);
                securityProviderSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Security/Directives/SecurityProvider/Templates/SecurityProviderSettingsTemplate.html"
        };

        function SecurityProviderSettings(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var securityProviderSelectorReadyAPI;
            var securityProviderSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};

            $scope.scopeModel.onSecurityProviderSelectorReady = function (api) {
                securityProviderSelectorReadyAPI = api;
                securityProviderSelectorReadyPromiseDeferred.resolve();
            };

            function initializeController() {

                var promises = [securityProviderSelectorReadyPromiseDeferred.promise];

                UtilsService.waitMultiplePromises(promises).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var defaultSecurityProviderId;

                    if (payload != undefined) {
                        defaultSecurityProviderId = payload.DefaultSecurityProviderId;
                    }

                    var securityProviderSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    var securityProviderPayload;
                    if (defaultSecurityProviderId != undefined) {
                        securityProviderPayload = { selectedIds: defaultSecurityProviderId };
                    }

                    VRUIUtilsService.callDirectiveLoad(securityProviderSelectorReadyAPI, securityProviderPayload, securityProviderSelectorLoadPromiseDeferred);
                    promises.push(securityProviderSelectorLoadPromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        DefaultSecurityProviderId: securityProviderSelectorReadyAPI.getSelectedIds(),
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);