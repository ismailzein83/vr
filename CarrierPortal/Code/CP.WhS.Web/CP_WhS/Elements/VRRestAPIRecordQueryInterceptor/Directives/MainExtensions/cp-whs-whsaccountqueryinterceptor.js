(function (app) {
    'use strict';
    VRRestAPIQueryInterceptorDirective.$inject = ['UtilsService', 'PartnerPortal_CustomerAccess_CarrierAccountType'];
    function VRRestAPIQueryInterceptorDirective(UtilsService, PartnerPortal_CustomerAccess_CarrierAccountType) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VRRestAPIQueryInterceptorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/CP_WhS/Elements/VRRestAPIRecordQueryInterceptor/Directives/MainExtensions/Templates/WhSAccountQueryInterceptorTemplate.html'
        };

        function VRRestAPIQueryInterceptorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.carrierAccountTypes = UtilsService.getArrayEnum(PartnerPortal_CustomerAccess_CarrierAccountType);
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.vrRestAPIRecordQueryInterceptor != undefined) {
                        $scope.scopeModel.selectedCarrierAccountType = UtilsService.getItemByVal($scope.scopeModel.carrierAccountTypes, payload.vrRestAPIRecordQueryInterceptor.AccountType, "value");
                        $scope.scopeModel.accountFieldName = payload.vrRestAPIRecordQueryInterceptor.FieldName;
                    }
                };

                api.getData = function () {

                    return {
                        $type: 'CP.WhS.Business.WhSAccountVRRestAPIRecordQueryInterceptor,CP.WhS.Business',
                        AccountType: $scope.scopeModel.selectedCarrierAccountType.value,
                        FieldName: $scope.scopeModel.accountFieldName
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('cpWhsWhsaccountqueryinterceptor', VRRestAPIQueryInterceptorDirective);
})(app);