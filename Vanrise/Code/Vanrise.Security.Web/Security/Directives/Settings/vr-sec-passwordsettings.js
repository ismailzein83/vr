'use strict';

app.directive('vrSecPasswordsettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var passwordSettings = new PasswordSettings(ctrl, $scope, $attrs);
                passwordSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Security/Directives/Settings/Templates/PasswordSettingsTemplate.html"
        };

        function PasswordSettings(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var passwordComplexityAPI;
            var passwordComplexityReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            $scope.scopeModel = {};

            $scope.scopeModel.onPasswordComplexitySelectorReady = function (api) {
                passwordComplexityAPI = api;
                passwordComplexityReadyPromiseDeferred.resolve();
            };


            function initializeController() {
                passwordComplexityReadyPromiseDeferred.promise.then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined)
                        $scope.scopeModel.passwordLength = payload.PasswordLength; 

                    var passwordComplexityLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    var passwordComplexityPayload = {
                        selectedIds: payload != undefined && payload.PasswordComplexity || undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(passwordComplexityAPI, passwordComplexityPayload, passwordComplexityLoadPromiseDeferred);
                    return passwordComplexityLoadPromiseDeferred.promise;

                };

                api.getData = function () {
                    return {
                        PasswordLength:$scope.scopeModel.passwordLength,
                        PasswordComplexity: passwordComplexityAPI.getSelectedIds()
                    };
                };
                
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);