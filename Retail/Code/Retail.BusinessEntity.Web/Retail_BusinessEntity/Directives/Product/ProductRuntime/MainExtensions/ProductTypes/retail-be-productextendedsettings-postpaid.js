(function (app) {

    'use strict';

    PostpaidSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function PostpaidSettingsDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new PostpaidSettingsCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Product/ProductRuntime/MainExtensions/ProductTypes/Templates/PostPaidSettingsTemplate.html'
        };

        function PostpaidSettingsCtor($scope, ctrl) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var currencyId;

                    if (payload != undefined && payload.extendedSettings != undefined) {
                        $scope.scopeModel.creditLimit = payload.extendedSettings.CreditLimit;
                        currencyId = payload.extendedSettings.CurrencyId;
                    }
                };

                api.getData = function () {

                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.ProductTypes.PostPaid.PostPaidSettings, Retail.BusinessEntity.MainExtensions",
                        CreditLimit: $scope.scopeModel.creditLimit
                    };

                    return obj;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeProductextendedsettingsPostpaid', PostpaidSettingsDirective);

})(app);