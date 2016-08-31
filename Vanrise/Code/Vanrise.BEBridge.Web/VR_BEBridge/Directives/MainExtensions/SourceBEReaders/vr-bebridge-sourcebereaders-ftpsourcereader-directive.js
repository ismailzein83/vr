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
                        $scope.scopeModel.Extension = payload.Extension;
                        $scope.scopeModel.Mask = payload.Mask;
                        $scope.scopeModel.Directory = payload.Directory;
                        $scope.scopeModel.ServerIp = payload.ServerIp;
                        $scope.scopeModel.UserName = payload.UserName;
                        $scope.scopeModel.Password = payload.Password;
                    }
                };
                api.getData = function () {
                    return {
                        $type: "Vanrise.BEBridge.MainExtensions.FTPSourceReader, Vanrise.BEBridge.MainExtensions",
                        Extension: $scope.scopeModel.Extension,
                        Mask: $scope.scopeModel.Mask,
                        Directory: $scope.scopeModel.Directory,
                        ServerIp: $scope.scopeModel.ServerIp,
                        UserName: $scope.scopeModel.UserName,
                        Password: $scope.scopeModel.Password
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
