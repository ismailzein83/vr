(function (app) {
    'use strict';
    BSCSInterceptor.$inject = ['UtilsService', 'VRUIUtilsService'];
    function BSCSInterceptor(UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var bscsInterceptor = new BSCSInterceptor(ctrl, $scope, $attrs);
                bscsInterceptor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/SOM_ST/Directives/HttpConnectionCallInterceptor/Templates/BSCSInterceptor.html"
        };

        function BSCSInterceptor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined && payload.Interceptor != undefined) {
                        $scope.scopeModel.userName = payload.Interceptor.UserName;
                        $scope.scopeModel.password = payload.Interceptor.Password;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "SOM.ST.Business.BSCSOnlineInterceptor, SOM.ST.Business",
                        UserName: $scope.scopeModel.userName,
                        Password: $scope.scopeModel.password,
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('somStHttpconnectioncallinterceptorBscs', BSCSInterceptor);
})(app);