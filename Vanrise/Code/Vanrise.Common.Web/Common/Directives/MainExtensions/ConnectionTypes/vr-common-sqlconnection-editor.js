'use strict';

        restrict: 'E',
app.directive('vrCommonSqlconnectionEditor', [function () {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var editor = new SQLConnectionEditor($scope, ctrl, $attrs);
            editor.initializeController();
        },
        controllerAs: 'sqlCtrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Common/Directives/MainExtensions/ConnectionTypes/Templates/SQLConnectionEditorTemplate.html'
    };

    function SQLConnectionEditor($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        function initializeController() {
            $scope.scopeModel = {};
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {             
                $scope.scopeModel.connectionString = payload != undefined && payload.data != undefined && payload.data.ConnectionString || undefined;
            };

            api.getData = function () {
                
                return {
                    $type: 'Vanrise.Common.Business.VRInterAppRestConnection, Vanrise.Common.Business',
                    BaseURL: $scope.scopeModel.baseURL,
                    Username: $scope.scopeModel.username,
                    Password: $scope.scopeModel.password

                };
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }
    }
}]);