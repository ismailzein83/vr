(function (app) {

    'use strict';

    QueueActivatorUpdateWhSBalances.$inject = ['UtilsService', 'VRUIUtilsService'];

    function QueueActivatorUpdateWhSBalances(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new QueueActivatorUpdateWhSBalancesCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_AccountBalance/Elements/QueueActivators/Directives/Templates/QueueActivatorUpdateWhSBalancesTemplate.html';
            }
        };

        function QueueActivatorUpdateWhSBalancesCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var updateAccountBalanceSettingsDirectiveAPI;
            var updateAccountBalanceSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onUpdateAccountBalanceSettingsDirectiveReady = function (api) {
                    updateAccountBalanceSettingsDirectiveAPI = api;
                    updateAccountBalanceSettingsDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var updateAccountBalanceSettings;

                    if (payload != undefined && payload.QueueActivator != undefined) {
                        updateAccountBalanceSettings = payload.QueueActivator.UpdateAccountBalanceSettings;
                    }

                    var updateAccountBalanceSettingsDirectiveLoadPromise = getUpdateAccountBalanceSettingsDirectiveLoadPromise();
                    promises.push(updateAccountBalanceSettingsDirectiveLoadPromise);

                    function getUpdateAccountBalanceSettingsDirectiveLoadPromise() {
                        var updateAccountBalanceSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        updateAccountBalanceSettingsDirectiveReadyDeferred.promise.then(function () {

                            var payload;
                            if (updateAccountBalanceSettings != undefined) {
                                payload = {
                                    updateAccountBalanceSettings: updateAccountBalanceSettings
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(updateAccountBalanceSettingsDirectiveAPI, payload, updateAccountBalanceSettingsDirectiveLoadDeferred);
                        });

                        return updateAccountBalanceSettingsDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        $type: 'TOne.WhS.AccountBalance.MainExtensions.QueueActivators.UpdateWhSBalancesQueueActivator, TOne.WhS.AccountBalance.MainExtensions',
                        UpdateAccountBalanceSettings: updateAccountBalanceSettingsDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('whsAccountbalanceQueueactivatorUpdatewhsbalances', QueueActivatorUpdateWhSBalances);

})(app);