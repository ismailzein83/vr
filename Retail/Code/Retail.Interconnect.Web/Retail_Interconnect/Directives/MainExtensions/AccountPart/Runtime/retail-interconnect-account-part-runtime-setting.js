'use strict';

app.directive('retailInterconnectAccountPartRuntimeSetting', ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var accountTypeSettingPartRuntime = new AccountTypeSettingPartRuntime($scope, ctrl, $attrs);
                accountTypeSettingPartRuntime.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Interconnect/Directives/MainExtensions/AccountPart/Runtime/Templates/AccountTypeSettingPartRuntimeTemplate.html'
        };

        function AccountTypeSettingPartRuntime($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};
                var partSettings;

                api.load = function (payload) {
                    if (payload != undefined) {
                        partSettings = payload.partSettings;
                        if (payload.partSettings != undefined) $scope.scopeModel.representASwitch = partSettings.RepresentASwitch;
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.Interconnect.Business.AccountPartInterconnectSetting,Retail.Interconnect.Business',
                        RepresentASwitch: $scope.scopeModel.representASwitch
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);

