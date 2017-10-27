(function (app) {

    'use strict';

    VRRestAPIAnalyticQueryInterceptorDirective.$inject = ['UtilsService'];

    function VRRestAPIAnalyticQueryInterceptorDirective(UtilsService) {
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
            templateUrl: '/Client/Modules/PartnerPortal_CustomerAccess/Elements/VRRestAPIAnalyticQueryInterceptor/Directives/MainExtensions/Templates/RetailAccountVRRestAPIAnalyticQueryInterceptorTemplate.html'
        };

        function VRRestAPIAnalyticQueryInterceptorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    console.log(payload);

                    if (payload != undefined && payload.vrRestAPIAnalyticQueryInterceptor != undefined) {
                        $scope.scopeModel.accountFieldName = payload.vrRestAPIAnalyticQueryInterceptor.AccountFieldName;
                        $scope.scopeModel.withSubAccounts = payload.vrRestAPIAnalyticQueryInterceptor.WithSubAccounts;
                    }
                };

                api.getData = function () {

                    return {
                        $type: 'PartnerPortal.CustomerAccess.MainExtensions.VRRestAPIAnalyticQueryInterceptor.RetailAccountVRRestAPIAnalyticQueryInterceptor, PartnerPortal.CustomerAccess.MainExtensions',
                        AccountFieldName: $scope.scopeModel.accountFieldName,
                        WithSubAccounts: $scope.scopeModel.withSubAccounts
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('partnerportalCustomeraccessRetailaccountanalyticqueryinterceptor', VRRestAPIAnalyticQueryInterceptorDirective);

})(app);