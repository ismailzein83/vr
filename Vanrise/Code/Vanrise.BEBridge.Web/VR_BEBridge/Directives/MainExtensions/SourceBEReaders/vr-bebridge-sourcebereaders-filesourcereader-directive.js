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
                        $scope.scopeModel.Extension = payload.Extension;
                        $scope.scopeModel.Mask = payload.Mask;
                        $scope.scopeModel.Directory = payload.Directory;
                    }
                };
                api.getData = function () {
                    return {
                        $type: "Vanrise.BEBridge.MainExtensions.SourceBEReaders.FileSourceReader, Vanrise.BEBridge.MainExtensions.SourceBEReaders",
                        Extension: $scope.scopeModel.Extension,
                        Mask: $scope.scopeModel.Mask,
                        Directory: $scope.scopeModel.Directory
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
