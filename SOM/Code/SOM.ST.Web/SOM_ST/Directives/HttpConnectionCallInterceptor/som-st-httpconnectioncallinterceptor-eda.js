(function (app) {
    'use strict';
    EDAInterceptor.$inject = ['UtilsService', 'VRUIUtilsService'];
    function EDAInterceptor(UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var edaInterceptor = new EDAInterceptor(ctrl, $scope, $attrs);
                edaInterceptor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/SOM_ST/Directives/HttpConnectionCallInterceptor/Templates/EDAInterceptor.html"
        };

        function EDAInterceptor(ctrl, $scope, $attrs) {
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
                        $scope.scopeModel.sessionId = payload.Interceptor.SessionId;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "SOM.ST.Business.EDAOnlineInterceptor, SOM.ST.Business",
                        SessionId: $scope.scopeModel.sessionId,
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('somStHttpconnectioncallinterceptorEda', EDAInterceptor);
})(app);