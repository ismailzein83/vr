'use strict';

restrict: 'E',
app.directive('vrCommonPop3connectionEditor', [function () {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var editor = new Pop3ConnectionEditor($scope, ctrl, $attrs);
            editor.initializeController();
        },
        controllerAs: 'pop3Ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Common/Directives/MainExtensions/ConnectionTypes/Templates/POP3ConnectionEditorTemplate.html'
    };

    function Pop3ConnectionEditor($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        function initializeController() {
            $scope.scopeModel = {};
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != null && payload.data!=null) {
                    $scope.scopeModel.userName = payload.data.UserName;
                    $scope.scopeModel.password = payload.data.Password;
                    $scope.scopeModel.server = payload.data.Server;
                    $scope.scopeModel.port = payload.data.Port;
                    $scope.scopeModel.ssl = payload.data.SSL;
                }
            };

            api.getData = function () {

                return {
                    $type: 'Vanrise.Common.Business.VRPop3Connection, Vanrise.Common.Business',
                    UserName: $scope.scopeModel.userName,
                    Password: $scope.scopeModel.password,
                    Server: $scope.scopeModel.server,
                    Port: $scope.scopeModel.port,
                    SSL: $scope.scopeModel.ssl,
                };
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }
    }
}]);