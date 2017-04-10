'use strict';

app.directive('whsAccountbalanceActionCustomerSendemail', ['UtilsService', function (UtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var sendMailCustomerAction = new SendMailCustomerAction($scope, ctrl, $attrs);
            sendMailCustomerAction.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/AccountBalanceActions/SendCustomerEmailAction/Directives/Templates/SendCustomerEmailAction.html'
    };

    function SendMailCustomerAction($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var accountSelectorAPI;
        var accountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var profileSelectorAPI;
        var profileSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onAccountMailMessageTemplateSelectorReady = function (api) {
                accountSelectorAPI = api;
                accountSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onProfileMailMessageTemplateSelectorReady = function (api) {
                profileSelectorAPI = api;
                profileSelectorReadyDeferred.resolve();
            };

            UtilsService.waitMultiplePromises([accountSelectorReadyDeferred.promise, profileSelectorReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                var extendedSettings;
                var mailMessageTypeId;

                if (payload != undefined && payload.selectedVRActionDefinition != undefined && payload.selectedVRActionDefinition.Settings != null) {
                    extendedSettings = payload.selectedVRActionDefinition.Settings.ExtendedSettings;
                }

                if (extendedSettings != undefined)
                    mailMessageTypeId = extendedSettings.MailMessageTypeId;

                var promises = [];

                var selectorPayload = {
                    filter: {
                        VRMailMessageTypeId: mailMessageTypeId
                    }
                };

                var loadAccountSelectorPromise = accountSelectorAPI.load(selectorPayload);
                promises.push(loadAccountSelectorPromise);

                var loadProfileSelectorPromise = profileSelectorAPI.load(selectorPayload);
                promises.push(loadProfileSelectorPromise);

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.AccountBalance.MainExtensions.SendCustomerEmailAction, TOne.WhS.AccountBalance.MainExtensions',
                    ActionName: 'Email Customer',
                    AccountMailTemplateId: accountSelectorAPI.getSelectedIds(),
                    ProfileMailTemplateId: profileSelectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);