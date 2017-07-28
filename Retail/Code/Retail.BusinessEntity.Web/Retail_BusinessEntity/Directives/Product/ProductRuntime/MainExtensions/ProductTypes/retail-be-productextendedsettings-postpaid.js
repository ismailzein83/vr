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

                    var productDefinition;
                    var extendedSettings;

                    if (payload != undefined) {
                        productDefinition = payload.productDefinition;
                        extendedSettings = payload.extendedSettings;
                    }

                    if (productDefinition != undefined && productDefinition.Settings != undefined && productDefinition.Settings.ExtendedSettings != undefined) {
                        $scope.scopeModel.showCreditLimit = !productDefinition.Settings.ExtendedSettings.InvisibleCreditLimit;
                    }

                    $scope.scopeModel.creditLimit = extendedSettings != undefined ? extendedSettings.CreditLimit : undefined;
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