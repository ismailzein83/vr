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

            var carrierAccountFiltering;

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                defineApi();
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {
                
                    if (payload != undefined && payload.data != undefined) {
                        carrierAccountFiltering = payload.data.CarrierAccountFiltering;
                    }
                    if (carrierAccountFiltering != undefined) {
                        $scope.scopeModel.ratePlanRestricted = carrierAccountFiltering.RatePlan;
                        $scope.scopeModel.customerRouteRestricted = carrierAccountFiltering.CustomerRoute;
                        $scope.scopeModel.productRouteRestricted = carrierAccountFiltering.ProductRoute;
                        $scope.scopeModel.trafficRestricted = carrierAccountFiltering.Traffic;
                        $scope.scopeModel.billingRestricted = carrierAccountFiltering.Billing;
                    }
                };
                api.getData = function () {
                    var carrierAccountFiltering = {
                        RatePlan: $scope.scopeModel.ratePlanRestricted,
                        CustomerRoute: $scope.scopeModel.customerRouteRestricted,
                        ProductRoute: $scope.scopeModel.productRouteRestricted,
                        Traffic: $scope.scopeModel.trafficRestricted,
                        Billing: $scope.scopeModel.billingRestricted
                    };
                    var obj = {
                        $type: "TOne.WhS.BusinessEntity.Entities.AccountManagerSettings,TOne.WhS.BusinessEntity.Entities",
                        CarrierAccountFiltering: carrierAccountFiltering
                    };
                    return obj;
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }
        return directiveDefinitionObject;
    }]);