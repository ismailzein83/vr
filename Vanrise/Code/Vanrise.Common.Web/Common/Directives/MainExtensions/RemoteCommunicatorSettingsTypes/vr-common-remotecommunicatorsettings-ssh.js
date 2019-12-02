'use strict';

app.directive('vrCommonRemotecommunicatorsettingsSsh', ['UtilsService', 'VRUIUtilsService',
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
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/MainExtensions/RemoteCommunicatorSettingsTypes/Templates/SSHCommunicatorSettings.html"
        };

        function sshSettingCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.settings != undefined) {
                        var settings = payload.settings;
                        $scope.scopeModel.host = settings.Host;
                        $scope.scopeModel.port = settings.Port;
                        $scope.scopeModel.username = settings.Username;
                        $scope.scopeModel.password = settings.Password;
                        $scope.scopeModel.connectionTimeOutInSeconds = settings.ConnectionTimeOutInSeconds;
                        $scope.scopeModel.readTimeOutInSeconds = settings.ReadTimeOutInSeconds;
                    }
                };

                api.getData = function () {
                    return {
                        "$type": "Vanrise.Common.SSHRemoteCommunicatorSettings, Vanrise.Common",
                        Settings: {
                            Host: $scope.scopeModel.host,
                            Port: $scope.scopeModel.port,
                            Username: $scope.scopeModel.username,
                            Password: $scope.scopeModel.password,
                            ConnectionTimeOutInSeconds: $scope.scopeModel.connectionTimeOutInSeconds,
                            ReadTimeOutInSeconds: $scope.scopeModel.readTimeOutInSeconds
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);