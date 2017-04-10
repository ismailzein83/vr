'use strict';

app.directive('whsAccountbalanceActiondefinitionCustomerSendemail', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var sendMailCustomerActionDefinition = new SendMailCustomerActionDefinition($scope, ctrl, $attrs);
            sendMailCustomerActionDefinition.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/AccountBalanceActions/SendCustomerEmailAction/Directives/Templates/SendCustomerEmailActionDefinition.html'
    };

    function SendMailCustomerActionDefinition($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var accountMailMessageTypeSelectorAPI;
        var accountMailMessageTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var profileMailMessageTypeSelectorAPI;
        var profileMailMessageTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onAccountMailMessageTypeSelectorReady = function (api) {
                accountMailMessageTypeSelectorAPI = api;
                accountMailMessageTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onProfileMailMessageTypeSelectorReady = function (api) {
                profileMailMessageTypeSelectorAPI = api;
                profileMailMessageTypeSelectorReadyDeferred.resolve();
            };

            UtilsService.waitMultiplePromises([accountMailMessageTypeSelectorReadyDeferred.promise, profileMailMessageTypeSelectorReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {

                var extendedSettings;
                var promises = [];
                if (payload != undefined && payload.Settings != null) {
                    extendedSettings = payload.Settings.ExtendedSettings;
                }

                promises.push(loadAccountMailMessageTypeSelector());
                promises.push(loadProfileMailMessageTypeSelector());

                function loadAccountMailMessageTypeSelector() {
                    var accountMailMessageTypePayload;
                    if (extendedSettings != undefined) {
                        accountMailMessageTypePayload = {
                            selectedIds: extendedSettings.AccountMessageTypeId
                        };
                    }
                    return accountMailMessageTypeSelectorAPI.load(accountMailMessageTypePayload);
                }
                function loadProfileMailMessageTypeSelector() {
                    var profileMailMessageTypePayload;
                    if (extendedSettings != undefined) {
                        profileMailMessageTypePayload = {
                            selectedIds: extendedSettings.ProfileMessageTypeId
                        };
                    }
                    return profileMailMessageTypeSelectorAPI.load(profileMailMessageTypePayload);
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.AccountBalance.MainExtensions.SendCustomerEmailActionDefinition, TOne.WhS.AccountBalance.MainExtensions',
                    ProfileMessageTypeId: profileMailMessageTypeSelectorAPI.getSelectedIds(),
                    AccountMessageTypeId: accountMailMessageTypeSelectorAPI.getSelectedIds(),
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);