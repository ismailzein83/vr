'use strict';

app.directive('vrAccountbalanceNotificationtypeSettings', ['UtilsService',
    function (UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountBalanceNotificationTypeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_AccountBalance/Directives/AccountBalanceNotification/Templates/AccountBalanceNotificationTypeSettingsTemplate.html'
        };

        function AccountBalanceNotificationTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.AccountBalance.Business.AccountBalanceNotificationTypeSettings, Vanrise.AccountBalance.Business'
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);