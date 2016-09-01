'use strict';

app.directive('vrBebridgeSourcebereadersFilesourcereaderDirective', ['VRNotificationService',
    function (vrNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var fileSourceReader = new filepSourceReader($scope, ctrl, $attrs);
                fileSourceReader.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_BEBridge/Directives/MainExtensions/SourceBEReaders/Templates/BEReceiveDefinitionFileSourceReaderTemplate.html'
        };

        function filepSourceReader($scope, ctrl, $attrs) {
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
                    }
                };
                api.getData = function () {
                    var setting =
                    {
                        Extension: $scope.scopeModel.Extension,
                        Mask: $scope.scopeModel.Mask,
                        Directory: $scope.scopeModel.Directory
                    }
                    return {
                        $type: "Vanrise.BEBridge.MainExtensions.SourceBEReaders.FileSourceReader, Vanrise.BEBridge.MainExtensions",
                        Setting: setting
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
