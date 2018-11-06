(function (app) {
    'use strict';
    VRRestAPIAnalyticQueryInterceptorDirective.$inject = ['UtilsService','PartnerPortal_CustomerAccess_CarrierAccountType'];
    function VRRestAPIAnalyticQueryInterceptorDirective(UtilsService, PartnerPortal_CustomerAccess_CarrierAccountType) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VRRestAPIAnalyticQueryInterceptorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/PartnerPortal_CustomerAccess/Elements/VRRestAPIAnalyticQueryInterceptor/Directives/MainExtensions/Templates/WhSAccountVRRestAPIAnalyticQueryInterceptorTemplate.html'
        };

        function VRRestAPIAnalyticQueryInterceptorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.carrierAccountTypes = UtilsService.getArrayEnum(PartnerPortal_CustomerAccess_CarrierAccountType);
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.vrRestAPIAnalyticQueryInterceptor != undefined) {
                        $scope.scopeModel.selectedCarrierAccountType = UtilsService.getItemByVal($scope.scopeModel.carrierAccountTypes, payload.vrRestAPIAnalyticQueryInterceptor.AccountType, "value");
                        $scope.scopeModel.accountDimensionName = payload.vrRestAPIAnalyticQueryInterceptor.AccountDimensionName;
                    }
                };

                api.getData = function () {

                    return {
                        $type: 'PartnerPortal.CustomerAccess.MainExtensions.VRRestAPIAnalyticQueryInterceptor.WhSAccountVRRestAPIAnalyticQueryInterceptor, PartnerPortal.CustomerAccess.MainExtensions',
                        AccountType: $scope.scopeModel.selectedCarrierAccountType.value,
                        AccountDimensionName:$scope.scopeModel.accountDimensionName
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('partnerportalCustomeraccessWhsaccountanalyticqueryinterceptor', VRRestAPIAnalyticQueryInterceptorDirective);

})(app);