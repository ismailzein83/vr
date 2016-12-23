'use strict';

app.directive('retailBeAccountviewdefinitionsettingsPackages', ['UtilsService',
    function (UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new PackagesViewDefinitionSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/MainExtensions/AccountViewDefinitions/Templates/PackagesViewSettingsTemplate.html'
        };

        function PackagesViewDefinitionSettings($scope, ctrl, $attrs) {
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
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountViews.AccountPackages, Retail.BusinessEntity.MainExtensions'
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);