'use strict';

app.directive('vrAccountbalanceAccounttypeSourceLivebalance', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var staticFieldSourceSetting = new LiveBalanceSourceSetting($scope, ctrl, $attrs);
                staticFieldSourceSetting.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_AccountBalance/Directives/MainExtensions/Account/MainExtensions/Templates/LiveBalanceSourceSetting.html'
        };

        function LiveBalanceSourceSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
         
                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                    }
                };

                api.getData = function () {
                    return {
                        $type: " Vanrise.AccountBalance.MainExtensions.AccountBalanceFieldSource.LiveBalanceFieldSourceSetting,  Vanrise.AccountBalance.MainExtensions",
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
