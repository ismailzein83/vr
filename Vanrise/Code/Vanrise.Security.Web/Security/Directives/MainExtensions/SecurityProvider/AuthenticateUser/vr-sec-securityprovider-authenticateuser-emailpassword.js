'use strict';

app.directive('vrSecSecurityproviderAuthenticateuserEmailpassword', ['UtilsService',
    function (UtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new EmailPasswordCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Security/Directives/MainExtensions/SecurityProvider/AuthenticateUser/Templates/EmailPasswordTemplate.html"
        };

        function EmailPasswordCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var email;
            function initializeController() {
                $scope.editMode = false;
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        $scope.editMode = true;
                        email = $scope.scopeModel.email = payload.username;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.Security.Entities.EmailPasswordSecurityProviderAuthenticationPayload, Vanrise.Security.Entities',
                        Email: email == undefined ? $scope.scopeModel.email : email,
                        Password: $scope.scopeModel.password
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);