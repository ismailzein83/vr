(function (app) {

    'use strict';

    UserObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function UserObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var userObjectTypeSecurity = new UserObjectTypeSecurity($scope, ctrl, $attrs);
                userObjectTypeSecurity.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Security/Directives/MainExtensions/VRObjectTypes/Templates/UserObjectTypeTemplate.html"

        };
        function UserObjectTypeSecurity($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Security.MainExtensions.VRObjectTypes.UserObjectType, Vanrise.Security.MainExtensions",
                    }
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrSecUserobjecttype', UserObjectType);

})(app);