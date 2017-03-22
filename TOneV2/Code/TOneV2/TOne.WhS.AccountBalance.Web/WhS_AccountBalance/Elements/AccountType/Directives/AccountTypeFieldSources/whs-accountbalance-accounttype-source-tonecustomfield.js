'use strict';

app.directive('whsAccountbalanceAccounttypeSourceTonecustomfield', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var toneCustomFieldSourceSetting = new TOneCustomFieldSourceSetting($scope, ctrl, $attrs);
                toneCustomFieldSourceSetting.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/AccountType/Directives/AccountTypeFieldSources/Templates/TOneCustomFieldSourceSetting.html'
        };

        function TOneCustomFieldSourceSetting($scope, ctrl, $attrs) {
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
                        $type: "TOne.WhS.AccountBalance.MainExtensions.TOneCustomFieldSourceSetting,  TOne.WhS.AccountBalance.MainExtensions",
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
