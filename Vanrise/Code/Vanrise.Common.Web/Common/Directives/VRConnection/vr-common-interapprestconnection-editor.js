'use strict';

        restrict: 'E',
app.directive('vrCommonInterapprestconnectionEditor', [function () {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var editor = new InterAppRestConnectionEditor($scope, ctrl, $attrs);
            accountTypeCompanyProfilePartDefinition.initializeController();
        },
        controllerAs: 'interAppRestCtrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Common/Directives/MainExtensions/ConnectionTypes/Templates/InterAppRestConnectionEditorTemplate.html'
    };

    function InterAppRestConnectionEditor($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        function initializeController() {
            $scope.scopeModel = {};
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined && payload.data !=undefined) {
                    $scope.scopeModel.baseURL = payload.data.BaseURL;
                    $scope.scopeModel.username = payload.data.Username;
                    $scope.scopeModel.password = payload.data.Password;
                }
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