'use strict';

app.directive('retailBeAccountviewdefinitionsettingsSubaccounts', ['UtilsService',
    function (UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SubAccountsViewDefinitionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountViewDefinition/MainExtensions/Templates/SubAccountsViewSettingsTemplate.html'
        };

        function SubAccountsViewDefinitionSettingsCtor($scope, ctrl, $attrs) {
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
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountViews.SubAccounts, Retail.BusinessEntity.MainExtensions'
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);