(function (app) {

    'use strict';

    AccountBalanceNotificationTypeSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function AccountBalanceNotificationTypeSettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountBalanceNotificationTypeSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_AccountBalance/Elements/AccountBalanceNotification/Directives/Templates/AccountBalanceNotificationTypeSettingsTemplate.html"
        };

        function AccountBalanceNotificationTypeSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.AccountBalance.Business.TOneAccountBalanceNotificationTypeSettings, TOne.WhS.AccountBalance.Business"
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsAccountbalanceNotificationtypeSettings', AccountBalanceNotificationTypeSettingsDirective);

})(app);