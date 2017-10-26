'use strict';

app.directive('vrAccountmanagerRuntime', ['UtilsService', 'VRUIUtilsService',
function (UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new AccountManagerRuntimeCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/VR_AccountManager/Elements/AccountManager/Directives/Template/RuntimeTemplate.html'
    };

    function AccountManagerRuntimeCtor(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;
        function initializeController() {
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function () {
                var promises = []
                UtilsService.waitMultiplePromises(promises);

            };
            api.getData = function () {
                var obj = {
                    $type: "Vanrise.AccountManager.MainExtensions.RegularAccountManagerSettings,Vanrise.AccountManager.MainExtensions",
                };

                return obj;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

    }

    return directiveDefinitionObject;
}]);