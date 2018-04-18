'use strict';

app.directive('vrCommonCommunicatorsettingsSsh', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new sshSettingCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'sshCtrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/CommunicatorSettings/Templates/SSHCommunicatorSettings.html"
        };

        function sshSettingCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.sshCommunicatorSettings != undefined) {
                        $scope.scopeModel.host = payload.ftpCommunicatorSettings.Host;
                        $scope.scopeModel.port = payload.ftpCommunicatorSettings.Port;
                        $scope.scopeModel.username = payload.ftpCommunicatorSettings.UserName;
                        $scope.scopeModel.password = payload.ftpCommunicatorSettings.Password;
                        $scope.scopeModel.connectionTimeOutInSeconds = payload.ftpCommunicatorSettings.ConnectionTimeOutInSeconds;
                        $scope.scopeModel.readTimeOutInSeconds = payload.ftpCommunicatorSettings.ReadTimeOutInSeconds;
                    }
                };

                api.getData = function () {
                    return {
                        Host: $scope.scopeModel.host,
                        Port: $scope.scopeModel.port,
                        UserName: $scope.scopeModel.username,
                        Password: $scope.scopeModel.password,
                        ConnectionTimeOutInSeconds: $scope.scopeModel.connectionTimeOutInSeconds,
                        ReadTimeOutInSeconds: $scope.scopeModel.readTimeOutInSeconds
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);