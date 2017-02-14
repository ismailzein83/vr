'use strict';

restrict: 'E',
app.directive('retailTelesTelesrestconnectionEditor', [function () {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var editor = new TelesRestConnectionEditor($scope, ctrl, $attrs);
            editor.initializeController();
        },
        controllerAs: 'interAppRestCtrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_Teles/Directives/ConnectionTypes/Templates/TelesRestConnectionEditorTemplate.html'
    };

    function TelesRestConnectionEditor($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        function initializeController() {
            $scope.scopeModel = {};
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined && payload.data != undefined) {
                    $scope.scopeModel.url = payload.data.URL;
                    $scope.scopeModel.actionPrefix = payload.data.ActionPrefix;
                    $scope.scopeModel.token = payload.data.Token;
                    $scope.scopeModel.domainId = payload.data.DefaultDomainId;
                    $scope.scopeModel.authorization = payload.data.Authorization;
                }
            };

            api.getData = function () {

                return {
                    $type: 'Retail.Teles.Business.TelesRestConnection, Retail.Teles.Business',
                    URL: $scope.scopeModel.url,
                    ActionPrefix: $scope.scopeModel.actionPrefix,
                    Token: $scope.scopeModel.token,
                    DefaultDomainId: $scope.scopeModel.domainId,
                    Authorization: $scope.scopeModel.authorization
                };
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }
    }
}]);