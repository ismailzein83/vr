(function (app) {

    'use strict';

    RetailBEChargeCustomObjectSettings.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function RetailBEChargeCustomObjectSettings(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RetailBEChargeCustomObjectSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/RetailBECharge/Templates/RetailBEChargeCustomObjectSettingsTemplate.html"

        };
        function RetailBEChargeCustomObjectSettingsCtor($scope, ctrl, $attrs) {
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
                    var data = {
                        $type: "Retail.BusinessEntity.Business.RetailBEChargeCustomObjectTypeSettings, Retail.BusinessEntity.Business"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeChargeCustomobjectsettings', RetailBEChargeCustomObjectSettings);

})(app);
