'use strict';

app.directive('vrCommonSessionlockSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/Settings/SessionLockSettings/Templates/SessionLockSettings.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                $scope.scopeModel = {};

                $scope.validateHeartbeatIntervalInSeconds = function () {
                    var heartbeat = parseInt($scope.scopeModel.heartbeatIntervalInSeconds);
                    var timeOut = parseInt($scope.scopeModel.timeOutInSeconds);
                    if (heartbeat < timeOut)
                        return null;
                    else return "Heartbeat interval should be less than Time Out.";
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.data != undefined) {
                        $scope.scopeModel.timeOutInSeconds = payload.data.TimeOutInSeconds;
                        $scope.scopeModel.heartbeatIntervalInSeconds = payload.data.HeartbeatIntervalInSeconds;
                    }

                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Entities.SessionLockSettings, Vanrise.Entities",
                        TimeOutInSeconds: $scope.scopeModel.timeOutInSeconds,
                        HeartbeatIntervalInSeconds: $scope.scopeModel.heartbeatIntervalInSeconds,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);