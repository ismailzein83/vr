(function (app) {

    'use strict';

    PrepaidSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function PrepaidSettingsDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new PrepaidSettingsCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Product/ProductRuntime/MainExtensions/ProductTypes/Templates/PrePaidSettingsTemplate.html'
        };

        function PrepaidSettingsCtor($scope, ctrl) {
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

                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.ProductTypes.PrePaid.PrePaidSettings, Retail.BusinessEntity.MainExtensions"
                    };

                    return obj;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeProductextendedsettingsPrepaid', PrepaidSettingsDirective);

})(app);