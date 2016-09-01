'use strict';

app.directive('vrBebridgeSourcebereadersFtpsourcereaderDirective', ['VRNotificationService',
    function (vrNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ftpSource = new ftpSourceReader($scope, ctrl, $attrs);
                ftpSource.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_BEBridge/Directives/MainExtensions/SourceBEReaders/Templates/BEReceiveDefinitionFTPSourceReaderTemplate.html'
        };

        function ftpSourceReader($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.Extension = payload.Setting.Extension;
                        $scope.scopeModel.Mask = payload.Setting.Mask;
                        $scope.scopeModel.Directory = payload.Setting.Directory;
                        $scope.scopeModel.ServerIP = payload.Setting.ServerIP;
                        $scope.scopeModel.UserName = payload.Setting.UserName;
                        $scope.scopeModel.Password = payload.Setting.Password;
                    }
                };
                api.getData = function () {
                    var setting =
                    {
                        Extension: $scope.scopeModel.Extension,
                        Mask: $scope.scopeModel.Mask,
                        Directory: $scope.scopeModel.Directory,
                        ServerIP: $scope.scopeModel.ServerIP,
                        UserName: $scope.scopeModel.UserName,
                        Password: $scope.scopeModel.Password
                    }
                    return {
                        $type: "Vanrise.BEBridge.MainExtensions.SourceBEReaders.FTPSourceReader,  Vanrise.BEBridge.MainExtensions",
                        Setting: setting
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
