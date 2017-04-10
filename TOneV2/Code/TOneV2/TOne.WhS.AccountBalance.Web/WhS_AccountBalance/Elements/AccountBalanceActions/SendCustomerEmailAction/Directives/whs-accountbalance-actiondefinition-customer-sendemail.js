'use strict';

app.directive('whsAccountbalanceActiondefinitionCustomerSendemail', [function () {
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

        var mailMessageTypeSelectorAPI;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onMailMessageTypeSelectorReady = function (api) {
                mailMessageTypeSelectorAPI = api;
                defineAPI();
            };
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {

                var extendedSettings;

                if (payload != undefined && payload.Settings != null) {
                    extendedSettings = payload.Settings.ExtendedSettings;
                }

                var mailMessageTypeSelectorPayload = {
                    selectedIds: (extendedSettings != undefined) ? extendedSettings.MailMessageTypeId : undefined
                };

                return mailMessageTypeSelectorAPI.load(mailMessageTypeSelectorPayload);
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.AccountBalance.MainExtensions.SendCustomerEmailActionDefinition, TOne.WhS.AccountBalance.MainExtensions',
                    MailMessageTypeId: mailMessageTypeSelectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);