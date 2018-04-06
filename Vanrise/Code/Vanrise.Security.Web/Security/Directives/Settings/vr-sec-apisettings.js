'use strict';

app.directive('vrSecApisettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var apiSettings = new APISettings(ctrl, $scope, $attrs);
                apiSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Security/Directives/Settings/Templates/APISettingsTemplate.html"
        };

        function APISettings(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;          
           
            $scope.scopeModel = {};
            function initializeController() {              
                    defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.exactExceptionMessage = payload.ExactExceptionMessage;
                    }
                };

                api.getData = function () {
                    return {
                        ExactExceptionMessage: $scope.scopeModel.exactExceptionMessage
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
}]);