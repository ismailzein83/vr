'use strict';

app.directive('vrCommonHttpconnectionApiconnectioninterceptorEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HttpConnectionAPIInterceptorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/MainExtensions/ConnectionTypes/Templates/HttpConnectionAPIConnectionInterceptorEditorTemplate.html"
        };

        function HttpConnectionAPIInterceptorCtor(ctrl, $scope, attrs) {
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
                        $scope.scopeModel.username = payload.Interceptor.Username;
                        $scope.scopeModel.password = payload.Interceptor.Password;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        $type: "Vanrise.Common.Business.VRAPIConnectionHttpConnectionCallInterceptor, Vanrise.Common.Business",
                        Username: $scope.scopeModel.username,
                        Password: $scope.scopeModel.password
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);