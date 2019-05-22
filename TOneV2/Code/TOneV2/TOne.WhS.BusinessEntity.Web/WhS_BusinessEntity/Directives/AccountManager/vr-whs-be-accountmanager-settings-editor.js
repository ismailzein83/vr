'use strict';

app.directive('vrWhsBeAccountmanagerSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (utilsService, vruiUtilsService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new accountmanagerSettingsCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/AccountManager/Templates/AccountManagerSettingsEditorTemplate.html"
        };

        function accountmanagerSettingsCtor(ctrl, $scope, $attrs) {

            var accountManagerSettings;

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                defineApi();
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined && payload.data != undefined) {
                        accountManagerSettings = payload.data.AccountManagerSettings;
                    }
                    if (accountManagerSettings != undefined && accountManagerSettings.CustomerFiltering) {
                        $scope.scopeModel.ratePlanRestricted = accountManagerSettings.CustomerFiltering.RatePlan;
                    }
                };
                api.getData = function () {
                    var customerFiltering = {
                        RatePlan: $scope.scopeModel.ratePlanRestricted
                    };
                    var obj = {
                        $type: "TOne.WhS.BusinessEntity.Entities.AccountManagerSettings,TOne.WhS.BusinessEntity.Entities",
                        CustomerFiltering: customerFiltering
                    };
                    return obj;
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }
        return directiveDefinitionObject;
    }]);