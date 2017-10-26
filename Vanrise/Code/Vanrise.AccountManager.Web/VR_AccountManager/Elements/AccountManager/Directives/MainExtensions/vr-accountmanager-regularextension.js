'use strict';

app.directive('vrAccountmanagerRegularextension', ['UtilsService', 'VRUIUtilsService',
function (UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new AccountManagerRegularExtensionCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/VR_AccountManager/Elements/AccountManager/Directives/MainExtensions/Templates/RegularExtensionTemplate.html'
    };

    function AccountManagerRegularExtensionCtor(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;
        function initializeController() {
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function () {
              
            };
            api.getData = function () {
                var obj = {
                    $type: "Vanrise.AccountManager.MainExtensions.RegularAccountManagerDefinitionSetting,Vanrise.AccountManager.MainExtensions",
                };

                return obj;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

    }

    return directiveDefinitionObject;
}]);