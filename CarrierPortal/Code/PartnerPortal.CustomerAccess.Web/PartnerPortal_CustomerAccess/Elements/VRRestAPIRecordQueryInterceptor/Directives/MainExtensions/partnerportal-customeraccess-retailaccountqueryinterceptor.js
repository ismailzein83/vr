(function (app) {

    'use strict';

    RestAPIRecordQueryInterceptorDirective.$inject = ['UtilsService'];

    function RestAPIRecordQueryInterceptorDirective(UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RestAPIRecordQueryInterceptorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/PartnerPortal_CustomerAccess/Elements/VRRestAPIRecordQueryInterceptor/Directives/MainExtensions/Templates/RetailAccountVRRestAPIRecordQueryInterceptorTemplate.html'
        };

        function RestAPIRecordQueryInterceptorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            $scope.scopeModel = {};

            function initializeController() {
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        if (payload.vrRestAPIRecordQueryInterceptor != undefined)
                        {
                            $scope.scopeModel.accountFieldName = payload.vrRestAPIRecordQueryInterceptor.AccountFieldName;
                            $scope.scopeModel.withSubAccounts = payload.vrRestAPIRecordQueryInterceptor.WithSubAccounts;
                        }
                    }
                };

                api.getData = function () {

                    return {
                        $type: 'PartnerPortal.CustomerAccess.MainExtensions.VRRestAPIRecordQueryInterceptor.RetailAccountVRRestAPIRecordQueryInterceptor, PartnerPortal.CustomerAccess.MainExtensions',
                        AccountFieldName: $scope.scopeModel.accountFieldName,
                        WithSubAccounts: $scope.scopeModel.withSubAccounts
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('partnerportalCustomeraccessRetailaccountqueryinterceptor', RestAPIRecordQueryInterceptorDirective);

})(app);