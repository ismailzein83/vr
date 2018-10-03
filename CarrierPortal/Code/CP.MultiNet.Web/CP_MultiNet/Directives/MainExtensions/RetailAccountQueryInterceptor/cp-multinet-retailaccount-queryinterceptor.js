(function (app) {

    'use strict';

    retailAccountQueryInterceptor.$inject = ['UtilsService'];

    function retailAccountQueryInterceptor(UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RetailAccountQueryInterceptorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/CP_MultiNet/Directives/MainExtensions/RetailAccountQueryInterceptor/Templates/RetailAccountQueryInterceptorTemplate.html'
        };

        function RetailAccountQueryInterceptorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.settings != undefined) {
                        $scope.scopeModel.accountFieldName = payload.settings.AccountFieldName;
                        $scope.scopeModel.withSubAccounts = payload.settings.WithSubAccounts;
                    }
                };

                api.getData = function () {

                    return {
                        $type: 'CP.MultiNet.Business.RetailAccountQueryInterceptor, CP.MultiNet.Business',
                        AccountFieldName: $scope.scopeModel.accountFieldName,
                        WithSubAccounts: $scope.scopeModel.withSubAccounts
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('cpMultinetRetailaccountQueryinterceptor', retailAccountQueryInterceptor);

})(app);