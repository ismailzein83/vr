(function (app) {
    'use strict';
    BPMInterceptor.$inject = ['UtilsService', 'VRUIUtilsService'];
    function BPMInterceptor(UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var bpmInterceptor = new BPMInterceptor(ctrl, $scope, $attrs);
                bpmInterceptor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/SOM_ST/Directives/HttpConnectionCallInterceptor/Templates/BPMInterceptor.html"
        };

        function BPMInterceptor(ctrl, $scope, $attrs) {
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
                        $scope.scopeModel.authenticationServiceURI = payload.Interceptor.AuthenticationServiceURI;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "SOM.ST.Business.BPMOnlineInterceptor, SOM.ST.Business",
                        UserName: $scope.scopeModel.userName,
                        Password: $scope.scopeModel.password,
                        AuthenticationServiceURI: $scope.scopeModel.authenticationServiceURI
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('somStHttpconnectioncallinterceptorBpm', BPMInterceptor);
})(app);